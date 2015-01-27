using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using FluentNHibernate.Mapping;
using SADRI.Domain.Entities;

namespace SADRI.Infrastructure.Data
{
    public class CategoryMap : ClassMap<Category>
    {
        public CategoryMap()
        {
            Id(x => x.CategoryId);
            Map(x => x.CategoryName);
            HasMany(x => x.Products);
            Table("Categories");
        }
    }
}