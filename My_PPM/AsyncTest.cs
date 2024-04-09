using System;
using System.Threading;
using System.Threading.Tasks;
using System.Numerics;

using System.Diagnostics;
namespace PPM_Fractal_Renderer
{
	public partial class SoftwareShader
	{       /*
		static int wibble = 64;
		static int wubble = 99999;
		int threadNumber = 16;
		double[] wobble = new double[wibble];
		public Complex CmplxFromInt(int i)
		{
			double i_d = Convert.ToDouble(i);
			double phase = i_d / Convert.ToDouble(wibble) * myTau;
			double radius = 0.5d;
			Complex foob = new Complex(radius * Math.Cos(phase), radius * Math.Sin(phase));
			return foob;
		}

		public async Task HardToUnderstand(double[] array, int start, int end)
		{
			for(int i = start; i < end; i++)
			{
				Complex rhdhdr = CmplxFromInt(i);
				Console.WriteLine("rhdhdr = " + $"{rhdhdr}");
				array[i] = awifoakf(rhdhdr);
			}
		}
		public void HardToUnderstandMT(double[] array, int start, int end, int name)
		{
			Thread.CurrentThread.Name = name.ToString();

			Console.WriteLine("Thread " + Thread.CurrentThread.Name + " started!");
			for (int j = start; j < end; j++)
			{
				Complex rhdhdr = CmplxFromInt(j);
				Console.WriteLine("Thread " + Thread.CurrentThread.Name + ", j = " + $"{j}" + ", rhdhdr = " + $"{rhdhdr}");
				array[j] = awifoakf(rhdhdr);
			}
			Console.WriteLine("Thread " + Thread.CurrentThread.Name + " done!");
			return;
		}

		public double awifoakf(Complex barb)
		{
			Complex z = barb;
			Complex c = barb;
			for (int j = 0; j < wubble; j++)
			{
				z = Complex.Pow(z, 2.0d) + c;
				if (z.Magnitude > 2)
				{
					return 0.1337d;
				}
			}
			return z.Magnitude;
		}

		public void DoSomeStuff(object input)
		{
			double output = 0;
			for (int i = 0; i < 300000; i++)
			{
				output += Convert.ToDouble(i);
	
			}
			input = output;
		}

		public void DoThingsAsync()	//please try and fix this
		{

			Task[] taskArray = new Task[threadNumber];
			double wibble_d = Convert.ToDouble(wibble);
			double threadNumber_d = Convert.ToDouble(threadNumber);

			double interval_d = wibble_d / threadNumber_d;
			int interval = Convert.ToInt32(interval_d);

			Console.WriteLine("Interval_d = " + $"{interval_d}");
			Console.WriteLine("Interval = " + $"{interval}");

			var timer = new Stopwatch();
			timer.Start();

			for (int i = 0; i < threadNumber; i++)
			{
				taskArray[i] = HardToUnderstand(wobble, i * interval, (i + 1) * interval);
			}
			Task.WaitAll(taskArray);

			for (int i = 0; i < wobble.Length; i++)
			{
				Console.WriteLine("wobble[" + $"{i}" + "] = " + $"{wobble[i]}");
			}
			timer.Stop();

			TimeSpan timeTaken = timer.Elapsed;
			string foo = "Time taken: " + timeTaken.ToString(@"m\:ss\.fff");
			Console.WriteLine(foo);

		}
		public void DoThingsAsyncMT() //Fuck it just use threads
		{
			double wibble_d = Convert.ToDouble(wibble);
			double threadNumber_d = Convert.ToDouble(threadNumber);
			double interval_d = wibble_d / threadNumber_d;
			int interval = Convert.ToInt32(interval_d);
			Console.WriteLine("Interval_d = " + $"{interval_d}");
			Console.WriteLine("Interval = " + $"{interval}");
			Thread[] threadArray = new Thread[threadNumber];
			var timer = new Stopwatch();
			timer.Start();
			int counter = 0;
			for (int i = 0; i < threadNumber - 1; i++)
			{
				Console.WriteLine("i = " + i.ToString());
				threadArray[i] = new Thread(unused => HardToUnderstandMT(wobble, i * interval, (i + 1) * interval - 1, i));
				Console.WriteLine("threadArray[i].Name = " + $"{threadArray[i].Name}");
				threadArray[i].IsBackground = false;
			}
			counter = 0;
			for (int i = 0; i < threadArray.Length - 1; i++)
			{
				threadArray[i].Start();
				Console.WriteLine("threadArray[i].Name = " + $"{threadArray[i].Name}");
			}
			Console.WriteLine("Waiting...");
			for(int k = 0; k < threadArray.Length - 1; k++)	//I HATE THIS
			{
				counter++;
				Console.WriteLine("counter = " + $"{counter}");
				threadArray[k].Join();
			}
			Console.WriteLine("All threads finished! Counter = " + counter.ToString());
			for (int i = 0; i < wobble.Length; i++)
			{
				Console.WriteLine("wobble[" + $"{i}" + "] = " + $"{wobble[i]}");
			}
			timer.Stop();
			TimeSpan timeTaken = timer.Elapsed;
			string foo = "Time taken: " + timeTaken.ToString(@"m\:ss\.fff");
			Console.WriteLine(foo);

		}
		public void DoThingsSync()
		{
			//Task[] taskArray = new Task[threadNumber];
			//double wibble_d = Convert.ToDouble(wibble);
			//double threadNumber_d = Convert.ToDouble(threadNumber);

			//double interval_d = wibble_d / threadNumber_d;
			//int interval = Convert.ToInt32(interval_d);

			//Console.WriteLine("Interval_d = " + $"{interval_d}");
			//Console.WriteLine("Interval = " + $"{interval}");

			var timer = new Stopwatch();
			timer.Start();

			for (int i = 0; i < wibble; i++)
			{
				Complex feoifjwo = CmplxFromInt(i);
				Console.WriteLine("rhdhdr = " + $"{feoifjwo}");
				wobble[i] = awifoakf(feoifjwo);
			}

			for (int i = 0; i < wobble.Length; i++)
			{
				Console.WriteLine("wobble[" + $"{i}" + "] = " + $"{wobble[i]}");
			}
			timer.Stop();

			TimeSpan timeTaken = timer.Elapsed;
			string foo = "Time taken: " + timeTaken.ToString(@"m\:ss\.fff");
			Console.WriteLine(foo);

		}

		public void ThreadDebugTestMutex()
		{
			int threadNumber = 16;
			int length = 160;
			double[] array = new double[160];
			Thread[] threads = new Thread[threadNumber];
			int span = length / threadNumber;
			Mutex mutex = new Mutex();
			Console.WriteLine("span = " + $"{span}");
			int IDK = 0;
			for (int i = 0; i < threadNumber; i++)
			{
				Console.WriteLine("i = " + $"{i}");
				threads[i] = new Thread(() =>
				{
					Console.WriteLine("{0} is requesting the mutex", Thread.CurrentThread.Name);
					mutex.WaitOne();
					Console.WriteLine("{0} has entered the protected area", Thread.CurrentThread.Name);
					for (int j = IDK * (span); j < (IDK + 1) * (span); j++)
					{
						Console.WriteLine("IDK = " + $"{IDK}" + ", j = " + $"{j}");
						array[j] = awifoakf(CmplxFromInt(j));
					}
					IDK++;
					Console.WriteLine("{0} is leaving the protected area", Thread.CurrentThread.Name);
					mutex.ReleaseMutex();
					Console.WriteLine("{0} has released the mutex", Thread.CurrentThread.Name);
				})
				{
					Name = String.Format("Thread{0}", i)
				};
			}
			for(int i = 0; i < threadNumber; i++)
			{
				threads[i].Start();
			}
			foreach(var thread in threads)
			{
				thread.Join();
			}
			for (int i = 0; i < array.Length; i++){
				Console.WriteLine("Array[" + $"{i}" + "] = " + $"{array[i]}");
			}
		}
		public void ThreadDebugTest()
		{
			int threadNumber = 16;
			int length = 160;
			double[] array = new double[160];
			Thread[] threads = new Thread[threadNumber];
			int span = length / threadNumber;
			Console.WriteLine("span = " + $"{span}");
			int IDK = 0;

			for (int i = 0; i < threadNumber; i++)
			{
				Console.WriteLine("i = " + $"{i}");
				threads[i] = new Thread(() =>
				{
					for (int j = IDK * (span); j < (IDK + 1) * (span); j++)
					{
						
						Console.WriteLine("IDK = " + $"{IDK}" + ", j = " + $"{j}");
						array[j] = awifoakf(CmplxFromInt(j));
					}
					IDK++;
				})
				{
					Name = String.Format("Thread_{0}", i)
				};
			}
			for (int i = 0; i < threadNumber; i++)
			{
				threads[i].Start();
			}
			foreach (var thread in threads)
			{
				thread.Join();
			}
			for (int i = 0; i < array.Length; i++)
			{
				Console.WriteLine("Array[" + $"{i}" + "] = " + $"{array[i]}");
			}
		}*/
	}
}