using System;
using System.Threading;
using System.Numerics;
using System.Diagnostics;
namespace PPM_Fractal_Renderer
{
	public partial class SoftwareShader
	{
		/// <summary>
		/// not too happy abt this but what can you do
		/// </summary>
		/// <param name="feshPince">The array whose elements end up holding the data output</param>
		/// <param name="ofBlair">The single-parameter code which gets called for each element</param>
		/// <param name="parmaham">The parameter used by ofBlair</param>
		public void AllocateThreads(bool[,] feshPince, Action<bool[,],int,int,object> ofBlair,object parmaham)
		{
			double ratio_d = feshPince.GetLength(1) / Convert.ToDouble(numThreads);
			Console.WriteLine("raito_d = " + ratio_d.ToString());
			int ratio_i = Convert.ToInt32(Math.Floor(ratio_d));
			Console.WriteLine("raito_i = " + ratio_i.ToString());
			Thread[] threads = new Thread[numThreads];
			for (int i = 0; i < numThreads - 1; i++)
			{
				Console.WriteLine("Allocating thread " + $"{i}" + "!");
				threads[i] = InitThread(feshPince, i * ratio_i, (i + 1) * ratio_i, ofBlair, parmaham);
				threads[i].Name = "Thread_" + $"{i}";
			}//Init threads
			{
				Console.WriteLine("Allocating final thead!");
				threads[numThreads - 1] = InitThread(feshPince, (numThreads - 1) * ratio_i, feshPince.GetLength(1), ofBlair, parmaham);
				threads[numThreads - 1].Name = "Thread_" + $"{numThreads - 1}";
				Console.WriteLine("numThreads * ratio_i = " + (numThreads * ratio_i).ToString());
			}//Init final thread
			for (int i = 0; i < numThreads; i++)
			{
				Console.WriteLine("Starting thread " + $"{i}" + "!");
				threads[i].Start();
			}//Start threads
			Console.WriteLine("Joining threads...");
			for (int i = 0; i < numThreads; i++)
			{
				Console.WriteLine("Joined thread " + $"{i}" + "!");
				threads[i].Join();
			}//Join threads
		}
		public void AllocateThreads(bool[,] feshPince, Action<bool[,], int, int> ofBlair)
		{
			double ratio_d = feshPince.GetLength(1) / Convert.ToDouble(numThreads);
			Console.WriteLine("raito_d = " + ratio_d.ToString());
			int ratio_i = Convert.ToInt32(Math.Floor(ratio_d));
			Console.WriteLine("raito_i = " + ratio_i.ToString());
			Thread[] threads = new Thread[numThreads];
			for (int i = 0; i < numThreads - 1; i++)
			{
				Console.WriteLine("Allocating thread " + $"{i}" + "!");
				threads[i] = InitThread(feshPince, i * ratio_i, (i + 1) * ratio_i, ofBlair);
				threads[i].Name = "Thread_" + $"{i}";
			}//Init threads
			{
				Console.WriteLine("Allocating final thead!");
				threads[numThreads - 1] = InitThread(feshPince, (numThreads - 1) * ratio_i, feshPince.GetLength(1), ofBlair);
				threads[numThreads - 1].Name = "Thread_" + $"{numThreads - 1}";
				Console.WriteLine("numThreads * ratio_i = " + (numThreads * ratio_i).ToString());
			}//Init final thread
			for (int i = 0; i < numThreads; i++)
			{
				Console.WriteLine("Starting thread " + $"{i}" + "!");
				threads[i].Start();
			}//Start threads
			Console.WriteLine("Joining threads...");
			for (int i = 0; i < numThreads; i++)
			{
				Console.WriteLine("Joined thread " + $"{i}" + "!");
				threads[i].Join();
			}//Join threads
		}


		/// <summary>
		/// not too happy abt this but what can you doJUST_FIND_A_WAY_TO_MAKE_ME_HAPPY
		/// </summary>
		/// <param name="feshPince">The array whose elements end up holding the data output.</param>
		/// <param name="ofBlair">The single-parameter code which gets called for each element</param>
		/// <param name="parmaham">The parameter used by ofBlair</param>
		public void AllocateThreads(Col3Byte[,] feshPince, Action<Col3Byte[,], int, int, object> ofBlair, object parmaham)//not too happy abt this but what can you doJUST_FIND_A_WAY_TO_MAKE_ME_HAPPY
		{
			double ratio_d = feshPince.GetLength(1) / Convert.ToDouble(numThreads);
			Console.WriteLine("raito_d = " + ratio_d.ToString());
			int ratio_i = Convert.ToInt32(Math.Floor(ratio_d));
			Console.WriteLine("raito_i = " + ratio_i.ToString());
			Thread[] threads = new Thread[numThreads];
			for (int i = 0; i < numThreads - 1; i++)
			{
				Console.WriteLine("Allocating thread " + $"{i}" + "!");
				threads[i] = InitThread(feshPince, i * ratio_i, (i + 1) * ratio_i, ofBlair, parmaham);
				threads[i].Name = "Thread_" + $"{i}";
			}//Init threads
			{
				Console.WriteLine("Allocating final thead!");
				threads[numThreads - 1] = InitThread(feshPince, (numThreads - 1) * ratio_i, feshPince.GetLength(1), ofBlair, parmaham);
				threads[numThreads - 1].Name = "Thread_" + $"{numThreads - 1}";
				Console.WriteLine("numThreads * ratio_i = " + (numThreads * ratio_i).ToString());
			}//Init final thread
			for (int i = 0; i < numThreads; i++)
			{
				Console.WriteLine("Starting thread " + $"{i}" + "!");
				threads[i].Start();
			}//Start threads
			Console.WriteLine("Joining threads...");
			for (int i = 0; i < numThreads; i++)
			{
				Console.WriteLine("Joined thread " + $"{i}" + "!");
				threads[i].Join();
			}//Join threads
		}
		public void TestFunction(Col3Byte[,] colourArray, int y_start, int y_end, object parmaham)
		{
			bool isInSet;
			for (int y = y_start; y < y_end; y++)
			{
				for (int x = 0; x < colourArray.GetLength(0); x++)
				{
					double x_d = Convert.ToDouble(x);
					double y_d = Convert.ToDouble(y); //Y and X global coordinates as doubles
																						//Console.WriteLine("x = " + $"{x}" + ", y = " + $"{y}");
					Complex z = new Complex //Centre of pixel
					(
						imgStepX * x_d,
						imgStepY * y_d
					) + imgImageBottomLeft;
					//zCentre = Complex.Reciprocal(zCentre);	//Generates inverse set. Accidentally creates cool looking abberation due to supersampling messing up
					Complex c = z;
					isInSet = true;
					//for (int k = 0; k < Convert.ToInt32(parmaham); k++)
					for (int k = 0; k < iterations; k++)
					{
						z = StandardMandelbrot(z, c);
						//z = Complex.Pow(z,Convert.ToDouble(parmaham)) + c;
						if (z.Magnitude >= escapeRadius)
						{
							isInSet = false;
							colourArray[x, y] = new Col3Byte(255, 255, 255);
							break;
						}
					}
					if (!isInSet)
					{
						//colourArray[x, y] = new Col3Byte(new Random().Next(0, 255), new Random().Next(0, 255), new Random().Next(0, 255));
						colourArray[x, y] = new Col3Byte(255,255,255);
					}
				}
			}
		}

		public Thread InitThread(bool[,] boolArray, int y_start, int y_end, Action<bool[,], int, int, object> funyuns, object parmaham)
		{
			Thread t = new Thread(() => funyuns(boolArray, y_start, y_end, parmaham));
			t.Priority = ThreadPriority.Highest;
			return t;
		}
		public Thread InitThread(bool[,] boolArray, int y_start, int y_end, Action<bool[,], int, int> funyuns)
		{
			Thread t = new Thread(() => funyuns(boolArray, y_start, y_end));
			t.Priority = ThreadPriority.Highest;
			return t;
		}
		public Thread InitThread(Col3Byte[,] colourArray, int y_start, int y_end, Action<Col3Byte[,], int, int, object> funyuns, object parmaham)
		{
			Thread t = new Thread(() => funyuns(colourArray, y_start, y_end, parmaham));
			t.Priority = ThreadPriority.Highest;
			return t;
		}


		//public Thread InitThread(bool[,] boolArray, int y_start, int y_end, object parmaham)
		//{
		//	Thread t = new Thread(() => WIN_THE_RACE_JUST_WIN_THE_RACE(boolArray, y_start, y_end, parmaham));
		//	t.Priority = ThreadPriority.Highest;
		//	return t;
		//}
		/*public void AllocateThreads(bool[,] feshPince, object parmaham)//not too happy abt this but what can you doJUST_FIND_A_WAY_TO_MAKE_ME_HAPPY
		{
			double ratio_d = imgHeight_d / Convert.ToDouble(numThreads);
			Console.WriteLine("raito_d = " + ratio_d.ToString());
			int ratio_i = Convert.ToInt32(Math.Floor(ratio_d));
			Console.WriteLine("raito_i = " + ratio_i.ToString());
			Thread[] threads = new Thread[numThreads];
			for (int i = 0; i < numThreads - 1; i++)
			{
				Console.WriteLine("Allocating thread " + $"{i}" + "!");
				threads[i] = InitThread(feshPince, i * ratio_i, (i + 1) * ratio_i, parmaham);
				threads[i].Name = "Thread_" + $"{i}";
			}//Init threads
			{
				Console.WriteLine("Allocating final thead!");
				threads[numThreads - 1] = InitThread(feshPince, (numThreads -1) * ratio_i, imgHeight, parmaham);
				threads[numThreads - 1].Name = "Thread_" + $"{numThreads - 1}";
				Console.WriteLine("numThreads * ratio_i = " + (numThreads * ratio_i).ToString());
			}//Init final thread
			for (int i = 0; i < numThreads; i++)
			{
				Console.WriteLine("Starting thread " + $"{i}" +"!");
				threads[i].Start();
			}//Start threads
			Console.WriteLine("Joining threads...");
			for (int i = 0; i < numThreads; i++)
			{
				Console.WriteLine("Joined thread " + $"{i}" + "!");
				threads[i].Join();
			}//Join threads
		}*/



		public Thread InitThread(Col3Byte[,] colourArray, int y_start, int y_end, object parmaham)
		{
			Thread t = new Thread(() => WIN_THE_RACE_JUST_WIN_THE_RACE(colourArray, y_start, y_end,parmaham));
			t.Priority = ThreadPriority.Highest;
			return t;
		}

		//public Thread InitThread(bool[,] colourArray, int y_start, int y_end, object parmaham)
		//{
		//	Thread t = new Thread(() => WIN_THE_RACE_JUST_WIN_THE_RACE(colourArray, y_start, y_end,parmaham));
		//	t.Priority = ThreadPriority.Highest;
		//	return t;
		//}
		/// <summary>
		/// I shoulda given this a better fucking name -_-
		/// </summary>
		public void WIN_THE_RACE_JUST_WIN_THE_RACE(Col3Byte[,] colourArray, int y_start, int y_end, object parmaham)
		{
			bool isInSet;
			Complex smolStep = new Complex(smolX, smolY);
			Col3Byte[,] antiAliasArray = new Col3Byte[rootNumSamples, rootNumSamples];
			double antiALength_d = Convert.ToDouble(antiAliasArray.Length);
			double maxRadius;
			for (int y = y_start; y < y_end; y++)
			{
				for (int x = 0; x < imgWidth; x++)
				{
					double x_d = Convert.ToDouble(x);
					double y_d = Convert.ToDouble(y); //Y and X global coordinates as doubles
					//Console.WriteLine("x = " + $"{x}" + ", y = " + $"{y}");
					Complex zCentre = new Complex   //Centre of pixel
					(
						imgStepX * x_d,
						imgStepY * y_d
					) + imgImageBottomLeft;
					//zCentre = Complex.Reciprocal(zCentre);	//Generates inverse set
					Complex zCorner = zCentre - //Inset corner of pixel, starting point for subsample stride
									(
										0.5d *
										new Complex(imgStepX, imgStepY)
									) +
									(
										0.5d *
										smolStep
									);
					Complex z = zCorner;
					//z = Complex.Reciprocal(z);
					Complex c = z;
					maxRadius = c.Magnitude;
					int iteration = 0;
					for (int i = 0; i < rootNumSamples; i++)
					{
						for (int j = 0; j < rootNumSamples; j++)
						{
							isInSet = false;
							for (int k = 0; k < iterations; k++)
							{
								if (double.IsNaN(z.Magnitude))
								{
									Console.WriteLine("z escaped!! c = " + $"{c}");
									isInSet = false;
									break;
								}
								//z = Newton(z,c);
								
								else if(z == NewtonRaphson2(z, c))
								{
									Console.WriteLine($"Root found! z = {z}");
									isInSet = true;
									iteration = k;
									break;
								}
								else
								
								{
									z = NewtonRaphson2(z, c);
								}
								/*
								if (z.Magnitude >= escapeRadius)
								{
									isInSet = false;
									iteration = k;
									break;
								}*/
								/*
								if (maxRadius <= z.Magnitude && maxRadius < 2.0d)
								{
									maxRadius = z.Magnitude;
								}
								*/
							}
							/*
							if(isInSet && c.Magnitude != 0)
							{
								//	Console.WriteLine("z in set! x = " + $"{x}" + ", y = " + $"{y}");
								antiAliasArray[i, j] = C3BHueValue(z.Magnitude, c.Magnitude, c.Magnitude, c.Magnitude, -0.6);
								//antiAliasArray[i, j] = new Col3Byte(127, 255, 191);
							}
							else*/
							{
								//if (maxRadius >= 2.0d) { antiAliasArray[i, j] = PhaseColour(2.0d); }
								//if (maxRadius >= 2.0d) { antiAliasArray[i, j] = new Col3Byte(0, 0, 0); }
								//else
								//	Console.WriteLine("z not in set! i = " + $"{i}" + ", j = " + $"{j}" + ", iteration = " + $"{iteration}");
								// antiAliasArray[i, j] = PhaseColour(maxRadius); }
								//{ antiAliasArray[i, j] = new Col3Byte(0,0,0); }
								
								{
									antiAliasArray[i, j] = C3BFromArg(z);
								}
							}
							z = zCorner +
								Complex.Multiply(Complex.ImaginaryOne,
								j * smolStep.Imaginary) +
								Complex.Multiply(Complex.One,
								i * smolStep.Real);
							//z = Complex.Reciprocal(z); //oh god why is this here again
							c = z;
						}
					}
					{
						int r_i = 0;//Use int types to avoid overflow error if all values are 255
						int g_i = 0;
						int b_i = 0;
						for (int i = 0; i < rootNumSamples; i++)
						{
							for (int j = 0; j < rootNumSamples; j++)
							{
								r_i += Convert.ToInt32(antiAliasArray[j, i].R);
								g_i += Convert.ToInt32(antiAliasArray[j, i].G);
								b_i += Convert.ToInt32(antiAliasArray[j, i].B);
							}
						}
						//Console.WriteLine("[r_i,g_i,b_i] = [" + $"{r_i}" + "," + $"{g_i}" + "," + $"{b_i}" + "]");
						double r_d = Convert.ToDouble(r_i);
						double g_d = Convert.ToDouble(g_i);
						double b_d = Convert.ToDouble(b_i);
						r_d /= antiALength_d;
						g_d /= antiALength_d;
						b_d /= antiALength_d;
						//Console.WriteLine("[r_d,g_d,b_d] = [" + $"{r_d}" + "," + $"{g_d}" + "," + $"{b_d}" + "]");
						byte r = Convert.ToByte(r_d);
						byte g = Convert.ToByte(g_d);
						byte b = Convert.ToByte(b_d);
						//Console.WriteLine("[r,g,b] = [" + $"{r}" + "," + $"{g}" + "," + $"{b}" + "]");
						colourArray[x, y] = new Col3Byte(r, g, b);
					}
				}
			}
		}//Currently broken, makes superposition of inverse/noninv sets???
		private void I_DONT_CARE_WHAT_IT_TAKES(Col3Byte[,] colourArray, int y_start, int y_end, Complex c)
		{
			bool isInSet;
			Complex smolStep = new Complex(smolX, smolY);
			Col3Byte[,] antiAliasArray = new Col3Byte[rootNumSamples, rootNumSamples];
			double antiALength_d = Convert.ToDouble(antiAliasArray.Length);
			for (int y = y_start; y < y_end; y++)
			{
				for (int x = 0; x < imgWidth; x++)
				{
					double x_d = Convert.ToDouble(x);
					double y_d = Convert.ToDouble(y);
					Complex zCentre = new Complex
					(
						 imgStepX * x_d,
						imgStepY * y_d
					) + imgImageBottomLeft;
					Complex zCorner = zCentre - //Inset corner of pixel, starting point for subsample stride
									(
										0.5d *
										new Complex(imgStepX, imgStepY)
									) +
									(
										0.5d *
										smolStep
									);
					Complex z = zCorner;
					int iteration = 0;
					for (int i = 0; i < rootNumSamples; i++)
					{
						for (int j = 0; j < rootNumSamples; j++)
						{
							isInSet = false;
							for (int k = 0; k < iterations; k++)
							{
								isInSet = true;
								/*z = Complex.Pow(
									new Complex
									(
										Math.Sin(z.Real),
										Math.Sin(z.Imaginary)
									), 2) + c;*/
								z = Complex.Pow(z, 2.0d) + c;
								if (z.Magnitude >= escapeRadius)
								{
									isInSet = false;
									iteration = k;
									break;
								}
							}
							if (isInSet)
							{
								antiAliasArray[i, j] = PeriodColour(z.Magnitude, c.Magnitude, iteration);
							}
							else
							{
								antiAliasArray[i, j] = PhaseColour(iteration);
							}
							z = zCorner +
								Complex.Multiply(Complex.ImaginaryOne,
								j * smolStep.Imaginary) +
								Complex.Multiply(Complex.One,
								i * smolStep.Real);
						}
					}
					{
						int r_i = 0;//Use int types to avoid overflow error if all values are 255
						int g_i = 0;
						int b_i = 0;
						for (int i = 0; i < rootNumSamples; i++)
						{
							for (int j = 0; j < rootNumSamples; j++)
							{
								r_i += Convert.ToInt32(antiAliasArray[j, i].R);
								g_i += Convert.ToInt32(antiAliasArray[j, i].G);
								b_i += Convert.ToInt32(antiAliasArray[j, i].B);
							}
						}
						//Console.WriteLine("[r_i,g_i,b_i] = [" + $"{r_i}" + "," + $"{g_i}" + "," + $"{b_i}" + "]");
						double r_d = Convert.ToDouble(r_i);
						double g_d = Convert.ToDouble(g_i);
						double b_d = Convert.ToDouble(b_i);
						r_d /= antiALength_d;
						g_d /= antiALength_d;
						b_d /= antiALength_d;
						//Console.WriteLine("[r_d,g_d,b_d] = [" + $"{r_d}" + "," + $"{g_d}" + "," + $"{b_d}" + "]");
						byte r = Convert.ToByte(r_d);
						byte g = Convert.ToByte(g_d);
						byte b = Convert.ToByte(b_d);
						//Console.WriteLine("[r,g,b] = [" + $"{r}" + "," + $"{g}" + "," + $"{b}" + "]");
						colourArray[x, y] = new Col3Byte(r, g, b);
					}
				}
			}
		}

		private void JUST_FIND_A_WAY_TO_MAKE_ME_HAPPY(bool[,] boolArray, int y_start, int y_end, object parmaham)
		{
			bool isInSet;
			for (int y = y_start; y < y_end; y++)
			{
				for (int x = 0; x < boolArray.GetLength(0); x++)
				{
					double x_d = Convert.ToDouble(x);
					double y_d = Convert.ToDouble(y); //Y and X global coordinates as doubles
					//Console.WriteLine("x = " + $"{x}" + ", y = " + $"{y}");
					Complex z = new Complex //Centre of pixel
					(
						imgStepX * x_d,
						imgStepY * y_d
					) + imgImageBottomLeft;
					//zCentre = Complex.Reciprocal(zCentre);	//Generates inverse set. Accidentally creates cool looking abberation due to supersampling messing up
					Complex c = z;
					isInSet = false;
					for (int k = 0; k < Convert.ToInt32(parmaham); k++)
					{
						isInSet = true;
						z = StandardMandelbrot(z,c);
						//z = Complex.Pow(z,Convert.ToDouble(parmaham)) + c;
						if (z.Magnitude >= escapeRadius)
						{
							isInSet = false;
							break;
						}
					}
					boolArray[x, y] = isInSet;
				}
			}
		}
	}
}