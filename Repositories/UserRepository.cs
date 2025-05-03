using MiniProjet.Models;
using MongoDB.Driver;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace MiniProjet.Repositories
{
    public class UserRepository
    {
        private readonly IMongoCollection<User> _users;

        public UserRepository(IMongoClient mongoClient)
        {
            var database = mongoClient.GetDatabase("MiniProjet");
            _users = database.GetCollection<User>("Users");
        }

        public async Task<List<User>> GetAllAsync() => await _users.Find(user => true).ToListAsync();

        public async Task<User> GetByIdAsync(string id) => await _users.Find(user => user.Id == id).FirstOrDefaultAsync();

        public async Task<User> GetByUsernameAsync(string username) =>
            await _users.Find(user => user.Username == username).FirstOrDefaultAsync();

        public async Task CreateAsync(User user) => await _users.InsertOneAsync(user);

        public async Task<bool> UpdateAsync(string id, User user)
        {
            var result = await _users.ReplaceOneAsync(u => u.Id == id, user);
            return result.ModifiedCount > 0;
        }

        public async Task DeleteAsync(string id) =>
            await _users.DeleteOneAsync(user => user.Id == id);

        public async Task<List<User>> GetUtilisateursBannisAsync()
        {
            return await _users.Find(u => u.DateFinBannissement != null && u.DateFinBannissement > DateTime.UtcNow).ToListAsync();
        }

       public async Task<List<User>> GetManyByIdsAsync(List<string> ids)
        {
            if (ids == null || ids.Count == 0) return new List<User>();
            return await _users.Find(u => ids.Contains(u.Id)).ToListAsync();
        }



        internal async Task<List<User>> GetUtilisateursNonBannisAsync()
        {
            return await _users.Find(u => u.DateFinBannissement == null).ToListAsync();
        }
    }


}
