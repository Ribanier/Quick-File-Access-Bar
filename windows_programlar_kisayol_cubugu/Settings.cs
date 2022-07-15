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

namespace windows_programlar_kisayol_cubugu
{

    public partial class Settings : Form
    {
        public Settings()
        {
            InitializeComponent();
        }
        SQLiteConnection cnt = new SQLiteConnection(@"Data Source = database.db");
        void baglanti()
        {
            if (cnt.State == ConnectionState.Closed) cnt.Open();
        }


        private void button4_Click(object sender, EventArgs e)
        {
            var folderBrowserDialog1 = new FolderBrowserDialog();
            if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
            {
                textBox1.Text = folderBrowserDialog1.SelectedPath;
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            try
            {
                if (System.IO.Directory.Exists(textBox1.Text))
                {
                    baglanti();
                    var comm = new SQLiteCommand("insert into Dir(Path) values(@klasoryolu)", cnt);
                    comm.Parameters.AddWithValue("@klasoryolu", textBox1.Text);
                    comm.ExecuteNonQuery();
                    comm.Dispose();
                    cnt.Close();
                    listdata();
                }
                else
                    MessageBox.Show("Klasör eklenemedi. Klasör yolunu kontrol ederek tekrar deneyiniz.");
            }
            catch (Exception hata)
            {
                MessageBox.Show(hata.ToString());
            }
        }
        void listdata()
        {
            baglanti();
            SQLiteCommand cmd = new SQLiteCommand("Select * from dir", cnt);
            var da = new SQLiteDataAdapter(cmd);
            var dt = new DataTable();
            da.Fill(dt);
            dataGridView1.DataSource = dt;
            cnt.Close();

        }
        private void Ayarlar_Load(object sender, EventArgs e)
        {
            this.Icon = new Icon("Appicon.ico", 250, 250);
            baglanti();
            SQLiteCommand com = new SQLiteCommand("SELECT * FROM sqlite_master WHERE type = 'table' and name=@tablename", cnt);
            com.Parameters.AddWithValue("@tablename", "Dir");
            SQLiteDataReader reader = com.ExecuteReader();
            if (reader.HasRows == false)
            {
                string create = "CREATE TABLE Dir(Path TEXT NOT NULL UNIQUE)";
                //(Path)tablo sütun adı,(TEXT)veri tipi, (Not Null)boş veriye izin yok,(UBIQUE)benzersiz veri
                SQLiteCommand comm = new SQLiteCommand(create, cnt);
                comm.ExecuteNonQuery();
                comm.Dispose();

                string create1 = "CREATE TABLE Settings(settingname TEXT NOT NULL UNIQUE, status INT NOT NULL UNIQUE)";
                SQLiteCommand comm1 = new SQLiteCommand(create1, cnt);
                // comm1.Parameters.AddWithValue("@varsayilandeger", 16);
                comm1.ExecuteNonQuery();
                comm1.Dispose();

                string addshortcuts = "insert into Settings(settingname,status) values(@settingname,@value)";
                SQLiteCommand comm2 = new SQLiteCommand(addshortcuts, cnt);
                comm2.Parameters.AddWithValue("@settingname", "shortcutsize");
                comm2.Parameters.AddWithValue("@value", 60);
                comm2.ExecuteNonQuery();
                comm2.Dispose();

            }
            reader.Close();
            com.Dispose();
            cnt.Close();
            listdata();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedCells.Count > 0)
            {
                int secimsayisi = dataGridView1.SelectedCells.Count;
                baglanti();
                SQLiteCommand cmd;
                for (int i = 0; i < secimsayisi; i++)
                {
                    cmd = new SQLiteCommand("Delete from dir where path=@dosyayolu", cnt);
                    cmd.Parameters.AddWithValue("@dosyayolu", dataGridView1.SelectedCells[i].Value);
                    cmd.ExecuteNonQuery();
                    cmd.Dispose();
                }

                cnt.Close();
                listdata();
            }
            else
                MessageBox.Show("Lütfen listeden seçin");
          
        }

        private void Settings_FormClosed(object sender, FormClosedEventArgs e)
        {
            shortcut_main sm = (shortcut_main)Application.OpenForms["shortcut_main"];
            sm.tabControl1.TabPages.Clear();
            sm.Tasarim();
        }

        /* private void button1_Click(object sender, EventArgs e)
         {
             baglanti();
             string addshortcuts = "update Settings set status = @value where settingname='shortcutvalue'";
             SQLiteCommand comm2 = new SQLiteCommand(addshortcuts,cnt);
             comm2.Parameters.AddWithValue("@value", Convert.ToInt32(numericUpDown1.Value));
             comm2.ExecuteNonQuery();
             comm2.Dispose();
             cnt.Close();
         }*/
    }
}