
using MiniProjet.Models;
using MongoDB.Bson;
using MongoDB.Driver;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MiniProjet.Repositories
{

    public class ListeRepository
    {
        private readonly IMongoCollection<Liste> _listes;

        public ListeRepository(IMongoClient mongoClient)
        {
            var database = mongoClient.GetDatabase("MiniProjet");

            _listes = database.GetCollection<Liste>("Listes");
        }

        // Récupérer toutes les listes
        public async Task<List<Liste>> GetAllAsync()
        {
            return await _listes.Find(l => true).ToListAsync();
        }

        // Récupérer une liste par son ID
        public async Task<Liste> GetByIdAsync(string id)
        {
            return await _listes.Find(l => l.Id == id).FirstOrDefaultAsync();
        }

        // Ajouter une nouvelle liste
        public async Task CreateAsync(Liste liste)
        {
            await _listes.InsertOneAsync(liste);
        }

        // Mettre à jour une liste existante
        public async Task UpdateAsync(string id, Liste liste)
        {
            await _listes.ReplaceOneAsync(l => l.Id == id, liste);
        }

        // Supprimer une liste par son ID
        public async Task DeleteAsync(string id)
        {
            await _listes.DeleteOneAsync(l => l.Id == id);
        }

        // Récupérer les listes créées par un utilisateur donné
        public async Task<List<Liste>> GetByCreateurIdAsync(string createurId)
        {
            return await _listes.Find(l => l.CreateurId == createurId).ToListAsync();
        }

        //Récupérer les listes publiques
        public async Task<List<Liste>> GetPublicAsync()
        {
            return await _listes.Find(l => l.IsPrivate == false).ToListAsync();
        }

        //Rendre une liste publique
        public async Task MakePublicAsync(string id)
        {
            var update = Builders<Liste>.Update.Set(l => l.IsPrivate, false);
            await _listes.UpdateOneAsync(l => l.Id == id, update);
        }

        //Rendre une liste privée
        public async Task MakePrivateAsync(string id)
        {
            var update = Builders<Liste>.Update.Set(l => l.IsPrivate, true);
            await _listes.UpdateOneAsync(l => l.Id == id, update);
        }

        // Ajouter place à une liste
        public async Task AddPlaceAsync(string listeId, string placeId)
        {
            var update = Builders<Liste>.Update.Push(l => l.LieuxIds, placeId);
            await _listes.UpdateOneAsync(l => l.Id == listeId, update);
        }

        // Supprimer place d'une liste
        public async Task RemovePlaceAsync(string listeId, string placeId)
        {
            var update = Builders<Liste>.Update.Pull(l => l.LieuxIds, placeId);
            await _listes.UpdateOneAsync(l => l.Id == listeId, update);
        }

        // Récupérer les lieux d'une liste
        public async Task<List<Place>> GetPlacesAsync(string listeId)
        {

            // Étape 1 : Vérifier si la liste existe
            var liste = await _listes.Find(l => l.Id == listeId).FirstOrDefaultAsync();
            if (liste == null)
            {
                Console.WriteLine("[GetPlacesAsync] ⚠️ Liste non trouvée !");
                return new List<Place>();
            }

            var places = new List<Place>();

            // Étape 2 : Vérifier si la liste contient des lieux
            if (liste.LieuxIds != null && liste.LieuxIds.Count > 0)
            {
                foreach (var placeId in liste.LieuxIds)
                {
                    Console.WriteLine($"[GetPlacesAsync] 🔍 Recherche du lieu avec l'ID : {placeId}");

                    // Étape 3 : Rechercher le lieu correspondant dans la collection Places
                    var place = await _listes.Database
                        .GetCollection<Place>("places")
                        .Find(p => p.Id == placeId)
                        .FirstOrDefaultAsync();

                    if (place != null)
                    {
                        Console.WriteLine($"[GetPlacesAsync] ✅ Lieu trouvé : {place.name}");
                        places.Add(place);
                    }
                  
                }
            }

            return places;
        }



        //get liste by name
        public async Task<List<Liste>> GetByNameAsync(string name)
        {
            Console.WriteLine($"🔍 Recherche de la liste avec le nom : {name}");

            var filter = Builders<Liste>.Filter.Regex(l => l.Nom, new BsonRegularExpression($"^{name}$", "i"));

            var result = await _listes.Find(filter).ToListAsync();

            Console.WriteLine($"🎯 Résultat de GetByNameAsync : {(result.Count > 0 ? "Liste trouvée" : "Aucune liste trouvée")}");

            return result;
        }

        public async Task UpdateManyAsync(UpdateDefinition<Liste> update)
        {
            await _listes.UpdateManyAsync(Builders<Liste>.Filter.Empty, update);
        }




    }

}
