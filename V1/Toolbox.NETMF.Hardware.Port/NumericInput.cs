using System;
using Microsoft.SPOT;

/*
 * Copyright 2012 Mario Vernari (http://netmftoolbox.codeplex.com/)
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *     http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */
namespace Toolbox.NETMF.Hardware
{
    /// <summary>
    /// Define the abstract class for a generic numeric input port
    /// </summary>
    public abstract class NumericInput
        : IDisposable
    {
        /// <summary>
        /// Constructor for the <see cref="Toolbox.NETMF.Hardware.NumericInput"/>
        /// </summary>
        protected NumericInput()
        {
            //set the default range
            this.Minimum = int.MinValue;
            this.Maximum = int.MaxValue;
        }



        /// <summary>
        /// The minimum inclusive bound of the range
        /// </summary>
        public int Minimum { get; private set; }



        /// <summary>
        /// The maximum inclusive bound of the range
        /// </summary>
        public int Maximum { get; private set; }


        
        private int _value;

        /// <summary>
        /// Gets/sets the value of the port
        /// </summary>
        public int Value
        {
            get { return this._value; }
            set
            {
                if (this._value != value)
                {
                    this._value = this.CoerceValue(value);
                    this.OnStatusChanged();
                }
            }
        }



        /// <summary>
        /// Gets the current value of the port
        /// </summary>
        /// <returns>Same as <see cref="Toolbox.NETMF.Hardware.NumericInput.Value"/></returns>
        /// <remarks>Provided for compatibility with MF ports</remarks>
        public int Read()
        {
            return this.Value;
        }



        /// <summary>
        /// Sets the range allowed for the value of the port
        /// </summary>
        /// <param name="minValue">The minimum inclusive allowed value</param>
        /// <param name="maxValue">The minimum inclusive allowed value</param>
        public void SetRange(
            int minValue,
            int maxValue)
        {
            //validate inputs
            if (minValue >= maxValue)
                return;

            //set new range
            this.Minimum = minValue;
            this.Maximum = maxValue;

            //adjust current value upon the new range
            this.Value = this.CoerceValue(this.Value);
        }



        /// <summary>
        /// Coerces the value to the specified range
        /// </summary>
        /// <param name="value">The proposed new value</param>
        /// <returns>The coerced value, guaranteed to be within the range</returns>
        private int CoerceValue(int value)
        {
            if (value < this.Minimum)
            {
                //coerce to minimum
                return this.Minimum;
            }
            else if (value > this.Maximum)
            {
                //coerce to maximum
                return this.Maximum;
            }
            else
            {
                //allowed
                return value;
            }
        }



        #region IDisposable Members

        private bool _disposed = false;



        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }



        protected virtual void Dispose(bool disposing)
        {
            if (!this._disposed)
            {
                if (disposing)
                {
                    //place here resources to be released

                }

                this._disposed = true;
            }
        }



        ~NumericInput()
        {
            this.Dispose(false);
        }

        #endregion



        #region EVT StatusChanged

        public event StatusChangedEventHandler StatusChanged;



        protected virtual void OnStatusChanged()
        {
            var handler = this.StatusChanged;

            if (handler != null)
            {
                handler(
                    this,
                    new StatusChangedEventArgs());
            }
        }

        #endregion
    }
}
