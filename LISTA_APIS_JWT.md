# ?? LISTA COMPLETA DE APIs - REQUISITOS JWT

---

## ?? **LEYENDA**

- ? **Requiere JWT** - Necesita token Bearer en el header
- ? **NO requiere JWT** - Público, sin autenticación
- ?? **Mixto** - Algunos endpoints requieren JWT, otros no

---

## 1?? **UsuariosPagosService**

Base URL: `https://usuarios-pagos-service.onrender.com`

### **Facturas** - ? **TODOS requieren JWT**

| Método | Endpoint | JWT |
|--------|----------|-----|
| GET | `/api/Facturas` | ? Sí |
| GET | `/api/Facturas/{id}` | ? Sí |
| POST | `/api/Facturas` | ? Sí |
| PUT | `/api/Facturas/{id}` | ? Sí |
| DELETE | `/api/Facturas/{id}` | ? Sí |

---

### **Pagos** - ? **TODOS requieren JWT**

| Método | Endpoint | JWT |
|--------|----------|-----|
| GET | `/api/Pagos` | ? Sí |
| GET | `/api/Pagos/{id}` | ? Sí |
| POST | `/api/Pagos` | ? Sí |
| PUT | `/api/Pagos/{id}` | ? Sí |
| DELETE | `/api/Pagos/{id}` | ? Sí |

---

### **Usuarios** - ?? **MIXTO**

| Método | Endpoint | JWT |
|--------|----------|-----|
| GET | `/api/Usuarios` | ? Sí |
| GET | `/api/Usuarios/{id}` | ? Sí |
| POST | `/api/Usuarios` | ? No |
| PUT | `/api/Usuarios/{id}` | ? Sí |
| DELETE | `/api/Usuarios/{id}` | ? Sí |
| POST | `/api/Usuarios/login` | ? **NO** (público) |

**Nota:** El login es el único endpoint público en Usuarios y tambien el crear para que se registren.

---

### **PDFs** - ? **TODOS requieren JWT**

| Método | Endpoint | JWT |
|--------|----------|-----|
| GET | `/api/Pdfs` | ? No |
| GET | `/api/Pdfs/{id}` | ? No |
| GET | `/api/Pdfs/factura/{idFactura}` | ? No |
| POST | `/api/Pdfs` | ?  Sí |
| PUT | `/api/Pdfs/{id}` | ? No |
| DELETE | `/api/Pdfs/{id}` | ? No |

---

## 2?? **CatalogosService**

Base URL: `https://catalogos-service.onrender.com`

### **Ciudades** - ?? **MIXTO**

| Método | Endpoint | JWT |
|--------|----------|-----|
| GET | `/api/Ciudades` | ? **NO** (público) |
| GET | `/api/Ciudades/{id}` | ? **NO** (público) |
| POST | `/api/Ciudades` | ? Sí |
| PUT | `/api/Ciudades/{id}` | ? Sí |
| DELETE | `/api/Ciudades/{id}` | ? Sí |

---

### **Países** - ?? **MIXTO**

| Método | Endpoint | JWT |
|--------|----------|-----|
| GET | `/api/Paises` | ? **NO** (público) |
| GET | `/api/Paises/{id}` | ? **NO** (público) |
| POST | `/api/Paises` | ? Sí |
| PUT | `/api/Paises/{id}` | ? Sí |
| DELETE | `/api/Paises/{id}` | ? Sí |

---

### **Hoteles** - ?? **MIXTO**

| Método | Endpoint | JWT |
|--------|----------|-----|
| GET | `/api/Hoteles` | ? **NO** (público) |
| GET | `/api/Hoteles/{id}` | ? **NO** (público) |
| POST | `/api/Hoteles` | ? Sí |
| PUT | `/api/Hoteles/{id}` | ? Sí |
| DELETE | `/api/Hoteles/{id}` | ? Sí |

---

### **Tipos de Habitación** - ?? **MIXTO**

| Método | Endpoint | JWT |
|--------|----------|-----|
| GET | `/api/TiposHabitacion` | ? **NO** (público) |
| GET | `/api/TiposHabitacion/{id}` | ? **NO** (público) |
| POST | `/api/TiposHabitacion` | ? Sí |
| PUT | `/api/TiposHabitacion/{id}` | ? Sí |
| DELETE | `/api/TiposHabitacion/{id}` | ? Sí |

---

### **Amenidades** - ?? **MIXTO**

| Método | Endpoint | JWT |
|--------|----------|-----|
| GET | `/api/Amenidades` | ? **NO** (público) |
| GET | `/api/Amenidades/{id}` | ? **NO** (público) |
| POST | `/api/Amenidades` | ? Sí |
| PUT | `/api/Amenidades/{id}` | ? Sí |
| DELETE | `/api/Amenidades/{id}` | ? Sí |

---

### **Roles** - ?? **MIXTO**

| Método | Endpoint | JWT |
|--------|----------|-----|
| GET | `/api/Roles` | ? **NO** (público) |
| GET | `/api/Roles/{id}` | ? **NO** (público) |
| POST | `/api/Roles` | ? Sí |
| PUT | `/api/Roles/{id}` | ? Sí |
| DELETE | `/api/Roles/{id}` | ? Sí |

---

### **Métodos de Pago** - ?? **MIXTO**

| Método | Endpoint | JWT |
|--------|----------|-----|
| GET | `/api/MetodosPago` | ? **NO** (público) |
| GET | `/api/MetodosPago/{id}` | ? **NO** (público) |
| POST | `/api/MetodosPago` | ? Sí |
| PUT | `/api/MetodosPago/{id}` | ? Sí |
| DELETE | `/api/MetodosPago/{id}` | ? Sí |

---

## 3?? **HabitacionesService (GraphQL)**

Base URL: `https://habitaciones-service.onrender.com/graphql`

### **GraphQL Queries** - ?? **Requiere JWT (configurado en header)**

| Operación | Tipo | JWT |
|-----------|------|-----|
| `habitaciones` | Query | ?? Depende de configuración |
| `habitacion(id)` | Query | ?? Depende de configuración |
| `crearHabitacion` | Mutation | ?? Depende de configuración |
| `actualizarHabitacion` | Mutation | ?? Depende de configuración |

**Nota:** GraphQL puede tener autenticación a nivel de resolver. Necesitarías revisar el código específico.

---

## 4?? **ReservasService (gRPC)**

Base URL: `https://reservas-service.onrender.com`

### **gRPC Services** - ? **Requiere JWT**

| Método | Endpoint | JWT |
|--------|----------|-----|
| `ObtenerReservaPorId` | gRPC | ? No |
| `CrearReserva` | gRPC | ? No |
| `ActualizarReserva` | gRPC | ? No |
| Health Check | `/health` | ? NO |

**Nota:** gRPC requiere configuración especial para JWT en metadata.

---

## 5?? **ApiGateway**

Base URL: `https://apigateway-hyaw.onrender.com`

### **Autenticación** - ? **TODOS públicos**

| Método | Endpoint | JWT |
|--------|----------|-----|
| POST | `/api/auth/token` | ? NO (genera token) |
| POST | `/api/auth/validate` | ? NO |
| POST | `/api/auth/refresh` | ? NO |

---

### **Integración (RECA API)** - ? **TODOS públicos**

| Método | Endpoint | JWT |
|--------|----------|-----|
| GET | `/api/integracion/habitaciones` | ? NO |
| POST | `/api/integracion/disponibilidad` | ? NO |
| POST | `/api/integracion/reservas/pre-reserva` | ? NO |
| POST | `/api/integracion/reservas/confirmar` | ? NO |
| POST | `/api/integracion/reservas/cancelar` | ? NO |

**Nota:** Los endpoints de integración son públicos porque son para clientes externos.

---

## ?? **RESUMEN POR SERVICIO**

| Servicio | Totalmente Protegido | Mixto | Totalmente Público |
|----------|---------------------|-------|--------------------|
| **UsuariosPagosService** | ? (excepto login) | | |
| **CatalogosService** | | ? GET públicos, resto protegido | |
| **HabitacionesService** | ?? GraphQL | | |
| **ReservasService** | ? gRPC | | |
| **ApiGateway** | | | ? Auth e Integración |

---

## ?? **PATRÓN COMÚN**

### **CatalogosService:**
- ? **GET** (consulta) ? Público
- ? **POST, PUT, DELETE** (modificación) ? Requiere JWT

### **UsuariosPagosService:**
- ? **TODOS** los endpoints ? Requieren JWT
- ? **EXCEPTO:** `/api/Usuarios/login` ? Público

### **ApiGateway:**
- ? **Auth** ? Público (para generar tokens)
- ? **Integración** ? Público (para clientes externos)

---

## ?? **CÓMO USAR JWT**

### **En Swagger:**

1. Genera token:
```
POST /api/auth/token
{
  "username": "admin",
  "password": "admin123",
  "role": "Admin"
}
```

2. Copia el token

3. Click **"Authorize"** ??

4. Pega el token (Swagger agrega "Bearer" automáticamente)

---

### **En Postman/código:**

```http
GET https://usuarios-pagos-service.onrender.com/api/Facturas
Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...
```

---

## ? **ENDPOINTS QUE NO NECESITAN JWT (PÚBLICOS)**

### **Para desarrollo/pruebas:**

```http
GET https://catalogos-service.onrender.com/api/Ciudades
GET https://catalogos-service.onrender.com/api/Paises
GET https://catalogos-service.onrender.com/api/Hoteles
GET https://catalogos-service.onrender.com/api/TiposHabitacion
GET https://catalogos-service.onrender.com/api/Amenidades
GET https://catalogos-service.onrender.com/api/Roles
GET https://catalogos-service.onrender.com/api/MetodosPago
POST https://apigateway-hyaw.onrender.com/api/auth/token
POST https://usuarios-pagos-service.onrender.com/api/Usuarios/login
```

---

## ?? **IMPORTANTE**

1. **Los GET de catálogos son públicos** porque necesitas consultarlos antes de autenticarte (ej: ciudades para registro)

2. **Los POST/PUT/DELETE de catálogos requieren JWT** para proteger la modificación de datos maestros

3. **UsuariosPagosService es el más restrictivo** porque maneja datos sensibles (usuarios, pagos, facturas)

4. **ApiGateway Auth es público** porque su propósito es generar tokens

5. **GraphQL y gRPC** requieren configuración especial para JWT en headers/metadata

---

<div align="center">

# ?? **LISTA COMPLETA GENERADA** ?

**Total APIs analizadas:** 20+ endpoints  
**Servicios:** 5 microservicios  
**Patrón:** GET público, modificaciones protegidas (catálogos)

</div>
