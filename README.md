# Gestion de Coworking

## Description
Une application web complète pour la gestion d'un espace de coworking. Ce projet permet d'automatiser et d'optimiser les réservations, de centraliser les informations, et de faciliter le travail des administrateurs, des utilisateurs (étudiants) et des techniciens. 

## Architecture
Le projet repose sur une architecture client-serveur moderne articulée autour de deux grandes couches :
- **Frontend** : Développé avec **Blazor WebAssembly**. Il offre une interface dynamique, interactive et fluide, entièrement exécutée côté navigateur en C#.
- **Backend** : Développé en **ASP.NET Core Web API**. Il expose des endpoints REST, implémente le Repository Pattern pour l'accès aux données, et utilise des tokens JWT pour une authentification sécurisée.
- **Base de données** : **SQL Server** géré avec Entity Framework Core.

## Fonctionnalités Principales

### 👨‍🎓 Étudiant (Utilisateur principal)
- Création et gestion de compte avec notification par email.
- Authentification sécurisée et réinitialisation de mot de passe.
- Souscription et gestion d'abonnements (vérification de la disponibilité et des périodes).
- Création et suivi de réservations d'espaces de coworking.

### 🛡️ Administrateur
- Gestion complète des utilisateurs et attribution des rôles (Admin, Technicien).
- Création, modification et suppression des espaces de coworking et de leurs caractéristiques (capacité, etc.).
- Gestion des ressources associées aux différents espaces.
- Création, assignation et suivi des demandes de maintenance.

### 🔧 Technicien
- Tableau de bord pour consulter les maintenances assignées.
- Mise à jour de l'état d'une maintenance (En cours, Terminée).

## Prérequis
- [Visual Studio](https://visualstudio.microsoft.com/) (avec la charge de travail ASP.NET et développement web)
- [.NET SDK](https://dotnet.microsoft.com/download)
- [SQL Server](https://www.microsoft.com/sql-server)

## Installation et Lancement

1. **Cloner le dépôt** :
   ```bash
   git clone https://github.com/aymen-kacem/Gestion-de-Biblioth-que-.git
   cd Gestion-de-Biblioth-que-
   ```

2. **Configuration de la base de données** :
   - Assurez-vous que votre instance locale SQL Server est en cours d'exécution.
   - Mettez à jour la chaîne de connexion (Connection String) dans le fichier `appsettings.json` du projet Backend (`CoworkingGes`).
   - Appliquez les migrations Entity Framework via la console du gestionnaire de packages (Package Manager Console) ou le CLI .NET pour créer la base de données :
     ```bash
     dotnet ef database update
     ```

3. **Lancer l'application** :
   - Ouvrez le fichier solution (`.sln`) dans Visual Studio.
   - Configurez la solution pour avoir de multiples projets de démarrage (Multiple Startup Projects) : définissez le Backend (`CoworkingGes`) et le Frontend (`CoworkingFrontend`) sur **Start**.
   - Appuyez sur `F5` pour lancer l'application.

## Documentation de l'API
Le backend intègre **Swagger (OpenAPI)**. Une fois le projet lancé, vous pouvez naviguer vers l'URL Swagger pour afficher toutes les routes disponibles, analyser les paramètres et tester rapidement les endpoints de l'API.

## Auteurs
Projet réalisé dans le cadre de l'année universitaire 2025/2026 à l'**Institut International de Technologie à Sfax (IIT)**.
- **Aymen Kacem**
