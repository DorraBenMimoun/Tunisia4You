using MiniProjet.DTOs;
using MiniProjet.Models;
using MiniProjet.Repositories;
using MongoDB.Driver;
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
        public async Task<Liste> CreateAsync(CreateListeDTO dto)
        {
            var liste = new Liste
            {
                Nom = dto.Nom,
                Description = dto.Description,
                IsPrivate = dto.IsPrivate,
                CreateurId = dto.CreateurId,
                LieuxIds = dto.LieuxIds ?? new List<string>(),
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            await _listeRepository.CreateAsync(liste);
            return liste;
        }



        // Mettre à jour une liste existante
        public async Task<bool> UpdateAsync(string id, UpdateListeDTO dto)
        {
            var existingListe = await _listeRepository.GetByIdAsync(id);
            if (existingListe == null) return false;

            existingListe.Nom = dto.Nom ?? existingListe.Nom;
            existingListe.Description = dto.Description ?? existingListe.Description;
            existingListe.IsPrivate = dto.IsPrivate ;
            existingListe.LieuxIds = dto.LieuxIds ?? existingListe.LieuxIds;
            existingListe.UpdatedAt = DateTime.UtcNow;

            await _listeRepository.UpdateAsync(id, existingListe);
            return true;
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

        public async Task RemovePlaceFromAllListsAsync(string placeId)
        {
            var update = Builders<Liste>.Update.Pull(l => l.LieuxIds, placeId);
            await _listeRepository.UpdateManyAsync(update);
        }

    }
}
