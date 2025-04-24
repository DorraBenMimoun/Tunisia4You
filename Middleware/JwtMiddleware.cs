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

            if (username != null)
            {
                // Récupérer l'utilisateur correspondant dans la base de données
                var user = await userRepository.GetByUsernameAsync(username);

                if (user != null)
                {
                    // Vérifier si l'utilisateur est un admin
                    if (user.IsAdmin)
                    {
                        // Si l'utilisateur est admin, ajouter cette information dans le contexte HTTP
                        context.Items["IsAdmin"] = true;
                    }
                    else
                    {
                        context.Items["IsAdmin"] = false;
                    }

                    // Ajouter l'utilisateur au contexte pour qu'il soit accessible dans le reste de la requête
                    context.Items["User"] = user;
                }
            }
        }
        catch
        {
            Console.WriteLine("Token JWT invalide !");
        }
    }

    // Appelle le middleware suivant dans le pipeline
    await _next(context);
}

    }
}
