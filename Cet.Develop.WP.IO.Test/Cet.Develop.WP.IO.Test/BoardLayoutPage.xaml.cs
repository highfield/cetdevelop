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
using Microsoft.Phone.Controls;
using System.Collections.ObjectModel;
using System.Threading;
using System.Windows.Navigation;
using System.Threading.Tasks;
using System.Windows.Threading;

using Cet.Develop.Windows.IO.Protocols;
using System.Diagnostics;


namespace Cet.Develop.Windows.IO
{
    public partial class BoardLayoutPage
        : PhoneApplicationPage
    {
        private const int InputCount = 8;
        private const int CoilCount = 6;
        private const int AnalogCount = 6;



        public BoardLayoutPage()
        {
            InitializeComponent();

            //init inputs view-model collection
            this.icInputs.ItemsSource = this._inputs;

            for (int i = 0; i < InputCount; i++)
            {
                var model = new BooleanPortModel(i, string.Empty);
                this._inputs.Insert(0, model);
            }

            //init coils (outputs) view-model collection
            this.icOutputs.ItemsSource = this._outputs;

            for (int i = 0; i < CoilCount; i++)
            {
                var descr = "Coil #" + i;
                var model = new BooleanPortModel(i, descr);
                this._outputs.Insert(0, model);
            }

            //init analogs view-model collection
            this.icAnalogs.ItemsSource = this._analogs;

            for (int i = 0; i < AnalogCount; i++)
            {
                var descr = "Analog #" + i;
                var model = new AnalogPortModel(i, descr);
                this._analogs.Insert(0, model);
            }
        }



        private ObservableCollection<BooleanPortModel> _inputs = new ObservableCollection<BooleanPortModel>();
        private ObservableCollection<BooleanPortModel> _outputs = new ObservableCollection<BooleanPortModel>();
        private ObservableCollection<AnalogPortModel> _analogs = new ObservableCollection<AnalogPortModel>();



        #region State machine and poller

        private enum MachineState
        {
            DiscreteRequest,
            DiscreteWorking,

            CoilRequest,
            CoilWorking,

            AnalogRequest,
            AnalogWorking,
        }



        private MachineState _status;



        private async void QueryDiscretes()
        {
            await TaskEx.Run(() =>
            {
                //compose the Modbus command to be submitted
                var command = new ModbusCommand(ModbusCommand.FuncReadInputDiscretes);
                command.Offset = 0;
                command.Count = InputCount;

                //execute the command synchronously
                CommResponse result = App.Modbus
                    .ExecuteGeneric(App.Client, command);

                if (result.Status == CommResponse.Ack)
                {
                    //command successfully
                    Deployment.Current.Dispatcher.BeginInvoke(new Action(() =>
                    {
                        for (int i = 0; i < command.Count; i++)
                        {
                            var input = this._inputs[i];
                            input.Value = command.Data[input.Offset] == 0;
                        }
                    }));
                }
                else
                {
                    //some error

                }

                this._status = MachineState.CoilRequest;
            });
        }



        private async void QueryCoils()
        {
            await TaskEx.Run(() =>
            {
                //compose the Modbus command to be submitted
                var command = new ModbusCommand(ModbusCommand.FuncForceMultipleCoils);
                command.Offset = 0;
                command.Count = CoilCount;
                command.Data = new ushort[CoilCount];

                for (int i = 0; i < CoilCount; i++)
                {
                    var output = this._outputs[i];
                    command.Data[output.Offset] = (ushort)(output.Value ? 1 : 0);
                }

                //execute the command synchronously
                CommResponse result = App.Modbus
                    .ExecuteGeneric(App.Client, command);

                if (result.Status == CommResponse.Ack)
                {
                    //command successfully

                }
                else
                {
                    //some error

                }

                this._status = MachineState.AnalogRequest;
            });
        }



        private async void QueryAnalogs()
        {
            await TaskEx.Run(() =>
            {
                //compose the Modbus command to be submitted
                var command = new ModbusCommand(ModbusCommand.FuncReadInputRegisters);
                command.Offset = 0;
                command.Count = AnalogCount;

                //execute the command synchronously
                CommResponse result = App.Modbus
                    .ExecuteGeneric(App.Client, command);

                if (result.Status == CommResponse.Ack)
                {
                    //command successfully
                    Deployment.Current.Dispatcher.BeginInvoke(new Action(() =>
                    {
                        for (int i = 0; i < command.Count; i++)
                        {
                            var analog = this._analogs[i];
                            analog.RawValue = command.Data[analog.Offset];
                        }
                    }));
                }
                else
                {
                    //some error

                }

                this._status = MachineState.DiscreteRequest;
            });
        }



        void ClockTick(object sender, EventArgs e)
        {
            switch (this._status)
            {
                case MachineState.DiscreteRequest:
                    this._status = MachineState.DiscreteWorking;
                    this.QueryDiscretes();
                    break;


                case MachineState.CoilRequest:
                    this._status = MachineState.CoilWorking;
                    this.QueryCoils();
                    break;


                case MachineState.AnalogRequest:
                    this._status = MachineState.AnalogWorking;
                    this.QueryAnalogs();
                    break;
            }
        }

        #endregion



        #region Timebase

        private DispatcherTimer _clock;



        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            this._status = MachineState.DiscreteRequest;
            this._clock = new DispatcherTimer();
            this._clock.Interval = TimeSpan.FromMilliseconds(500.0);
            this._clock.Tick += this.ClockTick;
            this._clock.Start();
        }



        protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            this._clock.Stop();
            this._clock.Tick -= this.ClockTick;
            this._clock = null;

            base.OnNavigatingFrom(e);
        }

        #endregion
    }
}