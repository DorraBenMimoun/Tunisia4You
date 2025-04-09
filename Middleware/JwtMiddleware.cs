using Microsoft.AspNetCore.Http;
using MiniProjet.Helpers;
using MiniProjet.Repositories;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Threading.Tasks;

namespace MiniProjet.Middleware
{
    public class JwtMiddleware
    {
        private readonly RequestDelegate _next; // Représente le prochain middleware dans le pipeline de traitement des requêtes
        private readonly JwtHelper _jwtHelper; // Helper personnalisé pour gérer les tokens JWT (optionnel ici)

        public JwtMiddleware(RequestDelegate next, JwtHelper jwtHelper)
        {
            _next = next;
            _jwtHelper = jwtHelper;
        }

        // Cette méthode est appelée à chaque requête HTTP
        public async Task Invoke(HttpContext context, UserRepository userRepository)
        {
            // Récupérer le token JWT dans l'en-tête "Authorization" (ex: "Bearer eyJhbGciOiJI...")
            var token = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();

            // Vérifie si le token existe bien dans la requête
            if (!string.IsNullOrEmpty(token))
            {
                try
                {
                    // Initialiser un gestionnaire pour lire le token JWT
                    var tokenHandler = new JwtSecurityTokenHandler();

                    // Lire le contenu du token JWT
                    var jwtToken = tokenHandler.ReadJwtToken(token);

                    // Extraire le nom d'utilisateur depuis les claims du token
                    var username = jwtToken.Claims.FirstOrDefault(x => x.Type.EndsWith("/name"))?.Value;

                    // Vérifie si le claim du username existe
                    if (username != null)
                    {
                        // Récupère l'utilisateur correspondant dans la base de données MongoDB
                        var user = await userRepository.GetByUsernameAsync(username);

                        // Si l'utilisateur existe, on le stocke dans le contexte HTTP pour y accéder plus tard
                        if (user != null)
                        {
                            context.Items["User"] = user;
                        }
                    }
                }
                catch
                {
                    // Si le token est invalide ou une erreur survient, on affiche un message dans la console
                    Console.WriteLine("Token JWT invalide !");
                    // ⚠️ Pas de levée d'exception ici pour ne pas interrompre le flux de la requête
                }
            }

            // Appelle le middleware suivant dans le pipeline
            await _next(context);
        }
    }
}
