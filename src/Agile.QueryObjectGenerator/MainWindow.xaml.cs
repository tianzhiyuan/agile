using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Agile.Common.Data;

namespace Agile.QueryObjectGenerator
{
	/// <summary>
	/// MainWindow.xaml 的交互逻辑
	/// </summary>
	public partial class MainWindow : Window
	{
		public MainWindow()
		{
			this.Initialized += OnInit;
			InitializeComponent();
			
		}
		protected void OnInit(object sender, EventArgs args)
		{
			var queryTypes = Util.GetAllQueryTypes(AppDomain.CurrentDomain.GetAssemblies()).OrderBy(o => o.Name);
			foreach (var type in queryTypes)
			{
				var button = new Button();
				button.Content = type.Name;
				button.Height = 22;
				button.Margin = new Thickness(5);
				button.ToolTip = type.FullName;
				button.Click += OnQueryBtnClick;
				button.DataContext = type;
				container.Children.Add(button);
			}
		}
		protected void OnQueryBtnClick(object sender, RoutedEventArgs e)
		{
			var btn = (Button) sender;
			var type = btn.DataContext;
			var window = new CodeWindow() {QueryType = (Type) type};
			var typeText = "";
			try
			{
				typeText = new QueryObjectParser().GenerateQueryStatement((Type) type);
			}
			catch (Exception error)
			{
				typeText = error.ToString();
			}
			window.code.Text = typeText;
			window.ShowDialog();
		}
		


	}
	public class Foo : BaseEntity
	{
		public Bar Bar { get; set; }
	}

	public class FooQuery : BaseQuery<Foo>
	{
		public BarQuery BarQuery { get; set; }

		#region override
		public override IQueryable<Foo> DoQuery(IQueryable<Foo> source)
		{
			if (this.Id != null)
			{
				source = source.Where(o => o.Id == this.Id);
			}
			if (this.IdList != null)
			{
				source = source.Where(o => this.IdList.Contains(o.Id));
			}
			if (this.CreatedAtRange != null)
			{
				if (this.CreatedAtRange.Left != null)
				{
					source = this.CreatedAtRange.LeftOpen ? source.Where(o => o.CreatedAt > this.CreatedAtRange.Left) : source.Where(o => o.CreatedAt >= this.CreatedAtRange.Left);
				}
				if (this.CreatedAtRange.Right != null)
				{
					source = this.CreatedAtRange.RightOpen ? source.Where(o => o.CreatedAt < this.CreatedAtRange.Right) : source.Where(o => o.CreatedAt <= this.CreatedAtRange.Right);
				}
			}
			if (this.LastModifiedAtRange != null)
			{
				if (this.LastModifiedAtRange.Left != null)
				{
					source = this.LastModifiedAtRange.LeftOpen ? source.Where(o => o.LastModifiedAt > this.LastModifiedAtRange.Left) : source.Where(o => o.LastModifiedAt >= this.LastModifiedAtRange.Left);
				}
				if (this.LastModifiedAtRange.Right != null)
				{
					source = this.LastModifiedAtRange.RightOpen ? source.Where(o => o.LastModifiedAt < this.LastModifiedAtRange.Right) : source.Where(o => o.LastModifiedAt <= this.LastModifiedAtRange.Right);
				}
			}
			if (this.BarQuery != null)
			{
				var naviQuery_1 = this.BarQuery;
				if (naviQuery_1.Id != null)
				{
					source = source.Where(o => o.Bar.Id == naviQuery_1.Id);
				}
				if (naviQuery_1.IdList != null)
				{
					source = source.Where(o => naviQuery_1.IdList.Contains(o.Bar.Id));
				}
				if (naviQuery_1.CreatedAtRange != null)
				{
					if (naviQuery_1.CreatedAtRange.Left != null)
					{
						source = naviQuery_1.CreatedAtRange.LeftOpen ? source.Where(o => o.Bar.CreatedAt > naviQuery_1.CreatedAtRange.Left) : source.Where(o => o.Bar.CreatedAt >= naviQuery_1.CreatedAtRange.Left);
					}
					if (naviQuery_1.CreatedAtRange.Right != null)
					{
						source = naviQuery_1.CreatedAtRange.RightOpen ? source.Where(o => o.Bar.CreatedAt < naviQuery_1.CreatedAtRange.Right) : source.Where(o => o.Bar.CreatedAt <= naviQuery_1.CreatedAtRange.Right);
					}
				}
				if (naviQuery_1.LastModifiedAtRange != null)
				{
					if (naviQuery_1.LastModifiedAtRange.Left != null)
					{
						source = naviQuery_1.LastModifiedAtRange.LeftOpen ? source.Where(o => o.Bar.LastModifiedAt > naviQuery_1.LastModifiedAtRange.Left) : source.Where(o => o.Bar.LastModifiedAt >= naviQuery_1.LastModifiedAtRange.Left);
					}
					if (naviQuery_1.LastModifiedAtRange.Right != null)
					{
						source = naviQuery_1.LastModifiedAtRange.RightOpen ? source.Where(o => o.Bar.LastModifiedAt < naviQuery_1.LastModifiedAtRange.Right) : source.Where(o => o.Bar.LastModifiedAt <= naviQuery_1.LastModifiedAtRange.Right);
					}
				}
			}
			switch (OrderField)
			{
				case "Id":
					source = (OrderDirection == Agile.Common.Data.OrderDirection.ASC) ? source.OrderBy(o => o.Id) : source.OrderByDescending(o => o.Id);
					break;
				case "CreatedAt":
					source = (OrderDirection == Agile.Common.Data.OrderDirection.ASC) ? source.OrderBy(o => o.CreatedAt) : source.OrderByDescending(o => o.CreatedAt);
					break;
				case "LastModifiedAt":
					source = (OrderDirection == Agile.Common.Data.OrderDirection.ASC) ? source.OrderBy(o => o.LastModifiedAt) : source.OrderByDescending(o => o.LastModifiedAt);
					break;
				default:
					source = (OrderDirection == Agile.Common.Data.OrderDirection.ASC) ? source.OrderBy(o => o.Id) : source.OrderByDescending(o => o.Id);
					break;
			}
			return source;
		}
		#endregion


	}

	public class Bar:BaseEntity
	{
		public Foo Foo { get; set; }
	}
	public class BarQuery:BaseQuery<Bar>
	{
		public FooQuery FooQuery { get; set; }
	}
}
