using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace ParticleSystemExample
{
    public class NovaEmitter
    {
        public static Random random = new Random();
        public List<ParticleSystem.Particle> CreateSpawn(Vector3 spawnPoint, ParticleSystem.ParticleSettings settings)
        {
            float speed = (Convert.ToSingle(random.Next(Convert.ToInt32(settings.MinSpeed * 1000), Convert.ToInt32(settings.MaxSpeed * 1000))) / 1000f);
            List<ParticleSystem.Particle> particles = new List<ParticleSystem.Particle>();
            for (int i = 0; i < settings.ParticleCount; i++)
            {
                particles.Add(new NovaParticle(settings.MaxDuration,
                                                spawnPoint,
                                                speed, 
                                                (float)settings.CustomData[0],
                                                (float)random.Next(1, 30000) / 1000f,
                                                (float)settings.CustomData[1]));
            }
            return particles;
        }
    }
}
