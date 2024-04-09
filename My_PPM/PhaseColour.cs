using System;
namespace PPM_Fractal_Renderer
{
	public partial class SoftwareShader
	{
		public Col3Byte PhaseColour(int iteration)
		{
			if (iteration != 0)
			{
				return C3BHueValue(iteration, iterations, iteration, iterations, 0.2d);
			}
			else//Horrible workaround
			{
				return C3BHueValue(1, iterations * 2, 1, Convert.ToInt32(iterations) * 2, 0.2d);
			}
		}
		public Col3Byte PhaseColour(double numbere)
		{
			{
				return C3BHueValue(numbere, 2.0d, numbere, 4.0d, 0.2d);
			}
		}
	}
}