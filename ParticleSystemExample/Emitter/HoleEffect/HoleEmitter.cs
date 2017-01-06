using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace ParticleSystemExample
{
    public class HoleEmitter
    {
        public static Random Random = new Random();
        public List<ParticleSystem.Particle> CreateSpawn(Vector3 spawnPoint, ParticleSystem.ParticleSettings settings)
        {
            List<ParticleSystem.Particle> particles = new List<ParticleSystem.Particle>();
            for (int i = 0; i < settings.ParticleCount; i++)
            {
                particles.Add(new HoleParticle(settings.MaxDuration, spawnPoint, (float)Random.Next(500, 1000) / 10000f , 500f, Random.Next(30, 60), Random.Next(10, 500)));
            }
            return particles;
        }
    }
}
