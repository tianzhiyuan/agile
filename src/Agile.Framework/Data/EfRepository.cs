using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Agile.Common.Data;

namespace Agile.Framework.Data
{
	public class EfRepository<TEntity, TEntityQuery> : IRepository<TEntity, TEntityQuery>
		where TEntity : BaseEntity
		where TEntityQuery : BaseQuery<TEntity>
	{
		protected string _dbContextName;
		protected IDbContextFactory _DbContextFactory;
		public EfRepository(IDbContextFactory factory, string name)
		{
			_dbContextName = name;
			_DbContextFactory = factory;
		}
		#region IRepository Members
		public TEntity Find(int id)
		{
			using (var db = GetDbContext())
			{
				var dbSet = db.Set<TEntity>();
				return dbSet.Find(id);
			}
		}

		public IEnumerable<TEntity> Find(int[] idList)
		{
			using (var db = GetDbContext())
			{
				var dbSet = db.Set<TEntity>();
				return dbSet.Where(o => idList.Contains(o.Id.Value)).AsNoTracking().ToList();
			}
		}

		public IEnumerable<TEntity> FindMany(TEntityQuery filters)
		{
			throw new NotImplementedException();
		}

		public void Insert(TEntity entity)
		{
			using (var db = GetDbContext())
			{
				var dbSet = db.Set<TEntity>();
				dbSet.Add(entity);
				db.SaveChanges();
			}
		}

		public void Update(TEntity entity)
		{
			using (var db = GetDbContext())
			{
				var entry = db.Entry<TEntity>(entity);
				entry.State = EntityState.Modified;
				db.SaveChanges();
			}
		}

		public void Delete(int key)
		{
			using (var db = GetDbContext())
			{
				db.Configuration.AutoDetectChangesEnabled = true;
				var dbSet = db.Set<TEntity>();
				var entity = dbSet.Find(key);
				dbSet.Remove(entity);
				db.SaveChanges();
			}
		}
		#endregion

		protected virtual DbContext GetDbContext()
		{
			var context = _DbContextFactory.CreateContext(_dbContextName);
			context.Configuration.AutoDetectChangesEnabled = false;
			context.Configuration.ProxyCreationEnabled = false;
			return context;
		}
	}
}
