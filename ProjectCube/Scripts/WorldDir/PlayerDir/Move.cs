using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace ProjectCube.Scripts.WorldDir.PlayerDir
{
	internal class Move(Window window) : Configs(window)
	{
		Vector2 lastPos;
		float pitch, yaw;
		bool firstMove = true;

		public void Update(ref Vector3 velocity, ref Vector2 angleRotation, out Vector3 rotation, Vector3 up , Window window, float time)
		{
			KeyboardState input = window.gameWindow.KeyboardState;
			window.gameWindow.CursorState = OpenTK.Windowing.Common.CursorState.Grabbed;

			Vector2 mouse = window.gameWindow.MousePosition;

			if (firstMove)
			{
				lastPos = (mouse.X, mouse.Y);
				firstMove = false;
				if(angleRotation == (0, 0))
				{
					yaw = firstYaw;
					pitch = firstPitch;
				}
				else
				{
					(yaw, pitch) = angleRotation;
				}
			}
			else
			{
				float deltaX = mouse.X - lastPos.X;
				float deltaY = mouse.Y - lastPos.Y;
				lastPos = (mouse.X, mouse.Y);
				yaw += deltaX * sensitivity;
				pitch -= deltaY * sensitivity;
				if (pitch > maxPitch) pitch = maxPitch;
				else if (pitch < minPitch) pitch = minPitch;
			}
			angleRotation = (yaw, pitch);

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
			float speed = Configs.speed;
			if (input.IsKeyDown(Keys.LeftControl)) speed *= ShiftSpeedMultiply;

			if (delta != Vector3.Zero) delta.Normalize();

			delta *= speed;

			velocity = (velocity * velocityK + delta * time) / (velocityK + time);
		}
	}
}
