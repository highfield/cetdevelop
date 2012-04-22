using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;


namespace Cet.Develop.Windows.IO
{
    public partial class LedControl 
        : UserControl
    {
        public LedControl()
        {
            InitializeComponent();
        }



        #region DP Value

        public static readonly DependencyProperty ValueProperty = DependencyProperty.Register(
            "Value",
            typeof(bool),
            typeof(LedControl),
            new PropertyMetadata(
                false,
                (obj, args) =>
                {
                    var ctl = (LedControl)obj;
                    ctl.ValueChanged(args);
                }));



        public bool Value
        {
            get { return (bool)GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }

        #endregion



        private void ValueChanged(DependencyPropertyChangedEventArgs args)
        {
            this.ImgOff.Visibility = (bool)args.NewValue
                ? Visibility.Collapsed
                : Visibility.Visible;

            this.ImgOn.Visibility = (bool)args.NewValue
                ? Visibility.Visible
                : Visibility.Collapsed;
        }

    }
}
