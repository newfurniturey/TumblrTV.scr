using System;
using System.Collections.Generic;
using System.IO;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace com.newfurniturey.TumblrTV {
	public class AppSettings {

		private static AppSettings instance = null;

		private AppSettings() {
		}

		public static AppSettings GetInstance() {
			if (AppSettings.instance == null) {
				AppSettings.instance = new AppSettings();
			}

			return AppSettings.instance;
		}

		public void Save() {
			string filePath = GetPath();
			using (var fstream = File.Open(filePath, FileMode.OpenOrCreate, FileAccess.ReadWrite)) {
				using (var writer = new StreamWriter(fstream)) {
					writer.Write("Testing settings...");
				}
			}
		}

		public void Load() {
			string data;
			string filePath = GetPath();
			using (var fstream = File.Open(filePath, FileMode.OpenOrCreate, FileAccess.Read)) {
				using (var reader = new StreamReader(fstream)) {
					data = reader.ReadToEnd();
				}
			}

			Console.WriteLine("Settings: " + data);
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
}
