using System;
using System.Collections.Generic;
using Meadow;
using Meadow.Hardware;
using Meadow.Foundation.Sensors;
using Meadow.Units;
using static WilderenessLabs.Clima.Meadow.WindVane;

namespace WilderenessLabs.Clima.Meadow
{
    // TODO: Need to consider how best to allow consumer to start and stop updating.
    // do we just expose the `StartUpdating` and `StopUpdating` methods on the
    // analog input port?

    /// <summary>
    /// Driver for a wind vane that outputs variable voltage, based on the
    /// azimuth of the wind. Matches the input voltage to the `AzimuthVoltages`
    /// dictionary lookup and returns the nearest azimuth to the voltage specified.
    ///
    /// By default it will use look ups that match voltage outputs from the windvane
    /// in the Sparkfun/Shenzen Fine Offset Electronics with a voltage divider of
    /// 4.7kΩ / 1kΩ, as can be found in the SparkFun weather shield, or Wilderness
    /// Labs Clima Pro board.
    /// </summary>
    public partial class WindVane : FilterableChangeObservableBase<WindVaneChangeResult, Azimuth>
    {
        /// <summary>
        /// Raised when the azimuth of the wind changes.
        /// </summary>
        public event EventHandler<WindVaneChangeResult> Updated = delegate { };

        /// <summary>
        /// The last recorded azimuth of the wind.
        /// </summary>
        public Azimuth LastRecordedWindAzimuth { get; protected set; } = 0;

        // TODO: consider making an `ImmutableDictionary` (need to add package
        /// <summary>
        /// Voltage -> wind azimuth lookup dictionary.
        /// </summary>
        public IDictionary<float, Azimuth> AzimuthVoltages { get; protected set; }

        protected IAnalogInputPort inputPort;

        /// <summary>
        /// Creates a new `WindVane` on the specified IO Device's analog input.
        /// Optionally, with a custom voltage to azimuth lookup.
        /// </summary>
        /// <param name="device">The IO Device.</param>
        /// <param name="analogInputPin">The analog input pin.</param>
        /// <param name="azimuthVoltages">Optional. Supply if you have custom azimuth voltages.</param>
        public WindVane(IIODevice device, IPin analogInputPin, IDictionary<float, Azimuth> azimuthVoltages = null)
            : this(device.CreateAnalogInputPort(analogInputPin), azimuthVoltages)
        {
        }

        /// <summary>
        /// Creates a new `WindVane` on the specified input port. Optionally,
        /// with a custom voltage to azimuth lookup.
        /// </summary>
        /// <param name="inputPort">The analog input.</param>
        /// <param name="azimuthVoltages">Optional. Supply if you have custom azimuth voltages.</param>
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

        /// <summary>
        /// Thread and inheritance safe way to raise the event and notify subs
        /// </summary>
        /// <param name="windAzimuth"></param>
        protected void RaiseUpdated(Azimuth windAzimuth)
        {
            WindVaneChangeResult result = new WindVaneChangeResult() {
                Old = this.LastRecordedWindAzimuth,
                New = windAzimuth
            };
            Updated?.Invoke(this, result);
            base.NotifyObservers(result);
        }

        /// <summary>
        /// Finds the closest wind azimuth that matches the passed in voltage,
        /// based on the `AziumuthVoltages`.
        /// </summary>
        /// <param name="voltage"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Loads a default set of voltage -> azimuth lookup values based on
        /// a 4.7kΩ / 1kΩ voltage divider.
        /// </summary>
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