using OpenTK.Mathematics;

namespace ProjectCube.Scripts.WorldDir.PlayerDir
{
    internal class Player : PlayerConfig
    {
		public Vector3 position;
		public Vector3 velocity;
		public Vector3 rotation;

		public Matrix4 view;
		public Matrix4 projection;

		Move move = new();

		public void Update(Window window, double time)
		{
			move.Update(ref velocity, ref rotation, window, time);
			position += velocity;


			view = Matrix4.LookAt(position, position + rotation, (0, 1, 0)) * projection;
		}
		

		public void Resize()
		{
			projection = Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(90), (float)WindowConfigs.width / WindowConfigs.height, 0.01f, 1000f);
		}
	}
}
