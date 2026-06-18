using System;
using System.Reflection;

namespace NOLoader.MissileCamera
{
    internal static class MfdPatchProbe
    {
        internal static void Run(string gameRoot)
        {
            try
            {
                Type? targetCamType = FindGameType("TargetCam");
                Type? tacScreenType = FindGameType("TacScreen");

                int setTargetCalls = CountCallOpcodes(targetCamType, "SetTargetCam");
                int tacToggleCalls = CountCallOpcodes(tacScreenType, "TacScreen_OnCamToggle");

                MfdLog.Info(
                    $"probe gameRoot={(string.IsNullOrEmpty(gameRoot) ? "?" : "ok")} " +
                    $"SetTargetCam_il_calls={setTargetCalls} TacScreen_OnCamToggle_il_calls={tacToggleCalls}");
            }
            catch (Exception ex)
            {
                MfdLog.Info("probe failed: " + ex.Message);
            }
        }

        private static Type? FindGameType(string name)
        {
            foreach (Assembly asm in AppDomain.CurrentDomain.GetAssemblies())
            {
                if (!string.Equals(asm.GetName().Name, "Assembly-CSharp", StringComparison.Ordinal))
                    continue;

                return asm.GetType(name);
            }

            return null;
        }

        private static int CountCallOpcodes(Type? type, string methodName)
        {
            if (type == null)
                return -1;

            MethodInfo? method = type.GetMethod(
                methodName,
                BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
            byte[]? il = method?.GetMethodBody()?.GetILAsByteArray();
            if (il == null || il.Length == 0)
                return 0;

            int count = 0;
            for (int i = 0; i < il.Length; i++)
            {
                if (il[i] == 0x28)
                    count++;
            }

            return count;
        }
    }
}
