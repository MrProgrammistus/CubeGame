using OpenTK.Mathematics;

namespace ProjectCube.Scripts.WorldDir.PlanetsDir
{
	internal class Lines
	{
		public Line[] lines = [];

		public int flags;

		public Vector2 posOF;
		public Vector3 posXYZ;
	}

	struct Line(Vector3 p1, Vector3 p2, Vector3 p3)
	{
		public Vector3 posXYZ1 = p1;
		public Vector3 posXYZ2 = p2;
		public Vector3 posXYZ3 = p3;

		public float y1;
		public float y2;
		public float y3;
	}
}
