using System.Collections.Generic;
using SADRI.Domain.Entities;

namespace SADRI.Domain.Interfaces
{
    public interface IProductRepository : IRepository<Product>
    {
        IList<Product> GetProductByCategoryId(int idCategory);

    }
}
