using OpenTK.Mathematics;

namespace PhysSim
{
	internal class CollisonEPA
	{
		public static Vector3 CustomEPA(Geometric g1, Geometric g2)
		{
			Vector3 delta = Vector3.Normalize(g2.pos - g1.pos);

			Vector3 normal = default;
			float scalar = float.MinValue;
            for (int i = 0; i < g1.normals.Length; i++)
            {
                if(scalar < Vector3.Dot(delta, g1.normals[i]))
				{
					scalar = Vector3.Dot(delta, g1.normals[i]);
					normal = g1.normals[i];
				}
            }
			for (int i = 0; i < g2.normals.Length; i++)
            {
                if(scalar < Vector3.Dot(delta, g2.normals[i]))
				{
					scalar = Vector3.Dot(delta, g2.normals[i]);
					normal = g2.normals[i];
				}
            }

            return normal;
		}
	}
}
