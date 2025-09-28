# Perla Metro - Station Service

## Descripción
Servicio de gestión de estaciones para el sistema de transporte subterráneo Perla Metro. Implementa una arquitectura SOA (Service-Oriented Architecture) como parte de un monolito distribuido.

## Arquitectura y Patrón de Diseño

### Arquitectura: SOA (Service-Oriented Architecture)
- **Patrón**: Repository Pattern con Service Layer
- **Base de Datos**: MySQL (Railway)
- **Framework**: .NET 8 Web API
- **ORM**: Entity Framework Core 8.0

## Funcionalidades Implementadas

### Gestión de Estaciones
- **Crear estación**: Registro con validaciones y UUID v4
- **Listar estaciones**: Con filtros por nombre, estado y tipo
- **Consultar por ID**: Obtener estación específica
- **Actualizar estación**: Modificación segura de datos
- **Eliminar estación**: Soft delete para trazabilidad

### Características Técnicas
- **Soft Delete**: Eliminación lógica preservando trazabilidad
- **Validaciones**: Integridad de datos y reglas de negocio
- **Filtros**: Búsqueda por múltiples criterios
- **Logging**: Registro detallado de operaciones
- **Manejo de Errores**: Middleware centralizado
- **Documentación**: Swagger/OpenAPI integrado

## Configuración del Entorno

### Prerrequisitos
- .NET 8 SDK
- MySQL Server (local) o cuenta en Railway
- Visual Studio 2022 o VS Code
- Git

### Variables de Entorno
```bash
# Producción (Railway)
ConnectionStrings__ProductionConnection="Server=your_railway_host;Port=3306;Database=railway;Uid=root;Pwd=your_railway_password;"
```

## Instalación y Ejecución

### 1. Clonar el repositorio
```bash
git clone https://github.com/tu-usuario/perla-metro-station-service.git
cd perla-metro-station-service
```

### 2. Instalar dependencias
```bash
dotnet restore
```

### 3. Configurar base de datos
```bash
# Actualizar cadena de conexión en appsettings.json
# Crear migraciones
dotnet ef migrations add InitialCreate

# Aplicar migraciones
dotnet ef database update
```

### 4. Ejecutar el proyecto
```bash
# Desarrollo
dotnet run
```

### 5. Acceder a la API
- **API Base**: http://localhost:5000/api/stations
## Endpoints de la API

### Estaciones

| Método | Endpoint | Descripción | Auth |
|--------|----------|-------------|------|
| POST | `/api/stations` | Crear nueva estación | ❌ |
| GET | `/api/stations` | Listar todas las estaciones | ✅ Admin |
| GET | `/api/stations/{id}` | Obtener estación por ID | ❌ |
| PUT | `/api/stations/{id}` | Actualizar estación | ✅ Admin |
| DELETE | `/api/stations/{id}` | Eliminar estación (soft) | ✅ Admin |
| HEAD | `/api/stations/{id}` | Verificar existencia | ❌ |

## Despliegue en la Nube

### Railway (MySQL)
1. Crear proyecto en [Railway](https://railway.app)
2. Agregar servicio MySQL
3. Configurar variables de entorno
4. Desplegar desde GitHub

### Variables de Entorno en Railway
```
ConnectionStrings__DefaultConnection=${{MySQL.DATABASE_URL}}
```

## Autor
**Luis Ardiles**  
Universidad Católica del Norte  
Ingeniería de Sistemas y Computación

---
*Taller N°1 - Monolito Distribuido con Arquitectura SOA*  
*Docente: David Araya Cadiz*  
*Fecha: Septiembre 2025*