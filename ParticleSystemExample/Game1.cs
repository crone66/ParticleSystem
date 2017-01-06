using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using ParticleSystem;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace ParticleSystemExample
{
    public class Game1 : Game
    {
        private static Random random = new Random();

        private GraphicsDeviceManager graphics;
        private SpriteBatch spriteBatch;

        private SimpleCamera camera;

        private int frameRate;
        private int frameCounter;
        private TimeSpan elapsedTime;

        private List<Texture2D> effectTextures;
        private Effect effect;
        private SpriteFont spriteFont;

        private Model warpGate;

        private Model currentModel;
        private int currentTextureIndex;
        private Matrix[] transforms;

        private DepthStencilState particleStates;

        private float startDistance;
        private float scale = 3f;
        private float zoomOut = 1820f;
        private float spreadMin = -2f;
        private float spreadMax = 2f;
        private List<ParticleEffect> ps;

        private bool showHelp;
        private KeyboardState lastKeyboardState;
        private delegate void LastSpawnMethode();
        private LastSpawnMethode spawner;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            IsFixedTimeStep = false;

            graphics.SynchronizeWithVerticalRetrace = false;

            //Change window resolution
            graphics.PreferredBackBufferHeight = 768;
            graphics.PreferredBackBufferWidth = 1024;
            //graphics.IsFullScreen = true;
            graphics.ApplyChanges();
        }

        protected override void Initialize()
        {
            base.Initialize();
        }
        
        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);

            showHelp = true;
            camera = new SimpleCamera(GraphicsDevice, new Vector3(0, 0f, zoomOut), Vector3.Zero);
            particleStates = new DepthStencilState() { DepthBufferWriteEnable = false };

            warpGate = Content.Load<Model>("WarpGate/Jumpgate1-3");
            
            startDistance = warpGate.Meshes["base"].BoundingSphere.Radius * scale;

            effectTextures = new List<Texture2D>();
            //TODO: should be replaced by one texture and Color parameter in particle effect
            effectTextures.Add(Content.Load<Texture2D>("nova_gruen"));
            effectTextures.Add(Content.Load<Texture2D>("nova_blau"));
            effectTextures.Add(Content.Load<Texture2D>("nova_rosa"));
            effectTextures.Add(Content.Load<Texture2D>("nova_rot"));
            effectTextures.Add(Content.Load<Texture2D>("nova_gelb"));
            effectTextures.Add(Content.Load<Texture2D>("nova_rosa"));
            effectTextures.Add(Content.Load<Texture2D>("nova_lila"));

            currentTextureIndex = 0;

            spriteFont = Content.Load<SpriteFont>("SpriteFont1");
            effect = Content.Load<Effect>("InstancingShader");

            ps = new List<ParticleEffect>();
        }
        
        private float Rand()
        {
            return MathHelper.Lerp(spreadMin, spreadMax, (float)random.NextDouble());
        }

        protected override void UnloadContent()
        {
        }
        
        protected override void Update(GameTime gameTime)
        {
            camera.Update(gameTime, Vector3.Zero);

            for (int i = 0; i < ps.Count; i++)
            {
                if (ps[i].Activ)
                    ps[i].Update(gameTime, camera.Position, null);
                else
                    ps[i].Activate(Vector3.Zero);
            }
            
            HandleInput();
            CalculateFrameRate(gameTime);
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);
            GraphicsDevice.DepthStencilState = DepthStencilState.Default;
            GraphicsDevice.BlendState = BlendState.Opaque;
            GraphicsDevice.SamplerStates[0] = SamplerState.LinearClamp;
            if (currentModel != null)
            {
                foreach (ModelMesh item in currentModel.Meshes)
                {
                    foreach (BasicEffect effect in item.Effects)
                    {
                        effect.World = transforms[item.ParentBone.Index] * Matrix.CreateScale(scale) * Matrix.CreateRotationY(MathHelper.ToRadians(90f)) * Matrix.CreateTranslation(Vector3.Zero);
                        effect.Projection = camera.Projection;
                        effect.View = camera.View;
                        effect.EnableDefaultLighting();
                    }
                    item.Draw();
                }
            }

            GraphicsDevice.DepthStencilState = particleStates;
            GraphicsDevice.BlendState = BlendState.Additive;

            for (int i = 0; i < ps.Count; i++)
            {
                ps[i].Draw(gameTime, camera.View, camera.Projection);
            }

            DrawOverlayText();
            frameCounter++;

            base.Draw(gameTime);
        }

        private void DrawOverlayText()
        {
            string text = string.Format(CultureInfo.CurrentCulture,
                                        "Frames per second: {0}",
                                        frameRate);


            spriteBatch.Begin();
            spriteBatch.DrawString(spriteFont, text, new Vector2(35, 35), Color.Black);
            spriteBatch.DrawString(spriteFont, text, new Vector2(34, 34), Color.White);
            if (showHelp)
            {
                spriteBatch.DrawString(spriteFont, "Up/Down: change effect color", new Vector2(35, 60), Color.White);
                spriteBatch.DrawString(spriteFont, "N = Nova effect / G = Warp-Gate effect / B = Blackhole effect / F= Starfield effect", new Vector2(35, 90), Color.White);
                spriteBatch.DrawString(spriteFont, "R: Restart effect", new Vector2(35, 120), Color.White);
                spriteBatch.DrawString(spriteFont, "W,A,S,D: Camera rotation", new Vector2(35, 150), Color.White);
                spriteBatch.DrawString(spriteFont, "H: Show/Hide control information", new Vector2(35, 180), Color.White);
            }
            spriteBatch.End();
        }

        private void CalculateFrameRate(GameTime gameTime)
        {
            elapsedTime += gameTime.ElapsedGameTime;
            if (elapsedTime > TimeSpan.FromSeconds(1))
            {
                elapsedTime -= TimeSpan.FromSeconds(1);
                frameRate = frameCounter;
                frameCounter = 0;
            }
        }

        private void HandleInput()
        {
            KeyboardState state = Keyboard.GetState();
            if (state.IsKeyDown(Keys.Escape))
                Exit();
            else
            {
                if (state.IsKeyDown(Keys.R) && lastKeyboardState.IsKeyUp(Keys.R))
                {
                    for (int i = 0; i < ps.Count; i++)
                    {
                        ps[i].Deactivate();
                        ps[i].Activate(Vector3.Zero);
                    }
                }
                else if (state.IsKeyDown(Keys.G) && lastKeyboardState.IsKeyUp(Keys.G))
                {
                    SpawnGateEffect();
                }
                else if (state.IsKeyDown(Keys.N) && lastKeyboardState.IsKeyUp(Keys.N))
                {
                    SpawnNova();
                }
                else if (state.IsKeyDown(Keys.Up) && lastKeyboardState.IsKeyUp(Keys.Up))
                {
                    CycleUpTexture();
                }
                else if (state.IsKeyDown(Keys.Down) && lastKeyboardState.IsKeyUp(Keys.Down))
                {
                    CycleDownTexture();
                }
                else if (state.IsKeyDown(Keys.B) && lastKeyboardState.IsKeyUp(Keys.B))
                {
                    SpawnHole();
                }
                else if(state.IsKeyDown(Keys.F) && lastKeyboardState.IsKeyUp(Keys.F))
                {
                    SpawnStarfield();
                }
                else if (state.IsKeyDown(Keys.H) && lastKeyboardState.IsKeyUp(Keys.H))
                {
                    showHelp = !showHelp;
                }
            }

            lastKeyboardState = state;
        }

        private void SpawnGateEffect()
        {
            spawner = SpawnGateEffect;

            ps.Clear();

            ChangeCurrentModel(warpGate);

            WarpGateEmitter epic = new WarpGateEmitter();
            for (int i = 0; i < 1; i++)
            {
                ps.Add(new ParticleEffect(GraphicsDevice, epic.CreateSpawn,
                    new ParticleSettings(2500, -1, -1, effect, effectTextures[currentTextureIndex], 0.05f, 0.083f, 1f, scale, startDistance * 0.67f),
                    true, false, true));

                ps[i].Activate(Vector3.Zero);
            }

            WarpGateSparkEmitter sparks = new WarpGateSparkEmitter();
            ps.Add(new ParticleEffect(GraphicsDevice, sparks.CreateSpawn,
                new ParticleSettings(1300, -1, -1, effect, effectTextures[currentTextureIndex], 0.05f, 0.083f, 1f, 50, startDistance, scale, startDistance * 0.25f),
                true, false, true));
        }

        private void SpawnNova()
        {
            currentModel = null;
            spawner = SpawnNova;

            ps.Clear();

            NovaEmitter emitter = new NovaEmitter();
            ps.Add(new ParticleEffect(GraphicsDevice, emitter.CreateSpawn, new ParticleSettings(500, -1, -1, effect, effectTextures[currentTextureIndex], 0.05f, 0.083f, 1f, 50f, 500f), false, false));

            ps[ps.Count - 1].Activate(Vector3.Zero);
        }

        private void SpawnHole()
        {
            currentModel = null;
            spawner = SpawnHole;

            ps.Clear();
            HoleEmitter emitter = new HoleEmitter();
            ps.Add(new ParticleEffect(GraphicsDevice, emitter.CreateSpawn, new ParticleSettings(1000, -1, -1, effect, effectTextures[currentTextureIndex], 0.05f, 0.083f, 1f), false, false));

            ps[ps.Count - 1].Activate(Vector3.Zero);
        }

        private void SpawnStarfield()
        {
            currentModel = null;
            spawner = SpawnStarfield;

            ps.Clear();
            StarfieldEmitter emitter = new StarfieldEmitter();
            ps.Add(new ParticleEffect(GraphicsDevice, emitter.CreateSpawn, new ParticleSettings(1000, -1, -1, effect, effectTextures[currentTextureIndex], 0.05f, 0.083f, 1f, camera.Position, GraphicsDevice.Viewport), false, false, true));

            ps[ps.Count - 1].Activate(Vector3.Zero);
        }

        private void CycleUpTexture()
        {
            currentTextureIndex++;
            if (currentTextureIndex == effectTextures.Count)
                currentTextureIndex = 0;

            spawner?.Invoke();
        }

        private void CycleDownTexture()
        {
            currentTextureIndex--;
            if (currentTextureIndex < 0)
                currentTextureIndex = effectTextures.Count - 1;

            spawner?.Invoke();
        }

        private void ChangeCurrentModel(Model newModel)
        {
            currentModel = newModel;
            transforms = new Matrix[currentModel.Bones.Count];
            currentModel.CopyBoneTransformsTo(transforms);
        }
    }
}
