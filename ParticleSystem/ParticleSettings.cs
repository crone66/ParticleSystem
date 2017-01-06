using Microsoft.Xna.Framework.Graphics;

namespace ParticleSystem
{
    public struct ParticleSettings
    {
        public int ParticleCount;
        public int MinDuration;
        public int MaxDuration;
        public Effect Effect;
        public Texture2D Texture;
        public float MinSpeed;
        public float MaxSpeed;
        public float Size;
        public object[] CustomData;

        /// <summary>
        /// Used to describe particle effects
        /// </summary>
        /// <param name="ParticleCount">Number of particles that should be used</param>
        /// <param name="MinDuration">Minimum effect duration</param>
        /// <param name="MaxDuration">Maximum effect duration</param>
        /// <param name="Effect">Shader</param>
        /// <param name="Texture">Particle texture</param>
        /// <param name="MinSpeed">Minimum particle speed</param>
        /// <param name="MaxSpeed">Maximum particle speed</param>
        /// <param name="Size">Particle size</param>
        /// <param name="CustomData">Additional information</param>
        public ParticleSettings(int ParticleCount, int MinDuration, int MaxDuration, Effect Effect, Texture2D Texture, float MinSpeed, float MaxSpeed, float Size, params object[] CustomData)
        {
            this.ParticleCount = ParticleCount;
            this.MinDuration = MinDuration;
            this.MaxDuration = MaxDuration;
            this.Effect = Effect;
            this.Texture = Texture;
            this.MinSpeed = MinSpeed;
            this.MaxSpeed = MaxSpeed;
            this.Size = Size;
            this.CustomData = CustomData;
        }
    }
}
