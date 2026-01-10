# Hotel Microservices - Arquitectura de Microservicios .NET 8

Sistema de gestión hotelera migrado de monolito a microservicios con:
- **JWT Authentication** en todos los servicios
- **1 servicio GraphQL** (autenticación obligatoria)
- **1 servicio gRPC** (comunicación entre servicios)
- **2 servicios REST**
- **API Gateway** centralizado
- **Event Bus** para comunicación asíncrona

## 🏗️ Arquitectura

```
┌─────────────────────────────────────────────────────────────────┐
│                        API GATEWAY                               │
│                      (http://localhost:5000)                     │
│                    JWT Auth + YARP Reverse Proxy                 │
└─────────────────────┬───────────────────────────────────────────┘
                      │
    ┌─────────────────┼─────────────────┬─────────────────┐
    │                 │                 │                 │
    ▼                 ▼                 ▼                 ▼
┌─────────┐    ┌──────────┐    ┌─────────────┐    ┌────────────┐
│Catálogos│    │Habitac.  │    │  Reservas   │    │Usuarios    │
│ (REST)  │    │(GraphQL) │    │   (gRPC)    │    │Pagos(REST) │
│  :5001  │    │  :5002   │    │   :5003     │    │   :5004    │
└─────────┘    └──────────┘    └──────┬──────┘    └─────┬──────┘
                                      │                  │
                                      └───── gRPC ───────┘
```

## 📦 Microservicios

### 1. CatalogosService (REST) - Puerto 5001
- `/api/hoteles` - CRUD de Hoteles
- `/api/ciudades` - CRUD de Ciudades  
- `/api/paises` - CRUD de Países
- `/api/tiposhabitacion` - CRUD de Tipos de Habitación
- `/api/amenidades` - CRUD de Amenidades
- `/api/roles` - CRUD de Roles
- `/api/metodospago` - CRUD de Métodos de Pago

### 2. HabitacionesService (GraphQL) - Puerto 5002
**Autenticación Obligatoria en todas las operaciones**
- Habitaciones, Imágenes, Amenidades por Habitación, Descuentos

### 3. ReservasService (gRPC) - Puerto 5003
- Reservas, HabXRes, DesxHabxRes, Hold

### 4. UsuariosPagosService (REST + Cliente gRPC) - Puerto 5004
- Usuarios, Pagos, Facturas, PDFs, Funciones Especiales
- **Comunicación gRPC** con ReservasService para validar reservas

### 5. API Gateway - Puerto 5000
- JWT Token generation/validation
- Reverse proxy con YARP

## 🔐 Obtener Token JWT

```bash
curl -X POST http://localhost:5000/api/auth/token \
  -H "Content-Type: application/json" \
  -d '{"username": "admin", "password": "admin123"}'
```

## 🚀 Iniciar Todos los Servicios

```powershell
.\start-all-services.ps1
```

## 📊 Event Bus

Eventos: ReservaCreatedEvent, PagoRealizadoEvent, FacturaEmitidaEvent, etc.

## 📁 Estructura

```
Microservicios/
├── Shared.DTOs/              # DTOs compartidos
├── Shared.Data/              # Repositorios
├── Shared.EventBus/          # Event Bus
├── CatalogosService/         # REST
├── HabitacionesService/      # GraphQL
├── ReservasService/          # gRPC
├── UsuariosPagosService/     # REST + gRPC Client
├── ApiGateway/               # Gateway
└── HotelMicroservices.sln
```
│                    │     SQL Server DB       │                          │
│                    │   (Base de datos)       │                          │
│                    └─────────────────────────┘                          │
│                                                                          │
└─────────────────────────────────────────────────────────────────────────┘
```

## Proyectos

| Proyecto | Tipo | Puerto | Descripción |
|----------|------|--------|-------------|
| `GrpcHotelService` | gRPC | 5001 | Gestión de hoteles vía gRPC |
| `GraphQLHabitacionService` | GraphQL | 5002 | Gestión de habitaciones vía GraphQL |
| `RestReservaService` | REST | 5003 | Gestión de reservas vía REST API |
| `RestUsuarioService` | REST | 5004 | Gestión de usuarios vía REST API |
| `RestPagoService` | REST | 5005 | Gestión de pagos y facturación vía REST API |
| `Shared.DTOs` | Librería | - | DTOs compartidos |
| `Shared.Data` | Librería | - | Repositorios de acceso a datos |

## Requisitos

- .NET 8 SDK
- SQL Server (la conexión está configurada para usar la BD existente)

## Instalación

1. Navegar a la carpeta de microservicios:
```bash
cd Microservicios
```

2. Restaurar paquetes:
```bash
dotnet restore HotelMicroservicios.sln
```

3. Compilar la solución:
```bash
dotnet build HotelMicroservicios.sln
```

## Ejecución

### Ejecutar todos los microservicios

Usar el script PowerShell incluido:
```powershell
.\start-all.ps1
```

### Ejecutar microservicios individuales

#### gRPC - Hotel (Puerto 5001)
```bash
cd GrpcHotelService
dotnet run
```

#### GraphQL - Habitaciones (Puerto 5002)
```bash
cd GraphQLHabitacionService
dotnet run
```
- Interfaz GraphQL: http://localhost:5002/graphql

#### REST - Reservas (Puerto 5003)
```bash
cd RestReservaService
dotnet run
```
- Swagger: http://localhost:5003/swagger

#### REST - Usuarios (Puerto 5004)
```bash
cd RestUsuarioService
dotnet run
```
- Swagger: http://localhost:5004/swagger

#### REST - Pagos/Facturación (Puerto 5005)
```bash
cd RestPagoService
dotnet run
```
- Swagger: http://localhost:5005/swagger

## Uso de los Servicios

### gRPC (Hotel)

Para probar el servicio gRPC, puedes usar herramientas como:
- **grpcurl**: Cliente de línea de comandos
- **BloomRPC**: Cliente con interfaz gráfica
- **Postman**: Soporte para gRPC

Ejemplo con grpcurl:
```bash
# Listar hoteles
grpcurl -plaintext localhost:5001 hotel.HotelGrpc/GetAll

# Obtener hotel por ID
grpcurl -plaintext -d '{"id_hotel": 1}' localhost:5001 hotel.HotelGrpc/GetById

# Crear hotel
grpcurl -plaintext -d '{"id_hotel": 10, "nombre_hotel": "Hotel Nuevo", "estado_hotel": true}' localhost:5001 hotel.HotelGrpc/Create
```

### GraphQL (Habitaciones)

Acceder a http://localhost:5002/graphql para usar el playground de GraphQL.

**Queries de ejemplo:**

```graphql
# Obtener todas las habitaciones
query {
  habitaciones {
    idHabitacion
    nombreHabitacion
    precioActualHabitacion
    capacidadHabitacion
    estadoHabitacion
  }
}

# Obtener habitación por ID
query {
  habitacion(id: "HAB001") {
    idHabitacion
    nombreHabitacion
    precioActualHabitacion
    idHotel
  }
}

# Buscar habitaciones con filtros
query {
  buscarHabitaciones(
    idHotel: 1
    precioMaximo: 200
    capacidadMinima: 2
    soloDisponibles: true
  ) {
    idHabitacion
    nombreHabitacion
    precioActualHabitacion
  }
}
```

**Mutations de ejemplo:**

```graphql
# Crear habitación
mutation {
  crearHabitacion(input: {
    idHabitacion: "HAB100"
    idTipoHabitacion: 1
    idCiudad: 1
    idHotel: 1
    nombreHabitacion: "Suite Premium"
    precioNormalHabitacion: 250.00
    precioActualHabitacion: 225.00
    capacidadHabitacion: 4
    estadoHabitacion: true
    estadoActivoHabitacion: true
  }) {
    success
    message
    habitacion {
      idHabitacion
      nombreHabitacion
    }
  }
}

# Actualizar precio
mutation {
  actualizarPrecio(id: "HAB001", nuevoPrecio: 180.00) {
    success
    message
    habitacion {
      precioActualHabitacion
    }
  }
}
```

### REST APIs

Todas las APIs REST incluyen documentación Swagger automática.

**Reservas (Puerto 5003):**
- `GET /api/reservas` - Listar todas
- `GET /api/reservas/{id}` - Obtener por ID
- `POST /api/reservas` - Crear
- `PUT /api/reservas/{id}` - Actualizar
- `DELETE /api/reservas/{id}` - Eliminar
- `PATCH /api/reservas/{id}/cancelar` - Cancelar reserva
- `PATCH /api/reservas/{id}/confirmar` - Confirmar reserva

**Usuarios (Puerto 5004):**
- `GET /api/usuarios` - Listar todos
- `GET /api/usuarios/{id}` - Obtener por ID
- `POST /api/usuarios` - Crear
- `PUT /api/usuarios/{id}` - Actualizar
- `DELETE /api/usuarios/{id}` - Eliminar
- `PATCH /api/usuarios/{id}/activar` - Activar usuario
- `PATCH /api/usuarios/{id}/desactivar` - Desactivar usuario

**Pagos (Puerto 5005):**
- `GET /api/pagos` - Listar todos
- `GET /api/pagos/{id}` - Obtener por ID
- `POST /api/pagos` - Crear
- `PUT /api/pagos/{id}` - Actualizar
- `DELETE /api/pagos/{id}` - Eliminar

**Facturas (Puerto 5005):**
- `GET /api/facturas` - Listar todas
- `GET /api/facturas/{id}` - Obtener por ID
- `POST /api/facturas` - Crear
- `PUT /api/facturas/{id}` - Actualizar
- `DELETE /api/facturas/{id}` - Eliminar
- `PATCH /api/facturas/{id}/anular` - Anular factura

## Configuración de Base de Datos

La cadena de conexión está configurada en `Shared.Data/DatabaseConfig.cs`. 
Puedes modificarla según tu entorno:

```csharp
public const string ConnectionString = 
    "Server=tu-servidor;Database=tu-base;User Id=usuario;Password=clave;...";
```

## Estructura de Carpetas

```
Microservicios/
├── HotelMicroservicios.sln          # Solución principal
├── README.md                         # Este archivo
├── start-all.ps1                     # Script para iniciar todos
│
├── Shared.DTOs/                      # DTOs compartidos
│   ├── HotelDto.cs
│   ├── HabitacionDto.cs
│   ├── ReservaDto.cs
│   └── ...
│
├── Shared.Data/                      # Capa de acceso a datos
│   ├── DatabaseConfig.cs
│   ├── HotelRepository.cs
│   ├── HabitacionRepository.cs
│   └── ...
│
├── GrpcHotelService/                 # Microservicio gRPC
│   ├── Protos/
│   │   └── hotel.proto
│   ├── Services/
│   │   └── HotelGrpcService.cs
│   └── Program.cs
│
├── GraphQLHabitacionService/         # Microservicio GraphQL
│   ├── GraphQL/
│   │   ├── Query.cs
│   │   ├── Mutation.cs
│   │   └── Types/
│   └── Program.cs
│
├── RestReservaService/               # Microservicio REST
│   ├── Controllers/
│   └── Program.cs
│
├── RestUsuarioService/               # Microservicio REST
│   ├── Controllers/
│   └── Program.cs
│
└── RestPagoService/                  # Microservicio REST
    ├── Controllers/
    └── Program.cs
```

## Notas

- La base de datos se mantiene igual que el proyecto original
- Cada microservicio es independiente y puede escalarse por separado
- Los proyectos compartidos (Shared.DTOs y Shared.Data) contienen la lógica común
- Todos los servicios están configurados para desarrollo local
