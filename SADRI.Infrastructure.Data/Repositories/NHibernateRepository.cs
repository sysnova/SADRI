using System.Collections.Generic;
using NHibernate;
using SADRI.Domain.Interfaces;
using NHibernate.Linq;

namespace SADRI.Infrastructure.Data
{
    public class NHibernateRepository<T> : IRepository<T> where T : class
    {
        protected ISession Session
        {
            get { return SessionManager.Instance.Session; }
        }

        public NHibernateRepository()
        {

        }

        /*
        public T Get(object id)
        {
            using (var session = DBSessionManager.OpenSession())
            using (var transaction = session.BeginTransaction())
            {
                T returnVal = session.Get<T>(id);
                transaction.Commit();
                return returnVal;
            }
        }

        public void Save(T value)
        {
            using (var session = DBSessionManager.OpenSession())
            using (var transaction = session.BeginTransaction())
            {
                session.Save(value);
                transaction.Commit();
            }
        }

        public void Update(T value)
        {
            using (var session = DBSessionManager.OpenSession())
            using (var transaction = session.BeginTransaction())
            {
                session.Update(value);
                transaction.Commit();
            }
        }

        public void Delete(T value)
        {
            using (var session = DBSessionManager.OpenSession())
            using (var transaction = session.BeginTransaction())
            {
                session.Delete(value);
                transaction.Commit();
            }
        }
        */
        public IList<T> GetAll()
        {
            using (var transaction = Session.BeginTransaction())
            {
                IList<T> returnVal = Session.CreateCriteria(typeof(T)).List<T>();

                transaction.Commit();

                return returnVal;
            }

        }

    }
}