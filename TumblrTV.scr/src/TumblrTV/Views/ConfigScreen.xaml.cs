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
		private AppSettings settings = null;
		public ConfigScreen(AppSettings settings) {
			this.settings = settings;
			DataContext = new ConfigViewModel(settings);
			InitializeComponent();
		}

		private void SaveButton_Click(object sender, RoutedEventArgs e) {
			this.settings.Save();
			this.Close();
		}

		private void CancelButton_Click(object sender, RoutedEventArgs e) {
			this.Close();
		}
	}
}
