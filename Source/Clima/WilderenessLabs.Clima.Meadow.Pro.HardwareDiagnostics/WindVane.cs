using System;
using Meadow;
using Meadow.Hardware;
using Meadow.Foundation.Sensors;
using Meadow.Units;
using static WilderenessLabs.Clima.Meadow.WindVane;

namespace WilderenessLabs.Clima.Meadow
{
    public partial class WindVane : FilterableChangeObservableBase<WindVaneChangeResult, Azimuth>
    {
        public event EventHandler<WindVaneChangeResult> Updated = delegate { };

        public Azimuth LastRecordedWindAzimuth { get; protected set; } = 0;

        protected IAnalogInputPort inputPort;

        public WindVane(IIODevice device, IPin analogInputPin)
            : this(device.CreateAnalogInputPort(analogInputPin))
        {
        }

        public WindVane(IAnalogInputPort inputPort)
        {
            this.inputPort = inputPort;
            this.Init();
        }

        protected void Init()
        {
            inputPort.Subscribe(new FilterableChangeObserver<FloatChangeResult, float>(
                result => {
                    var windAzimuth = LookupWindDirection(result.New);
                    RaiseUpdated(windAzimuth);
                    this.LastRecordedWindAzimuth = windAzimuth;
                },
                filter: null
                ));
        }


        protected void RaiseUpdated(Azimuth windAzimuth)
        {
            WindVaneChangeResult result = new WindVaneChangeResult() {
                Old = this.LastRecordedWindAzimuth,
                New = windAzimuth
            };

            Console.WriteLine($"1) Result.Old: {result.Old}, New: {result.New}");
            Console.WriteLine($"2) Delta: {result.Delta}");

            Updated?.Invoke(this, result);
            base.NotifyObservers(result);
        }

        protected Azimuth LookupWindDirection(float voltage)
        {
            return new Azimuth(Azimuth16PointCardinalNames.E);
        }

    }
}
