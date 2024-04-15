using OpenTK.Mathematics;

namespace CubeGame.Scripts
{
	internal class Collision
	{
		public static Vector3 CheckCollision(Vector3 lastPosition, Vector3 position, ref Vector3 velocity, World world)
		{
			// коллизии
			//разные позиции
			Vector3 offset = (0, 0, 0);

			// выходные переменные
			Vector3 outPosition = position;

			// условие коллизии
			for(int i = 0; i < 12; i++)
			{
				offset.X = (i % 2) * 1f;
				offset.Y = (i / 4 - 1) * 1f;
				offset.Z = (i / 2 % 2) * 1f;
				(_, Vector3i ap, Vector3i bp) = BlocksArray.GetPoses(position + offset, 0);
				(_, Vector3i lap, Vector3i lbp) = BlocksArray.GetPoses(lastPosition + offset, 0);

				if (world.arraysPos.TryGetValue(ap, out BlocksArray? tmp))
				{
					Block block = tmp.GetBlock(bp, ap);

					if (block != null && block.Type != Type.air)
					{
						//Y
						if (world.arraysPos.TryGetValue((ap.X, lap.Y, ap.Z), out tmp))
							if (tmp.GetBlock((bp.X, lbp.Y, bp.Z), (ap.X, lap.Y, ap.Z)).Type == Type.air)
							{
								outPosition.Y = lastPosition.Y;
								velocity.Y = 0;
								goto a;
							}

						//X
						if (world.arraysPos.TryGetValue((lap.X, ap.Y, ap.Z), out tmp))
							if (tmp.GetBlock((lbp.X, bp.Y, bp.Z), (lap.X, ap.Y, ap.Z)).Type == Type.air)
							{
								outPosition.X = lastPosition.X;
								velocity.X = 0;
								goto a;
							}

						//Z
						if (world.arraysPos.TryGetValue((ap.X, ap.Y, lap.Z), out tmp))
							if (tmp.GetBlock((bp.X, bp.Y, lbp.Z), (ap.X, ap.Y, lap.Z)).Type == Type.air)
							{
								outPosition.Z = lastPosition.Z;
								velocity.Z = 0;
								goto a;
							}

						//XY
						if (world.arraysPos.TryGetValue((lap.X, lap.Y, ap.Z), out tmp))
							if (tmp.GetBlock((lbp.X, lbp.Y, bp.Z), (lap.X, lap.Y, ap.Z)).Type == Type.air)
							{
								outPosition.Xy = lastPosition.Xy;
								velocity.Xy = (0, 0);
								goto a;
							}

						//XZ
						if (world.arraysPos.TryGetValue((lap.X, ap.Y, lap.Z), out tmp))
							if (tmp.GetBlock((lbp.X, bp.Y, lbp.Z), (lap.X, ap.Y, lap.Z)).Type == Type.air)
							{
								outPosition.Xz = lastPosition.Xz;
								velocity.Xz = (0, 0);
								goto a;
							}

						//ZY
						if (world.arraysPos.TryGetValue((ap.X, lap.Y, lap.Z), out tmp))
							if (tmp.GetBlock((bp.X, lbp.Y, lbp.Z), (ap.X, lap.Y, lap.Z)).Type == Type.air)
							{
								outPosition.Yz = lastPosition.Yz;
								velocity.Yz = (0, 0);
								goto a;
							}

						//XYZ
						if (world.arraysPos.TryGetValue(lap, out tmp))
							if (tmp.GetBlock(lbp, lap).Type == Type.air)
							{
								outPosition = lastPosition;
								velocity = (0, 0, 0);
								goto a;
							}
					}
				a:;
				}

			}

			return outPosition;
		}
	}
}
