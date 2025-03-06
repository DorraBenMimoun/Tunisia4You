using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Swashbuckle.AspNetCore.Annotations;
using System;
using System.Collections.Generic;

namespace MiniProjet.Models
{
    public class Liste
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        [SwaggerSchema(ReadOnly = true, Description = "Identifiant unique de la liste.")]
        public string? Id { get; set; }

        [BsonElement("nom")]
        [SwaggerSchema(Description = "Nom de la liste.")]
        public string Nom { get; set; }

        [BsonElement("description")]
        [SwaggerSchema(Description = "Brève description de la liste.")]
        public string Description { get; set; }

        [BsonElement("updatedAt")]
        [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
        [SwaggerSchema(Description = "Date de la dernière mise à jour de la liste.")]
        public DateTime? UpdatedAt { get; set; } = DateTime.UtcNow;

        [BsonElement("createdAt")]
        [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
        [SwaggerSchema(ReadOnly = true, Description = "Date de création de la liste.")]
        public DateTime? CreatedAt { get; set; } = DateTime.UtcNow;

        [BsonElement("isPrivate")]
        [SwaggerSchema(Description = "Indique si la liste est privée ou publique.")]
        public bool IsPrivate { get; set; }

        // 🔹 Relation avec l'utilisateur (créateur de la liste)
        [BsonElement("createurId")]
        [BsonRepresentation(BsonType.ObjectId)]
        [SwaggerSchema(Description = "Identifiant de l'utilisateur ayant créé la liste.")]
        public string? CreateurId { get; set; }

        // 🔹 Relation avec les lieux (lieux associés à la liste)
        [BsonElement("lieuxIds")]
        [BsonRepresentation(BsonType.ObjectId)]
        [SwaggerSchema(Description = "Liste des identifiants des lieux contenus dans la liste.")]
        public List<string>? LieuxIds { get; set; } = new List<string>();
    }
}
