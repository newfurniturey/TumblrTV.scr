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
	class ConfigViewModel : INotifyPropertyChanged {

		private AppSettings settings = null;

		#region Properties
		public string Tags {
			get {
				return String.Join(", ", this.settings.Tags);
			}
			set {
				var tags = value.Split(',').Select(tag => tag.Trim()).ToList<string>();
				if (!(new HashSet<string>(this.settings.Tags).SetEquals(tags))) {
					this.settings.Tags = tags;
					NotifyPropertyChanged();
				}
			}
		}

		public bool TagsShuffle {
			get {
				return this.settings.TagsShuffle;
			}

			set {
				if (this.settings.TagsShuffle != value) {
					this.settings.TagsShuffle = value;
					NotifyPropertyChanged();
				}
			}
		}

		public bool MonitorsUnique {
			get {
				return this.settings.MonitorsUnique;
			}

			set {
				if (this.settings.MonitorsUnique != value) {
					this.settings.MonitorsUnique = value;
					NotifyPropertyChanged();
				}
			}
		}

		public bool MonitorsDupes {
			get {
				return this.settings.MonitorsDupes;
			}

			set {
				if (this.settings.MonitorsDupes != value) {
					this.settings.MonitorsDupes = value;
					NotifyPropertyChanged();
				}
			}
		}

		public bool CachingLocal {
			get {
				return this.settings.CachingLocal;
			}

			set {
				if (this.settings.CachingLocal != value) {
					this.settings.CachingLocal = value;
					NotifyPropertyChanged();
				}
			}
		}

		public bool CachingNoNetwork {
			get {
				return this.settings.CachingNoNetwork;
			}

			set {
				if (this.settings.CachingNoNetwork != value) {
					this.settings.CachingNoNetwork = value;
					NotifyPropertyChanged();
				}
			}
		}
		#endregion

		public ConfigViewModel(AppSettings settings) {
			this.settings = settings;
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
