using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace com.newfurniturey.TumblrTV {
	public class AppSettings {
		Settings settings = null;

		#region LocalSettings
		public List<string> Tags {
			get {
				return this.settings.Tags;
			}

			set {
				this.settings.Tags = value;
			}
		}

		public bool TagsShuffle {
			get {
				return this.settings.TagsShuffle;
			}

			set {
				this.settings.TagsShuffle = value;
			}
		}

		public bool MonitorsUnique {
			get {
				return this.settings.MonitorsUnique;
			}

			set {
				this.settings.MonitorsUnique = value;
			}
		}

		public bool MonitorsDupes {
			get {
				return this.settings.MonitorsDupes;
			}

			set {
				this.settings.MonitorsDupes = value;
			}
		}

		public bool CachingLocal {
			get {
				return this.settings.CachingLocal;
			}

			set {
				this.settings.CachingLocal = value;
			}
		}

		public bool CachingNoNetwork {
			get {
				return this.settings.CachingNoNetwork;
			}

			set {
				this.settings.CachingNoNetwork = value;
			}
		}
		#endregion

		public AppSettings(bool autoLoad = true) {
			if (autoLoad) {
				Load();
			} else {
				this.settings = new Settings();
			}
		}

		public void Save() {
			if (this.settings == null) {
				throw new Exception("settings haven't been set yet...");
			}

			string data = JsonConvert.SerializeObject(this.settings, Formatting.Indented);
			Console.WriteLine("Saving Settings: " + data);

			string filePath = GetPath();
			using (var fstream = File.Open(filePath, FileMode.OpenOrCreate, FileAccess.ReadWrite)) {
				using (var writer = new StreamWriter(fstream)) {
					writer.Write(data);
				}
			}
		}

		public void Load() {
			string filePath = GetPath();
			try {
				using (var fstream = File.Open(filePath, FileMode.OpenOrCreate, FileAccess.Read)) {
					using (var reader = new StreamReader(fstream)) {
						this.settings = JsonConvert.DeserializeObject<Settings>(reader.ReadToEnd());
					}
				}
			} catch (Newtonsoft.Json.JsonReaderException e) {
				// eek
			}

			Console.WriteLine("Loaded Settings: " + JsonConvert.SerializeObject(this.settings, Formatting.Indented));
		}

		private String GetPath() {
			string path = String.Format(@"{0}\TumblrTV",
				Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData)
			);
			if (!Directory.Exists(path)) {
				Directory.CreateDirectory(path);
			}

			return String.Format(@"{0}\settings.json",
				path
			);
		}
	}

	class Settings {
		public List<string> Tags { get; set; }
		public bool TagsShuffle { get; set; }
		public bool MonitorsUnique { get; set; }
		public bool MonitorsDupes { get; set; }
		public bool CachingLocal { get; set; }
		public bool CachingNoNetwork { get; set; }
	}

}
