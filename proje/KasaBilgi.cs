using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace proje
{
    public partial class KasaBilgi : Form
    {
        MySqlConnection baglanti = new MySqlConnection("Server=localhost;Database=demirbas_takip;user=root;Pwd=;");
        private int kasaId;
        private int id;
        private int sicil_no;
        public KasaBilgi()
        {
            InitializeComponent();
        }
        public KasaBilgi(int kasaId, int id, int sicil_no)
        {
            this.kasaId = kasaId;
            this.id = id;
            this.sicil_no = sicil_no;
        }

        public int KASAID
        {
            get { return this.kasaId; }
            set { this.kasaId = value;}
        }

        public int ID
        {
            get { return this.id; }
            set { this.id = value;}
        }

        public int SICIL_NO
        {
            get { return this.sicil_no;}
            set { this.sicil_no = value;}
        }

        private void yazdir()
        {
            try
            {
                baglanti.Open();
                var cmd = new MySqlCommand();
                cmd.CommandText = "SELECT * FROM urunler WHERE urun_id=@kasa_id";
                cmd.Parameters.AddWithValue("@kasa_id", kasaId);
                cmd.Connection = baglanti;
                var dr = cmd.ExecuteReader();
                if (dr.Read())
                {
                    txtSicilNo.Text = sicil_no.ToString();
                    txtKasaId.Text = kasaId.ToString();
                    txtMarka.Text = dr["marka"].ToString();
                    txtModel.Text = dr["model"].ToString();
                    txtTarih.Text = dr["satin_alinma_tarihi"].ToString();
                    txtIslemci.Text = dr["islemci"].ToString();
                    txtRam.Text = dr["ram"].ToString();
                    txtHdd.Text = dr["hdd"].ToString();
                    txtGpu.Text = dr["ekran_karti"].ToString();
                    txtMonitor.Text = dr["ekran_boyutu"].ToString();
                }
                dr.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Hata: " + ex.Message);
            }
            finally
            {
                baglanti.Close();
            }
        }
  

        private void KasaBilgi_Load(object sender, EventArgs e)
        {
            yazdir();
        }

        private void btnKaydet_Click(object sender, EventArgs e)
        {
            try
            {
                baglanti.Open();
                var cmd = new MySqlCommand();
                cmd.CommandText = "UPDATE urunler SET marka=@marka, model=@model, satin_alinma_tarihi=@satin_alinma_tarihi, islemci=@islemci, ram=@ram, hdd=@hdd, ekran_karti=@ekran_karti, ekran_boyutu=@ekran_boyutu WHERE urun_id=@kasa_id";
                cmd.Parameters.AddWithValue("@marka", txtMarka.Text);
                cmd.Parameters.AddWithValue("@model", txtModel.Text);
                DateTime now = DateTime.Now;
                string formattedDate = now.ToString("yyyy-MM-dd");
                cmd.Parameters.AddWithValue("@satin_alinma_tarihi", formattedDate);
                cmd.Parameters.AddWithValue("@islemci", txtIslemci.Text);
                cmd.Parameters.AddWithValue("@ram", txtRam.Text);
                cmd.Parameters.AddWithValue("@hdd", txtHdd.Text);
                cmd.Parameters.AddWithValue("@ekran_karti", txtGpu.Text);
                cmd.Parameters.AddWithValue("@ekran_boyutu", txtMonitor.Text);
                cmd.Parameters.AddWithValue("@kasa_id", kasaId);
                cmd.Connection = baglanti;
                int sonuc = cmd.ExecuteNonQuery();
                if (sonuc > 0)
                {
                    MessageBox.Show("Ürün güncellendi.");
                    this.Close();
                }
                else
                {
                    MessageBox.Show("Ürün güncellenemedi.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Hata: " + ex.Message);
            }
            finally
            {
                baglanti.Close();
            }
        }

        private void btnSil_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Ürünü silmek istediğinize emin misiniz?", "Uyarı", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                try
                {
                    baglanti.Open();
                    var cmd = new MySqlCommand();
                    cmd.CommandText = "DELETE FROM urunler WHERE urun_id=@kasa_id";
                    cmd.Parameters.AddWithValue("@kasa_id", kasaId);
                    cmd.Connection = baglanti;
                    int sonuc = cmd.ExecuteNonQuery();
                    if (sonuc > 0)
                    {
                        MessageBox.Show("Ürün silindi.");
                        this.Close();
                    }
                    else
                    {
                        MessageBox.Show("Ürün silinemedi.");
                    }
                }
                catch (Exception ex)
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
