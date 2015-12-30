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
		}

		public Screensaver(IntPtr handle) : this() {
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
			winWPFContent.RootVisual = this.MainGrid;

			/*
			IntPtr thisHandle = (new WindowInteropHelper(this)).Handle;
			SetParent(thisHandle, handle);

			// Make this a child window so it'll auto-close when the parent closes
			// GWL_STYLE = -16, WS_CHILD = 0x40000000
			SetWindowLong(thisHandle, -16, new IntPtr(GetWindowLong(thisHandle, -16) | 0x40000000));

			// Position the window inside the preview rect
			Rectangle parentRect;
			GetClientRect(handle, out parentRect);
			this.Left = 0;
			this.Top = 0;
			this.Width = parentRect.Width;
			this.Height = parentRect.Height;
			*/

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
	}
}
