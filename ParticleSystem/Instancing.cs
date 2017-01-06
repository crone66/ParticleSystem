using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ParticleSystem
{
    public class Instancing
    {
        private Texture2D texture;

        private GraphicsDevice device;

        private Effect effect;

        private VertexPositionTexture[] vertices;
        private VertexBufferBinding[] vertexBindings;

        private VertexBuffer vertexBuffer;
        private VertexBuffer alphaBuffer;
        private VertexBuffer worldBuffer;
        private IndexBuffer indexBuffer;

        private VertexDeclaration alphaVertexDeclaration;

        private VertexDeclaration worldVertexDeclaration;
        private int objectCount;

        private InstancingAlpha[] instancingAlphaInfo;
        private InstancingWorldMatrix[] instancingWorldInfo;

        public Instancing(GraphicsDevice device, Effect effect, Texture2D texture, VertexPositionTexture[] vertices, short[] indices, int objectCount, List<InstancingWorldMatrix> instancingWorldInfo)
        {
            this.device = device;
            this.effect = effect;
            this.texture = texture;
            this.objectCount = objectCount;
            this.vertices = vertices;

            SetupVertexDeclaration();

            vertexBuffer = new VertexBuffer(device, typeof(VertexPositionTexture), vertices.Length, BufferUsage.WriteOnly);
            indexBuffer = new IndexBuffer(device, typeof(short), indices.Length, BufferUsage.WriteOnly);
            vertexBuffer.SetData(vertices);
            indexBuffer.SetData(indices);

            CreateAdditionalInformation(instancingWorldInfo);
        }

        public Instancing(GraphicsDevice device, Effect effect, Texture2D texture, VertexPositionTexture[] vertices, short[] indices, int objectCount, List<InstancingWorldMatrix> instancingWorldInfo, List<InstancingAlpha> instancingAlphaInfo)
        {
            this.device = device;
            this.effect = effect;
            this.texture = texture;
            this.objectCount = objectCount;
            this.vertices = vertices;

            SetupVertexDeclaration();

            vertexBuffer = new VertexBuffer(device, typeof(VertexPositionTexture), vertices.Length, BufferUsage.WriteOnly);
            indexBuffer = new IndexBuffer(device, typeof(short), indices.Length, BufferUsage.WriteOnly);
            vertexBuffer.SetData(vertices);
            indexBuffer.SetData(indices);

            CreateAdditionalInformation(instancingWorldInfo, instancingAlphaInfo);
        }

        /// <summary>
        /// Vertex declaration for world buffer and alpha buffer
        /// </summary>
        private void SetupVertexDeclaration()
        {
            alphaVertexDeclaration = new VertexDeclaration(
                    new VertexElement(0, VertexElementFormat.Single, VertexElementUsage.BlendWeight, 1)
                    );

            worldVertexDeclaration = new VertexDeclaration(new VertexElement(0, VertexElementFormat.Vector4, VertexElementUsage.Binormal, 1),
                   new VertexElement(16, VertexElementFormat.Vector4, VertexElementUsage.Binormal, 2),
                   new VertexElement(32, VertexElementFormat.Vector4, VertexElementUsage.Binormal, 3),
                   new VertexElement(48, VertexElementFormat.Vector4, VertexElementUsage.Binormal, 4));
        }

        /// <summary>
        /// Sets vertex buffer bindings for vertices, alpha value and world matrices
        /// </summary>
        private void SetVertexBinding()
        {
            vertexBindings = new VertexBufferBinding[3] { new VertexBufferBinding(vertexBuffer), new VertexBufferBinding(alphaBuffer, 0, 1), new VertexBufferBinding(worldBuffer, 0, 1) };
        }

        /// <summary>
        /// Updates alpha buffer
        /// </summary>
        /// <param name="alphaInfo">Alpha information</param>
        private void CreateAdditionalInformation(List<InstancingAlpha> alphaInfo)
        {
            instancingAlphaInfo = alphaInfo.ToArray();
            if (alphaBuffer != null) //Fix for a monogames memory leak? (Not required in XNA)
                alphaBuffer.Dispose();

            alphaBuffer = new VertexBuffer(device, alphaVertexDeclaration, objectCount, BufferUsage.WriteOnly);
            alphaBuffer.SetData(instancingAlphaInfo);

            SetVertexBinding();
        }

        /// <summary>
        /// Updates world matrices buffer and alpha buffer
        /// </summary>
        /// <param name="worldInfo">World matrices information</param>
        /// <param name="alphaInfo">Alpha value information</param>
        private void CreateAdditionalInformation(List<InstancingWorldMatrix> worldInfo, List<InstancingAlpha> alphaInfo)
        {
            instancingAlphaInfo = alphaInfo.ToArray();
            if(alphaBuffer != null) //Fix for a monogames memory leak? (Not required in XNA)
                alphaBuffer.Dispose();

            alphaBuffer = new VertexBuffer(device, alphaVertexDeclaration, objectCount, BufferUsage.WriteOnly);
            alphaBuffer.SetData(instancingAlphaInfo);
            
            instancingWorldInfo = worldInfo.ToArray();
            if(worldBuffer != null) //Fix for a monogames memory leak? (Not required in XNA)
                worldBuffer.Dispose();

            worldBuffer = new VertexBuffer(device, worldVertexDeclaration, objectCount, BufferUsage.WriteOnly);
            worldBuffer.SetData(instancingWorldInfo);

            SetVertexBinding();
        }

        /// <summary>
        /// Updates world matrices buffer
        /// </summary>
        /// <param name="worldInfo">World matrices information</param>
        private void CreateAdditionalInformation(List<InstancingWorldMatrix> worldInfo)
        {
            instancingWorldInfo = worldInfo.ToArray();
            if (worldBuffer != null) //Fix for a monogames memory leak? (Not required in XNA)
                worldBuffer.Dispose();

            worldBuffer = new VertexBuffer(device, worldVertexDeclaration, objectCount, BufferUsage.WriteOnly);
            worldBuffer.SetData(instancingWorldInfo);

            SetVertexBinding();
        }

        /// <summary>
        /// Update alpha buffer when required
        /// </summary>
        /// <param name="gameTime">Game time</param>
        /// <param name="alphaInfo">Alpha buffer information</param>
        /// <param name="updateAlphaInfo">Indicates whenther to update the alpha buffer or not</param>
        public void Update(GameTime gameTime, List<InstancingAlpha> alphaInfo, bool updateAlphaInfo = false)
        {
            if (updateAlphaInfo)
                CreateAdditionalInformation(alphaInfo);
        }

        /// <summary>
        /// Updates world matrices and alpha value when required
        /// </summary>
        /// <param name="gameTime">Game time</param>
        /// <param name="worldInfo">World matrices information</param>
        /// <param name="updateWorldInfo">Indicates whenther to update the world buffer or not</param>
        /// <param name="alphaInfo">Alpha value information</param>
        /// <param name="updateAlphaInfo">Indicates whenther to update alpha buffe or not</param>
        public void Update(GameTime gameTime, List<InstancingWorldMatrix> worldInfo, bool updateWorldInfo = false, List<InstancingAlpha> alphaInfo = null, bool updateAlphaInfo = false)
        {
            if (updateAlphaInfo && !updateWorldInfo)
                CreateAdditionalInformation(alphaInfo);
            else if (updateAlphaInfo && updateWorldInfo)
                CreateAdditionalInformation(worldInfo, alphaInfo);
            else if (!updateAlphaInfo && updateWorldInfo)
                CreateAdditionalInformation(worldInfo);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="gameTime"></param>
        /// <param name="view"></param>
        /// <param name="projection"></param>
        public void Draw(GameTime gameTime, Matrix view, Matrix projection)
        {
            effect.CurrentTechnique = effect.Techniques["Instancing"];
            effect.Parameters["ViewProjection"].SetValue(view * projection);
            effect.Parameters["Texture"].SetValue(texture);
            device.Indices = indexBuffer;

            effect.CurrentTechnique.Passes[0].Apply();

            device.SetVertexBuffers(vertexBindings);
            device.DrawInstancedPrimitives(PrimitiveType.TriangleList, 0, 0, vertices.Length, 0, 2, objectCount);
        }
    }
}
