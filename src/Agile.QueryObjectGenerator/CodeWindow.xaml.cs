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
using System.Windows.Shapes;

namespace Agile.QueryObjectGenerator
{
	/// <summary>
	/// CodeWindow.xaml 的交互逻辑
	/// </summary>
	public partial class CodeWindow : Window
	{
		public Type QueryType { get; set; }
		public CodeWindow()
		{
			Initialized += (sender, args) =>
				{
					code.Focus();
					code.SelectAll();
				};
			InitializeComponent();
			PreviewKeyDown += (sender, args) =>
				{
					if (args.Key == System.Windows.Input.Key.Escape)
					{
						Close();
					}
				};
			code.Focus();
			code.SelectAll();
		}

		private void btnSelectAll_Click(object sender, RoutedEventArgs e)
		{
			code.Focus();
			code.SelectAll();
			Clipboard.SetText(code.Text);
			info.Content = "√ 代码已经拷贝至粘贴板。";
		}
	}
}
