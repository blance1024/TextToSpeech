using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Speech.Synthesis;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TextToSpeech
{
    public partial class Form1 : Form
    {
        private SpeechSynthesizer reader;
        public Form1()
        {
            InitializeComponent();
        }

        private void buttonStart_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(textBox1.Text))
            {
                MessageBox.Show("需要输入转换的文字", "错误提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                reader = new SpeechSynthesizer();
                reader.Rate = trackBar1.Value;
                reader.SelectVoice(comboBox1.Text);
                reader.SpeakAsync(textBox1.Text.Trim());
                buttonStart.Enabled = false;
                buttonPause.Enabled = true;
                buttonStop.Enabled = true;
                reader.SpeakCompleted += new EventHandler<SpeakCompletedEventArgs>(reader_SpeakCompleted);
            }
        }

        private void buttonPause_Click(object sender, EventArgs e)
        {
            if (reader.State == SynthesizerState.Speaking)
            {
                reader.Pause();
                buttonPause.Enabled = false;
                buttonResume.Enabled = true;
            }
        }

        private void buttonResume_Click(object sender, EventArgs e)
        {
            if (reader.State == SynthesizerState.Paused)
            {
                reader.Resume();
                buttonPause.Enabled = true;
                buttonResume.Enabled = false;
            }
        }

        private void buttonStop_Click(object sender, EventArgs e)
        {
            if (reader != null)
            {
                reader.Dispose();
                buttonStart.Enabled = true;
                buttonPause.Enabled = false;
                buttonStop.Enabled = false;
                buttonResume.Enabled = false;
            }
        }

        //event handler 
        void reader_SpeakCompleted(object sender, SpeakCompletedEventArgs e)
        {
            buttonStart.Enabled = true;
            buttonPause.Enabled = false;
            buttonStop.Enabled = false;
            buttonResume.Enabled = false;
        }

        private void buttonSave_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(textBox1.Text))
            {
                MessageBox.Show("需要输入转换的文字", "错误提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                var _in = saveFileDialog1.ShowDialog();
                if (_in == DialogResult.OK)
                {
                    if (reader != null)
                    {
                        reader.Dispose();
                    }
                    buttonStart.Enabled = false;
                    buttonSave.Enabled = false;
                    reader = new SpeechSynthesizer();
                    reader.Rate = trackBar1.Value;
                    reader.SelectVoice(comboBox1.Text);
                    reader.SetOutputToWaveFile(saveFileDialog1.FileName);
                    reader.SpeakAsync(textBox1.Text.Trim());
                    reader.SpeakCompleted += new EventHandler<SpeakCompletedEventArgs>(reader_SaveCompleted);
                }
            }
        }

        void reader_SaveCompleted(object sender, SpeakCompletedEventArgs e)
        {
            buttonStart.Enabled = true;
            buttonSave.Enabled = true;
            reader.SetOutputToNull();
            MessageBox.Show("保存完成", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            buttonStop_Click(sender, e);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            if (reader == null)
                reader = new SpeechSynthesizer();
            var _ab = reader.GetInstalledVoices();
            comboBox1.DataSource = _ab.Where(r=>r.VoiceInfo.Culture.Name== CultureInfo.CurrentCulture.Name).Select(r => r.VoiceInfo.Name).ToList();
        }
    }
}
