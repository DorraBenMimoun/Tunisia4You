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
        public string name { get; set; }

        [BsonElement("category")]
        [BindRequired]
        [SwaggerSchema("Catégorie du lieu.")]
        public string category { get; set; }

        [BsonElement("description")]
        [BindRequired]
        [SwaggerSchema("Brève description du lieu.")]
        public string description { get; set; }

        [BsonElement("address")]
        [BindRequired]
        [SwaggerSchema("Adresse complète du lieu.")]
        public string? address { get; set; }

        [BsonElement("city")]
        [BindRequired]
        [SwaggerSchema("Ville où se situe le lieu.")]
        public string? city { get; set; }

        [BsonElement("latitude")]
        [BindRequired]
        [SwaggerSchema("Latitude du lieu pour la géolocalisation.")]
        public double latitude { get; set; }

        [BsonElement("longitude")]
        [BindRequired]
        [SwaggerSchema("Longitude du lieu pour la géolocalisation.")]
        public double longitude { get; set; }

        [BsonElement("phoneNumber")]
        [BindRequired]
        [SwaggerSchema("Numéro de téléphone du lieu.")]

        public string? phoneNumber { get; set; }

        [BsonElement("openingHours")]
        [BindRequired]
        [SwaggerSchema("Horaires d'ouverture du lieu.")]
        public Dictionary<string, string> openingHours { get; set; }

        [BsonElement("averageRating")]
        [SwaggerSchema("Note moyenne du lieu.")]
        public double averageRating { get; set; }

        [BsonElement("reviewCount")]
        [SwaggerSchema("Nombre total d'avis reçus.")]
        public int reviewCount { get; set; }

        [BsonElement("tags")]
        [BindRequired]
        [SwaggerSchema("Liste de tags associés au lieu.")]
        public List<string> tags { get; set; }

        [BsonElement("images")]
        [BindRequired]
        [SwaggerSchema("Liste des URLs des images du lieu.")]
        public List<string> images { get; set; }

        // Constructeur pour initialiser les listes et dictionnaires
        public Place()
        {
            tags = new List<string>();
            images = new List<string>();
            openingHours = new Dictionary<string, string>();
        }
    }
}
