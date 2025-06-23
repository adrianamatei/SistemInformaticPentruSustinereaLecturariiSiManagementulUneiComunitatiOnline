using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System.IO;

namespace AplicatieLicenta.Services
{
    public class DiplomaGenerator
    {
        public static void GenereazaDiploma(string nume, int loc, string path)
        {
            var bgImagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "diploma.jpeg");
            var backgroundImage = File.ReadAllBytes(bgImagePath);

            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(0); 
                    page.PageColor(Colors.White);
                    page.DefaultTextStyle(x => x.FontSize(18).FontFamily("Times New Roman"));

                    page.Background().Image(backgroundImage, ImageScaling.FitArea);

                    page.Content().Padding(4, Unit.Centimetre).Column(col =>
                    {
                        col.Item().AlignCenter().Text("Diplomă de Excelență")
                            .FontSize(38).Bold().FontColor(Colors.Black);

                        col.Item().PaddingVertical(25).AlignCenter().Text("Se acordă cu apreciere cititorului:")
                            .FontSize(20).FontColor(Colors.Black);

                        col.Item().AlignCenter().Text(nume)
                            .FontSize(30).Bold().Italic().FontColor(Colors.Black);

                        col.Item().PaddingVertical(20).AlignCenter().Text($"pentru obținerea locului {loc} în clasamentul general.")
                            .FontSize(20).FontColor(Colors.Black);

                        col.Item().PaddingTop(30).AlignCenter().Text($"Data acordării: {DateTime.Now:dd MMMM yyyy}")
                            .FontSize(16).Italic().FontColor(Colors.Black);

                      
                    });
                });
            });

            document.GeneratePdf(path);
        }
    }
}
