﻿using MongoDB.Bson;
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

        [BsonElement("city")]
        [BindRequired]
        [SwaggerSchema("Ville où se situe le lieu.")]
        public string? City { get; set; }

   
        [BsonElement("mapUrl")]
        [BindRequired]
        [SwaggerSchema("URL de la carte intégrée (Google Maps, etc.).")]
        public string? MapUrl { get; set; }

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

        [BsonElement("createdAt")]
        [SwaggerSchema("Date de création du lieu.")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;


        // Constructeur pour initialiser les listes et dictionnaires
        public Place()
        {
            Tags = new List<string>();
            Images = new List<string>();
            OpeningHours = new Dictionary<string, string>();
        }
    }
}
