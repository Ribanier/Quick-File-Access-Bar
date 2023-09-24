using IWshRuntimeLibrary;
using System;
using System.Data;
using System.Data.SQLite;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace Quick_File_Access_Bar
{
    public partial class QuickFileAccessBar : Form
    {
        public QuickFileAccessBar()
        {
            InitializeComponent();
        }
        [DllImport("shell32.dll", CharSet = CharSet.Auto)]
        public static extern IntPtr ExtractAssociatedIcon(IntPtr hInst, string lpIconPath, out ushort lpiIcon);

        private void QuickFileAccessBar_Load(object sender, EventArgs e)
        {
            this.Icon = new Icon("Appicon.ico");
            //Başlangıç ayarı
            button1.Text = "<";
            this.Width = 10;
            this.Height = 26;
            int frmheightloc =
                screenY % 2 == 0 ? (screenY / 2) - (this.Height / 2) : (++screenY / 2) - (this.Height / 2);
            this.Location = new Point(screenX - this.Width, frmheightloc);
            Connect();
            SQLiteCommand com =
                new SQLiteCommand("SELECT * FROM sqlite_master WHERE type = 'table' and name=@tablename", cnt);
            com.Parameters.AddWithValue("@tablename", "Dir");
            SQLiteDataReader reader = com.ExecuteReader();
            if (reader.HasRows == false)
            {
                var settings = new Settings();
                settings.Show();
                buttonCounter++;
                com.Dispose();
                reader.Close();
                cnt.Close();
                trackBar1.Value = TrackbarControl();
            }
            else
            {
                SQLiteCommand cmd = new SQLiteCommand("select * from dir", cnt);
                var rd = cmd.ExecuteReader();
                if (rd.HasRows == false)
                {
                    var settings = new Settings();
                    settings.Show();
                    buttonCounter++;
                    cmd.Dispose();
                    com.Dispose();
                    rd.Close();
                    reader.Close();
                    cnt.Close();
                    trackBar1.Value = TrackbarControl();
                }

            }

            cnt.Close();
            trackBar1.Value = TrackbarControl();
        }

        #region Methods

        void Connect() //veri tabanı bağlantısı kontrolu 
        {
            if (cnt.State == ConnectionState.Closed) cnt.Open();
        }

        public int TrackbarControl() //trackbarın veritabanındaki değerini kontrol eder
        {
            try
            {
                Connect();
                SQLiteCommand cmd =
                    new SQLiteCommand("select status from Settings where settingname =@settingname", cnt);
                cmd.Parameters.AddWithValue("@settingname", "shortcutsize");
                int a = Convert.ToInt32(cmd.ExecuteScalar());
                cmd.Dispose();
                cnt.Close();
                return a;
            }
            catch
            {
                return 30;
            }

        }

        public void Tasarim() //uygulamanın klasor olusumu ve düzenini tasarlamak
        {

            Connect();
            SQLiteCommand cmd = new SQLiteCommand("Select Path from Dir", cnt);
            SQLiteDataReader dr = cmd.ExecuteReader();
            while (dr.Read())
            {
                if (Directory.Exists(dr["Path"].ToString()))
                {
                    path = dr["Path"].ToString();
                    int fileCount = Directory.GetDirectories(path).Count() + Directory.GetFiles(path).Count();
                    string[] allDirAndFile = new string[fileCount]; //ilk klasörler sonra dostalar
                    int b = 0;
                    foreach (string dir in Directory.GetDirectories(path))
                        allDirAndFile[b++] = dir;
                    foreach (string dir2 in Directory.GetFiles(path))
                        allDirAndFile[b++] = dir2;
                    AddDynamicObject addDynamicObject = new AddDynamicObject();


                    var tabPage = addDynamicObject.addTabPage(path);
                    tabControl1.TabPages.Add(tabPage);

                    int tabPageWidth = tabPage.Width, tabPageHeight = tabPage.Height;
                    int scutwidthheight = trackBar1.Value;
                    int yanyanasirasi = tabPageWidth / scutwidthheight;
                    int fontSize = scutwidthheight > 82 ? 11 :
                        scutwidthheight > 64 ? 10 :
                        scutwidthheight > 46 ? 9 : 8;
                    int locX = 0, locY = 0;
                    int sayac1 = 0;
                    for (int i = 0; i < fileCount; i++)
                    {
                        var fileExtension = i < Directory.GetDirectories(path).Count()
                            ? ""
                            : new System.IO.FileInfo(allDirAndFile[i]).Extension;
                        string iconPic = @"Extension\default.png";
                        if (fileExtension != "")
                        {
                            if (System.IO.File.Exists(@"Extension\" + fileExtension.Split(".").Last() + ".png"))
                                iconPic = @"Extension\" + fileExtension.Split(".").Last() + ".png";
                        }

                        // MessageBox.Show(fileextension);
                        var pictureBox = addDynamicObject.addPictureBox(scutwidthheight, locY, locX, i, allDirAndFile);

                        if (fileExtension == ".lnk" || fileExtension == ".exe")
                        {
                            try
                            {
                                if (fileExtension == ".lnk")
                                {
                                    WshShell shell = new WshShell();
                                    WshShortcut shortcut = (WshShortcut)shell.CreateShortcut(allDirAndFile[i]);
                                    if (System.IO.Directory.Exists(shortcut.TargetPath))
                                        pictureBox.ImageLocation = @"Extension\directory.png";
                                    else
                                        pictureBox.Image =
                                            Bitmap.FromHicon(Icon.ExtractAssociatedIcon(shortcut.TargetPath).Handle);
                                }
                                else
                                {
                                    Icon fileIcon = GetFileIcon(allDirAndFile[i]);

                                    if (fileIcon != null)
                                    {
                                        // İkonu kullanabilirsiniz
                                        // Örneğin bir PictureBox içinde gösterebilirsiniz.

                                        pictureBox.Image = fileIcon.ToBitmap();


                                        // İkona erişimi serbest bırakın
                                        fileIcon.Dispose();
                                    }
                                    //  pictureBox.Image =Bitmap.FromHicon(Icon.ExtractAssociatedIcon(allDirAndFile[i]).Handle);
                                }

                            }
                            catch
                            {
                                pictureBox.ImageLocation = i < Directory.GetDirectories(path).Count()
                                    ? @"Extension\directory.png"
                                    : iconPic;
                            }
                        }
                        else
                            pictureBox.ImageLocation = i < Directory.GetDirectories(path).Count()
                                ? @"Extension\directory.png"
                                : iconPic;

                        var label = addDynamicObject.addLabel(allDirAndFile, i, scutwidthheight, locY, locX, fontSize);

                        pictureBox.Click += (sender, args) =>
                        {
                            //MessageBox.Show($"Picture #: {((PictureBox)sender).Tag}, Name: {((Control)sender).Name}, Current i:{i}");
                            OpenFile(((PictureBox)sender).Tag.ToString());
                        };
                        tabPage.Controls.Add(pictureBox);

                        label.MouseHover += (sender, args) =>
                        {
                            ToolTip tp = new ToolTip();
                            tp.SetToolTip(label, ((Label)sender).Tag.ToString());
                        };
                        tabPage.Controls.Add(label);

                        sayac1++;
                        if (sayac1 == yanyanasirasi)
                        {
                            sayac1 = 0;
                            locY += scutwidthheight + label.Height;
                            locX = 0;
                        }
                        else
                            locX += scutwidthheight;
                    }

                }
            }

            dr.Close();
            cmd.Dispose();
            cnt.Close();
            TabPage addTabPage = new TabPage()
            {
                Text = "+",

            };
            tabControl1.Controls.Add(addTabPage);

            selectedTab();
        }
        public static Icon GetFileIcon(string filePath)
        {
            ushort iconIndex = 0;
            IntPtr hIcon = ExtractAssociatedIcon(IntPtr.Zero, filePath, out iconIndex);

            if (hIcon != IntPtr.Zero)
            {
                return Icon.FromHandle(hIcon);
            }

            return null;
        }
        void Exiting() //Uygulama kapatılırken yapılacaklar
        {
            Connect();
            string addshortcuts = "update Settings set status=@newvalue where settingname =@settingname";
            SQLiteCommand comm2 = new SQLiteCommand(addshortcuts, cnt);
            comm2.Parameters.AddWithValue("@newvalue", trackBar1.Value);
            comm2.Parameters.AddWithValue("@settingname", "shortcutsize");
            comm2.ExecuteNonQuery();
            comm2.Dispose();
            cnt.Close();
        }

        public static void OpenFile(string path, bool isDirectory = false) //klasör açma
        {
            ProcessStartInfo pi = new ProcessStartInfo(path)
            {
                Arguments = Path.GetFileName(path),
                UseShellExecute = true,
                WindowStyle = ProcessWindowStyle.Normal,
                Verb = "OPEN"
            };
            Process proc = new Process
            {
                StartInfo = pi
            };
            proc.Start();
        }

        #endregion

        #region Classes

        class AddDynamicObject
        {
            public TabPage addTabPage(string tagAndText)
            {
                var tabPage = new TabPage
                {
                    Text = tagAndText.Split('\\').Last() == "" ? tagAndText : tagAndText.Split('\\').Last(),
                    Tag = tagAndText,
                    Name = tagAndText,
                    BackColor = Color.Transparent,
                    ForeColor = Color.Black,
                    Font = new Font("Verdana", 12),

                    AutoScroll = true
                };
                return tabPage;
            }

            public Label addLabel(string[] allDirAndFile, int i, int scutwidthheight, int locy, int locx,
                int fontboyut)
            {
                var label = new Label
                {
                    Text = allDirAndFile[i].Split("\\").Last(),
                    MaximumSize = new Size(scutwidthheight, 20),
                    Top = locy + scutwidthheight,
                    Left = locx,
                    ForeColor = Color.Black,
                    BackColor = Color.Transparent,
                    Font = new Font("Verdana", fontboyut),
                    Tag = allDirAndFile[i].Split("\\").Last()
                };
                return label;
            }

            public PictureBox addPictureBox(int scutwidthheight, int locy, int locx, int i, string[] allDirAndFile)
            {
                var pictureBox = new PictureBox
                {
                    Width = scutwidthheight,
                    Height = scutwidthheight,
                    Top = locy,
                    Left = locx,
                    //Location=new Point(tpagewidth,tpageheight),
                    Name = $"Pbox{i}",
                    Tag = allDirAndFile[i],
                    SizeMode = PictureBoxSizeMode.StretchImage,
                    BackColor = Color.Transparent,
                    // ImageLocation = i < dizi.Count() ? @"Extension\directory.png" : iconpic,
                    // Image = alldirandfile[i].Split(".").Last() == "exe" ? Bitmap.FromHicon(new Icon(Icon.ExtractAssociatedIcon(alldirandfile[i]), new Size(48, 48)).Handle): Bitmap.FromHicon(new Icon("a.ico", new Size(48, 48)).Handle)
                };
                return pictureBox;
            }

        }

        #endregion

        private void RightSizeAndLoc()
        {
            if (this.Height != screenY)
            {
                button1.Text = ">";
                this.Height = screenY;
                this.Width = screenX % 4 == 0 ? screenX / 4 : (screenX - (screenX % 4)) / 4;
                this.Location = new Point(screenX - this.Width, 0);
            }
            else
            {
                button1.Text = "<";
                this.Width = 10;
                this.Height = 26;
                var frmheightloc = screenY % 2 == 0
                    ? (screenY / 2) - (this.Height / 2)
                    : (++screenY / 2) - (this.Height / 2);
                this.Location = new Point(screenX - this.Width, frmheightloc);
            }
        }

        #region Variables

        SQLiteConnection cnt = new SQLiteConnection(@"Data Source = database.db");
        int screenX = Screen.GetBounds(new Point(0, 0)).Width;
        int screenY = Screen.GetBounds(new Point(0, 0)).Height;
        int buttonCounter = 0;
        public string path = "";
        int selectTab = -1;

        #endregion

        private void button1_Click(object sender, EventArgs e)
        {

            RightSizeAndLoc();
            if (buttonCounter == 0)
            {
                Tasarim();
                buttonCounter++;
            }
        }

        private void settingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var settings = new Settings();
            settings.Show();
        }

        private void reloadToolStripMenuItem_Click(object sender, EventArgs e)
        {

            /*  foreach (TabPage tpg in tabControl1.TabPages)
               {
                   tabControl1.TabPages.Remove(tpg);
               }*/
            reload();
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.ExitThread();
            Exiting();
        }

        void reload()
        {
            newTabSelect = false;
            tabControl1.TabPages.Clear();
            newTabSelect = true;
            Tasarim();
            selectedTab();
        }
        private void trackBar1_MouseCaptureChanged(object sender, EventArgs e)
        {
            reload();
        }

        private void QuickFileAccessBar_FormClosing(object sender, FormClosingEventArgs e)
        {
            Exiting();
        }

        void selectedTab()
        {
            if (selectTab != -1)
                tabControl1.SelectedIndex = selectTab;

        }

        private void tabControl1_Click(object sender, EventArgs e)
        {
            int tabPageCount = tabControl1.TabPages.Count;
            if (tabControl1.SelectedIndex != tabPageCount - 1)
            {
                selectTab = tabControl1.SelectedIndex;
            }


        }

        bool newTabSelect = true;
        private void tabControl1_Selecting(object sender, TabControlCancelEventArgs e)
        {
            if (newTabSelect)
            {
                if (tabControl1.SelectedIndex == tabControl1.TabPages.Count - 1)
                {
                    e.Cancel = true;
                    var folderBrowserDialog1 = new FolderBrowserDialog();
                    if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
                    {
                        var folderPath = folderBrowserDialog1.SelectedPath;
                        try
                        {
                            if (Directory.Exists(folderPath))
                            {
                                Connect();
                                var comm = new SQLiteCommand("insert into Dir(Path) values(@klasoryolu)", cnt);
                                comm.Parameters.AddWithValue("@klasoryolu", folderPath);
                                comm.ExecuteNonQuery();
                                comm.Dispose();
                                cnt.Close();
                                reload();
                            }
                            else
                                MessageBox.Show("Klasör eklenemedi. Klasör yolunu kontrol ederek tekrar deneyiniz.");
                        }
                        catch (Exception hata)
                        {
                            MessageBox.Show(hata.ToString());
                        }
                    }
                }
            }
        }
    }
}
