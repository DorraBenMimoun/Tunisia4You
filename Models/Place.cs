using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace MiniProjet.Models
{
    public class Place
    {

        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonElement("name")]
        public string Name { get; set; }

        [BsonElement("category")]
        public string Category { get; set; } // Restaurant, Café, Hôtel, Site touristique...

        [BsonElement("description")]
        public string Description { get; set; } // Brève description du lieu

        [BsonElement("address")]
        public string Address { get; set; } // Adresse complète

        [BsonElement("latitude")]
        public double Latitude { get; set; } // Coordonnées GPS (Latitude)

        [BsonElement("longitude")]
        public double Longitude { get; set; } // Coordonnées GPS (Longitude)

        [BsonElement("phoneNumber")]
        public string PhoneNumber { get; set; } // Numéro de contact

        [BsonElement("openingHours")]
        public Dictionary<string, string> OpeningHours { get; set; } // Stocke les horaires d'ouverture (ex: "Lundi": "08:00 - 22:00")

        [BsonElement("averageRating")]
        public double AverageRating { get; set; } // Note moyenne du lieu (calculée)

        [BsonElement("reviewCount")]
        public int ReviewCount { get; set; } // Nombre total d'avis

        [BsonElement("tags")]
        public List<string> Tags { get; set; } // Liste de tags comme "Wi-Fi", "Ambiance calme", etc.

        [BsonElement("images")]
        public List<string> Images { get; set; } // URLs des images du lieu

        // Constructeur pour initialiser les listes et dictionnaires
        public Place()
        {
            Tags = new List<string>();
            Images = new List<string>();
            OpeningHours = new Dictionary<string, string>();
        }
    }
}
