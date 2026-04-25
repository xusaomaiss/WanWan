using UnityEngine;

namespace Wanwan.Runtime
{
    public class BombState
    {
        public BombState(int initialCount)
        {
            Count = Mathf.Max(0, initialCount);
        }

        public int Count { get; private set; }
        public bool HasBomb => Count > 0;

        public bool TryConsume()
        {
            if (!HasBomb)
            {
                return false;
            }

            Count--;
            return true;
        }
    }
}
