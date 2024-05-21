namespace ProjectCube.Scripts
{
	internal class Bit
	{
		public static bool GetBit(int i, int n)
		{
			n = SetBit(n);

			return (i & n) == n;
		}

		public static int SetBit(int n)
		{
			return (int)MathF.Pow(2, n);
		}

		public static void SetBit(ref int i , int n)
		{
			n = SetBit(n);

			if (i < n) i += n;
		}

		public static void ResetBit(ref int i, int n)
		{
			n = SetBit(n);

			if (i >= n) i -= n;
		}
	}
}
