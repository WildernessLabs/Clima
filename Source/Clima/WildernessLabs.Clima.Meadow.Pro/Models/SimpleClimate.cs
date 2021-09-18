using System;
using Meadow.Units;
using SQLite;

namespace Clima.Meadow.Pro.Models
{
    [Table("SimpleClimateReadings")]
    public class SimpleClimate
    {
        [PrimaryKey, AutoIncrement]
        public int ID { get; set; }
       // public MyCoolClass CoolClass{ get; set; }

        //  public double DoubleValue { get; set; }
        //   public DateTime DateTimeValue { get; set; }

        [Ignore]
        public Temperature Temperature { get; set; }
    }

    public class MyCoolClass
    {
        public double member1;
    }


    [Serializable()]
    public struct MyCoolType
    {
        public int member1;
        public string member2;
        public string member3;
        public double member4;

        // A field that is not serialized.
        [NonSerialized()]
        public string member5;

        public MyCoolType(int mem1, string mem2, string mem3, double mem4)
        {
            member1 = 11;
            member2 = "hello";
            member3 = "hello";
            member4 = 3.14159265;
            member5 = "hello world!";
        }

        public void Print()
        {

            Console.WriteLine("member1 = '{0}'", member1);
            Console.WriteLine("member2 = '{0}'", member2);
            Console.WriteLine("member3 = '{0}'", member3);
            Console.WriteLine("member4 = '{0}'", member4);
            Console.WriteLine("member5 = '{0}'", member5);
        }
    }
}
