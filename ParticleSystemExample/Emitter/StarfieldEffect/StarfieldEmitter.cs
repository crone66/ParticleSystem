using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace ParticleSystemExample
{
    public class StarfieldEmitter
    {
        public static Random Random = new Random();
        public List<ParticleSystem.Particle> CreateSpawn(Vector3 spawnPoint, ParticleSystem.ParticleSettings settings)
        {
            List<ParticleSystem.Particle> particles = new List<ParticleSystem.Particle>();
            for (int i = 0; i < settings.ParticleCount; i++)
            {
                particles.Add(new StarfieldParticle(settings.MaxDuration, spawnPoint, (float)Random.Next(500, 1000) / 500f, (Vector3)settings.CustomData[0]));
            }
            return particles;
        }
    }
}
