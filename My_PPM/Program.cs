using System;
using System.IO;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Numerics;
using System.Text;
namespace PPM_Fractal_Renderer
{
	class Program
	{
		/// <summary>
		/// Wee woo waa. An <c>ASCII</c> PPM Read/Write class so I don't have to faff about w/ IO.TextWriter directly
		/// </summary>

		public partial class My_Portable_Pix_Map
		{

			const double myTau = Math.PI * 2.0d;
			//TODO: Add support for P1 and P2 and everything other than P3. For now P3 is assumed the default pix format
			public static readonly decimal Version = 1.0M;
			private List<string> headerText = new List<string>();
			private readonly string imgComment = "# Made with PPM_Fractal_Renderer in C-Sharp";
			private readonly int colourDepth = 255;
			private readonly int greyDepth = 65535;
			private List<string> colourMapTXT = new List<string>();
			private Col3Byte[,] colourMapRGB;
			private List<byte> colourMapByte = new List<byte>();
			UInt16[,] intGreyScaleMap;  //Unit32 used, maybe lower or raise precision as needed
			int imgWidth;
			int imgHeight;
			int bddWidth;
			int bddHeight;
			const int buddhaSuperSample = 5;//oh god why am I here
			public My_Portable_Pix_Map(int width, int height)
			{
				Console.WriteLine("Using PPM_Fractal_Renderer class!");
				this.imgHeight = height;
				this.imgWidth = width;
				this.bddHeight = height * buddhaSuperSample;
				this.bddWidth = width * buddhaSuperSample;
				colourMapRGB = new Col3Byte[imgWidth, imgHeight];
				intGreyScaleMap = new UInt16[bddHeight, bddHeight];
				List<string> tempStr = new List<string>() { "P6", imgComment, $"{imgWidth}", $"{imgHeight}", $"{colourDepth}" };
				//List<string> tempStr = new List<string>() { "P5", imgComment, $"{imgWidth}", $"{imgHeight}", $"{greyDepth}" };
				this.headerText = tempStr;
				for (int i = 0; i < imgWidth; i++)
				{
					for (int j = 0; j < imgHeight; j++)
					{
						colourMapRGB[i, j] = new Col3Byte(0, 0, 0);
					}
				}

				for (int i = 0; i < bddWidth; i++)
				{
					for (int j = 0; j < bddHeight; j++)
					{
						//intGreyScaleMap[i, j] = 0;
					}
				}
			}

			public void CalcColourMapMT()
			{
				//SoftwareShader shader = new SoftwareShader(imgWidth, imgHeight, intGreyScaleMap, buddhaSuperSample);
				SoftwareShader shader = new SoftwareShader(imgWidth, imgHeight, colourMapRGB);
				Console.WriteLine("imgWidth = " + $"{imgWidth}" + ", imgHeight = " + $"{imgHeight}");
				Console.WriteLine("bddWidth = " + $"{bddWidth}" + ", bddHeight = " + $"{bddHeight}");
				shader.AllocateThreads(colourMapRGB, shader.TestFunction, 2);
				//shader.TestFunction2();

				//shader.SuperEfficientGridYaya();
				//shader.Buddha(intGreyScaleMap);
			}
			/// <summary>
			/// Uses a user-supplied """"fragment shader"""" to populate <c>colourMapRGB</c>
			/// </summary>
			public void CalcColourMap()
			{
				SoftwareShader shader = new SoftwareShader(imgWidth, imgHeight, colourMapRGB);
				Console.WriteLine("imgWidth = " + $"{imgWidth}" + ", imgHeight = " + $"{imgHeight}");
				double percentDone = 0.0d;
				for (int i = 0; i < imgWidth; i++)
				{
					for (int j = 0; j < imgHeight; j++)
					{
						/*//	v debug shader v
						this.colourMapRGB[i, j] = new Col3Byte(
							Convert.ToInt32(255.0f * ( Convert.ToSingle(i) / Convert.ToSingle(this.imgWidth)	)),
							Convert.ToInt32(255.0f * ( Convert.ToSingle(j) / Convert.ToSingle(this.imgHeight)	)),
							127 );
						percentDone = 100.0d * ((Convert.ToDouble((imgHeight * i) + j)) / Convert.ToDouble(imgWidth * imgHeight));*/
						colourMapRGB[i, j] = shader.CalcBW(i, j);
						//Console.WriteLine("Progress : " + percentDone.ToString("00.0000") + "%");
					}
				}
			}
			/// <summary>
			/// Converts the contents of <c>colourMapRGB</c> into <c>colourMapTXT</c> and then puts it in a file with the header text
			/// </summary>
			public void WriteImage()
			{
				this.colourMapTXT.AddRange(headerText);
				CalcColourMap();

				ConvertRGBtoBinary();
				Console.WriteLine("Writing file...");
				string startString = string.Empty;
				for (int j = 0; j < headerText.ToArray().Length; j++)
				{
					startString += headerText[j];
					startString += "\n";
				}
				FileStream stream = new FileStream("Output_Image1.ppm", FileMode.Create, FileAccess.Write);
				stream.Write(Encoding.UTF8.GetBytes(startString), 0, Encoding.UTF8.GetBytes(startString).Length);
				stream.Write(colourMapByte.ToArray(), 0, colourMapByte.ToArray().Length);
				stream.Close();
				//System.IO.File.WriteAllBytes("Output_Image1.ppm", colourMapByte.ToArray());
				Console.WriteLine("File written to Output_Image1.ppm!");
			}
			public void WriteImageMT()
			{
				CalcColourMapMT();
				ConvertRGBtoBinary();
				//ConvertGreyScaletoBinary();
				Console.WriteLine("Writing file...");
				string startString = string.Empty;
				for (int j = 0; j < headerText.ToArray().Length; j++)
				{
					startString += headerText[j];
					startString += "\n";
				}
				//FileStream stream = new FileStream("Output_Image3.pgm", FileMode.Create, FileAccess.Write);
				FileStream stream = new FileStream("Output_Image00.ppm", FileMode.Create, FileAccess.Write);
				stream.Write(Encoding.UTF8.GetBytes(startString), 0, Encoding.UTF8.GetBytes(startString).Length);
				stream.Write(colourMapByte.ToArray(), 0, colourMapByte.ToArray().Length);
				stream.Close();
				//System.IO.File.WriteAllBytes("Output_Image00.ppm", colourMapByte.ToArray());
				Console.WriteLine("File written to Output_Image00.ppm!");
			}
			/// <summary>
			/// Experimental multiframe thing. Use in conjunction with like ffpmeg or smthn
			/// </summary>
			public void WriteImageVideoMT()//ughh we need to reset the text string EVERy time yuck
			{
				//Ideally I'd like a way to normalise the span of whatever it is you that you can set an
				//arbritrary numer of frames and it interpolates??
				int startFrame = 0;
				int currentFrame;
				int endFrame = 960;
				double d;
				double startEx = -16.0d;
				double maxEx = +16.0d;

				double exPerFrames = (maxEx - startEx) / Convert.ToDouble(endFrame - startFrame);
				for (int i = startFrame; i < endFrame; i++)
				{
					//d = startEx + ((maxEx - startEx) * (Convert.ToDouble(i - startFrame) / Convert.ToDouble(endFrame - startFrame)));
					d = startEx + (exPerFrames * i);
					Console.WriteLine("Convert.ToDouble(i) / Convert.ToDouble(endFrame - startFrame) = " + $"{Convert.ToDouble(i) / Convert.ToDouble(endFrame - startFrame)}");

					colourMapTXT = new List<string>();
					colourMapByte = new List<byte>();
					Console.WriteLine("i = " + $"{i}" + ", d = " + $"{d}");
					colourMapTXT.AddRange(headerText);
					CalcColourMapVideoMT(d);
					ConvertRGBtoBinary();
					string filePath = "./Frames/frame_" + i.ToString("00000") + ".ppm";
					string startString = string.Empty;
					for (int j = 0; j < headerText.ToArray().Length; j++)
					{
						startString += headerText[j];
						startString += "\n";
					}
					FileStream stream = new FileStream(filePath, FileMode.Create);
					stream.Write(Encoding.UTF8.GetBytes(startString), 0, Encoding.UTF8.GetBytes(startString).Length);
					stream.Write(colourMapByte.ToArray(), 0, colourMapByte.ToArray().Length);
					//System.IO.File.WriteAllBytes(filePath, colourMapByte.ToArray());
				}
			}
			public void Test()
			{
				SoftwareShader shader = new SoftwareShader(imgWidth, imgHeight, colourMapRGB);
				ColArrayBlackImage();
				//shader.TestWriteGrid(colourMapRGB);
				ConvertRGBtoBinary();//Test!!!
				Console.WriteLine("Writing file...");
				string startString = string.Empty;
				for (int j = 0; j < headerText.ToArray().Length; j++)
				{
					startString += headerText[j];
					startString += "\n";
				}
				FileStream stream = new FileStream("Output_Image00.ppm", FileMode.Create);
				stream.Write(Encoding.UTF8.GetBytes(startString), 0, Encoding.UTF8.GetBytes(startString).Length);
				stream.Write(colourMapByte.ToArray(), 0, colourMapByte.ToArray().Length);
			}
			public void CalcColourMapVideoMT(object parmaham)
			{
				SoftwareShader shader = new SoftwareShader(imgWidth, imgHeight, colourMapRGB);
				Console.WriteLine("imgWidth = " + $"{imgWidth}" + ", imgHeight = " + $"{imgHeight}");
				//	shader.AllocateThreads(parmaham);//Commented out 8.1.22
			}

			public void ColArrayBlackImage()
			{
				for (int i = 0; i < imgWidth; i++)
				{
					for (int j = 0; j < imgHeight; j++)
					{
						colourMapRGB[i, j] = new Col3Byte(0, 0, 0);
					}
				}
			}
			public void ColArrayWhiteImage()
			{
				for (int i = 0; i < imgWidth; i++)
				{
					for (int j = 0; j < imgHeight; j++)
					{
						colourMapRGB[i, j] = new Col3Byte(255, 255, 255);
					}
				}
			}
			public void ColArrayDebug()
			{
				for (int i = 0; i < imgWidth; i++)
				{
					for (int j = 0; j < imgHeight; j++)
					{
						colourMapRGB[i, j] = new Col3Byte(
						Convert.ToInt32(255.0f * (Convert.ToSingle(i) / Convert.ToSingle(this.imgWidth))),
						Convert.ToInt32(255.0f * (Convert.ToSingle(j) / Convert.ToSingle(this.imgHeight))),
						127);
					}
				}
			}

			/// <summary>
			/// Converts <c>colourMapRGB</c> to <c>colourMapTXT</c>
			/// </summary>
			private void ConvertRGBtoTXT()
			{
				for (int j = 0; j < colourMapRGB.GetLength(1); j++)
				{
					for (int i = 0; i < colourMapRGB.GetLength(0); i++)
					{
						string tempStr = string.Empty;
						if (colourMapRGB[i, j] != null)
						{
							for (int k = 0; k < colourMapRGB[i, j].RGBArray.Length; k++)
							{
								tempStr += Convert.ToString(colourMapRGB[i, j].RGBArray[k]);
								if (k == colourMapRGB[i, j].RGBArray.Length - 1) { }
								else
								{
									tempStr += " ";
								}
							}
							colourMapTXT.Add(tempStr);
						}
						else
						{
							Console.WriteLine("colourMapRGB[i,j] is null! .. = { "
								+ Convert.ToString(colourMapRGB[i, j].RGBArray[0]) +
								", " + Convert.ToString(colourMapRGB[i, j].RGBArray[1]) +
								", " + Convert.ToString(colourMapRGB[i, j].RGBArray[2]) + " }");
						}
					}
				}
			}
			private void ConvertRGBtoBinary() //This takes a while, maybe try and make it more efficient?
			{
				colourMapByte.Clear();//
				for (int j = 0; j < colourMapRGB.GetLength(1); j++)
				{
					for (int i = 0; i < colourMapRGB.GetLength(0); i++)
					{
						//byte[] tempBytes = new byte[3];
						/*
						string tempStr = string.Empty;
						if (colourMapRGB[i, j] != null)
						{
							for (int k = 0; k < colourMapRGB[i, j].RGBArray.Length; k++)
							{
								tempStr += Convert.ToString(colourMapRGB[i, j].RGBArray[k]);
								if (k == colourMapRGB[i, j].RGBArray.Length - 1) { }
								else
								{
									tempStr += " ";
								}
							}
							tempStr += "\n";
							*/
						//tempBytes = colourMapRGB[i, j].RGBArray;
						colourMapByte.AddRange(colourMapRGB[i, j].RGBArray);
						//colourMapByte.AddRange(Encoding.ASCII.GetBytes(tempStr));//Maybe comment out this line??

					}
				}
			}
			private void ConvertGreyScaletoBinary() //This takes a while, maybe try and make it more efficient?
			{
				colourMapByte.Clear();//
				UInt16 highestValue = 0;
				Complex highestCoord = new Complex();
				if (buddhaSuperSample == 1)
				{
					for (int i = 0; i < intGreyScaleMap.GetLength(0); i++)
					{
						for (int j = 0; j < intGreyScaleMap.GetLength(1); j++)
						{
							if (intGreyScaleMap[i, j] > highestValue)
							{
								highestValue = intGreyScaleMap[i, j];
								highestCoord = new Complex(i, j);
							}
						}
					}
				}
				else
				{
					for (int i = 0; i < intGreyScaleMap.GetLength(0); i += buddhaSuperSample)
					{
						for (int j = 0; j < intGreyScaleMap.GetLength(1); j += buddhaSuperSample)
						{
							double count_d = 0.0d;
							for (int k = 0; k < buddhaSuperSample; k++)
							{
								for (int l = 0; l < buddhaSuperSample; l++)
								{
									//Console.WriteLine($"[i+k,i+l] = [{i+k},{j+l}]");
									count_d += Convert.ToDouble(intGreyScaleMap[i + k, j + l]);
								}
							}
							count_d /= buddhaSuperSample * buddhaSuperSample;
							if (Convert.ToUInt16(count_d) > highestValue)
							{
								highestValue = Convert.ToUInt16(Math.Ceiling(count_d));
								highestCoord = new Complex(i, j);
							}
						}
					}
				}
				Console.WriteLine($"Highest value = {highestValue}");
				Console.WriteLine($"Highest coord = {highestCoord}");
				double highestValue_d = Convert.ToDouble(highestValue);
				if (buddhaSuperSample == 1)
				{
					for (int j = 0; j < intGreyScaleMap.GetLength(1); j++)
					{
						for (int i = 0; i < intGreyScaleMap.GetLength(0); i++)
						{
							double value_d = Convert.ToDouble(greyDepth) * Math.Pow((Convert.ToDouble(intGreyScaleMap[i, j]) / highestValue_d), 0.5d); //a lil uneasy bout this
																																																																				 //Col3Byte col = new Col3Byte(value_d, value_d, value_d);
							UInt16 value = Convert.ToUInt16(value_d);
							byte[] intBytes = new byte[2];
							intBytes[0] = (byte)(value >> 8);
							intBytes[1] = (byte)value;
							colourMapByte.AddRange(intBytes);
						}
					}
				}
				else
				{
					for (int j = 0; j < intGreyScaleMap.GetLength(1); j += buddhaSuperSample)
					{
						for (int i = 0; i < intGreyScaleMap.GetLength(0); i += buddhaSuperSample)
						{
							byte[] intBytes = new byte[2];
							double value_d = 0.0d;
							for (int k = 0; k < buddhaSuperSample; k++)
							{
								for (int l = 0; l < buddhaSuperSample; l++)
								{
									//Console.WriteLine($"[i+k,i+l] = [{i+k},{j+l}], val = {intGreyScaleMap[i + k, j + l]}, val_d = {Convert.ToDouble(intGreyScaleMap[i + k, j + l]) / highestValue_d}");
									value_d += Math.Sqrt(Convert.ToDouble(intGreyScaleMap[i+k,j+l]));
								}
							}
							UInt16 value_16 = Convert.ToUInt16(Math.Floor(Convert.ToDouble(greyDepth) * (value_d / Convert.ToDouble(highestValue_d * buddhaSuperSample * buddhaSuperSample))));
							//Console.WriteLine($"value_d = {value_d}");
							//Console.WriteLine($"value_16 = {value_16}");
							intBytes[0] = (byte)(value_16 >> 8);
							intBytes[1] = (byte)value_16;
							//Console.WriteLine($"intBytes[0] = {intBytes[0]}, intBytes[1] = {intBytes[1]}");
							colourMapByte.AddRange(intBytes);
						}
					}
				}
			}
		}
		static void Main()
		{
			My_Portable_Pix_Map Ppm = new My_Portable_Pix_Map(4096, 4096);
			var timer2 = new Stopwatch();
			Console.WriteLine("MT Start!");
			timer2.Start();
			Ppm.WriteImageMT();
			timer2.Stop();
			TimeSpan timeTaken2 = timer2.Elapsed;
			string timestring = "Time taken: " + timeTaken2.ToString(@"m\:ss\.fff");
			Console.WriteLine(timestring);
		}
	}
}