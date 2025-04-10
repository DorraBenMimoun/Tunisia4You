using System.Collections.Generic;
using System.Threading.Tasks;
using MiniProjet.Models;
using MongoDB.Bson;
using MongoDB.Driver;
using BCrypt.Net;
using MiniProjet.Repositories;
using MiniProjet.DTOs;

namespace MiniProjet.Services
{
    public class UserService 
    {
        private readonly IMongoCollection<User> _usersCollection; // Référence à la collection MongoDB
        private readonly UserRepository _userRepository;

        // Injection de la base de données via le constructeur
        public UserService(IMongoDatabase database, UserRepository userRepository)
        {
            _usersCollection = database.GetCollection<User>("Users"); // Accède à la collection "Users"
            _userRepository = userRepository;
        }

        // 🔹 Récupérer tous les utilisateurs
        public async Task<IEnumerable<User>> GetAllUsersAsync()
        {
            return await _userRepository.GetAllAsync();
        }

        // 🔹 Récupérer un utilisateur par ID
        public async Task<User> GetUserByIdAsync(string id)
        {
            return await _userRepository.GetByIdAsync(id);
        }

        // 🔹 Créer un utilisateur
        public async Task<User> CreateUserAsync(User user)
        {
            await _userRepository.CreateAsync(user);
            return user;
        }
        public async Task<User> CreateUserAsync(CreateUserDTO dto)
        {
            var user = new User
            {
                Username = dto.Username,
                Email = dto.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.PasswordHash),
                IsAdmin = false,
                CreatedAt = DateTime.UtcNow
            };

            await _userRepository.CreateAsync(user);
            return user;
        }


        // 🔹 Mettre à jour un utilisateur
        public async Task<bool> UpdateUserAsync(string id, UpdateUserDTO dto)
        {
            var existingUser = await _userRepository.GetByIdAsync(id);
            if (existingUser == null) return false;

            existingUser.Username = dto.Username ?? existingUser.Username;
            existingUser.Email = dto.Email ?? existingUser.Email;
            if (!string.IsNullOrEmpty(dto.PasswordHash))
            {
                existingUser.PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.PasswordHash);
            }
            existingUser.IsAdmin = dto.IsAdmin ?? existingUser.IsAdmin;

            await _userRepository.UpdateAsync(id, existingUser);
            return true;
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

        public async Task<User> GetUserByEmailAsync(string email)
        {
            return await _usersCollection.Find(u => u.Email == email).FirstOrDefaultAsync();
        }

        public async Task SetResetTokenAsync(User user, string token)
        {
            var update = Builders<User>.Update
                .Set(u => u.ResetPasswordToken, token)
                .Set(u => u.ResetPasswordTokenExpires, DateTime.UtcNow.AddHours(1));

            await _usersCollection.UpdateOneAsync(u => u.Id == user.Id, update);
        }

        public async Task<User> GetUserByResetTokenAsync(string token)
        {
            return await _usersCollection.Find(u =>
                u.ResetPasswordToken == token &&
                u.ResetPasswordTokenExpires > DateTime.UtcNow
            ).FirstOrDefaultAsync();
        }

        public async Task ResetPasswordAsync(User user, string newPassword)
        {
            var update = Builders<User>.Update
                .Set(u => u.PasswordHash, BCrypt.Net.BCrypt.HashPassword(newPassword))
                .Set(u => u.ResetPasswordToken, null)
                .Set(u => u.ResetPasswordTokenExpires, null);

            await _usersCollection.UpdateOneAsync(u => u.Id == user.Id, update);
        }

    }
}
