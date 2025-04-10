using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Swashbuckle.AspNetCore.Annotations;
using System;

namespace MiniProjet.Models
{
    [SwaggerSchema("Modèle représentant un utilisateur du système.")]
    public class User
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        [SwaggerSchema("Identifiant unique de l'utilisateur (ObjectId).", ReadOnly = true)]
        public string? Id { get; set; }

        [BsonElement("username")]
        [SwaggerSchema("Nom d'utilisateur unique.", Nullable = false)]
        public required string Username { get; set; }

        [BsonElement("email")]
        [SwaggerSchema("Adresse e-mail de l'utilisateur.", Format = "email", Nullable = false)]
        public required string Email { get; set; }

        [BsonElement("passwordHash")]
        [SwaggerSchema("Hash du mot de passe de l'utilisateur (crypté).", Nullable = false)]
        public required string PasswordHash { get; set; }

        [BsonElement("photo")]
        [SwaggerSchema("URL de la photo de profil de l'utilisateur.", Nullable = true)]
        public string? Photo { get; set; }

        [BsonElement("createdAt")]
        [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
        [SwaggerSchema("Date de création du compte (UTC).", ReadOnly = true, Format = "date-time")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [BsonElement("isAdmin")]
        [SwaggerSchema("Indique si l'utilisateur a des privilèges d'administrateur.", Nullable = false)]
        public bool IsAdmin { get; set; } = false;

        [BsonElement("resetPasswordToken")]
        public string? ResetPasswordToken { get; set; }

        [BsonElement("resetPasswordTokenExpires")]
        public DateTime? ResetPasswordTokenExpires { get; set; }

    }
}
