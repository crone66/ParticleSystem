using Microsoft.Xna.Framework;
using System;

namespace ParticleSystemExample
{
    public class NovaParticle : ParticleSystem.Particle
    {
        private float angle;
        private float distance;
        private float endDistance;

        public NovaParticle(float maxLifeTime, Vector3 startPosition, float speed, float scale, float startDistance, float endDistance)
            : base(maxLifeTime, startPosition, speed)
        {
            angle = NovaEmitter.random.Next(0, 360);
            this.endDistance = endDistance;
            distance = startDistance;
            Scale = scale;
            alpha = 1f;
            currentPosition = new Vector3((float)(startPosition.X + distance * Math.Cos((Math.PI / 180) * angle)),
                                            (float)(startPosition.Y + distance * Math.Sin((Math.PI / 180) * angle)),
                                            0);
        }

        public override void Update(GameTime gameTime, Vector3 camPos, bool faceToCam = false)
        {
            if (distance < endDistance)
            {
                distance += (float)gameTime.ElapsedGameTime.TotalMilliseconds * speed;
                alpha = 1f - distance / endDistance;
            }

            currentPosition = new Vector3((float)(StartPosition.X + distance * Math.Cos((Math.PI / 180) * angle)),
                                            (float)(StartPosition.Y + distance * Math.Sin((Math.PI / 180) * angle)),
                                            0);
            base.Update(gameTime, camPos, faceToCam);
        }

        public override void CustomEvent(string eventName, params object[] args)
        {
        }
    }
}
