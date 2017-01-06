using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ParticleSystem
{
    public struct Quad
    {
        private VertexPositionTexture[] vertices;
        private short[] indices;
        private float width;
        private float height;

        public VertexPositionTexture[] Vertices
        {
            get { return vertices; }
        }
        
        public float Width
        {
            get { return width; }
        }
        
        public float Height
        {
            get { return height; }
        }
        
        public short[] Indices
        {
            get { return indices; }
            set { indices = value; }
        }

        /// <summary>
        /// Initzializes a new quad
        /// </summary>
        /// <param name="width">Quad width</param>
        /// <param name="height">Quad height</param>
        public Quad(float width, float height)
        {
            this.width = width;
            this.height = height;

            vertices = new VertexPositionTexture[4];
            indices = new short[6];

            vertices[0].Position = new Vector3(width / -2, height / 2, 0); // lowleft
            vertices[1].Position = new Vector3(width / -2, height / -2, 0); //uppperleft
            vertices[2].Position = new Vector3(width / 2, height / 2, 0); //lowRight
            vertices[3].Position = new Vector3(width / 2, height / -2, 0); //upperRight

            vertices[0].TextureCoordinate = new Vector2(0f, 1f);
            vertices[1].TextureCoordinate = new Vector2(0f, 0f);
            vertices[2].TextureCoordinate = new Vector2(1f, 1f);
            vertices[3].TextureCoordinate = new Vector2(1f, 0f);

            indices[0] = 0;
            indices[1] = 1;
            indices[2] = 2;
            indices[3] = 2;
            indices[4] = 1;
            indices[5] = 3;
        }
    }
}
