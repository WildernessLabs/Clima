namespace MeadowClimaProKit.Diagnostics
{
    public class DiagnosticStatus
    {        
        public bool SolarWorking { get; set; } = false;

        public bool AllWorking 
        {
            get => SolarWorking;
        }

        public DiagnosticStatus()
        {
        }
    }
}