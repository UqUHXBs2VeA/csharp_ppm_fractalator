using System;
using System.Numerics;
namespace PPM_Fractal_Renderer
{
	public partial class SoftwareShader
	{ /// <summary>
		/// Normalises inputs 1 and 3 based on 2 and 4, and uses the first to find the hue on a colourwheel and the second to find the brightness
		/// </summary>
		/// <param name="hueInt"></param>
		/// <param name="hueMaximum"></param>
		/// <param name="valueInt"></param>
		/// <param name="valueMaximum"></param>
		/// <returns></returns>
		Col3Byte C3BHueValue(int hueInt, int hueMaximum, int valueInt, int valueMaximum, double phaseOffset)
		{
			double phongExponent = 0.35d;    //hideous nonphysical way of boosting the glow on the final image
			double hueDouble = Convert.ToDouble(hueInt);
			double hueMaxDouble = Convert.ToDouble(hueMaximum);
			double valDouble = Convert.ToDouble(valueInt);
			double valMaxDouble = Convert.ToDouble(valueMaximum);

			hueDouble /= hueMaxDouble * 2.0d; valDouble /= valMaxDouble;  //First divide by q to normalise it, then divide by 2 since we square the sine functions so the colours cycle twice otherwise

			int r, g, b;
			double r_d, g_d, b_d;

			b_d = Math.Pow(Math.Sin((hueDouble + (1.0d / 3.0d)) * myTau + phaseOffset), 2.0d);
			r_d = Math.Pow(Math.Sin((hueDouble + (2.0d / 3.0d)) * myTau + phaseOffset), 2.0d);
			g_d = Math.Pow(Math.Sin(hueDouble * myTau + phaseOffset), 2.0d);

			r_d *= Math.Pow(valDouble, phongExponent); g_d *= Math.Pow(valDouble, phongExponent); b_d *= Math.Pow(valDouble, phongExponent);

			r = Convert.ToInt32(r_d * 255.0d);
			g = Convert.ToInt32(g_d * 255.0d);
			b = Convert.ToInt32(b_d * 255.0d);
			//			Console.WriteLine("eee[r_d,g_d,b_d] = [" + $"{r_d}" + "," + $"{g_d}" + "," + $"{b_d}" + "]");
			//			Console.WriteLine("ooo[r,g,b] = [" + $"{r}" + "," + $"{g}" + "," + $"{b}" + "]");
			Col3Byte colour = new Col3Byte(r, g, b);
			return colour;
		}

		Col3Byte C3BHueValue(double hueInt, double hueMaximum, double valueInt, double valueMaximum, double phaseOffset)
		{
			double phongExponent = 0.35d;    //hideous nonphysical way of boosting the glow on the final image
			double hueDouble = hueInt;
			double hueMaxDouble = hueMaximum;
			double valDouble = valueInt;
			double valMaxDouble = valueMaximum;

			hueDouble /= hueMaxDouble * 2.0d; valDouble /= valMaxDouble;  //First divide by q to normalise it, then divide by 2 since we square the sine functions so the colours cycle twice otherwise

			int r, g, b;
			double r_d, g_d, b_d;
			b_d = Math.Sin((hueDouble + (1.0d / 3.0d)) * myTau + phaseOffset);
			r_d = Math.Sin((hueDouble + (2.0d / 3.0d)) * myTau + phaseOffset);
			g_d = Math.Sin(hueDouble * myTau + phaseOffset);

			//Console.WriteLine("iii[r_d,g_d,b_d] = [" + $"{r_d}" + "," + $"{g_d}" + "," + $"{b_d}" + "]");

			b_d = Math.Pow(b_d, 2.0d);
			r_d = Math.Pow(r_d, 2.0d);
			g_d = Math.Pow(g_d, 2.0d);

			//Console.WriteLine("aaa[r_d,g_d,b_d] = [" + $"{r_d}" + "," + $"{g_d}" + "," + $"{b_d}" + "]");
			r_d *= Math.Pow(valDouble, phongExponent); g_d *= Math.Pow(valDouble, phongExponent); b_d *= Math.Pow(valDouble, phongExponent);

			r = Convert.ToInt32(r_d * 255.0d);
			g = Convert.ToInt32(g_d * 255.0d);
			b = Convert.ToInt32(b_d * 255.0d);
			//Console.WriteLine("eee[r_d,g_d,b_d] = [" + $"{r_d}" + "," + $"{g_d}" + "," + $"{b_d}" + "]");
			//Console.WriteLine("ooo[r,g,b] = [" + $"{r}" + "," + $"{g}" + "," + $"{b}" + "]");
			Col3Byte colour = new Col3Byte(r, g, b);
			return colour;
		}
		Col3Byte C3BFromArg(Complex z)
		{
			if (double.IsNaN(z.Magnitude))
			{
				return new Col3Byte(0, 0, 0);
			}
			int r, g, b;
			double r_d, g_d, b_d;
			r_d = Math.Sin((1.0d / 3.0d) * myTau + ((z.Phase)/2.0d));
			g_d = Math.Sin(((2.0d / 3.0d)) * myTau + ((z.Phase) / 2.0d));
			b_d = Math.Sin((z.Phase) / 2.0d);
			b_d = Math.Pow(b_d, 2.0d);
			r_d = Math.Pow(r_d, 2.0d);
			g_d = Math.Pow(g_d, 2.0d);
			r = Convert.ToInt32(r_d * 255.0d);
			g = Convert.ToInt32(g_d * 255.0d);
			b = Convert.ToInt32(b_d * 255.0d);
			//Console.WriteLine("eee[r_d,g_d,b_d] = [" + $"{r_d}" + "," + $"{g_d}" + "," + $"{b_d}" + "]");
			//Console.WriteLine("ooo[r,g,b] = [" + $"{r}" + "," + $"{g}" + "," + $"{b}" + "]");
			Col3Byte colour = new Col3Byte(r, g, b);
			return colour;
		}
		Col3Byte C3BFromArgAndInt(Complex z,int iteration)
		{
			if (double.IsNaN(z.Magnitude))
			{
				return new Col3Byte(0, 0, 0);
			}
			int r, g, b;
			double r_d, g_d, b_d;
			r_d = Math.Sin((1.0d / 3.0d) * myTau + ((z.Phase) / 2.0d));
			g_d = Math.Sin(((2.0d / 3.0d)) * myTau + ((z.Phase) / 2.0d));
			b_d = Math.Sin((z.Phase) / 2.0d);
			double d = 1.0 - Convert.ToDouble(iteration) / Convert.ToDouble(iterations);
			r_d = Math.Pow(r_d, 2.0d)*d;
			g_d = Math.Pow(g_d, 2.0d)*d;
			b_d = Math.Pow(b_d, 2.0d)*d;
			r = Convert.ToInt32(r_d * 255.0d);
			g = Convert.ToInt32(g_d * 255.0d);
			b = Convert.ToInt32(b_d * 255.0d);
			//Console.WriteLine("eee[r_d,g_d,b_d] = [" + $"{r_d}" + "," + $"{g_d}" + "," + $"{b_d}" + "]");
			//Console.WriteLine("ooo[r,g,b] = [" + $"{r}" + "," + $"{g}" + "," + $"{b}" + "]");
			Col3Byte colour = new Col3Byte(r, g, b);
			return colour;
		}
	}
}