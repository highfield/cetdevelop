using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cet.IO.DemoModbusNetduino
{
    public class InputPort
        : INotifyPropertyChanged
    {

        #region PROP Value

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

        #endregion


        public bool Read()
        {
            return this.Value;
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
