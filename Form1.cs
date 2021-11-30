using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LINQSorgular
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        NorthwindEntities db;


        private void Form1_Load(object sender, EventArgs e)
        {
            db = new NorthwindEntities();
        }



        private void btn1_Click(object sender, EventArgs e) 
        {
            //1 ocak 1998 sonrası sipariş veren müşterilerimin isimlerini artan sırada sıralayınız(CompanyName, OrderID,OrderDate)

            dgvSonuc.DataSource = db.Orders.Where(x => x.OrderDate > new DateTime(1998,01,01)).OrderBy(x => x.Customer.CompanyName).Select(x => new
            {
                MusteriAdi=x.Customer.CompanyName,
                SiparisKodu=x.OrderID,
                SiparisTarihi=x.OrderDate
            }).ToList();
        }


        private void btn2_Click(object sender, EventArgs e)
        {
            //Şişede satılan ürünlerimi sipariş ile gönderdiğim ülkeler hangileridir?(productName, ShipCountry, kolonları)

            dgvSonuc.DataSource = db.Order_Details.Where(x => x.Product.QuantityPerUnit.Contains("bottle")).Select(x => new
            {
                SisedeSatilanUrunler=x.Product.QuantityPerUnit,
                UrunAdi=x.Product.ProductName,
                SiparisIleGonderilenUrunlerinUlkeleri=x.Order.ShipCountry
            }).ToList();
        }


        private void btn3_Click(object sender, EventArgs e)
        {
            //Kadın çalışanlarımın ilgilendiği siparişlerimin,gönderildiği müşterilerimden iletişime geçtiğim kişilerin isimleri ve şehirleri nelerdir?(Employee(FirstName LastName), CompanyName, ContactName, City kolonları)


            dgvSonuc.DataSource = db.Orders.Where(x => x.Employee.TitleOfCourtesy.Contains("Mrs.")|| x.Employee.TitleOfCourtesy.Contains("Ms.")).Select(x => new
            {
                Siparis=x.OrderID,
                Calisan=x.Employee.FirstName +" "+x.Employee.LastName,
                CalisanCinsi=x.Employee.TitleOfCourtesy,
                MüsteriAdi=x.Customer.CompanyName,
                IletisimKurulanKisi=x.Customer.ContactName,  //Distinct() ekleyince hata veriyor ama eklemezsek aynı satırı tekrar ekliyor?
                IletisimKurulanKisininSehri=x.Customer.City
            }).ToList();
        }


        private void btn4_Click(object sender, EventArgs e)
        {
            //Federal Shipping ile taşınmış ve Nancy'in almış olduğu siparişler listeleyiniz.(OrderID, Employee(FirstName LastName), Shipper(CompanyName) kolonları


            dgvSonuc.DataSource = db.Orders.Where(x => x.Shipper.CompanyName==("Federal Shipping") && x.Employee.FirstName==("Nancy")).Select(x => new
            {
                SiparisKodu=x.OrderID,
                Calisan = x.Employee.FirstName + " " + x.Employee.LastName,
                TasimaSirketi=x.Shipper.CompanyName
            }).ToList();
        }


        private void btn5_Click(object sender, EventArgs e)
        {
            //stoğu 20'den fazla olan siparişlerimin hangi kargo şirketleriyle teslim edildiğini listeleyiniz.(Shipper(CompanyName), OrderID, UnitsInStock kolonları)


            dgvSonuc.DataSource = db.Order_Details.Where(x => x.Product.UnitsInStock > 20 && x.Order.ShippedDate<=DateTime.Now).Select(x => new
            {
                SiparisinTeslimEdildigiKargoSirketi=x.Order.Shipper.CompanyName,
                SiparisKodu = x.OrderID,
                StokMiktari=x.Product.UnitsInStock
            }).ToList();
        }


        private void btn6_Click(object sender, EventArgs e)
        {
            //250'den fazla sipariş taşımış olan kargo firmalarını listeleyiniz. (Shipper(CompanyName), Shipper(Phone), Sipariş Sayısı kolonları

            //SELECT S.CompanyName, S.Phone, Count(O.OrderID) AS SiparisSayısı
            //From Orders O JOIN Shippers S ON S.ShipperID=O.ShipVia
            //Group By S.CompanyName,S.Phone
            //Having Count(Object.OrderID)>250

            dgvSonuc.DataSource = db.Orders.GroupBy(x=>new { x.Shipper.CompanyName, x.Shipper.Phone }).Select(x => new
            {
                FirmaAdi = x.Key.CompanyName,
                TelefonNumarasi = x.Key.Phone,
                ToplamSiparis =x.Count()
                
            }).Where(x=>x.ToplamSiparis>250).ToList();
        }
    }
}
