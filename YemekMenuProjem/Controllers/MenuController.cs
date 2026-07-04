using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using YemekMenuProjem.Models;

namespace YemekMenuProjem.Controllers
{
    public class MenuController : Controller
    {
        private readonly ApplicationDbContext dbcontext;
        public MenuController(ApplicationDbContext dbcontext)
        {
            this.dbcontext = dbcontext;
        }

        public IActionResult Index()
        {
            var result = dbcontext.Menuler.ToList();


            return View(result);
        }
        public IActionResult AdminMenu()
        {
            var result = dbcontext.Menuler.ToList();
            return View(result);
        }
        [HttpGet]
        public IActionResult Create()
        {
            ViewData["Id"] = new SelectList(dbcontext.Kategoriler, "Id", "KategoriAdi");
            return View();
        }
        [HttpPost]
        public IActionResult Create(Menu menu)
        {

            dbcontext.Menuler.Add(menu);
            dbcontext.SaveChanges();
            return RedirectToAction("AdminMenu");

        }

        public IActionResult Edit(int id)
        {
            var result = dbcontext.Menuler.Find(id);

            ViewBag.KategoriId = new SelectList(dbcontext.Kategoriler, "Id", "KategoriAdi", result.KategoriId);

            return View(result);
        }
        [HttpPost]
        public IActionResult Edit(Menu menu)
        {
            dbcontext.Menuler.Update(menu);
            dbcontext.SaveChanges();

            return RedirectToAction("AdminMenu");
        }
        public IActionResult Delete(int id)
        {
            var result = dbcontext.Menuler.Find(id);

            if (result != null)
            {
                dbcontext.Menuler.Remove(result);
                dbcontext.SaveChanges();
            }

            return RedirectToAction("AdminMenu");
        }



        public IActionResult ExportToPdf()
        {
            // 1. Veri tabanından güncel listeyi çekin
            var products = dbcontext.Menuler
            .Include(x => x.Kategori)
                .ToList();

            // 2. QuestPDF ile PDF dökümanını tasarlayın
            var pdfDocument = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(2, Unit.Centimetre);
                    page.PageColor(Colors.White);
                    page.DefaultTextStyle(x => x.FontSize(11).FontFamily("Arial"));

                    // Üst Bilgi (Header)
                    page.Header()
                        .Text("Yemek Listesi Raporu")
                        .SemiBold().FontSize(20).FontColor(Colors.Blue.Medium);

                    // İçerik (Tablo Oluşturma)
                    page.Content()
                        .PaddingTop(1, Unit.Centimetre)
                        .Table(table =>
                        {
                            // Sütun genişliklerini tanımlayın
                            table.ColumnsDefinition(columns =>
                            {
                                columns.ConstantColumn(50);  // ID sütunu genişliği
                                columns.RelativeColumn();    // YEMEK adı sütunu (esnek)
                                columns.ConstantColumn(100); // durum sütunu genişliği
                                columns.ConstantColumn(100); // KATEGORİ sütunu genişliği
                            });

                            // Tablo Başlıkları (Header Row)
                            table.Header(header =>
                            {
                                header.Cell().Background(Colors.Grey.Lighten2).Padding(5).Text("ID").Bold();
                                header.Cell().Background(Colors.Grey.Lighten2).Padding(5).Text("Yemek Adı").Bold();
                                header.Cell().Background(Colors.Grey.Lighten2).Padding(5).Text("Fiyat").Bold();
                                header.Cell().Background(Colors.Grey.Lighten2).Padding(5).Text("Kategori").Bold();
                            });

                            // Veri Satırları (Döngü ile verileri basıyoruz)
                            foreach (var item in products)
                            {
                                table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten3).Padding(5).Text(item.Id.ToString());
                                table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten3).Padding(5).Text(item.YemekAdi);
                                table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten3).Padding(5).Text(item.Fiyat.ToString());
                                table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten3).Padding(5).Text(item.Kategori.KategoriAdi);
                            }
                        });

                    // Alt Bilgi (Footer)
                    page.Footer()
                        .AlignCenter()
                        .Text(x =>
                        {
                            x.Span("Sayfa ");
                            x.CurrentPageNumber();
                        });
                });
            });

            // 3. PDF'i byte dizisine çevirip tarayıcıya indirtme
            var pdfBytes = pdfDocument.GeneratePdf();
            return File(pdfBytes, "application/pdf", $"Yemek_Listesi_{DateTime.Now:yyyyMMdd}.pdf");
        }
        public IActionResult ExportToExcel()
        {
            ExcelPackage.License.SetNonCommercialPersonal("Backend softito");

            // 2. Veri tabanından güncel listenizi çekin
            var products = dbcontext.Menuler
   .Include(x => x.Kategori)
   .ToList();

            // 3. Bellekte (Memory) boş bir Excel dosyası oluşturun
            using (var package = new ExcelPackage())
            {
                // Excel içinde "Yemek Listesi" adında bir sayfa aç
                var worksheet = package.Workbook.Worksheets.Add("Yemek Listesi");

                // 4. Tablo Başlıklarını Yazın (1. Satır)
                worksheet.Cells[1, 1].Value = "ID";
                worksheet.Cells[1, 2].Value = "Yemek Adı";
                worksheet.Cells[1, 3].Value = "Fiyat";
                worksheet.Cells[1, 4].Value = "Kategori";

                // 5. Başlık Satırını Şıklaştırın (Arka plan rengi, kalın yazı vb.)
                using (var range = worksheet.Cells[1, 1, 1, 3]) // 1. satır, 1'den 3. sütuna kadar seç
                {
                    range.Style.Font.Bold = true; // Yazıyı kalın yap
                    range.Style.Fill.PatternType = ExcelFillStyle.Solid;
                    range.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.FromArgb(41, 128, 185)); // Mavi arka plan
                    range.Style.Font.Color.SetColor(System.Drawing.Color.White); // Beyaz yazı rengi
                    range.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center; // Ortala
                }

                // 6. Verileri Döngü ile Excel Satırlarına Basın
                int rowNumber = 2; // Veriler 2. satırdan başlayacak
                foreach (var item in products)
                {
                    worksheet.Cells[rowNumber, 1].Value = item.Id;
                    worksheet.Cells[rowNumber, 2].Value = item.YemekAdi;
                    worksheet.Cells[rowNumber, 3].Value = item.Fiyat;
                    worksheet.Cells[rowNumber, 4].Value = item.Kategori.KategoriAdi;



                    rowNumber++;
                }



                //7.Sütun genişliklerini içeriğe göre otomatik ayarla

                worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();

                // 8. Excel dosyasını byte dizisine çevirip tarayıcıya fırlat
                var fileBytes = package.GetAsByteArray();
                string fileName = $"Yemek_Listesi_{DateTime.Now:yyyyMMdd}.xlsx";

                return File(fileBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);







            }
        }

    }
}
