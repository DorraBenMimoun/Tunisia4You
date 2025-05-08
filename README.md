# 🇹🇳 Tunisia4You – Backend API (.NET)

Bienvenue dans le backend de **Tunisia4You**, une plateforme web dédiée à la découverte, la notation et le partage des meilleurs lieux en Tunisie.


## 🚀 Fonctionnalités principales

- 🔐 Authentification avec JWT (Login/Register)
- 👥 Gestion des utilisateurs (admin et utilisateur normal)
- 📍 CRUD complet pour les **lieux (places)** avec support d'images
- 🏷️ Gestion des **tags**
- 📄 Gestion des **listes de lieux** (publiques / privées)
- ⭐ Ajout et modération des **avis (reviews)** avec système de signalement
- 🚫 Système de bannissement des utilisateurs avec date de fin
- 📊 Accès aux **statistiques** globales (lieux, utilisateurs, listes, avis)

## 🛠️ Technologies

- **ASP.NET Core 8**
- **MongoDB** (via MongoDB.Driver)
- **JWT** pour l’authentification sécurisée
- **Swagger** pour la documentation interactive
- **MailTrap** pour les emails de réinitialisation de mot de passe
- **AutoMapper**, **DTOs** et **DataAnnotations**

## 🚧 Installation et exécution

```bash
git clone https://github.com/ton-repo/tunisia4you-backend.git
cd tunisia4you-backend
dotnet restore
dotnet run
```
Le projet démarre sur https://localhost:5066

## 🔐 Authentification

- /register → Inscription
- /login → Connexion → renvoie un JWT
- Ajoute le token JWT dans Swagger via l'en-tête Authorization: Bearer <token>

## 📁 Structure du projet
```sql
MiniProjet/
│
├── Configurations/         → Paramètres applicatifs (MongoDB, SMTP…)
│   ├── MongoDbSettings.cs  → Configuration MongoDB
│   └── SmtpSettings.cs     → Configuration SMTP pour l’envoi d’emails
├── Controllers/            → API endpoints
├── DTOs/                   → Objets de transfert de données
├── Helpers/                → JWT, EmailHelper...
├── Middleware/             → Middleware (JWT, erreurs, etc.)
├── Models/                 → Représentation des entités (User, Place, Review...)
├── Repositories/           → Accès aux collections MongoDB
├── Services/               → Logique métier (UserService, PlaceService…)
├── Program.cs              → Démarrage et configuration globale
└── appsettings.json        → Fichier de configuration de l’application
```


## 🔒 Rôles
- Admin → Accès complet
- User → Droits limités (ajouter review, créer liste…)
