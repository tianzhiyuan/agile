using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Agile.Common.Data;

namespace Agile.Framework.Data
{
    public interface IModelService
    {
		IEnumerable<TModel> Select<TModel>(BaseQuery<TModel> query)
			where TModel :  BaseEntity;

		void Update<TModel>(params TModel[] models) where TModel :  BaseEntity, new();
		void Delete<TModle>(params TModle[] models) where TModle :  BaseEntity, new();
		void Create<TModel>(params TModel[] models) where TModel :  BaseEntity, new();

        

    }
}
