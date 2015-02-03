using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoMapper;
using Agile.Common.Components;
using Agile.Common.Configurations;
using System.Reflection;
using Agile.Common.Data;

namespace Agile.Framework.Data
{
    public static class ConfigExtensions
    {
        public static Configuration UseDataService(this Configuration configuration, Type dbContextType, string nameOrConnectionString)
        {
			var svc = new EfDataService() { DbContextType = dbContextType, NameOrConnectionString = nameOrConnectionString };
			configuration.SetDefault<IModelService, EfDataService>(svc);
            return configuration;
        }

        
        static void FillDefaultCreate(object sender, DataServiceEventArgs args)
        {
            var items = args.Items;
            foreach (var item in items)
            {
                if (item.CreatedAt == null)
                {
                    item.CreatedAt = DateTime.Now;
                }
                
            }
        }
        static void FillDefaultUpdate(object sender, DataServiceEventArgs args)
        {
            var items = args.Items;
            foreach (var item in items)
            {
                if (item.LastModifiedAt == null)
                {
                    item.LastModifiedAt = DateTime.Now;
                }

            }
        }
        

        public static Configuration UseDataServiceDefaultFiller(this Configuration configuration)
        {
            var svc = ObjectContainer.Resolve<IModelService>() as DataService;
            if (svc != null)
            {
                svc.BeforeUpdate += FillDefaultUpdate;
                svc.BeforeCreate += FillDefaultCreate;
            }
            return configuration;
        }
    }
}
