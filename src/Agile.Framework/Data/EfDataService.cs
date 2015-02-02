using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using Agile.Common.Data;
using Agile.Common.Utilities;

namespace Agile.Framework.Data
{
	public class EfDataService : IModelService
	{
		public EfDataService()
        {
            
        }

        private readonly object _contextCreatorLocker = new object();
        private Func<string, DbContext> _createContext;
		protected DbContext GetContext()
		{
			Func<string, DbContext> handler = _createContext;
			if (handler == null)
			{
				lock (_contextCreatorLocker)
				{
					if (_createContext == null)
					{
						var type = Type.GetType(ContextTypeString);
						if (type == null) throw new ArgumentNullException("ContextTypeString");

						var constructors = type.GetConstructors();
						var arg = Expression.Parameter(typeof(string));
						var predicateMethod = typeof(string).GetMethod("IsNullOrWhiteSpace");
						handler = _createContext = Expression.Lambda<Func<string, DbContext>>(
							Expression.Condition(
								Expression.Call(predicateMethod, arg),
								Expression.New(constructors.First(o => o.GetParameters().Length == 0)),
								Expression.New(constructors.First(o => o.GetParameters().Length == 1), arg)
							),
							arg
						).Compile();
					}
					else
					{
						handler = _createContext;
					}
				}
			}

			var db = handler(NameOrConnectionString);
			db.Configuration.AutoDetectChangesEnabled = false;
			db.Configuration.ProxyCreationEnabled = false;
			return db;
		}

		public event EventHandler<DataServiceEventArgs> BeforeUpdate;
		public event EventHandler<DataServiceEventArgs> AfterUpdate;
		public event EventHandler<DataServiceEventArgs> BeforeDelete;
		public event EventHandler<DataServiceEventArgs> AfterDelete;
		public event EventHandler<DataServiceEventArgs> BeforeCreate;
		public event EventHandler<DataServiceEventArgs> AfterCreate;
        public void Update<TModel>(params TModel[] models) where TModel : BaseEntity, new()
        {
            if (BeforeUpdate != null)
            {
                BeforeUpdate.Invoke(this, new DataServiceEventArgs() { Items = models });
            }
			try
			{
				using (var db = GetContext())
				{
					foreach (var model in models)
					{
						var clone = ObjectUtils.Clone(model);
						var entry = db.Entry<TModel>(clone);
						entry.State = EntityState.Modified;
					}
					int rowsAffected = db.SaveChanges();
					Debug.Assert(rowsAffected > 0);
				}
			}
			catch (DbEntityValidationException dbEx)
			{
				var msg = string.Empty;

				foreach (var validationErrors in dbEx.EntityValidationErrors)
					foreach (var validationError in validationErrors.ValidationErrors)
						msg += string.Format("Property: {0} Error: {1}", validationError.PropertyName, validationError.ErrorMessage) + Environment.NewLine;

				var fail = new Exception(msg, dbEx);
				//Debug.WriteLine(fail.Message, fail);
				throw fail;
			}
            if (AfterUpdate != null)
            {
                AfterUpdate.Invoke(this, new DataServiceEventArgs() { Items = models });
            }
        }

		public void Create<TModel>(params TModel[] models) where TModel : BaseEntity, new()
        {
            if (BeforeCreate != null)
            {
                BeforeCreate.Invoke(this, new DataServiceEventArgs() { Items = models });
            }
			try
			{
				using (var db = GetContext())
				{
					var dbSet = db.Set<TModel>();
					var clones = new List<TModel>();
					foreach (var model in models)
					{
						var clone = ObjectUtils.Clone(model);
						clones.Add(clone);
						dbSet.Add(clone);
					}
					var rowsAffected = db.SaveChanges();
					for (var index = 0; index < models.Count(); index++)
					{
						models[index].Id = clones[index].Id;
					}
					Debug.Assert(rowsAffected > 0);
				}
			}
			catch (DbEntityValidationException dbEx)
			{
				var msg = string.Empty;

				foreach (var validationErrors in dbEx.EntityValidationErrors)
					foreach (var validationError in validationErrors.ValidationErrors)
						msg += string.Format("Property: {0} Error: {1}", validationError.PropertyName, validationError.ErrorMessage) + Environment.NewLine;

				var fail = new Exception(msg, dbEx);
				//Debug.WriteLine(fail.Message, fail);
				throw fail;
			}
            if (AfterCreate != null)
            {
                AfterCreate.Invoke(this, new DataServiceEventArgs() { Items = models });
            }
        }

		public void Delete<TModel>(params TModel[] models) where TModel : BaseEntity, new()
        {
            if (BeforeDelete != null)
            {
                BeforeDelete.Invoke(this, new DataServiceEventArgs() { Items = models });
            }
			try
			{
				using (var db = GetContext())
				{
					foreach (var model in models)
					{
						var clone = ObjectUtils.Clone<TModel>(model);
						var entry = db.Entry(clone);
						entry.State = EntityState.Deleted;
					}
					var rowsAffected = db.SaveChanges();
					Debug.Assert(rowsAffected > 0);
				}
			}
			catch (DbEntityValidationException dbEx)
			{
				var msg = string.Empty;

				foreach (var validationErrors in dbEx.EntityValidationErrors)
					foreach (var validationError in validationErrors.ValidationErrors)
						msg += string.Format("Property: {0} Error: {1}", validationError.PropertyName, validationError.ErrorMessage) + Environment.NewLine;

				var fail = new Exception(msg, dbEx);
				//Debug.WriteLine(fail.Message, fail);
				throw fail;
			}
            
            if (AfterDelete != null)
            {
                AfterDelete.Invoke(this, new DataServiceEventArgs() { Items = models });
            }
        }

        public string NameOrConnectionString { get; set; }
        public string ContextTypeString { get; set; }
        public IEnumerable<TModel> SqlQuery<TModel>(string query, params object[] parameters) where TModel : class
        {
            using (var db = GetContext())
            {
                var source = db.Set<TModel>();
                return source.SqlQuery(query, parameters).AsNoTracking().ToList();
            }
        }
		public IEnumerable<TModel> Select<TModel>(BaseEntityQuery<TModel> query) where TModel : BaseEntity
		{
			using (var db = GetContext())
			{
				var source = db.Set<TModel>() as IQueryable<TModel>;
				IEnumerable<TModel> result = Enumerable.Empty<TModel>();

				if (query.Includes != null && query.Includes.Any())
				{
					source = query.Includes.Aggregate(source, (current, include) => current.Include(include));
				}
				//do some query
				source = query.DoQuery(source);
				var queryMode = query.Mode??QueryMode.Both;
				bool isNoTracking = query.IsNoTracking ?? true;
				if (queryMode == QueryMode.CountOnly)
				{
					query.CountOfResultSet = source.Count();
					return result;
				}
				if (isNoTracking)
				{
					source = source.AsNoTracking();
				}

				if (queryMode == QueryMode.Both)
				{
					query.CountOfResultSet = source.Count();
				}

				if (query.Skip > 0)
				{
					source = source.Skip(query.Skip ?? 0);
				}

				if (query.Take > 0)
				{
					source = source.Take(query.Take ?? 0);
				}


				if (!(query.Take <= 0))
				{
					result = source.ToList();
				}
				
				return result;
			}

		}

		#region IModelService Members

		IEnumerable<TModel> IModelService.Select<TModel>(BaseEntityQuery<TModel> query) 
		{
            return this.Select<TModel>(query);
        }

        void IModelService.Update<TModel>(params TModel[] models)
        {
            this.Update(models);
        }

        void IModelService.Delete<TModle>(params TModle[] models)
        {
            this.Delete(models);
        }

        void IModelService.Create<TModel>(params TModel[] models)
        {
            this.Create(models);
        }

        #endregion
	}
}
