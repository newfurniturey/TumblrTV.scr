using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace com.newfurniturey.Converters {
	public class BooleanConverter<T> : IValueConverter {
		public T True {
			get;
			set;
		}

		public T False {
			get;
			set;
		}

		public BooleanConverter(T trueValue, T falseValue) {
			this.True = trueValue;
			this.False = falseValue;
		}

		public virtual object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
			return ((value is bool && ((bool)value)) ? this.True : this.False);
		}

		public virtual object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
			return ((value is T) && EqualityComparer<T>.Default.Equals((T)value, this.True));
		}
	}
}
