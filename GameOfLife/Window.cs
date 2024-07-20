using OpenTK.Graphics.OpenGL4;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace GameOfLife
{
    internal class Window : GameWindow
    {
        private Game game;
        private Shader shader;
        private Matrix4 projection;

        public Window(int width, int height, string title)
            : base(GameWindowSettings.Default, new NativeWindowSettings() { ClientSize = (width, height), Title = title })
        {
        }

        protected override void OnLoad()
        {
            base.OnLoad();
            game = new(5);

            GL.ClearColor(0.1f, 0.1f, 0.12f, 1.0f);
            GL.Enable(EnableCap.DepthTest);

            // Setup shaders
            shader = new Shader("Shaders/shader.vert", "Shaders/shader.frag");


            float margin = 0.2f;
            projection = Matrix4.CreateOrthographicOffCenter(-1.0f - margin, 1.0f + margin, -1.0f - margin, 1.0f + margin, -1.0f, 1.0f);
        }

        protected override void OnRenderFrame(FrameEventArgs args)
        {
            base.OnRenderFrame(args);

            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            shader.Bind();
            shader.SetMatrix4("projection", projection);
            shader.SetColor(new Vector4(1.0f, 1.0f, 1.0f, 1.0f));

            game.DrawGrid();
            //game.DrawCells();

            SwapBuffers();
        }

        protected override void OnMouseUp(MouseButtonEventArgs e)
        {
            base.OnMouseUp(e);

            if(e.Button == MouseButton.Left)
            {
                var worldPos = ScreenToWorld(MousePosition.X, MousePosition.Y);
                game.OnClick(worldPos);
            }
        }

        protected override void OnResize(ResizeEventArgs e)
        {
            base.OnResize(e);
            GL.Viewport(0, 0, ClientSize.X, ClientSize.Y);  // Set the OpenGL viewport to cover the entire window

            float aspectRatio = ClientSize.X / (float)ClientSize.Y;
            float buffer = 0.05f;  // Add a small buffer to compensate

            float left, right, bottom, top;

            if (aspectRatio > 1.0)
            {
                // Window is wider than it is tall
                left = -aspectRatio - buffer;   // Stretch the left and right to fit the aspect ratio
                right = aspectRatio + buffer;
                bottom = -1.0f - buffer;        // Add buffer
                top = 1.0f + buffer;
            }
            else
            {
                // Window is taller than it is wide
                left = -1.0f - buffer;          // Left and right remain the same
                right = 1.0f + buffer;
                bottom = -1.0f / aspectRatio - buffer; // Stretch the bottom and top to fit the aspect ratio
                top = 1.0f / aspectRatio + buffer;
            }

            // Create an orthographic projection matrix that fits the grid as large as possible within the window
            projection = Matrix4.CreateOrthographicOffCenter(left, right, bottom, top, -1.0f, 1.0f);
        }

        protected override void OnUnload()
        {
            base.OnUnload();
            game.Unload();
            shader.Dispose();
        }

        protected override void OnUpdateFrame(FrameEventArgs args)
        {
            if (!IsFocused) return;

            base.OnUpdateFrame(args);

            // Comment out movement code for now
            /*
            var input = KeyboardState;
            if (input.IsKeyDown(Keys.Escape))
            {
                Close();
            }
            */
        }

        // Placeholder for camera movement code
        protected override void OnMouseMove(MouseMoveEventArgs e)
        {
            base.OnMouseMove(e);
        }

        private Vector2 ScreenToWorld(float x, float y)
        {
            // Get the size of the window
            float windowWidth = ClientSize.X;
            float windowHeight = ClientSize.Y;

            // Convert screen coordinates to normalized device coordinates (NDC)
            float ndcX = (2.0f * x) / windowWidth - 1.0f;
            float ndcY = 1.0f - (2.0f * y) / windowHeight;

            // Create a vector in normalized device coordinates
            Vector4 ndcPos = new Vector4(ndcX, ndcY, 0.0f, 1.0f);

            // Calculate the inverse of the projection matrix
            Matrix4 invProjection = Matrix4.Invert(projection);

            // Manually transform the NDC to world coordinates using the inverse projection matrix
            Vector4 worldPos = invProjection * ndcPos;

            // Return the world coordinates as a Vector2
            return new Vector2(worldPos.X, worldPos.Y);
        }
    }


}