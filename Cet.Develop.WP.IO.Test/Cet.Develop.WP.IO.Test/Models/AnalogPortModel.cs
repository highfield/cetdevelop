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
    public class AnalogPortModel
        : INotifyPropertyChanged
    {
        public AnalogPortModel(
            int offset,
            string description)
        {
            this.Offset = offset;
            this.Description = description;
        }



        public int Offset { get; private set; }
        public string Description { get; private set; }



        private int _rawValue;
        public int RawValue
        {
            get { return this._rawValue; }
            set
            {
                if (this._rawValue != value)
                {
                    this._rawValue = value;
                    this.OnPropertyChanged("RawValue");

                    this.UpdateValues();
                }
            }
        }



        private double _voltageValue;
        public double VoltageValue
        {
            get { return this._voltageValue; }
            private set
            {
                if (this._voltageValue != value)
                {
                    this._voltageValue = value;
                    this.OnPropertyChanged("VoltageValue");
                }
            }
        }



        private double _percentValue;
        public double PercentValue
        {
            get { return this._percentValue; }
            private set
            {
                if (this._percentValue != value)
                {
                    this._percentValue = value;
                    this.OnPropertyChanged("PercentValue");
                }
            }
        }



        private void UpdateValues()
        {
            double normalized = this.RawValue / 1023.0;
            this.VoltageValue = 3.3 * normalized;
            this.PercentValue = 100.0 * normalized;
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
