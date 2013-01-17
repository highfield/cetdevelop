using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cet.IO.DemoModbusNetduino
{
    public class AnalogInput
        : INotifyPropertyChanged
    {

        #region PROP Value

        private double _value;

        public double Value
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

        #endregion


        public int Read()
        {
            return (int)(this.Value * 1023.0);
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
                    new PropertyChangedEventArgs(propertyName)
                    );
            }
        }

        #endregion

    }
}
