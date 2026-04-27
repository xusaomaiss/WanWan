using UnityEngine;

namespace Wanwan.Runtime
{
    public class StageGameplayProfile
    {
        private const float FirstPowerCapsuleProgress = 0.14f;
        private const float LastPowerCapsuleProgress = 0.84f;
        private const float FirstGuaranteedPickupProgress = 0.08f;
        private const float LastGuaranteedPickupProgress = 0.9f;

        private static readonly AmmoPowerupType[] DefaultGuaranteedAmmoPacks =
        {
            AmmoPowerupType.Scatter,
            AmmoPowerupType.RapidFire,
            AmmoPowerupType.Pierce,
            AmmoPowerupType.Laser,
            AmmoPowerupType.Plasma,
            AmmoPowerupType.Burst,
            AmmoPowerupType.Homing,
            AmmoPowerupType.Wave,
            AmmoPowerupType.Guard
        };

        public StageGameplayProfile(
            StageCombatStyle formationBias,
            AmmoPowerupType[] signaturePowerups,
            int powerMeterCapsuleBudget,
            float rewardBias,
            float groundThreatBias)
        {
            FormationBias = formationBias;
            SignaturePowerups = signaturePowerups;
            PowerMeterCapsuleBudget = Mathf.Max(0, powerMeterCapsuleBudget);
            RewardBias = Mathf.Clamp01(rewardBias);
            GroundThreatBias = Mathf.Max(0f, groundThreatBias);
            GuaranteedAmmoPacks = (AmmoPowerupType[])DefaultGuaranteedAmmoPacks.Clone();
            GuaranteedMedicalPacks = 1;
        }

        public StageCombatStyle FormationBias { get; }
        public AmmoPowerupType[] SignaturePowerups { get; }
        public int PowerMeterCapsuleBudget { get; }
        public bool GuaranteesPowerMeterActivation => PowerMeterCapsuleBudget > 0;
        public float RewardBias { get; }
        public float GroundThreatBias { get; }
        public AmmoPowerupType[] GuaranteedAmmoPacks { get; }
        public int GuaranteedMedicalPacks { get; }
        public int GuaranteedPickupCount => GuaranteedMedicalPacks + (GuaranteedAmmoPacks?.Length ?? 0);

        public AmmoPowerupType ChooseThemedPowerup(int roll, bool rewardPhase)
        {
            if (SignaturePowerups == null || SignaturePowerups.Length == 0)
            {
                return AmmoPowerupType.Scatter;
            }

            if (rewardPhase)
            {
                return SignaturePowerups[0];
            }

            int safeIndex = Mathf.Abs(roll) % SignaturePowerups.Length;
            return SignaturePowerups[safeIndex];
        }

        public float GetPowerCapsuleUnlockProgress(int capsuleIndex)
        {
            return GetDistributedUnlockProgress(capsuleIndex, PowerMeterCapsuleBudget, FirstPowerCapsuleProgress, LastPowerCapsuleProgress);
        }

        public float GetGuaranteedPickupUnlockProgress(int pickupIndex)
        {
            return GetDistributedUnlockProgress(pickupIndex, GuaranteedPickupCount, FirstGuaranteedPickupProgress, LastGuaranteedPickupProgress);
        }

        private static float GetDistributedUnlockProgress(int index, int count, float firstProgress, float lastProgress)
        {
            if (count <= 1)
            {
                return firstProgress;
            }

            float normalizedIndex = Mathf.Clamp(index, 0, count - 1) / (float)(count - 1);
            return Mathf.Lerp(firstProgress, lastProgress, normalizedIndex);
        }
    }
}
