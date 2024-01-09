using System;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace ogrenci
{
    public partial class OgretmenMenuForm : Form
    {
        private int ogretmenID;
        private string connectionString = "Data Source=ADN1;Initial Catalog=OgrOtm;Integrated Security=true"; //veritabanı bağlantısı

        public OgretmenMenuForm(int _ogretmenID)
        {
            InitializeComponent();
            ogretmenID = _ogretmenID; //giriş yaparken alınan öğretmen ID
        }

        private void listView1_MouseDoubleClick(object sender, MouseEventArgs e) //listedeki bir satıra çift tıklandığında 
        {
            ListView.SelectedListViewItemCollection selectedItems = listView1.SelectedItems;

            if (selectedItems.Count > 0)
            {
                ListViewItem item = selectedItems[0];

                // Satırın verilerini combobox'lara taşı
                comboBox1.Text = item.SubItems[0].Text; // Öğrenci adı
                comboBox2.Text = item.SubItems[1].Text; // Vize değeri
                comboBox3.Text = item.SubItems[2].Text; // Final değeri
                comboBox4.Text = item.SubItems[3].Text; // Büt değeri

                comboBox1.Enabled = false; //seçilen öğrencinin adını kitle
            }
        }

        private void Temizle()
        {
            comboBox1.SelectedItem = null;
            comboBox2.SelectedItem = null;
            comboBox3.SelectedItem = null;
            comboBox4.SelectedItem = null;

        }


        private void ListeYukle() //listviewda veritabanındaki verileri görüntülemeyi sağlayan method
        {
            listView1.Items.Clear();
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    // Öğretmenin ID'sini içeren ders tablosundaki dersin ID'si alınıp label 6ya yazılır. Öğretmenin ID'si ise label1e.
                    string queryDersID = "SELECT ders_id FROM ders WHERE ogretmen_id = @OgretmenID";
                    SqlCommand cmdDersID = new SqlCommand(queryDersID, connection);
                    cmdDersID.Parameters.AddWithValue("@OgretmenID", ogretmenID);
                    int dersID = Convert.ToInt32(cmdDersID.ExecuteScalar());

                    label1.Text = ogretmenID.ToString();
                    label6.Text = dersID.ToString();

                    // Ders ID'yi içeren ogrenci_ders_notlar tablosunda bulunan öğrenci ID'lerinden Öğrenci adlarını alma ve tablonun diğer verilerini alma
                    string queryOgrenciNotlari = "SELECT ogrenci.adi, ogrenci_ders_notlar.vize_degeri, ogrenci_ders_notlar.final_degeri, ogrenci_ders_notlar.but_degeri " +
                                                "FROM ogrenci_ders_notlar " +
                                                "INNER JOIN ogrenci ON ogrenci.ogrenci_id = ogrenci_ders_notlar.ogrenci_id " +
                                                "WHERE ogrenci_ders_notlar.ders_id = @DersID";

                    SqlCommand cmdOgrenciNotlari = new SqlCommand(queryOgrenciNotlari, connection);
                    cmdOgrenciNotlari.Parameters.AddWithValue("@DersID", dersID);

                    SqlDataReader reader = cmdOgrenciNotlari.ExecuteReader();

                    while (reader.Read())
                    {
                        ListViewItem item = new ListViewItem(reader.GetString(0)); // Ogrenci adı
                        item.SubItems.Add(reader.IsDBNull(1) ? "" : reader.GetDouble(1).ToString()); // Vize değeri boşsa " " şeklinde al
                        item.SubItems.Add(reader.IsDBNull(2) ? "" : reader.GetDouble(2).ToString()); // Final değeri boşsa
                        item.SubItems.Add(reader.IsDBNull(3) ? "" : reader.GetDouble(3).ToString()); // Büt değeri boşsa
                        listView1.Items.Add(item);
                    }

                    connection.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Hata: " + ex.Message);
            }
        }

        private int GetOgrenciID(string ogrenciAdi) //öğrenci adından öğrenci ID'sini alma
        {
            int ogrenciID = -1;
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    string query = "SELECT ogrenci_id FROM ogrenci WHERE adi = @OgrenciAdi";
                    SqlCommand cmd = new SqlCommand(query, connection);
                    cmd.Parameters.AddWithValue("@OgrenciAdi", ogrenciAdi);

                    object result = cmd.ExecuteScalar();

                    if (result != null)
                    {
                        ogrenciID = Convert.ToInt32(result);
                    }

                    connection.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Hata: " + ex.Message);
            }
            return ogrenciID;
        }


        private void OgretmenMenuForm_Load(object sender, EventArgs e) // Form açıldığında
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    // Öğrenci adlarını veritabanından al
                    string queryOgrenciAdlari = "SELECT adi FROM ogrenci";
                    SqlCommand cmdOgrenciAdlari = new SqlCommand(queryOgrenciAdlari, connection);

                    SqlDataReader reader = cmdOgrenciAdlari.ExecuteReader();

                    while (reader.Read())
                    {
                        comboBox1.Items.Add(reader.GetString(0)); // Öğrenci adını ComboBox'a ekle
                    }

                    connection.Close();
                }

                // Öğretmen ID'ye göre ders tablosundaki kaydı kontrol et
                bool ogretmenDersVarMi = false;
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    string query = "SELECT COUNT(*) FROM ders WHERE ogretmen_id = @OgretmenID";
                    SqlCommand cmd = new SqlCommand(query, connection);
                    cmd.Parameters.AddWithValue("@OgretmenID", ogretmenID);

                    int dersKayitSayisi = (int)cmd.ExecuteScalar();
                    if (dersKayitSayisi > 0)
                    {
                        ogretmenDersVarMi = true;
                    }

                    connection.Close();
                }

                // Ders kaydı varsa
                if (ogretmenDersVarMi)
                {
                    // Gerekli kontrolleri görünür yap
                    button1.Visible = true;
                    button2.Visible = true;
                    button3.Visible = true;
                    button4.Visible = true;                    
                    listView1.Visible = true;
                    comboBox1.Visible = true;
                    comboBox2.Visible = true;
                    comboBox3.Visible = true;
                    comboBox4.Visible = true;
                    label2.Visible = true;
                    label3.Visible = true;
                    label4.Visible = true;
                    label5.Visible = true;

                    // Görünmesi gerekmeyen kontrolleri gizle
                    label7.Visible = false;
                }
                else
                {
                    // Gerekli kontrolleri gizle
                    button1.Visible = false;
                    button2.Visible = false;
                    button3.Visible = false;
                    button4.Visible = false;
                    listView1.Visible = false;
                    comboBox1.Visible = false;
                    comboBox2.Visible = false;
                    comboBox3.Visible = false;
                    comboBox4.Visible = false;
                    label2.Visible = false;
                    label3.Visible = false;
                    label4.Visible = false;
                    label5.Visible = false;

                    // Görünmesi gereken kontrolleri görünür yap
                    label7.Visible = true;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Hata: " + ex.Message);
            }

            ListeYukle();
        }
    
    private bool IsOgrenciExists(string ogrenciAdi) //Öğrenci adı listview'da var mı kontrolü
        {
            foreach (ListViewItem item in listView1.Items)
            {
                if (item.SubItems[0].Text.Equals(ogrenciAdi, StringComparison.OrdinalIgnoreCase))
                {
                    return true; // Öğrenci adı listede bulunuyor
                }
            }
            return false; // Öğrenci adı listede bulunmuyor
        }

        private void button1_Click(object sender, EventArgs e) //ekleme butonu
        {
            if (comboBox1.SelectedItem != null) //Öğrenci seçimi yapılmışsa
            {
                string ogrenciAdi = comboBox1.SelectedItem.ToString();

                float? vizeDegeri = null; //Bütün değişkenleri default olarak null alma
                float? finalDegeri = null;
                float? butDegeri = null;

                if (!string.IsNullOrWhiteSpace(comboBox2.SelectedItem?.ToString())) //comboboxta not seçimi yapılmışsa null olan değerlere not değerini atama
                {
                    vizeDegeri = float.Parse(comboBox2.SelectedItem.ToString());
                }

                if (!string.IsNullOrWhiteSpace(comboBox3.SelectedItem?.ToString()))
                {
                    finalDegeri = float.Parse(comboBox3.SelectedItem.ToString());
                }

                if (!string.IsNullOrWhiteSpace(comboBox4.SelectedItem?.ToString()))
                {
                    butDegeri = float.Parse(comboBox4.SelectedItem.ToString());
                }

                int ogrenciID = GetOgrenciID(ogrenciAdi); // Öğrenci adına göre öğrenci ID'sini al

                if (ogrenciID != -1)
                {
                    if (!IsOgrenciExists(ogrenciAdi)) //öğrenci adı listviewda yoksa 
                    {
                        int dersID = int.Parse(label6.Text); // label6'dan ders ID'sini al

                        try
                        {
                            using (SqlConnection connection = new SqlConnection(connectionString))
                            {
                                connection.Open();

                                //ekleme işlemini yap
                                string query = "INSERT INTO ogrenci_ders_notlar (ogrenci_id, ders_id, vize_degeri, final_degeri, but_degeri) " +
                                    "VALUES (@OgrenciID, @DersID, @VizeDegeri, @FinalDegeri, @ButDegeri)";

                                SqlCommand cmd = new SqlCommand(query, connection);
                                cmd.Parameters.AddWithValue("@OgrenciID", ogrenciID);
                                cmd.Parameters.AddWithValue("@DersID", dersID);
                                cmd.Parameters.AddWithValue("@VizeDegeri", vizeDegeri ?? (object)DBNull.Value);
                                cmd.Parameters.AddWithValue("@FinalDegeri", finalDegeri ?? (object)DBNull.Value);
                                cmd.Parameters.AddWithValue("@ButDegeri", butDegeri ?? (object)DBNull.Value);

                                cmd.ExecuteNonQuery();

                                connection.Close();
                            }

                            ListeYukle(); //liste güncelle
                            Temizle(); //comboboxları temizle
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("Hata: " + ex.Message);
                        }
                    }
                    else
                    {
                        MessageBox.Show("Bu öğrenci zaten listede var.");
                    }
                }
                else
                {
                    MessageBox.Show("Öğrenci bulunamadı.");
                }
            }
            else
            {
                MessageBox.Show("Lütfen öğrenci seçin.");
            }
        }

        private void button3_Click(object sender, EventArgs e) //silme butonu
        {
            if (!string.IsNullOrEmpty(label6.Text) && comboBox1.SelectedItem != null)
            {
                int dersID = int.Parse(label6.Text); // Label'dan ders ID'sini al
                string ogrenciAdi = comboBox1.SelectedItem.ToString();
                int ogrenciID = GetOgrenciID(ogrenciAdi); // Öğrenci adına göre öğrenci ID'sini al

                try
                {
                    using (SqlConnection connection = new SqlConnection(connectionString))
                    {
                        connection.Open();

                        // Belirli ders ID'sine ve öğrenci ID'sine sahip satırların ogrenci_ders_notlar_id'lerini al
                        string query = "SELECT ogrenci_ders_not_id FROM ogrenci_ders_notlar WHERE ders_id = @DersID AND ogrenci_id = @OgrenciID";
                        SqlCommand cmd = new SqlCommand(query, connection);
                        cmd.Parameters.AddWithValue("@DersID", dersID);
                        cmd.Parameters.AddWithValue("@OgrenciID", ogrenciID);

                        List<int> ogrenciDersNotlarIDs = new List<int>();
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                ogrenciDersNotlarIDs.Add(reader.GetInt32(0));
                            }
                        }

                        // Her bir ogrenci_ders_notlar_id için ilgili satırı sil
                        foreach (int ogrenciDersNotID in ogrenciDersNotlarIDs)
                        {
                            SqlCommand deleteCmd = new SqlCommand("DELETE FROM ogrenci_ders_notlar WHERE ogrenci_ders_not_id = @OgrenciDersNotID", connection);
                            deleteCmd.Parameters.AddWithValue("@OgrenciDersNotID", ogrenciDersNotID);
                            deleteCmd.ExecuteNonQuery();
                        }

                        connection.Close();

                        MessageBox.Show("Seçilen ders ve öğrenci notları başarıyla silindi.");
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Hata: " + ex.Message);
                }
            }
            else
            {
                MessageBox.Show("Lütfen bir öğrenci seçin.");
            }

            ListeYukle();
            Temizle();
            comboBox1.Enabled = true; //silme işlemi uygulandıktan sonra öğrenci seçimi yapılan combobox'ı tekrar aktif et
        }

        private void button2_Click(object sender, EventArgs e) //düzenleme butonu
        {
            if (!string.IsNullOrEmpty(label6.Text) && comboBox1.SelectedItem != null) //seçim yapılmışsa ve comboboxta öğrencinin adı bulunuyorsa
            {
                int dersID = int.Parse(label6.Text); // Label'dan ders ID'sini al
                string ogrenciAdi = comboBox1.SelectedItem.ToString();
                int ogrenciID = GetOgrenciID(ogrenciAdi); // Öğrenci adına göre öğrenci ID'sini al

                try
                {
                    using (SqlConnection connection = new SqlConnection(connectionString))
                    {
                        connection.Open();

                        // Belirli ders ID'sine ve öğrenci ID'sine sahip satırların ogrenci_ders_notlar_id'lerini al
                        string query = "SELECT ogrenci_ders_not_id FROM ogrenci_ders_notlar WHERE ders_id = @DersID AND ogrenci_id = @OgrenciID";
                        SqlCommand cmd = new SqlCommand(query, connection);
                        cmd.Parameters.AddWithValue("@DersID", dersID);
                        cmd.Parameters.AddWithValue("@OgrenciID", ogrenciID);

                        int ogrenciDersNotID = -1;
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                ogrenciDersNotID = reader.GetInt32(0);
                            }
                        }

                        if (ogrenciDersNotID != -1)
                        {
                            // Satırı güncelle eğer vize, final veya büt notu girilmemişse boş olanlara null değeri ata 
                            string updateQuery = "UPDATE ogrenci_ders_notlar SET vize_degeri = @VizeDegeri, final_degeri = @FinalDegeri, but_degeri = @ButDegeri WHERE ogrenci_ders_not_id = @OgrenciDersNotID";
                            SqlCommand updateCmd = new SqlCommand(updateQuery, connection);
                            updateCmd.Parameters.AddWithValue("@VizeDegeri", string.IsNullOrWhiteSpace(comboBox2.Text) ? DBNull.Value : float.Parse(comboBox2.Text));
                            updateCmd.Parameters.AddWithValue("@FinalDegeri", string.IsNullOrWhiteSpace(comboBox3.Text) ? DBNull.Value : float.Parse(comboBox3.Text));
                            updateCmd.Parameters.AddWithValue("@ButDegeri", string.IsNullOrWhiteSpace(comboBox4.Text) ? DBNull.Value : float.Parse(comboBox4.Text));
                            updateCmd.Parameters.AddWithValue("@OgrenciDersNotID", ogrenciDersNotID);
                            updateCmd.ExecuteNonQuery();

                            MessageBox.Show("Seçilen ders ve öğrenci notları başarıyla güncellendi.");
                        }
                        else
                        {
                            MessageBox.Show("Belirtilen ders ve öğrenci kombinasyonu bulunamadı.");
                        }

                        connection.Close();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Hata: " + ex.Message);
                }
            }
            else
            {
                MessageBox.Show("Lütfen bir ders seçin ve bir öğrenci seçin.");
            }

            ListeYukle(); //listeyi güncelle
            Temizle(); //comboboxlardaki seçimi temizle
            comboBox1.Enabled = true; //kitlenen öğrenci adını içeren combobox'ı tekrar aktif et
        }

        private void button4_Click(object sender, EventArgs e) //bilgilendirme butonu
        {
            MessageBox.Show("Veritabanına ekleme yapmak için artı butonu, güncelleme yapmak için kalem butonu, silmek için çöp kutusu butonunu kullanın. Güncelleme ve silme işlemi yapmak için öncelikle Listeden ilgili öğrenciyi çift tıklayarak seçin. Sınav notuna hiç bir giriş yapmazsanız bunun 0 olarak değil, sınav yapılmadı olarak değerlendirildiğini unutmayın.");
        }
    }
}
