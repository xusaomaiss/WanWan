using System.Collections.Generic;
using UnityEngine;

namespace Wanwan.Runtime
{
    public class GrazeDetector : MonoBehaviour
    {
        private GrazeState grazeState;
        private PlayerController player;
        private readonly HashSet<EnemyFireballController> grazedFireballs = new HashSet<EnemyFireballController>();
        private float nextCheckTime;

        public void Initialize(GrazeState state, PlayerController playerController)
        {
            grazeState = state;
            player = playerController;
        }

        private void Update()
        {
            if (grazeState == null || player == null)
                return;

            TimeCheckAndUpdate();

            grazeState.Tick(Time.deltaTime);
        }

        private void TimeCheckAndUpdate()
        {
            float now = Time.time;
            if (now < nextCheckTime)
                return;
            nextCheckTime = now + 0.06f;

            float radius = grazeState.GrazeRadius;
            Vector2 playerPos = player.transform.position;

            var fireballs = FindObjectsByType<EnemyFireballController>(FindObjectsSortMode.None);
            for (int i = 0; i < fireballs.Length; i++)
            {
                var fb = fireballs[i];
                if (grazedFireballs.Contains(fb))
                    continue;

                float dist = Vector2.Distance(playerPos, fb.transform.position);
                if (dist < radius)
                {
                    grazeState.RegisterGraze();
                    grazedFireballs.Add(fb);
                }
            }

            grazedFireballs.RemoveWhere(f => f == null);
        }
    }
}
