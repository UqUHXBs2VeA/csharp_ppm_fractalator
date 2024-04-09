using System;
using System.Numerics;
namespace PPM_Fractal_Renderer
{
	public partial class SoftwareShader
	{
		double Weiestrass(double x)
		{
			int weierIterations = 25;
			double b = 5.0d;	//Should be odd interger
			double a = 0.5d;
			double result = 0.0d;
			for(int i = 0; i < weierIterations; i++)
			{
				result += Math.Pow(a, i) * Math.Cos(Math.Pow(b, i) * Math.PI * x);
			}
			return result;
		}
		public void TestFunction2()
		{
			Complex z = new Complex(-1, 3);
			Complex c = new Complex(-1, 3);
			for (int i = 0; i< 20; i++)
			{
			Console.WriteLine($"z = {z}");
			Console.WriteLine($"NewtonRaphson2 = {NewtonRaphson2(z, c)}");
			z = NewtonRaphson2(z, c);
		}
	}
}
}