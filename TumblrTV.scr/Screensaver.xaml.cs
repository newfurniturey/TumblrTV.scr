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

namespace TumblrTV.scr {
	/// <summary>
	/// Interaction logic for Screensaver.xaml
	/// </summary>
	public partial class Screensaver : Window, INotifyPropertyChanged {
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
		string[] urls = null;
		List<Post> posts = new List<Post>();

		private string post_blog_name;
		public string BlogName {
			get {
				return post_blog_name;
			}
			set {
				if (post_blog_name != value) {
					post_blog_name = value;
					NotifyPropertyChanged();
				}
			}
		}

		public Screensaver() {
			DataContext = this;
			InitializeComponent();
			sizeImages();

			BackgroundWorker worker = new BackgroundWorker();
			worker.DoWork += loadTv;
			worker.WorkerReportsProgress = true;
			worker.ProgressChanged += loadTvProgress;
			worker.RunWorkerAsync();
		}

		public Screensaver(IntPtr handle) : this() {
			setParentWindow(handle);

			// set the preview flag
			this.isPreview = true;
		}

		private void loadTvProgress(object sender, ProgressChangedEventArgs e) {
			Post post = posts[0];

			var bitmap = new BitmapImage();
			bitmap.DownloadCompleted += bitmap_DownloadCompleted;
			bitmap.BeginInit();
			bitmap.UriSource = new Uri(urls[0], UriKind.Absolute);
			bitmap.EndInit();
		}

		void bitmap_DownloadCompleted(object sender, EventArgs e) {
			BitmapImage source = (BitmapImage)sender;

			if (((double)source.Width / (double)source.Height) < 1.0) {
				displayBgImage(source);
			}

			displayMainImage(source);

			toggleLoadingDisplay();

			BlogName = posts[0].Name;
			BitmapImage bitmap = new BitmapImage();
			bitmap.BeginInit();
			bitmap.UriSource = new Uri(posts[0].Avatar, UriKind.Absolute);
			bitmap.EndInit();
			blog_avatar.Source = bitmap;

		}

		private void displayBgImage(BitmapImage source) {
			var bg_image = new System.Windows.Controls.Image();
			ImageBehavior.SetAnimatedSource(bg_image, source);

			double target_width = ((double)MainCanvas.ActualWidth * 1.15);
			double n_percent = (target_width / (double)source.Width);
			bg_image.Width = target_width;
			bg_image.Height = (int)(source.Height * n_percent);
			bg_image.Stretch = Stretch.Fill;
			bg_image.Opacity = 0.5;

			Canvas.SetTop(bg_image, (MainCanvas.ActualHeight - bg_image.Height) / 2);
			Canvas.SetLeft(bg_image, (MainCanvas.ActualWidth - bg_image.Width) / 2);
			Canvas.SetZIndex(bg_image, 0);
			MainCanvas.Children.Add(bg_image);
		}

		private void displayMainImage(BitmapImage source) {
			var image = new System.Windows.Controls.Image();
			ImageBehavior.SetAnimatedSource(image, source);

			double asp_ratio = ((double)source.Width / (double)source.Height);
			image.Height = MainCanvas.ActualHeight;
			image.Width = (int)(MainCanvas.ActualWidth * asp_ratio);

			Canvas.SetTop(image, 0);
			Canvas.SetLeft(image, (MainCanvas.ActualWidth - image.Width) / 2);
			Canvas.SetZIndex(image, 1);
			MainCanvas.Children.Add(image);
		}

		private void loadTv(object sender, System.ComponentModel.DoWorkEventArgs e) {
			using (WebClient wc = new WebClient()) {
				wc.Headers.Add("X-Requested-With", "XMLHttpRequest");
				var jsonResponse = wc.DownloadString("https://www.tumblr.com/svc/tv/search/pancakes?size=1280&limit=40");

				dynamic json = JsonConvert.DeserializeObject(jsonResponse);
				urls = new string[((Newtonsoft.Json.Linq.JArray)json.response.images).Count];
				int i = 0;
				foreach (var image in json.response.images) {
					urls[i++] = image.media[0].url;

					posts.Add(new Post() {
						Url = image.media[0].url,
						Avatar = image.avatar,
						Name = image.tumblelog
					});
				}
			}

			((BackgroundWorker)sender).ReportProgress(1);
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

		#region INotifyPropertyChanged Handler(s)
		public event PropertyChangedEventHandler PropertyChanged;
		protected virtual void NotifyPropertyChanged([CallerMemberName] String propertyName = "") {
			if (PropertyChanged != null) {
				var handler = PropertyChanged;
				handler(this, new PropertyChangedEventArgs(propertyName));
			}
		}
		#endregion
	}

	class Post {
		public string Url { get; set; }
		public string Avatar { get; set; }
		public string Name { get; set; }
	}
}
