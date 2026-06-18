using NOLoader.API;
using NOLoader.ModConfig;
using UnityEngine;

namespace NOLoader.MissileCamera
{
    public sealed class MissileCameraMod : INOMod
    {
        private const string ModVersion = "0.26.0";

        private const string DefaultIni = @"[Layout]
Enabled=1
DisplayMode=split
OverlayMaxWidth=0.45
LeftWidth=0.58
MissilePanelBottom=0.38
WeaponsStripHeight=0.12
ShowDivider=1
DebugStub=0
StubLabel=MISSILE CAMERA

[MissileCameraFeed]
Enabled=1
NoseSkinInset=0.08
CameraBackOffset=0.35
Fov=60
FeedWidth=512
FeedHeight=512
HorizonLock=1
TurnLookBankScale=1
MaxTurnLookDegrees=90
DefaultMissileGLimit=20
TurnLookGDeadband=0.15
TurnLookGFilterHz=7
TurnLookSlewDegPerSec=120
TurnLookSmoothTime=0.18
PostExplosionHoldSeconds=0
RenderFps=30

[MissileCameraHud]
Enabled=1
SalvoWindowSeconds=0.5
ShowCenterCluster=1
ShowTargetMarker=1
InterceptColor=0,1,0,1
ReticleColor=0,0.4,1,1
HorizonColor=0.05,0.35,0.08,1
HorizonOutlineColor=0.2,1,0.25,1
MissileNameColor=1,0,1,1
TargetNameColor=0.4,0.9,1,1
LabelBackgroundColor=0.18,0.18,0.18,0.62
LabelBackgroundAlpha=0.62
";

        public void OnLoad(ref NOModContext ctx)
        {
            ModIniConfig.EnsureDefault(ctx.ModRoot, DefaultIni);
            MfdLayoutConfig.Init(ctx.ModRoot);
            MissileCameraFeedConfig.Refresh(force: true);
            MissileCameraHudConfig.Refresh(force: true);
            MissileCameraFeedDriverHost.Ensure();
            MfdPatchProbe.Run(ctx.GameRoot);
            MfdLog.Info("loaded v" + ModVersion);
        }

        public void OnUnload(ref NOModContext ctx)
        {
            MissileCameraFeedDriverHost.Shutdown();
        }
    }
}
