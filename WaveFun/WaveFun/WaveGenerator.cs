using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Media;
using System.Windows.Forms;
using static WaveFun.Chunks;

namespace WaveFun
{
	public class WaveGenerator
	{
		public WaveHeader header;
		public WaveFormatChunk format;
		public WaveDataChunk data;

		public SoundPlayer player { get; private set; }

		public List<double> notesList;

		//private double frequency;

		private double duration;

		private uint numSamples;

		private uint amplitude;

		Random rand;

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
			header = new WaveHeader();
			format = new WaveFormatChunk();
			data = new WaveDataChunk();

			player = new SoundPlayer();

			// volume
			amplitude = 12000; //32760 max for 16-bit audio

			// random generator
			rand = new Random();

			byte[] streamData = new byte[numSamples * 2];

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

		public void Play(WaveExampleType type, List<double> frequencies)
		{
			CalculateHeaderInfo(frequencies.Average());

			switch (type)
			{
				//	case WaveExampleType.ExampleSineWave:

				//		for (uint i = 0; i < numSamples - 1; i++)
				//		{
				//			// fill with sine wave
				//			for (int channel = 0; channel < format.wChannels; channel++)
				//			{
				//				data.sample16[i + channel] = Convert.ToInt16(amplitude * NoteToWave(Notes.C4, duration, i)); // single tone
				//			}
				//		}
				//		break;

				case WaveExampleType.AllNotes:

					for (uint i = 0; i < numSamples - 1; i++)
					{
						double[] waves = new double[notesList.Count];
						for (int channel = 0; channel < format.wChannels; channel++)
						{
							for (int j = 0; j < notesList.Count; j++)
							{
								waves[j] = NoteToWave(notesList[j], duration, i);
							}

							data.sample16[i + channel] = Convert.ToInt16(amplitude * Avg(waves));
						}
					}
					break;

				//	case WaveExampleType.AllNotesFunc:

				//		for (uint i = 0; i < numSamples - 1; i++)
				//		{
				//			double[] waves = new double[notesList.Count];
				//			for (int channel = 0; channel < format.wChannels; channel++)
				//			{
				//				for (int j = 0; j < notesList.Count; j++)
				//				{
				//					waves[j] = NoteToWave(notesList[j], duration, i);
				//				}

				//				data.sample16[i + channel] = Convert.ToInt16(amplitude * waves.Average());
				//			}
				//		}
				//		break;

				case WaveExampleType.Streaming:

					Stream wavStream = new MemoryStream();
					player.Stream = wavStream;
					HeaderToStream(wavStream);

					for (uint i = 0; i < numSamples - 1; i++)
					{
						double[] waves = new double[frequencies.Count];
						//NotesToWaves(frequencies, duration, i);
						for (int channel = 0; channel < format.wChannels; channel += format.wChannels)
						{
							for (int j = 0; j < frequencies.Count; j++)
							{
								waves[j] = NoteToWave(frequencies[j], duration, i);
							}

							data.sample16[i + channel] = Convert.ToInt16(amplitude * Avg(waves));
							byte[] byteArr = BitConverter.GetBytes(data.sample16[i + channel]);
							wavStream.Write(byteArr, 0, 2);
						}
					}
					wavStream.Seek(0, SeekOrigin.Begin);
					player.PlayLooping();
					break;

					//	case WaveExampleType.SquareWave:

					//		for (uint i = 0; i < numSamples - 1; i++)
					//		{
					//			double[] waves = new double[notesList.Count];
					//			for (int channel = 0; channel < format.wChannels; channel++)
					//			{
					//				for (int j = 0; j < notesList.Count; j++)
					//				{
					//					waves[j] = NoteToSquareWave(notesList[j], duration, i);
					//				}

					//				data.sample16[i + channel] = Convert.ToInt16(amplitude * Avg(waves));
					//				//byte[] tempArr = BitConverter.GetBytes(data.sample16[i + channel]);
					//				//wavStream.Write(tempArr, 0, 2);
					//			}
					//		}
					//		break;

					//	case WaveExampleType.WhiteNoise:
					//		for (uint i = 0; i < numSamples - 1; i++)
					//		{
					//			for (int channel = 0; channel < format.wChannels; channel++)
					//			{
					//				data.sample16[i + channel] = Convert.ToInt16(amplitude * Math.Sin(rand.Next()));
					//			}
					//		}
					//		break;
					//}
					//}
			}
		}

		public void CalculateHeaderInfo(double freq)
		{

			// length in seconds
			duration = 1 / freq * 250;

			// total number of samples = sample rate per second * channels * duration in seconds
			numSamples = (uint)(format.dwSamplesPerSec * format.wChannels * duration);

			// initialize the 16-bit array
			data.sample16 = new short[numSamples];

			// total file size 
			uint fileSize = numSamples * (format.dwBitsPerSample / 8) + 46; // 46 bytes in the header

			// write to dwFileLength
			header.dwFileLength = fileSize - 8; // -8 to exclude RIFF and WAVE

			// calculate data chunk size in bytes
			data.dwChunkSize = (numSamples * (format.dwBitsPerSample / 8));
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
			uint numSamples = (uint)(format.dwSamplesPerSec * format.wChannels * time);

			return (Math.PI * 2 * freq) / numSamples * time; // review this formula
		}

		// Average two doubles
		private double Avg(double num1, double num2)
		{
			return (num1 + num2) * .5;
		}

		// Averages an array of doubles
		private double Avg(double[] values)
		{
			return Avg(values, 0, 0);
		}

		// Recursively averages an array
		private double Avg(double[] values, double sum, uint index)
		{
			if (index >= values.Length)
				return sum / index;
			return Avg(values, sum + values[index], ++index);
		}

		private void HeaderToStream(Stream stream)
		{
			char[] riff = header.sGroupID.ToCharArray();
			char[] wave = header.sRiffType.ToCharArray();
			char[] fmt = format.sGroupID.ToCharArray();
			char[] dat = data.sGroupID.ToCharArray();

			// header
			foreach (char c in riff)
				stream.Write(BitConverter.GetBytes(c), 0, 1);

			stream.Write(BitConverter.GetBytes(header.dwFileLength), 0, 4);

			foreach (char c in wave)
				stream.Write(BitConverter.GetBytes(c), 0, 1);

			// format
			foreach (char c in fmt)
				stream.Write(BitConverter.GetBytes(c), 0, 1);

			stream.Write(BitConverter.GetBytes(format.dwChunkSize), 0, 4);

			stream.Write(BitConverter.GetBytes(format.wFormatTag), 0, 2);

			stream.Write(BitConverter.GetBytes(format.wChannels), 0, 2);

			stream.Write(BitConverter.GetBytes(format.dwSamplesPerSec), 0, 4);

			stream.Write(BitConverter.GetBytes(format.dwAvgBytesPerSec), 0, 4);

			stream.Write(BitConverter.GetBytes(format.wBlockAlign), 0, 2);

			stream.Write(BitConverter.GetBytes(format.dwBitsPerSample), 0, 4);

			// data
			foreach (char c in dat)
				stream.Write(BitConverter.GetBytes(c), 0, 1);

			stream.Write(BitConverter.GetBytes(data.dwChunkSize), 0, 4);
		}

		// Save the wave
		public void Save(string filePath)
		{
			// create file
			FileStream filestream = new FileStream(filePath, FileMode.Create);

			// use BinaryWriter to write the bytes to the file
			BinaryWriter writer = new BinaryWriter(filestream);

			// write header
			writer.Write(header.sGroupID.ToCharArray());
			writer.Write(header.dwFileLength); // this will be rewritten once file size is known
			writer.Write(header.sRiffType.ToCharArray());

			// write format chunk
			writer.Write(format.sGroupID.ToCharArray());
			writer.Write(format.dwChunkSize);
			writer.Write(format.wFormatTag);
			writer.Write(format.wChannels);
			writer.Write(format.dwSamplesPerSec);
			writer.Write(format.dwAvgBytesPerSec);
			writer.Write(format.wBlockAlign);
			writer.Write(format.dwBitsPerSample);

			// write data chunk
			writer.Write(data.sGroupID.ToCharArray());
			writer.Write(data.dwChunkSize);

			foreach (short dataPoint in data.sample16)
			{
				writer.Write(dataPoint);
			}

			//writer.Seek(4, SeekOrigin.Begin);
			//uint fileSize = (uint)writer.BaseStream.Length;
			//writer.Write(fileSize - 8);

			Console.WriteLine("File written.");

			// clean up
			writer.Close();
			filestream.Close();
		}
	}
}
