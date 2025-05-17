using MongoDB.Driver;
using MiniProjet.Models;

namespace MiniProjet.Services
{
    public class PreferencesService
    {
        private readonly IMongoCollection<Preferences> _preferences;

        public PreferencesService(IMongoDatabase database)
        {
            _preferences = database.GetCollection<Preferences>("preferences");
        }

        public async Task<Preferences?> GetUserPreferences(string userId)
        {
            return await _preferences.Find(p => p.UserId == userId).FirstOrDefaultAsync();
        }

        public async Task<Preferences> CreateOrUpdatePreferences(Preferences preferences)
        {
            var existingPreferences = await GetUserPreferences(preferences.UserId);
            
            if (existingPreferences != null)
            {
                preferences.Id = existingPreferences.Id;
                await _preferences.ReplaceOneAsync(p => p.Id == existingPreferences.Id, preferences);
                return preferences;
            }

            await _preferences.InsertOneAsync(preferences);
            return preferences;
        }
    }
} 