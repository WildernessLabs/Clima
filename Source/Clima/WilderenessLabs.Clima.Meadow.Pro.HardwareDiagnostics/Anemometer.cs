using System;
using Meadow;
using Meadow.Foundation.Sensors;
using Meadow.Hardware;
using static WilderenessLabs.Clima.Meadow.Anemometer;

namespace WilderenessLabs.Clima.Meadow
{
    public partial class Anemometer : FilterableChangeObservableBase<AnemometerChangeResult, float>
    {
        public event EventHandler<AnemometerChangeResult> SpeedUpdated = delegate {};

        public float LastRecordedWindSpeed { get; protected set; } = 0f;

        /// <summary>
        /// 
        /// </summary>
        public float KmhPerSwitchPerSecond { get; set; } = 2.4f;

        IDigitalInputPort inputPort;

        public Anemometer(IIODevice device, IPin digitalInputPin)
            : this(device.CreateDigitalInputPort(
                digitalInputPin, InterruptMode.EdgeFalling,
                ResistorMode.InternalPullUp, 20, 20))
        {
        }

        public Anemometer(IDigitalInputPort inputPort)
        {
            this.inputPort = inputPort;
            this.Init();
        }

        protected void Init()
        {
            inputPort.Subscribe(new FilterableChangeObserver<DigitalInputPortEventArgs, DateTime>(
                result => {
                    float newSpeed = SwitchIntervalToKmh(result.Delta);
                    RaiseWindSpeedChangedEvent(newSpeed);
                    this.LastRecordedWindSpeed = newSpeed;
                },
                filter: null
                ));
        }

        protected void RaiseWindSpeedChangedEvent(float newSpeed)
        {
            AnemometerChangeResult result = new AnemometerChangeResult() {
                Old = this.LastRecordedWindSpeed,
                New = newSpeed
            };

            SpeedUpdated?.Invoke(this, result);
            base.NotifyObservers(result);
        }

        /// <summary>
        /// A wind speed of 2.4km/h causes the switch to close once per second.
        /// </summary>
        /// <param name="interval"></param>
        /// <returns></returns>
        protected float SwitchIntervalToKmh(TimeSpan interval)
        {
            // A wind speed of 2.4km/h causes the switch to close once per second.
            return this.KmhPerSwitchPerSecond / ((float)interval.Milliseconds / 1000f);
        }
    }
}
