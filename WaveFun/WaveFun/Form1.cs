using System;
using System.Collections.Generic;
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
			frequencies.Clear();
			frequencies.Add(rand.Next(100, 800));
			numericUpDown1.Value = (decimal) frequency;
			wave.Play(frequencies.ToArray(), 16, 44100);
			frequencies.Clear();
		}

		private void numericUpDown1_ValueChanged(object sender, EventArgs e)
		{
			frequencies.Add( (double) numericUpDown1.Value );

			wave.Play(frequencies.ToArray());
		}

		bool playing = false;
		private void button1_KeyDown(object sender, KeyEventArgs e)
		{
			if (!playing || !keysPressed.Contains(e.KeyValue))
			{
				playing = true;
				keysPressed.Add(e.KeyValue);
				frequencies.Add((double)(e.KeyValue * 10));
				wave.Play(frequencies.ToArray());
			}
		}

		private void button1_KeyUp(object sender, KeyEventArgs e)
		{
			keysPressed.Remove(e.KeyValue);
			frequencies.Remove(e.KeyValue * 10);

			if (keysPressed.Count <= 0)
			{
				playing = false;
				wave.player.Stop();
			} else
			{
				wave.Play(frequencies.ToArray());
			}
		}

		//private void button1_KeyPress(object sender, KeyEventArgs e)
		//{

		//	if (!playing || !keysPressed.Contains(e.KeyValue))
		//	{
		//		playing = true;
		//		keysPressed.Add(e.KeyValue);
		//		frequencies.Add((double)(e.KeyValue *  e.KeyValue * .1));
		//		Console.WriteLine(keysPressed);
		//		Console.WriteLine(frequencies);
		//		wave.Play(frequencies.ToArray());
		//	}
		//}
	}
}
