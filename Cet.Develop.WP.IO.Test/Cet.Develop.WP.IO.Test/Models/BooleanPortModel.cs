using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.ComponentModel;


namespace Cet.Develop.Windows.IO
{
    public class BooleanPortModel
        : INotifyPropertyChanged
    {
        public BooleanPortModel(
            int offset,
            string description)
        {
            this.Offset = offset;
            this.Description = description;
        }



        public int Offset { get; private set; }
        public string Description { get; private set; }



        private bool _value;
        public bool Value
        {
            get { return this._value; }
            set
            {
                if (this._value != value)
                {
                    this._value = value;
                    this.OnPropertyChanged("Value");
                }
            }
        }



        #region EVT PropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;



        protected virtual void OnPropertyChanged(string propertyName)
        {
            var handler = this.PropertyChanged;

            if (handler != null)
            {
                handler(
                    this,
                    new PropertyChangedEventArgs(propertyName));
            }
        }

        #endregion
    }
}
