using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using Swashbuckle.AspNetCore.Annotations;

namespace MiniProjet.Models
{
    [SwaggerSchema("Modèle de signalement d'un avis inapproprié.")]
    public class Report
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        [SwaggerSchema("ID unique du signalement")]
        public string? Id { get; set; }

        [BsonElement("reviewId")]
        [BsonRepresentation(BsonType.ObjectId)]
        [SwaggerSchema("ID de la review signalée")]
        public string ReviewId { get; set; }

        [BsonElement("userId")]
        [BsonRepresentation(BsonType.ObjectId)]
        [SwaggerSchema("ID de l'utilisateur qui signale")]
        public string UserId { get; set; }

        [BsonElement("reportedUserId")]
        [BsonRepresentation(BsonType.ObjectId)]
        [SwaggerSchema("ID de l'utilisateur qui a créé la review signalée")]
        public string ReportedUserId { get; set; }

        [BsonElement("reason")]
        [SwaggerSchema("Raison du signalement")]
        public string Reason { get; set; }

        [BsonElement("createdAt")]
        [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
        [SwaggerSchema("Date du signalement")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
