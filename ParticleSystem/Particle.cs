using Microsoft.Xna.Framework;

namespace ParticleSystem
{
    public abstract class Particle
    {
        protected bool isActive;
        protected float alpha;

        protected float lifeTime;
        protected float maxLifeTime;

        private Vector3 startPosition;
        protected Vector3 currentPosition;
        protected bool updatePosition;
        protected float speed;

        private InstancingAlpha info;
        private InstancingWorldMatrix worldInfo;
        private Matrix rotationMatrix;
        private float scale;

        /// <summary>
        /// Indicates whenther the particle is active or not
        /// </summary>
        public bool IsActive
        {
            get { return isActive; }
            set { isActive = value; }
        }

        /// <summary>
        /// Alpha value
        /// </summary>
        public float Alpha
        {
            get { return alpha; }
            set { alpha = value; }
        }

        /// <summary>
        /// Current position
        /// </summary>
        public Vector3 Position
        {
            get
            {
                return currentPosition;
            }
            set
            {
                currentPosition = value;
            }
        }

        /// <summary>
        /// Alpha information
        /// </summary>
        public InstancingAlpha Info
        {
            get { return info; }
            set { info = value; }
        }

        /// <summary>
        /// World matrix information
        /// </summary>
        public InstancingWorldMatrix WorldInfo
        {
            get { return worldInfo; }
            set { worldInfo = value; }
        }

        /// <summary>
        /// Rotation matrix of particle
        /// </summary>
        public Matrix RotationMatrix
        {
            get
            {
                return rotationMatrix;
            }
            set
            {
                rotationMatrix = value;
            }
        }

        /// <summary>
        /// Particle scale
        /// </summary>
        public float Scale
        {
            get
            {
                return scale;
            }
            set
            {
                scale = value;
            }
        }

        /// <summary>
        /// Spawn position of particle
        /// </summary>
        public Vector3 StartPosition
        {
            get
            {
                return startPosition;
            }
            set
            {
                startPosition = value;
            }
        }

        /// <summary>
        /// Initzializes a new Particle
        /// </summary>
        /// <param name="maxLifeTime">Maximum life time of particle</param>
        /// <param name="startPosition">Spawn position</param>
        public Particle(float maxLifeTime, Vector3 startPosition)
        {
            this.maxLifeTime = maxLifeTime;

            isActive = true;
            alpha = 1f;
            scale = 1f;
            lifeTime = 0f;
            StartPosition = startPosition;
            speed = 0f;
            updatePosition = false;
            currentPosition = startPosition;

            info = new InstancingAlpha(alpha);
            worldInfo = new InstancingWorldMatrix(Matrix.Identity * Matrix.CreateTranslation(currentPosition));
        }

        /// <summary>
        /// Initzializes a new Particle
        /// </summary>
        /// <param name="maxLifeTime">Maximum life time of particle</param>
        /// <param name="startPosition">Spawn position</param>
        /// <param name="speed">Movement speed</param>
        public Particle(float maxLifeTime, Vector3 startPosition, float speed)
        {
            this.maxLifeTime = maxLifeTime;
            this.speed = speed;
            isActive = true;
            alpha = 1f;
            scale = 1f;
            lifeTime = 0f;
            StartPosition = startPosition;
            updatePosition = true;
            currentPosition = startPosition;

            info = new InstancingAlpha(alpha);
            worldInfo = new InstancingWorldMatrix(Matrix.Identity * Matrix.CreateTranslation(currentPosition));
        }

        /// <summary>
        /// Resets life time of this particle
        /// </summary>
        public virtual void Reset()
        {
            lifeTime = 0f;
            isActive = true;
        }

        /// <summary>
        /// Updates particle
        /// </summary>
        /// <param name="gameTime">Game time</param>
        /// <param name="cameraPosition">Current camera position</param>
        /// <param name="faceToCam">Indicates whenther the particle should face to the camera</param>
        public virtual void Update(GameTime gameTime, Vector3 cameraPosition, bool faceToCam = false)
        {
            if (isActive)
            {
                if(faceToCam)
                    rotationMatrix = ParticleEffect.FaceToCamera(cameraPosition, new Vector3(currentPosition.X, currentPosition.Y, currentPosition.Z));

                lifeTime += (float)gameTime.ElapsedGameTime.TotalMilliseconds;
                if (maxLifeTime > 0 && lifeTime > maxLifeTime)
                {
                    isActive = false;
                    info = new InstancingAlpha(0f);
                    return;
                }

                worldInfo = new InstancingWorldMatrix(Matrix.CreateScale(scale) * rotationMatrix * Matrix.CreateTranslation(currentPosition));
                info = new InstancingAlpha(alpha);
            }
        }

        /// <summary>
        /// Custom events can be used to change the behavior of the particle
        /// </summary>
        /// <param name="eventName">Event name</param>
        /// <param name="args">Event arguments</param>
        public abstract void CustomEvent(string eventName, params object[] args);
    }
}
