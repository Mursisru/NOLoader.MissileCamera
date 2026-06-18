# NOLoader.MissileCamera (Nuclear Option Mod)

[![Nuclear Option](https://img.shields.io/badge/Game-Nuclear%20Option-blue)](https://store.steampowered.com/app/2168680/Nuclear_Option/)
[![NOLoader](https://img.shields.io/badge/Loader-NOLoader-purple)](https://github.com/Mursisru/NOLoader)
[![Version](https://img.shields.io/badge/Version-0.26.0-green)]()

NOLoader mod for the flight sim **Nuclear Option** that adds a live seeker-eye view (Missile Nose Cam) and a tactical HUD overlay directly onto your cockpit MFD Target display.

**Mod id:** `com.at747.missilecamera`

**Origin:** NOLoader port of the BepInEx [MissileCamera](https://github.com/Mursisru/MissileCamera/tree/BepInExVersion) plugin. Same gameplay and `mod_config.ini` format; Cecil IL patches instead of Harmony. Use **one** loader — do not install both builds.

---

## Features

* **MFD split-screen UI:** Splits the wide tactical MFD (Target view) into zones and embeds the missile feed in the weapons panel area.
* **Seeker cam (missile nose cam):** Renders a live `RawImage` feed from your latest **player-owned** in-flight missile while it guides toward the target.
* **Tactical HUD overlay:** Telemetry (`SPD`, `ALT`, `RNG`), horizon reticle, salvo info, and target markers drawn on the live feed.
* **Per-aircraft layout (`DisplayMode=auto`):**
  * **Dedicated split** (e.g. KR-67): wide target cam on the left, missile panel on the right.
  * **Small tac overlay** (e.g. Cricket): mod **skipped** — vanilla tactical MFD unchanged.
* **Mission-only bootstrap:** Mod loads at **Mission** stage (`loadStage: Mission` in `mod.json`), matching the BepInEx mission-scene bootstrap.

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

> **Troubleshooting:** Close the game before PatchTool. Re-run PatchTool after game updates or mod DLL changes if hooks stop firing.

---

## Developer guide

Close the game before deploy (PatchTool needs managed DLLs unlocked).

### Quick deploy

```powershell
# DEV_SDK build + PatchTool (default)
.\scripts\deploy.ps1

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

Re-run PatchTool after game updates or mod DLL changes.

### Porting from BepInEx

When updating from the BepInEx `MissileCamera` repo (`source\repos\MissileCamera\`):

1. Copy changed logic under `src/Camera/`, `src/Hud/`, `src/Layout/`, `src/Access/`, `src/Ui/`.
2. Map Harmony postfix bodies to `src/Patches/Patches.cs` (same call order).
3. Keep `mod_config.ini` keys identical.
4. Rebuild, redeploy, and verify patch hashes if `Assembly-CSharp.dll` changed.

---

## Configuration (`mod_config.ini`)

Edit `NOLoader\mods\MissileCamera\mod_config.ini` (same format as the BepInEx `MissileCamera` plugin).

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

---

## Runtime lifecycle

1. **Bootstrap:** `INOMod.OnLoad` runs at **Mission** stage; Cecil IL hooks from `mod.json` are applied by PatchTool before play.
2. **Activation:** With Target MFD active, launch a trackable owned missile — layout applies and the feed binds.
3. **Rendering:** The auxiliary camera draws only while the overlay is active and a missile is in flight.
4. **Isolation:** Vanilla `TargetCam` geometry is not modified; layout uses UI zones and a separate render rig.

---

## Project layout

```text
NOLoader.MissileCamera/
├── NOLoader.MissileCamera.csproj
├── NOLoader.MissileCamera.sln
├── mod.json
├── mod_config.ini
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
└── scripts/
    └── deploy.ps1
```

**GitHub:** [Mursisru/NOLoader.MissileCamera](https://github.com/Mursisru/NOLoader.MissileCamera/tree/NOLoaderVersion) · **Upstream (BepInEx):** [Mursisru/MissileCamera](https://github.com/Mursisru/MissileCamera/tree/BepInExVersion)

---

## Licence

MIT License — see [LICENSE](LICENSE).
