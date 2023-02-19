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
        private const int MAX_PAGE_SIZE = 100;

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
            //DeleteDatabaseFile("ClimateReadings.db");

            var databasePath = Path.Combine(MeadowOS.FileSystem.DataDirectory, "ClimateReadings.db");
            var isNewDatabase = !File.Exists(databasePath);

            Database = new SQLiteConnection(databasePath);

            if (isNewDatabase)
            {
                // Create the tables we need
                Console.WriteLine("DatabaseManager: Create the tables we need");
                Database.DropTable<TemperatureTable>(); //convenience while we work on the model object
                Database.CreateTable<TemperatureTable>();
            }

            isConfigured = true;
        }

        protected void DeleteDatabaseFile(string fileName)
        {
            string dbFile = Path.Combine(MeadowOS.FileSystem.DataDirectory, fileName);
            if (File.Exists(dbFile))
            {
                File.Delete(dbFile);
            }
            dbFile += "-journal";
            if (File.Exists(dbFile))
            {
                File.Delete(dbFile);
            }
        }

        public bool SaveReading(TemperatureTable temperature)
        {
            LedController.Instance.SetColor(WildernessLabsColors.ChileanFireDark);

            if (isConfigured == false)
            {
                Console.WriteLine("SaveReading: DB not ready");
                return false;
            }

            if (temperature == null)
            {
                Console.WriteLine("SaveReading: Conditions is null");
                return false;
            }
            Console.WriteLine("SaveReading: Saving temperature reading to DB");

            Database.Insert(temperature);

            Console.WriteLine("SaveReading: Successfully saved to database");
            Console.WriteLine($"SaveReading: Count={GetCountReadings()} GetTotalMemory={GC.GetTotalMemory(true)}");
            LedController.Instance.SetColor(Color.Green);
            return true;
        }

        public List<TemperatureTable> GetAllTemperatureReadings()
        {
            int TotalCount = GetCountReadings();
            Console.WriteLine($"GetAllReadings:  TotalCount={TotalCount} ...");
            if (TotalCount > MAX_PAGE_SIZE)
            {
                return GetTemperatureReadings(MAX_PAGE_SIZE, 1); // return latest 100 readings to avoid memory overflow
            }
            return Database.Table<TemperatureTable>().ToList();
        }
        public List<TemperatureTable> GetTemperatureReadings(int pageSize, int pageNumber = 1)
        {
            int TotalCount = GetCountReadings();
            int PageSize = pageSize <= MAX_PAGE_SIZE ? pageSize : MAX_PAGE_SIZE; 
            int TotalPages = (int)Math.Ceiling(TotalCount / (double)pageSize);

            if (pageNumber <= 0)
            {
                pageNumber = 1;
            }
            if (pageNumber > TotalPages)
            {
                pageNumber = TotalPages;
            }
            int CurrentPage = pageNumber;

            Console.WriteLine($"GetReadings:  TotalCount={TotalCount} TotalPages={TotalPages} PageSize={PageSize} CurrentPage={CurrentPage} ");
            if (TotalCount < PageSize)
            {
                return Database.Table<TemperatureTable>().ToList();
            }
            //var items = Database.Table<TemperatureTable>().Take(pageSize).OrderByDescending(t => t.ID).ToList();
            //var items = Database.Table<TemperatureTable>().Skip(TotalCount - (CurrentPage * pageSize)).Take(pageSize).OrderByDescending(t => t.ID)..ToList();
            var items = Database.Table<TemperatureTable>().Skip((CurrentPage - 1) * pageSize).Take(pageSize).OrderByDescending(t => t.ID).ToList();
            return items;
        }
        public int GetCountReadings()
        {
            return Database.Table<TemperatureTable>().Count();
        }
    }
}