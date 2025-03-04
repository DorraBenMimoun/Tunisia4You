namespace MiniProjet.Configurations
{
    public class MongoDbSettings
    {
        public string ConnectionString { get; set; } = string.Empty; // URL de connexion à MongoDB
        public string DatabaseName { get; set; } = string.Empty; // Nom de la base de données
    }
}
