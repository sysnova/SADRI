using System.Collections.Generic;
using SADRI.Domain.Entities;
using SADRI.Domain.Interfaces;
using NHibernate.Criterion;

namespace SADRI.Infrastructure.Data
{
    public class ProductRepository : NHibernateRepository<Product>, IProductRepository
    {
        public IList<Product> GetProductByCategoryId(int categoryId)
        {
            using (var transaction = Session.BeginTransaction())
            {
                IList<Product> returnVal = Session.CreateCriteria<Product>()
                    .Add(Restrictions.Eq("Category.CategoryId", categoryId))
                    .List<Product>();
                transaction.Commit();
                return returnVal;
            }
        }
    }
}