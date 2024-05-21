using OpenTK.Mathematics;
using ProjectCube.Scripts.WorldDir.TerrainDir;

namespace ProjectCube.Scripts.WorldDir.PlanetsDir
{
	internal class GenSphereTerrain
	{
		Noise noise = new(0);

		public int n = 3;

		public void Gen(ref (Vector3, Vector3, Vector3)[] triangles)
		{
			for (int i = 0; i < triangles.Length; i++)
			{
				SphereVector sphereVector1 = SphereVector.ToSphere(triangles[i].Item1);
				SphereVector sphereVector2 = SphereVector.ToSphere(triangles[i].Item2);
				SphereVector sphereVector3 = SphereVector.ToSphere(triangles[i].Item3);

				triangles[i].Item1 *= 1 + noise.SphereHeightMap(sphereVector1.GetVector2()) * 0.1f;
				triangles[i].Item2 *= 1 + noise.SphereHeightMap(sphereVector2.GetVector2()) * 0.1f;
				triangles[i].Item3 *= 1 + noise.SphereHeightMap(sphereVector3.GetVector2()) * 0.1f;
			}
		}

		public void GenLines(out Lines lines, (Vector3, Vector3, Vector3) pos, int n, float radius)
		{
			Line[] linesList1 = [];
			List<Line> linesList2 = [new(pos.Item1, pos.Item2, pos.Item3)];

			lines = new();
			lines.posXYZ = ((pos.Item1 + pos.Item2 + pos.Item3) / 3).Normalized() * radius;
			lines.posOF = SphereVector.ToSphere(lines.posXYZ).GetVector2();

			for (int m = 0; m < n; m++)
			{
				linesList1 = [.. linesList2.ToArray()];
				linesList2.Clear();

				foreach (Line line in linesList1)
				{
					Vector3 i1 = line.posXYZ1;
					Vector3 i2 = line.posXYZ2;
					Vector3 i3 = line.posXYZ3;

					Vector3 i12 = (i1 + i2) / 2;
					Vector3 i23 = (i2 + i3) / 2;
					Vector3 i31 = (i3 + i1) / 2;

					i12 = i12.Normalized() * (radius + noise.SphereHeightMap(SphereVector.ToSphere(i12).GetVector2()) * radius * 0.1f);
					i23 = i23.Normalized() * (radius + noise.SphereHeightMap(SphereVector.ToSphere(i23).GetVector2()) * radius * 0.1f);
					i31 = i31.Normalized() * (radius + noise.SphereHeightMap(SphereVector.ToSphere(i31).GetVector2()) * radius * 0.1f);

					linesList2.Add(new(i1, i12, i31));
					linesList2.Add(new(i12, i2, i23));
					linesList2.Add(new(i23, i3, i31));
					linesList2.Add(new(i12, i23, i31));
				}
			}

			lines.lines = [.. linesList2.ToArray()];
		}
	}
}
