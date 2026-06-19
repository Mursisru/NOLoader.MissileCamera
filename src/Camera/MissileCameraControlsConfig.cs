using System;
using System.IO;
using NOLoader.ModConfig;
using UnityEngine;

namespace NOLoader.MissileCamera
{
    internal static class MissileCameraControlsConfig
    {
        private static DateTime _lastWriteUtc = DateTime.MinValue;

        internal static bool Enabled = true;
        internal static float ZoomStep = 0.5f;
        internal static float ZoomMin = -4f;
        internal static float ZoomMax = 4f;
        internal static float ZoomFovDegreesPerUnit = 5f;
        internal static float IndicatorSeconds = 0.5f;
        internal static int Revision;

        internal static void Refresh(bool force = false)
        {
            MfdLayoutConfig.EnsureInitialized();
            string modRoot = GetModRoot();
            if (string.IsNullOrEmpty(modRoot))
                return;

            string path = Path.Combine(modRoot, "mod_config.ini");
            if (!File.Exists(path))
                return;

            DateTime writeUtc = File.GetLastWriteTimeUtc(path);
            if (!force && writeUtc <= _lastWriteUtc)
                return;

            _lastWriteUtc = writeUtc;
            Revision++;
            Load(ModIniConfig.Load(modRoot));
        }

        private static string GetModRoot()
        {
            string? location = typeof(MissileCameraControlsConfig).Assembly.Location;
            if (!string.IsNullOrEmpty(location))
            {
                string? dir = Path.GetDirectoryName(location);
                if (!string.IsNullOrEmpty(dir))
                    return dir;
            }

            return string.Empty;
        }

        private static void Load(ModIniConfig cfg)
        {
            Enabled = cfg.GetBool("MissileCameraControls", "Enabled", true);
            ZoomStep = MathfClamp(cfg.GetFloat("MissileCameraControls", "ZoomStep", 0.5f), 0.05f, 4f);
            ZoomMin = MathfClamp(cfg.GetFloat("MissileCameraControls", "ZoomMin", -4f), -20f, 0f);
            ZoomMax = MathfClamp(cfg.GetFloat("MissileCameraControls", "ZoomMax", 4f), 0f, 20f);
            ZoomFovDegreesPerUnit = MathfClamp(cfg.GetFloat("MissileCameraControls", "ZoomFovDegreesPerUnit", 5f), 0.5f, 30f);
            IndicatorSeconds = MathfClamp(cfg.GetFloat("MissileCameraControls", "IndicatorSeconds", 0.5f), 0.1f, 3f);
        }

        internal static float ClampZoomOffset(float offset) =>
            MathfClamp(offset, ZoomMin, ZoomMax);

        internal static float ComputeEffectiveFov(float baseFov, float zoomOffset)
        {
            float fov = baseFov - zoomOffset * ZoomFovDegreesPerUnit;
            return fov < 10f ? 10f : fov > 120f ? 120f : fov;
        }

        private static float MathfClamp(float value, float min, float max) =>
            value < min ? min : value > max ? max : value;
    }
}
