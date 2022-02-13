using Meadow;
using Meadow.Foundation;
using MeadowClimaHackKit.Controller;
using SQLite;
using System;
using System.Collections.Generic;
using System.IO;

namespace MeadowClimaHackKit.Database
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

            Database.DropTable<TemperatureTable>(); //convenience while we work on the model object
            Database.CreateTable<TemperatureTable>();
            isConfigured = true;
        }

        public bool SaveReading(TemperatureTable temperature)
        {
            LedController.Instance.SetColor(WildernessLabsColors.ChileanFireDark);

            if (isConfigured == false)
            {
                Console.WriteLine("SaveUpdateReading: DB not ready");
                return false;
            }

            if (temperature == null)
            {
                Console.WriteLine("SaveUpdateReading: Conditions is null");
                return false;
            }

            Database.Insert(temperature);

            LedController.Instance.SetColor(Color.Green);
            return true;
        }

        public List<TemperatureTable> GetTemperatureReadings()
        {
            return Database.Table<TemperatureTable>().ToList();
        }
    }
}