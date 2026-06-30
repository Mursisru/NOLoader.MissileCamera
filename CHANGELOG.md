# Changelog

All notable changes to **NOLoader.MissileCamera** are documented here. Semver in `mod.json` / `MissileCameraMod.cs`.

Format follows [Keep a Changelog](https://keepachangelog.com/en/1.1.0/).

## [0.27.1] — 2026-06-29

### Documentation

- Full README refresh: loader choice, troubleshooting, developer NOLoader workflow.
- Added `CHANGELOG.md`, updated `release/v0.27.1/INSTALL.txt`.
- Expanded `.cursorrules` with NOLoader mod-author guide (sibling `NOLoader_Engine`, PatchTool, Gate L2).

### Notes

- **No gameplay or binary changes** from v0.27.0 — documentation-only release.

## [0.27.0] — 2026-06-19

### Added

- Manual missile cycling (Next/Prev) with **sticky** selection and **cyclic wrap** (6/6 → 1/6).
- Session zoom controls with HUD offset readout (0.5 s).
- `[MissileCameraControls]` in `mod_config.ini` (zoom step/limits; keybinds fixed in code).
- Fixed keybinds (US keyboard): Right Alt + `/` `,` `;` `.`; Right Shift + `.` reset zoom.

### Changed

- BepInEx upstream migrated to Configuration Manager; NOLoader keeps `mod_config.ini`.

## [0.26.1] — 2026-06-19

### Fixed

- Salvo counter across mixed weapon types and launch gaps.

## [0.26.0] — 2026-06-18

### Added

- Initial NOLoader port from BepInEx MissileCamera.
- Cecil IL patches (`mod.json`), mission-only `loadStage`, MFD split layout, seeker feed, tactical HUD.
