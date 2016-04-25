using System.Linq;
using Agile.Common.Data;

namespace Agile.Framework.Data
{
    public static class Extensions
    {
		public static TModel Single<TModel>(this IModelService svc, BaseQuery<TModel> query)
			where TModel : BaseEntity
        {
			query.Take = 1;
            var models = svc.Select(query);
			return models.SingleOrDefault();
        }

		public static TModel FirstOrDefault<TModel>(this IModelService svc, BaseQuery<TModel> query)
			where TModel :  BaseEntity
        {
            query.Take = 1;
            var models = svc.Select(query);
            return models.FirstOrDefault();
        }

        public static TModel FindById<TModel, TQuery>(this IModelService svc, int id)
			where TModel :  BaseEntity
			where TQuery :  BaseQuery<TModel>, new()
        {
            var query = new TQuery() {Id = id};
            var models = svc.Select(query);
            return models.FirstOrDefault();
        }

		public static int GetCount<TModel>(this IModelService svc, BaseQuery<TModel> query) where TModel :  BaseEntity
        {
            query.Take = 0;
            query.Skip = 0;
            svc.Select(query);
            return 0;
        }

		public static bool Any<TModel>(this IModelService svc, BaseQuery<TModel> query) where TModel :  BaseEntity
        {
            return svc.GetCount(query) > 0;
        }

        public static void Patch<TModel, TQuery>(this IModelService svc, params TModel[] models)
			where TModel :  BaseEntity, new()
			where TQuery : BaseQuery<TModel>, new()
        {
            PartialFiller.Fill<TModel, TQuery>(models);
            svc.Update(models);
        }
    }
}
