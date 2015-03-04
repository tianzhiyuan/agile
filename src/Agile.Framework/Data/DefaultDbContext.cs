using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Agile.Framework.Data
{
	public class DefaultDbContext : DbContext
	{
		public DefaultDbContext() : base("DefaultConnection") { }
		public DefaultDbContext(string nameOrConnectionString) : base(nameOrConnectionString) { }

		protected override void OnModelCreating(DbModelBuilder modelBuilder)
		{
			//OnModelCreating只会执行一次，所以这里用反射加载所有EntityTypeConfiguration不会有太大性能问题
			var typesToRegister = Assembly.GetExecutingAssembly()
			                              .GetTypes()
			                              .Where(
				                              type =>
				                              type.BaseType != null && type.BaseType.IsGenericType &&
				                              type.BaseType.GetGenericTypeDefinition() == typeof (EntityTypeConfiguration<>));
			foreach (var type in typesToRegister)
			{
				dynamic configurationInstance = Activator.CreateInstance(type);
				modelBuilder.Configurations.Add(configurationInstance);
			}
			base.OnModelCreating(modelBuilder);
		}
	}
}
