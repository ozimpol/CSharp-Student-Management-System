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
        SqlConnection con = new SqlConnection("Data Source=ADN1;Initial Catalog=OgrOtm;Integrated Security=true"); //Database baðlantýsý

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

        //Öðretmen ve Öðrenci giriþi yapýldýðý zaman databasele baðlantýyý kolaylaþtýrmak amacýyla deðiþken olarak tanýmlandý.

        private void button1_Click(object sender, EventArgs e)
        {
            string kullaniciAdi = textBox1.Text; //textbox1 i kullanýcý adý
            string sifre = textBox2.Text; //textbox2 yi þifre olarak belirledik.

            // 3 Farklý kullanýcý tipi var; Admin, Öðrenci Öðretmen. Admin kullanici isimli tablodan kontrol edilecek.
            // Öðretmen için kullanýcý adý telefon numarasý olarak, þifre TC kimlik numarasý olarak belirlendi.
            // Öðrenci için kullanýcý adý okul numarasý, þifre TC kimlik numarasý olarak belirlendi.
            string queryKullanici = "SELECT kullanici_tipi FROM kullanici WHERE kullanici_adi = @kullaniciAdi AND sifre = @sifre";
            string queryOgretmen = "SELECT ogretmen_id FROM ogretmen WHERE telefon_no = @kullaniciAdi AND tc_kimlik_no = @sifre";
            string queryOgrenci = "SELECT ogrenci_id FROM ogrenci WHERE okul_no = @kullaniciAdi AND tc_kimlik_no = @sifre";

            using (SqlCommand command = new SqlCommand(queryKullanici, con))
            {
                // Kullanýcý adý ve Þifre parametreleri databasede karþýlaþtýrma yapmak üzere alýnýr.
                command.Parameters.AddWithValue("@kullaniciAdi", kullaniciAdi);
                command.Parameters.AddWithValue("@sifre", sifre);

                con.Open();
                object kullaniciTipi = command.ExecuteScalar();
                con.Close();

                if (kullaniciTipi != null) // Kullanýcý tablosunda eþleþen veri bulunursa
                {
                    string tip = kullaniciTipi.ToString();
                    if (tip == "admin")
                    {
                        // Admin menüsü açýlýr
                        AdminMenuForm adminMenu = new AdminMenuForm();
                        adminMenu.Show();
                        this.Hide();
                    }
                }
                else
                {
                    // Kullanýcý tablosunda eþleþen veri bulunamazsa Öðretmen tablosuna bakýlýr.
                    SqlCommand commandOgretmen = new SqlCommand(queryOgretmen, con);
                    commandOgretmen.Parameters.AddWithValue("@kullaniciAdi", kullaniciAdi);
                    commandOgretmen.Parameters.AddWithValue("@sifre", sifre);

                    con.Open();
                    object ogretmenId = commandOgretmen.ExecuteScalar();
                    con.Close();

                    if (ogretmenId != null) 
                    {
                        // Giriþ baþarýlýysa Öðretmenin ID si diðer formlarda kullanýlmak üzere kaydedilir.
                        int ogretmenID = (int)ogretmenId;
                        // Öðretmen giriþi yapýlýr.
                        OgretmenMenuForm ogretmenMenu = new OgretmenMenuForm(ogretmenID);
                        ogretmenMenu.Show();
                        this.Hide();
                    }
                    else
                    {
                        // Eðer öðretmen tablosunda da uyuþan veri yoksa, öðrenci tablosuna bakýlýr.
                        SqlCommand commandOgrenci = new SqlCommand(queryOgrenci, con);
                        commandOgrenci.Parameters.AddWithValue("@kullaniciAdi", kullaniciAdi);
                        commandOgrenci.Parameters.AddWithValue("@sifre", sifre);

                        con.Open();
                        object ogrenciId = commandOgrenci.ExecuteScalar();
                        con.Close();

                        if (ogrenciId != null)
                        {
                            // Giriþ baþarýlýysa Öðrencinin ID si diðer formlarda kullanýlmak üzere kaydedilir.
                            int ogrenciID = (int)ogrenciId;
                            // Öðrenci giriþi yapýlýr.
                            OgrenciMenuForm ogrenciMenu = new OgrenciMenuForm(ogrenciID);
                            ogrenciMenu.Show();
                            this.Hide();
                        }
                        else //3 tabloda da uygun veri bulunamazsa
                        {
                            MessageBox.Show("Kullanýcý bulunamadý!");
                        }
                    }
                }
            }
        }
    }
}
