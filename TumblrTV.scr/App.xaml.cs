using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Interop;
using System.Windows.Media;
using Application = System.Windows.Application;

namespace TumblrTV.scr {
	/// <summary>
	/// Interaction logic for App.xaml
	/// </summary>
	public partial class App : Application {

		private void ApplicationStartup(object sender, StartupEventArgs e) {
			Window win = ProcessArgs(e.Args);
			if (win != null) {
				win.Show();
			}
		}

		private Window ProcessArgs(string[] args) {
			string firstArg = (args.Length > 0) ? args[0].ToLower().Trim() : null;
			string secondArg = null;

			// get our second arg, if any
			if ((firstArg != null) && (firstArg.Length > 2)) {
				secondArg = firstArg.Substring(3).Trim();
				firstArg = firstArg.Substring(0, 2);
			} else if (args.Length > 1) {
				secondArg = args[1];
			}

			// "Config"
			if ((firstArg == null) || (firstArg == "/c")) {
				return DisplayConfigScreen();
			}

			// "Screensaver"
			if (firstArg == "/s") {
				return DisplayScreensaver();
			}

			// "Preview"
			if (firstArg == "/p") {
				if (secondArg == null) {
					DisplayError("missing the window handle =[");
					return null;
				}

				return DisplayPreviewScreen(Int32.Parse(secondArg));
			}

			DisplayError("invalid parameters");
			return null;
		}

		private Window DisplayConfigScreen() {
			return new ConfigScreen();
		}

		private Window DisplayScreensaver() {
			foreach (Screen screen in Screen.AllScreens) {
				(new Screensaver()).Show();
			}

			return null;
		}

		private Window DisplayPreviewScreen(Int32 winHandle) {
			new Screensaver(new IntPtr(winHandle));
			return null;
		}

		private void DisplayError(string message) {
			System.Windows.MessageBox.Show(message, "TumblrTV - Error", MessageBoxButton.OK, MessageBoxImage.Exclamation);
			Application.Current.Shutdown();
		}
	}

}
