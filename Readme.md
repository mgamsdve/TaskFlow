# TaskFlow

TaskFlow est une application de gestion de tâches type Kanban, construite en **.NET 8** avec :
- un backend **ASP.NET Core Web API** (`src/TaskFlow.API`)
- un frontend **Blazor WebAssembly** (`src/TaskFlow.Blazor`)
- une base de données **PostgreSQL**

## Fonctionnement de l'app

L'application suit ce flux :
1. Un utilisateur crée un compte (`/register`) ou se connecte (`/login`).
2. Une fois connecté, il accède au dashboard (`/dashboard`) avec des métriques globales (projets, boards, cartes).
3. Dans `/projects`, il peut créer un projet (lié à une organisation), puis créer des boards.
4. Dans `/board/{id}`, il peut :
   - créer des colonnes (lists),
   - créer/éditer/déplacer/supprimer des cartes,
   - assigner des utilisateurs et ajouter des commentaires (via API).

### Rôles

Certaines actions sont restreintes :
- `Owner` / `Admin` : création projet, board, list, suppression carte, liste des utilisateurs.
- Les autres opérations nécessitent un utilisateur authentifié.

### Données initiales

Au démarrage de l'API, les migrations EF Core sont appliquées automatiquement, puis une organisation par défaut est seedée si absente :

`11111111-1111-1111-1111-111111111111`

Cette valeur peut être utilisée directement dans la page `Projects`.

## Prérequis

- .NET SDK 8
- Docker (recommandé pour PostgreSQL)

## Lancer le projet en dev (local)

### 1) Démarrer PostgreSQL

```bash
docker compose up -d postgres
```

### 2) Restaurer et compiler

```bash
dotnet restore TaskFlow.sln
dotnet build TaskFlow.sln
```

### 3) Lancer l'API (terminal 1)

```bash
dotnet run --project src/TaskFlow.API --launch-profile http
```

API + Swagger :
- `http://localhost:5270`
- `http://localhost:5270/swagger`

### 4) Lancer le frontend Blazor (terminal 2)

```bash
dotnet run --project src/TaskFlow.Blazor --launch-profile http
```

Frontend :
- `http://localhost:5279`

> Le frontend utilise `ApiBaseUrl` défini dans `src/TaskFlow.Blazor/wwwroot/appsettings.json` (`http://localhost:5270` par défaut).

## Utilisation en mode serveur dev (API + DB via Docker)

Pour lancer PostgreSQL + API dans Docker :

```bash
docker compose up --build -d
```

API disponible sur :
- `http://localhost:5000`
- `http://localhost:5000/swagger`

Si vous utilisez le frontend local avec cette API Docker, modifiez `src/TaskFlow.Blazor/wwwroot/appsettings.json` :

```json
{
  "ApiBaseUrl": "http://localhost:5000"
}
```

Puis relancez le frontend Blazor.

## Commandes utiles

### Exécuter les tests

```bash
dotnet test TaskFlow.sln
```

### Voir les logs Docker

```bash
docker compose logs -f taskflow-api
docker compose logs -f postgres
```

### Arrêter l'environnement Docker

```bash
docker compose down
```
