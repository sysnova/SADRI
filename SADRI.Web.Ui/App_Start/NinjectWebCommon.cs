using SADRI.Infrastructure.DependencyResolution;

[assembly: WebActivatorEx.PreApplicationStartMethod(typeof(SADRI.Web.Ui.App_Start.NinjectWebCommon), "Start")]
[assembly: WebActivatorEx.ApplicationShutdownMethodAttribute(typeof(SADRI.Web.Ui.App_Start.NinjectWebCommon), "Stop")]

namespace SADRI.Web.Ui.App_Start
{
    using System;
    using System.Web;

    using Microsoft.Web.Infrastructure.DynamicModuleHelper;

    using Ninject;
    using Ninject.Web.Mvc;
    using Ninject.Modules;
    using Ninject.Web.Common;

    using SADRI.Services.Interfaces;
    using SADRI.Web.Ui.Services;
    using System.Collections.Generic;

    //
    using Microsoft.Owin.Security;
    using NHibernate.AspNet.Identity;
    using Microsoft.AspNet.Identity;
    using SADRI.Web.Ui.ViewModels;
    using SharpArch.NHibernate;
    using NHibernate;
    //

    public static class NinjectWebCommon 
    {
        private static readonly Bootstrapper bootstrapper = new Bootstrapper();

        /// <summary>
        /// Starts the application
        /// </summary>
        public static void Start() 
        {
            DynamicModuleUtility.RegisterModule(typeof(OnePerRequestHttpModule));
            DynamicModuleUtility.RegisterModule(typeof(NinjectHttpModule));
            bootstrapper.Initialize(CreateKernel);
        }
        
        /// <summary>
        /// Stops the application.
        /// </summary>
        public static void Stop()
        {
            bootstrapper.ShutDown();
        }
        
        /// <summary>
        /// Creates the kernel that will manage your application.
        /// </summary>
        /// <returns>The created kernel.</returns>
        private static IKernel CreateKernel()
        {
            var kernel = new StandardKernel();
            try
            {
                kernel.Bind<Func<IKernel>>().ToMethod(ctx => () => new Bootstrapper().Kernel);
                kernel.Bind<IHttpModule>().To<HttpApplicationInitializationHttpModule>();
                //REGISTER USER
                kernel.Bind<ISession>().ToMethod(x => NHibernateSession.Current);
                kernel.Bind(typeof(IUserStore<ApplicationUser>)).To(typeof(UserStore<ApplicationUser>)).InRequestScope();
                //
                //ADD ROLE
                kernel.Bind(typeof(IRoleStore<IdentityRole>)).To(typeof(RoleStore<IdentityRole>)).InRequestScope();
                //
                RegisterServices(kernel);
                return kernel;
            }
            catch
            {
                kernel.Dispose();
                throw;
            }
        }

        /// <summary>
        /// Load your modules or register your services here!
        /// </summary>
        /// <param name="kernel">The kernel.</param>
        private static void RegisterServices(IKernel kernel)
        {
            // Bind local services
            kernel.Bind<IProductService>().To<ProductService>();

            // Add data and infrastructure modules
            var modules = new List<INinjectModule>
                {
                    new RepositoryModule(),
                    new LoggingModule()
                };
            kernel.Load(modules);
        }        
    }
}
