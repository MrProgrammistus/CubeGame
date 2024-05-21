using OpenTK.Mathematics;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace PhysSim
{
	internal class Player(GameWindow gameWindow)
	{
		public Vector3 position;
		public Vector3 deltaPos;
		public Vector3 velocity;
		public Vector3 rotation;
		public Vector2 angleRotation;

		public Matrix4 view;
		public Matrix4 projection;

		Move move = new(gameWindow);

		public void Update(float time)
		{
			move.Update(ref velocity, ref angleRotation, out rotation, time);
		}

		public void EndUpdate(float time)
		{
			position += velocity * time;
			deltaPos = velocity * time;
			view = Matrix4.LookAt(position, position + rotation, (0, 1, 0)) * projection;
		}

		public void Resize()
		{
			projection = Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(90), 8f / 6, 0.1f, 100);
		}
	}

	public class Move(GameWindow gameWindow)
	{
		Vector2 lastPos;
		float pitch, yaw;
		bool firstMove = true;

		public void Update(ref Vector3 velocity, ref Vector2 angleRotation, out Vector3 rotation, float time)
		{
			KeyboardState input = gameWindow.KeyboardState;
			gameWindow.CursorState = OpenTK.Windowing.Common.CursorState.Grabbed;

			Vector2 mouse = gameWindow.MousePosition;

			if (firstMove)
			{
				lastPos = (mouse.X, mouse.Y);
				firstMove = false;
				if(angleRotation == (0, 0))
				{

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
				yaw += deltaX * 0.15f;
				pitch -= deltaY * 0.15f;
				if (pitch > 89.9f) pitch = 89.9f;
				else if (pitch < -89.9f) pitch = -89.9f;
			}
			angleRotation = (yaw, pitch);

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
			float speed = 1;
			if (input.IsKeyDown(Keys.LeftControl)) speed *= 5;

			if (delta != Vector3.Zero) delta.Normalize();

			delta *= speed;

			velocity = (velocity * 0.2f + delta * time) / (0.2f + time);
		}
	}
}
