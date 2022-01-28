using System;
using System.Collections.Generic;
using System.IO;
using MeadowClimaProKit.Models;
using Meadow;
using SQLite;
using MeadowClimaProKit.Database;

namespace MeadowClimaProKit.DataAccessLayer
{
    public class DatabaseManager
    {
        //==== internals
        SQLiteConnection Database { get; set; }

        //==== singleton stuff
        private static readonly Lazy<DatabaseManager> instance =
            new Lazy<DatabaseManager>(() => new DatabaseManager());
        public static DatabaseManager Instance {
            get { return instance.Value; }
        }

        private DatabaseManager()
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
            Database.DropTable<ClimateReading>(); //convenience while we work on the model object
            Database.CreateTable<ClimateReading>();
            Console.WriteLine("Table created");
            isConfigured = true;
        }

        public bool SaveReading(ClimateReading climate)
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

        public ClimateReading GetClimateReading(int id)
        {
            return Database.Get<ClimateReading>(id);
        }

        public List<ClimateReading> GetAllClimateReadings()
        {
            return Database.Table<ClimateReading>().ToList();
        }
    }
}
