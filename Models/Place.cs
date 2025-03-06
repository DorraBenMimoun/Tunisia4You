using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Swashbuckle.AspNetCore.Annotations;
using System.Collections.Generic;

namespace MiniProjet.Models
{
    [SwaggerSchema("Modèle représentant un lieu (restaurant, hôtel, site touristique, etc.).")]
    public class Place
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        [SwaggerSchema("Identifiant unique du lieu (ObjectId).", ReadOnly = true)]
        public string? Id { get; set; }

        [BsonElement("name")]
        [BindRequired]
        [SwaggerSchema("Nom du lieu.")]
        public string Name { get; set; }

        [BsonElement("category")]
        [BindRequired]
        [SwaggerSchema("Catégorie du lieu.")]
        public string Category { get; set; }

        [BsonElement("description")]
        [BindRequired]
        [SwaggerSchema("Brève description du lieu.")]
        public string Description { get; set; }

        [BsonElement("address")]
        [BindRequired]
        [SwaggerSchema("Adresse complète du lieu.")]
        public string? Address { get; set; }

        [BsonElement("latitude")]
        [BindRequired]
        [SwaggerSchema("Latitude du lieu pour la géolocalisation.")]
        public double Latitude { get; set; }

        [BsonElement("longitude")]
        [BindRequired]
        [SwaggerSchema("Longitude du lieu pour la géolocalisation.")]
        public double Longitude { get; set; }

        [BsonElement("phoneNumber")]
        [BindRequired]
        [SwaggerSchema("Numéro de téléphone du lieu.")]

        public string? PhoneNumber { get; set; }

        [BsonElement("openingHours")]
        [BindRequired]
        [SwaggerSchema("Horaires d'ouverture du lieu.")]
        public Dictionary<string, string> OpeningHours { get; set; }

        [BsonElement("averageRating")]
        [SwaggerSchema("Note moyenne du lieu.")]
        public double AverageRating { get; set; }

        [BsonElement("reviewCount")]
        [SwaggerSchema("Nombre total d'avis reçus.")]
        public int ReviewCount { get; set; }

        [BsonElement("tags")]
        [BindRequired]
        [SwaggerSchema("Liste de tags associés au lieu.")]
        public List<string> Tags { get; set; }

        [BsonElement("images")]
        [BindRequired]
        [SwaggerSchema("Liste des URLs des images du lieu.")]
        public List<string> Images { get; set; }

        // Constructeur pour initialiser les listes et dictionnaires
        public Place()
        {
            Tags = new List<string>();
            Images = new List<string>();
            OpeningHours = new Dictionary<string, string>();
        }
    }
}
