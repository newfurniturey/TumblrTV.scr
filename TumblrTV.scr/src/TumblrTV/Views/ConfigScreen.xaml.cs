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
using com.newfurniturey.TumblrTV.ViewModels;

namespace com.newfurniturey.TumblrTV.Views {
	/// <summary>
	/// Interaction logic for ConfigScreen.xaml
	/// </summary>
	public partial class ConfigScreen : Window {
		public ConfigScreen(AppSettings settings) {
			DataContext = new ConfigViewModel(settings);
			InitializeComponent();
		}
	}
}
