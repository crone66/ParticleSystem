using Microsoft.Xna.Framework;

namespace ParticleSystemExample
{
    public class WarpGateSparkParticle : ParticleSystem.RepeatAbleParticle
    {
        public WarpGateSparkParticle(float maxLifeTime, Vector3 startPosition, float speed, float delay, float scale)
            : base(maxLifeTime, startPosition, speed, delay)
        {
            lifeTime = maxLifeTime + 1;
            Scale = 3.67f * scale;
            alpha = 0.8f;
        }

        public override void Update(GameTime gameTime, Vector3 camPos, bool faceToCam = false)
        {
            base.Update(gameTime, camPos, faceToCam);
        }

        public override void CustomEvent(string eventName, params object[] args)
        {
            if (eventName == "Shutdown")
            {

            }
            else if (eventName == "Collapse")
            {
            }
        }
    }
}
