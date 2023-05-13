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
    public partial class UrunEkle : Form
    {
        MySqlConnection baglanti = new MySqlConnection("Server=localhost;Database=demirbas_takip;user=root;Pwd=;");
        private int id;
        public UrunEkle()
        {
            InitializeComponent();
        }

        public UrunEkle(int id)
        {
            this.id = id;
        }

        public int ID
        {
            get { return this.id; }
            set { this.id = value; }
        }

        private void UrunEkle_Load(object sender, EventArgs e)
        {
            lblId.Text = "Kullanıcı ID: " + id.ToString();
        }

        private void btnYeniUrunEkle_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtMarka.Text) ||
                string.IsNullOrEmpty(txtModel.Text) ||
                string.IsNullOrEmpty(txtTarih.Text) ||
                string.IsNullOrEmpty(txtIslemci.Text) ||
                string.IsNullOrEmpty(txtRam.Text) ||
                string.IsNullOrEmpty(txtHdd.Text) ||
                string.IsNullOrEmpty(txtEkranKarti.Text) ||
                string.IsNullOrEmpty(txtEkranBoyutu.Text))
            {
                MessageBox.Show("Lütfen tüm alanları doldurunuz.", "Eksik Girdi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            DateTime satinAlinmaTarihi;
            if (!DateTime.TryParse(txtTarih.Text, out satinAlinmaTarihi))
            {
                MessageBox.Show("Lütfen geçerli bir tarih giriniz. (YYYY/AA/GG)", "Geçersiz Tarih", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            baglanti.Open();
            var komut = new MySqlCommand();
            komut.CommandText = "INSERT INTO urunler (personel_id, marka, model, satin_alinma_tarihi, islemci, ram, hdd, ekran_karti, ekran_boyutu) VALUES (@personel_id, @marka, @model, @satin_alinma_tarihi, @islemci, @ram, @hdd, @ekran_karti, @ekran_boyutu)";
            komut.Parameters.AddWithValue("@personel_id", id);
            komut.Parameters.AddWithValue("@marka", txtMarka.Text);
            komut.Parameters.AddWithValue("@model", txtModel.Text);
            komut.Parameters.AddWithValue("@satin_alinma_tarihi", txtTarih.Text);
            komut.Parameters.AddWithValue("@islemci", txtIslemci.Text);
            komut.Parameters.AddWithValue("@ram", txtRam.Text);
            komut.Parameters.AddWithValue("@hdd", txtHdd.Text);
            komut.Parameters.AddWithValue("@ekran_karti", txtEkranKarti.Text);
            komut.Parameters.AddWithValue("@ekran_boyutu", txtEkranBoyutu.Text);
            komut.Connection = baglanti;
            komut.ExecuteNonQuery();
            komut.Dispose();
            baglanti.Close();
            MessageBox.Show("Yeni ürün eklendi.");
            this.Close();
        }
    }
}
