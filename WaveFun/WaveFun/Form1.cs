using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Media;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WaveFun
{
	public partial class WaveForm : Form
	{

		public double frequency;

		private Random rand = new Random();

		private WaveGenerator wave;

		List<double> frequencies;
		List<int> keysPressed;

		public WaveForm()
		{
			InitializeComponent();
		}

		private void Form1_Load(object sender, EventArgs e)
		{
			wave = new WaveGenerator();
			frequencies = new List<double>();
			keysPressed = new List<int>();
		}

		private void button1_Click(object sender, EventArgs e)
		{
			//string filePath = @"C:\Users\whoever\Desktop\test.wav";
			frequencies.Clear();
			frequencies.Add(rand.Next(100, 800));
			numericUpDown1.Value = (decimal) frequency;
			//WaveGenerator wave = new WaveGenerator(WaveGenerator.WaveExampleType.Streaming,
			/*rand.Next((int) WaveGenerator.Notes.C3, (int) WaveGenerator.Notes.B6)*/
			//	frequency
			//	);
			//wave.Save(filePath);
			wave.Play(WaveGenerator.WaveExampleType.Streaming, frequencies);
			frequencies.Clear();
			//SoundPlayer player = new SoundPlayer(filePath);
			//player.Play();
		}

		private void numericUpDown1_ValueChanged(object sender, EventArgs e)
		{
			frequencies.Add( (double) numericUpDown1.Value );
			//numericUpDown1.Value = (decimal)frequency;

			wave.Play(WaveGenerator.WaveExampleType.Streaming, frequencies);
			//WaveGenerator wave = new WaveGenerator(WaveGenerator.WaveExampleType.Streaming, frequency);
		}

		//private void WaveForm_KeyPress(object sender, KeyPressEventArgs e)
		//{
		//	if (!playing)
		//	{
		//		playing = true;
		//		char letter = e.KeyChar;

		//		frequency = letter * 20;
		//		WaveGenerator wave = new WaveGenerator(WaveGenerator.WaveExampleType.Streaming, frequency);
		//	}

		//}

		//private void WaveForm_KeyDown(object sender, KeyEventArgs e)
		//{
		//	frequency = e.KeyValue * 20;
		//	Console.WriteLine(frequency);
		//	WaveGenerator wave = new WaveGenerator(WaveGenerator.WaveExampleType.Streaming, frequency);

		//}
		bool playing = false;
		private void button1_KeyDown(object sender, KeyEventArgs e)
		{
			if (!playing || !keysPressed.Contains(e.KeyValue))
			{
				playing = true;
				keysPressed.Add(e.KeyValue);
				frequencies.Add((double)(e.KeyValue * e.KeyValue *.1));
				wave.Play(WaveGenerator.WaveExampleType.Streaming, frequencies);
				//WaveGenerator wave = new WaveGenerator(WaveGenerator.WaveExampleType.Streaming, frequency);
			}
		}

		private void button1_KeyUp(object sender, KeyEventArgs e)
		{
			keysPressed.Remove(e.KeyValue);
			frequencies.Remove(e.KeyValue * e.KeyValue * .1);

			if (keysPressed.Count <= 0)
			{
				playing = false;
				wave.player.Stop();
			} else
			{
				wave.Play(WaveGenerator.WaveExampleType.Streaming, frequencies);
			}
		}

		private void button1_KeyPress(object sender, KeyEventArgs e)
		{

			if (!playing || !keysPressed.Contains(e.KeyValue))
			{
				playing = true;
				keysPressed.Add(e.KeyValue);
				frequencies.Add((double)(e.KeyValue *  e.KeyValue * .1));
				Console.WriteLine(keysPressed);
				Console.WriteLine(frequencies);
				wave.Play(WaveGenerator.WaveExampleType.Streaming, frequencies);
				//WaveGenerator wave = new WaveGenerator(WaveGenerator.WaveExampleType.Streaming, frequency);
			}
		}
	}
}
