using UnityEngine;

namespace NOLoader.MissileCamera
{
    internal static class Patches
    {
        public static void SetupCameraPostfix(TargetScreenUI __instance, Camera cam, Camera UICam, object aircraftObj)
        {
            Bootstrap();
            if (aircraftObj is not Component aircraft)
                return;

            TargetCam? targetCam = TargetCamAccess.GetTargetCam(aircraft);
            if (targetCam == null)
                return;

            TacScreen? tacScreen = TacScreenAccess.Resolve(aircraft);
            if (tacScreen != null)
                TacScreenAccess.Register(aircraft, tacScreen);

            MfdLayoutController.OnSetupCamera(__instance, targetCam);
        }

        public static void SetLandingCamPostfix(TargetCam __instance)
        {
            Bootstrap();
            MfdLayoutController.OnSetLandingCam(__instance);
        }

        public static void CancelTargetPostfix(TargetCam __instance)
        {
            Bootstrap();
            MfdLayoutController.OnCancelTarget(__instance);
        }

        public static void OnDestroyPostfix(TargetCam __instance)
        {
            Bootstrap();
            MfdLayoutController.OnTargetCamDestroy(__instance);
        }

        public static void TacScreenInitializePostfix(TacScreen __instance)
        {
            Bootstrap();
            Component? aircraft = TacScreenAccess.GetAircraft(__instance);
            if (aircraft != null)
                TacScreenAccess.Register(aircraft, __instance);

            if (aircraft == null)
                return;

            TargetCam? targetCam = TargetCamAccess.GetTargetCam(aircraft);
            if (targetCam == null)
                return;

            string? jsonKey = MfdDisplayMode.GetAircraftJsonKey(targetCam);
            if (!TacScreenAccess.UsesEarlyTacLayoutTrigger(jsonKey))
                return;

            MfdLayoutController.OnTacScreenReady(__instance, targetCam);
        }

        public static void TacScreenOnCamTogglePostfix(TacScreen __instance, TargetCam.OnCamToggle e)
        {
            Bootstrap();
            Component? aircraft = TacScreenAccess.GetAircraft(__instance);
            if (aircraft != null)
                TacScreenAccess.Register(aircraft, __instance);

            TargetCam? targetCam = aircraft != null ? TargetCamAccess.GetTargetCam(aircraft) : null;

            if (!e.enabled || e.camMode == TargetCam.CamMode.landingMode)
            {
                MfdLayoutController.OnTargetCamDisabled(targetCam);
                return;
            }

            if (targetCam == null)
                return;

            MfdLayoutController.OnTacCamToggle(targetCam);
        }

        public static void TargetListChangedPostfix(WeaponManager __instance)
        {
            Bootstrap();
            if (WeaponManagerAccess.GetTargetCount(__instance) > 0)
                return;

            Component? aircraft = WeaponManagerAccess.GetAircraft(__instance);
            TargetCam? targetCam = aircraft != null ? TargetCamAccess.GetTargetCam(aircraft) : null;
            MfdLayoutController.OnTargetListCleared(targetCam);
        }

        private static void Bootstrap()
        {
            MfdLayoutConfig.EnsureInitialized();
        }
    }
}
