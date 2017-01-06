using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace ParticleSystemExample
{
    public class WarpGateSparkEmitter
    {
        public static Random Random = new Random();
        public List<ParticleSystem.Particle> CreateSpawn(Vector3 spawnPoint, ParticleSystem.ParticleSettings settings)
        {
            int sparkCount = (int)settings.CustomData[0];
            float distance = (float)settings.CustomData[1];
            float scale = (float)settings.CustomData[2];
            float sparkLength = (float)settings.CustomData[3];

            int particlePerSpark = settings.ParticleCount / sparkCount;

            int counter = 0;
            List<ParticleSystem.Particle> particles = new List<ParticleSystem.Particle>();
            while (counter < sparkCount)
            {
                counter++;
                float angle = Random.Next(0, 360);
                Vector3 startPoint = new Vector3((float)(spawnPoint.X + distance * Math.Cos((Math.PI / 180) * angle)),
                                     (float)(spawnPoint.Y + distance * Math.Sin((Math.PI / 180) * angle)), 0);


                angle = Random.Next(0, 360);
                Vector3 endPoint = new Vector3((float)(startPoint.X + sparkLength * Math.Cos((Math.PI / 180) * angle)),
                                     (float)(startPoint.Y + sparkLength * Math.Sin((Math.PI / 180) * angle)), 0);

                Vector3 dir = endPoint - startPoint;
                dir.Normalize();
                float delay = (float)Random.Next(1300, 5000);
                for (int i = 0; i < particlePerSpark; i++)
                {
                    if (particles.Count < settings.ParticleCount)
                    {
                        //setup spark
                        float multiplier = ((Random.Next(1000, Convert.ToInt32((sparkLength + 2f) * 1000f)) / 1000f) - 1f);

                        particles.Add(new WarpGateSparkParticle(500f,
                            startPoint + dir * multiplier
                            ,0f, delay + sparkLength , scale));
                    }
                }
            }
            return particles;
        }
    }
}
