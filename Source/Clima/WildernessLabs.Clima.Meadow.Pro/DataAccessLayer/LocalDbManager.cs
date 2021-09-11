using System;
using System.IO;
using Clima.Meadow.Pro.Models;
using Meadow;
using SQLite;

namespace Clima.Meadow.Pro.DataAccessLayer
{
    public class LocalDbManager
    {
        //==== internals
        SQLiteConnection Database { get; set; }

        //==== singleton stuff
        private static readonly Lazy<LocalDbManager> instance =
            new Lazy<LocalDbManager>(() => new LocalDbManager());
        public static LocalDbManager Instance {
            get { return instance.Value; }
        }

        private LocalDbManager()
        {
            // database files should go in the `DataDirectory`
            var databasePath = Path.Combine(MeadowOS.FileSystem.DataDirectory, "ClimateReadings.db");
            // make the connection
            Database = new SQLite.SQLiteConnection(databasePath);

            ConfigureDatabase();

            // subscribe to climate updates and save them to the database
            ClimateMonitorAgent.Instance.ClimateConditionsUpdated += (s, e) => {
                SaveUpdateReading(e);
            };
        }

        protected void ConfigureDatabase()
        {
            // add table(s)
            Database.CreateTable<Climate>();
        }

        public int SaveUpdateReading(ClimateConditions conditions)
        {
            Console.WriteLine("Saving climate reading to DB.");

            int rowID;

            // if it has an ID, update it.
            if (conditions.New?.ID is { } id) {
                Database.Update(conditions.New);
                rowID = id;
            } else { // otherwise, insert
                rowID = Database.Insert(conditions.New);
            }

            Console.WriteLine($"Successfully saved to database, row ID {rowID}");

            return rowID;
        }

        public Climate GetClimateReading(int id)
        {
            return Database.Get<Climate>(id);
        }
    }
}
