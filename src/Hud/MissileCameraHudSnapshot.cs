using System.Collections.Generic;
using UnityEngine;

namespace NOLoader.MissileCamera
{
    internal readonly struct MissileCameraHudSnapshot
    {
        internal readonly bool HasFeed;
        internal readonly bool HasTarget;
        internal readonly bool HasAimPoint;
        internal readonly string MissileName;
        internal readonly string TargetName;
        internal readonly string SpeedText;
        internal readonly string AltitudeText;
        internal readonly string RangeText;
        internal readonly string SalvoText;
        internal readonly string SpeedRow;
        internal readonly string AltitudeRow;
        internal readonly string RangeRow;
        internal readonly int SalvoIndex;
        internal readonly int SalvoTotal;
        internal readonly GlobalPosition AimPoint;
        internal readonly GlobalPosition TargetPosition;
        internal readonly float PitchDeg;
        internal readonly float RollDeg;

        private MissileCameraHudSnapshot(
            bool hasFeed,
            bool hasTarget,
            bool hasAimPoint,
            string missileName,
            string targetName,
            string speedText,
            string altitudeText,
            string rangeText,
            string salvoText,
            string speedRow,
            string altitudeRow,
            string rangeRow,
            int salvoIndex,
            int salvoTotal,
            GlobalPosition aimPoint,
            GlobalPosition targetPosition,
            float pitchDeg,
            float rollDeg)
        {
            HasFeed = hasFeed;
            HasTarget = hasTarget;
            HasAimPoint = hasAimPoint;
            MissileName = missileName;
            TargetName = targetName;
            SpeedText = speedText;
            AltitudeText = altitudeText;
            RangeText = rangeText;
            SalvoText = salvoText;
            SpeedRow = speedRow;
            AltitudeRow = altitudeRow;
            RangeRow = rangeRow;
            SalvoIndex = salvoIndex;
            SalvoTotal = salvoTotal;
            AimPoint = aimPoint;
            TargetPosition = targetPosition;
            PitchDeg = pitchDeg;
            RollDeg = rollDeg;
        }

        internal static MissileCameraHudSnapshot Empty => new MissileCameraHudSnapshot(
            hasFeed: false,
            hasTarget: false,
            hasAimPoint: false,
            missileName: string.Empty,
            targetName: "---",
            speedText: "---",
            altitudeText: "---",
            rangeText: "---",
            salvoText: "1/1",
            speedRow: "S:---",
            altitudeRow: "A:---",
            rangeRow: "R:---",
            salvoIndex: 1,
            salvoTotal: 1,
            aimPoint: default,
            targetPosition: default,
            pitchDeg: 0f,
            rollDeg: 0f);

        internal static MissileCameraHudSnapshot Build(
            Missile? missile,
            MissileCameraRig? rig,
            IReadOnlyList<Missile> ownedActive)
        {
            _ = ownedActive;
            if (missile == null || missile.disabled || missile.rb == null)
                return Empty;

            MissileCameraSalvoTracker.GetSalvoInfo(missile, out int salvoIndex, out int salvoTotal);

            bool hasTarget = MissileAccess.TryGetTargetPosition(missile, out GlobalPosition targetPosition);
            bool hasAimPoint = MissileAccess.TryGetAimPoint(missile, out GlobalPosition aimPoint);

            float boreRollDeg = rig != null ? rig.BoreRollDeg : 0f;
            float pitchDeg = rig != null ? -HorizonFrame.ComputeCameraPitchDeg(rig.FeedCamera) : 0f;
            float rollDeg = -boreRollDeg;

            string speedText = MissileCameraTelemetry.FormatSpeed(missile);
            string altitudeText = MissileCameraTelemetry.FormatAltitude(missile);
            string rangeText = MissileCameraTelemetry.FormatRange(missile);

            return new MissileCameraHudSnapshot(
                hasFeed: rig?.Texture != null,
                hasTarget: hasTarget,
                hasAimPoint: hasAimPoint,
                missileName: MissileAccess.GetMissileName(missile),
                targetName: MissileAccess.GetTargetName(missile),
                speedText: speedText,
                altitudeText: altitudeText,
                rangeText: rangeText,
                salvoText: $"{salvoIndex}/{salvoTotal}",
                speedRow: MissileCameraTelemetry.FormatLabeledRow("S", speedText),
                altitudeRow: MissileCameraTelemetry.FormatLabeledRow("A", altitudeText),
                rangeRow: MissileCameraTelemetry.FormatLabeledRow("R", rangeText),
                salvoIndex: salvoIndex,
                salvoTotal: salvoTotal,
                aimPoint: aimPoint,
                targetPosition: targetPosition,
                pitchDeg: pitchDeg,
                rollDeg: rollDeg);
        }
    }
}
