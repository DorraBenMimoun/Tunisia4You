using MiniProjet.Models;
using MiniProjet.Repositories;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MiniProjet.Services
{
    public class ListeService
    {
        private readonly ListeRepository _listeRepository;

        public ListeService(ListeRepository listeRepository)
        {
            _listeRepository = listeRepository;
        }

        // Récupérer toutes les listes
        public async Task<List<Liste>> GetAllAsync()
        {
            return await _listeRepository.GetAllAsync();
        }

        // Récupérer une liste par son ID
        public async Task<Liste> GetByIdAsync(string id)
        {
            return await _listeRepository.GetByIdAsync(id);
        }

        // Ajouter une nouvelle liste
        public async Task CreateAsync(Liste liste)
        {
            await _listeRepository.CreateAsync(liste);
        }

        // Mettre à jour une liste existante
        public async Task UpdateAsync(string id, Liste liste)
        {
            await _listeRepository.UpdateAsync(id, liste);
        }

        // Supprimer une liste par son ID
        public async Task DeleteAsync(string id)
        {
            await _listeRepository.DeleteAsync(id);
        }

        // Récupérer les listes créées par un utilisateur donné
        public async Task<List<Liste>> GetByCreateurIdAsync(string createurId)
        {
            return await _listeRepository.GetByCreateurIdAsync(createurId);
        }

        // Récupérer les listes publiques
        public async Task<List<Liste>> GetPublicAsync()
        {
            return await _listeRepository.GetPublicAsync();
        }

        // Rendre une liste publique
        public async Task MakePublicAsync(string id)
        {
            await _listeRepository.MakePublicAsync(id);
        }

        // Rendre une liste privée
        public async Task MakePrivateAsync(string id)
        {
            await _listeRepository.MakePrivateAsync(id);
        }

        // Ajouter un lieu à une liste
        public async Task AddPlaceAsync(string listeId, string placeId)
        {
            await _listeRepository.AddPlaceAsync(listeId, placeId);
        }

        // Supprimer un lieu d'une liste
        public async Task RemovePlaceAsync(string listeId, string placeId)
        {
            await _listeRepository.RemovePlaceAsync(listeId, placeId);
        }

        // Récupérer les lieux d'une liste
        public async Task<List<Place>> GetPlacesAsync(string listeId)
        {
            return await _listeRepository.GetPlacesAsync(listeId);
        }

        // Recuperer liste par nom
        public async Task<List<Liste>> GetByNameAsync(string name)
        {
            return await _listeRepository.GetByNameAsync(name);
        }
    }
}
