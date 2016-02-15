using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Net;
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
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using WpfAnimatedGif;
using System.Runtime.CompilerServices;
using com.newfurniturey.TumblrTV.ViewModels;

namespace com.newfurniturey.TumblrTV.Views {
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

		public Screensaver(AppSettings settings) {
			DataContext = new ScreensaverViewModel(settings);
			InitializeComponent();
			sizeImages();
		}

		public Screensaver(AppSettings settings, Rectangle bounds) : this(settings) {
			this.Left = bounds.Left;
			this.Top = bounds.Top;
			this.Width = bounds.Width;
			this.Height = bounds.Height;
			if (this.IsLoaded) {
				this.WindowState = System.Windows.WindowState.Maximized;
			}
		}

		public Screensaver(AppSettings settings, IntPtr handle) : this(settings) {
			setParentWindow(handle);

			// set the preview flag
			this.isPreview = true;
		}

		private void sizeImages() {
			// stretch the static to full-screen
			img_static.Width = MainWindow.ActualWidth;
			img_static.Height = MainWindow.ActualHeight;

			// absolute-center for the loading logo
			Canvas.SetLeft(logo_loading, (MainCanvas.ActualWidth - logo_loading.Width) / 2);
			Canvas.SetTop(logo_loading, (MainCanvas.ActualHeight - logo_loading.Height) / 2);
		}

		private void toggleLoadingDisplay() {
			if (logo_loading.Visibility == Visibility.Visible) {
				logo_loading.Visibility = Visibility.Hidden;
				img_static.Visibility = Visibility.Hidden;
				if (!this.isPreview) {
					logo_small.Visibility = Visibility.Visible;
					blog_info.Visibility = Visibility.Visible;
				}
			} else {
				logo_loading.Visibility = Visibility.Visible;
				img_static.Visibility = Visibility.Visible;
				logo_small.Visibility = Visibility.Hidden;
				blog_info.Visibility = Visibility.Hidden;
			}
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
				double moveX = Math.Abs(((Point)this.mouseLocation).X - pos.X);
				double moveY = Math.Abs(((Point)this.mouseLocation).Y - pos.Y);

				// if the cursor has moved more than X pixels, kill the app
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

		private void post_image_bg_AnimationLoaded(object sender, RoutedEventArgs e) {
			ImageSource source = WpfAnimatedGif.ImageBehavior.GetAnimatedSource((System.Windows.Controls.Image)sender);

			if ((((double)source.Width / (double)source.Height) < 1.0) || ((double)source.Width < (double)MainCanvas.ActualWidth)) {
				sizeBgImage(source);
			}
			sizeMainImage(source);
		}

		private void sizeMainImage(ImageSource source) {
			double asp_ratio = ((double)source.Width / (double)source.Height);
			post_image.Height = MainCanvas.ActualHeight;
			post_image.Width = (int)(MainCanvas.ActualWidth * asp_ratio);

			Canvas.SetTop(post_image, 0);
			Canvas.SetLeft(post_image, (MainCanvas.ActualWidth - post_image.Width) / 2);
			Canvas.SetZIndex(post_image, 1);
			//MainCanvas.Children.Add(image);
			
		}

		private void sizeBgImage(ImageSource source) {
			double target_width = ((double)MainCanvas.ActualWidth * 1.15);
			double n_percent = (target_width / (double)source.Width);
			post_image_bg.Width = target_width;
			post_image_bg.Height = (int)(source.Height * n_percent);
			post_image_bg.Stretch = Stretch.Fill;
			post_image_bg.Opacity = 0.5;

			Canvas.SetTop(post_image_bg, (MainCanvas.ActualHeight - post_image_bg.Height) / 2);
			Canvas.SetLeft(post_image_bg, (MainCanvas.ActualWidth - post_image_bg.Width) / 2);
			//Canvas.SetZIndex(post_image_bg, 0);
		}
	}
}
