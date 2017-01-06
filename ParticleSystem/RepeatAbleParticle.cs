using Microsoft.Xna.Framework;

namespace ParticleSystem
{
    public abstract class RepeatAbleParticle : Particle
    {
        private int repeats;
        private float delay;
        private float elapsedTime;

        public int Repeats
        {
            get
            {
                return repeats;
            }
            set
            {
                repeats = value;
            }
        }

        public float Delay
        {
            get
            {
                return delay;
            }
            set
            {
                delay = value;
            }
        }

        /// <summary>
        /// Initzializes a new repeatable particle
        /// </summary>
        /// <param name="maxLifeTime">Maximum life time</param>
        /// <param name="startPosition">Spawn position</param>
        /// <param name="delay">Activation delay</param>
        /// <param name="repeats">Number of repeats</param>
        public RepeatAbleParticle(float maxLifeTime, Vector3 startPosition, float delay, int repeats = -1)
            : base(maxLifeTime, startPosition)
        {
            Repeats = repeats;
            Delay = delay;
            elapsedTime = 0f;
        }

        public RepeatAbleParticle(float maxLifeTime, Vector3 startPosition, float speed, float delay, int repeats = -1)
            : base(maxLifeTime, startPosition, speed)
        {
            Repeats = repeats;
            Delay = delay;
            elapsedTime = 0f;
        }

        /// <summary>
        /// Updates repeatable particle
        /// </summary>
        /// <param name="gameTime">Game time</param>
        /// <param name="cameraPosition">Current camera position</param>
        /// <param name="faceToCam">Indicates whenther the particle should face to the camera or not</param>
        public override void Update(GameTime gameTime, Vector3 cameraPosition, bool faceToCam = false)
        {
            if(!isActive && Repeats != 0)
            {
                elapsedTime += (float)gameTime.ElapsedGameTime.TotalMilliseconds;
                if(elapsedTime > Delay)
                {
                    if (Repeats > 0)
                        Repeats--;

                    elapsedTime = 0f;
                    Reset();
                }
            }
            base.Update(gameTime, cameraPosition, faceToCam);
        }
    }
}
