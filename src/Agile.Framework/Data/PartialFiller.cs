using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using AutoMapper;
using Agile.Common;
using Agile.Common.Components;
using Agile.Common.Data;
using Agile.Common.Utilities;

namespace Agile.Framework.Data
{
    [Component(LifeStyle.Singleton)]
    public class PartialFiller : IAssemblyInitializer
    {
        /// <summary>
        /// 提供类似部分更新的功能。
        /// 更新/删除时常常需要填充模型，注意必须提供ID值
        /// </summary>
        /// <param name="model">被填充的模型</param>
        /// <returns>当前数据库中的对象拷贝</returns>
        public static TModel Fill<TModel, TQuery>(TModel model)
            where TModel: BaseEntity
            where TQuery:BaseEntityQuery<TModel>,new()
        {
            if (model == null || model.Id == null) return model;
            var query = new TQuery() {Id = model.Id};
            var svc = ObjectContainer.Resolve<IModelService>();
            var current = svc.FirstOrDefault(query);
            AutoMapper.Mapper.Map(current, model);
            return current;
        }
        /// <summary>
        /// 提供类似部分更新的功能。
        /// 更新/删除时常常需要填充模型，注意必须提供ID值
        /// </summary>
        /// <param name="models">被填充的模型</param>
        /// <returns>当前数据库中的对象拷贝</returns>
        public static TModel[]  Fill<TModel, TQuery>(TModel[] models)
            where TModel :  BaseEntity
            where TQuery : BaseEntityQuery<TModel>, new()
        {
            if (models == null || !models.Any()) return new TModel[0];
            var query = new TQuery() {IdList = models.Select(o => o.Id).OfType<int>().ToArray()};
            var svc = ObjectContainer.Resolve<IModelService>();
            var origins = svc.Select(query);
            foreach (var model in models)
            {
                var origin = origins.FirstOrDefault(o => o.Id == model.Id);
                AutoMapper.Mapper.Map(origin, model);
            }
            return origins.ToArray();
        }

        void CreateMap<TModel>()
        {
            Mapper.Configuration.AllowNullCollections = true;
            Mapper.CreateMap<TModel, TModel>().ForAllMembers(config =>
                config.Condition(context =>
                    context.DestinationValue == null
                )
            );
        }
        public void Initialize(Assembly[] assemblies)
        {
            foreach (var assembly in assemblies)
            {
                try
                {
                    foreach (var type in assembly.GetTypes().Where(TypeUtils.IsBaseEntity))
                    {
                        var method =
                            new Action(this.CreateMap<BaseEntity>).Method.GetGenericMethodDefinition()
                                                                     .MakeGenericMethod(type);
                        method.Invoke(this, null);

                    }
                }
                catch
                {
                }
            }
        }
    }
}
