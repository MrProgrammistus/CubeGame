
namespace ProjectCube.Scripts.TerrainDir
{
	internal class Material
	{
		public Type type;
		public Material(Type type)
		{
			this.type = type;
		}
	}

	enum Type
	{
		none,
		air,
		stone,
	}
}
