using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Ninject;
using Ninject.Modules;

using SADRI.Domain.Interfaces;
using SADRI.Infrastructure.Data;
using SADRI.Infrastructure.Interfaces;

namespace SADRI.Infrastructure.DependencyResolution
{
    public class RepositoryModule : NinjectModule
    {
        public override void Load()
        {
            // Get config service

            //var configService = Kernel.Get<IConfigService>();

            // Bind repositories

            //Bind<ICategoryRepository>().To<CategoryRepository>()
            //    .WithConstructorArgument("connectionString", configService.NorthwindConnection);
            //Bind<IProductRepository>().To<ProductRepository>()
            //    .WithConstructorArgument("connectionString", configService.NorthwindConnection);

            Bind(typeof(IRepository<>)).To(typeof(NHibernateRepository<>));

            Bind<IProductRepository>().To<ProductRepository>();


        }
    }
}