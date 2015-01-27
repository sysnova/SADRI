using System;
using System.Web;
using NHibernate;
using NHibernate.Cfg;
using NHibernate.Context;
using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;

namespace SADRI.Infrastructure.Data
{
    public sealed class SessionManager
    {
        [ThreadStatic]
        private readonly NHibernate.Cfg.Configuration _configuration;
        private readonly ISessionFactory _sessionFactory;

        #region Constructor

        #region Singleton
        public static SessionManager Instance
        {
            get
            {
                return Singleton.Instance;
            }
        }

        private class Singleton
        {
            static Singleton() { }
            internal static readonly SessionManager Instance = new SessionManager();
        }
        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="SessionManager"/> class.
        /// </summary>
        private SessionManager()
        {
            _configuration = new NHibernate.Cfg.Configuration();
            _configuration.Configure();
            _sessionFactory = Fluently.Configure(_configuration)
                 .Mappings(m => m.FluentMappings.AddFromAssemblyOf<ProductMap>())
                 .BuildSessionFactory();
        }
        #endregion

        #region NHibernate Setup
        /// <summary>
        /// Gets the <see cref="NHibernate.ISessionFactory"/>
        /// </summary>
        internal ISessionFactory SessionFactory { get { return _sessionFactory; } }

        /// <summary>
        /// Closes the session factory.
        /// </summary>
        public void Close()
        {
            SessionFactory.Close();
        }

        #endregion

        #region NHibernate SessionContext (1.2+)

        /// <summary>
        /// Opens the conversation (if already existing, reuses it). Call this from Application_BeginPreRequestHandlerExecute
        /// </summary>
        /// <returns>A <see cref="NHibernate.ISession"/></returns>
        public ISession OpenConversation()
        {
            //you must set <property name="current_session_context_class">web</property> (or thread_static etc)
            ISession session = Session; //get the current session (or open one). We do this manually, not using SessionFactory.GetCurrentSession()
            //for session per conversation (otherwise remove)
            session.FlushMode = FlushMode.Never; //Only save on session.Flush() - because we need to commit on unbind in PauseConversation
            session.BeginTransaction(); //start a transaction
            CurrentSessionContext.Bind(session); //bind it
            return session;
        }

        /// <summary>
        /// Ends the conversation. If an exception occurs, rethrows it ensuring session is closed. Call this (or <see cref="PauseConversation"/> if session per conversation) from Application_PostRequestHandlerExecute
        /// </summary>
        public void EndConversation()
        {
            ISession session = CurrentSessionContext.Unbind(SessionFactory);
            if (session == null) return;
            try
            {
                session.Flush();
                session.Transaction.Commit();
            }
            catch (Exception)
            {
                session.Transaction.Rollback();
                throw;
            }
            finally
            {
                session.Close();
            }
        }

        /// <summary>
        /// Pauses the conversation. Call this (or <see cref="EndConversation"/>) from Application_EndRequest
        /// </summary>
        public void PauseConversation()
        {
            ISession session = CurrentSessionContext.Unbind(SessionFactory);
            if (session == null) return;
            try
            {
                session.Transaction.Commit(); //with flushMode=Never, this closes connections but doesn't flush
            }
            catch (Exception)
            {
                session.Transaction.Rollback();
                throw;
            }
            //we don't close the session, and it's still in Asp SessionState
        }
        #endregion

        #region NHibernate Sessions

        /// <summary>
        /// Explicitly open a session. If you have an open session, close it first.
        /// </summary>
        /// <returns>The <see cref="NHibernate.ISession"/></returns>
        public ISession OpenSession()
        {
            ISession session = SessionFactory.OpenSession();
            return session;
        }
        /// <summary>
        /// Gets the current <see cref="NHibernate.ISession"/>. Although this is a singleton, this is specific to the thread/ asp session. If you want to handle multiple sessions, use <see cref="OpenSession"/> directly. If a session it not open, a new open session is created and returned.
        /// </summary>
        /// <value>The <see cref="NHibernate.ISession"/></value>
        public ISession Session
        {
            get
            {
                ISession session = SessionFactory.GetCurrentSession();
                if (session != null && session.IsOpen)
                    return session;
                return OpenSession();
            }
        }
        #endregion
    }

}
