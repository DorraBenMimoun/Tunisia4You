using MiniProjet.Models;
using MongoDB.Driver;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace MiniProjet.Repositories
{
    public class UserRepository : IRepository<User>
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

        public async Task UpdateAsync(string id, User user) =>
            await _users.ReplaceOneAsync(u => u.Id == id, user);

        public async Task DeleteAsync(string id) =>
            await _users.DeleteOneAsync(user => user.Id == id);
    }
}
