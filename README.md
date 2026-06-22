# 🚀 Git Proxy Manager

> 🎯 **Gestiona tu proxy de Git de forma sencilla y elegante**

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
- [🏗️ Arquitectura](#️-arquitectura)
- [🧩 Patrón MVVM](#-patrón-mvvm)
- [📦 Estructura del Proyecto](#-estructura-del-proyecto)
- [🔧 Clases Principales](#-clases-principales)
- [🚀 Instalación](#-instalación)
- [📦 Instalador MSI](#-instalador-msi)
- [💻 Uso](#-uso)
- [⚙️ Configuración](#️-configuración)
- [🎨 Diseño UI/UX](#-diseño-uiux)
- [🛠️ Tecnologías](#️-tecnologías)
- [📸 Capturas](#-capturas)
- [ Roadmap](#-roadmap)
- [🐛 Known Issues](#-known-issues)
- [📝 License](#-license)

---

## ✨ Descripción

**Git Proxy Manager** es una aplicación de escritorio para Windows que te permite gestionar la configuración de proxy de Git de forma visual e intuitiva. 

¿Cansado de escribir comandos cada vez que cambias de red? ¿Olvidas si el proxy está activado o desactivado? 

**¡Esta app es para ti!** 🎉

### 🔥 ¿Por qué usar Git Proxy Manager?

| Sin Git Proxy Manager | Con Git Proxy Manager |
|:---------------------:|:---------------------:|
| 😫 Escribir comandos manualmente | 🎯 Un solo clic |
| 🤔 Recordar IPs y puertos | 💾 Configuración guardada |
| 😰 Olvidar activar/desactivar | 🔄 Toggle instantáneo |
| 🚫 Sin indicador visual | 📊 System tray informativo |

---

## 🎯 Características

### 🌟 Funcionalidades Principales

- **🔘 Toggle Button** - Activa/desactiva el proxy con un solo clic
- **📝 Configuración de Proxy** - Host y puerto personalizables
- **🖥️ System Tray** - Icono en la bandeja del sistema con estado actual
- **💾 Persistencia** - La configuración se guarda automáticamente
- **🎨 UI Moderna** - Tema dark con MahApps.Metro
- **⚡ Rendimiento** - Ligero y rápido

### 📊 System Tray

El icono en la bandeja del sistema muestra:

| Estado | Color | Significado |
|:------:|:-----:|:------------|
| 🟢 | Verde | Proxy activo |
| ⚪ | Gris | Proxy inactivo |

**Menú contextual:**
- 📂 Abrir ventana principal
- 🔀 Habilitar/Desactivar proxy
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
│  │  │GitProxy     │ │ Config      │   │                │
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
                         ▼
                    ┌──────────┐
                    │   File   │
                    │(JSON)    │
                    └──────────┘
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
│   └── 📄 ConfigService.cs        # Servicio de configuración
│
├── 📁 ViewModels/
│   └── 📄 MainViewModel.cs        # ViewModel principal
│
├── 📁 Resources/
│   └── 📁 Icons/                   # Iconos de la app
│
├── 📁 Converters.cs               # Convertidores de datos
├── 📄 App.xaml                     # Recursos de la aplicación
├── 📄 App.xaml.cs                  # Lógica de inicio
├── 📄 MainWindow.xaml              # Ventana principal
├── 📄 MainWindow.xaml.cs           # Code-behind de ventana
└── 📄 GitProxyManager.csproj       # Archivo de proyecto
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
}
```

**Descripción:** Modelo que representa la configuración del proxy.

| Propiedad | Tipo | Descripción |
|:---------:|:----:|:------------|
| `IsEnabled` | `bool` | Estado del proxy (activado/desactivado) |
| `Host` | `string` | Dirección del servidor proxy |
| `Port` | `int` | Puerto del proxy (default: 3128) |

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

**Descripción:** Servicio estático que interactúa con git para gestionar el proxy.

| Método | Descripción |
|:------:|:------------|
| `ApplyProxy()` | Aplica la configuración de proxy a git |
| `RemoveProxy()` | Elimina la configuración de proxy de git |
| `ReadCurrentConfig()` | Lee la configuración actual de git |

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

**Descripción:** Servicio para persistir la configuración en disco.

| Método | Descripción |
|:------:|:------------|
| `Load()` | Carga la configuración desde el archivo JSON |
| `Save()` | Guarda la configuración en el archivo JSON |

**Ubicación del archivo:** `%AppData%\GitProxyManager\config.json`

---

### 🧠 MainViewModel.cs

```csharp
namespace GitProxyManager.ViewModels;

public partial class MainViewModel : ObservableObject
{
    [ObservableProperty]
    private bool _isEnabled;

    [ObservableProperty]
    private string _host = string.Empty;

    [ObservableProperty]
    private int _port = 3128;

    [ObservableProperty]
    private string _statusMessage = string.Empty;

    [RelayCommand]
    private void Apply();

    [RelayCommand]
    private void Reset();

    [RelayCommand]
    private void ShowWindow();
}
```

**Descripción:** ViewModel principal que maneja la lógica de presentación.

| Propiedad/Método | Descripción |
|:----------------:|:------------|
| `IsEnabled` | Estado del toggle de proxy |
| `Host` | Texto del host del proxy |
| `Port` | Puerto del proxy |
| `StatusMessage` | Mensaje de estado para el usuario |
| `Apply` | Comando para aplicar configuración |
| `Reset` | Comando para restablecer configuración |
| `ShowWindow` | Comando para mostrar la ventana |

---

## 🚀 Instalación

### 📋 Requisitos Previos

- **Windows 10/11** (64-bit)
- **.NET 10.0 Runtime** o superior
- **Git** instalado y configurado

### 📥 Opción 1: Descarga Directa

1. Ve a la sección [Releases](https://github.com/tu-usuario/GitProxyManager/releases)
2. Descarga `GitProxyManager-v1.0.0-setup.exe`
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
| **📏 Tamaño** | `0.98 MB` |
| **🏗️ Plataforma** | `x64` |
| **🔧 Herramienta** | `WiX Toolset v5` |

### 📂 Contenido del MSI

| Componente | Descripción |
|:----------:|:------------|
| 📦 **App** | Ejecutable + DLLs en `Program Files\GitProxyManager` |
| 🔗 **Start Menu** | Acceso directo + Desinstalar en Menú Inicio |
| 📋 **Registry** | Entradas de desinstalación en Windows |

### 🛠️ Regenerar el MSI

```powershell
# 🔨 Compilar app en Release
dotnet build "C:\dev\GitProxyManager\GitProxyManager.csproj" -c Release

# 📦 Compilar MSI
dotnet build "C:\dev\GitProxyManager\installer\GitProxyManager.Installer.wixproj" -c Release
```

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

### 🎨 Icono de la Aplicación

La aplicación incluye un icono personalizado que representa:
- 🔵 **Círculo azul** - Representa la conexión de red
- 🟢 **Líneas verdes** - Simbolizan el flujo de datos
- 🟡 **Flecha amarilla** - Indica dirección del proxy

```
┌─────────────────┐
│    ╭─────────╮  │
│   │  ═══════  │  │
│   │  ═══════  │  │
│   │    ──▶    │  │
│    ╰─────────╯  │
└─────────────────┘
```

---

## 💻 Uso

### 🎬 Primeros Pasos

1. **Inicia la aplicación**
   - El ícono aparecerá en la bandeja del sistema
   - El ícono será gris (proxy desactivado)

2. **Configura tu proxy**
   - Haz clic derecho en el ícono de la bandeja
   - Selecciona "Abrir"
   - Ingresa el host y puerto del proxy
   - Haz clic en "Aplicar"

3. **Activa/Desactiva el proxy**
   - Usa el toggle button en la ventana principal
   - O usa el menú contextual del system tray

### 📊 Indicadores del System Tray

| Estado | Acción |
|:------:|:-------|
| 🟢 Verde | Proxy activo - Las operaciones de git pasan por el proxy |
| ⚪ Gris | Proxy inactivo - Git usa conexión directa |

### ⌨️ Atajos de Teclado

| Tecla | Acción |
|:-----:|:-------|
| `Doble clic` | Abrir ventana principal |
| `Clic derecho` | Menú contextual |

---

## ⚙️ Configuración

### 📁 Archivo de Configuración

**Ubicación:** `%AppData%\GitProxyManager\config.json`

```json
{
  "IsEnabled": true,
  "Host": "172.16.65.62",
  "Port": 3128
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

### 🎯 Configuración por Defecto

| Parámetro | Valor por Defecto | Descripción |
|:---------:|:-----------------:|:------------|
| `IsEnabled` | `false` | Proxy desactivado |
| `Host` | `""` | Sin host configurado |
| `Port` | `3128` | Puerto estándar de Squid |

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
│  ┌─────────────────────────────────────────────────┐   │
│  │  🔘 Proxy habilitado          [=====○] ON       │   │
│  │                                    🟢            │   │
│  └─────────────────────────────────────────────────┘   │
│                                                         │
│  ┌─────────────────────────────────────────────────┐   │
│  │  Proxy: [172.16.65.62    ]  Puerto: [3128   ]  │   │
│  └─────────────────────────────────────────────────┘   │
│                                                         │
│  ┌──────────────┐        ┌──────────────┐              │
│  │   Aplicar    │        │ Restablecer  │              │
│  └──────────────┘        └──────────────┘              │
│                                                         │
│  ┌─────────────────────────────────────────────────┐   │
│  │  🟢 Proxy activo: 172.16.65.62:3128             │   │
│  └─────────────────────────────────────────────────┘   │
└─────────────────────────────────────────────────────────┘
```

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
| **Git** | Control de versiones |
| **PowerShell** | Scripts de automatización |

---

## 📸 Capturas

### 🖼️ Ventana Principal

```
┌─────────────────────────────────────────────────────────┐
│  Git Proxy Manager                                       │
│                                                          │
│  ╔═══════════════════════════════════════════════════╗   │
│  ║  🔘 Proxy habilitado              [=====○] ON     ║   │
│  ╚═══════════════════════════════════════════════════╝   │
│                                                          │
│  ╔═══════════════════════════════════════════════════╗   │
│  ║  Proxy: [172.16.65.62    ]  Puerto: [3128   ]    ║   │
│  ╚═══════════════════════════════════════════════════╝   │
│                                                          │
│  ╔══════════════╗        ╔══════════════╗                │
│  ║   Aplicar    ║        ║ Restablecer  ║                │
│  ╚══════════════╝        ╚══════════════╝                │
│                                                          │
│  🟢 Proxy activo: 172.16.65.62:3128                     │
└─────────────────────────────────────────────────────────┘
```

### 🖥️ System Tray

```
┌─────────────────────┐
│ 📂 Abrir            │
│─────────────────────│
│ ☑ Habilitar Proxy   │
│─────────────────────│
│ ❌ Salir            │
└─────────────────────┘
```

---

## 🗺️ Roadmap

### ✅ v1.0.0 (Actual)

- [x] Toggle button para proxy
- [x] Configuración de host y puerto
- [x] System tray con menú contextual
- [x] Persistencia de configuración
- [x] Tema dark moderno

### 🔜 v1.1.0 (Próximo)

- [ ] Múltiples perfiles de proxy
- [ ] Detección automática de red
- [ ] Notificaciones de cambio de estado
- [ ] Inicio automático con Windows

### 🚀 v2.0.0 (Futuro)

- [ ] Soporte multiplataforma (Linux/macOS)
- [ ] Configuración por repositorio
- [ ] Integración con VPN
- [ ] Logs de actividad
- [ ] Temas personalizables

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
```

**Problema:** La app no inicia con Windows
```
Solución: Agregar a Inicio automático
1. Win + R → shell:startup
2. Crear acceso directo a GitProxyManager.exe
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

### [1.0.0] - 2026-06-19

#### ✅ Added
- Toggle button para activar/desactivar proxy
- Configuración de host y puerto
- System tray con menú contextual
- Persistencia de configuración en JSON
- Tema dark con MahApps.Metro

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
