# ?? COMANDOS EXACTOS PARA PUBLICAR

Copia y pega estos comandos en PowerShell en el orden indicado.

---

## ?? PASO 0: Navegar a la carpeta del proyecto

```powershell
cd "D:\Jossue\Desktop\RETO 3\BACK\V1\Microservicios"
```

---

## ?? PASO 1: Verificar que todo compila

```powershell
# Limpiar builds anteriores
dotnet clean

# Restaurar dependencias
dotnet restore

# Compilar todo
dotnet build --configuration Release
```

**? Resultado esperado:** "Build succeeded"

---

## ?? PASO 2: Crear repositorio en GitHub

### 2.1 En el navegador:

1. Ve a: **https://github.com/new**
2. Nombre del repositorio: **`hotel-microservices`**
3. Descripción: **"Sistema de gestión hotelera con microservicios en .NET 8"**
4. **Público** o **Privado** (como prefieras)
5. **? NO marques:** README, .gitignore, ni licencia
6. Click en **"Create repository"**
7. **Copia la URL** que aparece (ejemplo: `https://github.com/TU_USUARIO/hotel-microservices.git`)

### 2.2 En PowerShell:

```powershell
# Reemplaza TU_USUARIO con tu nombre de usuario de GitHub
.\init-github.ps1 -RepoUrl "https://github.com/TU_USUARIO/hotel-microservices.git"
```

**Si el script pide usuario/contraseña:**
- Usa tu **email de GitHub** como usuario
- Usa un **Personal Access Token** como contraseña (no tu contraseña normal)

**Para generar el token:**
1. GitHub ? Settings ? Developer settings ? Personal access tokens ? Tokens (classic)
2. Generate new token ? Marcar: **repo** (todos)
3. Copiar el token generado

**? Resultado esperado:** "? Código subido a GitHub"

---

## ?? PASO 3: Configurar Railway

### 3.1 Crear cuenta y proyecto

```powershell
# Abre Railway en el navegador
start https://railway.app
```

1. Click en **"Login"**
2. **"Login with GitHub"**
3. Autoriza Railway
4. Click en **"New Project"**
5. **"Deploy from GitHub repo"**
6. Selecciona **"hotel-microservices"**

### 3.2 Crear los 5 servicios

**Por cada servicio (repetir 5 veces):**

1. Click en **"New"** (botón morado)
2. **"GitHub Repo"**
3. Selecciona **"hotel-microservices"**
4. Click en **"Add variables"** ? **"Cancel"** (por ahora)
5. Click en el servicio que se creó
6. **"Settings"** (arriba a la derecha)
7. **"Service"** ? **"Root Directory"**
8. Escribe uno de estos (según el servicio):
   - Primera vez: **`ApiGateway`**
   - Segunda vez: **`CatalogosService`**
   - Tercera vez: **`HabitacionesService`**
   - Cuarta vez: **`ReservasService`**
   - Quinta vez: **`UsuariosPagosService`**
9. Railway redesplegará automáticamente

### 3.3 Agregar PostgreSQL

1. Click en **"New"** (botón morado)
2. **"Database"**
3. **"Add PostgreSQL"**
4. Espera 30 segundos a que se cree
5. Click en **"Postgres"** (el que se creó)
6. Click en **"Variables"**
7. Busca **"DATABASE_URL"**
8. **Click en el icono de copiar** (??)
9. Pégalo en un archivo temporal (lo necesitarás después)

---

## ?? PASO 4: Generar URLs públicas

**Para CADA uno de los 5 servicios:**

1. Click en el servicio (ApiGateway, CatalogosService, etc.)
2. **"Settings"** ? **"Networking"**
3. **"Public Networking"** ? **"Generate Domain"**
4. Espera 5 segundos
5. **Copia la URL** que apareció (ejemplo: `apigateway-production.up.railway.app`)
6. Guárdala en un archivo temporal

**Template para guardar las URLs:**

```
CATALOGOS_SERVICE_URL=https://catalogos-production.up.railway.app
HABITACIONES_SERVICE_URL=https://habitaciones-production.up.railway.app
RESERVAS_SERVICE_URL=https://reservas-production.up.railway.app
USUARIOS_PAGOS_SERVICE_URL=https://usuarios-pagos-production.up.railway.app
API_GATEWAY_URL=https://apigateway-production.up.railway.app
```

---

## ?? PASO 5: Configurar variables de entorno

### 5.1 Para CatalogosService

1. Click en **"CatalogosService"**
2. **"Variables"**
3. **"Raw Editor"** (arriba a la derecha)
4. Pega esto (reemplaza `<DATABASE_URL>` con tu URL real):

```env
ASPNETCORE_ENVIRONMENT=Production
DATABASE_URL=<PEGA_AQUI_EL_DATABASE_URL_DE_POSTGRESQL>
JWT_SECRET_KEY=HotelMicroservicesSecretKey2024!@#$%^&*()_+
RABBITMQ_URL=
```

5. **"Add"** (abajo)

### 5.2 Para HabitacionesService

1. Click en **"HabitacionesService"**
2. **"Variables"** ? **"Raw Editor"**
3. Pega lo mismo:

```env
ASPNETCORE_ENVIRONMENT=Production
DATABASE_URL=<PEGA_AQUI_EL_DATABASE_URL_DE_POSTGRESQL>
JWT_SECRET_KEY=HotelMicroservicesSecretKey2024!@#$%^&*()_+
RABBITMQ_URL=
```

4. **"Add"**

### 5.3 Para ReservasService

1. Click en **"ReservasService"**
2. **"Variables"** ? **"Raw Editor"**
3. Pega lo mismo:

```env
ASPNETCORE_ENVIRONMENT=Production
DATABASE_URL=<PEGA_AQUI_EL_DATABASE_URL_DE_POSTGRESQL>
JWT_SECRET_KEY=HotelMicroservicesSecretKey2024!@#$%^&*()_+
RABBITMQ_URL=
```

4. **"Add"**

### 5.4 Para UsuariosPagosService

1. Click en **"UsuariosPagosService"**
2. **"Variables"** ? **"Raw Editor"**
3. Pega esto (reemplaza las URLs):

```env
ASPNETCORE_ENVIRONMENT=Production
DATABASE_URL=<PEGA_AQUI_EL_DATABASE_URL_DE_POSTGRESQL>
JWT_SECRET_KEY=HotelMicroservicesSecretKey2024!@#$%^&*()_+
RABBITMQ_URL=
RESERVAS_SERVICE_URL=<PEGA_AQUI_LA_URL_DE_RESERVAS_SERVICE>
```

4. **"Add"**

### 5.5 Para ApiGateway (IMPORTANTE)

1. Click en **"ApiGateway"**
2. **"Variables"** ? **"Raw Editor"**
3. Pega esto (reemplaza TODAS las URLs con las que copiaste):

```env
ASPNETCORE_ENVIRONMENT=Production
JWT_SECRET_KEY=HotelMicroservicesSecretKey2024!@#$%^&*()_+
CATALOGOS_SERVICE_URL=<PEGA_URL_DE_CATALOGOS>
HABITACIONES_SERVICE_URL=<PEGA_URL_DE_HABITACIONES>
RESERVAS_SERVICE_URL=<PEGA_URL_DE_RESERVAS>
USUARIOS_PAGOS_SERVICE_URL=<PEGA_URL_DE_USUARIOS_PAGOS>
```

**Ejemplo completo (usa TUS URLs):**
```env
ASPNETCORE_ENVIRONMENT=Production
JWT_SECRET_KEY=HotelMicroservicesSecretKey2024!@#$%^&*()_+
CATALOGOS_SERVICE_URL=https://catalogos-production-a1b2.up.railway.app
HABITACIONES_SERVICE_URL=https://habitaciones-production-c3d4.up.railway.app
RESERVAS_SERVICE_URL=https://reservas-production-e5f6.up.railway.app
USUARIOS_PAGOS_SERVICE_URL=https://usuarios-pagos-production-g7h8.up.railway.app
```

4. **"Add"**

---

## ?? PASO 6: Redesplegar ApiGateway

1. Click en **"ApiGateway"**
2. **"Deployments"**
3. Click en los **3 puntos (...)** del deploy más reciente
4. **"Redeploy"**
5. Espera 1-2 minutos

---

## ? PASO 7: Verificar que funciona

```powershell
# Abre tu API Gateway en el navegador
# Reemplaza con TU URL real
start https://TU-APIGATEWAY.up.railway.app/swagger
```

**? Deberías ver:** Swagger UI con todos los endpoints

**Prueba un endpoint:**
```powershell
# Reemplaza con TU URL
curl https://TU-APIGATEWAY.up.railway.app/api/catalogos/ciudades
```

---

## ?? PASO 8: Conectar con Angular

En tu proyecto Angular, actualiza `src/environments/environment.prod.ts`:

```typescript
export const environment = {
  production: true,
  apiUrl: 'https://TU-APIGATEWAY.up.railway.app'
};
```

---

## ?? VERIFICACIÓN FINAL

Revisa que todos los servicios estén "deployed" (verde) en Railway:

- [ ] ? CatalogosService - Deployed
- [ ] ? HabitacionesService - Deployed
- [ ] ? ReservasService - Deployed
- [ ] ? UsuariosPagosService - Deployed
- [ ] ? ApiGateway - Deployed
- [ ] ? Postgres - Active

---

## ?? ¡LISTO!

Tu API está publicada y funcionando en:

```
https://TU-APIGATEWAY.up.railway.app
```

---

## ?? Si algo sale mal

### Error: "Git authentication failed"

```powershell
# Configura Git
git config --global user.name "Tu Nombre"
git config --global user.email "tu@email.com"

# Genera token en GitHub:
# https://github.com/settings/tokens
```

### Error: "Service deployment failed"

1. Click en el servicio ? **"Deployments"**
2. Click en el deploy fallido ? **"View Logs"**
3. Lee el error y busca en la documentación

### Error: "Database connection failed"

- Verifica que copiaste **DATABASE_URL** correctamente
- Debe empezar con: `postgresql://`
- Asegúrate de que PostgreSQL esté "Active" (verde)

---

## ?? Costo Total

**$0 USD/mes** con el crédito gratuito de Railway ($5 USD)

---

## ?? Tiempo Total Estimado

- Paso 1: 2 minutos
- Paso 2: 3 minutos
- Paso 3: 8 minutos
- Paso 4: 2 minutos
- Paso 5: 5 minutos
- Paso 6: 1 minuto
- Paso 7: 1 minuto

**Total: ~22 minutos**

---

<div align="center">

# ? ¡ÉXITO! ?

Tu sistema de microservicios está publicado.

</div>
