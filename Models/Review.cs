using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Swashbuckle.AspNetCore.Annotations;
using System;

namespace MiniProjet.Models
{
    [SwaggerSchema("Représente un avis laissé par un utilisateur sur un lieu.")]
    public class Review
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        [SwaggerSchema(ReadOnly = true, Description = "Identifiant unique de la review.")]
        public string? Id { get; set; }

        [BsonElement("commentaire")]
        [SwaggerSchema(Description = "Commentaire de l'utilisateur sur le lieu.")]
        public string Commentaire { get; set; }

        [BsonElement("note")]
        [SwaggerSchema(Description = "Note attribuée par l'utilisateur, par exemple de 1 à 5.")]
        public int Note { get; set; }

        [BsonElement("createdAt")]
        [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
        [SwaggerSchema(ReadOnly = true, Description = "Date de création de l'avis.")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [BsonElement("userId")]
        [BsonRepresentation(BsonType.ObjectId)]
        [SwaggerSchema(Description = "Identifiant de l'utilisateur qui a laissé l'avis.")]
        public required string UserId { get; set; }

        [BsonElement("placeId")]
        [BsonRepresentation(BsonType.ObjectId)]
        [SwaggerSchema(Description = "Identifiant du lieu concerné par l'avis.")]
        public required string PlaceId { get; set; }
    }
}
