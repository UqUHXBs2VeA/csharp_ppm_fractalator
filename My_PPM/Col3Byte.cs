using System;
namespace PPM_Fractal_Renderer
{
	public class Col3Byte
	{
		public byte[] RGBArray = new byte[3];

		public Col3Byte(int r, int g, int b)
		{
			this.RGBArray[0] = Convert.ToByte(r);
			this.RGBArray[1] = Convert.ToByte(g);
			this.RGBArray[2] = Convert.ToByte(b);
		}
		public Col3Byte(UInt16 r, UInt16 g, UInt16 b)//12.2.22 test!!
		{
			this.RGBArray[0] = Convert.ToByte(r);
			this.RGBArray[1] = Convert.ToByte(g);
			this.RGBArray[2] = Convert.ToByte(b);
		}

		public Col3Byte(byte r, byte g, byte b)
		{
			this.RGBArray[0] = r;
			this.RGBArray[1] = g;
			this.RGBArray[2] = b;
		}
		public Col3Byte(double r, double g, double b)
		{
			this.RGBArray[0] = Convert.ToByte(r);
			this.RGBArray[1] = Convert.ToByte(g);
			this.RGBArray[2] = Convert.ToByte(b);
		}
		public Col3Byte(double phase, double amplitude)
		{
			double diff = (Math.PI * 2.0 / 3.0);
			this.RGBArray[0] = Convert.ToByte(Math.Floor(255.0 * (Math.Pow(Math.Cos(phase),2.0d)/2.0d) * amplitude));
			this.RGBArray[1] = Convert.ToByte(Math.Floor(255.0 * (Math.Pow(Math.Cos(phase + diff), 2.0d)/2.0d) * amplitude));
			this.RGBArray[2] = Convert.ToByte(Math.Floor(255.0 * (Math.Pow(Math.Cos(phase + (2.0d * diff)), 2.0d)/2.0d) * amplitude));
		}
		public byte R
		{
			get
			{
				return RGBArray[0];
			}
			set
			{
				RGBArray[0] = value;
			}
		}
		public byte G
		{
			get
			{
				return RGBArray[1];
			}
			set
			{
				RGBArray[1] = value;
			}
		}
		public byte B
		{
			get
			{
				return RGBArray[2];
			}
			set
			{
				RGBArray[2] = value;
			}
		}
	}
}
