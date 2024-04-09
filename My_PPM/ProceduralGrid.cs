using System;
using System.Threading;
using System.Numerics;
using System.Diagnostics;
using System.Collections.Generic;
/*
XX[][][][][][][][]
[]XX[]..[]......[]
[][]XX..[]......[]
[]....XX[]......[]
[][][][]XX......[]
[]........XX....[]
[]..........XX..[]
[]............XX[]
[][][][][][][][]XX
*/


namespace PPM_Fractal_Renderer
{
	public partial class SoftwareShader
	{
		/*
		Okay
		So
		I think the best thing for now (at least in terms of convenience and code readability)
		is to create a unified isInSetStencil system
		perhaps even be able to read/write binary pbm files if RAM is low enough for
		holding multiple isInSetStencils + an RGB map to be an issue.
		Anyway, for now I'll just concern myself with various things.
		The current idea would be (with the Bhuddabrot in mind):
			- First iterate a few times to isolate an area which deffo isn't in the
				set and won't contribute to the buddhabrot

			- Create an in/out isInSetStencil out of that, then use that with the procedural
				grid algorithm to create another isInSetStencil which is the inner area of the mandelbrot set

			- Add the two to find a isInSetStencil of values to iterate over (tightly close to the set but not too far in it,
				and not too far out to not contribute at all)

			- Then iterate over each of those.

		A convenient side effect of this is that it divorces the grid of values being iterated and the final resolution
		Normally that would suck absolute ass for a render, but since the Bhuddabrot "resolution"
		is proportional to the number of points sampled, which might not necessarily be the number of points
		of the final image.

		Perhaps I should calculate the resolution of the final image based off the number of points sampled instead???
		I don't know. I'll just focus on the isInSetStencil side of things for now.
		*/
		bool[,] outOfSetStencil;//A few (???) iterations of the mandelbrot set
		bool[,] innerAreaStencil;//Inner part of the set, expensive and useless to render as buddha
		bool[,] pixelsToBeIteratred;//The difference of the two above, da good stuff used during rendering
		bool[,] subDivAreaStencil;//Stencil of bits to be subdivided, can be discarded
		bool[,] debugBoarderStencil;//dunno lol
		static bool[,] pixelsToBeCheckedIfInSet;//True - Pixels known to be one or other, False - Pixels unknown
		static bool[,] pixelsToBeSubdivided;
		List<(int, int, int)> boardersOfBlocks = new List<(int, int, int)>();	//x,y,exponent
		/*

		public static void STCheckIfBlocksAreInSet(bool[,] ugh, List<(int, int, int)> listOfBlocks)
		{
			for(int index = 0; index < listOfBlocks.Count; index++)
			{
				int x = listOfBlocks[index].Item1;
				int y = listOfBlocks[index].Item2;
				int boxSize = Convert.ToInt32(Math.Pow(2.0d,Convert.ToDouble(listOfBlocks[index].Item3)));
				int boxWidth = boxSize + 1;
				bool[] boarderIsInSet = new bool[4 * boxSize];
				if (x + boxSize >= ugh.GetLength(0) || y + boxSize >= ugh.GetLength(1)) { Console.WriteLine("eek!"); }
				else
				{
					for (int j = 0; j < boxSize; j++)
					{
						Console.WriteLine("x = " + $"{x + j}" + ", y = " + $"{y}");
						boarderIsInSet[j] = ugh[x + j, y];
						if (boarderIsInSet[j] == true)
						{
							//colourMapRGB[x + j, y] = new Col3Byte(127, 0, 127);
						}
					}
					for (int j = 0; j < boxSize; j++)
					{
						Console.WriteLine("x = " + $"{x + boxSize}" + ", y = " + $"{y + j}");
						boarderIsInSet[boxSize + j] = ugh[x + boxSize, y + j];
						if (boarderIsInSet[boxSize + j] == true)
						{
							//colourMapRGB[x + boxSize, y + j] = new Col3Byte(0, 0, 191);
						}
					}
					for (int j = 0; j < boxSize; j++)
					{
						Console.WriteLine("x = " + $"{x + boxSize - j}" + ", y = " + $"{y + boxSize}");
						boarderIsInSet[2 * boxSize + j] = ugh[x + boxSize - j, y + boxSize];
						if (boarderIsInSet[2 * boxSize + j] == true)
						{
							//colourMapRGB[boxSize - j, y + boxSize] = new Col3Byte(0, 127, 127);
						}
					}
					for (int i = 0; i < boxSize; i++)
					{
						Console.WriteLine("x = " + $"{x}" + ", y = " + $"{y + boxSize - i}");
						boarderIsInSet[3 * boxSize + i] = ugh[x, y + boxSize - i];
						if (boarderIsInSet[3 * boxSize + i] == true)
						{
							//colourMapRGB[x, y + boxSize - i] = new Col3Byte(0, 191, 0);
						}
					}
				bool startBool = true;
				bool isBoarderUniform = true;
				for (int i = 0; i < boarderIsInSet.Length; i++)//Debugging loop
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
					Console.Write(str);
				}
				Console.Write(", x = " + $"{x}" + ", y = " + $"{y}");
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
				Console.WriteLine("isBoarderUniform = " + $"{isBoarderUniform}");

				if (isBoarderUniform)//fill block with that thing
				{
					if (startBool == true) { Console.WriteLine(", block is in set"); }
					else { Console.WriteLine(", block isn't in set"); }
					for (int i = 0; i < boxSize; i++)
					{
						for (int j = 0; j < boxSize; j++)
						{
							ugh[x + i, y + j] = startBool;
						}
					}
				}
				else//subdivide
				{
					int halfSize = Convert.ToInt32(Convert.ToDouble(listOfBlocks[index].Item3));
					int j = listOfBlocks[index].Item3;
					for(int i = listOfBlocks[index].Item3; i > 0; i--)
					{
						if(halfSize + x > ugh.GetLength(0) || halfSize + y > ugh.GetLength(1))
						{
							halfSize /= 2;
							j--;
						}
					}
					listOfBlocks.RemoveAt(index);
					listOfBlocks.Add((x, y, j));
					listOfBlocks.Add((x + halfSize, y, j));
					listOfBlocks.Add((x, y + halfSize, j));
					listOfBlocks.Add((x + halfSize, y + halfSize, j));
					}
				}
			}
		}

		public void STCheckWholeGrid(int exponent, int boxSize)
		{
			for (int x = 0; x < imgWidth; x++)
			{
				for (int y = 0; y < imgHeight; y++)
				{
					if (((x % Convert.ToInt32(Math.Pow(2.0d, exponent)) == 0)) && ((y % Convert.ToInt32(Math.Pow(2.0d, exponent))) == 0))
					{
						if (colourMapRGB.GetLength(0) <= x + boxSize || colourMapRGB.GetLength(1) <= y + boxSize)
						{
							Console.WriteLine("Edge reached!!!");
						}
						else
						{
							bool[] boarderIsInSet = new bool[4 * boxSize];
							for (int j = 0; j < boxSize; j++)
							{
								Console.WriteLine("x = " + $"{x + j}" + ", y = " + $"{y}");
								boarderIsInSet[j] = pixelsInSet[x + j, y];
								if (boarderIsInSet[j] == true)
								{
									//colourMapRGB[x + j, y] = new Col3Byte(127, 0, 127);
								}
							}
							for (int j = 0; j < boxSize; j++)
							{
								Console.WriteLine("x = " + $"{x + boxSize}" + ", y = " + $"{y + j}");
								boarderIsInSet[boxSize + j] = pixelsInSet[x + boxSize, y + j];
								if (boarderIsInSet[boxSize + j] == true)
								{
									//colourMapRGB[x + boxSize, y + j] = new Col3Byte(0, 0, 191);
								}
							}
							for (int j = 0; j < boxSize; j++)
							{
								Console.WriteLine("x = " + $"{x + boxSize - j}" + ", y = " + $"{y + boxSize}");
								boarderIsInSet[2 * boxSize + j] = pixelsInSet[x + boxSize - j, y + boxSize];
								if (boarderIsInSet[2 * boxSize + j] == true)
								{
									//colourMapRGB[boxSize - j, y + boxSize] = new Col3Byte(0, 127, 127);
								}
							}
							for (int i = 0; i < boxSize; i++)
							{
								Console.WriteLine("x = " + $"{x}" + ", y = " + $"{y + boxSize - i}");
								boarderIsInSet[3 * boxSize + i] = pixelsInSet[x, y + boxSize - i];
								if (boarderIsInSet[3 * boxSize + i] == true)
								{
									//colourMapRGB[x, y + boxSize - i] = new Col3Byte(0, 191, 0);
								}
							}
							bool startBool = true;
							bool isBoarderUniform = true;
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
								Console.Write(str);
							}
							Console.Write(", x = " + $"{x}" + ", y = " + $"{y}");
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
							Console.Write("isBoarderUniform = " + $"{isBoarderUniform}");
							if (startBool == true) { Console.WriteLine(", block is in set"); }
							else { Console.WriteLine(", block isn't in set"); }
							if (isBoarderUniform)//fill block with that thing, either confirmed false or true
							{
								for (int i = 0; i < boxSize; i++)
								{
									for (int j = 0; j < boxSize; j++)
									{
										pixelsInSet[x + i, y + j] = startBool;
									}
								}
							}
							else//subdivide
							{
								for (int i = 1; i < boxSize; i++)
								{
									for (int j = 1; j < boxSize; j++)
									{
										pixelsToBeSubdivided[x + i, y + j] = true;
									}
								}
							}
						}
					}
				}
			}
		}
		public void STCreateGridFromStart(bool[,] pixelsInSet, int exponentHighest)
		{
			for (int exponent = exponentHighest; exponent >= 1; exponent--)
			{
				Console.WriteLine("exponent = " + $"{exponent}");
				int boxSize = Convert.ToInt32(Math.Pow(2.0d, exponent));
				for (int x = 0; x < pixelsInSet.GetLength(0); x++)
				{
					for (int y = 0; y < pixelsInSet.GetLength(1); y++)
					{
						if (((x % Convert.ToInt32(Math.Pow(2.0d, exponent)) == 0)) || ((y % Convert.ToInt32(Math.Pow(2.0d, exponent))) == 0))//Cell found
						{
							Console.WriteLine("Cell found for exponent " + exponent.ToString());
							boardersOfBlocks.Add((x,y,exponent));
							if (x + 1 >= imgWidth || y + 1 >= imgHeight) { Console.WriteLine("Escaped image!"); break; }//delete me!!!
							pixelsInSet[x,y] = CalcForComplexCoord(x, y, MandelbrotVoid);	//Did it like this so if you're using something like the burning ship, you just change the fn
						}
					}
				}
			}
		}*/
		/*
		public void STCreateGridFromGrid(List<(int, int, int)> listOfBlocks)
		{
			for(int index = 0; index < listOfBlocks.Count;index++)
			{
				int x = listOfBlocks[index].Item1;
				int y = listOfBlocks[index].Item2;
				int exponent = listOfBlocks[index].Item3;
				int boxSize = Convert.ToInt32(Math.Pow(2.0d, exponent));
				for(int i = 0; i < boxSize; i++)
				{
					for (int j = 0; i < boxSize; i++)
					{
						if((j%boxSize != 0 ) || (i%boxSize!= 0)) { } else
						{

						}
					}
				}
				pixelsInSet[x, y] = CalcForComplexCoord(x, y, MandelbrotVoid);  //Did it like this so if you're using something like the burning ship, you just change the fn
			}
		}
		*/
		public void STCheckGridFromDet(bool[,] detArray, bool[,] pixArray, int eHigh)
		{
			for (int exponent = eHigh; exponent >= 0 ; exponent--)
			{
				bool allDet = false;
				Console.WriteLine("exponent = " + $"{exponent}");
				int boxSize = Convert.ToInt32(Math.Pow(2.0d, exponent));
				Console.WriteLine("boxSize = " + boxSize.ToString());
				for (int x = 0; x < bddWidth; x++)
				{
					for (int y = 0; y < bddHeight; y++)
					{
						if (((x % boxSize == 0)) && ((y % boxSize == 0)))//Cell found
						{
							//Console.WriteLine("Cell found for x = " + x.ToString() + ", y = " + y.ToString());
							allDet = false; //Should be false anyway but can't do any harm
							if (((x + boxSize + 1 >= bddWidth || y + boxSize + 1 >= bddHeight)))
							//if (((x + boxSize >= imgWidth || y + boxSize >= imgHeight) || (x >= imgWidth || y >= imgHeight)))
							{
								//Console.WriteLine($"Escaped image for x = {x}, y = {y}");
							}
							else
							{
								//if (IsBoolArrayBoarderUniform(x, y, boxSize, detArray) && detArray[x, y] == true)//< this leads to weird stuff but works it out in the end, might be more efficient
								if (IsBoolArrayRegionUniform(x, y, boxSize, detArray) && detArray[x, y] == true)//all pixels detted, skip this block
									{
									//Console.Write("doot-");
									allDet = true;  //Skips the next code block as the whole region is known to be either in/out of set
								}
								else if (IsBoolArrayBoarderUniform(x, y, boxSize, detArray) && detArray[x, y] == false)//none detted
								{
									//Console.WriteLine("None detted!");]
									//CalcForComplexBoarder(x, y, boxSize, MandelbrotVoid, pixArray);
									for (int i = 0; i < boxSize + 1; i++)
									{
										for (int j = 0; j < boxSize + 1; j++)
										{
											//Console.WriteLine($"i = {i}, j = {j}, logic = {((i % boxSize == 0) | (j % boxSize) == 0)}");
											if ((i % boxSize == 0) || (j % boxSize) == 0)
											{
												detArray[x + i, y + j] = true;
												MandelbrotVoidArray(x + i, y + j, pixArray);
											}
										}
									}
								}
								else if (!IsBoolArrayBoarderUniform(x, y, boxSize, detArray)) //Boarder is not all determined to be in set.
								{
									//CalcForComplexBoarder(x, y, boxSize, MandelbrotVoid, pixArray);
									for (int i = 0; i < boxSize + 1; i++)
									{
										for (int j = 0; j < boxSize + 1; j++)
										{
											if (!detArray[x + i, y + j] && (i % boxSize == 0) || (j % boxSize) == 0)
											{
												detArray[x + i, y + j] = true;
												MandelbrotVoidArray(x + i, y + j, pixArray);
											}
										}
									}
								}

								//Next we check if the boarder of the block we just encountered is unifrom
								if (allDet)
								{
								}
								else if ((detArray[x, y] == true) && IsBoolArrayBoarderUniform(x, y, boxSize, detArray) && IsBoolArrayBoarderUniform(x, y, boxSize, pixArray))
								//All boarder pixels determnined and in set
								{
									bool startBool = pixArray[x, y];
									//Console.WriteLine($"Good block found! startBool = {startBool}");
									for (int i = 0; i < boxSize + 1; i++)
									{
										for (int j = 0; j < boxSize + 1; j++)
										{
											pixArray[x + i, y + j] = startBool;
											detArray[x + i, y + j] = true;
										}
									}
								}
							}
						}
					}
				}
				//DebugBoolGridFramed(pixelsInSet, pixelsDetermined, boxSize);
			}
			/*
			int counter = 0;
			foreach (bool b in detArray)
			{
				if (b == false)
				{
					counter++;
				}
			}
			Console.WriteLine("Counter = " + counter.ToString());
			*/
		}
		public void TestFunction()
		{
			bool[,] testArray = new bool[9, 9];
			for(int i = 0; i < testArray.GetLength(0); i++)
			{
				for(int j = 0; j < testArray.GetLength(0); j++)
				{
					testArray[i, j] = false;
				}
			}
			testArray[0, 0] = true;
			Console.WriteLine($"Result = {IsBoolArrayBoarderUniform(0, 0, 7, testArray)}");
		}
		public void SuperEfficientGridYaya()
		{
			/*
			for (int x = 0; x < imgWidth; x++)
			{
				for (int y = 0; y < imgHeight; y++)
				{
					//initial sweep
					pixelsDetermined[x, y] = false;
					pixelsInSet[x, y] = false;
				}
			}
			*/
			int exponentHighest;
			if (bddHeight > bddWidth)
			{
				exponentHighest = Convert.ToInt32(Math.Floor(Math.Log2(bddWidth_d)));
				if (exponentHighest > bddWidth_d / 2.0d)
				{
					exponentHighest--;
				}
			}
			else
			{
				exponentHighest = Convert.ToInt32(Math.Floor(Math.Log2(bddHeight_d)));
				if (exponentHighest > bddHeight_d / 2.0d)
				{
					exponentHighest--;
				}
			}
			//exponentHighest = 10;//override for large renders, pls remove if not needed!!
			bool areAllPixelsFound = false;
			while(areAllPixelsFound == false)
			{
				STCheckGridFromDet(pixelsDetermined,pixelsInSet,exponentHighest);

				/*
				for (int x = 0; x < imgWidth; x++)
				{
					for (int y = 0; y < imgHeight; y++)
					{
						areAllPixelsFound = true;
						if (pixelsDetermined[x, y] == false)
						{
							Console.WriteLine("Not all pixels determined!");
							areAllPixelsFound = false;
							break;
						}
					}
					if (areAllPixelsFound == false)
					{
						break;
					}
				}
				*/

				Console.WriteLine("DEGUB");//otherwise the program will run forever but its so poorly optimised I wouldn't be able to tell.
				break;//debug
			}
			if(areAllPixelsFound == true)
			{
				Console.WriteLine("All pixels determined!");
			}
			else
			{
				Console.WriteLine("Pixels not yet determined");
			}

			Console.WriteLine("Finished Grid Checking!");
			//DebugBoolToCol(pixelsInSet);
		}
		/*
			Also make a method to make an image from a stencil for debugging!!!
		*/
		public void CreateOuterStencil(bool[,] outOfSetStencil)
		{
			AllocateThreads(outOfSetStencil, CheckWhetherIsInSet, outerIterations);
		}
		public void CreateInnerStencil(bool[,] innerAreaStencil)
		{
			AllocateThreads(innerAreaStencil, CheckWhetherIsInSet, interiorIterations);
		}

		public static Action<bool[,], int, int, object> CheckWhetherIsInSet = (isInSetStencil, x, y, iterations) =>
		{
			double x_d = Convert.ToDouble(x);
			double y_d = Convert.ToDouble(y); //Y and X global coordinates as doubles
			double width_d = Convert.ToDouble(isInSetStencil.GetLength(0));
			double height_d = Convert.ToDouble(isInSetStencil.GetLength(1));
			Complex z = new Complex
			(
				imgImagX * (x_d / width_d),
				imgImagY * (y_d / height_d)
			) + imgImageBottomLeft;
			bool isInSet;
			Complex c = z;
			int iteration = 0;
			isInSet = false;
			for (int k = 0; k < Convert.ToInt32(iterations); k++)
			{
				isInSet = true;
				z = Complex.Pow(z, 2.0d) + c;
				if (z.Magnitude >= escapeRadius)
				{
					isInSet = false;
					break;
				}
				isInSetStencil[x, y] = isInSet;
			}
		};

		public Action<bool[,], int, int, object> CheckInnerStencilGrid = (innerAreaStencil, x, y, parmaham) =>
		{
			int exponentHighest;
			int width = innerAreaStencil.GetLength(0);
			int height = innerAreaStencil.GetLength(1);
			if (height > width)
			{
				exponentHighest = Convert.ToInt32(Math.Floor(Math.Log2(width)));
			}
			else
			{
				exponentHighest = Convert.ToInt32(Math.Floor(Math.Log2(height)));
			}
			for (int exponent = exponentHighest; exponent >= 1; exponent--) //Test whether exponent >= 0 works out!!!!!!!!!!!!!!!!!!!
			{
				Console.WriteLine("exponent = " + $"{exponent}");
				int boxSize = Convert.ToInt32(Math.Pow(2.0d, exponent));
				Console.WriteLine("boxSize = " + $"{boxSize}");
				if (((x % Convert.ToInt32(Math.Pow(2.0d, exponent)) == 0)) || ((y % Convert.ToInt32(Math.Pow(2.0d, exponent))) == 0)) //ideally should always = true by the time this code is called but it can't help to check
				{
					CheckWhetherIsInSet(innerAreaStencil, x, y, parmaham);
				}
			}
		};
		
		/*
				public Action<bool[,], int, int, object> WriteInnerStencilArea = (isInSetStencil, x, y, parmaham) =>	//<<<<<<< This used to work, in principle the code is still good (but ST)
				{//8.1.22 oh god I just remembered how dumb this code is. but hey it works so idk!!
					{
						for (int x = 0; x < imgWidth; x++)
						{
							for (int y = 0; y < imgHeight; y++)
							{
								if (((x % Convert.ToInt32(Math.Pow(2.0d, exponent)) == 0)) && ((y % Convert.ToInt32(Math.Pow(2.0d, exponent))) == 0))
								{
									if (colourMapRGB.GetLength(0) <= x + boxSize || colourMapRGB.GetLength(1) <= y + boxSize)
									{
										Console.WriteLine("Edge reached!!!");
									}
									else
									{
										bool[] boarderIsInSet = new bool[4 * boxSize];
										for (int j = 0; j < boxSize; j++)
										{
											//Console.WriteLine("x = " + $"{x + j}" + ", y = " + $"{y}");
											boarderIsInSet[j] = isInSetStencil[x + j, y];
											if (boarderIsInSet[j] == true)
											{
												//colourMapRGB[x + j, y] = new Col3Byte(127, 0, 127);
											}
										}
										for (int j = 0; j < boxSize; j++)
										{
											//Console.WriteLine("x = " + $"{x + boxSize}" + ", y = " + $"{y + j}");
											boarderIsInSet[boxSize + j] = isInSetStencil[x + boxSize, y + j];
											if (boarderIsInSet[boxSize + j] == true)
											{
												//colourMapRGB[x + boxSize, y + j] = new Col3Byte(0, 0, 191);
											}
										}
										for (int j = 0; j < boxSize; j++)
										{
											//Console.WriteLine("x = " + $"{x + boxSize - j}" + ", y = " + $"{y + boxSize}");
											boarderIsInSet[2 * boxSize + j] = isInSetStencil[x + boxSize - j, y + boxSize];
											if (boarderIsInSet[2 * boxSize + j] == true)
											{
												//colourMapRGB[boxSize - j, y + boxSize] = new Col3Byte(0, 127, 127);
											}
										}
										for (int i = 0; i < boxSize; i++)
										{
											//Console.WriteLine("x = " + $"{x}" + ", y = " + $"{y + boxSize - i}");
											boarderIsInSet[3 * boxSize + i] = isInSetStencil[x, y + boxSize - i];
											if (boarderIsInSet[3 * boxSize + i] == true)
											{
												//colourMapRGB[x, y + boxSize - i] = new Col3Byte(0, 191, 0);
											}
										}
										bool startBool = true;
										bool isBoarderUniform = true;
										/*for (int i = 0; i < boarderIsInSet.Length; i++)
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
											Console.Write(str);
										}
										Console.Write(", x = " + $"{x}" + ", y = " + $"{y}");
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
										Console.Write("isBoarderUniform = " + $"{isBoarderUniform}");
										if (startBool == true) { Console.WriteLine(", block is in set"); }
										else { Console.WriteLine(", block isn't in set"); }
										if (isBoarderUniform)//fill block with that thing
										{
											for (int i = 0; i < boxSize; i++)
											{
												for (int j = 0; j < boxSize; j++)
												{
													isInSetStencil[x + i, y + j] = startBool;
													if (startBool == true)
													{
														colourMapRGB[x + i, y + j] = inSetColour;
													}
													else
													{
														colourMapRGB[x + i, y + j] = notSetColour;
													}
												}
											}
										}
										else//subdivide
										{
											for (int i = 1; i < boxSize; i++)
											{
												for (int j = 1; j < boxSize; j++)
												{
													colourMapRGB[x + i, y + j] = subDivColour;
												}
											}
										}
									}
								}
							}
						}
					}
					for (int x = 1; x < imgWidth - 1; x++)
					{
						for (int y = 1; y < imgHeight - 1; y++)
						{
							int counter = 0;
							if (colourMapRGB[x, y] == inSetColour)
							{
								counter += colourMapRGB[x, y + 1] == subDivColour ? 1 : 0;
								counter += colourMapRGB[x, y - 1] == subDivColour ? 1 : 0;
								counter += colourMapRGB[x + 1, y] == subDivColour ? 1 : 0;
								counter += colourMapRGB[x - 1, y] == subDivColour ? 1 : 0;
								Console.WriteLine("counter = " + $"{counter}");
							}
							if (counter >= 2)
							{
								colourMapRGB[x, y] = boarderColour;
							}
						}
					}
				};
		*/
		public Action<bool[,], bool[,], int, int, object> TheCoolerWriteInnerStencilArea = (isInSetStencil, subDivAreaStencil, y_min, y_max, parmaham) => //8.1.22 I just thought, had deltarune chapter 2 come out when I wrote this??? woah this code is like so old????
		{
			int exponentHighest;
			if (isInSetStencil.GetLength(1) > isInSetStencil.GetLength(0))
			{
				exponentHighest = Convert.ToInt32(Math.Floor(Math.Log2(Convert.ToDouble(isInSetStencil.GetLength(0)))));
			}
			else
			{
				exponentHighest = Convert.ToInt32(Math.Floor(Math.Log2(Convert.ToDouble(isInSetStencil.GetLength(1)))));
			}
			for (int exponent = exponentHighest; exponent >= 1; exponent--)
			{
				Console.WriteLine("exponent = " + $"{exponent}");
				int boxSize = Convert.ToInt32(Math.Pow(2.0d, exponent));
				Console.WriteLine("boxSize = " + $"{boxSize}");
				for (int x = 0; x < isInSetStencil.GetLength(0); x++)
				{
					for (int y = 0; y < isInSetStencil.GetLength(1); y++)
					{
						if (((x % Convert.ToInt32(Math.Pow(2.0d, exponent)) == 0)) && ((y % Convert.ToInt32(Math.Pow(2.0d, exponent))) == 0))
						{
							if (isInSetStencil.GetLength(0) <= x + boxSize || isInSetStencil.GetLength(1) <= y + boxSize)
							{
								Console.WriteLine("Edge reached!!!");
							}
							else
							{
								bool[] boarderIsInSet = new bool[4 * boxSize];
								for (int j = 0; j < boxSize; j++)
								{
									Console.WriteLine("x = " + $"{x + j}" + ", y = " + $"{y}");
									boarderIsInSet[j] = isInSetStencil[x + j, y];
									if (boarderIsInSet[j] == true)
									{
										//colourMapRGB[x + j, y] = new Col3Byte(127, 0, 127);
									}
								}
								for (int j = 0; j < boxSize; j++)
								{
									Console.WriteLine("x = " + $"{x + boxSize}" + ", y = " + $"{y + j}");
									boarderIsInSet[boxSize + j] = isInSetStencil[x + boxSize, y + j];
									if (boarderIsInSet[boxSize + j] == true)
									{
										//colourMapRGB[x + boxSize, y + j] = new Col3Byte(0, 0, 191);
									}
								}
								for (int j = 0; j < boxSize; j++)
								{
									Console.WriteLine("x = " + $"{x + boxSize - j}" + ", y = " + $"{y + boxSize}");
									boarderIsInSet[2 * boxSize + j] = isInSetStencil[x + boxSize - j, y + boxSize];
									if (boarderIsInSet[2 * boxSize + j] == true)
									{
										//colourMapRGB[boxSize - j, y + boxSize] = new Col3Byte(0, 127, 127);
									}
								}
								for (int i = 0; i < boxSize; i++)
								{
									Console.WriteLine("x = " + $"{x}" + ", y = " + $"{y + boxSize - i}");
									boarderIsInSet[3 * boxSize + i] = isInSetStencil[x, y + boxSize - i];
									if (boarderIsInSet[3 * boxSize + i] == true)
									{
										//colourMapRGB[x, y + boxSize - i] = new Col3Byte(0, 191, 0);
									}
								}
								bool startBool = true;
								bool isBoarderUniform = true;
								for (int i = 0; i < boarderIsInSet.Length; i++)//Debugging loop
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
									Console.Write(str);
								}
								Console.Write(", x = " + $"{x}" + ", y = " + $"{y}");
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
								Console.WriteLine("isBoarderUniform = " + $"{isBoarderUniform}");
								if (startBool == true) { Console.WriteLine(", block is in set"); }
								else { Console.WriteLine(", block isn't in set"); }
								if (isBoarderUniform)//fill block with that thing
								{
									for (int i = 0; i < boxSize; i++)
									{
										for (int j = 0; j < boxSize; j++)
										{
											isInSetStencil[x + i, y + j] = startBool;
											if (startBool == true)
											{
												isInSetStencil[x + i, y + j] = true;
											}
											else
											{
												isInSetStencil[x + i, y + j] = false;
											}
										}
									}
								}
								else//subdivide
								{
									for (int i = 1; i < boxSize; i++)
									{
										for (int j = 1; j < boxSize; j++)
										{
											subDivAreaStencil[x + i, y + j] = true;
										}
									}
								}
							}
						}
					}
				}
				/*	Doesnt work, used to be used to debug boarders. Should probably write dedicated function to do that
				for (int x = 1; x < isInSetStencil.GetLength(0) - 1; x++)
				{
					for (int y = 1; y < isInSetStencil.GetLength(1) - 1; y++)
					{
						int counter = 0;
						if (isInSetStencil[x, y] == true)
						{
							counter += subDivAreaStencil[x, y + 1] == true ? 1 : 0;
							counter += subDivAreaStencil[x, y - 1] == true ? 1 : 0;
							counter += subDivAreaStencil[x + 1, y] == true ? 1 : 0;
							counter += subDivAreaStencil[x - 1, y] == true ? 1 : 0;
							Console.WriteLine("counter = " + $"{counter}");
						}
						if (counter >= 2)
						{
							debugBoarderStencil[x, y] = true;
						}
					}*/
			}
		};
		
		/*
		public void TestWriteGrid(Col3Byte[,] colourMapRGB)	//Single Threaded, Deprecated
		{
			bool[,] isInSetStencil = new bool[colourMapRGB.GetLength(0), colourMapRGB.GetLength(1)];
			int exponentHighest;
			Col3Byte subDivColour = new Col3Byte(0, 63, 191);
			Col3Byte inSetColour = new Col3Byte(255, 127, 0);
			Col3Byte notSetColour = new Col3Byte(31, 0, 31);
			Col3Byte boarderColour = new Col3Byte(255, 191, 191);
			if (imgHeight > imgWidth)
			{
				exponentHighest = Convert.ToInt32(Math.Floor(Math.Log2(imgWidth_d)));
			}
			else
			{
				exponentHighest = Convert.ToInt32(Math.Floor(Math.Log2(imgHeight_d)));
			}
			for (int exponent = exponentHighest; exponent >= 1; exponent--)
			{
				Console.WriteLine("exponent = " + $"{exponent}");
				int boxSize = Convert.ToInt32(Math.Pow(2.0d, exponent));
				Console.WriteLine("boxSize = " + $"{boxSize}");
				for (int x = 0; x < imgWidth; x++)
				{
					for (int y = 0; y < imgHeight; y++)
					{
						if (((x % Convert.ToInt32(Math.Pow(2.0d, exponent)) == 0)) || ((y % Convert.ToInt32(Math.Pow(2.0d, exponent))) == 0))
						{ 
							if(x+1 >= imgWidth || y+1 >= imgHeight) { Console.WriteLine("Escaped image!"); break; }//delete me!!!
							else if (exponent != exponentHighest && colourMapRGB[x,y] != inSetColour)	//THIS IS EXTREMELY BAD!!!!!!!!!!!!!!!!!!!!
							{
								isInSetStencil[x, y] = CheckWhetherIsInSet(x, y);
							}
							else if(exponent == exponentHighest)
							{
								isInSetStencil[x, y] = CheckWhetherIsInSet(x, y);
							}
						}
					}
				}

				for (int x = 0; x < imgWidth; x++)
				{
					for (int y = 0; y < imgHeight; y++)
					{
						//assume colourMap is black
						if (((x % Convert.ToInt32(Math.Pow(2.0d, exponent)) == 0)) && ((y % Convert.ToInt32(Math.Pow(2.0d, exponent))) == 0))
						{
							if (colourMapRGB.GetLength(0) <= x + boxSize || colourMapRGB.GetLength(1) <= y + boxSize)
							{
								Console.WriteLine("Edge reached!!!");
							}
							else
							{
								bool[] boarderIsInSet = new bool[4 * boxSize];
								for (int j = 0; j < boxSize; j++)
								{
									//Console.WriteLine("x = " + $"{x + j}" + ", y = " + $"{y}");
									boarderIsInSet[j] = isInSetStencil[x + j, y];
									if (boarderIsInSet[j] == true)
									{
										//colourMapRGB[x + j, y] = new Col3Byte(127, 0, 127);
									}
								}
								for (int j = 0; j < boxSize; j++)
								{
									//Console.WriteLine("x = " + $"{x + boxSize}" + ", y = " + $"{y + j}");
									boarderIsInSet[boxSize + j] = isInSetStencil[x + boxSize, y + j];
									if (boarderIsInSet[boxSize + j] == true)
									{
										//colourMapRGB[x + boxSize, y + j] = new Col3Byte(0, 0, 191);
									}
								}
								for (int j = 0; j < boxSize; j++)
								{
									//Console.WriteLine("x = " + $"{x + boxSize - j}" + ", y = " + $"{y + boxSize}");
									boarderIsInSet[2 * boxSize + j] = isInSetStencil[x + boxSize - j, y + boxSize];
									if (boarderIsInSet[2 * boxSize + j] == true)
									{
										//colourMapRGB[boxSize - j, y + boxSize] = new Col3Byte(0, 127, 127);
									}
								}
								for (int i = 0; i < boxSize; i++)
								{
									//Console.WriteLine("x = " + $"{x}" + ", y = " + $"{y + boxSize - i}");
									boarderIsInSet[3 * boxSize + i] = isInSetStencil[x, y + boxSize - i];
									if (boarderIsInSet[3 * boxSize + i] == true)
									{
										//colourMapRGB[x, y + boxSize - i] = new Col3Byte(0, 191, 0);
									}
								}
								bool startBool = true;
								bool isBoarderUniform = true;
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
									Console.Write(str);
								}
								Console.Write(", x = " + $"{x}" + ", y = " + $"{y}");
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
								Console.Write("isBoarderUniform = " + $"{isBoarderUniform}");
								if(startBool == true) { Console.WriteLine(", block is in set"); }
								else { Console.WriteLine(", block isn't in set"); }
								if (isBoarderUniform)//fill block with that thing
								{
									for (int i = 0; i < boxSize; i++)
									{
										for (int j = 0; j < boxSize; j++)
										{
											isInSetStencil[x + i, y + j] = startBool;
											if (startBool == true)
											{
												colourMapRGB[x + i, y + j] = inSetColour;
											}
											else
											{
												colourMapRGB[x + i, y + j] = notSetColour;
											}
										}
									}
								}
								else//subdivide
								{
									for (int i = 1; i < boxSize; i++)
									{
										for (int j = 1; j < boxSize; j++)
										{
											colourMapRGB[x + i, y + j] = subDivColour;
										}
									}
								}
							}
						}
					}
				}
			}
			for (int x = 1; x < imgWidth - 1; x++)
			{
				for (int y = 1; y < imgHeight -1; y++)
				{
					int counter = 0;
					if(colourMapRGB[x,y] == inSetColour)
					{
						counter += colourMapRGB[x, y + 1] == subDivColour ? 1 : 0;
						counter += colourMapRGB[x, y - 1] == subDivColour ? 1 : 0;
						counter += colourMapRGB[x + 1, y] == subDivColour ? 1 : 0;
						counter += colourMapRGB[x - 1, y] == subDivColour ? 1 : 0;
						Console.WriteLine("counter = " + $"{counter}");
					}
					if(counter >= 2)
					{
						colourMapRGB[x, y] = boarderColour;
					}
				}
			}
		*/
	}
}