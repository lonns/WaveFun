// https://blogs.msdn.microsoft.com/dawate/2009/06/24/intro-to-audio-programming-part-3-synthesizing-simple-wave-audio-using-c/

using System;

namespace WaveFun
{
	public class Chunks
	{
		public class WaveHeader
		{
			public String sGroupID;			// always "RIFF"
			public uint dwFileLength;		// total file length in bytes minus 8 for RIFF and WAVE
			public String sRiffType;		// always "WAVE"

			/// <summary>
			/// Initializes a WaveHeader object with default values.
			/// </summary>
			public WaveHeader()
			{
				dwFileLength = 0;
				sGroupID ="RIFF";
				sRiffType = "WAVE";
			}

		}

		public class WaveFormatChunk
		{
			public String sGroupID;			// four bytes: "fmt "
			public uint dwChunkSize;		// length of header in bytes
			public ushort wFormatTag;		// 1 (MS PCM)
			public ushort wChannels;		// number of channels
			public uint dwSamplesPerSec;	// freq of audio in Hz... 44100
			public uint dwAvgBytesPerSec;	// for estimating RAM allocation
			public ushort wBlockAlign;		// sample frame size, in bytes
			public uint dwBitsPerSample;	// bits per sample

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

		public class WaveDataChunk
		{
			public String sGroupID;
			public uint dwChunkSize;
			public byte[] sample8; // 8-bit
			public short[] sample16; // 16-bit
			public float[] sample32; // 32bit

			/// <summary>
			/// Initializes a new data chunk with default values.
			/// </summary>
			public WaveDataChunk()
			{
				sGroupID = "data";
				sample16 = new short[0];
				dwChunkSize = 0;
			}
		}
	}
}
