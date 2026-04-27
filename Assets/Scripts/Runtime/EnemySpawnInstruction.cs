namespace Wanwan.Runtime
{
    public class EnemySpawnInstruction
    {
        public EnemySpawnInstruction(float time, EnemyFormationType formation, int count, bool elite = false, AmmoPowerupType guaranteedDrop = AmmoPowerupType.None, EnemyType enemyType = EnemyType.Normal)
        {
            Time = time;
            Formation = formation;
            Count = count;
            Elite = elite;
            GuaranteedDrop = guaranteedDrop;
            EnemyType = enemyType;
        }

        public float Time { get; }
        public EnemyFormationType Formation { get; }
        public int Count { get; }
        public bool Elite { get; }
        public AmmoPowerupType GuaranteedDrop { get; }
        public EnemyType EnemyType { get; }
    }
}
