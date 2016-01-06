﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace TumblrTV.scr {
	public sealed class BooleanToVisibilityConverter : BooleanConverter<Visibility> {
		public BooleanToVisibilityConverter() : base(Visibility.Visible, Visibility.Collapsed) {
		}
	}
}
