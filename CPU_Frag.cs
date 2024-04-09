using System;
using System.Numerics;

namespace My_PPM
{
	public class CPUShader
	{
		private int imgWidth;
		private int imgHeight;
		public CPUShader(int imgWidth, int imgHeight)
		{
			this.imgWidth = imgWidth;
			this.imgHeight = imgHeight;
		}
		public Col3Byte Calc(int x, int y)
		{
			Col3Byte aaaaa = new Col3Byte(127, 0, 255);
			Console.WriteLine(aaaaa);
		}
	}
}