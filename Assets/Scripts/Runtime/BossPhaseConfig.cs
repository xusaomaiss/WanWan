namespace Wanwan.Runtime
{
    public class BossPhaseConfig
    {
        public BossPhaseConfig(float triggerHealthNormalized, float fireInterval, int salvoCount, float spreadAngle, bool aimedCoreShot)
        {
            TriggerHealthNormalized = triggerHealthNormalized;
            FireInterval = fireInterval;
            SalvoCount = salvoCount;
            SpreadAngle = spreadAngle;
            AimedCoreShot = aimedCoreShot;
        }

        public float TriggerHealthNormalized { get; }
        public float FireInterval { get; }
        public int SalvoCount { get; }
        public float SpreadAngle { get; }
        public bool AimedCoreShot { get; }
    }
}
