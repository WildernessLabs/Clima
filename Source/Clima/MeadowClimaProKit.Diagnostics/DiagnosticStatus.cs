using System;

namespace MeadowClimaProKit.Diagnostics
{
    public class DiagnosticStatus
    {
        public bool BmeWorking { get; set; } = false;
        public bool SolarWorking { get; set; } = false;

        public bool AllWorking {
            get => BmeWorking && SolarWorking;
        }

        public DiagnosticStatus()
        {
        }
    }
}