using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace com.newfurniturey.TumblrTV.src.TumblrTV {
	public class TumblrTV {
		public const int INVALID_OBJ = -1;

		private AppSettings settings = null;
		private static TumblrTV instance = null;
		private string[] urls = null;
		private ConcurrentDictionary<string, List<Post>> posts = new ConcurrentDictionary<string, List<Post>>();
		private ConcurrentDictionary<string, int> currentIndexes = new ConcurrentDictionary<string, int>();

		private static ConcurrentDictionary<ITvSubscriber, int> registeredTVs = new ConcurrentDictionary<ITvSubscriber, int>();
		private static int id = 0;
		private static bool postsLoaded = false;

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

		public int GetId(ITvSubscriber obj) {
			return TumblrTV.registeredTVs.ContainsKey(obj) ? TumblrTV.registeredTVs[obj] : TumblrTV.INVALID_OBJ;
		}

		public int Register(ITvSubscriber obj) {
			if (!TumblrTV.registeredTVs.ContainsKey(obj)) {
				TumblrTV.registeredTVs[obj] = TumblrTV.id++;
			}

			return TumblrTV.registeredTVs[obj];
		}
		
		public Post NextPost(int id) {
			if (!TumblrTV.postsLoaded) {
				Console.WriteLine("[TumblrTV] Posts not yet loaded.");
				return null;
			}

			string tag = settings.Tags[id % settings.Tags.Count];
			if (!posts.ContainsKey(tag) || (posts[tag].Count == 0)) {
				Console.WriteLine("[TumblrTV] Tag " + tag + " not found.");
				return null;
			}
			
			Post currentPost = posts[tag][currentIndexes[tag]];
			currentIndexes[tag] = (++currentIndexes[tag] % posts[tag].Count);
			return currentPost;
		}

		private void Init() {
			BackgroundWorker worker = new BackgroundWorker();
			worker.WorkerReportsProgress = true;
			worker.ProgressChanged += tvProgressChanged;
			worker.DoWork += loadTv;
			worker.RunWorkerAsync();
		}

		private void loadTv(object sender, System.ComponentModel.DoWorkEventArgs e) {
			int complete = 0;
			int total = this.settings.Tags.Count;
			Parallel.ForEach(this.settings.Tags, (tag) => {
				currentIndexes[tag] = 0;
				posts[tag] = new List<Post>();

				using (WebClient wc = new WebClient()) {
					wc.Headers.Add("X-Requested-With", "XMLHttpRequest");
					var jsonResponse = wc.DownloadString("https://www.tumblr.com/svc/tv/search/" + WebUtility.UrlEncode(tag) + "?size=1280&limit=40");

					dynamic json = JsonConvert.DeserializeObject(jsonResponse);
					urls = new string[((Newtonsoft.Json.Linq.JArray)json.response.images).Count];
					int i = 0;
					foreach (var image in json.response.images) {
						urls[i++] = image.media[0].url;

						posts[tag].Add(new Post() {
							Url = image.media[0].url,
							Avatar = image.avatar,
							Name = image.tumblelog
						});
					}
				}

				complete++;
				((BackgroundWorker)sender).ReportProgress((int)(((float)complete / (float)total) * 100));
			});
		}

		private void tvProgressChanged(object sender, ProgressChangedEventArgs e) {
			if (e.ProgressPercentage == 100) {
				Console.WriteLine("[TumblrTV] Posts Loaded!");
				TumblrTV.postsLoaded = true;
			}
		}
	}
}
