using System.Collections.Generic;
using System.Threading.Tasks;
using MiniProjet.Models;
using MongoDB.Bson;
using MongoDB.Driver;
using BCrypt.Net;

namespace MiniProjet.Services
{
    public class UserService : IUserService
    {
        private readonly IMongoCollection<User> _usersCollection; // Référence à la collection MongoDB

        // Injection de la base de données via le constructeur
        public UserService(IMongoDatabase database)
        {
            _usersCollection = database.GetCollection<User>("Users"); // Accède à la collection "Users"
        }

        // 🔹 Récupérer tous les utilisateurs
        public async Task<IEnumerable<User>> GetAllUsersAsync()
        {
            return await _usersCollection.Find(user => true).ToListAsync();
        }

        // 🔹 Récupérer un utilisateur par ID
        public async Task<User> GetUserByIdAsync(string id)
        {
            return await _usersCollection.Find(user => user.Id == id).FirstOrDefaultAsync();
        }

        // 🔹 Créer un utilisateur
        public async Task<User> CreateUserAsync(User user)
        {
            await _usersCollection.InsertOneAsync(user);
            return user;
        }

        // 🔹 Mettre à jour un utilisateur
        public async Task<bool> UpdateUserAsync(string id, User user)
        {
            if (!ObjectId.TryParse(id, out ObjectId objectId)) // Validation correcte de l'ID
            {
                throw new FormatException("L'ID fourni n'est pas un ObjectId valide.");
            }

            var existingUser = await _usersCollection.Find(u => u.Id == objectId.ToString()).FirstOrDefaultAsync();

            if (existingUser == null)
            {
                return false; // L'utilisateur n'existe pas
            }

            var updateDefinition = new List<UpdateDefinition<User>>();

            // Mise à jour des champs uniquement s'ils sont fournis
            if (!string.IsNullOrWhiteSpace(user.Username))
            {
                updateDefinition.Add(Builders<User>.Update.Set(u => u.Username, user.Username));
            }
            if (!string.IsNullOrWhiteSpace(user.Email))
            {
                updateDefinition.Add(Builders<User>.Update.Set(u => u.Email, user.Email));
            }
            if (user.IsAdmin != existingUser.IsAdmin) // Vérifier si l'admin a changé
            {
                updateDefinition.Add(Builders<User>.Update.Set(u => u.IsAdmin, user.IsAdmin));
            }
            if (!string.IsNullOrWhiteSpace(user.PasswordHash))
            {
                string hashedPassword = HashPassword(user.PasswordHash);

                updateDefinition.Add(Builders<User>.Update.Set(u => u.PasswordHash, hashedPassword));
            }

            // Vérifier s'il y a des modifications à faire
            if (updateDefinition.Count == 0)
            {
                return false; // Rien à mettre à jour
            }

            var updateResult = await _usersCollection.UpdateOneAsync(
                Builders<User>.Filter.Eq(u => u.Id, objectId.ToString()),
                Builders<User>.Update.Combine(updateDefinition)
            );

            return updateResult.ModifiedCount > 0;
        }

        // Fonction de hashage du mot de passe avec BCrypt
        private string HashPassword(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password);
        }


        // 🔹 Supprimer un utilisateur
        public async Task<bool> DeleteUserAsync(ObjectId objectId)
        {
            var result = await _usersCollection.DeleteOneAsync(user => user.Id == objectId.ToString());

            return result.DeletedCount > 0;
        }


    }
}
