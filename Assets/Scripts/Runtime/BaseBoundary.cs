using UnityEngine;

namespace Wanwan.Runtime
{
    public class BaseBoundary : MonoBehaviour
    {
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.TryGetComponent(out BlockController block))
            {
                block.ReachBase();
            }
        }
    }
}
