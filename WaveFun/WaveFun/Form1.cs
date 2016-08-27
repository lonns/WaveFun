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
	public partial class Form1 : Form
	{
		public Form1()
		{
			InitializeComponent();
		}

		private void Form1_Load(object sender, EventArgs e)
		{

		}

		private void button1_Click(object sender, EventArgs e)
		{
			string filePath = @"C:\Users\whoever\Desktop\test.wav";
			WaveGenerator wave = new WaveGenerator(WaveGenerator.WaveExampleType.AllNotes);
			wave.Save(filePath);

			SoundPlayer player = new SoundPlayer(filePath);
			player.Play();
		}

		private void numericUpDown1_ValueChanged(object sender, EventArgs e)
		{

		}
	}
}
