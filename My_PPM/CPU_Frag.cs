using System;
using System.Numerics;
using System.Diagnostics;

namespace PPM_Fractal_Renderer
{
	public partial class SoftwareShader
	{
		public int imgWidth;
		public int imgHeight;
		public int bddWidth;
		public int bddHeight;
		private  double imgWidth_d;
		private  double imgHeight_d;
		private double bddWidth_d;
		private double bddHeight_d;
		private int buddhaSuperSample;
		bool youTube = false;	//Am I watching a youtube video while rendering
		private int numThreads = Environment.ProcessorCount;
		static double imgImagX = 4d; //Imagninary width and height to be rendered
		static double imgImagY = 4d; //TODO ^ this name makes no sense pls fix????
		static double bddImagX = imgImagX; //Imagninary width and height to be rendered
		static double bddImagY = imgImagY; //TODO ^ this name makes no sense pls fix????
		//static double imgImagX = 12.0;
		//static double imgImagY = 12.0;
		static double imgStepX; static double imgStepY; //Imaginary and real strides to take each pixel
		static double bddStepX; static double bddStepY; //Imaginary and real strides to take each pixel
		//Complex imgImagCentre = new Complex(-1.76, 0.0d);
		static Complex imgImagCentre = new Complex(-0, 0.0);
		static Complex bddImagCentre = imgImagCentre;//hmmm
		static double smolX; static double smolY;
		static Complex imgImageBottomLeft;
		static Complex bddImageBottomLeft;
		const int iterations = 2000;
		const int buddhaIterations = 10;
		const int outerIterations = 5;
		const int interiorIterations = 2000;
		static int rootNumSamples = 4; //We use this value squared for subpixel sampling, EXPENSIVE!!
		Col3Byte[,] colourMapRGB;
		const double myTau = Math.PI * 2.0d;	//Please for the LOVE of GOD fucking FIX THIS SHIT
		static double escapeRadius = 2.0d; //Escape radius for samples. Usually set to 2 if you're not mad like me

		bool[,] pixelsInSet;
		bool[,] pixelsDetermined;

		public SoftwareShader(int width, int height, Col3Byte[,] colourMapRGB)
		{
			this.imgWidth = width;
			this.imgHeight = height;

			this.pixelsInSet = new bool[width, height];
			this.pixelsDetermined = new bool[width, height];

			this.colourMapRGB = colourMapRGB;
			this.imgWidth_d = Convert.ToDouble(imgWidth);
			this.imgHeight_d = Convert.ToDouble(imgHeight);
			imgStepX = imgImagX / imgWidth_d;
			imgStepY = imgImagY / imgHeight_d;
			Console.WriteLine("imgStepX = " + $"{imgStepX}");
			Console.WriteLine("imgStepY = " + $"{imgStepY}");
			smolX = imgStepX / Convert.ToDouble(rootNumSamples);
			smolY = imgStepY / Convert.ToDouble(rootNumSamples);
			imgImageBottomLeft = new Complex
				(imgImagCentre.Real -
				(imgImagX / 2.0d),
				imgImagCentre.Imaginary -
				(imgImagY / 2.0d));
			Console.WriteLine("imgImageBottomLeft = " + $"{imgImageBottomLeft}");
			if(youTube == true)
			{
				numThreads -= 2;
			}
		}
		public SoftwareShader(int width, int height, UInt16[,] greyMap, int superBuddha)
		{
			this.imgWidth = width;
			this.imgHeight = height;
			this.buddhaSuperSample= superBuddha;
			this.bddWidth = width * buddhaSuperSample;
			this.bddHeight = height * buddhaSuperSample;
			this.pixelsInSet = new bool[bddWidth, bddHeight];
			this.pixelsDetermined = new bool[bddWidth, bddHeight];

			this.imgWidth_d = Convert.ToDouble(imgWidth);
			this.imgHeight_d = Convert.ToDouble(imgHeight);
			this.bddWidth_d = imgWidth_d * buddhaSuperSample;
			this.bddHeight_d = imgHeight_d * buddhaSuperSample;

			imgStepX = imgImagX / imgWidth_d;
			imgStepY = imgImagY / imgHeight_d;
			bddStepX = imgImagX / bddWidth_d;
			bddStepY = imgImagX / bddHeight_d;
			Console.WriteLine("imgStepX = " + $"{imgStepX}");
			Console.WriteLine("imgStepY = " + $"{imgStepY}");
			Console.WriteLine("bddStepX = " + $"{bddStepX}");
			Console.WriteLine("bddStepY = " + $"{bddStepY}");
			imgImageBottomLeft = new Complex
				(imgImagCentre.Real -
				(imgImagX / 2.0d),
				imgImagCentre.Imaginary -
				(imgImagY / 2.0d));
			bddImageBottomLeft = new Complex
				(bddImagCentre.Real -
				(bddImagX / 2.0d),
				bddImagCentre.Imaginary -
				(bddImagY / 2.0d));
			Console.WriteLine("imgImageBottomLeft = " + $"{imgImageBottomLeft}");
			Console.WriteLine("bddImageBottomLeft = " + $"{bddImageBottomLeft}");
		}
		Col3Byte[,] antiAliasArray;
		public Col3Byte Calc(int x, int y)
		{
			antiAliasArray = new Col3Byte[rootNumSamples, rootNumSamples];
			double x_d = Convert.ToDouble(x);
			double y_d = Convert.ToDouble(y); //Y and X global coordinates as doubles

			Complex zCentre = new Complex  //Centre of pixel
				(
					imgImagX * (x_d / imgWidth_d),
					imgImagY * (y_d / imgHeight_d)
				) + imgImageBottomLeft;

			//zCentre = Complex.Reciprocal(zCentre);	//Generates inverse mandelbrot. Accidentally creates cool looking abberation due to supersampling messing up

			bool isInSet;
			/*
			okay quick essay moment
			how do you calculate the location of the fragment given the base z_0
			and the number of samples
			_ _ _ _ _ _ _ _ _
			|   |   |   |   |
			|___|___|___|___|
			|   |   |   |   |
			|___|___|___|___|
			|   |   |   |   |
			|___|___|___|___|
			|   |   |   |   |
			|___|___|___|___|

			So if you want your samples to form a nice equally spaced grid, you'd ideally need an intersample spacing of the pixel length
			divided by the number of samples [4 in the above example]
			and then shift them all by 1/2 of that down and to the right I guess???
			However, if you're working with the centre coordinate, you'd _add_ half of rootNumSamples times the sample lenght
			so you traverse to the top left, _then_ move them down half a sample.
			*/
			double smolX = imgStepX / Convert.ToDouble(rootNumSamples);
			double smolY = imgStepY / Convert.ToDouble(rootNumSamples);
			Complex smolStep = new Complex(smolX, smolY);

			Complex zCorner = zCentre -	//Inset corner of pixel, starting point for subsample stride
								(
									0.5d * 
									new Complex(imgStepX, imgStepY)
								)	+
								(
									0.5d *
									new Complex(smolX, smolY)
								);

//			Console.WriteLine("zCorner = " + $"{zCorner}");
			Complex z = zCorner;
			Complex c = z;

			int iteration = 0;
			for (int i = 0; i < rootNumSamples; i++)
			{

				for (int j = 0; j < rootNumSamples; j++)
				{

					isInSet = false;

					for (int k = 0; k < iterations; k++)
					{
						isInSet = true;

						z = Complex.Pow(z, 2.0d) + c;
						if( z.Magnitude >= escapeRadius)	//Test what this looks like, don't forget to revert!!
						{
							isInSet = false;
							iteration = k;
							k = iterations;
							break;
						}
					}
					if(isInSet)
					{
						//Console.WriteLine("z in set! i = " + $"{i}" + ", j = " + $"{j}");
						antiAliasArray[i, j] = PeriodColour(z.Magnitude, c.Magnitude, iteration);
					}
					else
					{
						//Console.WriteLine("z not in set! i = " + $"{i}" + ", j = " + $"{j}" + ", iteration = " + $"{iteration}");
						antiAliasArray[i, j] = PhaseColour(iteration);
					}
					z = zCorner + Complex.Multiply(Complex.ImaginaryOne , j * smolStep.Imaginary) + Complex.Multiply(Complex.One, i * smolStep.Real);
					c = z;
				//	Console.WriteLine("update! z = " + $"{z}");
				}
			}

			int r_i = 0;	//Use int types to avoid overflow error if all values are 255
			int g_i = 0;
			int b_i = 0;

			for (int i = 0; i < rootNumSamples; i++)
			{
				for(int j = 0; j < rootNumSamples; j++)
				{
					r_i += Convert.ToInt32(antiAliasArray[i, j].R);
					g_i += Convert.ToInt32(antiAliasArray[i, j].G);
					b_i += Convert.ToInt32(antiAliasArray[i, j].B);
				}
			}
//			Console.WriteLine("[r_i,g_i,b_i] = [" + $"{r_i}" + "," + $"{g_i}" + "," + $"{b_i}" + "]");
			double r_d = Convert.ToDouble(r_i);
			double g_d = Convert.ToDouble(g_i);
			double b_d = Convert.ToDouble(b_i);
			double killMe = Convert.ToDouble(antiAliasArray.Length);
			r_d /= killMe;
			g_d /= killMe;
			b_d /= killMe;
//			Console.WriteLine("[r_d,g_d,b_d] = [" + $"{r_d}" + "," + $"{g_d}" + "," + $"{b_d}" + "]");
			byte r = Convert.ToByte(r_d);
			byte g = Convert.ToByte(g_d);
			byte b = Convert.ToByte(b_d);
//			Console.WriteLine("[r,g,b] = [" + $"{r}" + "," + $"{g}" + "," + $"{b}" + "]");
			Col3Byte avgerageCol = new Col3Byte(r,g,b);
			return avgerageCol;
			//			double magnitude = 1.0d;
			//			double phase = myTau * (1.0d + 7.0d/8.0d)/ 4.0d;
			//			Complex c = new Complex(magnitude * Math.Cos(phase), magnitude * Math.Sin(phase));
			/*
						for (int i = 0; i < iterations; i++)
						{
							z = Complex.Pow(z, 2) + c;
							if( z.Magnitude >= 2)
							{
								return PhaseColour(i, iterations);
							}
							*/
		}
		public Col3Byte CalcNotAntiA(int x, int y)
		{

			double x_d = Convert.ToDouble(x);
			double y_d = Convert.ToDouble(y); //Y and X global coordinates as doubles

			Complex zCentre = new Complex  //Centre of pixel
				(
					imgImagX * (x_d / imgWidth_d),
					imgImagY * (y_d / imgHeight_d)
				) + imgImageBottomLeft;

			bool isInSet;
		
			Complex smolStep = new Complex(smolX, smolY);

			Complex zCorner = zCentre;

			Complex z = zCorner;
			Complex c = z;

			Col3Byte colourByte = new Col3Byte(255, 63, 127);
			int iteration = 0;
			isInSet = false;
			for (int k = 0; k < iterations; k++)
			{
				isInSet = true;
				z = Complex.Pow(z, 2.0d) + c;
				if (z.Magnitude >= escapeRadius)  //Test what this looks like, don't forget to revert!!
				{
					isInSet = false;
					iteration = k;
					k = iterations;
					break;
				}
			}
			if (isInSet)
			{
				//Console.WriteLine("z in set! i = " + $"{i}" + ", j = " + $"{j}");
				colourByte = PeriodColour(z.Magnitude, c.Magnitude, iteration);
			}
			else
			{
				//Console.WriteLine("z not in set! i = " + $"{i}" + ", j = " + $"{j}" + ", iteration = " + $"{iteration}");
				colourByte = PhaseColour(iteration);
			}
			//Console.WriteLine("update! z = " + $"{z}");

			return colourByte;
			//			double magnitude = 1.0d;
			//			double phase = myTau * (1.0d + 7.0d/8.0d)/ 4.0d;
			//			Complex c = new Complex(magnitude * Math.Cos(phase), magnitude * Math.Sin(phase));
			/*
						for (int i = 0; i < iterations; i++)
						{
							z = Complex.Pow(z, 2) + c;
							if( z.Magnitude >= 2)
							{
								return PhaseColour(i, iterations);
							}
							*/
		}
		public Col3Byte CalcBW(int x, int y)
		{
			{

				double x_d = Convert.ToDouble(x);
				double y_d = Convert.ToDouble(y); //Y and X global coordinates as doubles
				Complex zCentre = new Complex  //Centre of pixel
					(
						imgImagX * (x_d / imgWidth_d),
						imgImagY * (y_d / imgHeight_d)
					) + imgImageBottomLeft;

				bool isInSet;
				Complex zCorner = zCentre;
				Complex z = zCorner;
				Complex c = z;
				int iteration = 0;
				isInSet = false;
				for (int k = 0; k < iterations; k++)
				{
					isInSet = true;
					z = Complex.Pow(z, 2.0d) + c;
					if (z.Magnitude >= escapeRadius)  //Test what this looks like, don't forget to revert!!
					{
						isInSet = false;
						iteration = k;
						break;
					}
				}
				if (isInSet)
				{
					//Console.WriteLine("z in set! i = " + $"{i}" + ", j = " + $"{j}");
					return new Col3Byte(255,255,255);
				}
				else
				{
					//Console.WriteLine("z not in set! i = " + $"{i}" + ", j = " + $"{j}" + ", iteration = " + $"{iteration}");
					return new Col3Byte(0, 0, 0);
				}
			}
		}
	}
}
