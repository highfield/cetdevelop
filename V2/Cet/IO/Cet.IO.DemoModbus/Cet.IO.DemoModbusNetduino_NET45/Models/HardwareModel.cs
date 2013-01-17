using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Threading;


namespace Cet.IO.DemoModbusNetduino
{
    public enum Role
    {
        Master,
        Slave,
    }


    public enum CommMediumType
    {
        Tcp,
        Udp,
        Rtu,
    }


    public sealed class HardwareModel
        : INotifyPropertyChanged
    {
        #region Singleton pattern

        private HardwareModel() 
        {
            this._discretes[0] = new InputPort();
            this._discretes[1] = new InputPort();
            this._discretes[2] = new InputPort();
            this._discretes[3] = new InputPort();
            this._discretes[4] = new InputPort();
            this._discretes[5] = new InputPort();
            this._discretes[6] = new InputPort();
            this._discretes[7] = new InputPort();

            this._coils[0] = new OutputPort();
            this._coils[1] = new OutputPort();
            this._coils[2] = new OutputPort();
            this._coils[3] = new OutputPort();
            this._coils[4] = new OutputPort();
            this._coils[5] = new OutputPort();
            this._coils[6] = new OutputPort();
            this._coils[7] = new OutputPort();

            this._analogs[0] = new AnalogInput();
            this._analogs[1] = new AnalogInput();
            this._analogs[2] = new AnalogInput();
            this._analogs[3] = new AnalogInput();
            this._analogs[4] = new AnalogInput();
            this._analogs[5] = new AnalogInput();
        }

        private static HardwareModel _instance;

        public static HardwareModel Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new HardwareModel();

                return _instance;
            }
        }

        #endregion


        #region PROP Role

        private Role _role;

        public Role Role
        {
            get { return this._role; }
            set
            {
                if (this._role != value)
                {
                    this._role = value;
                    this.OnPropertyChanged("Role");
                }
            }
        }

        #endregion


        #region PROP MediumType

        private CommMediumType _mediumType;

        public CommMediumType MediumType
        {
            get { return this._mediumType; }
            set
            {
                if (this._mediumType != value)
                {
                    this._mediumType = value;
                    this.OnPropertyChanged("MediumType");
                }
            }
        }

        #endregion


        #region PROP Address

        private int _address = 1;

        public int Address
        {
            get { return this._address; }
            set
            {
                if (this._address != value)
                {
                    this._address = value;
                    this.OnPropertyChanged("Address");
                }
            }
        }

        #endregion


        #region PROP SerialSettings

        private string _serialSettings = "38400,E,8,1";

        public string SerialSettings
        {
            get { return this._serialSettings; }
            set
            {
                if (this._serialSettings != value)
                {
                    this._serialSettings = value;
                    this.OnPropertyChanged("SerialSettings");
                }
            }
        }

        #endregion


        #region PROP SerialPort

        private string _serialPort;

        public string SerialPort
        {
            get { return this._serialPort; }
            set
            {
                if (this._serialPort != value)
                {
                    this._serialPort = value;
                    this.OnPropertyChanged("SerialPort");
                }
            }
        }

        #endregion


        #region PROP NetworkIP

        private string _networkIP;

        public string NetworkIP
        {
            get { return this._networkIP; }
            set
            {
                if (this._networkIP != value)
                {
                    this._networkIP = value;
                    this.OnPropertyChanged("NetworkIP");
                }
            }
        }

        #endregion


        #region PROP NetworkPort

        private int _networkPort = 502;

        public int NetworkPort
        {
            get { return this._networkPort; }
            set
            {
                if (this._networkPort != value)
                {
                    this._networkPort = value;
                    this.OnPropertyChanged("NetworkPort");
                }
            }
        }

        #endregion


        #region PROP Analogs

        private readonly AnalogInput[] _analogs = new AnalogInput[6];

        public AnalogInput[] Analogs
        {
            get { return this._analogs; }
        }

        #endregion


        #region PROP Coils

        private readonly OutputPort[] _coils = new OutputPort[8];

        public OutputPort[] Coils
        {
            get { return this._coils; }
        }

        #endregion


        #region PROP Discretes

        private readonly InputPort[] _discretes = new InputPort[8];

        public InputPort[] Discretes
        {
            get { return this._discretes; }
        }

        #endregion


        #region PROP IsPollingEnabled

        private bool _isPollingEnabled;

        public bool IsPollingEnabled
        {
            get { return this._isPollingEnabled; }
            set
            {
                if (this._isPollingEnabled != value)
                {
                    this._isPollingEnabled = value;
                    this.OnPropertyChanged("IsPollingEnabled");
                }
            }
        }

        #endregion


        private CancellationTokenSource _cts;
        private Thread _thread;


        public void StartWorker(IWorker worker)
        {
            if (this._cts == null)
            {
                this._cts = new CancellationTokenSource();
                this._thread = new Thread(this.WorkerCore);
                this._thread.Start(worker);
            }
        }


        public void StopWorker()
        {
            if (this._cts != null)
            {
                this._cts.Cancel();
                this._thread.Join();
                this._cts = null;
            }
        }


        private void WorkerCore(object state)
        {
            var actualWorker = (IWorker)state;
            actualWorker.Run(this._cts.Token);
        }


        #region EVT PropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;


        private void OnPropertyChanged(string propertyName)
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
