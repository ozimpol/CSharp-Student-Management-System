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
    public partial class AdminOgretmen : Form
    {

        SqlConnection con = new SqlConnection("Data Source=ADN1;Initial Catalog=OgrOtm;Integrated Security=true"); //veri tabanı bağlantısı

        public AdminOgretmen()
        {
            InitializeComponent();
        }

        private void ListeYukle()
        {
            listView1.Items.Clear();
            // Öğretmen tablosundan verileri çekmek için gerekli SQL sorgusu
            string query = "SELECT adi, tc_kimlik_no, telefon_no FROM ogretmen";

            // SQL sorgusunu çalıştırmak için SqlConnection ve SqlCommand

            {
                SqlCommand command = new SqlCommand(query, con);
                con.Open();

                SqlDataReader reader = command.ExecuteReader();

                // ListView sütunlarını oluşturma
                listView1.Columns.Add("Ad");
                listView1.Columns.Add("TC Kimlik No");
                listView1.Columns.Add("Telefon No");

                while (reader.Read())
                {
                    // Databasedeki Öğretmen tablosundaki verileri listView'a ekleme
                    ListViewItem item = new ListViewItem(reader["adi"].ToString());
                    item.SubItems.Add(reader["tc_kimlik_no"].ToString());
                    item.SubItems.Add(reader["telefon_no"].ToString());

                    listView1.Items.Add(item);
                }

                reader.Close();
                con.Close();
            }
        }

        private void Temizle()
        {
            textBox2.Text = "";
            textBox5.Text = "";
            textBox7.Text = "";
        }

        private void listView1_MouseDoubleClick(object sender, MouseEventArgs e) //listView'daki bir satıra çift tıklandığında 
        {
            if (listView1.SelectedItems.Count > 0) //çift tıklanan bir satır varsa
            {
                ListViewItem selectedRow = listView1.SelectedItems[0];

                //listView'da çift tıklanan satırdaki veriler TextBox'lara yerleştirilir. 
                textBox2.Text = selectedRow.SubItems[0].Text; // Adı
                textBox5.Text = selectedRow.SubItems[1].Text; // TC Kimlik No
                textBox7.Text = selectedRow.SubItems[2].Text; // Telefon No


                //Kayıt ve Düzenleme işlemlerinde kullanmak üzere seçilen satırdaki TC Kimlik Numarasına göre 
                //Öğretmen tablosundan Öğretmen ID alınıp label8'e yazılır.
                string tckimlikNo = textBox5.Text;
                con.Open();
                string queryGetOgretmenId = "SELECT ogretmen_id FROM ogretmen WHERE tc_kimlik_no = @tckimlikNo";

                SqlCommand commandGetOgretmenId = new SqlCommand(queryGetOgretmenId, con);
                commandGetOgretmenId.Parameters.AddWithValue("@tckimlikNo", tckimlikNo);

                int ogretmenId = (int)commandGetOgretmenId.ExecuteScalar();

                label8.Text = ogretmenId.ToString();
                con.Close();
            }
        }

        private void button4_Click(object sender, EventArgs e) //bilgilendirme butonu
        {
            MessageBox.Show("Öğretmen eklemek için artı simgesini, silmek için çöp kutusu simgesini, düzenlemek için kalem simgesini kullanınız. Silme ve düzenleme için listeden üzerinde işlem yapmak istediğiniz satırı seçmeyi unutmayınız.", "BİLGİLENDİRME");

        }

        private void button5_Click(object sender, EventArgs e) //geri butonu
        {
            AdminMenuForm adminMenu = new AdminMenuForm();
            adminMenu.Show();
            this.Hide();
        }

        private void button1_Click(object sender, EventArgs e) //ekleme butonu
        {
            string adi = textBox2.Text;
            string tcKimlikNo = textBox5.Text;
            string telefonNo = textBox7.Text;

            if (string.IsNullOrEmpty(adi) || string.IsNullOrEmpty(tcKimlikNo) || string.IsNullOrEmpty(telefonNo)) //Girilen verilerde boş olan varsa
            {
                MessageBox.Show("Lütfen bütün alanları doldurun!");
                return;
            }

            con.Open();

            //Girilen TC Kimlik numarasını ve Telefon numarasını içeren satırlar sayılır. 
            SqlCommand commandCheckTcKimlikNo = new SqlCommand("SELECT COUNT(*) FROM ogretmen WHERE tc_kimlik_no = @tcKimlikNo", con);
            SqlCommand commandCheckTelefonNo = new SqlCommand("SELECT COUNT(*) FROM ogretmen WHERE telefon_no = @telefonNo", con);

            commandCheckTcKimlikNo.Parameters.AddWithValue("@tcKimlikNo", tcKimlikNo);
            commandCheckTelefonNo.Parameters.AddWithValue("@telefonNo", telefonNo);

            int countTcKimlikNo = (int)commandCheckTcKimlikNo.ExecuteScalar();
            int countTelefonNo = (int)commandCheckTelefonNo.ExecuteScalar();


            if (countTcKimlikNo > 0) //Girilen TC Kimlik numarası veritabanında varsa
            {
                MessageBox.Show("TC kimlik numarası sistemde kayıtlı!");
                Temizle();
                con.Close();
                return;
            }
             
            if (countTelefonNo > 0) //Girilen Telefon Numarası veritabanında varsa
            {
                MessageBox.Show("Telefon numarası sistemde kayıtlı!");
                Temizle();
                con.Close();
                return;
            }

            //Girilen verilerin veritabanına işlenmesi
            string insertQuery = "INSERT INTO ogretmen (adi, tc_kimlik_no, telefon_no) VALUES (@adi, @tcKimlikNo, @telefonNo)";
            SqlCommand insertCommand = new SqlCommand(insertQuery, con);
            insertCommand.Parameters.AddWithValue("@adi", adi);
            insertCommand.Parameters.AddWithValue("@tcKimlikNo", tcKimlikNo);
            insertCommand.Parameters.AddWithValue("@telefonNo", telefonNo);

            insertCommand.ExecuteNonQuery();
            con.Close();

            MessageBox.Show("Öğretmen başarıyla kaydedildi!");
            Temizle();

            ListeYukle(); // Listenin güncel halini görüntülemek için ListView'a veritabanındaki verileri yükleyen method çağırılır.
        }

        private void AdminOgretmen_Load(object sender, EventArgs e)
        {
            ListeYukle(); //Form ilk yüklendiğinde ListView'a veritabanındaki değerler yerleştirilir.
        }

        private void button2_Click(object sender, EventArgs e)
        {
            //ListViewa çift tıklandığında label'a kaydedilmesi gereken Öğretmen ID nin kontrolu yapılır.
            //Eğer bu değer boşsa Listeden seçim yapılmamış demektir.
            if (label8.Text.Trim() == "")
            {
                MessageBox.Show("Listeden öğretmen seçin!");
                return;
            }

            int ogretmenId = int.Parse(label8.Text); // Label8'de tutulan öğretmen ID'sini alın

            string adi = textBox2.Text;
            string tcKimlikNo = textBox5.Text;
            string telefonNo = textBox7.Text;

            // Öğretmeni güncelleme sorgusu
            string updateQuery = "UPDATE ogretmen SET adi = @adi, tc_kimlik_no = @tcKimlikNo, telefon_no = @telefonNo WHERE ogretmen_id = @ogretmenId";

            SqlCommand updateCommand = new SqlCommand(updateQuery, con);
            updateCommand.Parameters.AddWithValue("@adi", adi);
            updateCommand.Parameters.AddWithValue("@tcKimlikNo", tcKimlikNo);
            updateCommand.Parameters.AddWithValue("@telefonNo", telefonNo);
            updateCommand.Parameters.AddWithValue("@ogretmenId", ogretmenId);

            con.Open();
            int affectedRows = updateCommand.ExecuteNonQuery();
            con.Close();

            if (affectedRows > 0)
            {
                MessageBox.Show("Öğretmen bilgileri başarıyla güncellendi!");
            }
            else
            {
                MessageBox.Show("Öğretmen bilgileri güncellenemedi!");
            }

            ListeYukle(); //ListView'i güncelleme
            Temizle(); //TextBoxtaki değerleri sıfırlama
            label8.Text = " "; //Çift tıklandıktan sonra label'da tutulan Öğretmen ID sini temizleme
        }

        private void button3_Click(object sender, EventArgs e) //silme işlemi
        {
            string ogretmenId = label8.Text; //çift tıklanan satır kontrolü yapmak amacıyla Öğretmen ID si label'da sorgulanır.

            if (string.IsNullOrEmpty(ogretmenId)) //label boşsa
            {
                MessageBox.Show("Lütfen önce seçim yapınız!");
                return;
            }

            con.Open();

            //Öğretmen ID'sini içeren bir satır olup olmadığına dair kontrol
            string queryCheckOgretmen = "SELECT COUNT(*) FROM ogretmen WHERE ogretmen_id = @ogretmenId";
            SqlCommand commandCheckOgretmen = new SqlCommand(queryCheckOgretmen, con);
            commandCheckOgretmen.Parameters.AddWithValue("@ogretmenId", ogretmenId);

            int countOgretmen = (int)commandCheckOgretmen.ExecuteScalar();

            if (countOgretmen == 0) //Öğrenci ID'si bulunamazsa
            {
                MessageBox.Show("Kullanıcı bilgisi bulunamadı!");
                con.Close();
                return;
            }

            //İlgili Öğrenci ID'sini içeren satırın silinmesi
            string deleteQuery = "DELETE FROM ogretmen WHERE ogretmen_id = @ogretmenId";
            SqlCommand deleteCommand = new SqlCommand(deleteQuery, con);
            deleteCommand.Parameters.AddWithValue("@ogretmenId", ogretmenId);

            deleteCommand.ExecuteNonQuery();

            con.Close();

            MessageBox.Show("Öğretmen başarıyla kaldırıldı!");
            ListeYukle(); //ListView güncelleme
            Temizle(); //TextBoxları temizleme
            label8.Text = " "; //label'daki Öğretmen ID sini silme

        }
    }
}
