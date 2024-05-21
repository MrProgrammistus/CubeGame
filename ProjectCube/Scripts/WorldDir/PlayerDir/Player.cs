using OpenTK.Mathematics;
using ProjectCube.Scripts.WorldDir.PlanetsDir;
using ProjectCube.Scripts.WorldDir.SceneDir;

namespace ProjectCube.Scripts.WorldDir.PlayerDir
{
    internal class Player(Window window) : InterBase
	{
		public Vector3 position = (-350, 0, 0);
		public Vector3 lastPosition;
		public Vector3 velocity;
		public Vector3 rotation = (1, 0, 0);
		public Vector2 angleRotation;

		public Matrix4 view;
		public Matrix4 projection;

		public Vector3 up;

		//Move move = new(window);
		SphereMove sphereMove = new(window);

		public override void Update(float time)
		{
			Matrix4 matrix = Matrix4.Identity;

			Vector3 centerOfMass = FindCenterOfMass();

			SphereVector vector = SphereVector.ToSphere(position);
			matrix *= Matrix4.CreateRotationZ(MathHelper.DegreesToRadians(-vector.O));
			matrix *= Matrix4.CreateRotationY(MathHelper.DegreesToRadians(-vector.F));

			up = position;


			//move.Update(ref velocity, ref angleRotation, out rotation, (0, 1, 0), window, time);
			sphereMove.Update(ref velocity, ref angleRotation, ref rotation, matrix, (0, 1, 0), window, time);
			position += velocity * time;
			lastPosition = position;

			//view = Matrix4.LookAt(position, position + rotation, (0, 1, 0)) * projection;
			view = Matrix4.LookAt(position, position + rotation, up) * projection;
		}
		
		public Vector3 FindCenterOfMass()
		{
			Vector3 centerOfMass = (0, 0, 0);
			float totalMass = 0;
			List<(Vector3, float)> masses = [];
			foreach (var i in window.scene.gameObjects)
			{
				if (i.GetElement<Planet>() != null)
				{
					masses.Add((i.position, i.GetElement<Planet>().mass));
				}
			}
			foreach (var i in masses)
			{
				totalMass += i.Item2;
				centerOfMass += i.Item1 * i.Item2;
			}
			centerOfMass /= totalMass;

			return centerOfMass;
		}

		public override void Resize()
		{
			projection = Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(Configs.fov), (float)Configs.width / Configs.height, Configs.depthNear, Configs.depthFar);
		}
	}
}
