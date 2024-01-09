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

namespace ogrenci
{
    public partial class OgrenciMenuForm : Form
    {
        private int ogrenciID; // ogrenciID değişkeni tanımlama

        // Parametreli kurucu metot oluşturuluyor
        public OgrenciMenuForm(int _ogrenciID)
        {
            InitializeComponent();
            ogrenciID = _ogrenciID; //  giriş yapılırken kaydedilen OgrenciID'yi al
        }

        private void OgrenciMenuForm_Load(object sender, EventArgs e) //form açıldığında
        {
            //Öğrenci ID'sini içeren satırı bulup, satırdaki ders IDsini kullanarak ders tablosundan ders adını alıp
            // bütün bu verileri listede görübtüleme
            string query = "SELECT ders.ders_adi, ogrenci_ders_notlar.vize_degeri, ogrenci_ders_notlar.final_degeri, ogrenci_ders_notlar.but_degeri " +
               "FROM ogrenci_ders_notlar " +
               "INNER JOIN ders ON ogrenci_ders_notlar.ders_id = ders.ders_id " +
               "WHERE ogrenci_ders_notlar.ogrenci_id = @ogrenciID";

            using (SqlConnection connection = new SqlConnection("Data Source=ADN1;Initial Catalog=OgrOtm;Integrated Security=true"))
            {
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@ogrenciID", ogrenciID);



                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();

                    while (reader.Read())
                    {
                        //ortalama hesaplamaları için bütün gerekli verileri tanımlama.
                        //notlara default olarak not olarak verilemeyecek bir değer atama
                        string dersAdi = reader["ders_adi"].ToString();
                        string harfnotu = "";
                        string vizeDegeri2 = "";
                        string finalDegeri2 = "";
                        string butDegeri2 = "";
                        float vizeDegeri = 101, finalDegeri = 101, butDegeri = 101;

                        if (!reader.IsDBNull(reader.GetOrdinal("vize_degeri"))) //veritabanında vize notu varsa vize değeri değişkenini güncelleme
                        {
                            vizeDegeri = float.Parse(reader["vize_degeri"].ToString());
                        }

                        if (!reader.IsDBNull(reader.GetOrdinal("final_degeri"))) // veritabanında final notu varsa final notu değişkenini güncelleme
                        {
                            finalDegeri = float.Parse(reader["final_degeri"].ToString());
                        }

                        if (!reader.IsDBNull(reader.GetOrdinal("but_degeri"))) // veritabanında büt notu varsa büt notu değişkenini güncelleme
                        {
                            butDegeri = float.Parse(reader["but_degeri"].ToString());
                        }

                        float ortalama = 0; //ortalama hesabı için değişken tanımlama
                        string durum = ""; //dersten geçme/kalma bilgisi içeren sütun için değişken tanımlama

                        if (vizeDegeri != 101 && finalDegeri != 101 && butDegeri != 101) //vize, final ve büt notlarının hepsi varsa
                        {
                            //vizenin %30u ve bütün %70iyle ortalama hesaplama
                            ortalama = (vizeDegeri * 0.3f) + (butDegeri * 0.7f);
                            vizeDegeri2 = vizeDegeri.ToString(); //listView'a işlemek için bütün notların string değere başka bir isimle kaydedilmesi
                            finalDegeri2 = finalDegeri.ToString();
                            butDegeri2 = butDegeri.ToString();
                        }
                        else if (vizeDegeri != 101 && finalDegeri != 101 && butDegeri == 101) //vize ve final notu varsa büt yoksa
                        {
                            
                            ortalama = (vizeDegeri * 0.3f) + (finalDegeri * 0.7f);
                            vizeDegeri2 = vizeDegeri.ToString();
                            finalDegeri2 = finalDegeri.ToString();
                            butDegeri2 = " "; //boş olan not default olarak "101" olduğu için " " atayıp listViewda doğru görüntüleme sağlanması 

                        }
                        else if (vizeDegeri != 101 && finalDegeri == 101 && butDegeri != 101) //vize ve büt notu varsa final yoksa 
                        {
                            
                            ortalama = (vizeDegeri * 0.3f) + (butDegeri * 0.7f);
                            vizeDegeri2 = vizeDegeri.ToString();
                            finalDegeri2 = " ";
                            butDegeri2 = butDegeri.ToString();
                        }
                        else if (vizeDegeri == 101 && finalDegeri != 0 && butDegeri != 101) //final ve büt notu varsa vize yoksa
                        {
                            
                            ortalama = butDegeri * 0.7f;
                            vizeDegeri2 = vizeDegeri.ToString();
                            finalDegeri2 = finalDegeri.ToString();
                            butDegeri2 = butDegeri.ToString();
                        }
                        else if (vizeDegeri != 101 && finalDegeri == 101 && butDegeri == 101) //sadece vize notu varsa
                        {
                            
                            ortalama = vizeDegeri;
                            vizeDegeri2 = vizeDegeri.ToString();
                            finalDegeri2 = " ";
                            butDegeri2 = " ";
                        }
                        else if (vizeDegeri == 101 && finalDegeri != 101 && butDegeri == 101) //sadece final notu varsa
                        {
                            
                            ortalama = finalDegeri;
                            vizeDegeri2 = " ";
                            finalDegeri2 = finalDegeri.ToString();
                            butDegeri2 = " ";
                        }
                        else if (vizeDegeri == 101 && finalDegeri == 101 && butDegeri != 101) //sadece büt notu varsa
                        {
                            
                            ortalama = butDegeri;
                            vizeDegeri2 = " ";
                            finalDegeri2 = " ";
                            butDegeri2 = butDegeri.ToString();
                        }

                        //ortalamaya göre harf notu belirleme
                        if (ortalama >= 90)
                        {
                            harfnotu = "AA";
                        }
                        else if (ortalama >= 85)
                        {
                            harfnotu = "BA";
                        }
                        else if (ortalama >= 75)
                        {
                            harfnotu = "BB";
                        }
                        else if (ortalama >= 70)
                        {
                            harfnotu = "CB";
                        }
                        else if (ortalama >= 60)
                        {
                            harfnotu = "CC";
                        }
                        else if (ortalama >= 55)
                        {
                            harfnotu = "DC";
                        }
                        else if (ortalama >= 50)
                        {
                            harfnotu = "DD";
                        }
                        else
                        {
                            harfnotu = "FF";
                        }


                        //ortalamaya göre dersten geçilip geçilmediğinin belirlenmesi
                        if (ortalama >= 60)
                        {
                            durum = "GEÇTİ";
                        }
                        else if (ortalama >= 50)
                        {
                            durum = "ŞARTLI";
                        }
                        else
                        {
                            durum = "KALDI";
                        }

                        // ListView gibi bir kontrolde gösterme işlemleri burada yapılabilir
                        ListViewItem item = new ListViewItem(dersAdi);
                        item.SubItems.Add(vizeDegeri2);
                        item.SubItems.Add(finalDegeri2);
                        item.SubItems.Add(butDegeri2);
                        item.SubItems.Add(harfnotu);
                        item.SubItems.Add(durum);

                        listView1.Items.Add(item);
                    }

                }
            }
        }
    }
}
