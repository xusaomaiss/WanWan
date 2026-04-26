using UnityEngine;

namespace Wanwan.Runtime
{
    public class FireLevelState
    {
        public const int MinLevel = 1;
        public const int MaxLevel = 4;

        public int Level { get; private set; } = MinLevel;

        public void Increase()
        {
            Level = Mathf.Min(MaxLevel, Level + 1);
        }
    }
}
