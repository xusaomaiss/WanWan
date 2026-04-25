namespace Wanwan.Runtime
{
    public class StageWaveConfig
    {
        public StageWaveConfig(StagePhase phase, string banner, float durationSeconds, bool waitForClear, EnemySpawnInstruction[] instructions)
        {
            Phase = phase;
            Banner = banner;
            DurationSeconds = durationSeconds;
            WaitForClear = waitForClear;
            Instructions = instructions;
        }

        public StagePhase Phase { get; }
        public string Banner { get; }
        public float DurationSeconds { get; }
        public bool WaitForClear { get; }
        public EnemySpawnInstruction[] Instructions { get; }
    }
}
