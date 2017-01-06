using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace ParticleSystemExample
{
    public class SimpleCamera
    {
        private float distance = 400f;
        private float cameraArc = 0;
        private Matrix view, projection;
        private Vector3 position;

        public Matrix Projection
        {
            get { return projection; }
            set { projection = value; }
        }

        public Matrix View
        {
            get { return view; }
            set { view = value; }
        }
        
        public Vector3 Position
        {
            get { return position; }
            set { position = value; }
        }

        public SimpleCamera(GraphicsDevice device, Vector3 position, Vector3 lookAt)
        {
            this.position = position;
            view = Matrix.CreateLookAt(position, lookAt, Vector3.Up);
            projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(45f),
                                                       device.Viewport.AspectRatio,
                                                        1f,
                                                        10000);
        }

        public void Update(GameTime gameTime, Vector3 lookAt)
        {
            KeyboardState currentKeyboardState = Keyboard.GetState();
            float time = (float)gameTime.ElapsedGameTime.TotalMilliseconds;
            float newArc = 0f, newRot = 0f;
            if (currentKeyboardState.IsKeyDown(Keys.W))
            {
                newArc += time * 0.0025f;
            }
            if (currentKeyboardState.IsKeyDown(Keys.S))
            {
                newArc -= time * 0.0025f;
            }

            // Limit the arc movement.
            if (cameraArc > 90.0f)
                cameraArc = 90.0f;
            else if (cameraArc < -90.0f)
                cameraArc = -90.0f;

            // Check for input to rotate the camera around the model.
            if (currentKeyboardState.IsKeyDown(Keys.D))
            {
                newRot += time * 0.005f;
            }
            if (currentKeyboardState.IsKeyDown(Keys.A))
            {
                newRot -= time * 0.005f;
            }

            position = Vector3.Transform(position - Vector3.Zero, Matrix.CreateRotationY(newRot) * Matrix.CreateRotationX(newArc));
            view = Matrix.CreateLookAt(position, lookAt, Vector3.Up);
        }
    }
}
