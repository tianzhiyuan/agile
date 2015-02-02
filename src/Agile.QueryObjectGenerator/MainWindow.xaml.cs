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
			InitializeComponent();
		}

		private void btnAnalyze_Click(object sender, RoutedEventArgs e)
		{
			var typeText = modelTypeTxt.Text;
			var queryType = Type.GetType(typeText);
			if (queryType == null)
			{
				codeBlock.Text = "类型错误";
				return;
			}
			try
			{
				codeBlock.Text = new QueryObjectParser().GenerateQueryStatement(queryType);
			}
			catch (Exception ex)
			{
				codeBlock.Text = ex.ToString();
			}

		}


	}
	public class Foo : BaseEntity
	{

	}
	public class FooQuery : BaseEntityQuery<Foo>
	{
		public override IQueryable<Foo> DoQuery(IQueryable<Foo> source)
		{
			if (Id != null)
			{
				source = source.Where(o => o.Id == Id);
			}
			if (IdList != null)
			{
				source = source.Where(o => IdList.Contains(o.Id.Value));
			}
			if (CreatorIdList != null)
			{
				source = source.Where(o => CreatorIdList.Contains(o.CreatorId.Value));
			}
			if (LastModifierIdList != null)
			{
				source = source.Where(o => LastModifierIdList.Contains(o.LastModifierId.Value));
			}
			if (CreatedAtRange != null)
			{
				if (CreatedAtRange.Left != null)
				{
					source = CreatedAtRange.LeftOpen ? source.Where(o => o.CreatedAt > CreatedAtRange.Left) : source.Where(o => o.CreatedAt >= CreatedAtRange.Left);
				}
				if (CreatedAtRange.Right != null)
				{
					source = CreatedAtRange.RightOpen ? source.Where(o => o.CreatedAt < CreatedAtRange.Right) : source.Where(o => o.CreatedAt <= CreatedAtRange.Right);
				}
			}
			if (LastModifiedAtRange != null)
			{
				if (LastModifiedAtRange.Left != null)
				{
					source = LastModifiedAtRange.LeftOpen ? source.Where(o => o.LastModifiedAt > LastModifiedAtRange.Left) : source.Where(o => o.LastModifiedAt >= LastModifiedAtRange.Left);
				}
				if (LastModifiedAtRange.Right != null)
				{
					source = LastModifiedAtRange.RightOpen ? source.Where(o => o.LastModifiedAt < LastModifiedAtRange.Right) : source.Where(o => o.LastModifiedAt <= LastModifiedAtRange.Right);
				}
			}
			switch (OrderField)
			{
				case "Id":
					source = (OrderDirection == Agile.Common.Data.OrderDirection.ASC) ? source.OrderBy(o => o.Id) : source.OrderByDescending(o => o.Id);
					break;
				case "CreatorId":
					source = (OrderDirection == Agile.Common.Data.OrderDirection.ASC) ? source.OrderBy(o => o.CreatorId) : source.OrderByDescending(o => o.CreatorId);
					break;
				case "LastModifierId":
					source = (OrderDirection == Agile.Common.Data.OrderDirection.ASC) ? source.OrderBy(o => o.LastModifierId) : source.OrderByDescending(o => o.LastModifierId);
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
	}
}
