using System;
using System.Web;
using NHibernate;
using NHibernate.Context;


namespace SADRI.Infrastructure.Data
{
    public class NHibernateModule : IHttpModule
    {
            #region Implementation of IHttpModule

    /// <summary>
    /// Initializes a module and prepares it to handle requests.
    /// </summary>
    /// <param name="context">An <see cref="T:System.Web.HttpApplication"/> that provides access to the methods, properties, and events common to all application objects within an ASP.NET application
    /// </param>
    public void Init(HttpApplication context)
    {
        context.BeginRequest += context_BeginRequest;
        context.EndRequest += context_EndRequest;
    }

    private static void context_BeginRequest(object sender, EventArgs e)
    {
        //use my session manager
        ISession session = SessionManager.Instance.OpenSession();
        CurrentSessionContext.Bind(session);
    }

    private static void context_EndRequest(object sender, EventArgs e)
    {
        ISessionFactory sessionFactory = SessionManager.Instance.SessionFactory;
        ISession session = CurrentSessionContext.Unbind(sessionFactory);
       
        session.Close();
    }

    /// <summary>
    /// Disposes of the resources (other than memory) used by the module that implements <see cref="T:System.Web.IHttpModule"/>.
    /// </summary>
    public void Dispose()
    {
    }

    #endregion
  }

}

