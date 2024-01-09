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
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace ogrenci
{
    public partial class AdminOgrenci : Form
    {

        SqlConnection con = new SqlConnection("Data Source=ADN1;Initial Catalog=OgrOtm;Integrated Security=true"); //veritabanı bağlantısı

        private void ListeYukle() //ListViewda veritabanındaki verileri görüntülemeye yarayan method
        {
            listView1.Items.Clear();
            // Öğrenci tablosundan verileri çekmek için gerekli SQL sorgusu
            string query = "SELECT adi, tc_kimlik_no, okul_no, bolumu, sinifi, dogum_tarihi, telefon_no FROM ogrenci";

            // SQL sorgusunu çalıştırmak için SqlConnection ve SqlCommand kullanın

            {
                SqlCommand command = new SqlCommand(query, con);
                con.Open();

                SqlDataReader reader = command.ExecuteReader();

                // ListView sütunlarını oluşturma
                listView1.Columns.Add("Öğrenci No");
                listView1.Columns.Add("Ad");
                listView1.Columns.Add("Bölüm");
                listView1.Columns.Add("Sınıf");
                listView1.Columns.Add("TC Kimlik No");
                listView1.Columns.Add("Doğum Tarihi");
                listView1.Columns.Add("Telefon No");

                while (reader.Read())
                {
                    // Her bir öğrenci için ListViewItem oluşturarak ListView'e ekleme
                    ListViewItem item = new ListViewItem(reader["okul_no"].ToString());
                    item.SubItems.Add(reader["adi"].ToString());
                    item.SubItems.Add(reader["bolumu"].ToString());
                    item.SubItems.Add(reader["sinifi"].ToString());
                    item.SubItems.Add(reader["tc_kimlik_no"].ToString());
                    item.SubItems.Add(reader["dogum_tarihi"].ToString());
                    item.SubItems.Add(reader["telefon_no"].ToString());

                    listView1.Items.Add(item);
                }

                reader.Close();
                con.Close();
            }
        }

        private void Temizle()
        {
            textBox1.Text = "";
            textBox2.Text = "";
            textBox3.Text = "";
            textBox4.Text = "";
            textBox5.Text = "";
            textBox6.Text = "";
            textBox7.Text = "";
        }

        public AdminOgrenci()
        {
            InitializeComponent();
        }

        private void button5_Click(object sender, EventArgs e) //geri butonu
        {
            AdminMenuForm adminMenu = new AdminMenuForm();
            adminMenu.Show();
            this.Hide();
        }

        private void button4_Click(object sender, EventArgs e) //bilgilendirme butonu
        {
            MessageBox.Show("Öğrenci eklemek için artı simgesini, silmek için çöp kutusu simgesini, düzenlemek için kalem simgesini kullanınız. Silme ve düzenleme için listeden üzerinde işlem yapmak istediğiniz satırı seçmeyi unutmayınız.", "BİLGİLENDİRME");

        }

        private void AdminOgrenci_Load(object sender, EventArgs e)
        {
            ListeYukle(); //Form yüklendiği an Listenin görüntülenebilmesi
        }

        private void listView1_MouseDoubleClick(object sender, MouseEventArgs e) //ListViewdaki bir satıra çift tıklanıldığında
        {

            if (listView1.SelectedItems.Count > 0) //Çift tıklanan satır varsa
            {
                //Satırdaki verileri TextBoxlara yerleştirme
                ListViewItem selectedRow = listView1.SelectedItems[0];

                textBox1.Text = selectedRow.SubItems[0].Text; // Öğrenci No
                textBox2.Text = selectedRow.SubItems[1].Text; // Adı
                textBox3.Text = selectedRow.SubItems[2].Text; // Bölümü
                textBox4.Text = selectedRow.SubItems[3].Text; // Sınıfı
                textBox5.Text = selectedRow.SubItems[4].Text; // TC Kimlik No
                textBox6.Text = selectedRow.SubItems[5].Text; // Doğum Tarihi
                textBox7.Text = selectedRow.SubItems[6].Text; // Telefon No
                
                
                //Öğrenci numarasını kullanarak seçilen öğrencinin ID değerini almak
                string ogrenciNo = textBox1.Text; 
                con.Open();
                string queryGetOgrenciId = "SELECT ogrenci_id FROM ogrenci WHERE okul_no = @ogrenciNo";

                SqlCommand commandGetOgrenciId = new SqlCommand(queryGetOgrenciId, con);
                commandGetOgrenciId.Parameters.AddWithValue("@ogrenciNo", ogrenciNo);

                int ogrenciId = (int)commandGetOgrenciId.ExecuteScalar(); 

                label8.Text = ogrenciId.ToString(); // ogrenci_id'yi label8'e yazdır
                con.Close();
            }
        }



        private void button1_Click(object sender, EventArgs e) //ekleme butonu
        {
            //TextBoxlardaki verileri alma
            string okulNo = textBox1.Text;
            string adi = textBox2.Text;
            string bolumu = textBox3.Text;
            string sinifi = textBox4.Text;
            string tcKimlikNo = textBox5.Text;
            string dogumTarihi = textBox6.Text;
            string telefonNo = textBox7.Text;

            if (string.IsNullOrEmpty(okulNo) || string.IsNullOrEmpty(adi) || string.IsNullOrEmpty(bolumu) ||
                string.IsNullOrEmpty(sinifi) || string.IsNullOrEmpty(tcKimlikNo) || string.IsNullOrEmpty(dogumTarihi) ||
                string.IsNullOrEmpty(telefonNo)) //Herhangi bir textbox boşsa
            {
                MessageBox.Show("Lütfen bütün alanları doldurun!");
                return;
            }

            con.Open();

            //Benzersiz olması gereken veriler Okul numarası, TC Kimlik numarası, Telefon numarası benzeri var mı diye kontrol edilir.
            SqlCommand commandCheckOkulNo = new SqlCommand("SELECT COUNT(*) FROM ogrenci WHERE okul_no = @okulNo", con);
            SqlCommand commandCheckTcKimlikNo = new SqlCommand("SELECT COUNT(*) FROM ogrenci WHERE tc_kimlik_no = @tcKimlikNo", con);
            SqlCommand commandCheckTelefonNo = new SqlCommand("SELECT COUNT(*) FROM ogrenci WHERE telefon_no = @telefonNo", con);

            commandCheckOkulNo.Parameters.AddWithValue("@okulNo", okulNo);
            commandCheckTcKimlikNo.Parameters.AddWithValue("@tcKimlikNo", tcKimlikNo);
            commandCheckTelefonNo.Parameters.AddWithValue("@telefonNo", telefonNo);

            int countOkulNo = (int)commandCheckOkulNo.ExecuteScalar();
            int countTcKimlikNo = (int)commandCheckTcKimlikNo.ExecuteScalar();
            int countTelefonNo = (int)commandCheckTelefonNo.ExecuteScalar();

            if (countOkulNo > 0) //Okul Numarası veritabanında varsa
            {
                MessageBox.Show("Okul numarası sistemde kayıtlı!");
                Temizle();
                con.Close();
                return;
            }

            if (countTcKimlikNo > 0) //TC kimlik numarası veritabanında varsa
            {
                MessageBox.Show("TC kimlik numarası sistemde kayıtlı!");
                Temizle();
                con.Close();
                return;
            }

            if (countTelefonNo > 0) //Telefon numarası veritabanında varsa
            {
                MessageBox.Show("Telefon numarası sistemde kayıtlı!");
                Temizle();
                con.Close();
                return;
            }


            //Veritabanında Öğrenci tablosuna ekleme
            string insertQuery = "INSERT INTO ogrenci (adi, tc_kimlik_no, okul_no, bolumu, sinifi, dogum_tarihi, telefon_no) VALUES (@adi, @tcKimlikNo, @okulNo, @bolumu, @sinifi, @dogumTarihi, @telefonNo)";
            SqlCommand insertCommand = new SqlCommand(insertQuery, con);
            insertCommand.Parameters.AddWithValue("@adi", adi);
            insertCommand.Parameters.AddWithValue("@tcKimlikNo", tcKimlikNo);
            insertCommand.Parameters.AddWithValue("@okulNo", okulNo);
            insertCommand.Parameters.AddWithValue("@bolumu", bolumu);
            insertCommand.Parameters.AddWithValue("@sinifi", sinifi);
            insertCommand.Parameters.AddWithValue("@dogumTarihi", dogumTarihi);
            insertCommand.Parameters.AddWithValue("@telefonNo", telefonNo);

            insertCommand.ExecuteNonQuery();
            con.Close();

            MessageBox.Show("Öğrenci başarıyla kaydedildi!");
            Temizle();

            ListeYukle();
        }

        private void button3_Click(object sender, EventArgs e) //Silme butonu
        {


            if (label8.Text.Trim() == "") //label8 boşsa yani listeden öğrenci seçilmemişse
            {
                MessageBox.Show("Listeden öğrenci seçin!");
                return;
            }

            int okulNo = int.Parse(label8.Text); // Label8'de tutulan öğrenci ID'sini alın

            con.Open();

            //Öğrenci ID'sinin veritabanında olup olmadığını kontrol etme
            string queryCheckOkulNo = "SELECT COUNT(*) FROM ogrenci WHERE ogrenci_id = @okulNo";
            SqlCommand commandCheckOkulNo = new SqlCommand(queryCheckOkulNo, con);
            commandCheckOkulNo.Parameters.AddWithValue("@okulNo", okulNo);

            int countOkulNo = (int)commandCheckOkulNo.ExecuteScalar();

            if (countOkulNo == 0) //yoksa
            {
                MessageBox.Show("Kullanıcı bilgisi bulunamadı!");
                con.Close();
                return;
            }

            //varsa silme işlemi
            string deleteQuery = "DELETE FROM ogrenci WHERE ogrenci_id = @okulNo";
            SqlCommand deleteCommand = new SqlCommand(deleteQuery, con);
            deleteCommand.Parameters.AddWithValue("@okulNo", okulNo);

            deleteCommand.ExecuteNonQuery();

            con.Close();

            MessageBox.Show("Öğrenci başarıyla kaldırıldı!");
            ListeYukle();
            Temizle();
            label8.Text = " ";

        }

        private void button2_Click(object sender, EventArgs e) // Düzenleme butonu
        {
            if (label8.Text.Trim() == "") // Label8 boşsa yani listeden öğrenci seçilmemişse
            {
                MessageBox.Show("Listeden öğrenci seçin!");
                return;
            }

            int ogrenciId = int.Parse(label8.Text); // Label8'de tutulan öğrenci ID'sini alın

            string okulNo = textBox1.Text;
            string adi = textBox2.Text;
            string bolumu = textBox3.Text;
            string sinifi = textBox4.Text;
            string tcKimlikNo = textBox5.Text;
            string dogumTarihi = textBox6.Text;
            string telefonNo = textBox7.Text;

            // Okul numarası, TC Kimlik numarası, Telefon numarası için benzersizlik kontrolü sorguları
            string uniqueOkulNoQuery = "SELECT COUNT(*) FROM ogrenci WHERE ogrenci_id != @ogrenciId AND okul_no = @okulNo";
            string uniqueTcKimlikNoQuery = "SELECT COUNT(*) FROM ogrenci WHERE ogrenci_id != @ogrenciId AND tc_kimlik_no = @tcKimlikNo";
            string uniqueTelefonNoQuery = "SELECT COUNT(*) FROM ogrenci WHERE ogrenci_id != @ogrenciId AND telefon_no = @telefonNo";

            // Benzersizlik kontrolü için SqlCommand nesneleri
            SqlCommand uniqueOkulNoCommand = new SqlCommand(uniqueOkulNoQuery, con);
            uniqueOkulNoCommand.Parameters.AddWithValue("@ogrenciId", ogrenciId);
            uniqueOkulNoCommand.Parameters.AddWithValue("@okulNo", okulNo);

            SqlCommand uniqueTcKimlikNoCommand = new SqlCommand(uniqueTcKimlikNoQuery, con);
            uniqueTcKimlikNoCommand.Parameters.AddWithValue("@ogrenciId", ogrenciId);
            uniqueTcKimlikNoCommand.Parameters.AddWithValue("@tcKimlikNo", tcKimlikNo);

            SqlCommand uniqueTelefonNoCommand = new SqlCommand(uniqueTelefonNoQuery, con);
            uniqueTelefonNoCommand.Parameters.AddWithValue("@ogrenciId", ogrenciId);
            uniqueTelefonNoCommand.Parameters.AddWithValue("@telefonNo", telefonNo);

            // Benzersizlik kontrolü için bağlantı açma ve sorgu çalıştırma
            con.Open();
            int existingOkulNoCount = (int)uniqueOkulNoCommand.ExecuteScalar();
            int existingTcKimlikNoCount = (int)uniqueTcKimlikNoCommand.ExecuteScalar();
            int existingTelefonNoCount = (int)uniqueTelefonNoCommand.ExecuteScalar();
            con.Close();

            // Benzersizlik kontrolleri
            if (existingOkulNoCount > 0)
            {
                MessageBox.Show("Bu okul numarasına sahip başka bir öğrenci bulunmaktadır. Lütfen farklı bir okul numarası giriniz!");
            }
            else if (existingTcKimlikNoCount > 0)
            {
                MessageBox.Show("Bu TC kimlik numarasına sahip başka bir öğrenci bulunmaktadır. Lütfen farklı bir TC kimlik numarası giriniz!");
            }
            else if (existingTelefonNoCount > 0)
            {
                MessageBox.Show("Bu telefon numarasına sahip başka bir öğrenci bulunmaktadır. Lütfen farklı bir telefon numarası giriniz!");
            }
            else //eğer benzersizse
            {
                // Öğrenciyi güncelleme sorgusu
                string updateQuery = "UPDATE ogrenci SET okul_no = @okulNo, adi = @adi, bolumu = @bolumu, sinifi = @sinifi, tc_kimlik_no = @tcKimlikNo, dogum_tarihi = @dogumTarihi, telefon_no = @telefonNo WHERE ogrenci_id = @ogrenciId";

                SqlCommand updateCommand = new SqlCommand(updateQuery, con);
                updateCommand.Parameters.AddWithValue("@okulNo", okulNo);
                updateCommand.Parameters.AddWithValue("@adi", adi);
                updateCommand.Parameters.AddWithValue("@bolumu", bolumu);
                updateCommand.Parameters.AddWithValue("@sinifi", sinifi);
                updateCommand.Parameters.AddWithValue("@tcKimlikNo", tcKimlikNo);
                updateCommand.Parameters.AddWithValue("@dogumTarihi", dogumTarihi);
                updateCommand.Parameters.AddWithValue("@telefonNo", telefonNo);
                updateCommand.Parameters.AddWithValue("@ogrenciId", ogrenciId);

                con.Open();
                int affectedRows = updateCommand.ExecuteNonQuery();
                con.Close();

                if (affectedRows > 0)
                {
                    MessageBox.Show("Öğrenci bilgileri başarıyla güncellendi!");
                }
                else
                {
                    MessageBox.Show("Öğrenci bilgileri güncellenemedi!");
                }

                ListeYukle(); // Listeyi güncelleme
                Temizle(); // TextBox'ları temizleme
                label8.Text = " "; // Seçili öğrenci ID'sini temizleme
            }
        }
    }
}
