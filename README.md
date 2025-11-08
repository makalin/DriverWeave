# 🧵 DriverWeave  
[![Build](https://img.shields.io/badge/build-passing-brightgreen.svg)](#)
[![Platform](https://img.shields.io/badge/platform-Windows%2010%2F11-blue.svg)](#)
[![License](https://img.shields.io/badge/license-MIT-lightgrey.svg)](LICENSE)
[![.NET](https://img.shields.io/badge/.NET-8.0-purple.svg)](#)
[![Maintainer](https://img.shields.io/badge/maintainer-Mehmet%20T.%20Akalın-orange.svg)](https://github.com/makalin)
[![Release](https://img.shields.io/github/v/release/makalin/DriverWeave.svg)](https://github.com/makalin/DriverWeave/releases)

> **DriverWeave** — a fast, modern, and open-source Windows tool to **list, compare, and update drivers** with one click.  
> Built by [Mehmet T. Akalın](https://github.com/makalin) using .NET 8, WPF, and native Windows APIs.

---

## ✨ Overview
DriverWeave scans your system’s installed drivers, compares them against Microsoft Update and OEM sources, and lets you safely update or roll back — all from a clean UI or CLI.  
It’s lightweight, transparent, and built for developers and technicians who prefer **control and clarity** over opaque OEM utilities.

---

## 🚀 Features
- 🧩 **Driver inventory** with provider, class, INF, version, date, and hardware IDs  
- 🌐 **Update discovery** via:
  - Windows Update (WUApiLib)
  - Optional OEM JSON catalogs  
- ⚙️ **One-click updates** with restore points and signature validation  
- 🕵️ **Rollback and version history** for safety  
- 📊 **Export inventory** to CSV/JSON for audits  
- 💻 **CLI mode** for automation and remote administration  
- 📦 **Offline caching** for downloaded CABs and INFs  
- 🌓 Modern dark-themed WPF interface  

---

## 🧩 Architecture
```

DriverWeave/
├─ src/
│   ├─ DriverWeave.Core/        # Core models, version diff, hashing
│   ├─ DriverWeave.Discovery/   # WMI + SetupAPI driver enumerator
│   ├─ DriverWeave.Update/      # Windows Update & OEM catalog client
│   ├─ DriverWeave.Install/     # pnputil wrapper & rollback logic
│   ├─ DriverWeave.App/         # WPF UI
│   └─ DriverWeave.Cli/         # Console frontend
├─ catalogs/oem-sources.json    # Optional vendor endpoints
└─ tools/scripts/               # PowerShell helpers

````

---

## 🖥️ UI Preview
DriverWeave provides a minimal, responsive WPF dashboard:

- **Scan** → list all installed drivers  
- **Check Updates** → query Windows Update + OEM feeds  
- **Update Selected** → install via `pnputil`  
- **Filter & Export** → quick search and JSON/CSV export  

> 🖤 Default dark mode | Fluent Design | Instant filtering

---

## ⚙️ CLI Usage
```bash
# List installed drivers
DriverWeave.Cli.exe --list

# Check for available updates
DriverWeave.Cli.exe --check

# Update all outdated drivers
DriverWeave.Cli.exe --update-all

# Export results to CSV
DriverWeave.Cli.exe --export drivers.csv
````

---

## 🧱 Build Instructions

```bash
git clone https://github.com/makalin/DriverWeave.git
cd DriverWeave

# Build
dotnet build

# Publish portable single-file binaries
dotnet publish src/DriverWeave.App -c Release -r win-x64 -p:PublishSingleFile=true
dotnet publish src/DriverWeave.Cli -c Release -r win-x64 -p:PublishSingleFile=true
```

---

## 🛡️ Safety & Rollback

* 🔒 Automatically creates a **Windows Restore Point** before updates
* ✅ Installs **digitally signed** packages only
* 🔁 Rollback any driver using the previous INF snapshot
* 🧾 Logs stored under `%ProgramData%\DriverWeave\logs`

---

## 🧠 Roadmap

* [ ] Weekly scheduled background checks
* [ ] Fluent UI 3.0 redesign
* [ ] Remote driver inventory (via PowerShell Remoting)
* [ ] Health scoring based on version age
* [ ] Auto-update notifications in system tray

---

## 🧰 Requirements

* Windows 10 or 11 (64-bit)
* .NET 8 Runtime
* Administrator rights for update/rollback operations

---

## 🪪 License

**MIT License** — free for personal and commercial use.
Copyright © 2025 [Mehmet T. Akalın](https://github.com/makalin)

---

## 🤝 Credits

* [WUApiLib](https://learn.microsoft.com/en-us/windows/win32/wua_sdk/wuapilib) – Windows Update API
* [SetupAPI / pnputil](https://learn.microsoft.com/en-us/windows-hardware/drivers/install/pnputil-command-syntax)
* [LiteDB](https://www.litedb.org/) – lightweight embedded cache
* [Microsoft .NET 8 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/8.0)

---

## 🌟 Author

**Mehmet T. Akalın**
Full-stack developer | Systems & AI engineer
🔗 [github.com/makalin](https://github.com/makalin) | 🌐 [desnd.com](https://desnd.com)

> *DriverWeave — Weave your drivers, don’t wrestle them.*
