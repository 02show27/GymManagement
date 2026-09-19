using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace GymManagement.Infrastructure.Services;

public interface IReportePdfService
{
    byte[] GenerarReporteFinancieroPdf(decimal ingresosTotales, int totalSocios, int totalReservas);
}

public class ReportePdfService : IReportePdfService
{
    public ReportePdfService()
    {
        QuestPDF.Settings.License = LicenseType.Community;
    }

    public byte[] GenerarReporteFinancieroPdf(decimal ingresosTotales, int totalSocios, int totalReservas)
    {
        var document = Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(2, Unit.Centimetre);
                page.PageColor(Colors.White);
                page.DefaultTextStyle(x => x.FontSize(11));

                page.Header()
                    .Row(row =>
                    {
                        row.RelativeItem().Column(col =>
                        {
                            col.Item().Text("GymManagement - Sistema Integral")
                               .FontSize(20)
                               .Bold()
                               .FontColor(Colors.Blue.Darken2);
                               
                            col.Item().Text("Reporte Financiero y Ejecutivo de Operaciones")
                               .FontSize(12)
                               .Italic();
                        });
                        row.ConstantItem(120).AlignRight().Text($"Fecha: {DateTime.Now:dd/MM/yyyy}");
                    });

                page.Content()
                    .PaddingVertical(1, Unit.Centimetre)
                    .Column(col =>
                    {
                        col.Item().Text("Resumen General de Ingresos y Operaciones")
                           .FontSize(14)
                           .Bold()
                           .Underline();

                        col.Item().PaddingBottom(10);

                        col.Item().Table(table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                                columns.RelativeColumn(2);
                                columns.RelativeColumn(1);
                            });

                            table.Header(header =>
                            {
                                header.Cell().Background(Colors.Grey.Lighten2).Padding(5).Text("Métrica Operativa").Bold();
                                header.Cell().Background(Colors.Grey.Lighten2).Padding(5).AlignRight().Text("Valor").Bold();
                            });

                            table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten1).Padding(5).Text("Ingresos Totales del Mes");
                            table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten1).Padding(5).AlignRight().Text($"{ingresosTotales:0.00} Bs");

                            table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten1).Padding(5).Text("Socios Activos Registrados");
                            table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten1).Padding(5).AlignRight().Text($"{totalSocios}");

                            table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten1).Padding(5).Text("Reservas Confirmadas Procesadas");
                            table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten1).Padding(5).AlignRight().Text($"{totalReservas}");
                        });

                        col.Item().PaddingTop(20)
                           .Text("Nota: Este reporte es un comprobante interno generado automáticamente por el sistema.")
                           .FontSize(9)
                           .Italic()
                           .FontColor(Colors.Grey.Darken1);
                    });

                page.Footer()
                    .AlignCenter()
                    .Text(x =>
                    {
                        x.Span("Página ");
                        x.CurrentPageNumber();
                    });
            });
        });

        return document.GeneratePdf();
    }
}