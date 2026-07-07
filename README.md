**Developer:** Mursisru

# NOLoader.MissileCamera (Nuclear Option Mod)

[![Nuclear Option](https://img.shields.io/badge/Game-Nuclear%20Option-blue)](https://store.steampowered.com/app/2168680/Nuclear_Option/)
[![NOLoader](https://img.shields.io/badge/Loader-NOLoader-purple)](https://github.com/Mursisru/NOLoader)
[![Version](https://img.shields.io/badge/Version-0.27.1-green)](https://github.com/Mursisru/NOLoader.MissileCamera/releases/tag/v0.27.1)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow)](https://github.com/Mursisru/NOLoader.MissileCamera/blob/NOLoaderVersion/LICENSE)

---

## Critical warnings
> [!CAUTION]
> **Never install NOLoader and BepInEx in the same game folder** - remove BepInEx `winhttp.dll` before deploying this mod.

> [!IMPORTANT]
> - **NOLoader required** - install [NOLoader](https://github.com/Mursisru/NOLoader/releases) before this mod.
> - **Close Nuclear Option before PatchTool** - PatchTool edits `Managed\Assembly-CSharp.dll`; a running game causes Win32 IO error 1224.
> - **Run PatchTool once** after install or game update so Cecil IL patches from `mod.json` apply.

> [!WARNING]
> - **Gate L2 hash mismatch after game updates** - re-bake all `expectedSignatureHash` values in `mod.json` when `Assembly-CSharp.dll` changes.
> - **Third-party aircraft / MFD mods may break layout** - set `[Layout] DisplayMode=skip` in `mod_config.ini` if needed.

> [!NOTE]
> **`loadStage: Mission`** - mod boots in mission scenes only (parity with BepInEx mission host).

NOLoader mod for the flight sim **Nuclear Option** that adds a live seeker-eye view (Missile Nose Cam) and a tactical HUD overlay directly onto your cockpit MFD Target display.

**Mod id:** `com.at747.missilecamera`

**Origin:** NOLoader port of the BepInEx [MissileCamera](https://github.com/Mursisru/MissileCamera/tree/BepInExVersion) plugin. Same gameplay; NOLoader uses `mod_config.ini`, BepInEx uses **Configuration Manager**. Use **one** loader — do not install both builds.

---

## Table of contents

- [Critical warnings](#critical-warnings)
* [Features](#features)
* [Choose your loader](#choose-your-loader)
* [Requirements](#requirements)
* [Player installation](#player-installation)
* [Controls & keybinds](#controls--keybinds)
* [Configuration (`mod_config.ini`)](#configuration-mod_configini)
* [Runtime lifecycle](#runtime-lifecycle)
* [Developer guide](#developer-guide)
* [Project layout](#project-layout)
* [Compatibility & limitations](#compatibility--limitations)
* [Troubleshooting](#troubleshooting)
* [Changelog](#changelog)
* [Licence](#licence)

## Features

* **MFD split-screen UI:** Splits the wide tactical MFD (Target view) into zones and embeds the missile feed in the weapons panel area.
* **Seeker cam (missile nose cam):** Renders a live `RawImage` feed from your latest **player-owned** in-flight missile while it guides toward the target.
* **Tactical HUD overlay:** Telemetry (`SPD`, `ALT`, `RNG`), horizon reticle, salvo info, and target markers drawn on the live feed.
* **Manual feed controls:** Cycle in-flight owned missiles and adjust camera zoom while the MFD overlay is active (see **Controls** below).
* **Per-aircraft layout (`DisplayMode=auto`):**
  * **Dedicated split** (e.g. KR-67): wide target cam on the left, missile panel on the right.
  * **Small tac overlay** (e.g. Cricket): mod **skipped** — vanilla tactical MFD unchanged.
* **Mission-only bootstrap:** Mod loads at **Mission** stage (`loadStage: Mission` in `mod.json`), matching the BepInEx mission-scene bootstrap.

---

## Choose your loader

| | **This repo (NOLoader)** | [MissileCamera BepInEx](https://github.com/Mursisru/MissileCamera/tree/BepInExVersion) |
|---|---|---|
| Loader | [NOLoader](https://github.com/Mursisru/NOLoader) (`winhttp.dll` proxy) | BepInEx 5 + Harmony |
| Config | `mod_config.ini` in mod folder | BepInEx Configuration Manager (`.cfg`) |
| Patches | Cecil IL via `mod.json` + PatchTool | Harmony runtime |
| Install path | `NOLoader\mods\MissileCamera\` | `BepInEx\plugins\MissileCamera\` |
| Game update | Re-run PatchTool; verify `expectedSignatureHash` | Clear Harmony cache if needed |

**Do not install both loaders** in the same game directory (`winhttp.dll` conflict).

---

## Requirements

* **Nuclear Option** ([Steam](https://store.steampowered.com/app/2168680/Nuclear_Option/)).
* Matching game `Assembly-CSharp.dll` (for **patch hashes** in `mod.json`).
* **[NOLoader](https://github.com/Mursisru/NOLoader)** — [install](https://github.com/Mursisru/NOLoader/blob/master/docs/INSTALL.md) · player zip from [Release v0.1.0](https://github.com/Mursisru/NOLoader/releases/tag/v0.1.0) (RDYTU or RDYTU.mini).
* **Developers:** sibling clone [NOLoader](https://github.com/Mursisru/NOLoader) as `source\repos\NOLoader_Engine\` next to this project ([DEV.SDK](https://github.com/Mursisru/NOLoader/blob/master/docs/DEV_SDK.md)).

---

## Player installation

1. Install [NOLoader](https://github.com/Mursisru/NOLoader) into the game (`NOLoader\core\`, `winhttp.dll` proxy). See [INSTALL.md](https://github.com/Mursisru/NOLoader/blob/master/docs/INSTALL.md) · [RDYTU](https://github.com/Mursisru/NOLoader/blob/master/docs/RDYTU.md) · [RDYTU.mini](https://github.com/Mursisru/NOLoader/blob/master/docs/RDYTU.mini.md).
2. Copy into:

   ```text
   Nuclear Option\NOLoader\mods\MissileCamera\
   ```

   * `NOLoader.MissileCamera.dll`
   * `NOLoader.ModConfig.dll`
   * `mod.json`
   * `mod_config.ini`

3. Run PatchTool once (or use the deploy script) so Cecil patches apply to `Assembly-CSharp.dll`.

4. Download release zip from [GitHub Releases](https://github.com/Mursisru/NOLoader.MissileCamera/releases) or use `release/v0.27.1/INSTALL.txt` as a checklist.

---

## Controls & keybinds

Active only while the missile feed overlay is on and you have **player-owned** in-flight missiles. **US English keyboard layout** (Right Alt may act as AltGr on some EU keyboards). Keybinds are **fixed in code** (not in `mod_config.ini`).

| Keybind | Unity `KeyCode` | Action |
| :--- | :--- | :--- |
| **Right Alt** + `/` | `RightAlt` + `Slash` | Next missile (newer; wraps 6/6 → 1/6) |
| **Right Alt** + `,` | `RightAlt` + `Comma` | Previous missile (older; wraps 1/6 → 6/6) |
| **Right Alt** + `;` | `RightAlt` + `Semicolon` | Zoom in (narrower FOV) |
| **Right Alt** + `.` | `RightAlt` + `Period` | Zoom out (wider FOV) |
| **Right Shift** + `.` | `RightShift` + `Period` | Reset zoom offset to `0.0` |

**Sticky selection:** after Next/Prev the camera stays on your chosen missile when new ones launch. If it is destroyed, the feed falls back to the newest remaining missile.

**Zoom HUD:** each zoom change shows the current **offset** (e.g. `2.0`, `-0.5`) for **0.5 s** above feed center (`0.0` = default FOV from `[MissileCameraFeed]` `Fov`). Zoom limits/step: `[MissileCameraControls]` in `mod_config.ini`.

---

## Configuration (`mod_config.ini`)

Edit `NOLoader\mods\MissileCamera\mod_config.ini`. Changes are polled every ~0.5 s during a mission (hot-reload without restart).

### `[Layout]`

| Key | Default | Description |
| :--- | :---: | :--- |
| `Enabled` | `1` | Master switch for MFD layout changes |
| `DisplayMode` | `split` | `auto` (per-aircraft) \| `skip` (bypass) \| `split` (forced split) |
| `OverlayMaxWidth` | `0.45` | Max normalized width for tac overlay detection |
| `LeftWidth` | `0.58` | Target cam column width (0–1) |
| `MissilePanelBottom` | `0.38` | Bottom edge of missile panel (engine strip below) |
| `WeaponsStripHeight` | `0.12` | Compressed weapons wireframe strip height |
| `ShowDivider` | `1` | Zone divider lines |
| `DebugStub` | `0` | Bright magenta test panel |
| `StubLabel` | `MISSILE CAMERA` | Label on debug stub |

### `[MissileCameraFeed]`

| Key | Default | Description |
| :--- | :---: | :--- |
| `Enabled` | `1` | Live missile camera feed |
| `NoseSkinInset` | `0.08` | Keep camera outside nose mesh (meters) |
| `CameraBackOffset` | `0.35` | Pull camera back from nose point (meters) |
| `Fov` | `60` | Field of view (degrees) |
| `FeedWidth` | `512` | RenderTexture width |
| `FeedHeight` | `512` | RenderTexture height |
| `HorizonLock` | `1` | World-up roll lock; body-follow pitch/yaw |
| `TurnLookBankScale` | `1` | G-load turn look scale |
| `MaxTurnLookDegrees` | `90` | Max turn-look offset (degrees) |
| `DefaultMissileGLimit` | `20` | Fallback G limit |
| `TurnLookGDeadband` | `0.15` | G deadband |
| `TurnLookGFilterHz` | `7` | G filter cutoff (Hz) |
| `TurnLookSlewDegPerSec` | `120` | Turn-look slew rate (deg/s) |
| `TurnLookSmoothTime` | `0.18` | Turn-look smoothing |
| `PostExplosionHoldSeconds` | `0` | Hold last frame after missile loss (0 = off) |
| `RenderFps` | `30` | Feed refresh rate |

### `[MissileCameraHud]`

| Key | Default | Description |
| :--- | :---: | :--- |
| `Enabled` | `1` | HUD overlay on feed |
| `SalvoWindowSeconds` | `0.5` | Salvo grouping window (seconds) |
| `ShowCenterCluster` | `1` | Center reticle / intercept ring |
| `ShowTargetMarker` | `1` | Target diamond marker |
| `InterceptColor` | `0,1,0,1` | Intercept ring RGBA (0–1) |
| `ReticleColor` | `0,0.4,1,1` | Reticle RGBA |
| `HorizonColor` | `0.05,0.35,0.08,1` | Horizon fill |
| `HorizonOutlineColor` | `0.2,1,0.25,1` | Horizon outline |
| `MissileNameColor` | `1,0,1,1` | Missile name label |
| `TargetNameColor` | `0.4,0.9,1,1` | Target name label |
| `LabelBackgroundColor` | `0.18,0.18,0.18,0.62` | Label backdrop |
| `LabelBackgroundAlpha` | `0.62` | Backdrop alpha |

### `[MissileCameraControls]`

| Key | Default | Description |
| :--- | :---: | :--- |
| `Enabled` | `1` | Keyboard missile cycling and zoom (keybinds are fixed; see **Controls & keybinds**) |
| `ZoomStep` | `0.5` | Offset change per zoom key press |
| `ZoomMin` | `-4` | Minimum zoom offset |
| `ZoomMax` | `4` | Maximum zoom offset |
| `ZoomFovDegreesPerUnit` | `5` | FOV delta (degrees) per offset unit |
| `IndicatorSeconds` | `0.5` | Zoom HUD readout duration (seconds) |

---

## Runtime lifecycle

1. **Bootstrap:** `INOMod.OnLoad` runs at **Mission** stage; Cecil IL hooks from `mod.json` are applied by PatchTool before play.
2. **Activation:** With Target MFD active, launch a trackable owned missile — layout applies and the feed binds.
3. **Rendering:** The auxiliary camera draws only while the overlay is active and a missile is in flight.
4. **Isolation:** Vanilla `TargetCam` geometry is not modified; layout uses UI zones and a separate render rig.

---

## Developer guide

Close the game before deploy (PatchTool needs managed DLLs unlocked).

**AI / mod authors:** see `.cursorrules` in this repo and `NOLoader_Engine/.cursorrules` for full NOLoader architecture (DEV_SDK, PatchTool, Gate L2, `INOMod`).

### Quick deploy

```powershell
**Developer:** Mursisru

# DEV_SDK build + PatchTool (default)
.\scripts\deploy.ps1

**Developer:** Mursisru

# RDYTU loader in game — build DEV_SDK, patch with RDYTU:
.\scripts\deploy.ps1 -PatchToolConfiguration RDYTU
```

### Build

Requires `NOLoader_Engine` at `source\repos\NOLoader_Engine\` (sibling of this repo).

Set the game path in `Directory.Build.props` (`NuclearOptionRoot`) if needed. Copy `Directory.Build.props.example` to `Directory.Build.props` and adjust the path.

```powershell
dotnet build NOLoader.MissileCamera.csproj -c DEV_SDK
```

Output: `bin\DEV_SDK\net48\NOLoader.MissileCamera.dll`

Open `NOLoader.MissileCamera.sln` in Visual Studio or JetBrains Rider for IDE builds.

### IL patches (`mod.json`)

Seven Cecil postfix injections (hashes must match game build):

* `TargetScreenUI::SetupCamera`
* `TargetCam::SetLandingCam` / `CancelTarget` / `OnDestroy`
* `TacScreen::Initialize` / `TacScreen_OnCamToggle`
* `WeaponManager::TargetListChanged`

Re-run PatchTool after game updates or mod DLL changes. Gate L2 rejects patches when `expectedSignatureHash` mismatches.

### Porting from BepInEx

When updating from the BepInEx `MissileCamera` repo (`source\repos\MissileCamera\`):

1. Copy changed logic under `src/Camera/`, `src/Hud/`, `src/Layout/`, `src/Access/`, `src/Ui/`.
2. Map Harmony postfix bodies to `src/Patches/Patches.cs` (same call order).
3. Keep `mod_config.ini` keys identical to BepInEx Configuration Manager defaults.
4. Rebuild, redeploy, and verify patch hashes if `Assembly-CSharp.dll` changed.

---

## Project layout

```text
NOLoader.MissileCamera/
├── NOLoader.MissileCamera.csproj
├── NOLoader.MissileCamera.sln
├── mod.json
├── mod_config.ini
├── CHANGELOG.md
├── src/
│   ├── Mod/
│   │   └── MissileCameraMod.cs   # INOMod entry
│   ├── Patches/
│   │   └── Patches.cs            # Cecil inject targets
│   ├── Camera/                   # Feed rig (port from MissileCamera)
│   ├── Hud/                      # Overlay widgets
│   ├── Layout/                   # MFD zone split
│   ├── Access/                   # Game API wrappers
│   ├── Ui/                       # HUD graphics helpers
│   └── Logging/
├── release/
│   └── v0.27.1/
│       └── INSTALL.txt
└── scripts/
    └── deploy.ps1
```

**GitHub:** [Mursisru/NOLoader.MissileCamera](https://github.com/Mursisru/NOLoader.MissileCamera/tree/NOLoaderVersion) · **Upstream (BepInEx):** [Mursisru/MissileCamera](https://github.com/Mursisru/MissileCamera/tree/BepInExVersion)

---

## Compatibility & limitations

Developed and tested against **vanilla Nuclear Option** aircraft and the stock Target MFD. The mod may work **incorrectly or not at all** when:

* **Third-party / custom aircraft** — non-vanilla cockpit MFD hierarchy, custom `TargetScreenUI` layouts, or unusual weapon integration can break layout detection (`DisplayMode=auto`), feed binding, nose-cam placement, or salvo tracking.
* **Other mods that change the MFD** — tactical UI overlays, layout replacers, or patches to `TargetScreenUI`, `TacScreen`, `TargetCam`, or target/weapon lists may **conflict** with this mod's Cecil IL hooks and UI zone split.

**Mitigation:** set `[Layout] DisplayMode=skip` in `mod_config.ini` to keep vanilla MFD layout (feed may still bind if hooks remain compatible), or disable conflicting MFD mods. For modded setups, include aircraft/mod names and repro steps in issue reports.

---

## Troubleshooting

| Symptom | Likely cause | Fix |
| :--- | :--- | :--- |
| Mod not loading | NOLoader not installed / wrong folder | Verify `NOLoader\mods\MissileCamera\` contents and `mod.json` |
| Hooks never fire | PatchTool not run or game was open | Close game; run PatchTool or `deploy.ps1` |
| Gate L2 / hash fail after game update | `expectedSignatureHash` stale | Re-bake hashes; update `mod.json`; redeploy |
| No feed on MFD | No owned in-flight missile / overlay off | Launch missile with Target MFD active |
| Layout wrong on modded aircraft | Custom MFD hierarchy | `DisplayMode=skip` or report with aircraft name |
| Keybinds ignored | Wrong keyboard layout / overlay inactive | US layout; feed must be active |
| BepInEx + NOLoader conflict | Both loaders installed | Remove one loader installation |

**Logs:** `NOLoader\logs\noloader_ring.log` (DEV loader: F10 overlay).

---

## Changelog

See [CHANGELOG.md](CHANGELOG.md).

---

## Licence

MIT License — see [LICENSE](LICENSE).
