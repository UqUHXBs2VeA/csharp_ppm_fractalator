using System;
using System.Numerics;
using System.Collections.Generic;

namespace PPM_Fractal_Renderer
{
	public partial class SoftwareShader
	{
		public void Buddha(UInt16[,] array)
		{
			for(int y = 0; y < bddHeight; y++)
			{
				Console.Write($"[{y}]");
				for(int x = 0; x < bddWidth; x++)
				{
					if(!pixelsInSet[x, y])
					{
						List<Complex> pointsPassed = new List<Complex>();
						double x_d = Convert.ToDouble(x);
						double y_d = Convert.ToDouble(y); //Y and X global coordinates as doubles
						Complex z = new Complex  //Centre of pixel
							(
								imgImagX * (x_d / bddWidth_d),
								imgImagY * (y_d / bddHeight_d)
							) + bddImageBottomLeft;
						bool isInSet;
						Complex c = z;
						isInSet = true;
						pointsPassed.Add(z);
						for (int k = 0; k < buddhaIterations; k++)
						{
							z = StandardMandelbrot(z,c);
							if (z.Magnitude >= escapeRadius)
							{
								isInSet = false;
								break;
							}
							pointsPassed.Add(z);
						}
						if (isInSet)
						{
						}
						else
						{
							//Console.Write($"pointsPassed.Count = {pointsPassed.Count} : ");
							for (int index = 0; index < pointsPassed.Count; index++)
							{
								Complex p = pointsPassed[index];
								p -= bddImageBottomLeft;
								double X_d = p.Real * (bddWidth_d / imgImagX);
								double Y_d = p.Imaginary * (bddHeight_d / imgImagY);
								int X = Convert.ToInt32(X_d);
								int Y = Convert.ToInt32(Y_d);
								if((X >= 0 && X < bddWidth) && (Y >= 0 && Y < bddHeight))
								{
									//Console.Write($"{pointsPassed[index]}");
									array[X, Y]++;
								}
							}
							//Console.Write($"\n");
						}
					}
				}
			}
		}
	}
}