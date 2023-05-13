using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MySql.Data.MySqlClient;


namespace proje
{
    public partial class DemirbasTakip : Form
    {
        private int id;
        private int kasaId;
        private int sicil_no;
        MySqlConnection baglanti = new MySqlConnection("Server=localhost;Database=demirbas_takip;user=root;Pwd=;");
        public DemirbasTakip()
        {
            InitializeComponent();
        }
        public DemirbasTakip(int id)
        {
            this.id = id;
        }
        public int ID
        {
            get { return id; }
            set { id = value; }
        }
        private void DemirbasTakip_Load(object sender, EventArgs e)
        {
            yazdir();
            listele();
        }
        private void yazdir()
        {
            baglanti.Open();
            var komut = new MySqlCommand();
            komut.CommandText = "SELECT * FROM personel WHERE id=@id";
            komut.Parameters.AddWithValue("@id", id);
            komut.Connection = baglanti;
            var dr = komut.ExecuteReader();

            if (dr.Read())
            {
                txtAd.Text = dr["ad"].ToString();
                txtSoyad.Text = dr["soyad"].ToString();
                txtSicilNo.Text = dr["sicil_no"].ToString();
                sicil_no = Convert.ToInt32(dr["sicil_no"]);
                txtUnvan.Text = dr["unvan"].ToString();
                txtBolum.Text = dr["bolum"].ToString();
                txtEposta.Text = dr["email"].ToString();
                txtOdaNumarasi.Text = dr["oda_numarasi"].ToString();
                txtIseBaslamaTarihi.Text = dr["ise_baslama_tarihi"].ToString();
                rtxtNotlar.Text = dr["notlar"].ToString();
                lblAd.Text = dr["ad"].ToString() + " " + dr["soyad"].ToString();
                lblSicil.Text = "Sicil No: " + dr["sicil_no"].ToString();

                byte[] resimBytes = (byte[])dr["picture"];

                using (var ms = new MemoryStream(resimBytes))
                {
                    pictureBox1.Image = Image.FromStream(ms);
                }
            }

            dr.Close();
            komut.Dispose();
            baglanti.Close();
        }

        private void listele()
        {
            baglanti.Open();
            var adaptor = new MySqlDataAdapter("SELECT * FROM urunler WHERE personel_id = @id", baglanti);
            adaptor.SelectCommand.Parameters.AddWithValue("@id", id);
            var dt = new DataTable();

            dt.Clear();
            adaptor.Fill(dt);
            dataGridView1.DataSource = dt;
            adaptor.Dispose();
            baglanti.Close();
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            kasaId = Convert.ToInt32(dataGridView1.SelectedRows[0].Cells["urun_id"].Value.ToString());
        }

        private void btnPersonelKaydet_Click(object sender, EventArgs e)
        {
            baglanti.Open();
            var komut = new MySqlCommand();
            komut.CommandText = "UPDATE personel SET ad=@ad, soyad=@soyad, sicil_no=@sicil_no, unvan=@unvan, bolum=@bolum, email=@email, oda_numarasi=@oda_numarasi, ise_baslama_tarihi=@ise_baslama_tarihi, picture=@picture, notlar=@notlar WHERE id=@id";
            komut.Parameters.AddWithValue("@ad", txtAd.Text);
            komut.Parameters.AddWithValue("@soyad", txtSoyad.Text);
            komut.Parameters.AddWithValue("@sicil_no", txtSicilNo.Text);
            komut.Parameters.AddWithValue("@unvan", txtUnvan.Text);
            komut.Parameters.AddWithValue("@bolum", txtBolum.Text);
            komut.Parameters.AddWithValue("@email", txtEposta.Text);
            komut.Parameters.AddWithValue("@oda_numarasi", txtOdaNumarasi.Text);
            komut.Parameters.AddWithValue("@ise_baslama_tarihi", txtIseBaslamaTarihi.Text);
            komut.Parameters.AddWithValue("@notlar", rtxtNotlar.Text);
            komut.Parameters.AddWithValue("@id", id);
            byte[] resimBytes = null;
            if (pictureBox1.Image != null)
            {
                using (var ms = new MemoryStream())
                {
                    pictureBox1.Image.Save(ms, ImageFormat.Jpeg);
                    resimBytes = ms.ToArray();
                    komut.Parameters.AddWithValue("@picture", resimBytes);
                }
            }
            else
            {
                var resimKomutu = new MySqlCommand("SELECT picture FROM personel WHERE id=@id", baglanti);
                resimKomutu.Parameters.AddWithValue("@id", id);
                using (var okuyucu = resimKomutu.ExecuteReader())
                {
                    if (okuyucu.Read())
                    {
                        resimBytes = (byte[])okuyucu["picture"];
                        komut.Parameters.AddWithValue("@picture", resimBytes);
                    }
                }
            }

            komut.Connection = baglanti;
            komut.ExecuteNonQuery();
            baglanti.Close();
            yazdir();
            MessageBox.Show("Kullanıcı Güncellendi.");
        }



        private void btnKasa_Click(object sender, EventArgs e)
        {
            if (kasaId == 0)
            {
                MessageBox.Show("Lütfen bir ürün seçiniz!");
                return;
            }

            KasaBilgi kasaBilgi = new KasaBilgi();
            kasaBilgi.ID = id;
            kasaBilgi.KASAID = kasaId;
            kasaBilgi.SICIL_NO = sicil_no;
            this.Hide();
            kasaBilgi.ShowDialog();
            this.Show();
            listele();
        }

        private void btnYeniUrunEkle_Click(object sender, EventArgs e)
        {
            var urunEkle = new UrunEkle();
            urunEkle.ID = id;
            this.Hide();
            urunEkle.ShowDialog();
            this.Show();
            listele();
        }

        private void btnResimSec_Click(object sender, EventArgs e)
        {
            var dosya = new OpenFileDialog();
            dosya.Filter = "Resim Dosyası |*.jpg;*.nef;*.png | Tüm Dosyalar |*.*";
            dosya.Title = "Resim Seçiniz..";
            dosya.ShowDialog();

            if (dosya.FileName != "")
            {
                pictureBox1.Image = Image.FromFile(dosya.FileName);
                using (var komut = new MySqlCommand("UPDATE personel SET picture=@resim WHERE id=@id"))
                {
                    using (var ms = new MemoryStream())
                    {
                        pictureBox1.Image.Save(ms, ImageFormat.Jpeg);
                        byte[] resimBytes = ms.ToArray();
                        komut.Parameters.AddWithValue("@resim", resimBytes);
                        komut.Parameters.AddWithValue("@id", id);
                        try
                        {
                            komut.Connection = baglanti;
                            baglanti.Open();
                            komut.ExecuteNonQuery();
                        }
                        catch (MySqlException ex)
                        {
                            MessageBox.Show("Hata: " + ex.Message);
                        }
                        finally
                        {
                            baglanti.Close();
                        }
                    }
                }
            }
        }

    }
}
