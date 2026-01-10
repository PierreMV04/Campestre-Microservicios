# ?? LISTA COMPLETA DE APIs - REQUISITOS JWT (ACTUALIZADA)

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
| POST | `/api/Usuarios` | ? **NO** (registro público) |
| PUT | `/api/Usuarios/{id}` | ? Sí |
| DELETE | `/api/Usuarios/{id}` | ? Sí |
| POST | `/api/Usuarios/login` | ? **NO** (público) |

**? CAMBIO APLICADO:** El endpoint POST (registro) y login son públicos para permitir que nuevos usuarios se registren sin JWT.

---

### **PDFs** - ? **TODOS públicos**

| Método | Endpoint | JWT |
|--------|----------|-----|
| GET | `/api/Pdfs` | ? No |
| GET | `/api/Pdfs/{id}` | ? No |
| GET | `/api/Pdfs/factura/{idFactura}` | ? No |
| POST | `/api/Pdfs` | ? No |
| PUT | `/api/Pdfs/{id}` | ? No |
| DELETE | `/api/Pdfs/{id}` | ? No |

**? CAMBIO APLICADO:** Todos los endpoints de PDFs son ahora públicos.

---

### **Funciones Especiales** - ? **TODOS públicos**

| Método | Endpoint | JWT |
|--------|----------|-----|
| POST | `/api/funciones-especiales/prereserva` | ? No |
| GET | `/api/funciones-especiales/prereserva/{idHold}` | ? No |
| DELETE | `/api/funciones-especiales/prereserva/{idHold}` | ? No |
| POST | `/api/funciones-especiales/confirmar` | ? No |
| POST | `/api/funciones-especiales/emitir-factura` | ? No |
| POST | `/api/funciones-especiales/expirar/{idHold}` | ? No |

**? CAMBIO APLICADO:** Todos los endpoints de FuncionesEspeciales son ahora públicos para facilitar la integración.

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

**Nota:** GraphQL puede tener autenticación a nivel de resolver.

---

## 4?? **ReservasService (gRPC)**

Base URL: `https://reservas-service.onrender.com`

### **gRPC Services** - ? **NO requiere JWT**

| Método | Endpoint | JWT |
|--------|----------|-----|
| `ObtenerReservaPorId` | gRPC | ? No |
| `CrearReserva` | gRPC | ? No |
| `ActualizarReserva` | gRPC | ? No |
| Health Check | `/health` | ? NO |

**Nota:** gRPC es usado internamente entre microservicios.

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

---

## ?? **RESUMEN DE CAMBIOS APLICADOS**

| Endpoint | Antes | Ahora | Motivo |
|----------|-------|-------|--------|
| `POST /api/Usuarios` | ? JWT | ? Público | Permitir registro |
| `POST /api/Usuarios/login` | ? Público | ? Público | Sin cambios |
| `/api/Pdfs/*` (todos) | ? JWT | ? Público | Acceso a facturas |
| `/api/funciones-especiales/*` (todos) | ? JWT | ? Público | Flujo reserva |

---

## ?? **PATRÓN ACTUALIZADO**

### **UsuariosPagosService:**
- ? **Facturas, Pagos** ? Requieren JWT
- ? **Usuarios (POST/login), PDFs, FuncionesEspeciales** ? Públicos

### **CatalogosService:**
- ? **GET** (consulta) ? Público
- ? **POST, PUT, DELETE** (modificación) ? Requiere JWT

### **ApiGateway:**
- ? **Auth e Integración** ? Públicos

---

## ? **ENDPOINTS PÚBLICOS (Sin JWT)**

### **Flujo de registro y autenticación:**
```http
POST https://usuarios-pagos-service.onrender.com/api/Usuarios
POST https://usuarios-pagos-service.onrender.com/api/Usuarios/login
POST https://apigateway-hyaw.onrender.com/api/auth/token
```

### **Flujo de pre-reserva y confirmación:**
```http
POST https://usuarios-pagos-service.onrender.com/api/funciones-especiales/prereserva
GET  https://usuarios-pagos-service.onrender.com/api/funciones-especiales/prereserva/{idHold}
POST https://usuarios-pagos-service.onrender.com/api/funciones-especiales/confirmar
POST https://usuarios-pagos-service.onrender.com/api/funciones-especiales/emitir-factura
DELETE https://usuarios-pagos-service.onrender.com/api/funciones-especiales/prereserva/{idHold}
POST https://usuarios-pagos-service.onrender.com/api/funciones-especiales/expirar/{idHold}
```

### **Consulta de catálogos:**
```http
GET https://catalogos-service.onrender.com/api/Ciudades
GET https://catalogos-service.onrender.com/api/Paises
GET https://catalogos-service.onrender.com/api/Hoteles
GET https://catalogos-service.onrender.com/api/TiposHabitacion
GET https://catalogos-service.onrender.com/api/Amenidades
GET https://catalogos-service.onrender.com/api/Roles
GET https://catalogos-service.onrender.com/api/MetodosPago
```

### **PDFs:**
```http
GET https://usuarios-pagos-service.onrender.com/api/Pdfs
GET https://usuarios-pagos-service.onrender.com/api/Pdfs/{id}
GET https://usuarios-pagos-service.onrender.com/api/Pdfs/factura/{idFactura}
```

---

## ?? **CÓMO PROBAR**

### **1. Registro de usuario (sin JWT):**
```http
POST https://usuarios-pagos-service.onrender.com/api/Usuarios
Content-Type: application/json

{
  "idUsuario": 0,
  "idRol": 2,
  "nombreUsuario": "Juan Pérez",
  "correoUsuario": "juan@example.com",
  "contrasenaUsuario": "Password123!",
  "estadoUsuario": true
}
```

### **2. Login (sin JWT):**
```http
POST https://usuarios-pagos-service.onrender.com/api/Usuarios/login
Content-Type: application/json

{
  "correo": "juan@example.com",
  "password": "Password123!"
}
```

### **3. Pre-reserva (sin JWT):**
```http
POST https://usuarios-pagos-service.onrender.com/api/funciones-especiales/prereserva
Content-Type: application/json

{
  "idHabitacion": "HAB001",
  "fechaInicio": "2026-02-01T14:00:00",
  "fechaFin": "2026-02-05T12:00:00",
  "numeroHuespedes": 2,
  "duracionHoldSeg": 900,
  "precioActual": 150.00
}
```

---

## ?? **IMPORTANTE**

1. ? **POST /api/Usuarios ya no requiere JWT** - Permite registro de nuevos usuarios
2. ? **FuncionesEspeciales son públicos** - Facilita el flujo de reserva
3. ? **PDFs son públicos** - Permite descargar facturas sin autenticación
4. ?? **Facturas y Pagos siguen protegidos** - Datos sensibles requieren JWT

---

<div align="center">

# ?? **LISTA ACTUALIZADA** ?

**Cambios aplicados:** 3 controllers modificados  
**Compilación:** ? Exitosa  
**Endpoints públicos:** Registro, Login, FuncionesEspeciales, PDFs

**¡Listo para desplegar!**

</div>
