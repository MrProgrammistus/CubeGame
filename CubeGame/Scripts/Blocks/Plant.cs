using OpenTK.Graphics.ES30;
using OpenTK.Mathematics;

namespace CubeGame.Scripts.Blocks
{
	internal class Plant : BaseBS
	{
		public int stage = 0;
		public int h = 0;
		// 0 - цветение бревна
		// 1 - цветение листьев

		public Plant() { }
		public Plant(int stage, int h) { this.stage = stage; this.h = h; }

		Random r = new();
		public override void Update(Vector3 pos, Vector3 ap, BlocksArray ba, Render render)
		{
			if(stage == 0)
			{
			add:
				Vector3i rp = (0, 1, 0);
				int rr = r.Next(20);
				if (rr == 0) rp = (1, 0, 0);
				else if (rr == 1) rp = (-1, 0, 0);
				else if (rr == 2) rp = (0, 0, 1);
				else if (rr == 3) rp = (0, 0, -1);

				int m = 20;

				if (ba.GetBlock((Vector3i)pos + rp, (Vector3i)ap).Type == Type.air)
				{
					Block.SetBlockQueue((Vector3i)pos + rp, (Vector3i)ap, ba, Type.plant, r.Next((m - h) < 0? 0 : (m - h)) == 0? new Plant(1, h) : new Plant(0, h + 1));
				}

				if (r.Next(5) == 0) { goto add; }

				stage = -1;
				Block.SetBlockQueue((Vector3i)pos, (Vector3i)ap, ba, Type.log, null);
			}
			else if (stage == 1)
			{
				int m = 10;
				int a = (m - h) < 0 ? 2 : (m - h);

				if (h > 10) h--;

				if (r.Next(a) > 0 && ba.GetBlock((Vector3i)pos + (1, 0, 0), (Vector3i)ap).Type == Type.air) Block.SetBlockQueue((Vector3i)pos + (1, 0, 0), (Vector3i)ap, ba, r.Next(4) > 0 ? Type.leaves : Type.air, null);
				else if (ba.GetBlock((Vector3i)pos + (1, 0, 0), (Vector3i)ap).Type == Type.air) Block.SetBlockQueue((Vector3i)pos + (1, 0, 0), (Vector3i)ap, ba, Type.plant, new Plant(1, h - 1));
				
				if (r.Next(a) > 0 && ba.GetBlock((Vector3i)pos + (-1, 0, 0), (Vector3i)ap).Type == Type.air) Block.SetBlockQueue((Vector3i)pos + (-1, 0, 0), (Vector3i)ap, ba, r.Next(4) > 0 ? Type.leaves : Type.air, null);
				else if (ba.GetBlock((Vector3i)pos + (-1, 0, 0), (Vector3i)ap).Type == Type.air) Block.SetBlockQueue((Vector3i)pos + (-1, 0, 0), (Vector3i)ap, ba, Type.plant, new Plant(1, h - 1));

				if (r.Next(a) > 0 && ba.GetBlock((Vector3i)pos + (0, 1, 0), (Vector3i)ap).Type == Type.air) Block.SetBlockQueue((Vector3i)pos + (0, 1, 0), (Vector3i)ap, ba, r.Next(4) > 0 ? Type.leaves : Type.air, null);
				else if (ba.GetBlock((Vector3i)pos + (0, 1, 0), (Vector3i)ap).Type == Type.air) Block.SetBlockQueue((Vector3i)pos + (0, 1, 0), (Vector3i)ap, ba, Type.plant, new Plant(1, h - 1));
				
				if (r.Next(a) > 0 && ba.GetBlock((Vector3i)pos + (0, -1, 0), (Vector3i)ap).Type == Type.air) Block.SetBlockQueue((Vector3i)pos + (0, -1, 0), (Vector3i)ap, ba, r.Next(4) > 0 ? Type.leaves : Type.air, null);
				else if (ba.GetBlock((Vector3i)pos + (0, -1, 0), (Vector3i)ap).Type == Type.air) Block.SetBlockQueue((Vector3i)pos + (0, -1, 0), (Vector3i)ap, ba, Type.plant, new Plant(1, h - 1));
				
				if (r.Next(a) > 0 && ba.GetBlock((Vector3i)pos + (0, 0, 1), (Vector3i)ap).Type == Type.air) Block.SetBlockQueue((Vector3i)pos + (0, 0, 1), (Vector3i)ap, ba, r.Next(4) > 0 ? Type.leaves : Type.air, null);
				else if (ba.GetBlock((Vector3i)pos + (0, 0, 1), (Vector3i)ap).Type == Type.air) Block.SetBlockQueue((Vector3i)pos + (0, 0, 1), (Vector3i)ap, ba, Type.plant, new Plant(1, h - 1));

				if (r.Next(a) > 0 && ba.GetBlock((Vector3i)pos + (0, 0, -1), (Vector3i)ap).Type == Type.air) Block.SetBlockQueue((Vector3i)pos + (0, 0, -1), (Vector3i)ap, ba, r.Next(4) > 0 ? Type.leaves : Type.air, null);
				else if (ba.GetBlock((Vector3i)pos + (0, 0, -1), (Vector3i)ap).Type == Type.air) Block.SetBlockQueue((Vector3i)pos + (0, 0, -1), (Vector3i)ap, ba, Type.plant, new Plant(1, h - 1));

				stage = -1;
				Block.SetBlockQueue((Vector3i)pos, (Vector3i)ap, ba, Type.leaves, null);
			}
		}
	}
}
