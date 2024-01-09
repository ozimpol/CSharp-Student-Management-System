using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ogrenci
{
    public partial class Form1 : Form
    {
        SqlConnection con = new SqlConnection("Data Source=ADN1;Initial Catalog=OgrOtm;Integrated Security=true"); //Database ba�lant�s�

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }

        int ogrenciID;
        int ogretmenID;

        //��retmen ve ��renci giri�i yap�ld��� zaman databasele ba�lant�y� kolayla�t�rmak amac�yla de�i�ken olarak tan�mland�.

        private void button1_Click(object sender, EventArgs e)
        {
            string kullaniciAdi = textBox1.Text; //textbox1 i kullan�c� ad�
            string sifre = textBox2.Text; //textbox2 yi �ifre olarak belirledik.

            // 3 Farkl� kullan�c� tipi var; Admin, ��renci ��retmen. Admin kullanici isimli tablodan kontrol edilecek.
            // ��retmen i�in kullan�c� ad� telefon numaras� olarak, �ifre TC kimlik numaras� olarak belirlendi.
            // ��renci i�in kullan�c� ad� okul numaras�, �ifre TC kimlik numaras� olarak belirlendi.
            string queryKullanici = "SELECT kullanici_tipi FROM kullanici WHERE kullanici_adi = @kullaniciAdi AND sifre = @sifre";
            string queryOgretmen = "SELECT ogretmen_id FROM ogretmen WHERE telefon_no = @kullaniciAdi AND tc_kimlik_no = @sifre";
            string queryOgrenci = "SELECT ogrenci_id FROM ogrenci WHERE okul_no = @kullaniciAdi AND tc_kimlik_no = @sifre";

            using (SqlCommand command = new SqlCommand(queryKullanici, con))
            {
                // Kullan�c� ad� ve �ifre parametreleri databasede kar��la�t�rma yapmak �zere al�n�r.
                command.Parameters.AddWithValue("@kullaniciAdi", kullaniciAdi);
                command.Parameters.AddWithValue("@sifre", sifre);

                con.Open();
                object kullaniciTipi = command.ExecuteScalar();
                con.Close();

                if (kullaniciTipi != null) // Kullan�c� tablosunda e�le�en veri bulunursa
                {
                    string tip = kullaniciTipi.ToString();
                    if (tip == "admin")
                    {
                        // Admin men�s� a��l�r
                        AdminMenuForm adminMenu = new AdminMenuForm();
                        adminMenu.Show();
                        this.Hide();
                    }
                }
                else
                {
                    // Kullan�c� tablosunda e�le�en veri bulunamazsa ��retmen tablosuna bak�l�r.
                    SqlCommand commandOgretmen = new SqlCommand(queryOgretmen, con);
                    commandOgretmen.Parameters.AddWithValue("@kullaniciAdi", kullaniciAdi);
                    commandOgretmen.Parameters.AddWithValue("@sifre", sifre);

                    con.Open();
                    object ogretmenId = commandOgretmen.ExecuteScalar();
                    con.Close();

                    if (ogretmenId != null) 
                    {
                        // Giri� ba�ar�l�ysa ��retmenin ID si di�er formlarda kullan�lmak �zere kaydedilir.
                        int ogretmenID = (int)ogretmenId;
                        // ��retmen giri�i yap�l�r.
                        OgretmenMenuForm ogretmenMenu = new OgretmenMenuForm(ogretmenID);
                        ogretmenMenu.Show();
                        this.Hide();
                    }
                    else
                    {
                        // E�er ��retmen tablosunda da uyu�an veri yoksa, ��renci tablosuna bak�l�r.
                        SqlCommand commandOgrenci = new SqlCommand(queryOgrenci, con);
                        commandOgrenci.Parameters.AddWithValue("@kullaniciAdi", kullaniciAdi);
                        commandOgrenci.Parameters.AddWithValue("@sifre", sifre);

                        con.Open();
                        object ogrenciId = commandOgrenci.ExecuteScalar();
                        con.Close();

                        if (ogrenciId != null)
                        {
                            // Giri� ba�ar�l�ysa ��rencinin ID si di�er formlarda kullan�lmak �zere kaydedilir.
                            int ogrenciID = (int)ogrenciId;
                            // ��renci giri�i yap�l�r.
                            OgrenciMenuForm ogrenciMenu = new OgrenciMenuForm(ogrenciID);
                            ogrenciMenu.Show();
                            this.Hide();
                        }
                        else //3 tabloda da uygun veri bulunamazsa
                        {
                            MessageBox.Show("Kullan�c� bulunamad�!");
                        }
                    }
                }
            }
        }
    }
}
