using Meadow;
using SQLite;
using System;
using System.Collections.Generic;
using System.IO;

namespace Clima_SQLite_Demo.Database
{
    public class DatabaseManager
    {
        private static readonly Lazy<DatabaseManager> instance =
            new Lazy<DatabaseManager>(() => new DatabaseManager());
        public static DatabaseManager Instance => instance.Value;

        bool isConfigured = false;

        SQLiteConnection Database { get; set; }

        private DatabaseManager()
        {
            Initialize();
        }

        protected void Initialize()
        {
            var databasePath = Path.Combine(MeadowOS.FileSystem.DataDirectory, "ClimateReadings.db");
            Database = new SQLiteConnection(databasePath);

            Database.DropTable<ClimateReading>(); //convenience while we work on the model object
            Database.CreateTable<ClimateReading>();
            isConfigured = true;
        }

        public bool SaveReading(ClimateReading climate)
        {
            if (isConfigured == false)
            {
                Console.WriteLine("SaveUpdateReading: DB not ready");
                return false;
            }

            if (climate == null)
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