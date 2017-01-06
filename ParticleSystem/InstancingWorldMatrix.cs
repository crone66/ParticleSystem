using Microsoft.Xna.Framework;

namespace ParticleSystem
{
    public struct InstancingWorldMatrix
    {
        public Matrix world;

        public InstancingWorldMatrix(Matrix world)
        {
            this.world = world;
        }
    }
}
