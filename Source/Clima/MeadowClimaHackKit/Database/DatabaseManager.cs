using Meadow;
using MeadowClimaHackKit.Controller;
using SQLite;
using System;
using System.Collections.Generic;
using Meadow.Foundation;
using System.IO;

namespace MeadowClimaHackKit.Database
{
    public class DatabaseManager
    {
        bool isConfigured = false;

        SQLiteConnection Database { get; set; }

        private static readonly Lazy<DatabaseManager> instance =
            new Lazy<DatabaseManager>(() => new DatabaseManager());
        public static DatabaseManager Instance => instance.Value;

        private DatabaseManager()
        {
            Initialize();
        }

        protected void Initialize()
        {
            var databasePath = Path.Combine(MeadowOS.FileSystem.DataDirectory, "ClimateReadings.db");
            Database = new SQLiteConnection(databasePath);

            Database.DropTable<TemperatureTable>();
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