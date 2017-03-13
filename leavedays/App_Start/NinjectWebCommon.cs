[assembly: WebActivatorEx.PreApplicationStartMethod(typeof(leavedays.App_Start.NinjectWebCommon), "Start")]
[assembly: WebActivatorEx.ApplicationShutdownMethod(typeof(leavedays.App_Start.NinjectWebCommon), "Stop")]

namespace leavedays.App_Start
{
    using System;
    using System.Web;

    using Microsoft.Web.Infrastructure.DynamicModuleHelper;

    using Ninject;
    using Ninject.Web.Common;
    using NHibernate;
    using NHibernate.Cfg;
    using NHibernate.Tool.hbm2ddl;
    using Models;
    using Services;
    using Models.Interfaces.Repository;
    using NewsWebSite.Models.Repository;
    using Models.Repository.Interfaces;
    using Models.Repository;

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
            kernel.Bind<CompanyService>().To<CompanyService>();
            kernel.Bind<IUserRepository>().To<UserRepository>();
            kernel.Bind<ICompanyRepository>().To<CompanyRepository>();
            kernel.Bind<IRequestRepository>().To<RequestRepository>();

            kernel.Bind<ISessionFactory>().ToMethod(context =>
            {
                var configuration = new Configuration();

                configuration.Configure();
                configuration.AddAssembly(typeof(User).Assembly);
                ISessionFactory sessionFactory = configuration.BuildSessionFactory();
                new SchemaUpdate(configuration).Execute(true, true);

                return sessionFactory;
            }).InSingletonScope();

           

        }        
    }
}
