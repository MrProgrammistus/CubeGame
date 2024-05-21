using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;
using ProjectCube.Scripts.WorldDir.PlanetsDir;

namespace ProjectCube.Scripts.WorldDir.PlayerDir
{
	internal class SphereMove(Window window) : Configs(window)
	{
		Vector2 lastPos;
		float pitch, yaw;
		bool firstMove = true;

		public void Update(ref Vector3 velocity, ref Vector2 angleRotation, ref Vector3 rotation, Matrix4 matrix, Vector3 up, Window window, float time)
		{
			KeyboardState input = window.gameWindow.KeyboardState;
			window.gameWindow.CursorState = OpenTK.Windowing.Common.CursorState.Grabbed;

			Vector2 mouse = window.gameWindow.MousePosition;

			if (firstMove)
			{
				lastPos = (mouse.X, mouse.Y);
				firstMove = false;
				if (angleRotation == (0, 0))
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

			float pitch2 = pitch;
			float yaw2 = yaw;

			pitch2 = MathHelper.DegreesToRadians(pitch2);
			yaw2 = MathHelper.DegreesToRadians(yaw2);

			rotation.X = MathF.Cos(pitch2)
					   * MathF.Cos(yaw2);
			rotation.Y = MathF.Sin(pitch2);
			rotation.Z = MathF.Cos(pitch2)
					   * MathF.Sin(yaw2);


			Vector3 delta = new();

			if (input.IsKeyDown(Keys.W)) delta += Vector3.Normalize(rotation - Planets.Projection(rotation ,up));
			if (input.IsKeyDown(Keys.S)) delta -= Vector3.Normalize(rotation - Planets.Projection(rotation, up));
			if (input.IsKeyDown(Keys.A)) delta -= Vector3.Normalize(Vector3.Cross(rotation - Planets.Projection(rotation, up), up));
			if (input.IsKeyDown(Keys.D)) delta += Vector3.Normalize(Vector3.Cross(rotation - Planets.Projection(rotation, up), up));
			if (input.IsKeyDown(Keys.Space)) delta += up;
			if (input.IsKeyDown(Keys.LeftShift)) delta -= up;

			if (!input.IsKeyDown(Keys.Space) && !input.IsKeyDown(Keys.LeftShift)) delta.Y = 0;

			float speed = Configs.speed;
			if (input.IsKeyDown(Keys.LeftControl)) speed *= ShiftSpeedMultiply;

			Matrix4 result = matrix; // * window.scene.Find("Planet0").GetMatrix()

			Vector4 rotation4 = new(rotation.X, rotation.Y, rotation.Z, 1);
			rotation = (rotation4 * result).Xyz;
			rotation.Normalize();

			Vector4 delta4 = new(delta.X, delta.Y, delta.Z, 1);
			delta = (delta4 * result).Xyz;
			if (delta != Vector3.Zero) delta.Normalize();

			delta *= speed;

			velocity = (velocity * velocityK + delta * time) / (velocityK + time);
		}
	}
}
