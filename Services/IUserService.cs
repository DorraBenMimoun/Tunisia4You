using System.Collections.Generic;
using System.Threading.Tasks;
using MiniProjet.Models; // Importation du modèle User
using MongoDB.Driver;
using MongoDB.Bson;

namespace MiniProjet.Services
{
    public interface IUserService
    {
        // Récupérer tous les utilisateurs
        Task<IEnumerable<User>> GetAllUsersAsync();

        // Récupérer un utilisateur par son ID
        Task<User> GetUserByIdAsync(string id);

        // Créer un utilisateur
        Task<User> CreateUserAsync(User user);

        // Mettre à jour un utilisateur
        Task<bool> UpdateUserAsync(string id, User user);

        // Supprimer un utilisateur
        Task<bool> DeleteUserAsync(ObjectId objectId);
    }
}
