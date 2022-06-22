using FSharp.Data.Runtime.WorldBank;
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
using System.Data.SqlClient;
using VideoLibrary;
using MediaToolkit;

namespace Media_Player
{
    public partial class Form1 : Form
    {
        String[] paths, files;
        Boolean format = true;
        public Form1()
        {
            InitializeComponent();
            Volume.Value = 60;
        }
        
        private void label2_Click(object sender, EventArgs e)
        {

        }


        private void btnPlaying_Click(object sender, EventArgs e)
        {
            bunifuShapes1.Top = btnPlaying.Top+5;
            bunifuPages1.SetPage(0);

        }

        private void btnExplore_Click(object sender, EventArgs e)
        {
            bunifuShapes1.Top=btnExplore.Top+5;
            bunifuPages1.SetPage(1);
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void imgbtnClose_Click(object sender, EventArgs e)
        {
            Environment.Exit(0);

        }

        private void track_list_SelectedIndexChanged(object sender, EventArgs e)
        {
            Player.URL = track_list.SelectedItems[0].ToString();
            bunifuShapes1.Top = btnPlaying.Top + 5;
            bunifuPages1.SetPage(0);
            try
            {
                var file=TagLib.File.Create(paths[track_list.SelectedIndex]);
                var bin = (byte[])(file.Tag.Pictures[0].Data.Data);
                Abum_Art.Image = Image.FromStream(new MemoryStream(bin));
                pictureBox4.Image = Image.FromStream(new MemoryStream(bin));
                lblTitle.Text = file.Tag.Title;
                lblAuthor.Text = file.Tag.Artists[0];
                lblGenre.Text= file.Tag.Genres[0];
            }
            catch (Exception ex)
            {

            }
        }

        private void btnPause_Click(object sender, EventArgs e)
        {
            Player.Ctlcontrols.stop();
        }

        private void btnPlay_Click(object sender, EventArgs e)
        {
            if(Player.status.ToLower().Contains("playing"))
            {
                Player.Ctlcontrols.pause();
            }
            else {
                Player.Ctlcontrols.play();
            }
            
        }

        private void btnNext_Click(object sender, EventArgs e)
        {
            if(track_list.SelectedIndex<track_list.Items.Count-1)
            {
                track_list.SelectedIndex = track_list.SelectedIndex + 1;
            }
            
        }

        private void btnPrevious_Click(object sender, EventArgs e)
        {
            if (track_list.SelectedIndex > 0)
            {
                track_list.SelectedIndex = track_list.SelectedIndex - 1;
            }
            
        }

        private void Player_PlayStateChange(object sender, AxWMPLib._WMPOCXEvents_PlayStateChangeEvent e)
        {
            lblHeader.Text = Player.status;
            imgVisualize.Visible = Player.status.ToLower().Contains("playing");
            //imgVisualize.Enabled = Player.status.ToLower().Contains("playing");

        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            lblTime.Text = Player.Ctlcontrols.currentPositionString;
            Slider.Value = (int)Player.Ctlcontrols.currentPosition;
        }

        private void Volume_Scroll(object sender, Utilities.BunifuSlider.BunifuHScrollBar.ScrollEventArgs e)
        {
            Player.settings.volume = Volume.Value;
        }

        private void Slider_MouseDown(object sender, MouseEventArgs e)
        {
            Player.Ctlcontrols.currentPosition = Player.currentMedia.duration * e.X / Slider.Width;
        }

        private void imgbtnMinimize_Click(object sender, EventArgs e)
        {
            
            this.WindowState=FormWindowState.Minimized;
        }

        

        private async void btnDownload_Click(object sender, EventArgs e)
        {
            using (FolderBrowserDialog dlg = new FolderBrowserDialog() { Description = "please select a folder" })
            {
                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    MessageBox.Show("Download has started please wait...", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    var yt = YouTube.Default;
                    var Video = await yt.GetVideoAsync(txtURL.Text);
                    lblStatus.Text = "Downloading...";
                    btnDownload.Enabled = false;
                    try
                    {
                        File.WriteAllBytes(dlg.SelectedPath + @"\" + Video.FullName, await Video.GetBytesAsync());

                    }
                    catch (Exception)
                    {
                        MessageBox.Show("No Ethernet Connection Please Try Again");
                        throw;
                    }

                    var inputfile = new MediaToolkit.Model.MediaFile { Filename = dlg.SelectedPath + @"\" + Video.FullName };
                    var outputfile = new MediaToolkit.Model.MediaFile { Filename = $"{dlg.SelectedPath + @"\" + Video.FullName}.mp3" };



                    using (var enging = new Engine())
                    {
                        enging.GetMetadata(inputfile);
                        enging.Convert(inputfile, outputfile);
                    }
                    if (format == true)
                    {
                        File.Delete(dlg.SelectedPath + @"\" + Video.FullName);
                    }
                    else
                    {
                        File.Delete($"{dlg.SelectedPath + @"\" + Video.FullName}.mp3");
                    }

                    MessageBox.Show("Download is Completed!", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    txtURL.Text = "";
                    lblStatus.Text = "......";
                    btnDownload.Enabled = true;
                }
                else
                {
                    MessageBox.Show("Please Select a Folder :", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

                }
            }

        }

        private void txtURL_MouseClick(object sender, MouseEventArgs e)
        {
            txtURL.Clear();
            txtURL.ForeColor = Color.Black;
        }

        private void btnYopen_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Multiselect = true;
            if (ofd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                files = ofd.FileNames;
                paths = ofd.FileNames;
                for (int i = 0; i < files.Length; i++)
                {
                    YT_tracklist.Items.Add(files[i]);
                }
            }
        }

        private void btnYoutube_Click(object sender, EventArgs e)
        {
            bunifuShapes1.Top = btnYoutube.Top + 5;
            bunifuPages1.SetPage(4);
        }

        private void YT_tracklist_SelectedIndexChanged(object sender, EventArgs e)
        {
            Player.URL = YT_tracklist.SelectedItems[0].ToString();
            bunifuShapes1.Top = btnPlaying.Top + 5;
            bunifuPages1.SetPage(0);
            try
            {
                var file = TagLib.File.Create(paths[track_list.SelectedIndex]);
                var bin = (byte[])(file.Tag.Pictures[0].Data.Data);
                Abum_Art.Image = Image.FromStream(new MemoryStream(bin));
                pictureBox4.Image = Image.FromStream(new MemoryStream(bin));
                lblTitle.Text = file.Tag.Title;
                lblAuthor.Text = file.Tag.Artists[0];
                lblGenre.Text = file.Tag.Genres[0];
            }
            catch (Exception ex)
            {

            }

        }

        private void btnOpen_Click(object sender, EventArgs e)
        {
           
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Multiselect = true;
            if(ofd.ShowDialog()==System.Windows.Forms.DialogResult.OK)
            {
                files=ofd.FileNames;
                paths = ofd.FileNames;
                for(int i=0;i<files.Length;i++)
                {
                    track_list.Items.Add(files[i]);
                }
            }

        }
    }
}
