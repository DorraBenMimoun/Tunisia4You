using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using MiniProjet.Models;

namespace MiniProjet.Helpers
{
    public class JwtHelper
    {
        private readonly IConfiguration _configuration;

        // 🔹 Constructeur qui injecte l'objet IConfiguration pour récupérer les paramètres depuis appsettings.json
        public JwtHelper(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        // 🔹 Méthode pour générer un JWT à partir d'un utilisateur
        public string GenerateToken(User user)
        {
            // 📌 1️⃣ Création des "claims" du token, qui stockent les informations de l'utilisateur
            var claims = new[]
            {
                new Claim(ClaimTypes.Name, user.Username), // Stocke le nom d'utilisateur
                new Claim(ClaimTypes.Email, user.Email) // Stocke l'email de l'utilisateur
            };

            // 📌 2️⃣ Récupération de la clé secrète depuis appsettings.json et conversion en bytes
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JwtSettings:Secret"]));

            // 📌 3️⃣ Création des informations de signature du token en utilisant HMAC SHA256
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            // 📌 4️⃣ Création du token JWT avec toutes les configurations nécessaires
            var token = new JwtSecurityToken(
                issuer: _configuration["JwtSettings:Issuer"],       // Qui émet le token (ex: "CovoitTN")
                audience: _configuration["JwtSettings:Audience"],   // Pour qui le token est valide (ex: "CovoitTNUsers")
                claims: claims,                                     // Ajout des informations de l'utilisateur
                expires: DateTime.UtcNow.AddMinutes(Convert.ToDouble(_configuration["JwtSettings:ExpiryMinutes"])), // Durée de validité
                signingCredentials: creds                           // Signature du token avec la clé secrète et HMAC SHA256
            );

            // 📌 5️⃣ Conversion du token en chaîne de caractères et retour au client
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
