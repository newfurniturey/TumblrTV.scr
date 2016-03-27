using com.newfurniturey.TumblrTV.src.TumblrTV;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using TV = com.newfurniturey.TumblrTV.src.TumblrTV.TumblrTV;

namespace com.newfurniturey.TumblrTV.ViewModels {
	class ScreensaverViewModel : INotifyPropertyChanged {

		private TV tv = null;
		private int tvId = -1;

		#region Properties
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

		private string post_blog_avatar;
		public string BlogAvatarUrl {
			get {
				return post_blog_avatar;
			}
			set {
				if (post_blog_avatar != value) {
					post_blog_avatar = value;
					NotifyPropertyChanged();
				}
			}
		}



		private string post_image_url = null;
		public string PostImageUrl {
			get {
				return post_image_url;
			}
			set {
				if (post_image_url != value) {
					post_image_url = value;
					NotifyPropertyChanged();
					NotifyPropertyChanged("PostVisible");
				}
			}
		}

		public bool PostVisible {
			get {
				return PostImageUrl != null;
			}
		}
		#endregion

		public ScreensaverViewModel(TV tv) {
			this.tv = tv;
			this.tvId = tv.GetId();

			loadTv();
		}

		private void loadTv() {
			Post post = this.tv.NextPost(this.tvId);
			if (post == null) {

			}
		}

		private int counter = 0;
		private void tim(object source, System.Timers.ElapsedEventArgs e) {
			if (counter >= posts.Count) {
				counter = 0;
			}

			Post post = posts[counter++];
			PostImageUrl = post.Url;
			BlogName = post.Name;
			BlogAvatarUrl = post.Avatar;
		}

		private void createTimer() {
			System.Timers.Timer t = new System.Timers.Timer();
			t.Elapsed += new System.Timers.ElapsedEventHandler(tim);
			t.Interval = 5000;
			t.Enabled = true;
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
}
