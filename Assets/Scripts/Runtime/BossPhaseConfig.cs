namespace Wanwan.Runtime
{
    public class BossPhaseConfig
    {
        public BossPhaseConfig(float triggerHealthNormalized, float fireInterval, int salvoCount, float spreadAngle, bool aimedCoreShot, bool extraRingShot = false, float movementSpeedMultiplier = 1f, bool isEnraged = false)
        {
            TriggerHealthNormalized = triggerHealthNormalized;
            FireInterval = fireInterval;
            SalvoCount = salvoCount;
            SpreadAngle = spreadAngle;
            AimedCoreShot = aimedCoreShot;
            ExtraRingShot = extraRingShot;
            MovementSpeedMultiplier = movementSpeedMultiplier;
            IsEnraged = isEnraged;
        }

        public float TriggerHealthNormalized { get; }
        public float FireInterval { get; }
        public int SalvoCount { get; }
        public float SpreadAngle { get; }
        public bool AimedCoreShot { get; }
        public bool ExtraRingShot { get; }
        public float MovementSpeedMultiplier { get; }
        public bool IsEnraged { get; }
    }
}
