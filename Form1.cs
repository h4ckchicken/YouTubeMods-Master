using Microsoft.Toolkit.Uwp.Notifications;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.NetworkInformation;
using System.Reflection.Emit;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using YoutubeExplode;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using YoutubeExplode.Converter;
using YoutubeExplode.Videos.Streams;
using YoutubeExplode.Videos;


namespace YouTubeMods
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            DarkTitleBarClass.UseImmersiveDarkMode(Handle, true);
            this.AutoScroll = true;
            this.HorizontalScroll.Enabled = true;
            this.HorizontalScroll.Visible = true;          
        }

        internal class DarkTitleBarClass
        {
            [DllImport("dwmapi.dll")]
            private static extern int DwmSetWindowAttribute(IntPtr hwnd, int attr,
            ref int attrValue, int attrSize);

            private const int DWMWA_USE_IMMERSIVE_DARK_MODE_BEFORE_20H1 = 19;
            private const int DWMWA_USE_IMMERSIVE_DARK_MODE = 20;

            internal static bool UseImmersiveDarkMode(IntPtr handle, bool enabled)
            {
                if (IsWindows10OrGreater(17763))
                {
                    var attribute = DWMWA_USE_IMMERSIVE_DARK_MODE_BEFORE_20H1;
                    if (IsWindows10OrGreater(18985))
                    {
                        attribute = DWMWA_USE_IMMERSIVE_DARK_MODE;
                    }

                    int useImmersiveDarkMode = enabled ? 1 : 0;
                    return DwmSetWindowAttribute(handle, attribute, ref useImmersiveDarkMode, sizeof(int)) == 0;
                }

                return false;
            }
            private static bool IsWindows10OrGreater(int build = -1)
            {
                return Environment.OSVersion.Version.Major >= 10 && Environment.OSVersion.Version.Build >= build;
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void yenile_Click(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                System.Diagnostics.Process.Start("chrome.exe", "https://www.youtube.com/");
            }
            catch
            {
                System.Diagnostics.Process.Start("C:\\Program Files (x86)\\Microsoft\\Edge\\Application\\msedge.exe", "https://www.youtube.com/");
            }
        }

        private void OpenFileButton_Click(object sender, EventArgs e)
        {
            Process.Start("explorer.exe", Environment.GetFolderPath(Environment.SpecialFolder.MyVideos));
        }

        private void DownloadVideoButton_Click(object sender, EventArgs e)
        {
            SearchButton.Enabled = false;
            DownloadVideoButton.Enabled = true;
            var youtube = new YoutubeClient();
            info.Text = "Ýndirme Baþladý!";
            DownloadVideo(youtube, SearchBox.Text);
            DownloadVideoButton.Enabled = false;
            SearchButton.Enabled = false;
        }

        private async void DownloadVideo(YoutubeClient youtube, string url)
        {
            try
            {
                info.Text = "";
                pictureBox2.ImageLocation = "loading.gif";
                var streamManifest = await youtube.Videos.Streams.GetManifestAsync(url);
                var streamInfo = streamManifest.GetMuxedStreams().GetWithHighestVideoQuality();
                var video = await youtube.Videos.GetAsync(url);
                var stream = await youtube.Videos.Streams.GetAsync(streamInfo);
                var audioStreamInfo = streamManifest.GetAudioStreams().GetWithHighestBitrate();
                var videoStreamInfo = streamManifest.GetVideoStreams().First(s => s.VideoQuality.Label == comboBox1.SelectedItem.ToString());
                var streamInfos = new IStreamInfo[] { audioStreamInfo, videoStreamInfo };
                String videoName = Environment.GetFolderPath(Environment.SpecialFolder.MyVideos) + "\\" + "Video_" + video.Id + ".mp4";
                await youtube.Videos.DownloadAsync(streamInfos, new ConversionRequestBuilder(videoName).Build());
                new ToastContentBuilder().AddText("Video Baþarýyla Ýndirildi!").AddAppLogoOverride(new Uri("file:///" + Path.GetFullPath(@"icon.ico"), UriKind.Absolute), ToastGenericAppLogoCrop.Circle);Show();
                SearchButton.Enabled = true;
                pictureBox2.ImageLocation = null;
                info.Text = "Video Ýndirildi!";
                DownloadVideoButton.Enabled = true;
                if (check1.Checked == true)
                {
                    Process.Start("explorer.exe", Environment.GetFolderPath(Environment.SpecialFolder.MyVideos));
                }
                else
                {
                    //biþe yapma!
                }
            }
            catch (Exception ex)
            {
                new ToastContentBuilder().AddText(ex.Message);Show();
                MessageBox.Show("Lütfen Kaliteyi Seçiniz!", "Uyarý", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                info.Text = "";
                pictureBox2.ImageLocation = null;
                SearchButton.Enabled = true;
                DownloadVideoButton.Enabled = true;
            }


        }

        private async void SearchButton_Click(object sender, EventArgs e)
        {
            try
            {
                SearchButton.Enabled = false;
                info.Text = "Link Alýnýyor...";
                var youtube = new YoutubeClient();
                var video = await youtube.Videos.GetAsync(Clipboard.GetText());
                SearchBox.Text = Clipboard.GetText();
                var streamManifest = await youtube.Videos.Streams.GetManifestAsync(SearchBox.Text);
                VideoTitle.Text = video.Title.ToString();
                VideoAuthor.Text = video.Author.ChannelTitle;
                VideoDuration.Text = video.Duration.ToString();
                VideoUrl.Text = video.Url.ToString();
                DownloadVideoButton.Enabled = true;
                comboBox1.Items.Clear();
                comboBox1.Items.Add("360p");
                comboBox1.Items.Add("720p");
                comboBox1.Items.Add("1080p");
                ThumbnailBox.ImageLocation = "https://img.youtube.com/vi/" + video.Id + "/hqdefault.jpg";
                info.Text = "Kaliteyi Seçin!";
                SearchButton.Enabled = true;
                check1.Enabled = true;
            }
            catch
            {
                MessageBox.Show("Lütfen Linki Kontrol Edin!", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                info.Text = "";
                SearchButton.Enabled = true;
            }

        }

        private async void SearchButton2_Click(object sender, EventArgs e)
        {
            try
            {
                SearchButton2.Enabled = false;
                info2.Text = "Link Alýnýyor...";
                var youtube = new YoutubeClient();
                var video = await youtube.Videos.GetAsync(Clipboard.GetText());
                SearchBox2.Text = Clipboard.GetText();
                var streamManifest = await youtube.Videos.Streams.GetManifestAsync(SearchBox2.Text);
                VideoTitle2.Text = video.Title.ToString();
                VideoAuthor2.Text = video.Author.ChannelTitle;
                VideoDuration2.Text = video.Duration.ToString();
                VideoUrl2.Text = video.Url.ToString();
                ThumbnailBox2.ImageLocation = "https://img.youtube.com/vi/" + video.Id + "/hqdefault.jpg";
                DownloadVideoButton2.Enabled = true;
                info2.Text = "Hazýr!";
                SearchButton2.Enabled = true;
                check2.Enabled = true;
            }
            catch
            {
                MessageBox.Show("Lütfen Linki Kontrol Edin!", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                info2.Text = "";
                pictureBox2.ImageLocation = null;
                SearchButton2.Enabled = true;
            }
        }

        private void OpenFileButton2_Click(object sender, EventArgs e)
        {
            Process.Start("explorer.exe", Environment.GetFolderPath(Environment.SpecialFolder.MyVideos));
        }

        private void DownloadVideoButton2_Click(object sender, EventArgs e)
        {
            SearchButton2.Enabled = false;
            DownloadVideoButton2.Enabled = true;
            var youtube = new YoutubeClient();
            DownloadVideo2(youtube, SearchBox2.Text);;
            DownloadVideoButton2.Enabled = false;
            SearchButton2.Enabled = false;

        }

        private async void DownloadVideo2(YoutubeClient youtube, string url)
        {
            try
            {
                info2.Text = "";
                pictureBox3.ImageLocation = "loading.gif";
                var streamManifest = await youtube.Videos.Streams.GetManifestAsync(url);
                var streamInfo = streamManifest.GetMuxedStreams().GetWithHighestVideoQuality();
                var video = await youtube.Videos.GetAsync(url);
                var stream = await youtube.Videos.Streams.GetAsync(streamInfo);
                var audioStreamInfo = streamManifest.GetAudioStreams().GetWithHighestBitrate();
                var streamInfos = new IStreamInfo[] { audioStreamInfo};
                String videoName = Environment.GetFolderPath(Environment.SpecialFolder.MyVideos) + "\\" + "Music_" + video.Id + ".mp3";
                await youtube.Videos.DownloadAsync(streamInfos, new ConversionRequestBuilder(videoName).Build());
                new ToastContentBuilder().AddText("Müzik Baþarýyla Ýndirildi!").AddAppLogoOverride(new Uri("file:///" + Path.GetFullPath(@"icon.ico"), UriKind.Absolute), ToastGenericAppLogoCrop.Circle); Show();
                SearchButton2.Enabled = true;
                pictureBox3.ImageLocation = null;
                info2.Text = "Ýndirildi!";
                DownloadVideoButton2.Enabled = true;
                SearchButton2.Enabled = true;
                if (check2.Checked == true)
                {
                    Process.Start("explorer.exe", Environment.GetFolderPath(Environment.SpecialFolder.MyVideos));
                }
                else
                {
                    //biþe yapma!
                }
            }
            catch (Exception ex)
            {
                new ToastContentBuilder().AddText(ex.Message); Show();
                SearchButton2.Enabled = true;
            }


        }

        private void hakkýmda_Click(object sender, EventArgs e)
        {
            Form2 f2 = new Form2();
            f2.Show();

        }

        private void button2_Click(object sender, EventArgs e)
        {
            VideoTitle2.Text = null;
            VideoAuthor2.Text = null;
            VideoDuration2.Text = null;
            VideoUrl2.Text = null;
            ThumbnailBox2.ImageLocation = null;
            DownloadVideoButton2.Enabled = false;
            SearchBox2.Text = null;
            info2.Text = null;
            check2.Enabled = false;
            check2.Checked = false;

            VideoTitle.Text = null;
            VideoAuthor.Text = null;
            VideoDuration.Text = null;
            VideoUrl.Text = null;
            ThumbnailBox.ImageLocation = null;
            DownloadVideoButton.Enabled = false;
            SearchBox.Text = null;
            info.Text = null;
            check1.Enabled = false;
            check1.Checked = false;
            comboBox1.Items.Clear();
            comboBox1.Text = null;
        }
    }
}