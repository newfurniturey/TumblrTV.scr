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
	class ScreensaverViewModel : ITvSubscriber, INotifyPropertyChanged {

		public const int MAX_RETRIES = 2;

		private TV tv = null;
		private int tvId = -1;
		private System.Timers.Timer timer = null;
		private int retryCount = 0;

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
					NotifyPropertyChanged("LoadingVisible");
				}
			}
		}
		private string next_post_image_url = null;
		public string PostImageUrlBackground {
			get {
				return next_post_image_url;
			}
			set {
				if (next_post_image_url != value) {
					next_post_image_url = value;
					NotifyPropertyChanged();
				}
			}
		}

		public bool PostVisible {
			get {
				return (!not_found && (PostImageUrl != null));
			}
		}

		public bool LoadingVisible {
			get {
				return !PostVisible && !NotFound;
			}
		}

		private bool not_found = false;
		public bool NotFound {
			get {
				return not_found;
			}
			set {
				if (not_found != value) {
					not_found = value;
					NotifyPropertyChanged();
					NotifyPropertyChanged("PostVisible");
					NotifyPropertyChanged("LoadingVisible");
				}
			}
		}

		private string not_found_tag = "";
		public string NotFoundTag {
			get {
				return not_found_tag;
			}
			set {
				if (not_found_tag != value) {
					not_found_tag = value;
					NotifyPropertyChanged();
				}
			}
		}
		#endregion

		public ScreensaverViewModel(TV tv) {
			this.tv = tv;
			this.tvId = tv.Register(this);

			loadTv();
			createTimer();
		}

		private Post nextPost = null;
		public void ImageLoaded() {
			PostImageUrl = nextPost.Url;
			BlogName = nextPost.Name;
			BlogAvatarUrl = nextPost.Avatar;
			nextPost = null;
			this.timer.Start();
		}

		private void loadTv() {
			Post post = this.tv.NextPost(this.tvId);
			if (post == null) {
				if (this.retryCount++ >= ScreensaverViewModel.MAX_RETRIES) {
					this.timer.Stop();
					this.NotFoundTag = tv.GetCurrentTag(this.tvId);
					this.NotFound = true;
				}

				return;
			}
			this.retryCount = 0;

			nextPost = post;
			PostImageUrlBackground = post.Url;
			/*
			PostImageUrl = post.Url;
			BlogName = post.Name;
			BlogAvatarUrl = post.Avatar;
			*/
		}
		
		private void timerEvent(object source, System.Timers.ElapsedEventArgs e) {
			this.timer.Stop();
			loadTv();
		}

		private void createTimer() {
			this.timer = new System.Timers.Timer();
			this.timer.Elapsed += new System.Timers.ElapsedEventHandler(timerEvent);
			this.timer.Interval = 5000;
			this.timer.Enabled = true;
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
