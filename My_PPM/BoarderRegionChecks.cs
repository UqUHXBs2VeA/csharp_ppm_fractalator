using System;
using System.Collections.Generic;

using System.Text;
using System.IO;

namespace PPM_Fractal_Renderer
{
	public partial class SoftwareShader
	{
		public void DebugBoolGrid(bool[,] array)
		{
			Col3Byte trueColour = new Col3Byte(0, 127, 255);
			Col3Byte falseColour = new Col3Byte(15, 15, 15);
			for (int x = 0; x < colourMapRGB.GetLength(0); x++)
			{
				for (int y = 0; y < colourMapRGB.GetLength(1); y++)
				{
					if (array[x, y] == true)
					{
						colourMapRGB[x, y] = trueColour;
					}
					else
					{
						colourMapRGB[x, y] = falseColour;
					}
				}
			}
		}
		public void DebugBoolGrid(bool[,] array1, bool[,] array2)
		{
			Col3Byte col1T2T = new Col3Byte(0, 127, 127);
			Col3Byte col1T2F = new Col3Byte(0, 255, 63);
			Col3Byte col1F2T = new Col3Byte(0, 63, 255);
			Col3Byte col1F2F = new Col3Byte(127, 0, 63);
			for (int x = 0; x < colourMapRGB.GetLength(0); x++)
			{
				for (int y = 0; y < colourMapRGB.GetLength(1); y++)
				{
					if (array1[x, y] && array2[x, y])
					{
						colourMapRGB[x, y] = col1T2T;
					}
					else if (!array1[x, y] && array2[x, y])
					{
						colourMapRGB[x, y] = col1F2T;
					}
					else if (array1[x, y] && !array2[x, y])
					{
						colourMapRGB[x, y] = col1T2F;
					}
					else
					{
						colourMapRGB[x, y] = col1F2F;
					}
				}
			}
		}
		public void DebugBoolToCol(bool[,] array)
		{

			Col3Byte tCol = new Col3Byte(255, 255, 255);
			Col3Byte fCol = new Col3Byte(0, 0, 0);
			for (int x = 0; x < colourMapRGB.GetLength(0); x++)
			{
				for (int y = 0; y < colourMapRGB.GetLength(1); y++)
				{
					if (array[x, y])
					{
						colourMapRGB[x,y] = tCol;
					}
					else
					{
						colourMapRGB[x, y] = fCol;
					}
				}
			}
		}
		public void DebugBoolGridFramed(bool[,] array1, bool[,] array2, int index)
		{
			Col3Byte col1T2T = new Col3Byte(0, 127, 127);
			Col3Byte col1T2F = new Col3Byte(0, 255, 63);
			Col3Byte col1F2T = new Col3Byte(0, 63, 255);
			Col3Byte col1F2F = new Col3Byte(127, 0, 63);
			for (int x = 0; x < colourMapRGB.GetLength(0); x++)
			{
				for (int y = 0; y < colourMapRGB.GetLength(1); y++)
				{
					if (array1[x, y] && array2[x, y])
					{
						colourMapRGB[x, y] = col1T2T;
					}
					else if (!array1[x, y] && array2[x, y])
					{
						colourMapRGB[x, y] = col1F2T;
					}
					else if (array1[x, y] && !array2[x, y])
					{
						colourMapRGB[x, y] = col1T2F;
					}
					else
					{
						colourMapRGB[x, y] = col1F2F;
					}
				}
			}
			List<string> bloreblre = new List<string>() { "P6", "#ass blast usa", $"{imgWidth}", $"{imgHeight}", "255" };
		
			Console.WriteLine("Writing file...");
			List<byte> colourMapByte = new List<byte>();
			for (int j = 0; j < colourMapRGB.GetLength(1); j++)
			{
				for (int i = 0; i < colourMapRGB.GetLength(0); i++)
				{
					byte[] tempBytes = new byte[3];
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
						tempBytes = colourMapRGB[i, j].RGBArray;
						colourMapByte.AddRange(tempBytes);
						//colourMapByte.AddRange(Encoding.ASCII.GetBytes(tempStr));//Maybe comment out this line??
					}
				}
			}
			string startString = string.Empty;
			for (int j = 0; j < bloreblre.ToArray().Length; j++)
			{
				startString += bloreblre[j];
				startString += "\n";
			}
			FileStream stream = new FileStream($"DebugFrame_{index.ToString("D4")}.ppm", FileMode.Create, FileAccess.Write);
			stream.Write(Encoding.UTF8.GetBytes(startString), 0, Encoding.UTF8.GetBytes(startString).Length);
			stream.Write(colourMapByte.ToArray(), 0, colourMapByte.ToArray().Length);
			stream.Close();
			//System.IO.File.WriteAllBytes("Output_Image3.ppm", colourMapByte.ToArray());
			Console.WriteLine($"File written to DebugFrame_{index.ToString("D4")}.ppm!");
		}//Haha oh god this is so bad
	
		public bool IsBoolArrayRegionUniform(int xStart, int yStart, int boxSize, bool[,] array)
		{
			//This uses what I'm calling anti-Mersenne standard for what counts as a "block" and its "boarder"
			//Basically, for exponent n, the boarder has 2^e + 1 elements on each size (including conrers)
			//And spans from 0 to 2^e (using 0 indexing)
			//The "region" also contains this boarder, for now
			//boxSize refers to 2^e, boxWidth is 2^e + 1
			bool start = array[xStart, yStart];
			for (int x = xStart; x < xStart + boxSize + 1; x++)
			{
				for (int y = yStart; y < yStart + boxSize + 1; y++)
				{
					if (array[x, y] != start)
					{
						return false;
					}
				}
			}
			return true;
		}
		public bool IsBoolArrayBoarderUniform(int x, int y, int boxSize, bool[,] array)
		{
			//Console.Write($"<{boxSize}>");
			int boxWidth = boxSize + 1;
			bool[] boarderIsInSet = new bool[(boxSize - 1) * 2 + boxWidth * 2];
			for (int j = 0; j < boxSize; j++)
			{
				//Console.Write($"({x + j},{y})");
				boarderIsInSet[j] = array[x + j, y];
			}
			for (int j = 0; j < boxSize; j++)
			{
				//Console.Write($"({x + boxSize},{y + j})");
				boarderIsInSet[boxSize + j] = array[x + boxSize, y + j];
			}
			for (int j = 0; j < boxSize; j++)
			{
				//Console.Write($"({x + boxSize - j},{y + boxSize})");
				boarderIsInSet[2 * boxSize + j] = array[x + boxSize - j, y + boxSize];
			}
			for (int j = 0; j < boxSize; j++)
			{
				//Console.Write($"({x},{y + boxSize - j})");
				boarderIsInSet[3 * boxSize + j] = array[x, y + boxSize - j];
			}
			bool startBool = true;
			bool isBoarderUniform = true;
			//Console.Write("\n[");
			for (int i = 0; i < boarderIsInSet.Length; i++)
			{

				string str;
				if (boarderIsInSet[i] == true)
				{
					str = "1";
				}
				else
				{
					str = "0";
				}
				//Console.Write(str);
			}
			//Console.Write("]");
			//Console.Write(", x = " + $"{x}" + ", y = " + $"{y}");
			for (int i = 0; i < boarderIsInSet.Length; i++)
			{
				if (i == 0)
				{
					startBool = boarderIsInSet[0];
				}
				else
				{
					if (boarderIsInSet[i] == startBool && isBoarderUniform)
					{
						isBoarderUniform = true;
					}
					else
					{
						isBoarderUniform = false;
					}
				}
			}
			//Console.WriteLine(", isBoarderUniform = " + $"{isBoarderUniform}");
			if (isBoarderUniform)
			{
				//if (startBool == true) { Console.Write(", block is in set"); }
				//else { Console.Write(", block isn't in set"); }
			}
			return isBoarderUniform;
		}
	}
}
