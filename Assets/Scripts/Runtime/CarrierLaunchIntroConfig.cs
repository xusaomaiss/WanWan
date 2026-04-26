namespace Wanwan.Runtime
{
    public readonly struct CarrierLaunchIntroConfig
    {
        public CarrierLaunchIntroConfig(
            float holdSeconds,
            float launchDurationSeconds,
            float startYInset,
            float targetYInset,
            float gameplayStartYInset,
            float initialSpeed,
            float maxSpeed,
            float backgroundBoost,
            float exhaustIntensity,
            float skipInputGraceSeconds)
        {
            HoldSeconds = holdSeconds;
            LaunchDurationSeconds = launchDurationSeconds;
            StartYInset = startYInset;
            TargetYInset = targetYInset;
            GameplayStartYInset = gameplayStartYInset;
            InitialSpeed = initialSpeed;
            MaxSpeed = maxSpeed;
            BackgroundBoost = backgroundBoost;
            ExhaustIntensity = exhaustIntensity;
            SkipInputGraceSeconds = skipInputGraceSeconds;
        }

        public float HoldSeconds { get; }
        public float LaunchDurationSeconds { get; }
        public float StartYInset { get; }
        public float TargetYInset { get; }
        public float GameplayStartYInset { get; }
        public float InitialSpeed { get; }
        public float MaxSpeed { get; }
        public float BackgroundBoost { get; }
        public float ExhaustIntensity { get; }
        public float SkipInputGraceSeconds { get; }
        public float TotalDurationSeconds => HoldSeconds + LaunchDurationSeconds;

        public static CarrierLaunchIntroConfig Default => new CarrierLaunchIntroConfig(
            0.5f,
            3.2f,
            1.15f,
            9.4f,
            4.15f,
            0.18f,
            1.0f,
            3.1f,
            1.0f,
            0.35f);
    }
}
