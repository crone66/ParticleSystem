using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ParticleSystem
{
    public class ParticleEffect
    {
        public static int MaxCount = 250;
        public static int FreeIndex = 0;
        public static int ActivNoven = 0;

        public delegate List<Particle> ParticleEmitter(Vector3 spawnPoint, ParticleSettings settings);
        private ParticleEmitter emitter;
        private ParticleSettings particleSettings;

        private bool instancingIsDirty;
        private bool visable;
        private bool activ;
        private float duration;
        private float elapsedTime;

        private Quad quad;
        private Instancing instancing;
        private List<Particle> particles;
        private List<InstancingAlpha> instancingAlphaInfo;
        private List<InstancingWorldMatrix> instancingWorldInfo;
        private Random random = new Random();
        private GraphicsDevice device;

        private Vector3 centerPoint;
        private BoundingSphere sphere;

        private int maxDistanceIndex = -1;
        private bool useOptimizer;
        private bool calculateRotationForeachParticle;
        private bool useCollision;

        public bool Activ
        {
            get { return activ; }
            set { activ = value; }
        }
        
        public bool Visable
        {
            get { return visable; }
            set
            {
                if (value && !visable)
                    instancingIsDirty = true;
                visable = value;
            }
        }
      
        /// <summary>
        /// Initzializes a new particle effect
        /// </summary>
        /// <param name="device">Graphics device<param>
        /// <param name="emitter">Particle effect emitter</param>
        /// <param name="particleSettings">Particle effect settings</param>
        /// <param name="useOptimizer">Indicates whenther particles rotation matrix should be update or not</param>
        /// <param name="useCollision">Indicates whenther the particle effect should have a boundingSphere (Can be used for damage caluclations or what ever you need)</param>
        /// <param name="calculateRotationForeachParticle">Indicates whenther particles should share the same rotation matrix or not</param>
        public ParticleEffect(GraphicsDevice device, ParticleEmitter emitter, ParticleSettings particleSettings, bool useOptimizer, bool useCollision = false, bool calculateRotationForeachParticle = false)
        {
            this.useCollision = useCollision;
            this.calculateRotationForeachParticle = calculateRotationForeachParticle;
            this.useOptimizer = useOptimizer;
            this.device = device;
            this.particleSettings = particleSettings;
            this.emitter = emitter;
            duration = particleSettings.MaxDuration;
            particles = new List<Particle>();
            instancingAlphaInfo = new List<InstancingAlpha>();
            instancingWorldInfo = new List<InstancingWorldMatrix>();
            quad = new Quad(particleSettings.Size, particleSettings.Size);
        }

        /// <summary>
        /// Changes particle effect
        /// </summary>
        /// <param name="emitter">Emitter delegate</param>
        /// <param name="particleSettings">Particle effect settings</param>
        /// <param name="useCollision">Indicates whenther the particle effect should have a boundingSphere</param>
        public void ChangeEffect(ParticleEmitter emitter, ParticleSettings particleSettings, bool useCollision)
        {
            if (!activ)
            {
                this.useCollision = useCollision;
                this.particleSettings = particleSettings;
                this.emitter = emitter;
                visable = false;
                duration = particleSettings.MaxDuration;
                quad = new Quad(particleSettings.Size, particleSettings.Size);
            }
            else
                throw new NotImplementedException();
        }

        /// <summary>
        /// Generates particles
        /// </summary>
        /// <param name="spawnPoint">Spawn position (center of particle effect)</param>
        private void GenerateParticles(Vector3 spawnPoint)
        {
            centerPoint = spawnPoint;
            sphere = new BoundingSphere(centerPoint, 1);
            float maxDistance = float.MinValue;

            if (emitter != null)
            {
                particles = emitter(spawnPoint, particleSettings);
                for (int i = 0; i < particles.Count; i++)
                {
                    instancingAlphaInfo.Add(particles[i].Info);
                    instancingWorldInfo.Add(particles[i].WorldInfo);

                    float distance = Vector3.Distance(spawnPoint, particles[i].Position);
                    if (distance > maxDistance)
                    {
                        maxDistance = distance;
                        maxDistanceIndex = i;
                    }
                }

                instancing = new Instancing(device, particleSettings.Effect, particleSettings.Texture, quad.Vertices, quad.Indices, particleSettings.ParticleCount, instancingWorldInfo, instancingAlphaInfo);
                sphere = new BoundingSphere(spawnPoint, maxDistance + 0.1f);
            }
        }

        /// <summary>
        /// Activates particle effect
        /// </summary>
        /// <param name="spawnPoint">Spawn position</param>
        public void Activate(Vector3 spawnPoint)
        {
            if (!activ)
            {
                if (useCollision)
                    ActivNoven++;
                FreeIndex++;
                if (FreeIndex >= MaxCount)
                    FreeIndex = 0;

                GenerateParticles(spawnPoint);
                activ = true;
                visable = true;
            }
        }

        /// <summary>
        /// Deativates the particle effect and clears all buffered information
        /// </summary>
        public void Deactivate()
        {
            if (useCollision)
                ActivNoven--;

            instancingAlphaInfo.Clear();
            instancingWorldInfo.Clear();
            particles.Clear();
            instancing = null;
            elapsedTime = 0f;
            activ = false;
            visable = false;

            instancingIsDirty = false;
        }

        /// <summary>
        /// Updates particle effect and all including particles
        /// </summary>
        /// <param name="gameTime">Game time</param>
        /// <param name="cameraPosition">Current camera position</param>
        /// <param name="Position">Particle spawn position</param>
        public void Update(GameTime gameTime, Vector3 cameraPosition, Vector3? Position)
        {
            if (activ)
            {
                elapsedTime += (float)gameTime.ElapsedGameTime.TotalMilliseconds;
                if (duration > 0 && elapsedTime >= duration)
                {
                    Deactivate();
                }
                else
                {
                    if (visable || !visable && !useOptimizer)
                    {
                        Matrix rot = Matrix.Identity;
                        for (int i = 0; i < particles.Count; i++)
                        {
                            if (Position.HasValue)
                                particles[i].StartPosition = Position.Value;

                            particles[i].Update(gameTime, cameraPosition, calculateRotationForeachParticle || (i == 0 && !calculateRotationForeachParticle));
                            if (!calculateRotationForeachParticle)
                            {
                                if (i == 0)
                                    rot = particles[i].RotationMatrix;
                                else
                                    particles[i].RotationMatrix = rot;
                            }

                            if (visable)
                            {
                                instancingAlphaInfo[i] = particles[i].Info;
                                instancingWorldInfo[i] = particles[i].WorldInfo;
                            }
                        }

                        float distance = Vector3.Distance(centerPoint, particles[maxDistanceIndex].Position);
                        if (distance > 0)
                            sphere = new BoundingSphere(centerPoint, distance + 0.1f);
                        else
                            sphere = new BoundingSphere(centerPoint, sphere.Radius);

                        if (visable)
                        {
                            instancing.Update(gameTime, instancingWorldInfo, true, instancingAlphaInfo, true);
                            instancingIsDirty = false;
                        }
                    }
                    else
                    {
                        particles[maxDistanceIndex].Update(gameTime, cameraPosition);

                        float distance = Vector3.Distance(centerPoint, particles[maxDistanceIndex].Position);
                        if (distance > 0)
                            sphere = new BoundingSphere(centerPoint, distance + 0.1f);
                        else
                            sphere = new BoundingSphere(centerPoint, sphere.Radius);
                    }
                }
            }
        }

        /// <summary>
        /// Draws all particles with hardware instancing
        /// </summary>
        /// <param name="gameTime">Game time</param>
        /// <param name="view">View matrix</param>
        /// <param name="projection">Projection matrix</param>
        public void Draw(GameTime gameTime, Matrix view, Matrix projection)
        {
            if (activ && visable && !instancingIsDirty)
            {
                instancing.Draw(gameTime, view, projection);
            }
        }

        /// <summary>
        /// Calculates rotation matrix to face particle to the camera
        /// </summary>
        /// <param name="cameraPosition">Current camera posiion</param>
        /// <param name="target">Particle position</param>
        /// <returns>Returns a rotation matrix</returns>
        public static Matrix FaceToCamera(Vector3 cameraPosition, Vector3 target)
        {
            Vector3 dest = Vector3.Normalize(cameraPosition - target);
            float dot = Vector3.Dot(Vector3.Forward, dest);

            if (Math.Abs(dot - (-1.0f)) < 0.000001f)
                return Matrix.CreateFromQuaternion(new Quaternion(Vector3.Up, MathHelper.ToRadians(180.0f)));
            if (Math.Abs(dot - (1.0f)) < 0.000001f)
                return Matrix.Identity;

            Vector3 rotAxis = Vector3.Normalize(Vector3.Cross(Vector3.Forward, dest));
            return Matrix.CreateFromAxisAngle(rotAxis, (float)Math.Acos(dot));
        }
    }
}
