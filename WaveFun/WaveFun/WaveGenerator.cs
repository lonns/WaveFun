using System;
using System.Collections.Generic;
using System.IO;
using static WaveFun.Chunks;

namespace WaveFun
{
	public class WaveGenerator
	{
		public WaveHeader header;
		public WaveFormatChunk format;
		public WaveDataChunk data;

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
			Testing = 2
		}

		public WaveGenerator(WaveExampleType type)
		{
			header = new WaveHeader();
			format = new WaveFormatChunk();
			data = new WaveDataChunk();

			SortedDictionary<string, double> notes = new SortedDictionary<string, double>();

			List<double> notesList = new List<double>();
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

			// length in seconds
			uint duration = 10;

			// total number of samples = sample rate per second * channels * duration in seconds
			uint numSamples = format.dwSamplesPerSec * format.wChannels * duration; // 88200

			// initialize the 16-bit array
			data.sample16 = new short[numSamples];

			// volume
			int amplitude = 2000; //32760 max for 16-bit audio

			// example frequency
			double freq1 = 690; //440.0; // Concert A: 440Hz

			//double freq2 = 392; // 261.25; // Concert C: 261.25Hz
			Random rand = new Random();

			// the "angle" used in the function, adjusted for the number of channels and sample rate.
			// this value is like the period of the wave.
			double t1 = (Math.PI * 2 * freq1) / numSamples * duration; // (6.283 * freq) ... 2764 / 88200 = .03134468...
			//double t2 = (Math.PI * 2 * freq2) / numSamples * duration; // 1641.482... / 88200 = .0186109...
			switch (type)
			{
				case WaveExampleType.ExampleSineWave:

					for (uint i = 0; i < numSamples - 1; i++)
					{
						// fill with simple sine wave at max amplitude
						for (int channel = 0; channel < format.wChannels; channel++)
						{
							data.sample16[i + channel] = Convert.ToInt16(amplitude * Math.Sin(t1 * i)); // single tone
						}
					}
					break;

				case WaveExampleType.AllNotes:

					for (uint i = 0; i < numSamples - 1; i++)
					{
						// fill with simple sine wave at max amplitude
						double[] waves = new double[notesList.Count];
						for (int channel = 0; channel < format.wChannels; channel++)
						{
							for (int j = 0; j < notesList.Count; j++)
							{
								waves[j] = Math.Sin(TrigFreq(notesList[j], duration) * i);
							}

							data.sample16[i + channel] = Convert.ToInt16(amplitude * Avg(waves));
						}
					}
					break;

				case WaveExampleType.Testing:

					for (uint i = 0; i < numSamples - 1; i++)
					{
						// fill with simple sine wave at max amplitude
						double[] waves = new double[notesList.Count];
						for (int channel = 0; channel < format.wChannels; channel++)
						{
							for (int j = 0; j < notesList.Count; j++)
							{
								waves[j] = Math.Sin(TrigFreq(notesList[j], duration) * i);
							}

							data.sample16[i + channel] = Convert.ToInt16(amplitude * Avg(waves));
						}
					}
					break;
			}

			// calculate data chunk size in bytes
			data.dwChunkSize = (uint)(data.sample16.Length * (format.dwBitsPerSample / 8));
		}

		private double NoteToWave(double note, uint index)
		{
			return Math.Sin(TrigFreq(note, 1) * index);
		}

		private List<double> NotesToWaves(List<double> notes, uint index)
		{
			List<double> waves = new List<double>();
			foreach(double note in notes)
			{
				waves.Add(Math.Sin(TrigFreq(note, 1) * index));
			} 
			return waves;
		} 

		private double Avg(double num1, double num2)
		{
			return (num1 + num2) * .5;
		}

		private double Avg(double[] list)
		{
			return Avg(list, 0, 0);
		}

		// Recursively averages an array
		private double Avg(double[] list, double sum, uint index)
		{
			if (index >= list.Length)
				return sum / list.Length;
			return Avg(list, sum + list[index], ++index);
		}

		double TrigFreq(double freq, uint time)
		{
			uint numSamples = format.dwSamplesPerSec * format.wChannels * time;

			return (Math.PI * 2 * freq) / numSamples * time;
		}

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

			writer.Seek(4, SeekOrigin.Begin);
			uint fileSize = (uint)writer.BaseStream.Length;
			writer.Write(fileSize - 8);

			// clean up
			writer.Close();
			filestream.Close();
		}
	}
}
