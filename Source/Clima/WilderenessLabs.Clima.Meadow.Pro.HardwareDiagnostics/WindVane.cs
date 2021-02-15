using System;
using System.Collections.Generic;
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

        // TODO: consider making an `ImmutableDictionary` (need to add package
        /// <summary>
        /// Azimuth voltage lookup.
        /// </summary>
        public IDictionary<float, Azimuth> AzimuthVoltages { get; protected set; }

        protected IAnalogInputPort inputPort;

        public WindVane(IIODevice device, IPin analogInputPin, IDictionary<float, Azimuth> azimuthVoltages = null)
            : this(device.CreateAnalogInputPort(analogInputPin), azimuthVoltages)
        {
        }

        public WindVane(IAnalogInputPort inputPort, IDictionary<float, Azimuth> azimuthVoltages = null)
        {
            this.AzimuthVoltages = azimuthVoltages;
            this.inputPort = inputPort;
            this.Init();
        }

        protected void Init()
        {
            // if no lookup has been provided, load the defaults
            if (AzimuthVoltages == null) { LoadDefaultAzimuthVoltages(); }

            inputPort.Subscribe(new FilterableChangeObserver<FloatChangeResult, float>(
                result => {
                    var windAzimuth = LookupWindDirection(result.New);
                    RaiseUpdated(windAzimuth);
                    this.LastRecordedWindAzimuth = windAzimuth;
                },
                filter: null
                ));
            // TODO: consider surfacing the standby duration as a ctor argument
            inputPort.StartSampling(standbyDuration: 500);
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
            Tuple<Azimuth, double> closestFit = null;

            // loop through each azimuth lookup and compute the difference
            // between the measured voltage and the voltage for that azimumth
            double difference;
            foreach (var a in AzimuthVoltages) {
                difference = Math.Abs(a.Key - voltage);
                // if the closest fit hasn't been set or is further than the
                // computed voltage difference, then we've found a better fit.
                if (closestFit == null || closestFit.Item2 > difference) {
                    closestFit = new Tuple<Azimuth, double>(a.Value, difference);
                }
            }

            return closestFit.Item1;
        }

        protected void LoadDefaultAzimuthVoltages()
        {
            Console.WriteLine("Loading default azimuth voltages");
            this.AzimuthVoltages = new Dictionary<float, Azimuth> {
                { 2.9f, new Azimuth(Azimuth16PointCardinalNames.N) },
                { 2.04f, new Azimuth(Azimuth16PointCardinalNames.NNE) },
                { 2.19f, new Azimuth(Azimuth16PointCardinalNames.NE) },
                { 0.95f, new Azimuth(Azimuth16PointCardinalNames.ENE) },
                { 0.989f, new Azimuth(Azimuth16PointCardinalNames.E) },
                { 0.874f, new Azimuth(Azimuth16PointCardinalNames.ESE) },
                { 1.34f, new Azimuth(Azimuth16PointCardinalNames.SE) },
                { 1.12f, new Azimuth(Azimuth16PointCardinalNames.SSE) },
                { 1.689f, new Azimuth(Azimuth16PointCardinalNames.S) },
                { 1.55f, new Azimuth(Azimuth16PointCardinalNames.SSW) },
                { 2.59f, new Azimuth(Azimuth16PointCardinalNames.SW) },
                { 2.522f, new Azimuth(Azimuth16PointCardinalNames.WSW) },
                { 3.18f, new Azimuth(Azimuth16PointCardinalNames.W) },
                { 2.98f, new Azimuth(Azimuth16PointCardinalNames.WNW) },
                { 3.08f, new Azimuth(Azimuth16PointCardinalNames.NW) },
                { 2.74f, new Azimuth(Azimuth16PointCardinalNames.NNW) },
            };
        }
    }
}