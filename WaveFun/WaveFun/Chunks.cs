// https://blogs.msdn.microsoft.com/dawate/2009/06/24/intro-to-audio-programming-part-3-synthesizing-simple-wave-audio-using-c/

using System;

namespace WaveFun
{
	public class Chunks
	{
		public class WaveHeaderChunk        // 12 bytes
		{
			public String sGroupID;         // always "RIFF"
			public uint dwFileLength;       // total file length in bytes minus 8 for RIFF and WAVE
			public String sRiffType;        // always "WAVE"

			/// <summary>
			/// Initializes a WaveHeader object with default values.
			/// </summary>
			public WaveHeaderChunk()
			{
				dwFileLength = 0;
				sGroupID = "RIFF";
				sRiffType = "WAVE";
			}

		}

		public class WaveFormatChunk        // 26 bytes
		{
			public String sGroupID;         // four bytes: "fmt "
			public uint dwChunkSize;        // length of format chunk in bytes, excluding sGroupID and dwChunkSize
			public ushort wFormatTag;       // 1 (MS PCM)
			public ushort wChannels;        // number of channels
			public uint dwSamplesPerSec;    // freq of audio in Hz... 44100
			public uint dwAvgBytesPerSec;   // for estimating RAM allocation
			public ushort wBlockAlign;      // sample frame size, in bytes
			public uint dwBitsPerSample;    // bits per sample

			/// <summary>
			/// Initializes a format chunk with the following properties:
			/// Sample rate: 44100 Hz
			/// Channels: Stereo
			/// Bit depth: 16-bit
			/// </summary>
			public WaveFormatChunk()
			{
				sGroupID = "fmt "; // note trailing space to make 4 bytes
				dwChunkSize = 18;
				wFormatTag = 1;
				wChannels = 2;
				dwSamplesPerSec = 44100;
				dwBitsPerSample = 16;
				wBlockAlign = (ushort)(wChannels * (dwBitsPerSample / 8));
				dwAvgBytesPerSec = dwSamplesPerSec * wBlockAlign;
			}
		}

		public class WaveDataChunk          // 8 bytes + data
		{
			public String sGroupID;
			public uint dwChunkSize;
			public byte[] sample8;          // 8-bit
			public short[] sample16;        // 16-bit
			public float[] sample32;        // 32bit

			/// <summary>
			/// Initializes a new data chunk with default values.
			/// </summary>
			public WaveDataChunk()
			{
				sGroupID = "data";
				dwChunkSize = 0;
			}
		}

		public class WaveHeader
		{
			public WaveHeaderChunk header;
			public WaveFormatChunk format;
			public WaveDataChunk data;

			public uint numSamples;
			public double duration;

			public WaveHeader()
			{
				header = new WaveHeaderChunk();
				format = new WaveFormatChunk();
				data = new WaveDataChunk();
			}

			public void init(double duration, uint bitsPerSample = 16, uint sampleRate = 44100)
			{
				format.dwBitsPerSample = bitsPerSample;
				format.dwSamplesPerSec = sampleRate;

				this.duration = duration;

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
		}
	}
}
