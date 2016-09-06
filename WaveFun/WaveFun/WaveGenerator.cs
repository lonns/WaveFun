using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Media;
using static WaveFun.Chunks;

namespace WaveFun
{
	public struct Notes
	{
		public const double C3 = 130.81;
		public const double D3 = 146.83;
		public const double E3 = 164.81;
		public const double F3 = 176.61;
		public const double G3 = 196;
		public const double A3 = 220;
		public const double B3 = 246.94;
		public const double C4 = 261.63;
		public const double D4 = 293.66;
		public const double E4 = 329.63;
		public const double F4 = 349.23;
		public const double G4 = 392;
		public const double A4 = 440;
		public const double B4 = 493.88;
		public const double C5 = 523.25;
		public const double D5 = 587.33;
		public const double E5 = 659.25;
		public const double F5 = 698.46;
		public const double G5 = 783.99;
		public const double A5 = 880;
		public const double B5 = 987.77;
		public const double C6 = 1046.50;
		public const double D6 = 1174.66;
		public const double E6 = 1318.51;
		public const double F6 = 1396.91;
		public const double G6 = 1567.98;
		public const double A6 = 1760;
		public const double B6 = 1975.53;
	}

	public class WaveGenerator
	{
		public WaveHeader waveHeader { get; private set; }

		public SoundPlayer player { get; private set; }

		public List<double> notesList { get; private set; }

		public uint amplitude;

		public enum WaveExampleType
		{
			ExampleSineWave = 0,
			AllNotes = 1,
			AllNotesFunc = 2,
			Streaming = 3,
			SquareWave = 4,
			WhiteNoise = 5
		}

		public WaveGenerator()
		{
			waveHeader = new WaveHeader();

			player = new SoundPlayer();

			// volume
			amplitude = 12000; //32760 max for 16-bit audio

			notesList = new List<double>();
			notesList.Add(Notes.C3);
			notesList.Add(Notes.D3);
			notesList.Add(Notes.E3);
			notesList.Add(Notes.F3);
			notesList.Add(Notes.G3);
			notesList.Add(Notes.A3);
			notesList.Add(Notes.B3);
			notesList.Add(Notes.C4);
			notesList.Add(Notes.D4);
			notesList.Add(Notes.E4);
			notesList.Add(Notes.F4);
			notesList.Add(Notes.G4);
			notesList.Add(Notes.A4);
			notesList.Add(Notes.B4);
			notesList.Add(Notes.C4);
			notesList.Add(Notes.D4);
			notesList.Add(Notes.E4);
			notesList.Add(Notes.F4);
			notesList.Add(Notes.G4);
			notesList.Add(Notes.A4);
			notesList.Add(Notes.B4);
			notesList.Add(Notes.C5);
			notesList.Add(Notes.D5);
			notesList.Add(Notes.E5);
		}

		public void Play(double[] frequencies, uint bitsPerSample = 16, uint sampleRate = 44100)
		{
			Stream waveStream = new MemoryStream();
			player.Stream = waveStream;
			waveHeader.init(MinDuration(frequencies));
			//waveHeader.init(1 / frequencies.Min());
			HeaderToStream(waveHeader, waveStream);

			for (uint i = 0, j = waveHeader.numSamples; i < j - 1; i++)
			{
				//NotesToWaves(frequencies, duration, i);
				double[] waves = new double[frequencies.Length];

				for (int channel = 0; channel < waveHeader.format.wChannels; channel += waveHeader.format.wChannels)
				{
					for (int k = 0; k < frequencies.Length; k++)
					{
						waves[k] = NoteToWave(frequencies[k], waveHeader.duration, i);
					}
					waveHeader.data.sample16[i + channel] = Convert.ToInt16(amplitude * Avg(waves));
					byte[] waveBytes = BitConverter.GetBytes(waveHeader.data.sample16[i + channel]);
					waveStream.Write(waveBytes, 0, 2);
				}
			}
			waveStream.Seek(0, SeekOrigin.Begin);
			player.PlayLooping();
		}

		// Convert a list of notes to a list of waves
		private List<double> NotesToWaves(List<double> notes, double time, uint index)
		{
			List<double> waves = new List<double>();
			foreach (double note in notes)
			{
				waves.Add(NoteToWave(note, time, index));
			}
			return waves;
		}

		// Convert a note to a wave with range -1 to 1
		private double NoteToWave(double note, double time, uint index)
		{
			return Math.Sin(TrigFreq(note, time) * index);
		}

		// Convert a note to a square wave
		private double NoteToSquareWave(double note, double time, uint index)
		{
			return Math.Sin(TrigFreq(note, time) * index) > 0 ? 1 : -1;
		}

		// Trig function to turn a frequency into a wave over a duration of time
		double TrigFreq(double freq, double time)
		{
			uint numSamples = (uint)(waveHeader.format.dwSamplesPerSec * waveHeader.format.wChannels * time); // revise this

			return ((Math.PI * 2 * freq) / numSamples) * time;
		}

		// Recursively averages an array
		private double Avg(double[] values, double sum = 0, uint index = 0)
		{
			if (index >= values.Length)
				return sum / index;
			return Avg(values, sum + values[index], ++index);
		}

		private double MinDuration(double[] values)
		{
			double lcm = LCM(values);
			double longest = values.Min();
			double highest = values.Max();
			double period = 1 / longest;
			double multiplier = lcm / highest;

			double rtn = period * multiplier;

			return rtn;

		}

		private double LCM2(double num1, double num2)
		{
			double higher, lower;

			if (num1 > num2)
			{
				higher = num1;
				lower = num2;
			}
			else
			{
				higher = num2;
				lower = num1;
			}

			for (int i = 1; i <= lower; i++)
			{
				if ((higher * i) % num2 == 0)
					return higher * i;
			}
			return lower * higher;
		}

		private double LCM2(double[] values)
		{
			double lcm = 1;

			for (int i = 0, j = values.Length; i < j; i++)
			{
				lcm = LCM3(lcm, values[i]);
			}
			Console.WriteLine("Least Common Multiple is " + lcm);
			return lcm;
		}

		private double LCM3(double num1, double num2)
		{
			double x, y, lcm = 0;
			x = num1;
			y = num2;

			while (num1 != num2)
			{
				if (num1 > num2)
				{
					num1 = num1 - num2;
				}
				else
				{
					num2 = num2 - num1;
				}
			}
			lcm = (x * y) / num1;
			return lcm;
		}

		// recursive function to find the Lowest Common Multiple for an array of doubles
		private double LCM(double[] values, int multiplier = 1)
		{
			double highest = values.Max();
			bool failed = false;


			for (int i = 0, j = values.Length; i < j; i++)
			{
				if ((highest * multiplier) % values[i] != 0)
				{
					failed = true;
					break;
				}
			}

			if (!failed)
			{
				return (highest * multiplier);
			}
			else return LCM(values, ++multiplier);
		}

		// recursive function to find the lowest common multiple of 2 doubles
		private double LowestCommonMultiple(double num1, double num2, int multiplier = 1)
		{
			double higher, lower;

			if (num1 > num2)
			{
				higher = num1;
				lower = num2;
			}
			else
			{
				higher = num2;
				lower = num1;
			}

			if ((higher * multiplier) % lower == 0)
				return higher * multiplier;
			else return LowestCommonMultiple(num1, num2, ++multiplier);

		}

		/// <summary>
		/// Write wave header to the stream
		/// </summary>
		/// <param name="stream">Stream to write to</param>
		private void HeaderToStream(WaveHeader waveHeader, Stream stream)
		{
			char[] riff = waveHeader.header.sGroupID.ToCharArray();
			char[] wave = waveHeader.header.sRiffType.ToCharArray();
			char[] fmt = waveHeader.format.sGroupID.ToCharArray();
			char[] dat = waveHeader.data.sGroupID.ToCharArray();

			// header
			foreach (char c in riff)
				stream.Write(BitConverter.GetBytes(c), 0, 1);

			stream.Write(BitConverter.GetBytes(waveHeader.header.dwFileLength), 0, 4);

			foreach (char c in wave)
				stream.Write(BitConverter.GetBytes(c), 0, 1);

			// format
			foreach (char c in fmt)
				stream.Write(BitConverter.GetBytes(c), 0, 1);

			stream.Write(BitConverter.GetBytes(waveHeader.format.dwChunkSize), 0, 4);
			stream.Write(BitConverter.GetBytes(waveHeader.format.wFormatTag), 0, 2);
			stream.Write(BitConverter.GetBytes(waveHeader.format.wChannels), 0, 2);
			stream.Write(BitConverter.GetBytes(waveHeader.format.dwSamplesPerSec), 0, 4);
			stream.Write(BitConverter.GetBytes(waveHeader.format.dwAvgBytesPerSec), 0, 4);
			stream.Write(BitConverter.GetBytes(waveHeader.format.wBlockAlign), 0, 2);
			stream.Write(BitConverter.GetBytes(waveHeader.format.dwBitsPerSample), 0, 4);

			// data
			foreach (char c in dat)
				stream.Write(BitConverter.GetBytes(c), 0, 1);

			stream.Write(BitConverter.GetBytes(waveHeader.data.dwChunkSize), 0, 4);
		}

		// Save the wave
		public void Save(string filePath)
		{
			// create file
			FileStream filestream = new FileStream(filePath, FileMode.Create);

			// use BinaryWriter to write the bytes to the file
			BinaryWriter writer = new BinaryWriter(filestream);

			// write header
			writer.Write(waveHeader.header.sGroupID.ToCharArray());
			writer.Write(waveHeader.header.dwFileLength); // this will be rewritten once file size is known
			writer.Write(waveHeader.header.sRiffType.ToCharArray());

			// write format chunk
			writer.Write(waveHeader.format.sGroupID.ToCharArray());
			writer.Write(waveHeader.format.dwChunkSize);
			writer.Write(waveHeader.format.wFormatTag);
			writer.Write(waveHeader.format.wChannels);
			writer.Write(waveHeader.format.dwSamplesPerSec);
			writer.Write(waveHeader.format.dwAvgBytesPerSec);
			writer.Write(waveHeader.format.wBlockAlign);
			writer.Write(waveHeader.format.dwBitsPerSample);

			// write data chunk
			writer.Write(waveHeader.data.sGroupID.ToCharArray());
			writer.Write(waveHeader.data.dwChunkSize);

			foreach (short dataPoint in waveHeader.data.sample16)
			{
				writer.Write(dataPoint);
			}

			// clean up
			writer.Close();
			filestream.Close();
		}
	}
}
