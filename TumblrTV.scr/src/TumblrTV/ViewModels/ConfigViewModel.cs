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
		private List<string> tags = new List<string>();
		public string Tags {
			get {
				return String.Join(",", this.tags);
			}
			set {
				var tags = value.Split(',').Select(tag => tag.Trim()).ToList<string>();
				if (!(new HashSet<string>(this.tags).SetEquals(tags))) {
					this.tags = tags;
					NotifyPropertyChanged();
				}
			}
		}

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
