using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Agile.Common.Data;
using Agile.Common.Exceptions;

namespace Agile.Framework.Data
{
    public static class Extensions
    {
		public static TModel Single<TModel>(this IModelService svc, BaseEntityQuery<TModel> query)
			where TModel : BaseEntity
        {
            var models = svc.Select(query);
            if (models == null || !models.Any()) return null;
            if (models.Count() == 1) return models.FirstOrDefault();
            throw new RuleViolatedException("Sequence contains more than one element.");
        }

		public static TModel FirstOrDefault<TModel>(this IModelService svc, BaseEntityQuery<TModel> query)
			where TModel :  BaseEntity
        {
            query.Take = 1;
            var models = svc.Select(query);
            return models.FirstOrDefault();
        }

        public static TModel FindByID<TModel, TQuery>(this IModelService svc, int ID)
			where TModel :  BaseEntity
			where TQuery :  BaseEntityQuery<TModel>, new()
        {
            var query = new TQuery() {Id = ID};
            var models = svc.Select(query);
            return models.FirstOrDefault();
        }

		public static int GetCount<TModel>(this IModelService svc, BaseEntityQuery<TModel> query) where TModel :  BaseEntity
        {
            query.Take = 0;
            query.Skip = 0;
            svc.Select(query);
            return query.CountOfResultSet ?? 0;
        }

		public static bool Any<TModel>(this IModelService svc, BaseEntityQuery<TModel> query) where TModel :  BaseEntity
        {
            return svc.GetCount(query) > 0;
        }

        public static void Patch<TModel, TQuery>(this IModelService svc, params TModel[] models)
			where TModel :  BaseEntity, new()
			where TQuery : BaseEntityQuery<TModel>, new()
        {
            PartialFiller.Fill<TModel, TQuery>(models);
            svc.Update(models);
        }
    }
}
