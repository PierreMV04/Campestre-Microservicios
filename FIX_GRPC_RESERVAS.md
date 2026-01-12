# 🔧 FIX: ERROR gRPC ReservasService en Render

---

## ❌ **PROBLEMA**

### **Error en build de Docker:**
```
error CS0246: The type or namespace name 'IEventBus' could not be found
error CS0246: The type or namespace name 'NullEventBus' could not be found
```

### **Error en tiempo de ejecución:**
```
Bad gRPC response. HTTP status code: 400
Status(StatusCode="Internal", Detail="Bad gRPC response. HTTP status code: 400")
```

---

## 🔍 **CAUSAS**

### **1. Faltaba `using Shared.EventBus`**

En `ReservasService/Program.cs` se usaban `IEventBus` y `NullEventBus` pero no se importaba el namespace.

### **2. Protocolo HTTP incorrecto**

Render y Cloudflare hacen TLS termination, por lo que el tráfico llega como HTTP/1.1, pero gRPC requiere HTTP/2.

### **3. Faltaba gRPC-Web**

Para que gRPC funcione detrás de proxies HTTP/1.1 (Render, Cloudflare), necesita gRPC-Web.

---

## ✅ **SOLUCIÓN APLICADA**

### **1. Agregar using statement**

```csharp
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.IdentityModel.Tokens;
using ReservasService.Services;
using Shared.Data;
using Shared.EventBus;  // ✅ AGREGADO
using System.Text;
```

---

### **2. Configurar EventBus con fallback**

**Antes:**
```csharp
builder.Services.AddSingleton<IEventBus, NullEventBus>();
```

**Después:**
```csharp
builder.Services.AddSingleton<IEventBus>(sp =>
{
    var config = sp.GetRequiredService<IConfiguration>();
    var logger = sp.GetRequiredService<ILoggerFactory>()
                   .CreateLogger("EventBus");

    var host = config["RabbitMQ:Host"];

    if (string.IsNullOrWhiteSpace(host))
    {
        logger.LogWarning("RabbitMQ no configurado, usando NullEventBus");
        return new NullEventBus();
    }

    try
    {
        return new RabbitMqEventBus(host);
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "RabbitMQ no disponible, usando NullEventBus");
        return new NullEventBus();
    }
});
```

---

### **3. Habilitar HTTP/1 y HTTP/2**

**Antes:**
```csharp
listenOptions.Protocols = HttpProtocols.Http2;
```

**Después:**
```csharp
listenOptions.Protocols = HttpProtocols.Http1AndHttp2;
```

Esto permite que Kestrel acepte tanto HTTP/1.1 (del proxy de Render) como HTTP/2 (para gRPC directo).

---

### **4. Habilitar gRPC-Web**

```csharp
// En middleware
app.UseGrpcWeb();

// En endpoints
app.MapGrpcService<ReservasGrpcService>().EnableGrpcWeb();
```

gRPC-Web permite que gRPC funcione sobre HTTP/1.1, que es lo que Render/Cloudflare envían después del TLS termination.

---

### **5. Agregar fallback para JWT_SECRET_KEY**

```csharp
var jwtKey = builder.Configuration["Jwt:Key"]
             ?? builder.Configuration["JWT_SECRET_KEY"]
             ?? "HotelMicroservicesSecretKey2024!@#$%^&*()_+";
```

Ahora lee tanto `Jwt:Key` como `JWT_SECRET_KEY` de las variables de entorno.

---

## 📊 **VARIABLES DE ENTORNO NECESARIAS EN RENDER**

Para **ReservasService**, asegúrate de tener:

```
ASPNETCORE_ENVIRONMENT=Production
ASPNETCORE_URLS=http://0.0.0.0:$PORT
JWT_SECRET_KEY=HotelMicroservicesSecretKey2024!@#$%^&*()_+
Jwt__Issuer=HotelMicroservices
Jwt__Audience=HotelMicroservicesClients
```

Opcional (si usas RabbitMQ):
```
RABBITMQ_URL=amqp://...
```

---

## 🚀 **DESPLEGAR**

```powershell
cd "D:\Jossue\Desktop\RETO 3\BACK\V1\Microservicios"
.\update-render.ps1
```

**Tiempo:** 5-7 minutos

---

## 🧪 **VERIFICACIÓN**

### **1. Verificar que el servicio arrancó:**

```
https://reservas-service.onrender.com/health
```

**Respuesta esperada:** 
- HTTP 200 "Healthy"
- O mensaje "An HTTP/1.x request was sent to an HTTP/2 only endpoint" (esto también es bueno, significa que está escuchando)

---

### **2. Probar desde ApiGateway:**

```
GET https://apigateway-hyaw.onrender.com/api/reservas-grpc/fechas-ocupadas/HAJO000001
```

**Antes:** Error 500 "Bad gRPC response"

**Después:** Debería retornar array de fechas ocupadas o array vacío `[]`

---

### **3. Verificar logs en Render:**

Busca en los logs:

```
✅ BUENO:
Now listening on: http://0.0.0.0:10000
Application started
RabbitMQ no configurado, usando NullEventBus

❌ MALO:
error CS0246: The type or namespace name 'IEventBus'
Bad gRPC response
```

---

## 📋 **CHECKLIST DE DESPLIEGUE**

- [x] Código corregido
- [x] using Shared.EventBus agregado
- [x] EventBus configurado con fallback
- [x] HTTP/1 y HTTP/2 habilitados
- [x] gRPC-Web habilitado
- [x] JWT fallback agregado
- [x] Compilación exitosa ✅
- [ ] Cambios subidos a GitHub
- [ ] Render redesplegando
- [ ] Logs sin errores
- [ ] /health responde
- [ ] gRPC endpoints funcionan desde ApiGateway

---

## 🔧 **ARQUITECTURA gRPC EN RENDER**

```
Cliente (Frontend)
       ↓
       ↓ HTTPS
       ↓
Cloudflare (CDN)
       ↓ HTTP/1.1
       ↓
Render Proxy (TLS termination)
       ↓ HTTP/1.1
       ↓
ApiGateway (REST to gRPC adapter)
       ↓ gRPC-Web (HTTP/1.1)
       ↓
ReservasService (Kestrel)
  - Escucha: HTTP/1 y HTTP/2
  - gRPC-Web habilitado
  - Convierte a gRPC nativo
       ↓
Lógica de negocio + SQL Server
```

---

## 💡 **POR QUÉ NECESITAMOS gRPC-Web**

### **Problema:**
- gRPC **requiere HTTP/2**
- Render/Cloudflare hacen **TLS termination**
- El tráfico llega como **HTTP/1.1** al container

### **Solución:**
- **gRPC-Web** es un protocolo que funciona sobre HTTP/1.1
- Kestrel convierte gRPC-Web → gRPC nativo internamente
- Transparente para el código de negocio

---

## 🎯 **ENDPOINTS AFECTADOS**

Todos los endpoints gRPC en ApiGateway ahora deberían funcionar:

```
GET  /api/reservas-grpc/fechas-ocupadas/{idHabitacion}
GET  /api/reservas-grpc/habxres
GET  /api/reservas-grpc/reservas
POST /api/reservas-grpc/crear-reserva
```

---

## ⚠️ **TROUBLESHOOTING**

### **Si sigue dando "Bad gRPC response":**

1. **Verifica variables de entorno** en Render (ReservasService)
2. **Revisa logs** para ver error específico
3. **Prueba /health** primero para confirmar que arranca
4. **Espera 30-60 seg** si acabas de desplegar (cold start)

### **Si da error 400:**

Probablemente el request no está en formato gRPC correcto. Verifica que ApiGateway esté usando el cliente gRPC correctamente.

### **Si da timeout:**

El servicio está dormido. Espera 30-60 segundos para que despierte.

---

## 📚 **ARCHIVOS MODIFICADOS**

1. ✅ `ReservasService/Program.cs`
2. ✅ `update-render.ps1`
3. ✅ Este documento: `FIX_GRPC_RESERVAS.md`

---

<div align="center">

# ✅ **FIX COMPLETO** ✅

**Problema:** IEventBus no encontrado + gRPC no funciona en Render

**Solución:** using agregado + gRPC-Web habilitado + HTTP/1+2

**Ejecuta:** `.\update-render.ps1`

**Espera:** 5-7 minutos

**Verifica:** `/health` y endpoints gRPC

</div>
