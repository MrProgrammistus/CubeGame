using OpenTK.Mathematics;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace CubeGame.Scripts
{
    internal class Camera
    {
        public Matrix4 projection;
        Vector3 up = (0, 1, 0);

        Vector2 lastPos;
        float pitch, yaw;
        float sensitivity = 0.1f;
        bool firstMove = true;

        public Camera(int width, int height)
        {
            Resize((float)width / height);
        }

        public void Move(GameWindow gameWindow, float deltaTime, ref Vector3 velocity, ref Vector3 rotation, float speed)
        {
            KeyboardState input = gameWindow.KeyboardState;
            gameWindow.CursorState = OpenTK.Windowing.Common.CursorState.Grabbed;

            Vector2 mouse = gameWindow.MousePosition;

            if (firstMove)
            {
                lastPos = (mouse.X, mouse.Y);
                firstMove = false;
            }
            else
            {
                float deltaX = mouse.X - lastPos.X;
                float deltaY = mouse.Y - lastPos.Y;
                lastPos = (mouse.X, mouse.Y);
                yaw += deltaX * sensitivity;
                pitch -= deltaY * sensitivity;
                if (pitch > 80) pitch = 80;
                else if (pitch < -80) pitch = -80;
            }

            rotation.X = MathF.Cos(MathHelper.DegreesToRadians(pitch)) * MathF.Cos(MathHelper.DegreesToRadians(yaw));
            rotation.Y = MathF.Sin(MathHelper.DegreesToRadians(pitch));
            rotation.Z = MathF.Cos(MathHelper.DegreesToRadians(pitch)) * MathF.Sin(MathHelper.DegreesToRadians(yaw));
            rotation = Vector3.Normalize(rotation);

            Vector3 delta = new();

            if (input.IsKeyDown(Keys.W)) delta += Vector3.Normalize((rotation.X, 0, rotation.Z));
            if (input.IsKeyDown(Keys.S)) delta -= Vector3.Normalize((rotation.X, 0, rotation.Z));
            if (input.IsKeyDown(Keys.A)) delta -= Vector3.Normalize(Vector3.Cross(rotation, up));
            if (input.IsKeyDown(Keys.D)) delta += Vector3.Normalize(Vector3.Cross(rotation, up));
            if (input.IsKeyDown(Keys.Space)) delta += up;
			if (input.IsKeyDown(Keys.LeftShift)) delta -= up;
            if (input.IsKeyDown(Keys.LeftControl)) speed *= 5;

			if(delta != Vector3.Zero) delta.Normalize();
            delta *= deltaTime;

			velocity = (velocity * 9 + delta * speed) / 10;
        }

        public void Resize(float aspect)
        {
            projection = Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(90), aspect, 0.01f, 1000f);
        }
    }
}
