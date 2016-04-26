using System;
using Agile.Common;
using Agile.Common.Components;

namespace Agile.Framework.Data
{
    public static class ConfigExtensions
    {
        public static WireUp UseDataService(this WireUp configuration, Type dbContextType, string nameOrConnectionString)
        {
			var svc = new EfDataService() { DbContextType = dbContextType, NameOrConnectionString = nameOrConnectionString };
//			configuration.SetDefault<IModelService, EfDataService>(svc);
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
        

        public static WireUp UseDataServiceDefaultFiller(this WireUp configuration)
        {
            var svc = ObjectContainer.Resolve<IModelService>() as EfDataService;
            if (svc != null)
            {
                svc.BeforeUpdate += FillDefaultUpdate;
                svc.BeforeCreate += FillDefaultCreate;
            }
            return configuration;
        }
    }
}
