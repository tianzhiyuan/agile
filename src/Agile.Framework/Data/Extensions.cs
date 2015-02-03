using System.Linq;
using Agile.Common.Data;

namespace Agile.Framework.Data
{
    public static class Extensions
    {
		public static TModel Single<TModel>(this IModelService svc, BaseEntityQuery<TModel> query)
			where TModel : BaseEntity
        {
			query.Mode = QueryMode.ResultSetOnly;
			query.Take = 1;
            var models = svc.Select(query);
			return models.SingleOrDefault();
        }

		public static TModel FirstOrDefault<TModel>(this IModelService svc, BaseEntityQuery<TModel> query)
			where TModel :  BaseEntity
        {
			query.Mode = QueryMode.ResultSetOnly;
            query.Take = 1;
            var models = svc.Select(query);
            return models.FirstOrDefault();
        }

        public static TModel FindById<TModel, TQuery>(this IModelService svc, int id)
			where TModel :  BaseEntity
			where TQuery :  BaseEntityQuery<TModel>, new()
        {
            var query = new TQuery() {Id = id, Mode = QueryMode.ResultSetOnly};
            var models = svc.Select(query);
            return models.FirstOrDefault();
        }

		public static int GetCount<TModel>(this IModelService svc, BaseEntityQuery<TModel> query) where TModel :  BaseEntity
        {
            query.Take = 0;
            query.Skip = 0;
			query.Mode = QueryMode.CountOnly;
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
