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

namespace windows_programlar_kisayol_cubugu
{
    public partial class shortcut_main : Form
    {
        SQLiteConnection cnt = new SQLiteConnection(@"Data Source = database.db");
        void connect()
        {
            if (cnt.State == ConnectionState.Closed) cnt.Open();
        }
        public shortcut_main()
        {
            InitializeComponent();
        }
        int ekran_x = Screen.GetBounds(new Point(0, 0)).Width;
        int ekran_y = Screen.GetBounds(new Point(0, 0)).Height;
        private void Form1_Load(object sender, EventArgs e)
        {
            //Başlangıç ayarı
            button1.Text = "<";
            this.Width = 10; this.Height = 26;
            var frmheightloc = ekran_y % 2 == 0 ? (ekran_y / 2) - (this.Height / 2) : (++ekran_y / 2) - (this.Height / 2);
            this.Location = new Point(ekran_x - this.Width, frmheightloc);
            connect();
            SQLiteCommand cmd = new SQLiteCommand("select * from dir", cnt);
            var rd = cmd.ExecuteReader();
            if (rd.HasRows == false)
            {
                var settings = new Settings();
                settings.Show();
            }
            cmd.Dispose();
            rd.Close();
            cnt.Close();
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

            Tasarim(sayac++);

        }
        int sayac = 0;//tasarim metodundaki kodların sadece bir kere çalışması için çalıştıktan sonra 1 veya daha büyük olur
        void Tasarim(int a = 0)
        {
            if (a == 0)
            {              
               // SQLiteCommand cmd = new SQLiteCommand("Select count(*) as Path from Dir", cnt);
                connect();
                SQLiteCommand cmd0 = new SQLiteCommand("Select Status from Settings where settingname = 'shortcutvalue'", cnt);
                int tpshortcuts = Convert.ToInt32(cmd0.ExecuteScalar());
                cmd0.Dispose();
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
                        //MessageBox.Show(file_count.ToString());
                        string[] alldirandfile = new string[file_count];//ilk klasörler sonra dostalar
                        int b = 0;
                        foreach (string dir in dizi)
                            alldirandfile[b++] = dir;
                        foreach (string dir2 in dizi2)
                            alldirandfile[b++] = dir2;
                        //MessageBox.Show(alldirandfile.Count().ToString());
 
                        int tpagewidth = tabControl1.Width-10, tpageheight = tabControl1.Height;
                        int tambolunebilenwidth = tpagewidth % 4 == 0 ? tpagewidth / 4 : (tpagewidth - (tpagewidth % 4)) / 4; 
                        int heightfit = tpageheight % tpagewidth == 0 ? tpageheight / tambolunebilenwidth : (tpageheight - (tpageheight % tambolunebilenwidth)) / tambolunebilenwidth;
                        int maxfile = heightfit * 4;
                        int sayac3 = 0;
                        int calistirma=maxfile>=file_count?1
                            :file_count%maxfile==0?file_count/maxfile
                            :(file_count-(file_count%maxfile))/maxfile+1;
                      //  MessageBox.Show(calistirma.ToString());
                        for (int j = 0; j < calistirma; j++)
                        {
                            var tabPage1 = new TabPage
                            {
                                Text = path.Split('\\').Last(),
                                //Name = ,
                                BackColor = Color.Transparent,
                                ForeColor = Color.Black,
                                Font = new Font("Verdana", 12),
                                Width = 100,
                                Height = 100,

                            }; tabControl1.TabPages.Add(tabPage1);

                            int locx = 0, locy = 0; int sayac1 = 0;

                            for (int i = 0; i < maxfile; i++)
                            {
                                var fileextension = sayac3 < dizi.Count()? "":new System.IO.FileInfo(alldirandfile[sayac3]).Extension;
                                string iconpic = @"Extension\default.png";
                                if(fileextension!="")
                                {
                                    if (System.IO.File.Exists(@"Extension\" + fileextension.Split(".").Last() + ".png"))
                                        iconpic = @"Extension\"+ fileextension.Split(".").Last() + ".png";
                                }
                                // MessageBox.Show(fileextension);
                                var pbox = new PictureBox
                                {
                                    Width = tambolunebilenwidth,
                                    Height = tambolunebilenwidth,

                                    Top = locy,
                                    Left = locx,
                                    //Location=new Point(tpagewidth,tpageheight),
                                    Name = $"Pbox{sayac3}",
                                    Tag = alldirandfile[sayac3],
                                    SizeMode = PictureBoxSizeMode.StretchImage,
                                    BackColor =/*sayac3< dizi.Count() ?*/ Color.Transparent/* : Color.FromArgb(new Random().Next(200), new Random().Next(200), new Random().Next(200))*/,
                                    ImageLocation = sayac3 < dizi.Count() ? @"Extension\directory.png":iconpic,
                                    /*sayac3 < dizi.Count() ? @"Extension\directory.png"
                                    : fileextension==".JPG"||fileextension==".PNG"||fileextension==".JPEG"? @"Extension\picture.png"
                                    : fileextension==".PDF" ? @"Extension\pdf.png" : @"Extension\file.png"*/
                                    // Image = alldirandfile[i].Split(".").Last() == "exe" ? Bitmap.FromHicon(new Icon(Icon.ExtractAssociatedIcon(alldirandfile[i]), new Size(48, 48)).Handle): Bitmap.FromHicon(new Icon("a.ico", new Size(48, 48)).Handle)
                                };
                            //   MessageBox.Show(alldirandfile[sayac3]);
                                pbox.Click += (sender, args) => { MessageBox.Show($"Picture #: {((PictureBox)sender).Tag}, Name: {((Control)sender).Name}, Current i:{sayac3}"); OpenFile(((PictureBox)sender).Tag.ToString()); };
                                tabPage1.Controls.Add(pbox);
                                // locx += 30;

                                sayac1++;
                                if (sayac1 == 4)
                                {
                                    sayac1 = 0;
                                    locy += tambolunebilenwidth;
                                    locx = 0;
                                }
                                else
                                    locx += tambolunebilenwidth;

                                sayac3++;
                                if (sayac3 == file_count) break;
                            }

                        }
                    }

                }
                dr.Close();
                cmd.Dispose();
                cnt.Close();
            }
        }

        public string path = "";
        void Click_picbox(object sender, System.EventArgs e)
        {
            OpenFile(path);
            //  System.Diagnostics.Process.Start(path,);
            /* System.Diagnostics.Process prc = new System.Diagnostics.Process();
             prc.StartInfo.FileName = path;
             prc.Start();*/
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
        private void ayarlarToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var settings = new Settings();
            settings.Show();
        }
    }
}
