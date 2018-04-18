using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
//using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing.Imaging;
using System.Threading;
using System.IO;

namespace WindowsFormsApplication1
{
    public partial class Form1 : Form
    {
        private string[] _imagePaths;
        private string _outpath = "Output/";
        private int _qualify = 50;

        public Form1()
        {
            InitializeComponent();
        }

        private static ImageCodecInfo GetEncoderInfo(string mimeType)
        {
            // Get image codecs for all image formats 
            ImageCodecInfo[] codecs = ImageCodecInfo.GetImageEncoders();

            // Find the correct image codec 
            for (int i = 0; i < codecs.Length; i++)
                if (codecs[i].MimeType == mimeType)
                    return codecs[i];

            return null;
        }

        public static void SaveJpeg(string path, Image image, int quality)
        {
            //ensure the quality is within the correct range
            if ((quality < 0) || (quality > 100))
            {
                //create the error message
                string error = string.Format("Jpeg image quality must be between 0 and 100, with 100 being the highest quality.  A value of {0} was specified.", quality);
                //throw a helpful exception
                throw new ArgumentOutOfRangeException(error);
            }

            //create an encoder parameter for the image quality
            EncoderParameter qualityParam = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, quality);
            //get the jpeg codec
            ImageCodecInfo jpegCodec = GetEncoderInfo("image/jpeg");

            //create a collection of all parameters that we will pass to the encoder
            EncoderParameters encoderParams = new EncoderParameters(1);
            //set the quality parameter for the codec
            encoderParams.Param[0] = qualityParam;
            //save the image using the codec and the parameters
            image.Save(path, jpegCodec, encoderParams);
        }

        private void btnBrowse_Click(object sender, EventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Multiselect = true;

            if (dialog.ShowDialog() == DialogResult.OK)
            {
                this._imagePaths = dialog.FileNames;
                
                label1.Text += _imagePaths.Length.ToString() + " files";
            }

            progressBar1.Minimum = 0;
            progressBar1.Maximum = _imagePaths.Length;
        }

        delegate void PathAgrReturning(string path); // Delegate cua Image
        delegate void ProgressAgrReturning();

        private void SetProgress()
        {
            if (progressBar1.InvokeRequired)
            {
                ProgressAgrReturning d = new ProgressAgrReturning(SetProgress);
                this.Invoke(d);
            }
            else
            {
                progressBar1.Value += 1;
            }
        }


        private void SetImage(string path)
        {
            //Image image = Image.FromFile(path);

            if (pictureBox1.InvokeRequired)
            {
                PathAgrReturning d = new PathAgrReturning(SetImage);
                this.Invoke(d, new object[] { path });
            }
            else
            {
                if (pictureBox1.Image == null)
                {
                    Image image = Image.FromFile(path);

                    pictureBox1.Image = image;
                    pictureBox1.Refresh();
                }
                else
                {
                    Image old = pictureBox1.Image;
                    pictureBox1.Image = Image.FromFile(path);
                    pictureBox1.Refresh();
                    old.Dispose();
                }
            }
        }

        private void Process()
        {
            try
            {
                foreach (string path in _imagePaths)
                {
                    Image image = Image.FromFile(path); // Hinh goc
                    string outpath = _outpath + "/" + Path.GetFileName(path); // Path ra
                    SaveJpeg(outpath, image, this._qualify); // Compress anh
                    image.Dispose(); /// Xoa anh

                    SetImage(path);
                    SetProgress();

                    Thread.Sleep(500);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        

        private void btnCompress_Click(object sender, EventArgs e)
        {
            int files = _imagePaths.Length;
            this._qualify = Int32.Parse(txtQualify.Text);

            Thread t1 = new Thread(Process);
            t1.Start();
            
        }

        private void btnOuput_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog dialog = new FolderBrowserDialog();

            if (dialog.ShowDialog() == DialogResult.OK)
            {
                _outpath = dialog.SelectedPath;

                la_outpath.Text = _outpath;
            }

        }
    }
}
