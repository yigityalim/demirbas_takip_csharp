using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace proje
{
    public partial class Form1 : Form
    {
        MySqlConnection baglanti = new MySqlConnection("Server = localhost; Database = demirbas_takip; User = root; Pwd =;");
        public Form1()
        {
            InitializeComponent();
        }

        private void btnGiris_Click(object sender, EventArgs e)
        {
            String email = txtEmail.Text;
            String sifre = txtSifre.Text;
            baglanti.Open();

            using (var komut = new MySqlCommand())
            {
                komut.Connection = baglanti;
                komut.CommandText = "SELECT id FROM personel WHERE email=@email AND password=@sifre";
                komut.Parameters.AddWithValue("@email", email);
                komut.Parameters.AddWithValue("@sifre", sifre);

                var id = komut.ExecuteScalar();
                if (id != null)
                {
                    var demirbasTakip = new DemirbasTakip();
                    demirbasTakip.ID = Convert.ToInt32(id);
                    this.Hide();
                    demirbasTakip.ShowDialog();
                    this.Show();
                }
                else
                {
                    MessageBox.Show("Giriş Başarısız");
                }
            }
            baglanti.Close();
        }
    }
}
