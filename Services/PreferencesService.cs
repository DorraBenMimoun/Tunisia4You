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

        public async Task<Preferences> CreateOrUpdatePreferences(Preferences preferences, string userId)
        {
            var existingPreferences = await GetUserPreferences(userId);
            preferences.UserId = userId; // On met l'id du user connecté 
            
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