using System;
using System.Collections.Generic;
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
        }

        bool isConfigured = false;
        protected void ConfigureDatabase()
        {
            // add table(s)
            Console.WriteLine("ConfigureDatabase");
            Database.DropTable<Climate>(); //convenience while we work on the model object
            Database.CreateTable<Climate>();
            Console.WriteLine("Table created");
            isConfigured = true;
        }

        public bool SaveReading(Climate climate)
        {
            if(isConfigured == false)
            {
                Console.WriteLine("SaveUpdateReading: DB not ready");
                return false;
            }

            if(climate == null)
            {
                Console.WriteLine("SaveUpdateReading: Conditions is null");
                return false;
            }

            Console.WriteLine("Saving climate reading to DB");

            Database.Insert(climate);

            Console.WriteLine($"Successfully saved to database");

            return true;
        }

        public Climate GetClimateReading(int id)
        {
            return Database.Get<Climate>(id);
        }

        public List<Climate> GetAllClimateReadings()
        {
            return Database.Table<Climate>().ToList();
        }
    }
}
