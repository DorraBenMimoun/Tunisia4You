using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using Swashbuckle.AspNetCore.Annotations;

namespace MiniProjet.Models
{
    public class TagPlace
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        [SwaggerSchema(ReadOnly = true, Description = "Identifiant unique du tag.")]

        public string? Id { get; set; }

        [BsonElement("libelle")]
        [SwaggerSchema(Description = "Libellé du tag (doit être unique).")]

        public string Libelle { get; set; }
    }
}
