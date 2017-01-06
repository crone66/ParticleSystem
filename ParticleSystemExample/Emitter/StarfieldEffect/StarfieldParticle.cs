using Microsoft.Xna.Framework;
using System;

namespace ParticleSystemExample
{
    public class StarfieldParticle : ParticleSystem.Particle
    {
        private float angle;
        private float startDistance = -300f;
        public StarfieldParticle(float maxLifeTime, Vector3 startPosition, float speed, Vector3 camPos)
            : base(maxLifeTime, startPosition, speed)
        {
            Scale = 5f;
            ResetParticle(camPos);
            
        }


        public override void Update(GameTime gameTime, Vector3 camPos, bool faceToCam = false)
        {
            if (isActive)
            {
                currentPosition += new Vector3(0, 0, speed * (float)gameTime.ElapsedGameTime.TotalMilliseconds);

                if (currentPosition.Z < 0)
                    alpha = 1f - (currentPosition.Z / startDistance);
                else
                    alpha = 1f;

                if (currentPosition.Z > camPos.Z)
                {
                    ResetParticle(camPos);
                }
            }
            base.Update(gameTime, camPos, faceToCam);
        }

        private void ResetParticle(Vector3 camPos)
        {
            angle = HoleEmitter.Random.Next(0, 360);
            currentPosition = new Vector3((float)(StartPosition.X + StarfieldEmitter.Random.Next(0, 1300) * Math.Cos((Math.PI / 180) * angle)), (float)(StartPosition.Y + StarfieldEmitter.Random.Next(0, 1300) * Math.Sin((Math.PI / 180) * angle)), StarfieldEmitter.Random.Next(Convert.ToInt32(startDistance), 1));
            alpha = 0.01f;
        }

        public override void CustomEvent(string eventName, params object[] args)
        {
        }
    }
}
