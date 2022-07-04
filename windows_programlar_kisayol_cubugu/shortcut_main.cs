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
                // MessageBox.Show(a.ToString());
                connect();
                /* SQLiteCommand cmd = new SQLiteCommand("Select count(*) as Path from Dir", cnt);
             int kayitSayisi = Convert.ToInt32(cmd.ExecuteScalar());
                 //path sütununda kaç veri var
                 MessageBox.Show(kayitSayisi.ToString());*/

                //içindeki kısayollarının ayarı
                SQLiteCommand cmd = new SQLiteCommand("Select Path from Dir", cnt);
                SQLiteDataReader dr = cmd.ExecuteReader();


                while (dr.Read())
                {
                    path = dr["Path"].ToString();
                    //  MessageBox.Show(gelenveri);
                    if (System.IO.Directory.Exists(path))
                    {

                        TabPage tabPage1 = new TabPage();
                        //  tabPage1.Name = "tabPage" + i.ToString();
                        tabPage1.Text = path.Split('\\').Last();
                        tabPage1.BackColor = Color.Transparent;
                        tabPage1.ForeColor = Color.Black;
                        tabPage1.Font = new Font("Verdana", 12);
                        tabPage1.Width = 100;
                        tabPage1.Height = 100;
                        tabControl1.TabPages.Add(tabPage1);
                        var picbox = new PictureBox();
                        picbox.Width = 15; picbox.Height = 100;
                        picbox.Location = new Point(12, 24);
                        picbox.BackColor = Color.Black;
                        picbox.Click += new EventHandler(Click_button);
                        tabPage1.Controls.Add(picbox);

                        /*  string[] dizi = System.IO.Directory.GetFiles(gelenveri);
                          string[] dizi2 = System.IO.Directory.GetDirectories(gelenveri);
                          foreach (string dizi1 in dizi)
                          {
                              MessageBox.Show("aaaaaaaaaaaa"+dizi1);
                          }
                          foreach (string dizi3 in dizi2)
                          {
                              MessageBox.Show("aaaaaaaaaaaaa"+dizi3);
                          }*/
                    }
                }
                dr.Close();
                cmd.Dispose();
            }
        }
        public string path = "";
        void Click_button(object sender, System.EventArgs e)
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
