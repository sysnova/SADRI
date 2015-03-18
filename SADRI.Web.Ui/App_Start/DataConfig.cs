using System;
using System.Collections.Generic;
using System.Linq;
//
using System.IO;
using NHibernate.AspNet.Identity;
using NHibernate.AspNet.Identity.Helpers;
//
//using NHibernate.AspNet.Web.Models;
using SADRI.Web.Ui.ViewModels;
//
using NHibernate.Cfg;
using NHibernate.Mapping.ByCode;
using NHibernate.Tool.hbm2ddl;
using SharpArch.Domain.DomainModel;
using SharpArch.NHibernate;
//

namespace SADRI.Web.Ui
{
    public class DataConfig
    {
        public static void Configure(ISessionStorage storage)
        {
            var internalTypes = new[] {
                typeof(ApplicationUser)
            };

            var mapping = MappingHelper.GetIdentityMappings(internalTypes);
            System.Diagnostics.Debug.WriteLine("LEO:///////////////////////// - " + mapping.AsString());

            var configuration = NHibernateSession.Init(storage, mapping);
            //BuildSchema(configuration);
        }

        private static void DefineBaseClass(ConventionModelMapper mapper, System.Type[] baseEntityToIgnore)
        {
            if (baseEntityToIgnore == null) return;
            mapper.IsEntity((type, declared) =>
                baseEntityToIgnore.Any(x => x.IsAssignableFrom(type)) &&
                !baseEntityToIgnore.Any(x => x == type) &&
                !type.IsInterface);
            mapper.IsRootEntity((type, declared) => baseEntityToIgnore.Any(x => x == type.BaseType));
        }

        private static void BuildSchema(Configuration config)
        {
            var path = Path.Combine(AppDomain.CurrentDomain.GetData("DataDirectory").ToString(), @"schema.sql");

            // this NHibernate tool takes a configuration (with mapping info in)
            // and exports a database schema from it
            new SchemaExport(config)
                .SetOutputFile(path)
                .Create(true, true /* DROP AND CREATE SCHEMA */);
        }

    }
}