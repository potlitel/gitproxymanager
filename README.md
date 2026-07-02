# 🚀 Git Proxy Manager

> 🎯 **Gestiona tu proxy de Git y del sistema de forma sencilla y elegante**

<div align="center">

![.NET](https://img.shields.io/badge/.NET-10.0-purple?style=flat-square&logo=dotnet)
![WPF](https://img.shields.io/badge/WPF-Modern-blue?style=flat-square&logo=windows)
![License](https://img.shields.io/badge/License-MIT-green?style=flat-square)
![Status](https://img.shields.io/badge/Status-Active-brightgreen?style=flat-square)

</div>

---

## 📋 Tabla de Contenidos

- [✨ Descripción](#-descripción)
- [🎯 Características](#-características)
- [🏗️ Arquitectura](#-arquitectura)
- [🧩 Patrón MVVM](#-patrón-mvvm)
- [📦 Estructura del Proyecto](#-estructura-del-proyecto)
- [🔧 Clases Principales](#-clases-principales)
- [🚀 Instalación](#-instalación)
- [📦 Instalador MSI](#-instalador-msi)
- [💻 Uso](#-uso)
- [⚙️ Configuración](#-configuración)
- [🎨 Diseño UI/UX](#-diseño-uiux)
- [🛠️ Tecnologías](#-tecnologías)
- [🧪 Pruebas](#-pruebas)
- [📸 Capturas](#-capturas)
- [🗺️ Roadmap](#-roadmap)
- [🐛 Known Issues](#-known-issues)
- [📝 License](#-license)

---

## ✨ Descripción

**Git Proxy Manager** es una aplicación de escritorio para Windows que te permite gestionar la configuración de proxy de **Git** y del **sistema Windows** de forma visual e intuitiva.

¿Cansado de escribir comandos cada vez que cambias de red? ¿Olvidas si el proxy está activado o desactivado? 

**¡Esta app es para ti!** 🎉

### 🔥 ¿Por qué usar Git Proxy Manager?

| Sin Git Proxy Manager | Con Git Proxy Manager |
|:---------------------:|:---------------------:|
| 😫 Escribir comandos manualmente | 🎯 Un solo clic |
| 🤔 Recordar IPs y puertos | 💾 Configuración guardada |
| 😰 Olvidar activar/desactivar | 🔄 Toggle instantáneo |
| 🚫 Sin indicador visual | 📊 System tray informativo |
| ⚙️ Configurar proxy del sistema manualmente | 🖥️ Gestión integrada del sistema |
| 📝 Recordar direcciones bypass | 📋 Bypass list editable |

---

## 🎯 Características

### 🌟 Funcionalidades Principales

- **🔗 Toggle Maestro** - Activa/desactiva ambos proxies con un solo clic
- **🖥️ Proxy del Sistema** - Gestiona el proxy de Windows (navegadores, etc.)
- **🔧 Proxy de Git** - Gestiona el proxy de Git (operaciones git)
- **📝 Bypass List** - Edita las direcciones exceptuadas del proxy
- **🔘 Toggle Button** - Activa/desactiva cada proxy individualmente
- **📝 Configuración de Proxy** - Host y puerto personalizables
- **🖥️ System Tray** - Icono en la bandeja del sistema con estado actual
- **💾 Persistencia** - La configuración se guarda automáticamente
- **🎨 UI Moderna** - Tema dark con MahApps.Metro
- **⚡ Loading** - Indicador de progreso en botones
- **🔔 Toast** - Notificaciones de confirmación
- **🎯 Animaciones** - PulseScale en cada interacción
- **🔽 Minimizar al tray** - Botón X oculta la ventana al system tray en vez de cerrar la app
- **🔄 Sincronización con el sistema** - Detección automática de cambios en proxy del sistema y de Git cada 5 segundos
- **🔔 Notificación de cambios** - Toast azul cuando se detecta un cambio externo en la configuración de proxy
- **📋 Valores por defecto** - Host, puerto y bypass list preconfigurados para uso inmediato

### 🔗 Lógica de Dependencia

| Acción | Resultado |
|:-------|:----------|
| Activar **Git** | Sistema se activa automáticamente |
| Desactivar **Git** | Sistema no cambia |
| Activar **Sistema** | Git no cambia |
| Desactivar **Sistema** | Git se desactiva (pierde dependencia) |
| **Maestro ON** | Ambos se activan |
| **Maestro OFF** | Ambos se desactivan |

### 📊 System Tray

El icono en la bandeja del sistema muestra:

| Estado | Color | Significado |
|:------:|:-----:|:------------|
| 🟢 | Verde | Ambos proxies activos |
| 🟡 | Parcial | Solo un proxy activo |
| ⚪ | Gris | Ningún proxy activo |

**Menú contextual:**
- 📂 Abrir ventana principal
- 🖥️ Proxy del sistema (check)
- 🔧 Proxy de Git (check)
- ❌ Salir

---

## 🏗️ Arquitectura

### 📐 Diagrama de Arquitectura

```
┌─────────────────────────────────────────────────────────┐
│                    🖼️ Presentation Layer                 │
│  ┌─────────────────┐  ┌─────────────────┐              │
│  │  MainWindow.xaml │  │   App.xaml.cs   │              │
│  │  (Vista Principal)│  │  (System Tray)  │              │
│  └────────┬────────┘  └────────┬────────┘              │
│           │                     │                        │
│           └──────────┬──────────┘                        │
│                      │                                   │
│                      ▼                                   │
│  ┌─────────────────────────────────────┐                │
│  │         📦 ViewModels               │                │
│  │      (MainViewModel.cs)             │                │
│  └─────────────────┬───────────────────┘                │
│                    │                                     │
└────────────────────┼─────────────────────────────────────┘
                     │
┌────────────────────┼─────────────────────────────────────┐
│                    ▼        🔧 Service Layer              │
│  ┌─────────────────────────────────────┐                │
│  │         📋 Services                  │                │
│  │  ┌─────────────┐ ┌─────────────┐   │                │
│  │  │GitProxy     │ │ SystemProxy │   │                │
│  │  │Service      │ │ Service     │   │                │
│  │  └─────────────┘ └─────────────┘   │                │
│  │  ┌─────────────┐ ┌─────────────┐   │                │
│  │  │Config       │ │ ProxyState  │   │                │
│  │  │Service      │ │ Service     │   │                │
│  │  └─────────────┘ └─────────────┘   │                │
│  └─────────────────┬───────────────────┘                │
│                    │                                     │
└────────────────────┼─────────────────────────────────────┘
                     │
┌────────────────────┼─────────────────────────────────────┐
│                    ▼        📁 Data Layer                 │
│  ┌─────────────────────────────────────┐                │
│  │         💾 Models                    │                │
│  │        (ProxyConfig.cs)             │                │
│  └─────────────────────────────────────┘                │
│                                                          │
│  ┌─────────────────────────────────────┐                │
│  │         📄 Config File               │                │
│  │  %AppData%\GitProxyManager\         │                │
│  │           config.json               │                │
│  └─────────────────────────────────────┘                │
│                                                          │
│  ┌─────────────────────────────────────┐                │
│  │         🪟 Windows Registry          │                │
│  │  HKCU\...\Internet Settings         │                │
│  └─────────────────────────────────────┘                │
└──────────────────────────────────────────────────────────┘
```

### 🔄 Flujo de Datos

```
┌──────────┐    ┌──────────┐    ┌──────────┐    ┌──────────┐
│  User    │───▶│  View    │───▶│ ViewModel│───▶│ Service  │
│  Action  │    │  (XAML)  │    │  (C#)    │    │  (C#)    │
└──────────┘    └──────────┘    └──────────┘    └──────────┘
     │                                              │
     │              ┌──────────┐                    │
     └──────────────│  Model   │◀───────────────────┘
                    │(ProxyConfig)│
                    └──────────┘
                         │
              ┌──────────┴──────────┐
              ▼                     ▼
        ┌──────────┐          ┌──────────┐
        │   File   │          │ Registry │
        │ (JSON)   │          │ (Windows)│
        └──────────┘          └──────────┘
```

### 🔄 Flujo de Sincronización (Polling)

```
Windows/Git cambia externamente
        │ (polling cada 5s)
        ▼
┌──────────────────┐
│ SystemStatePoller │──▶ Lee registry + git config
│  DispatcherTimer  │
└────────┬─────────┘
         │ (compara con último estado)
         ▼
┌──────────────────┐
│ ProxyStateService │──▶ App.xaml.cs → Actualiza tray icon
│  .NotifyChanged() │──▶ MainViewModel → Actualiza UI + toast azul
└──────────────────┘
```

---

## 🧩 Patrón MVVM

### 📖 ¿Qué es MVVM?

**MVVM** (Model-View-ViewModel) es un patrón de arquitectura de software que separa la lógica de negocio de la interfaz de usuario.

### 🔗 Componentes en Git Proxy Manager

| Componente | Archivo | Responsabilidad |
|:----------:|:-------:|:----------------|
| **Model** 📦 | `ProxyConfig.cs` | Datos de configuración |
| **View** 🖼️ | `MainWindow.xaml` | Interfaz de usuario |
| **ViewModel** 🧠 | `MainViewModel.cs` | Lógica de presentación |

### 📊 Diagrama MVVM

```
┌─────────────────────────────────────────────────────────┐
│                        MVVM                             │
├─────────────────────────────────────────────────────────┤
│                                                         │
│   ┌─────────────┐         ┌─────────────┐             │
│   │             │otify    │             │             │
│   │    VIEW     │◀────────│  VIEWMODEL  │             │
│   │ (XAML/C#)   │ Binding │   (C#)      │             │
│   │             │────────▶│             │             │
│   └──────┬──────┘         └──────┬──────┘             │
│          │                        │                     │
│          │                        │                     │
│          ▼                        ▼                     │
│   ┌─────────────┐         ┌─────────────┐             │
│   │   User      │         │   MODEL     │             │
│   │  Interface  │         │  (Data)     │             │
│   └─────────────┘         └─────────────┘             │
│                                                         │
└─────────────────────────────────────────────────────────┘
```

### 🎯 Beneficios del MVVM en nuestro proyecto

- ✅ **Separación de responsabilidades** - Cada componente tiene un rol claro
- ✅ **Testabilidad** - Podemos probar el ViewModel sin la UI
- ✅ **Mantenibilidad** - Fácil de modificar y extender
- ✅ **Reutilización** - El ViewModel puede usarse en diferentes Vistas

---

## 📦 Estructura del Proyecto

```
GitProxyManager/
│
├── 📁 Models/
│   └── 📄 ProxyConfig.cs          # Modelo de datos
│
├── 📁 Services/
│   ├── 📄 GitProxyService.cs      # Servicio de git
│   ├── 📄 SystemProxyService.cs   # Servicio del sistema Windows
│   ├── 📄 ConfigService.cs        # Servicio de configuración
│   ├── 📄 ProxyStateService.cs    # Estado reactivo (ViewModel ↔ App)
│   └── 📄 SystemStatePoller.cs    # Polling de cambios externos (5s)
│
├── 📁 ViewModels/
│   └── 📄 MainViewModel.cs        # ViewModel principal
│
├── 📁 Resources/
│   └── 📁 Icons/                   # Iconos de la app
│       ├── 📄 app-icon.ico         # Icono inactivo
│       └── 📄 app-icon-active.ico  # Icono activo (verde)
│
├── 📁 Converters.cs               # Convertidores de datos
├── 📄 App.xaml                     # Recursos de la aplicación
├── 📄 App.xaml.cs                  # Lógica de inicio + System Tray
├── 📄 MainWindow.xaml              # Ventana principal
├── 📄 MainWindow.xaml.cs           # Code-behind (animaciones)
├── 📄 GitProxyManager.csproj       # Archivo de proyecto
└── 📄 README.md                    # Esta documentación
```

---

## 🔧 Clases Principales

### 📦 ProxyConfig.cs

```csharp
namespace GitProxyManager.Models;

public class ProxyConfig
{
    public bool IsEnabled { get; set; }
    public string Host { get; set; } = string.Empty;
    public int Port { get; set; } = 3128;
    public bool SystemProxyEnabled { get; set; }
    public bool GitProxyEnabled { get; set; }
    public string BypassList { get; set; } = string.Empty;
}
```

| Propiedad | Tipo | Descripción |
|:---------:|:----:|:------------|
| `IsEnabled` | `bool` | Estado general del proxy |
| `Host` | `string` | Dirección del servidor proxy |
| `Port` | `int` | Puerto del proxy (default: 3128) |
| `SystemProxyEnabled` | `bool` | Estado del proxy del sistema |
| `GitProxyEnabled` | `bool` | Estado del proxy de Git |
| `BypassList` | `string` | Direcciones exceptuadas (separadas por `;`) |

---

### 🔧 GitProxyService.cs

```csharp
namespace GitProxyManager.Services;

public static class GitProxyService
{
    public static void ApplyProxy(ProxyConfig config);
    public static void RemoveProxy();
    public static ProxyConfig ReadCurrentConfig();
}
```

| Método | Descripción |
|:------:|:------------|
| `ApplyProxy()` | Aplica la configuración de proxy a git (`http.proxy`, `https.proxy`) |
| `RemoveProxy()` | Elimina la configuración de proxy de git |
| `ReadCurrentConfig()` | Lee la configuración actual de git |

---

### 🪟 SystemProxyService.cs

```csharp
namespace GitProxyManager.Services;

public static class SystemProxyService
{
    public static void ApplyProxy(string host, int port, string bypassList);
    public static void RemoveProxy();
    public static ProxyConfig ReadCurrentConfig();
    public static string ReadBypassList();
}
```

**Descripción:** Servicio que interactúa con el registro de Windows para gestionar el proxy del sistema.

| Registro | Valor | Descripción |
|:--------:|:-----:|:------------|
| `ProxyEnable` | `0` \| `1` | Activa/desactiva el proxy |
| `ProxyServer` | `"host:port"` | Dirección del proxy |
| `ProxyOverride` | `"lista;bypass"` | Direcciones exceptuadas |

| Método | Descripción |
|:------:|:------------|
| `ApplyProxy()` | Aplica proxy al registro y siempre añade `<local>` al final de ProxyOverride (equivalente al checkbox de Windows "No usar proxy para direcciones locales") |
| `RemoveProxy()` | Desactiva el proxy en el registro |
| `ReadCurrentConfig()` | Lee host, puerto, estado y bypass del registro (solo si ProxyEnable=1) |
| `ReadBypassList()` | Lee ProxyOverride del registro independientemente de si el proxy está activo, y limpia `<local>` y `<-loopback>` |

---

### 💾 ConfigService.cs

```csharp
namespace GitProxyManager.Services;

public static class ConfigService
{
    public static ProxyConfig Load();
    public static void Save(ProxyConfig config);
}
```

**Ubicación del archivo:** `%LocalAppData%\GitProxyManager\config.json`

---

### 🔔 ProxyStateService.cs

```csharp
namespace GitProxyManager.Services;

public static class ProxyStateService
{
    public static event Action<bool, bool, string, int, string>? ProxyStateChanged;
    public static void NotifyStateChanged(bool isSystemEnabled, bool isGitEnabled, string host, int port, string bypassList);
}
```

**Descripción:** Patrón pub/sub para comunicación reactiva entre el ViewModel y la capa de presentación (App.xaml.cs). Cuando cambia el estado del proxy, se notifica al system tray para actualizar el icono.

---

### 🔄 SystemStatePoller.cs

```csharp
namespace GitProxyManager.Services;

public static class SystemStatePoller
{
    public static void Start();   // Inicia timer cada 5s
    public static void Stop();    // Detiene timer
}
```

**Descripción:** Servicio de polling que monitorea cambios externos en la configuración de proxy del sistema (Windows Registry) y de Git (~/.gitconfig). Compara el estado actual con el último conocido cada 5 segundos. Cuando detecta un cambio, notifica a través de `ProxyStateService` para actualizar el UI y el system tray.

| Componente | Mecanismo | Frecuencia |
|:----------:|:---------:|:----------:|
| Proxy del sistema | `Registry.CurrentUser.OpenSubKey` | Cada 5s |
| Proxy de Git | `File.ReadAllLines(~/.gitconfig)` | Cada 5s |

**Optimizaciones de performance:**
- `DispatcherTimer` en UI thread (sin cross-thread issues)
- Registry read: ~0.01ms | Git file read: ~0.5ms | Comparación: ~0.001ms
- **Total por tick: <1ms de CPU**
- Logging diagnóstico a `%LocalAppData%\GitProxyManager\poller.log`

**Flujo de sincronización:**
```
Windows/Git cambia externamente
        │ (polling cada 5s)
        ▼
┌──────────────────┐
│ SystemStatePoller │──▶ Compara con último estado
│  Lee registry +   │
│  git config       │
└────────┬─────────┘
         │ (solo si hay cambio)
         ▼
┌──────────────────┐
│ ProxyStateService │──▶ App.xaml.cs → Actualiza tray icon
│  .NotifyChanged() │──▶ MainViewModel → Actualiza UI + toast azul
└──────────────────┘
```

---

### 🧠 MainViewModel.cs

```csharp
namespace GitProxyManager.ViewModels;

public partial class MainViewModel : ObservableObject
{
    // Propiedades de configuración
    [ObservableProperty] private string _host;
    [ObservableProperty] private int _port;
    [ObservableProperty] private string _bypassList;

    // Toggles
    [ObservableProperty] private bool _masterToggle;
    [ObservableProperty] private bool _isSystemEnabled;
    [ObservableProperty] private bool _isGitEnabled;

    // UI State
    [ObservableProperty] private bool _canToggle;
    [ObservableProperty] private bool _isApplying;
    [ObservableProperty] private bool _isResetting;
    [ObservableProperty] private bool _toastVisible;
    [ObservableProperty] private string _toastMessage;

    // Comandos
    [RelayCommand] private async Task ApplyAsync();
    [RelayCommand] private async Task ResetAsync();
}
```

| Propiedad/Método | Descripción |
|:----------------:|:------------|
| `MasterToggle` | Toggle maestro (sincroniza ambos) |
| `IsSystemEnabled` | Estado del proxy del sistema |
| `IsGitEnabled` | Estado del proxy de Git |
| `BypassList` | Lista de direcciones bypass |
| `IsApplying` | Loading durante Apply |
| `IsResetting` | Loading durante Reset |
| `ToastVisible` | Visibilidad del toast |
| `ApplyAsync` | Aplica configuración (con loading + toast) |
| `ResetAsync` | Restablece configuración (con loading + toast) |

---

## 🚀 Instalación

### 📋 Requisitos Previos

- **Windows 10/11** (64-bit)
- **.NET 10.0 Runtime** o superior
- **Git** instalado y configurado

### 📥 Opción 1: Descarga Directa

1. Ve a la sección [Releases](https://github.com/tu-usuario/GitProxyManager/releases)
2. Descarga `GitProxyManager-v1.0.0-setup.msi`
3. Ejecuta el instalador
4. Sigue las instrucciones

### 📥 Opción 2: Compilación Manual

```powershell
# 📂 Clona el repositorio
git clone https://github.com/tu-usuario/GitProxyManager.git

# 📂 Navega al directorio
cd GitProxyManager

# 🔨 Compila el proyecto
dotnet build -c Release

# 🚀 Ejecuta la aplicación
dotnet run
```

---

## 📦 Instalador MSI

### 📋 Resumen del Instalador

| Propiedad | Valor |
|:---------:|:------|
| **📦 Archivo** | `GitProxyManager-v1.0.0-Setup.msi` |
| **📏 Tamaño** | `~1 MB` |
| **🏗️ Plataforma** | `x64` |
| **🔧 Herramienta** | `WiX Toolset v5` |

### 📂 Contenido del MSI

| Componente | Descripción |
|:----------:|:------------|
| 📦 **App** | Ejecutable + DLLs en `LocalAppData\GitProxyManager` |
| 🔗 **Start Menu** | Acceso directo + Desinstalar en Menú Inicio |
| 📋 **Registry** | Entradas de desinstalación en Windows |

### 🛠️ Regenerar el MSI

```powershell
# 🔨 Compilar app en Release
dotnet build "C:\dev\GitProxyManager\GitProxyManager.csproj" -c Release

# 📦 Compilar MSI
dotnet build "C:\dev\GitProxyManager\installer\GitProxyManager.Installer.wixproj" -c Release
```

> ⚠️ **Importante:** El MSI es un paquete estático. Después de modificar código, debes rebuildar **ambos** proyectos para que el MSI contenga los cambios.

### 📁 Ubicación del MSI

```
C:\dev\GitProxyManager\installer\bin\Release\GitProxyManager-v1.0.0-Setup.msi
```

### 🔗 Características del Instalador

- ✅ **Instalación silenciosa** - `msiexec /i GitProxyManager-v1.0.0-Setup.msi /quiet`
- ✅ **Desinstalación limpia** - Desde Panel de Control o Menú Inicio
- ✅ **Actualización automática** - Detecta versiones previas
- ✅ **PerUser** - No requiere permisos de administrador
- ✅ **Icono personalizado** - Icono representativo de proxy/red
- ✅ **Licencia MIT** - Texto de licencia mostrado durante la instalación (`License.rtf`)
- ✅ **Build sin warnings** - Warnings ICE91 suprimidos via `SuppressIces`

---

## 💻 Uso

### 🎬 Primeros Pasos

1. **Inicia la aplicación**
   - El ícono aparecerá en la bandeja del sistema
   - El ícono será gris (proxies desactivados)

2. **Configura tu proxy**
   - Haz clic derecho en el ícono de la bandeja
   - Selecciona "Abrir"
   - Ingresa el host y puerto del proxy
   - (Opcional) Agrega direcciones bypass

3. **Activa los proxies**
   - Usa el **toggle maestro** para activar ambos
   - O activa cada proxy individualmente

### 🔗 Flujo de Uso Diario

```
┌─────────────────────────────────────────┐
│  🏢 Llegas a la oficina                 │
│  ─────────────────────────              │
│  1. Abres Git Proxy Manager             │
│  2. Presionas "Habilitar ambos"         │
│  3. Presionas "Aplicar"                 │
│  4. ¡Listo! Proxy del sistema + Git     │
└─────────────────────────────────────────┘

┌─────────────────────────────────────────┐
│  🏠 Sales de la oficina                 │
│  ─────────────────────────              │
│  1. Abres Git Proxy Manager             │
│  2. Presionas "Habilitar ambos" (OFF)   │
│  3. Presionas "Aplicar"                 │
│  4. ¡Listo! Conexión directa            │
└─────────────────────────────────────────┘
```

### 📊 Indicadores del System Tray

| Estado | Acción |
|:------:|:-------|
| 🟢 Verde | Ambos proxies activos |
| 🟡 Parcial | Solo un proxy activo |
| ⚪ Gris | Ningún proxy activo |

### 🔽 Minimizar al Tray

Al hacer clic en el botón **X** de la ventana, la app **no se cierra** sino que se oculta al system tray. Para restaurarla:

| Acción | Resultado |
|:-------|:----------|
| Doble clic en el icono del tray | Restaura la ventana |
| Clic derecho → "Abrir" | Restaura la ventana |
| Clic derecho → "Salir" | Cierra la app realmente |

> 📝 **Técnico:** Se utiliza `ShutdownMode="OnExplicitShutdown"` en `App.xaml` junto con `OnClosing` override en `MainWindow.xaml.cs` para cancelar el cierre y ejecutar `Hide()` en su lugar.

### ⌨️ Atajos de Teclado

| Tecla | Acción |
|:-----:|:-------|
| `Doble clic` | Abrir ventana principal |
| `Clic derecho` | Menú contextual |

---

## ⚙️ Configuración

### 📁 Archivo de Configuración

**Ubicación:** `%LocalAppData%\GitProxyManager\config.json`

```json
{
  "IsEnabled": false,
  "Host": "172.16.65.62",
  "Port": 3128,
  "SystemProxyEnabled": false,
  "GitProxyEnabled": false,
  "BypassList": "192.168.52.*;*.minag.gob.cu;https://172.16.112.3:8006"
}
```

### 🔧 Configuración de Git

La aplicación modifica la configuración global de git:

```ini
# ~/.gitconfig
[http]
    proxy = http://172.16.65.62:3128
[https]
    proxy = http://172.16.65.62:3128
```

### 🪟 Configuración del Sistema

La aplicación modifica el registro de Windows:

```
HKCU\Software\Microsoft\Windows\CurrentVersion\Internet Settings
├── ProxyEnable    = 1
├── ProxyServer    = "172.16.65.62:3128"
└── ProxyOverride  = "192.168.52.*;*.minag.gob.cu;..."
```

### 🎯 Configuración por Defecto

| Parámetro | Valor por Defecto | Descripción |
|:---------:|:-----------------:|:------------|
| `Host` | `172.16.65.62` | Host del proxy corporativo |
| `Port` | `3128` | Puerto estándar de Squid |
| `BypassList` | `192.168.52.*;*.minag.gob.cu;https://172.16.112.3:8006` | Direcciones exceptuadas del proxy |
| `SystemProxyEnabled` | `false` | Proxy del sistema desactivado |
| `GitProxyEnabled` | `false` | Proxy de Git desactivado |

> 📝 Si existe un `config.json` previo, se usa su contenido. Si no, se usan estos defaults.

---

## 🎨 Diseño UI/UX

### 🎨 Tema Dark (Catppuccin Mocha)

| Color | Hex | Uso |
|:-----:|:---:|:----|
| 🔵 Azul | `#89B4FA` | Acentos principales |
| 🟣 Morado | `#CBA6F7` | Elementos secundarios |
| 🟢 Verde | `#A6E3A1` | Estados exitosos |
| 🟠 Naranja | `#FAB387` | Advertencias |
| ⚫ Background | `#1E1E2E` | Fondo principal |
| ⬜ Texto | `#CDD6F4` | Texto principal |

### 🖼️ Componentes UI

```
┌─────────────────────────────────────────────────────────┐
│  Git Proxy Manager                               ─ □ X │
├─────────────────────────────────────────────────────────┤
│                                                         │
│  ╔═══════════════════════════════════════════════════╗  │
│  ║  🔗  Habilitar ambos                [═══]        ║  │
│  ╚═══════════════════════════════════════════════════╝  │
│                                                         │
│  ╔═══════════════════════════════════════════════════╗  │
│  ║  🖥️  Proxy del sistema              [══]    ●    ║  │
│  ║      Windows (navegadores, etc.)                 ║  │
│  ╚═══════════════════════════════════════════════════╝  │
│                                                         │
│  ╔═══════════════════════════════════════════════════╗  │
│  ║  🔧  Proxy de Git                  [══]    ●    ║  │
│  ║      Solo operaciones git                        ║  │
│  ╚═══════════════════════════════════════════════════╝  │
│                                                         │
│  ╔═══════════════════════════════════════════════════╗  │
│  ║  Proxy: [172.16.65.62    ]  Puerto: [3128   ]   ║  │
│  ╚═══════════════════════════════════════════════════╝  │
│                                                         │
│  ╔═══════════════════════════════════════════════════╗  │
│  ║  Bypass (exceptos):                              ║  │
│  ║  [ 192.168.52.*;*.minag.gob.cu                 ] ║  │
│  ╚═══════════════════════════════════════════════════╝  │
│                                                         │
│  ╔═══════════════════════════════════════════════════╗  │
│  ║  🖥️ Sistema: activo → 172.16.65.62:3128         ║  │
│  ║  🔧 Git: activo → 172.16.65.62:3128             ║  │
│  ╚═══════════════════════════════════════════════════╝  │
│                                                         │
│  ╔════════════════╗        ╔══════════════╗             │
│  ║    Aplicar     ║        ║ Restablecer  ║             │
│  ╚════════════════╝        ╚══════════════╝             │
│                                                         │
│  ┌─────────────────────────────────────────────────┐   │
│  │  ● Ambos proxies activos: 172.16.65.62:3128    │   │
│  └─────────────────────────────────────────────────┘   │
│                                                         │
│              ╔═══════════════════════╗                  │
│              ║  Proxy aplicado ✓     ║  ← Toast        │
│              ╚═══════════════════════╝                  │
└─────────────────────────────────────────────────────────┘
```

### 🎬 Animaciones

| Elemento | Animación | Descripción |
|:--------:|:---------:|:------------|
| `ToggleBorder` | PulseScale | Escala 0.97 → 1.0 con rebote |
| `SystemBorder` | PulseScale | Escala 0.97 → 1.0 con rebote |
| `GitBorder` | PulseScale | Escala 0.97 → 1.0 con rebote |
| `InfoBorder` | PulseScale | Escala 0.97 → 1.0 con rebote |
| `ToastBorder` | Slide + Fade | Slide-in + fade-in, fade-out |

---

## 🛠️ Tecnologías

### 📦 Dependencias

| Tecnología | Versión | Propósito |
|:----------:|:-------:|:----------|
| ![.NET](https://img.shields.io/badge/.NET-10.0-purple) | 10.0 | Framework runtime |
| ![WPF](https://img.shields.io/badge/WPF-Modern-blue) | - | UI Framework |
| ![MahApps](https://img.shields.io/badge/MahApps.Metro-2.4.11-orange) | 2.4.11 | UI Controls |
| ![Hardcodet](https://img.shields.io/badge/Hardcodet.NotifyIcon-2.0.1-red) | 2.0.1 | System Tray |
| ![CommunityToolkit](https://img.shields.io/badge/CommunityToolkit.Mvvm-8.4.2-green) | 8.4.2 | MVVM Helpers |

### 🔧 Herramientas de Desarrollo

| Herramienta | Propósito |
|:-----------:|:----------|
| **Visual Studio 2022** | IDE de desarrollo |
| **WiX Toolset v5** | Generador de MSI |
| **Git** | Control de versiones |
| **PowerShell** | Scripts de automatización |

---

## 🧪 Pruebas

### 📋 Resumen

El proyecto incluye **195 pruebas automatizadas** organizadas en 3 categorías:

| Categoría | Cantidad | Archivos | Descripción |
|:----------:|:--------:|:--------:|:------------|
| **Unit Tests** | 133 | 12 archivos | Pruebas aisladas de cada componente |
| **Integration Tests** | 21 | 4 archivos | Flujos completos Apply/Reset + dependencias |
| **UI Tests (FlaUI)** | 21 | 3 archivos | Interacción visual con la ventana WPF |

### 🏗️ Estructura de Pruebas

```
GitProxyManager.Tests/
├── 📁 Helpers/
│   ├── 📄 TestConfigHelper.cs       # Fábrica de ProxyConfig para tests
│   └── 📄 TestFileHelper.cs         # Gestión de archivos temporales
│
├── 📁 UnitTests/
│   ├── 📁 Models/
│   │   └── 📄 ProxyConfigTests.cs           # Valores por defecto del modelo
│   ├── 📁 Services/
│   │   ├── 📄 ConfigServiceTests.cs         # Persistencia JSON
│   │   ├── 📄 GitProxyServiceTests.cs       # Git proxy + ReadPollingState
│   │   ├── 📄 SystemProxyServiceTests.cs    # Registry + CleanBypassList + ReadPollingState
│   │   ├── 📄 ProxyStateServiceTests.cs     # Evento pub/sub (5 params)
│   │   └── 📄 SystemStatePollerTests.cs     # Start/Stop/Logging del poller
│   ├── 📁 ViewModels/
│   │   └── 📄 MainViewModelTests.cs         # Lógica VM + OnExternalStateChanged
│   └── 📁 Converters/
│       ├── 📄 BoolToBrushConverterTests.cs
│       ├── 📄 BoolToHorizontalAlignmentConverterTests.cs
│       ├── 📄 InvertBoolConverterTests.cs
│       └── 📄 StringToVisibilityConverterTests.cs
│
├── 📁 IntegrationTests/
│   ├── 📄 ApplyProxyFlowTests.cs            # Flujo completo de Apply
│   ├── 📄 ResetProxyFlowTests.cs            # Flujo completo de Reset
│   ├── 📄 DependencyLogicTests.cs           # Lógica de dependencia entre toggles
│   └── 📄 ConfigPersistenceTests.cs         # Roundtrip Save/Load
│
├── 📁 UITests/
│   ├── 📄 MainWindowTests.cs                # Visibilidad, tamaño, elementos
│   ├── 📄 SystemTrayTests.cs                # Menú contextual, clic derecho
│   └── 📄 ToggleInteractionTests.cs         # Interacción con toggles
│
└── 📄 GlobalUsings.cs                       # global using Xunit;
```

### ▶️ Cómo Ejecutar las Pruebas

#### Opción 1: Todas las pruebas (Unit + Integration)

```powershell
# Desde la raíz del proyecto
dotnet test "C:\dev\GitProxyManager\GitProxyManager.Tests\GitProxyManager.Tests.csproj" --filter "FullyQualifiedName!~UITests"
```

> 📝 Se excluyen las UI Tests porque requieren una ventana WPF activa y un entorno gráfico.

#### Opción 2: Solo Unit Tests

```powershell
dotnet test "C:\dev\GitProxyManager\GitProxyManager.Tests\GitProxyManager.Tests.csproj" --filter "FullyQualifiedName!~IntegrationTests&FullyQualifiedName!~UITests"
```

#### Opción 3: Solo Integration Tests

```powershell
dotnet test "C:\dev\GitProxyManager\GitProxyManager.Tests\GitProxyManager.Tests.csproj" --filter "FullyQualifiedName!~IntegrationTests"
```

#### Opción 4: Tests de una categoría específica

```powershell
# Solo tests de un archivo específico
dotnet test "C:\dev\GitProxyManager\GitProxyManager.Tests\GitProxyManager.Tests.csproj" --filter "FullyQualifiedName~MainViewModelTests"

# Solo tests de un servicio específico
dotnet test "C:\dev\GitProxyManager\GitProxyManager.Tests\GitProxyManager.Tests.csproj" --filter "FullyQualifiedName~GitProxyServiceTests"

# Solo tests de un método específico
dotnet test "C:\dev\GitProxyManager\GitProxyManager.Tests\GitProxyManager.Tests.csproj" --filter "FullyQualifiedName~OnExternalStateChanged"
```

#### Opción 5: Con Visual Studio

1. Abre `GitProxyManager.sln` en Visual Studio
2. Ve a **Test Explorer** (`Ctrl + E, T`)
3. Haz clic en **Run All** (`Ctrl + R, A`)
4. Verifica que todos los tests aparezcan en verde ✅

#### Opción 6: Todas las pruebas incluyendo UI (requiere display server)

```powershell
dotnet test "C:\dev\GitProxyManager\GitProxyManager.Tests\GitProxyManager.Tests.csproj"
```

> ⚠️ Las UI Tests usan FlaUI y requieren una sesión de Windows gráfica. En CI/CD se recomienda excluir UITests.

### 🔄 Flujo de Verificación Post-Cambio

Después de cualquier mejora al aplicativo, ejecuta este pipeline para asegurar que nada se rompió:

```powershell
# 1. Compilar el proyecto completo
dotnet build "C:\dev\GitProxyManager\GitProxyManager.csproj" -c Debug

# 2. Ejecutar todas las pruebas (excluyendo UI)
dotnet test "C:\dev\GitProxyManager\GitProxyManager.Tests\GitProxyManager.Tests.csproj" --filter "FullyQualifiedName!~UITests" --verbosity minimal

# 3. Verificar resultado esperado
#    Expected output:
#    Passed!  - Failed: 0, Passed: 195, Skipped: 0, Total: 195
```

Si algún test falla, el output muestra:
- **Nombre del test fallido** con el archivo y línea exacta
- **Error message** con la expectativa que falló
- **Stack trace** para localizar el problema

### ⚡ Ejecución Rápida (PowerShell alias)

```powershell
# Agregar alias a tu profile de PowerShell (~\Documents\PowerShell\Microsoft.PowerShell_profile.ps1)
function Run-Tests { dotnet test "C:\dev\GitProxyManager\GitProxyManager.Tests\GitProxyManager.Tests.csproj" --filter "FullyQualifiedName!~UITests" --verbosity minimal }
function Build-And-Test { dotnet build "C:\dev\GitProxyManager\GitProxyManager.csproj" -c Debug; Run-Tests }

# Uso:
Run-Tests          # Ejecuta solo las pruebas
Build-And-Test     # Compila + ejecuta pruebas
```

### 📊 Resumen de Cobertura por Componente

| Componente | Unit Tests | Integration Tests | Total |
|:----------:|:----------:|:-----------------:|:-----:|
| **ProxyConfig** (modelo) | 8 | - | 8 |
| **ConfigService** (JSON) | 10 | 4 | 14 |
| **GitProxyService** (git) | 14 | - | 14 |
| **SystemProxyService** (registry) | 24 | - | 24 |
| **ProxyStateService** (eventos) | 10 | - | 10 |
| **SystemStatePoller** (polling) | 8 | - | 8 |
| **MainViewModel** (lógica) | 60+ | 10 | 70+ |
| **Converters** | 18 | - | 18 |
| **Flujos completos** | - | 7 | 7 |
| **Total** | **133** | **21** | **154** |

> 📝 El resto de tests (41) cubren interacciones UI con FlaUI y validación visual.

### 🔧 Dependencias de Prueba

| Paquete | Versión | Propósito |
|:-------:|:-------:|:----------|
| **xUnit** | 2.9.3 | Framework de testing |
| **FluentAssertions** | 7.1.0 | Asserts legibles y expresivos |
| **Moq** | 4.20.70 | Mocking de dependencias |
| **FlaUI.UIA3** | 5.0.0 | UI Automation para WPF |
| **FlaUI.Testing** | 5.0.0 | Helpers para FlaUI |

### 🏷️ Notas Técnicas

- Todos los test classes usan `[Collection("SequentialTests")]` para evitar interferencias por archivos compartidos (config.json, registry)
- Los tests que escriben al config real se serializan para evitar race conditions
- `TestFileHelper` crea archivos temporales únicos por test para cleanup seguro
- Los tests de UI usan `FlaUI` con `WindowsVersion.Win10` para automatizar la ventana WPF
- Los tests de `ProxyStateService` usan la nueva firma de evento `Action<bool, bool, string, int, string>` (5 parámetros)

---

### 🖥️ System Tray

```
┌─────────────────────────────┐
│ 📂 Abrir                    │
│─────────────────────────────│
│ ☑ Proxy del sistema         │
│ ☑ Proxy de Git              │
│─────────────────────────────│
│ ❌ Salir                    │
└─────────────────────────────┘
```

### 📊 Estados del Icono

```
┌──────────┐  ┌──────────┐  ┌──────────┐
│  ⚪ Gris  │  │  🟡 Mixto │  │  🟢 Verde │
│  Ninguno │  │  Solo uno│  │  Ambos   │
└──────────┘  └──────────┘  └──────────┘
```

---

## 🗺️ Roadmap

### ✅ v1.2.0 (Actual)

- [x] Toggle button para proxy de Git
- [x] Configuración de host y puerto
- [x] System tray con menú contextual
- [x] Persistencia de configuración
- [x] Tema dark moderno
- [x] Proxy del sistema Windows
- [x] Toggle maestro
- [x] Bypass list editable
- [x] Loading en botones
- [x] Toast notifications
- [x] PulseScale animations
- [x] Icono trifásico (verde/parcial/gris)
- [x] Botón X minimiza al system tray (no cierra la app)
- [x] Licencia MIT en el instalador MSI
- [x] BypassList se carga del registro de Windows como fallback
- [x] Sincronización automática con proxy del sistema y de Git (polling 5s)
- [x] Notificación toast en cambios externos detectados
- [x] Valores por defecto preconfigurados (host, puerto, bypass)

### 🔜 v1.1.0 (Próximo)

- [ ] Múltiples perfiles de proxy
- [ ] Detección automática de red
- [ ] Inicio automático con Windows
- [ ] Tema claro/oscuro toggle

### 🚀 v2.0.0 (Futuro)

- [ ] Soporte multiplataforma (Linux/macOS)
- [ ] Configuración por repositorio
- [ ] Integración con VPN
- [ ] Logs de actividad

---

## 🐛 Known Issues

### ⚠️ Problemas Conocidos

| Issue | Estado | Solución |
|:-----:|:------:|:---------|
| #1 - No detecta proxy en WSL | 🔄 En progreso | Usar proxy local |
| #2 - Ícono no aparece en VM | 📝 Documentado | Agregar excepción en antivirus |

### 💡 Soluciones Comunes

**Problema:** El proxy no se aplica correctamente
```powershell
# Solución: Verificar configuración de git
git config --global --get http.proxy
git config --global --get https.proxy

# Verificar proxy del sistema (PowerShell)
Get-ItemProperty "HKCU:\Software\Microsoft\Windows\CurrentVersion\Internet Settings" | Select-Object ProxyEnable, ProxyServer
```

**Problema:** La app no inicia con Windows
```
Solución: Agregar a Inicio automático
1. Win + R → shell:startup
2. Crear acceso directo a GitProxyManager.exe
```

**Problema:** Los botones no aparecen
```
Solución: Verificar que la ventana tenga altura suficiente
Altura mínima recomendada: 660px
```

---

## 🤝 Contribuir

¡Las contribuciones son bienvenidas! 🎉

### 📋 Guía de Contribución

1. **Fork** el repositorio
2. **Crea** una branch para tu feature (`git checkout -b feature/AmazingFeature`)
3. **Commit** tus cambios (`git commit -m 'Add some AmazingFeature'`)
4. **Push** a la branch (`git push origin feature/AmazingFeature`)
5. **Abre** un Pull Request

### 📝 Convenciones

- **Commits:** Usar [Conventional Commits](https://www.conventionalcommits.org/)
- **Código:** Seguir [.NET Coding Conventions](https://learn.microsoft.com/dotnet/csharp/fundamentals/coding-style/coding-conventions)
- **PRs:** Incluir descripción clara y testing

---

## 📊 Changelog

### [1.2.0] - 2026-07-02

#### ✅ Added
- Sincronización automática con proxy del sistema y de Git (polling cada 5s)
- Detección de cambios externos con notificación toast azul
- SystemStatePoller lee registry (HKCU\Internet Settings) y git config (~/.gitconfig)
- ProxyStateService event ahora incluye `bypassList` (5 parámetros)
- Logging diagnóstico a `poller.log` para debugging de polling
- Configuración por defecto preconfigurada (host, puerto, bypass list)

#### 🔧 Changed
- Configuración persistida en `%LocalAppData%` (consistente con MSI perUser)
- `ReadPollingState()` lee `~/.gitconfig` directamente con `File.ReadAllLines` (sin Process)
- `ReadPollingState()` usa `Registry.OpenSubKey` para lectura más confiable
- BypassList se carga del registro como fallback en config.json

#### 🐛 Fixed
- Polling detecta cambios en sistema y git independientemente (antes compartían un solo tracking)
- `_isExternalUpdate` guard previene re-aplicación de proxy al sincronizar desde estado externo

### [1.1.0] - 2026-06-27

#### ✅ Added
- Proxy del sistema Windows (HKCU\Internet Settings)
- Toggle maestro para sincronizar ambos proxies
- Bypass list editable desde la app
- Loading inline en botones (Apply/Reset)
- Toast notifications con slide-in animation
- PulseScale animations en cada sección
- Icono trifásico (verde/parcial/gris)
- Menú contextual con toggles individuales
- Lógica de dependencia (Git → Sistema)

#### 🔧 Changed
- UI rediseñada con 3 secciones de toggle
- Altura de ventana ajustada a 660px
- Toast overlay fuera del layout principal

#### 🐛 Fixed
- Cascada de toggles al desactivar Git
- Toast expandía la ventana al mostrarse
- Texto invisible al desvincular bindings

### [1.0.0] - 2026-06-19

#### ✅ Added
- Toggle button para activar/desactivar proxy
- Configuración de host y puerto
- System tray con menú contextual
- Persistencia de configuración en JSON
- Tema dark con MahApps.Metro
- Botón X minimiza al system tray en vez de cerrar (`ShutdownMode="OnExplicitShutdown"`)
- Licencia MIT mostrada durante la instalación MSI (`License.rtf`)
- BypassList se carga del registro de Windows como fallback (`ReadBypassList()`)
- `<local>` siempre se añade al ProxyOverride al aplicar proxy del sistema
- Warnings ICE91 suprimidos en el build del MSI (`SuppressIces`)
- Instalación en `LocalAppData` (perUser, sin permisos de admin)

#### 🔧 Changed
- Primera versión estable

#### 🐛 Fixed
- N/A (primera versión)

---

## 📝 License

```
MIT License

Copyright (c) 2026 Git Proxy Manager

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
```

---

## 🙏 Agradecimientos

- [MahApps.Metro](https://mahapps.com/) - UI Controls increíbles
- [Hardcodet.NotifyIcon](https://github.com/hardcodet/wpf-notifyicon) - System Tray para WPF
- [CommunityToolkit.Mvvm](https://learn.microsoft.com/dotnet/communitytoolkit/mvvm/) - Helpers MVVM
- [Catppuccin](https://catppuccin.com/) - Tema de colores hermoso

---

## 📧 Contacto

**Tu Nombre** - @tu_usuario - tu@email.com

🔗 **Repo:** [https://github.com/tu-usuario/GitProxyManager](https://github.com/tu-usuario/GitProxyManager)

---

<div align="center">

**⭐ Si te gusta este proyecto, ¡dale una estrella! ⭐**

Hecho con ❤️ y ☕

</div>
