using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using Point = System.Windows.Point;

namespace TumblrTV.scr {
	/// <summary>
	/// Interaction logic for Screensaver.xaml
	/// </summary>
	public partial class Screensaver : Window {
		#region Win32_API_functions
		[DllImport("user32.dll")]
		static extern IntPtr SetParent(IntPtr hWndChild, IntPtr hWndNewParent);

		[DllImport("user32.dll")]
		static extern int SetWindowLong(IntPtr hWnd, int nIndex, IntPtr dwNewLong);

		[DllImport("user32.dll", SetLastError = true)]
		static extern int GetWindowLong(IntPtr hWnd, int nIndex);

		[DllImport("user32.dll")]
		static extern bool GetClientRect(IntPtr hWnd, out Rectangle lpRect);
		#endregion

		private bool isPreview = false;
		private Point? mouseLocation = null;
		private int mouseMoveThreshold = 10;

		public Screensaver() {
			InitializeComponent();
			sizeImages();
		}

		public void sizeImages() {
			img_static.Width = MainWindow.ActualWidth;
			img_static.Height = MainWindow.ActualHeight;
			
			double left = (MainCanvas.ActualWidth - logo_loading.Width) / 2;
			Canvas.SetLeft(logo_loading, left);

			double top  = (MainCanvas.ActualHeight - logo_loading.Height) / 2;
			Canvas.SetTop(logo_loading, top);
		}

		public Screensaver(IntPtr handle) : this() {
			setParentWindow(handle);

			// Set the preview flag
			this.isPreview = true;
		}

		private void Window_KeyDown(object sender, KeyEventArgs e) {
			if (this.isPreview) {
				return;
			}

			Application.Current.Shutdown();
		}

		private void Window_MouseDown(object sender, MouseButtonEventArgs e) {
			if (this.isPreview) {
				return;
			}

			Application.Current.Shutdown();
		}

		private void Window_MouseMove(object sender, MouseEventArgs e) {
			if (this.isPreview) {
				return;
			}

			Point pos = e.GetPosition(null);
			if (this.mouseLocation != null) {
				double moveX = Math.Abs(((Point)this.mouseLocation).X - pos.X),
					moveY = Math.Abs(((Point)this.mouseLocation).Y - pos.Y);

				if ((moveX >= this.mouseMoveThreshold) || (moveY >= this.mouseMoveThreshold)) {
					Application.Current.Shutdown();
				}
				return;
			}

			this.mouseLocation = pos;
		}

		private void setParentWindow(IntPtr handle) {
			Rectangle lpRect = new Rectangle();
			GetClientRect(handle, out lpRect);

			HwndSourceParameters sourceParams = new HwndSourceParameters("sourceParams");

			sourceParams.PositionX = 0;
			sourceParams.PositionY = 0;
			sourceParams.Height = lpRect.Bottom - lpRect.Top;
			sourceParams.Width = lpRect.Right - lpRect.Left;
			sourceParams.ParentWindow = handle;
			sourceParams.WindowStyle = (int)(0x10000000 | 0x40000000 | 0x02000000);

			HwndSource winWPFContent = new HwndSource(sourceParams);
			winWPFContent.Disposed += (o, args) => this.Close();
			winWPFContent.RootVisual = this.MainCanvas;
		}

		private void MainWindow_SizeChanged(object sender, SizeChangedEventArgs e) {
			sizeImages();
		}
	}
}
