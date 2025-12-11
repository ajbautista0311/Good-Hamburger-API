# 🍔 Good Hamburger API

API REST para sistema de pedidos de hamburguesas construida con **Clean Architecture**, **.NET 10**, **SQL Server**, **Redis Cache** y **Docker**.

## 🚀 Características

- ✅ **Clean Architecture** (Domain, Application, Infrastructure, API)
- ✅ **.NET 10** 
- ✅ **Entity Framework Core 9** con Code First
- ✅ **SQL Server 2022** para persistencia
- ✅ **Redis Cache** para el menú (patrón Cache-Aside)
- ✅ **Swagger/OpenAPI** para documentación interactiva
- ✅ **xUnit + Moq + FluentAssertions** para testing
- ✅ **Docker & Docker Compose** para contenedorización
- ✅ **Migraciones automáticas** al iniciar

---

## 📋 Requisitos Previos

### Para Docker (Recomendado):
- Docker Desktop instalado
- Docker Compose incluido

### Para desarrollo local:
- .NET 10 SDK
- SQL Server (Express o Developer Edition)
- Redis Server
- (Opcional) Visual Studio 2022 / VS Code / Rider

---

## 🏗️ Arquitectura
```
GoodHamburger/
├── src/
│   ├── GoodHamburger.Domain/          # Entidades, Interfaces, Lógica de Negocio
│   ├── GoodHamburger.Application/     # Casos de Uso, DTOs
│   ├── GoodHamburger.Infrastructure/  # EF Core, SQL Server, Redis, Repositorios
│   └── GoodHamburger.API/             # Controllers, Program.cs, Swagger
├── tests/
│   └── GoodHamburger.Tests/           # Tests Unitarios (xUnit + Moq)
├── docker-compose.yml
└── Dockerfile
```

### Capas de Clean Architecture:
```
┌─────────────────────────────────────────┐
│              API Layer                  │  ← Controllers, Middleware
├─────────────────────────────────────────┤
│         Application Layer               │  ← Use Cases, DTOs
├─────────────────────────────────────────┤
│           Domain Layer                  │  ← Entities, Interfaces, Services
├─────────────────────────────────────────┤
│       Infrastructure Layer              │  ← EF Core, SQL Server, Redis
└─────────────────────────────────────────┘
```

---

## 🔧 Instalación y Ejecución

### Opción 1: Con Docker (Recomendado)
```bash
# 1. Clonar el repositorio
git clone <repository-url> //Se envia como archivo comprimido, no como git,
por lo que se descomprime el archivo y se navega a la carpeta descomprimida
cd GoodHamburger

# 2. Construir y ejecutar todos los servicios
docker-compose up --build -d

# 3. La API estará disponible en:
# - API: http://localhost:5000
# - OpenApi: http://localhost:5000/scalar/v1
# - Health: http://localhost:5000/health

# 4. Ver logs
docker-compose logs -f api

# 5. Detener servicios
docker-compose down
```

### Opción 2: Local sin Docker
```bash
# 1. Asegúrate de tener SQL Server y Redis corriendo

# 2. Restaurar paquetes
dotnet restore

# 3. Aplicar migraciones
dotnet ef database update \
  --project src/GoodHamburger.Infrastructure \
  --startup-project src/GoodHamburger.API

# 4. Ejecutar la API
dotnet run --project src/GoodHamburger.API

# O en modo watch (recarga automática)
dotnet watch run --project src/GoodHamburger.API
```

---

## 📚 Endpoints de la API

### 🍔 Menu

| Método | Endpoint | Descripción | Cache |
|--------|----------|-------------|-------|
| `GET` | `/api/menu` | Obtener todos los items del menú | ✅ Redis (24h) |
| `GET` | `/api/menu/sandwiches` | Obtener solo sandwiches | ✅ Redis (24h) |
| `GET` | `/api/menu/extras` | Obtener solo extras | ✅ Redis (24h) |

### 📦 Orders

| Método | Endpoint | Descripción |
|--------|----------|-------------|
| `POST` | `/api/orders` | Crear una nueva orden |
| `GET` | `/api/orders` | Listar todas las órdenes |
| `PUT` | `/api/orders/{id}` | Actualizar una orden |
| `DELETE` | `/api/orders/{id}` | Eliminar una orden |

### ❤️ Health Check

| Método | Endpoint | Descripción |
|--------|----------|-------------|
| `GET` | `/health` | Retorna el estado actual del servicio |
|
