using System;
using System.Numerics;
namespace PPM_Fractal_Renderer
{
	public partial class SoftwareShader
	{
		public Col3Byte PeriodColour(Complex z, Complex c, int iteration)
		{
			return(C3BHueValue(z.Magnitude, escapeRadius, z.Magnitude, escapeRadius, -0.2d));

			//return (C3BHueValue(z.Magnitude, c.Magnitude, 2.0d * z.Magnitude / 3.0d, escapeRadius, -0.2d));
		}
	}
}