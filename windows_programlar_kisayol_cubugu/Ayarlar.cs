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

    public partial class Ayarlar : Form
    {
        public Ayarlar()
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
            baglanti();
            SQLiteCommand comm = new SQLiteCommand("SELECT * FROM sqlite_master WHERE type = 'table' and name=@tablename", cnt);
            comm.Parameters.AddWithValue("@tablename", "Dir");
            SQLiteDataReader reader = comm.ExecuteReader();
            if (reader.HasRows == false)
            {
                string sad = "CREATE TABLE Dir(Path TEXT NOT NULL UNIQUE)";
                //(Path)tablo sütun adı,(TEXT)veri tipi, (Not Null)boş veriye izin yok,(UBIQUE)benzersiz veri
                SQLiteCommand comm2 = new SQLiteCommand(sad, cnt);
                comm2.ExecuteNonQuery();
                comm2.Dispose();
            }
            reader.Close();
            comm.Dispose();
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
    }
}