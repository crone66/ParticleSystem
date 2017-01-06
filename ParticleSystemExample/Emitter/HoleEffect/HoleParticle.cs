using Microsoft.Xna.Framework;
using System;

namespace ParticleSystemExample
{
    public class HoleParticle : ParticleSystem.Particle
    {
        private float maxDistance;
        private float minDistance;
        private float distance = 0;
        private float angle;

        public HoleParticle(float maxLifeTime, Vector3 startPosition, float speed, float maxDistance, float minDistance, float distance)
            : base(maxLifeTime, startPosition, speed)
        {
            this.maxDistance = maxDistance;
            this.minDistance = minDistance;
            this.distance = distance;
            Scale = 5f;
            angle = HoleEmitter.Random.Next(0, 360);
            currentPosition = new Vector3((float)(currentPosition.X + maxDistance * Math.Cos((Math.PI / 180) * angle)), (float)(currentPosition.Y + maxDistance * Math.Sin((Math.PI / 180) * angle)), 0);
        }


        public override void Update(GameTime gameTime, Vector3 camPos, bool faceToCam = false)
        {
            if (isActive)
            {
                distance -= speed * (float)gameTime.ElapsedGameTime.TotalMilliseconds;
                if (distance <= minDistance)
                {
                    distance = maxDistance;
                    angle = HoleEmitter.Random.Next(0, 360);
                }

                alpha = 0.7f;
                distance -= speed * (float)gameTime.ElapsedGameTime.TotalMilliseconds;
                currentPosition = new Vector3((float)(StartPosition.X + distance * Math.Cos((Math.PI / 180) * angle)), (float)(StartPosition.Y + distance * Math.Sin((Math.PI / 180) * angle)), 0);
            }
            base.Update(gameTime, camPos, faceToCam);
        }

        public override void CustomEvent(string eventName, params object[] args)
        {
        }
    }
}
