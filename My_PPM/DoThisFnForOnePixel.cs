using System;
using System.Numerics;
namespace PPM_Fractal_Renderer
{
	public partial class SoftwareShader
	{
		public bool CalcForPixel(int x, int y, Action<Complex, Complex> Function)
		{
			double x_d = Convert.ToDouble(x);
			double y_d = Convert.ToDouble(y);

			Complex z = new Complex
				(
					imgImagX * (x_d / imgWidth_d),
					imgImagY * (y_d / imgHeight_d)
				) + imgImageBottomLeft;
			bool isInSet;
			Complex c = z;
			isInSet = true;
			for (int k = 0; k < iterations; k++)
			{
				Function(z, c);
				if (z.Magnitude >= escapeRadius)
				{
					isInSet = false;
					break;
				}
			}
			return isInSet;
		}
		public bool CalcForComplexCoord(int x, int y, Action<Complex, Complex> Function)
		{
			double x_d = Convert.ToDouble(x);
			double y_d = Convert.ToDouble(y);

			Complex z = new Complex
				(
					imgImagX * (x_d / imgWidth_d),
					imgImagY * (y_d / imgHeight_d)
				) + imgImageBottomLeft;
			bool isInSet;
			Complex c = z;
			isInSet = true;
			for (int k = 0; k < iterations; k++)
			{
				Function(z, c);
				if (z.Magnitude >= escapeRadius)
				{
					isInSet = false;
					break;
				}
			}
			return isInSet;
		}
		public void  CalcForComplexBoarder(int xStart, int yStart, int boxSize, Action<int, int,bool[,]> Function,bool[,] array)//11.2.22<<< this fuck is the issue rn
		{
			for (int x = xStart; x < xStart + boxSize + 1; x++)
			{
				for (int y = yStart; y < yStart + boxSize + 1; y++)
				{
					if(x% boxSize == 0 || y% boxSize == 0)
					{
						Function(x, y, array);
					}
				}
			}
		}
		public void CalcForComplexBoarder(int xStart, int yStart, int boxSize, Action<Complex, Complex> Function, bool[,] array)//11.2.22<<< this fuck is the issue rn
		{
			for (int x = xStart; x < xStart + boxSize + 1; x++)
			{
				for (int y = yStart; y < yStart + boxSize + 1; y++)
				{
					if (x % boxSize == 0 || y % boxSize == 0)
					{
						double x_d = Convert.ToDouble(x);
						double y_d = Convert.ToDouble(y);

						Complex z = new Complex
							(
								imgImagX * (x_d / imgWidth_d),
								imgImagY * (y_d / imgHeight_d)
							) + imgImageBottomLeft;
						bool isInSet;
						Complex c = z;
						isInSet = true;
						for (int k = 0; k < iterations; k++)
						{
							Function(z, c);
							if (z.Magnitude >= escapeRadius)
							{
								isInSet = false;
								break;
							}
						}
						array[x,y] = isInSet;
					}
				}
			}
		}
		public void CalcForComplexRegion(int xStart, int yStart, int boxSize, Action<Complex, Complex> Function, bool[,] array)
		{
			for (int x = xStart; x < xStart + boxSize + 1; x++)
			{
				for (int y = yStart; y < yStart + boxSize + 1; y++)
				{
					double x_d = Convert.ToDouble(x);
					double y_d = Convert.ToDouble(y);

					Complex z = new Complex
						(
							imgImagX * (x_d / imgWidth_d),
							imgImagY * (y_d / imgHeight_d)
						) + imgImageBottomLeft;
					bool isInSet;
					Complex c = z;
					isInSet = true;
					for (int k = 0; k < iterations; k++)
					{
						Function(z, c);
						if (z.Magnitude >= escapeRadius)
						{
							isInSet = false;
							break;
						}
					}
					array[x, y] = isInSet;
				}
			}
		}
		public void CalcForBoarder(int xStart, int yStart, int boxSize, Action<int, int, bool[,]> Function, bool[,] array)
		{
			for (int x = xStart; x < xStart + boxSize + 1; x++)
			{
				for (int y = yStart; y < yStart + boxSize + 1; y++)
				{
					if (x % boxSize == 0 || y % boxSize == 0)
					{
						Function(x, y, array);
					}
				}
			}
		}
		public void CalcForRegion(int xStart, int yStart, int boxSize, Action<int, int, bool[,]> Function, bool[,] array)
		{
			for (int x = xStart; x < xStart + boxSize + 1; x++)
			{
				for (int y = yStart; y < yStart + boxSize + 1; y++)
				{
					Function(x, y,array);
				}
			}
		}
		public void SingleDebugCircleTest(int x, int y,bool[,] array)
		{
					double x_d = Convert.ToDouble(x);
					double y_d = Convert.ToDouble(y);
					x_d -= imgWidth_d / 2;
					y_d -= imgHeight_d / 2;
					double radius = Math.Sqrt(Math.Pow(x_d, 2) + Math.Pow(y_d, 2));
					double imageRadius = Math.Sqrt(Math.Pow(imgHeight_d / 2, 2) + Math.Pow(imgWidth_d / 2, 2));
					if (radius < imageRadius / 2)
					{
						array[x,y] = true;
					}
					else
					{
						array[x, y] = false;
					}
		}
		public void DebugCircleTest(int xStart, int yStart, int boxWidth, bool[,] array)
		{
			for (int x = xStart; x < xStart + boxWidth; x++)
			{
				for (int y = yStart; y < yStart + boxWidth; y++)
				{
					double x_d = Convert.ToDouble(x);
					double y_d = Convert.ToDouble(y);
					x_d -= imgWidth_d / 2;
					y_d -= imgHeight_d / 2;
					double radius = Math.Sqrt(Math.Pow(x_d, 2) + Math.Pow(y_d, 2));
					double imageRadius = Math.Sqrt(Math.Pow(imgHeight_d / 2, 2) + Math.Pow(imgWidth_d / 2, 2));
					if ( radius < imageRadius/2)
					{
						array[x, y] = true;
					}
					else
					{
						array[x, y] = false;
					}
				}
			}
		}
	}
}
