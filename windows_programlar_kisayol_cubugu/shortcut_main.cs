using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SQLite;
using System.IO;
using System.Diagnostics;
using IWshRuntimeLibrary;

namespace windows_programlar_kisayol_cubugu
{
    public partial class shortcut_main : Form
    {
        public shortcut_main()
        {
            InitializeComponent();
        }
        #region Methods
        void connect()//veri tabanı bağlantısı kontrolu 
        {
            if (cnt.State == ConnectionState.Closed) cnt.Open();
        }
        public int trackbarcontrol()//trackbarın veritabanındaki değerini kontrol eder
        {
            try
            {
                connect();
                SQLiteCommand cmd1 = new SQLiteCommand("select status from Settings where settingname =@settingname", cnt);
                cmd1.Parameters.AddWithValue("@settingname", "shortcutsize");
                int a = Convert.ToInt32(cmd1.ExecuteScalar());
                cmd1.Dispose();
                cnt.Close();
                return a;
            }
            catch
            {
                return 30;
            }

        }
        public void Tasarim()//uygulamanın klasor olusumu ve düzenini tasarlamak
        {
            // SQLiteCommand cmd = new SQLiteCommand("Select count(*) as Path from Dir", cnt);
            /*SQLiteCommand cmd0 = new SQLiteCommand("Select Status from Settings where settingname = 'shortcutvalue'", cnt);
              int tpshortcuts = Convert.ToInt32(cmd0.ExecuteScalar());
              cmd0.Dispose();*/
            connect();
            SQLiteCommand cmd = new SQLiteCommand("Select Path from Dir", cnt);
            SQLiteDataReader dr = cmd.ExecuteReader();
            while (dr.Read())
            {
                if (System.IO.Directory.Exists(dr["Path"].ToString()))
                {
                    path = dr["Path"].ToString();
                    //  MessageBox.Show(gelenveri);
                    string[] dizi = System.IO.Directory.GetDirectories(path);
                    string[] dizi2 = System.IO.Directory.GetFiles(path);
                    int file_count = dizi.Count() + dizi2.Count();

                    string[] alldirandfile = new string[file_count];//ilk klasörler sonra dostalar
                    int b = 0;
                    foreach (string dir in dizi)
                        alldirandfile[b++] = dir;
                    foreach (string dir2 in dizi2)
                        alldirandfile[b++] = dir2;

                    var tabPage1 = new TabPage
                    {
                        Text = path.Split('\\').Last() == "" ? path : path.Split('\\').Last(),
                        Tag = path,
                        BackColor = Color.Transparent,
                        ForeColor = Color.Black,
                        Font = new Font("Verdana", 12),
                        Width = 100,
                        Height = 100,
                        AutoScroll = true
                    };
                    tabControl1.TabPages.Add(tabPage1);

                    int tpagewidth = tabPage1.Width, tpageheight = tabPage1.Height;
                    int scutwidthheight = trackBar1.Value;
                    int yanyanasirasi = tpagewidth / scutwidthheight;
                    int fontboyut = scutwidthheight > 82 ? 11 : scutwidthheight > 64 ? 10 : scutwidthheight > 46 ? 9 : 8;

                    int locx = 0, locy = 0; int sayac1 = 0;

                    for (int i = 0; i < file_count; i++)
                    {
                        var fileextension = i < dizi.Count() ? "" : new System.IO.FileInfo(alldirandfile[i]).Extension;
                        string iconpic = @"Extension\default.png";
                        if (fileextension != "")
                        {
                            if (System.IO.File.Exists(@"Extension\" + fileextension.Split(".").Last() + ".png"))
                                iconpic = @"Extension\" + fileextension.Split(".").Last() + ".png";
                        }
                        // MessageBox.Show(fileextension);
                        var pbox = new PictureBox
                        {
                            Width = scutwidthheight,
                            Height = scutwidthheight,
                            Top = locy,
                            Left = locx,
                            //Location=new Point(tpagewidth,tpageheight),
                            Name = $"Pbox{i}",
                            Tag = alldirandfile[i],
                            SizeMode = PictureBoxSizeMode.StretchImage,
                            BackColor = Color.Transparent,
                            // ImageLocation = i < dizi.Count() ? @"Extension\directory.png" : iconpic,
                            // Image = alldirandfile[i].Split(".").Last() == "exe" ? Bitmap.FromHicon(new Icon(Icon.ExtractAssociatedIcon(alldirandfile[i]), new Size(48, 48)).Handle): Bitmap.FromHicon(new Icon("a.ico", new Size(48, 48)).Handle)
                        };

                        if (fileextension == ".lnk" || fileextension == ".exe")
                        {
                            try
                            {

                                if (fileextension == ".lnk")
                                {
                                    WshShell shell = new WshShell();
                                    WshShortcut shortcut = (WshShortcut)shell.CreateShortcut(alldirandfile[i]);
                                    if (System.IO.Directory.Exists(shortcut.TargetPath))
                                        pbox.ImageLocation = @"Extension\directory.png";
                                    else
                                        pbox.Image = Bitmap.FromHicon(Icon.ExtractAssociatedIcon(shortcut.TargetPath).Handle);
                                }
                                else
                                    pbox.Image = Bitmap.FromHicon(Icon.ExtractAssociatedIcon(alldirandfile[i]).Handle);
                            }
                            catch { pbox.ImageLocation = i < dizi.Count() ? @"Extension\directory.png" : iconpic; }
                        }
                        else
                            pbox.ImageLocation = i < dizi.Count() ? @"Extension\directory.png" : iconpic;

                        var label = new Label
                        {
                            Text = alldirandfile[i].Split("\\").Last(),
                            MaximumSize = new Size(scutwidthheight, 20),
                            Top = locy + scutwidthheight,
                            Left = locx,
                            ForeColor = Color.Black,
                            BackColor = Color.Transparent,
                            Font = new Font("Verdana", fontboyut),
                            Tag = alldirandfile[i].Split("\\").Last()
                        };

                        pbox.Click += (sender, args) =>
                        {
                            //MessageBox.Show($"Picture #: {((PictureBox)sender).Tag}, Name: {((Control)sender).Name}, Current i:{i}");
                            OpenFile(((PictureBox)sender).Tag.ToString());
                        };
                        tabPage1.Controls.Add(pbox);

                        label.MouseHover += (sender, args) =>
                        {
                            ToolTip tp = new ToolTip();
                            tp.SetToolTip(label, ((Label)sender).Tag.ToString());
                        };
                        tabPage1.Controls.Add(label);

                        sayac1++;
                        if (sayac1 == yanyanasirasi)
                        {
                            sayac1 = 0;
                            locy += scutwidthheight + label.Height;
                            locx = 0;
                        }
                        else
                            locx += scutwidthheight;
                    }

                }

            }
            dr.Close();
            cmd.Dispose();
            cnt.Close();
        }
        void exit()//Uygulama kapatılırken yapılacaklar
        {
            connect();
            string addshortcuts = "update Settings set status=@newvalue where settingname =@settingname";
            SQLiteCommand comm2 = new SQLiteCommand(addshortcuts, cnt);
            comm2.Parameters.AddWithValue("@newvalue", trackBar1.Value);
            comm2.Parameters.AddWithValue("@settingname", "shortcutsize");
            comm2.ExecuteNonQuery();
            comm2.Dispose();
            cnt.Close();
        }
        public static void OpenFile(string path, bool isDirectory = false)//klasör açma
        {

            ProcessStartInfo pi = new ProcessStartInfo(path);
            pi.Arguments = Path.GetFileName(path);
            pi.UseShellExecute = true;
            pi.WindowStyle = ProcessWindowStyle.Normal;
            pi.Verb = "OPEN";
            Process proc = new Process();
            proc.StartInfo = pi;
            proc.Start();

        }
        #endregion

        #region Variables
        SQLiteConnection cnt = new SQLiteConnection(@"Data Source = database.db");
        int ekran_x = Screen.GetBounds(new Point(0, 0)).Width;
        int ekran_y = Screen.GetBounds(new Point(0, 0)).Height;
        int btnsayac = 0;
        public string path = "";
        #endregion

        private void Form1_Load(object sender, EventArgs e)
        {
            this.Icon = new Icon("Appicon.ico");
            //Başlangıç ayarı
            button1.Text = "<";
            this.Width = 10; this.Height = 26;
            var frmheightloc = ekran_y % 2 == 0 ? (ekran_y / 2) - (this.Height / 2) : (++ekran_y / 2) - (this.Height / 2);
            this.Location = new Point(ekran_x - this.Width, frmheightloc);
            connect();
            SQLiteCommand com = new SQLiteCommand("SELECT * FROM sqlite_master WHERE type = 'table' and name=@tablename", cnt);
            com.Parameters.AddWithValue("@tablename", "Dir");
            SQLiteDataReader reader = com.ExecuteReader();
            if (reader.HasRows == false)
            {
                var settings = new Settings();
                settings.Show();
                btnsayac++;
                com.Dispose();
                reader.Close();
                cnt.Close();
                trackBar1.Value = trackbarcontrol();
            }
            else
            {
                SQLiteCommand cmd = new SQLiteCommand("select * from dir", cnt);
                var rd = cmd.ExecuteReader();
                if (rd.HasRows == false)
                {
                    var settings = new Settings();
                    settings.Show();
                    btnsayac++;
                    cmd.Dispose();
                    com.Dispose();
                    rd.Close();
                    reader.Close();
                    cnt.Close();
                    trackBar1.Value = trackbarcontrol();
                }

            }
            cnt.Close();
            trackBar1.Value = trackbarcontrol();
        }
     
        private void button1_Click(object sender, EventArgs e)
        {
            if (this.Height != ekran_y)
            {
                button1.Text = ">";
                this.Height = ekran_y;
                this.Width = ekran_x % 4 == 0 ? ekran_x / 4 : (ekran_x - (ekran_x % 4)) / 4;
                this.Location = new Point(ekran_x - this.Width, 0);
            }
            else
            {
                button1.Text = "<";
                this.Width = 10; this.Height = 26;
                var frmheightloc = ekran_y % 2 == 0 ? (ekran_y / 2) - (this.Height / 2) : (++ekran_y / 2) - (this.Height / 2);
                this.Location = new Point(ekran_x - this.Width, frmheightloc);
            }
            if (btnsayac == 0)
            {
                Tasarim();
                btnsayac++;
            }

        }

        private void trackBar1_MouseCaptureChanged(object sender, EventArgs e)
        {
            tabControl1.TabPages.Clear();
            Tasarim();
        }

        private void settingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var settings = new Settings();
            settings.Show();
        }

        private void reloadToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            tabControl1.TabPages.Clear();
            Tasarim();
        }

        private void exitToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            Application.ExitThread(); exit();
        }

        private void shortcut_main_FormClosing(object sender, FormClosingEventArgs e)
        {
            exit();
        }
    }
}
