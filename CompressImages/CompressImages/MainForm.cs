using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Drawing.Imaging;

namespace CompressImages
{
    public partial class MainForm : Form
    {
        private string directory_name = "Output"; // Output directory
        public MainForm()
        {
            InitializeComponent();

            //Create output directory
            createDirectory();
        }

        //This function is used to create output directory
        private void createDirectory()
        {
            if (!Directory.Exists(directory_name))
            {
                Directory.CreateDirectory(directory_name);
            }
        }

        private string createSavepath(string filename)
        {
            StringBuilder builder = new StringBuilder(directory_name);
            builder.Append("/");
            builder.Append(filename);

            return builder.ToString();
        }

        //get encoder 
        private static ImageCodecInfo GetCodecInfo(string mimeType)
        {
            foreach (ImageCodecInfo encoder in ImageCodecInfo.GetImageEncoders())
                if (encoder.MimeType == mimeType)
                    return encoder;
            throw new ArgumentOutOfRangeException(
                string.Format("'{0}' not supported", mimeType));
        }

        private void compressImage(System.Drawing.Image Image, ImageCodecInfo codec, long quality, string savepath)
        {
            EncoderParameters parameters = new EncoderParameters(1);
            parameters.Param[0] = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, quality);

            Image.Save(savepath, codec, parameters);
        }

        private void btn_select_Click(object sender, EventArgs e)
        {            
            try
            {
                OpenFileDialog opendialog = new OpenFileDialog();

                if (opendialog.ShowDialog() == DialogResult.OK)
                {
                    //Create image from file
                    System.Drawing.Image image = System.Drawing.Image.FromFile(opendialog.FileName);

                    //Create savepath
                    string savepath = createSavepath(opendialog.SafeFileName);

                    //Get encoder (Now just support jpeg)
                    ImageCodecInfo codec = GetCodecInfo("image/jpeg");

                    //Get quality
                    long quality = 50;

                    compressImage(image, codec, quality, savepath);
                    MessageBox.Show(savepath);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Da co loi xay ra: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
