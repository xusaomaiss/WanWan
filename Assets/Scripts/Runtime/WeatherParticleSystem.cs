using UnityEngine;
using Wanwan.Runtime.Pools;

namespace Wanwan.Runtime
{
    public class WeatherParticleSystem : MonoBehaviour
    {
        private StageWeather weather;
        private PoolCollection pools;
        private GameObject[] activeParticles;
        private int particleCount;
        private float spawnTimer;
        private float leftBound;
        private float rightBound;
        private float topBound;
        private float bottomBound;

        public void Initialize(StageWeather weather, PoolCollection pools, float minX, float maxX, float minY, float maxY)
        {
            this.weather = weather;
            this.pools = pools;
            leftBound = minX;
            rightBound = maxX;
            topBound = maxY;
            bottomBound = minY;
            particleCount = weather == StageWeather.None ? 0 :
                SessionState.VisualEffectsQuality == VisualEffectsQuality.Full ? 40 : 16;
            activeParticles = new GameObject[particleCount];

            for (int i = 0; i < particleCount; i++)
            {
                SpawnParticle(i, Random.Range(0f, 999f));
            }
        }

        private void Update()
        {
            if (weather == StageWeather.None || particleCount == 0)
                return;

            spawnTimer -= Time.deltaTime;
            if (spawnTimer <= 0f)
            {
                spawnTimer = 0.08f;
            }

            for (int i = 0; i < particleCount; i++)
            {
                if (activeParticles[i] == null)
                    continue;

                Vector3 pos = activeParticles[i].transform.position;

                switch (weather)
                {
                    case StageWeather.Rain:
                        pos.x -= 1.2f * Time.deltaTime;
                        pos.y -= 12f * Time.deltaTime;
                        break;
                    case StageWeather.Spores:
                        pos.x += Mathf.Sin(Time.time * 0.8f + i) * 0.4f * Time.deltaTime;
                        pos.y -= 0.6f * Time.deltaTime;
                        break;
                    case StageWeather.Sand:
                        pos.x += 1.8f * Time.deltaTime;
                        pos.y -= 1.5f * Time.deltaTime;
                        break;
                }

                if (pos.y < bottomBound - 0.5f)
                {
                    pos.y = topBound + Random.Range(0f, 1f);
                    pos.x = Random.Range(leftBound, rightBound);
                }
                else if (pos.x < leftBound - 0.5f)
                {
                    pos.x = rightBound + Random.Range(0f, 0.5f);
                    pos.y = Random.Range(bottomBound, topBound);
                }
                else if (pos.x > rightBound + 0.5f)
                {
                    pos.x = leftBound - Random.Range(0f, 0.5f);
                    pos.y = Random.Range(bottomBound, topBound);
                }

                activeParticles[i].transform.position = pos;
            }
        }

        private void SpawnParticle(int index, float y)
        {
            if (pools != null)
            {
                activeParticles[index] = pools.RentParticle();
            }
            else
            {
                activeParticles[index] = new GameObject("WeatherParticle");
            }

            activeParticles[index].transform.position = new Vector3(
                Random.Range(leftBound, rightBound),
                topBound + Random.Range(0f, 2f),
                5f);

            SpriteRenderer renderer = activeParticles[index].GetComponent<SpriteRenderer>();
            if (renderer == null) renderer = activeParticles[index].AddComponent<SpriteRenderer>();

            Color particleColor = weather switch
            {
                StageWeather.Rain => new Color(0.6f, 0.75f, 1f, 0.35f),
                StageWeather.Spores => new Color(0.4f, 1f, 0.7f, 0.2f),
                StageWeather.Sand => new Color(1f, 0.85f, 0.5f, 0.25f),
                _ => Color.clear
            };

            renderer.color = particleColor;
            renderer.sortingOrder = -40;

            float scale = weather == StageWeather.Rain ? 0.04f : 0.08f;
            activeParticles[index].transform.localScale = Vector3.one * scale;
        }
    }
}
