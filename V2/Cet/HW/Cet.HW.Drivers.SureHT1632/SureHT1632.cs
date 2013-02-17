using System;
using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;
using SecretLabs.NETMF.Hardware.Netduino;

using Cet.HW.GDI;

/*
 * Copyright 2012, 2013 by Mario Vernari, Cet Electronics
 * Part of "Cet Open Toolbox" (http://cetdevelop.codeplex.com/)
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
namespace Cet.HW.Drivers
{
    public sealed class SureHT1632
        : ICompositionRenderer, IDisposable
    {
        private const int SystemDisable = 0x0800;
        private const int SystemEnable = 0x0802;

        private const int LedOff = 0x0804;
        private const int LedOn = 0x0806;

        private const int BlinkOff = 0x0810;
        private const int BlinkOn = 0x0812;

        private const int RcMasterMode = 0x0830;
        private const int NmosCom8 = 0x0840;
        private const int Pwm16 = 0x095E;


        private static readonly SPI.Configuration _spicfg = new SPI.Configuration(
            Pins.GPIO_NONE,    // SS-pin
            false,             // SS-pin active state (not used)
            0,                 // The setup time for the SS port (not used)
            0,                 // The hold time for the SS port (not used)
            true,              // The idle state of the clock
            true,              // The sampling clock edge
            1333,              // The SPI clock rate in KHz
            SPI_Devices.SPI1   // The used SPI bus (refers to a MOSI MISO and SCLK pinset)
        );


        public SureHT1632(
            Cpu.Pin cspin,
            Cpu.Pin clkpin)
        {
            this._htcs = new OutputPort(cspin, true);
            this._htclk = new OutputPort(clkpin, false);

            this._sck = new TristatePort(
                Pins.GPIO_PIN_D13,
                false,
                false,
                Port.ResistorMode.Disabled);

            this._sda = new TristatePort(
                Pins.GPIO_PIN_D11,
                false,
                false,
                Port.ResistorMode.Disabled);
        }


        private OutputPort _htcs;
        private OutputPort _htclk;
        private TristatePort _sck;
        private TristatePort _sda;

        private readonly byte[] _rawR = new byte[64];
        private readonly byte[] _rawG = new byte[64];
        private byte[] _spiBuffer;
        private int _lastHash;

#if false
        public void Dump(CompositionTargetSureHT1632 target)
        {
            if (this._spiBuffer == null)
            {
                this.Init();
            }

            int hash = target.GetCacheHash();
            if (hash == this._lastHash)
                return;

            this._lastHash = hash;
            target.GetBuffers(
                this._rawR,
                this._rawG);

            using (var spi = new SPI(_spicfg))
            {
                this._htcs.Write(false);

                for (int i = 0; i < 64; i += 16)
                {
                    Array.Copy(
                        this._rawG,
                        i,
                        this._spiBuffer,
                        1,
                        16);

                    Array.Copy(
                        this._rawR,
                        i,
                        this._spiBuffer,
                        17,
                        16);

                    this._htclk.Write(false);
                    this._htclk.Write(true);

                    this._sck.Active = true;
                    this._sck.Active = false;

                    this._sck.Active = true;
                    this._sda.Active = true;
                    this._sck.Active = false;
                    this._sda.Active = false;

                    spi.Write(this._spiBuffer);

                    this._htcs.Write(true);
                }
            }

            this._htclk.Write(false);
            this._htclk.Write(true);
        }
#endif

        private void Init()
        {
            HtCommand(SystemEnable);
            HtCommand(LedOn);
            HtCommand(BlinkOff);
            HtCommand(RcMasterMode);
            HtCommand(NmosCom8);
            HtCommand(Pwm16);

            this._spiBuffer = new byte[33];
            this._spiBuffer[0] = 0x80;
        }


        private void HtCommand(int command)
        {
            this._htcs.Write(false);
            this._htclk.Write(false);
            this._htclk.Write(true);
            this._htclk.Write(false);
            this._htclk.Write(true);
            this._htclk.Write(false);
            this._htclk.Write(true);
            this._htclk.Write(false);
            this._htclk.Write(true);

            bool old = this._sda.Active;
            bool active = old;
            for (int k = 0x800; k > 0; k >>= 1)
            {
                active = (command & k) == 0;

                this._sck.Active = true;
                if (active != old)
                {
                    this._sda.Active = (old = active);
                }

                this._sck.Active = false;
            }

            if (active)
                this._sda.Active = false;

            this._htcs.Write(true);
            this._htclk.Write(false);
            this._htclk.Write(true);
            this._htclk.Write(false);
            this._htclk.Write(true);
            this._htclk.Write(false);
            this._htclk.Write(true);
            this._htclk.Write(false);
            this._htclk.Write(true);
        }


        public void Dispose()
        {
            this._htcs.Dispose();
            this._htclk.Dispose();

            if (this._sck != null)
                this._sck.Dispose();

            if (this._sda != null)
                this._sda.Dispose();
        }


        #region ICompositionRenderer members

        CompositionTargetBase ICompositionRenderer.CreateTarget()
        {
            return new CompositionTargetSureHT1632();
        }


        void ICompositionRenderer.Dump(CompositionTargetBase composition)
        {
            //checks for driver initialization
            if (this._spiBuffer == null)
            {
                this.Init();
            }

            //checks whether something in the cache has changed
            var target = composition as CompositionTargetSureHT1632;
            int hash = target.GetCacheHash();
            if (hash == this._lastHash)
                return;
            else
                this._lastHash = hash;

            //retrieve the R+G buffers
            target.GetBuffers(
                this._rawR,
                this._rawG);

            //flushes the data out to the physical device
            using (var spi = new SPI(_spicfg))
            {
                this._htcs.Write(false);

                for (int i = 0; i < 64; i += 16)
                {
                    Array.Copy(
                        this._rawG,
                        i,
                        this._spiBuffer,
                        1,
                        16);

                    Array.Copy(
                        this._rawR,
                        i,
                        this._spiBuffer,
                        17,
                        16);

                    this._htclk.Write(false);
                    this._htclk.Write(true);

                    this._sck.Active = true;
                    this._sck.Active = false;

                    this._sck.Active = true;
                    this._sda.Active = true;
                    this._sck.Active = false;
                    this._sda.Active = false;

                    spi.Write(this._spiBuffer);

                    this._htcs.Write(true);
                }
            }

            this._htclk.Write(false);
            this._htclk.Write(true);
        }

        #endregion

    }
}
