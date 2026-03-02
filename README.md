# Demo Taller API - Azure REST API

API REST de nivel producción construida con .NET 7 para Azure, con arquitectura moderna, autenticación JWT, y gestión completa de productos, usuarios y órdenes.

## 🚀 Características

- ✅ **Arquitectura Moderna**: Repository Pattern, DTOs, AutoMapper
- ✅ **Autenticación JWT**: Login/Register con tokens seguros
- ✅ **API Versioning**: Versionado por URL (`/api/v1/`)
- ✅ **Rate Limiting**: 100 requests/minuto por usuario
- ✅ **Paginación**: Todos los endpoints GET soportan paginación
- ✅ **CORS**: Configurado para múltiples orígenes
- ✅ **Swagger**: Documentación interactiva con autenticación
- ✅ **Soft Delete**: Eliminación lógica de registros
- ✅ **Manejo Global de Excepciones**: Respuestas consistentes
- ✅ **Azure SQL Database**: Base de datos en la nube

## 📋 Recursos Disponibles

### Products
- `GET /api/v1/product` - Listar productos (paginado, filtro por categoría)
- `GET /api/v1/product/{id}` - Obtener producto
- `POST /api/v1/product` - Crear producto (Admin)
- `PUT /api/v1/product/{id}` - Actualizar producto (Admin)
- `DELETE /api/v1/product/{id}` - Eliminar producto (Admin)

### Users
- `GET /api/v1/user` - Listar usuarios (Admin, paginado)
- `GET /api/v1/user/{id}` - Obtener usuario
- `POST /api/v1/user` - Crear usuario (Admin)
- `PUT /api/v1/user/{id}` - Actualizar usuario (Admin)
- `DELETE /api/v1/user/{id}` - Eliminar usuario (Admin)

### Orders
- `GET /api/v1/order` - Listar órdenes (Admin, paginado)
- `GET /api/v1/order/{id}` - Obtener orden
- `GET /api/v1/order/user/{userId}` - Órdenes por usuario (paginado)
- `POST /api/v1/order` - Crear orden
- `PATCH /api/v1/order/{id}/status` - Actualizar estado (Admin)
- `DELETE /api/v1/order/{id}` - Eliminar orden (solo pendientes)

### Authentication
- `POST /api/v1/auth/register` - Registrar usuario
- `POST /api/v1/auth/login` - Iniciar sesión

### Limitations
- `GET /api/v1/limitations` - Ver configuración de rate limiting
- `GET /api/v1/limitations/status` - Estado actual del usuario

## 🛠️ Tecnologías

- **.NET 7.0**
- **Entity Framework Core 7.0**
- **SQL Server** (Azure SQL Database)
- **AutoMapper** - Mapeo de objetos
- **JWT Bearer** - Autenticación
- **AspNetCoreRateLimit** - Rate limiting
- **Swagger/OpenAPI** - Documentación
- **Serilog** - Logging

## 📦 Instalación Local

### Prerrequisitos
- .NET 7 SDK
- SQL Server (local o Azure)

### Pasos

1. **Clonar el repositorio**
```bash
git clone https://github.com/LeonardoDiazO/ApiPrueba.Net.git
cd ApiPrueba.Net
```

2. **Configurar la base de datos**

Editar `appsettings.json` con tu connection string:
```json
{
  "ConnectionStrings": {
    "StoreConnection": "tu-connection-string-aqui"
  }
}
```

3. **Ejecutar migraciones**
```bash
cd DemoTallerApi
dotnet ef database update
```

4. **Ejecutar la aplicación**
```bash
dotnet run
```

5. **Abrir Swagger**
```
https://localhost:7XXX
```

## 🔐 Autenticación

La API usa JWT Bearer tokens. Para autenticarte:

1. **Registrar un usuario**:
```bash
POST /api/v1/auth/register
{
  "email": "user@example.com",
  "password": "Password123",
  "firstName": "John",
  "lastName": "Doe"
}
```

2. **Obtener token**:
```bash
POST /api/v1/auth/login
{
  "email": "user@example.com",
  "password": "Password123"
}
```

3. **Usar el token**:
```
Authorization: Bearer {tu-token-aqui}
```

En Swagger, haz clic en "Authorize" y pega el token.

## 📊 Estructura del Proyecto

```
DemoTallerApi/
├── Controllers/          # Endpoints de la API
│   ├── ProductController.cs
│   ├── UserController.cs
│   ├── OrderController.cs
│   ├── AuthController.cs
│   └── LimitationsController.cs
├── DTOs/                 # Data Transfer Objects
├── Models/               # Entidades de base de datos
├── Repositories/         # Patrón Repository
├── Service/              # Lógica de negocio
├── Helpers/              # JWT, Password helpers
├── Middleware/           # Exception handling
├── Mappings/             # AutoMapper profiles
└── Data/                 # DbContext
```

## 🔧 Configuración

### JWT Settings (appsettings.json)
```json
{
  "JwtSettings": {
    "Secret": "tu-secret-key-de-al-menos-32-caracteres",
    "Issuer": "DemoTallerApi",
    "Audience": "DemoTallerApiUsers",
    "ExpirationInMinutes": 60
  }
}
```

### Rate Limiting
```json
{
  "IpRateLimiting": {
    "GeneralRules": [
      { "Endpoint": "*", "Period": "1m", "Limit": 100 },
      { "Endpoint": "*", "Period": "1h", "Limit": 1000 }
    ]
  }
}
```

### CORS
```json
{
  "CorsSettings": {
    "AllowedOrigins": ["http://localhost:3000", "http://localhost:4200"]
  }
}
```

## 🚀 Deployment a Azure

### App Service

1. **Crear recursos en Azure**:
   - App Service Plan
   - App Service
   - SQL Database

2. **Configurar Connection String** en Azure Portal:
```
Configuration > Connection strings > Add
Name: StoreConnection
Value: tu-azure-sql-connection-string
Type: SQLAzure
```

3. **Configurar JWT Secret** en Application Settings:
```
JwtSettings__Secret = tu-secret-key
```

4. **Deploy**:
```bash
dotnet publish -c Release
# Usar Azure CLI o GitHub Actions
```

## 📝 Ejemplos de Uso

### Crear una Orden
```bash
POST /api/v1/order
Authorization: Bearer {token}
Content-Type: application/json

{
  "userId": 1,
  "orderItems": [
    {
      "productId": 1,
      "quantity": 2
    },
    {
      "productId": 3,
      "quantity": 1
    }
  ]
}
```

### Listar Productos con Paginación
```bash
GET /api/v1/product?pageNumber=1&pageSize=10&category=Electronics
```

## 🧪 Testing

```bash
# Ejecutar tests
dotnet test

# Build
dotnet build

# Verificar migraciones
dotnet ef migrations list
```

## 📄 Licencia

Este proyecto es de código abierto.

## 👥 Autor

Leonardo Díaz O.
