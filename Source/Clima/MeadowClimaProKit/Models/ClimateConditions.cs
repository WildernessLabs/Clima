using MeadowClimaProKit.Database;

namespace MeadowClimaProKit.Models
{
    public class ClimateConditions
    {
        public ClimateReading? New { get; set; }
        public ClimateReading? Old { get; set; }

        public ClimateConditions() { }
        public ClimateConditions(ClimateReading newClimate, ClimateReading oldClimate)
        {
            New = newClimate;
            Old = oldClimate;
        }
    }
}