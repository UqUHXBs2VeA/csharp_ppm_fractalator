using System;
using System.Numerics;

namespace PPM_Fractal_Renderer
{
	public partial class SoftwareShader
	{
		private Complex StandardMandelbrot(Complex z, Complex c)
		{
			return Complex.Pow(z, 2.0d) + c;
		}
		
		private void MandelbrotVoid(Complex z, Complex c)
		{
			z = Complex.Pow(z, 2.0d) + c;
		}
		private void MandelbrotVoidArray(int x, int y,bool[,] array)
		{
			double x_d = Convert.ToDouble(x);
			double y_d = Convert.ToDouble(y);

			Complex z = new Complex
				(
					imgImagX * (x_d / imgWidth_d),
					imgImagY * (y_d / imgHeight_d)
				) + imgImageBottomLeft;
			Complex c = z;
			//Console.WriteLine($"c = {c}");
			//Console.WriteLine($"z = {z}");
			bool isInSet = true;
			for (int k = 0; k < iterations; k++)
			{
				z = Complex.Pow(z, 2.0d) + c;
				//Console.WriteLine($"z = {z}");
				if(z.Magnitude > escapeRadius)
				{
					//Console.WriteLine("Not in set!");
					isInSet = false;
					break;
				}
			}
			array[x, y] = isInSet;
		}
		private void BurningShipVoidArray(int x, int y, bool[,] array)
		{
			double x_d = Convert.ToDouble(x);
			double y_d = Convert.ToDouble(y);

			Complex z = new Complex
				(
					imgImagX * (x_d / bddWidth_d),
					imgImagY * (y_d / bddHeight_d)
				) + bddImageBottomLeft;
			Complex c = z;
			bool isInSet = true;
			for (int k = 0; k < iterations; k++)
			{
				z = Complex.Pow(new Complex(
					Math.Abs(z.Real),
					Math.Abs(z.Imaginary)
					), 2.0d) + c;
				if (z.Magnitude > escapeRadius)
				{
					isInSet = false;
					break;
				}
			}
			array[x, y] = isInSet;
		}
		private Complex BurningShip(Complex z, Complex c)
		{
			return Complex.Pow(new Complex(
					Math.Abs(z.Real),
					Math.Abs(z.Imaginary)
					), 2.0d) + c;
		}
		private Complex MagnetFractal1(Complex z, Complex c)
		{
			return Complex.Divide
					(
						Complex.Pow(z, 2.0d) + (c - 1.0d),
						(2.0d * z) + (c - 2)
					);

		}
		private Complex PerpendicularMandelBrot(Complex z, Complex c)
		{
			return Complex.Pow(new Complex
					(
						Math.Abs(z.Real),
						-z.Imaginary
					),2.0d) + c;

		}
		private Complex MagnetFractal2(Complex z, Complex c)
		{
			return Complex.Pow
				(
					Complex.Divide
					(
						(Complex.Pow(z, 3.0d) + ((z * 3.0d) * (c-1)) + ((c - 1.0d) * (c - 2.0d))),
						(3.0d * Complex.Pow(z,2.0d)) + (3.0d * (c - 2.0d) * z) + ((c - 1.0d) * (c - 2.0d)) + 1.0d
					)
				, 2.0d);
		}
		private Complex RationalTest0001(Complex z)	//Newton-Raphson lookin ass
		{
			return z - Complex.Divide
						(
							(Complex.Pow(z, -3.0d) - 1),
							(-3.0d * Complex.Pow(z,-4.0d))
						);
		}
		private Complex RationalTest0002(Complex z)
		{
			return z - Complex.Divide
						(
							(Complex.Pow(z, -2.0d) - 1),
							(-2.0d * Complex.Pow(z, -3.0d))
						);
		}
		private Complex RationalTest0003(Complex z)
		{
			return z - Complex.Divide
						(
							Complex.Sin(z),
							Complex.Cos(z)
						);
		}
		private Complex Newton(Complex z, Complex c)
		{
			//fn  = Complex.Pow(z,5.0d) + Complex.Pow(z,2.0d) - z + 1
			//fn' = 5.0d * Complex.Pow(z,4.0d) + (2.0d * z) -1
			//return z - Complex.Divide
			//	(
			//		Complex.Pow(z, 5.0d) + Complex.Pow(z, 2.0d) - z + 1,
			//		5.0d * Complex.Pow(z, 4.0d) + (2.0d * z) - 1
			//	);
				return z - Complex.Divide
					(
						Complex.Pow(z, 3.0d) - 1,
						3.0d * Complex.Pow(z,2.0d)
					);
		}
		private Complex NewtonRaphson2(Complex z, Complex c)
		{
			//fn  = Complex.Pow(z,5.0d) + Complex.Pow(z,2.0d) - z + 1
			//fn' = 5.0d * Complex.Pow(z,4.0d) + (2.0d * z) -1
			//return z - Complex.Divide
			//	(
			//		Complex.Pow(z, 5.0d) + Complex.Pow(z, 2.0d) - z + 1,
			//		5.0d * Complex.Pow(z, 4.0d) + (2.0d * z) - 1
			//	);
			//F = Complex.Pow(z,3.0d) -	1
			//dF_dz = 3.0d*Complex.Pow(z, 2.0d)
			//d2F_dz2 = 6.0d * z
			/*
			Complex.Divide
			(
				-3.0d*Complex.Pow(z, 2.0d) + Complex.Sqrt(Complex.Pow(3.0d*Complex.Pow(z, 2.0d),2.0d) - (Complex.Pow(z,3.0d) -	1)*(6.0d * z)),
				6.0d * z
			);
			*/
			Complex val = z - Complex.Divide
			(
				(-3.0d * Complex.Pow(z, 2.0d)) + Complex.Sqrt((9.0d * Complex.Pow(z, 4.0d)) - ((Complex.Pow(z, 3.0d) - 1) * 12.0d * z)),
				6.0d * z
			);
			return val;
		}
		private Complex NewtonPara(Complex z, Complex c, double exponent)
		{
			//fn  = Complex.Pow(z,5.0d) + Complex.Pow(z,2.0d) - z + 1
			//fn' = 5.0d * Complex.Pow(z,4.0d) + (2.0d * z) -1
			//return z - Complex.Divide
			//	(
			//		Complex.Pow(z, 5.0d) + Complex.Pow(z, 2.0d) - z + 1,
			//		5.0d * Complex.Pow(z, 4.0d) + (2.0d * z) - 1
			//	);
			return z - Complex.Divide
				(
					Complex.Pow(z, exponent) - 1,
					exponent * Complex.Pow(z, exponent - 1)
				);
		}
		private void AlienVoidArray(int x, int y, bool[,] array)
		{
			double x_d = Convert.ToDouble(x);
			double y_d = Convert.ToDouble(y);

			Complex z = new Complex
				(
					imgImagX * (x_d / imgWidth_d),
					imgImagY * (y_d / imgHeight_d)
				) + imgImageBottomLeft;
			Complex c = z;
			//Console.WriteLine($"c = {c}");
			//Console.WriteLine($"z = {z}");
			bool isInSet = true;
			for (int k = 0; k < iterations; k++)
			{
				z = Complex.Pow(new Complex(Math.Sin(z.Real),Math.Sin(z.Imaginary)), 2.0d) + c;
				//Console.WriteLine($"z = {z}");
				if (z.Magnitude > escapeRadius)
				{
					//Console.WriteLine("Not in set!");
					isInSet = false;
					break;
				}
			}
			array[x, y] = isInSet;
		}
		private Complex Alien(Complex z, Complex c)
		{
				return Complex.Pow(new Complex(Math.Sin(z.Real), Math.Sin(z.Imaginary)), 2.0d) + c;
		}
		//private Complex BuddhaBrot(Complex z)//Oh jesus fuck how do I do this
		//{
		//So, with the multithreading set up I've got, we gotta asign a colour to each fragment
		//This makes no sense with the buddhabrot in mind. For the naive buddhabrot you have
		//to iterate over each fragment to find a set of other, completely different fragments'
		//colour. So I have to completely remake the whole y_start/y_end slicing proceedure
		//I think I should first of all write an algorithm that mass-detects areas within the
		//set versus those out of the set, as the buddhabrot method is super intensive
		//and pixels in the set give us nothing and are the most expensive to calc.
		//if we assume that we're rendering the whole set
		//I guess strictly speaking we can still split the whole thing in y,
		//and then atomically increment the colourmap.
		//We'd need to remake the colourmap so it supports int32, single chanel.
		//Shouldn't be too hard!
		//... aside from the fact I have to refactor EVERYTHING!!!!!!!!!!!!!!!
		//}
	}
}
