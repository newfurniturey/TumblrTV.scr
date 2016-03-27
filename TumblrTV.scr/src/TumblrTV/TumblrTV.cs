using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace com.newfurniturey.TumblrTV.src.TumblrTV {
	public class TumblrTV {

		private AppSettings settings = null;
		private static TumblrTV instance = null;
		private string[] urls = null;
		private List<Post> posts = new List<Post>();
		private static int id = 0;

		private TumblrTV(AppSettings settings) {
			this.settings = settings;
			Init();
		}

		public static TumblrTV GetInstance(AppSettings settings = null) {
			if (TumblrTV.instance == null) {
				if (settings == null) {
					throw new ArgumentNullException("Settings cannot be null during initialization");
				}

				TumblrTV.instance = new TumblrTV(settings);
			}
			return TumblrTV.instance;
		}

		public int GetId() {
			return TumblrTV.id++;
		}

		public Post NextPost(int id) {
			return null;
		}

		private void Init() {
			BackgroundWorker worker = new BackgroundWorker();
			worker.DoWork += loadTv;
			worker.RunWorkerAsync();
		}

		private void loadTv(object sender, System.ComponentModel.DoWorkEventArgs e) {
			int complete = 0;
			int total = this.settings.Tags.Count;
			Parallel.ForEach(this.settings.Tags, (tag) => {
				using (WebClient wc = new WebClient()) {
					wc.Headers.Add("X-Requested-With", "XMLHttpRequest");
					var jsonResponse = wc.DownloadString("https://www.tumblr.com/svc/tv/search/" + tag + "?size=1280&limit=40");

					dynamic json = JsonConvert.DeserializeObject(jsonResponse);
					urls = new string[((Newtonsoft.Json.Linq.JArray)json.response.images).Count];
					int i = 0;
					foreach (var image in json.response.images) {
						urls[i++] = image.media[0].url;
						Console.WriteLine("loading: " + tag + " => " + image.media[0].url);
						posts.Add(new Post() {
							Url = image.media[0].url,
							Avatar = image.avatar,
							Name = image.tumblelog
						});
					}
				}

				((BackgroundWorker)sender).ReportProgress(++complete / total);
			});
		}
	}
}
