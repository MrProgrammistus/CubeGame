using OpenTK.Mathematics;

namespace WorldReader
{
	internal class Program
	{
		public virtual void B()
		{
			Console.WriteLine("err");
		}

		List<Program> list = [];

		public Program(Program p) => list.Add(p);

		static void Main()
		{

		}
	}
}
