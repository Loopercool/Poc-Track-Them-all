using SQLite;
using MyMauiApp.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Diagnostics;

namespace MyMauiApp.Services
{
    public class AppDatabase
    {
        private readonly SQLiteAsyncConnection _database;

        public AppDatabase(string dbPath)
        {
            _database = new SQLiteAsyncConnection(dbPath);
            // Create the table if it doesn't exist.
            _database.CreateTableAsync<LocationRecord>().Wait();
        }

        public async Task<int> SaveLocationAsync(LocationRecord record)
        {
            int result = await _database.InsertAsync(record);
            Debug.WriteLine($"[AppDatabase] Inserted record with result: {result}");
            return result;
        }

        public Task<List<LocationRecord>> GetLocationsAsync() =>
            _database.Table<LocationRecord>().ToListAsync();
    }
}
