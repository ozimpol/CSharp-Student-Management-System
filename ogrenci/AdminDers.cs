using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace ogrenci
{
    public partial class AdminDers : Form
    {

        SqlConnection con = new SqlConnection("Data Source=ADN1;Initial Catalog=OgrOtm;Integrated Security=true"); //veritabanı bağlantısı

        public AdminDers()
        {
            InitializeComponent();
        }

        private void Temizle()
        {
            textBox1.Text = "";
            comboBox1.SelectedItem = null;
            label3.Text = "";
        }

        private void ListeYukle() //listView'a veritabanındaki verileri ekleme
        {
            listView1.Items.Clear();

            string query = "SELECT d.ders_adi, o.adi AS ogretmen_adi " +
                           "FROM ders d " +
                           "INNER JOIN ogretmen o ON d.ogretmen_id = o.ogretmen_id";

            SqlCommand command = new SqlCommand(query, con);
            SqlDataAdapter adapter = new SqlDataAdapter(command);
            DataTable dataTable = new DataTable();

            try
            {
                con.Open();
                adapter.Fill(dataTable);

                foreach (DataRow row in dataTable.Rows)
                {
                    ListViewItem item = new ListViewItem(row["ders_adi"].ToString());
                    item.SubItems.Add(row["ogretmen_adi"].ToString());
                    listView1.Items.Add(item);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Veritabanından veri çekerken hata oluştu: " + ex.Message);
            }
            finally
            {
                con.Close();
            }
        }

        private void OgretmenAdlariComboBoxDoldur() //comboboxlara öğretmen isimlerini ekleme
        {
            comboBox1.Items.Clear();

            string query = "SELECT adi FROM ogretmen";

            SqlCommand command = new SqlCommand(query, con);

            try
            {
                con.Open();
                SqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    comboBox1.Items.Add(reader["adi"].ToString());
                }

                reader.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Öğretmen adları alınırken bir hata oluştu: " + ex.Message);
            }
            finally
            {
                con.Close();
            }
        }

        private void button5_Click(object sender, EventArgs e) //geri butonu
        {
            AdminMenuForm adminMenu = new AdminMenuForm();
            adminMenu.Show();
            this.Hide();
        }

        private void listView1_MouseDoubleClick(object sender, MouseEventArgs e) //listedeki bir satıra çift tıklandığında
        {
            if (listView1.SelectedItems.Count > 0) //çift tıklanan satır varsa
            {
                ListViewItem selectedRow = listView1.SelectedItems[0];

                textBox1.Text = selectedRow.SubItems[0].Text; // Ders Adını textbox1 e
                comboBox1.SelectedItem = selectedRow.SubItems[1].Text; // Öğretmen adını textbox2 ye yazdır

                string dersAdi = textBox1.Text;
                con.Open();
                string queryGetDersId = "SELECT ders_id FROM ders WHERE ders_adi = @dersAdi";

                SqlCommand commandGetDersId = new SqlCommand(queryGetDersId, con);
                commandGetDersId.Parameters.AddWithValue("@dersAdi", dersAdi);

                int DersId = (int)commandGetDersId.ExecuteScalar(); // ders_id'yi al

                label3.Text = DersId.ToString(); // ders_id'yi label3'e yazdır
                con.Close();
            }
        }

        private void AdminDers_Load(object sender, EventArgs e)
        {
            ListeYukle();
            OgretmenAdlariComboBoxDoldur();
        }

        private void button1_Click(object sender, EventArgs e) //ekleme butonu
        {
            if (!string.IsNullOrEmpty(textBox1.Text)) //ders adı boş değilse
            {
                string dersAdi = textBox1.Text.Trim(); // TextBox'tan ders adını al

                if (comboBox1.SelectedItem != null)
                {
                    string secilenOgretmenAdi = comboBox1.SelectedItem.ToString(); // ComboBox'tan seçilen öğretmen adını al

                    // Ders adının veritabanında olup olmadığını kontrol et
                    con.Open();
                    string queryDersKontrol = "SELECT COUNT(*) FROM ders WHERE ders_adi = @dersAdi";
                    SqlCommand commandDersKontrol = new SqlCommand(queryDersKontrol, con);
                    commandDersKontrol.Parameters.AddWithValue("@dersAdi", dersAdi);
                    int dersSayisi = (int)commandDersKontrol.ExecuteScalar();
                    con.Close();

                    if (dersSayisi == 0)
                    {
                        // ComboBox'tan seçilen öğretmenin ders sayısını kontrol et
                        con.Open();
                        string queryOgretmenDersSayisi = "SELECT COUNT(*) FROM ders d INNER JOIN ogretmen o ON d.ogretmen_id = o.ogretmen_id WHERE o.adi = @ogretmenAdi";
                        SqlCommand commandOgretmenDersSayisi = new SqlCommand(queryOgretmenDersSayisi, con);
                        commandOgretmenDersSayisi.Parameters.AddWithValue("@ogretmenAdi", secilenOgretmenAdi);
                        int ogretmenDersSayisi = (int)commandOgretmenDersSayisi.ExecuteScalar();
                        con.Close();

                        if (ogretmenDersSayisi < 1)
                        {
                            // Öğretmenin ders sayısı 1'den az ise dersi ve öğretmeni ekle
                            con.Open();
                            string queryGetOgretmenId = "SELECT ogretmen_id FROM ogretmen WHERE adi = @ogretmenAdi";
                            SqlCommand commandGetOgretmenId = new SqlCommand(queryGetOgretmenId, con);
                            commandGetOgretmenId.Parameters.AddWithValue("@ogretmenAdi", secilenOgretmenAdi);
                            int ogretmenId = (int)commandGetOgretmenId.ExecuteScalar();

                            string queryDersEkle = "INSERT INTO ders (ders_adi, ogretmen_id) VALUES (@dersAdi, @ogretmenId)";
                            SqlCommand commandDersEkle = new SqlCommand(queryDersEkle, con);
                            commandDersEkle.Parameters.AddWithValue("@dersAdi", dersAdi);
                            commandDersEkle.Parameters.AddWithValue("@ogretmenId", ogretmenId);
                            commandDersEkle.ExecuteNonQuery();

                            con.Close();
                            ListeYukle();
                            MessageBox.Show("Ders başarıyla eklendi.");
                            Temizle();
                        }
                        else
                        {
                            MessageBox.Show("Seçilen öğretmen başka bir derse atandığı için yeni ders eklenemiyor.");
                        }
                    }
                    else
                    {
                        MessageBox.Show("Bu ders zaten sistemde mevcut.");
                    }
                }
                else
                {
                    MessageBox.Show("Lütfen bir öğretmen seçiniz.");
                }
            }
            else
            {
                MessageBox.Show("Ders adı bölümü boş bırakılamaz");
            }
        }


        private void button2_Click(object sender, EventArgs e) //düzenleme butonu 
        {
            if (!string.IsNullOrEmpty(label3.Text)) //label 3 boş değilse yani seçilmiş bir satır varsa
            {
                string dersAdi = textBox1.Text.Trim(); // TextBox'tan ders adını al
                string secilenOgretmenAdi = comboBox1.SelectedItem.ToString(); // ComboBox'tan seçilen öğretmen adını al
                int dersId = int.Parse(label3.Text); // Label3'teki ders_id'sini al

                // Ders adının benzersizlik kontrolü
                con.Open();
                string queryDersKontrol = "SELECT COUNT(*) FROM ders WHERE ders_adi = @dersAdi AND ders_id <> @dersId";
                SqlCommand commandDersKontrol = new SqlCommand(queryDersKontrol, con);
                commandDersKontrol.Parameters.AddWithValue("@dersAdi", dersAdi);
                commandDersKontrol.Parameters.AddWithValue("@dersId", dersId);
                int dersSayisi = (int)commandDersKontrol.ExecuteScalar();

                //öğretmenin başka bir derse atanıp atanmadığının kontrolü
                string queryOgretmenDersSayisi = "SELECT COUNT(*) FROM ders d INNER JOIN ogretmen o ON d.ogretmen_id = o.ogretmen_id WHERE o.adi = @ogretmenAdi AND d.ders_id <> @dersId";
                SqlCommand commandOgretmenDersSayisi = new SqlCommand(queryOgretmenDersSayisi, con);
                commandOgretmenDersSayisi.Parameters.AddWithValue("@ogretmenAdi", secilenOgretmenAdi);
                commandOgretmenDersSayisi.Parameters.AddWithValue("@dersId", dersId);
                int ogretmenDersSayisi = (int)commandOgretmenDersSayisi.ExecuteScalar();

                if (dersSayisi == 0 && ogretmenDersSayisi < 1) //ders adı ve öğretmen benzersizse
                {
                    string queryGuncelle = "UPDATE ders SET ders_adi = @dersAdi, ogretmen_id = @ogretmenId WHERE ders_id = @dersId";
                    SqlCommand commandGuncelle = new SqlCommand(queryGuncelle, con);
                    commandGuncelle.Parameters.AddWithValue("@dersAdi", dersAdi);

                    string queryGetOgretmenId = "SELECT ogretmen_id FROM ogretmen WHERE adi = @ogretmenAdi";
                    SqlCommand commandGetOgretmenId = new SqlCommand(queryGetOgretmenId, con);
                    commandGetOgretmenId.Parameters.AddWithValue("@ogretmenAdi", secilenOgretmenAdi);
                    int ogretmenId = (int)commandGetOgretmenId.ExecuteScalar();
                    commandGuncelle.Parameters.AddWithValue("@ogretmenId", ogretmenId);

                    commandGuncelle.Parameters.AddWithValue("@dersId", dersId);
                    commandGuncelle.ExecuteNonQuery();

                    con.Close();
                    ListeYukle(); //listeyi güncelle
                    MessageBox.Show("Ders bilgileri güncellendi.");
                    Temizle(); //textboxları ve combobox'ı temizle
                }
                else
                {
                    MessageBox.Show("Ders adı zaten mevcut veya seçilen öğretmen başka bir derse atanmış olduğu için güncelleme yapılamıyor.");
                    con.Close();
                }
            }
            else
            {
                MessageBox.Show("İşlem yapmak için önce listeden bir sıraya çift tıklayın.");
            }
        }

        private void button3_Click(object sender, EventArgs e) //silme butonu
        {
            if (!string.IsNullOrEmpty(label3.Text)) //label 3 boş değilse yani seçilmiş bir satır varsa
            {
                string dersAdi = textBox1.Text.Trim(); // TextBox'tan ders adını al
                int dersId = int.Parse(label3.Text); // Label3'teki ders_id'sini al

                // Ders adının ve ders_id'sinin kontrolü
                con.Open();
                string queryDersKontrol = "SELECT COUNT(*) FROM ders WHERE ders_adi = @dersAdi AND ders_id = @dersId";
                SqlCommand commandDersKontrol = new SqlCommand(queryDersKontrol, con);
                commandDersKontrol.Parameters.AddWithValue("@dersAdi", dersAdi);
                commandDersKontrol.Parameters.AddWithValue("@dersId", dersId);
                int dersSayisi = (int)commandDersKontrol.ExecuteScalar();

                if (dersSayisi > 0) //ders adı ve ders idsi veritabanında yer alıyorsa silme işlemi
                {
                    string querySil = "DELETE FROM ders WHERE ders_id = @dersId";
                    SqlCommand commandSil = new SqlCommand(querySil, con);
                    commandSil.Parameters.AddWithValue("@dersId", dersId);
                    commandSil.ExecuteNonQuery();

                    con.Close();
                    ListeYukle();
                    MessageBox.Show("Ders bilgisi sistemden silindi.");
                    Temizle();
                }
                else //eğer listeye çift tıklanmış ve label boş değilse ama textboxtaki ders adı ile labeldaki ders id uyumlu değilse
                     //bu listeye çift tıklandıktan sonra direkt silmeden önce textboxta oynanma yapıldığı anlamına gelir. buna karşılık verilen uyarı mesajı
                {
                    con.Close();
                    MessageBox.Show("Ders bilgisini sistemden silmek için, listeden bir ders seçtikten sonra direkt olarak sil tuşuna basınız.");
                }
            }
            else
            {
                MessageBox.Show("Silme işlemi için önce listeden bir sıraya çift tıklayın.");
            }
        }

        private void button4_Click(object sender, EventArgs e) //bilgilendirme butonu
        {
            MessageBox.Show("Yeni bir ders eklemek için artı butonu, mevcut bir dersi düzenlemek için kalem butonu, silmek için ise çöp kutusu butonunu kullanınız. Üzerinde işlem yapmak istediğiniz dersi, listeden çift tıklayarak seçiniz.");
        }
    }
}
