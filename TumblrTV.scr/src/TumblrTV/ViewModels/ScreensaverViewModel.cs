using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace com.newfurniturey.TumblrTV.ViewModels {
	class ScreensaverViewModel : INotifyPropertyChanged {

		private AppSettings settings = null;
		private string[] urls = null;
		private List<Post> posts = new List<Post>();

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

		public ScreensaverViewModel(AppSettings settings) {
			this.settings = settings;

			BackgroundWorker worker = new BackgroundWorker();
			worker.DoWork += loadTv;
			worker.WorkerReportsProgress = true;
			worker.ProgressChanged += loadTvProgress;
			worker.RunWorkerAsync();
		}

		private void loadTv(object sender, System.ComponentModel.DoWorkEventArgs e) {
			using (WebClient wc = new WebClient()) {
				string type = ((new Random()).Next(0, 2) == 1) ? "pancakes" : "beach";
				wc.Headers.Add("X-Requested-With", "XMLHttpRequest");
				var jsonResponse = wc.DownloadString("https://www.tumblr.com/svc/tv/search/" + type + "?size=1280&limit=40");

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

		private void loadTvProgress(object sender, ProgressChangedEventArgs e) {
			if (e.ProgressPercentage == 1) {
				tim(null, null);
				createTimer();
			}
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
