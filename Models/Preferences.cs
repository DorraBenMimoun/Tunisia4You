using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Swashbuckle.AspNetCore.Annotations;
using System;
using System.Collections.Generic;

namespace MiniProjet.Models
{
    [SwaggerSchema("Représente les préferences de l'utilisateur.")]
    public class Preferences
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        [SwaggerSchema(ReadOnly = true, Description = "Identifiant unique de la préference.")]
        public string? Id { get; set; }

        [BsonElement("userId")]
        [BsonRepresentation(BsonType.ObjectId)]
        [SwaggerSchema(Description = "Identifiant de l'utilisateur à qui appartient la préference.")]
        public required string UserId { get; set; }

        [BsonElement("preferredTags")]
        [BsonRepresentation(BsonType.ObjectId)]
        [SwaggerSchema(Description = "Liste des identifiants des tags préférés de l'utilisateur.")]
        public required List<string> PreferredTags { get; set; }

        [BsonElement("preferredCities")]
        [SwaggerSchema(Description = "Liste des villes préférées de l'utilisateur.")]
        public required List<string> PreferredCities { get; set; }

        [BsonElement("preferredCategories")]
        [SwaggerSchema(Description = "Liste des catégories préférées de l'utilisateur.")]
        public required List<string> PreferredCategories { get; set; }

        [BsonElement("minRating")]
        [SwaggerSchema(Description = "Note minimale requise pour les lieux.")]
        public required double MinRating { get; set; }

        [BsonElement("priceRange")]
        [SwaggerSchema(Description = "Gamme de prix préférée (any, low, medium, high).")]
        public required string PriceRange { get; set; }
    }
}
