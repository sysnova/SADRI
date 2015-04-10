using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Ninject;
using Ninject.Modules;

using SADRI.Infrastructure.Interfaces;
using SADRI.Infrastructure.Workflow;
using SADRI.Domain.Entities.Enums;
//
using Ninject.Extensions.Factory;//Extension para crear Factories.
//

namespace SADRI.Infrastructure.DependencyResolution
{
    public class WorkflowModule : NinjectModule
    {
        public override void Load()
        {
            //Bind<ICategoryRepository>().To<CategoryRepository>()
            //    .WithConstructorArgument("connectionString", configService.NorthwindConnection);
            //Bind<IProductRepository>().To<ProductRepository>()
            //    .WithConstructorArgument("connectionString", configService.NorthwindConnection);
            //Bind(typeof(IRepository<>)).To(typeof(NHibernateRepository<>));
            
            //Bind<IWorkflowWizardUser>().To<WizardUser>();
               //.WithConstructorArgument("_state", States.UserWizard.Step1);
            
            Bind<IWorkflowWizardGenericFactory>().ToFactory();
            //Bind<IWorkflowWizardUser>().To<WizardUser>();
            Bind<IWorkflowWizardGeneric>().To<WizardUser>();
 
        }
    }
}
