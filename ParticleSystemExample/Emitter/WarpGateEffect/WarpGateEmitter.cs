using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace ParticleSystemExample
{
    public class WarpGateEmitter
    {
        public static Random Random = new Random();
        public List<ParticleSystem.Particle> CreateSpawn(Vector3 spawnPoint, ParticleSystem.ParticleSettings settings)
        {
            List<ParticleSystem.Particle> particles = new List<ParticleSystem.Particle>();
            for (int i = 0; i < settings.ParticleCount; i++)
            {
                particles.Add(new WarpGateParticle(settings.MaxDuration,
                                                spawnPoint,
                                                (Convert.ToSingle(Random.Next(Convert.ToInt32(settings.MinSpeed * 1000), Convert.ToInt32(settings.MaxSpeed * 1000))) / 1000f) * (float)settings.CustomData[0],
                                                settings.CustomData));
            }
            return particles;
        }
    }
}
