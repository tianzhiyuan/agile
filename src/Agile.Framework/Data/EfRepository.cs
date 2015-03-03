using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Agile.Common.Data;
using EntityFramework.Extensions;

namespace Agile.Framework.Data
{
	public class EfRepository<TEntity> : IRepository<TEntity> where TEntity : BaseEntity
	{
		private IDbContextFactory _dbContextFactory;

		protected DbContext GetContext()
		{
			var context = _dbContextFactory.Create(typeof(TEntity));
			context.Configuration.AutoDetectChangesEnabled = false;
			context.Configuration.ProxyCreationEnabled = false;
			return context;
		}
		public EfRepository(IDbContextFactory factory)
		{
			_dbContextFactory = factory;
		}

		public IQueryable<TEntity> GetTable()
		{
			using (var db = GetContext())
			{
				return db.Set<TEntity>();
			}
		}

		public TEntity Get(int id)
		{
			using (var db = GetContext())
			{
				return db.Set<TEntity>().Find(id);
			}
		}

		public IEnumerable<TEntity> GetMany(IEnumerable<int> ids)
		{
			return GetMany(o => ids.Contains(o.Id));
		}

		public IEnumerable<TEntity> GetMany(Expression<Func<TEntity, bool>> predicate, IPagingOption<TEntity> pagingOption = null)
		{
			using (var db = GetContext())
			{
				var source = db.Set<TEntity>().AsQueryable();
				source = source.Where(predicate);
				if (pagingOption != null)
				{
					source = pagingOption.Apply(source);
				}
				return source.AsNoTracking().ToList();
			}
		}

		public IEnumerable<TEntity> GetMany(Expression<Func<TEntity, bool>> predicate, IPagingOption<TEntity> pagingOption, string[] includes)
		{
			using (var db = GetContext())
			{
				var source = db.Set<TEntity>().AsQueryable();
				if (includes != null && includes.Any())
				{
					source = includes.Aggregate(source, (current, include) => current.Include(include));
				}
				source = source.Where(predicate);
				if (pagingOption != null)
				{
					source = pagingOption.Apply(source);
				}
				return source.AsNoTracking().ToList();
			}
		}

		public int Count(Expression<Func<TEntity, bool>> predicate)
		{
			using (var db = GetContext())
			{
				var source = db.Set<TEntity>().AsQueryable();
				return source.Count(predicate);
			}
		}

		public void Update(TEntity entity)
		{
			try
			{
				using (var db = GetContext())
				{
					var entry = db.Entry<TEntity>(entity);
					entry.State = EntityState.Modified;

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
				throw fail;
			}
		}

		public void Delete(TEntity entity)
		{
			try
			{
				using (var db = GetContext())
				{
					var entry = db.Entry<TEntity>(entity);
					entry.State = EntityState.Deleted;

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
		}

		public void Create(TEntity entity)
		{
			try
			{
				using (var db = GetContext())
				{
					var set = db.Set<TEntity>();
					set.Add(entity);
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
				throw fail;
			}
		}

		public void UpdateBatch(Expression<Func<TEntity, TEntity>> updateProperties, Expression<Func<TEntity, bool>> predicate)
		{
			using (var db = GetContext())
			{
				db.Set<TEntity>().Where(predicate).Update(updateProperties);
			}
		}

		public void DeleteBatch(Expression<Func<TEntity, bool>> predicate)
		{
			using (var db = GetContext())
			{
				db.Set<TEntity>().Where(predicate).Delete();
			}
		}

		public IEnumerable<TModel> SqlQuery<TModel>(string query, params object[] parameters) where TModel : class
		{
			using (var db = GetContext())
			{
				return db.Database.SqlQuery<TModel>(query, parameters).ToList();
			}
		}
	}
}
