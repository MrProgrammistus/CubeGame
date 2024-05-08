using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace ProjectCube.Scripts.WorldDir.PlayerDir
{
	internal class Move : PlayerConfig
	{
		Vector2 lastPos;
		float pitch, yaw;
		float sensitivity = 0.1f;
		bool firstMove = true;
		float speed = 2f;

		public void Update(ref Vector3 velocity, ref Vector3 rotation, Window window, double time)
		{
			KeyboardState input = window.gameWindow.KeyboardState;
			window.gameWindow.CursorState = OpenTK.Windowing.Common.CursorState.Grabbed;

			Vector2 mouse = window.gameWindow.MousePosition;

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
			if (input.IsKeyDown(Keys.A)) delta -= Vector3.Normalize(Vector3.Cross(rotation, (0, 1, 0)));
			if (input.IsKeyDown(Keys.D)) delta += Vector3.Normalize(Vector3.Cross(rotation, (0, 1, 0)));
			if (input.IsKeyDown(Keys.Space)) delta += (0, 1, 0);
			if (input.IsKeyDown(Keys.LeftShift)) delta -= (0, 1, 0);
			float speed = this.speed;
			if (input.IsKeyDown(Keys.LeftControl)) speed *= 5;

			if (delta != Vector3.Zero) delta.Normalize();

			delta *= (float)time * speed;

			velocity = (velocity * 9 + delta * 1) / 10;
		}
	}
}
