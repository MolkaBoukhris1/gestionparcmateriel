using ClosedXML.Excel;
using GestionParcMateriel.API.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GestionParcMateriel.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ExportController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ExportController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("materiels")]
        public async Task<IActionResult> ExportMateriels()
        {
            var materiels = await _context.Materiels
                .Include(m => m.TypeMateriel)
                .Include(m => m.Etat)
                .Include(m => m.Localisation)
                .Include(m => m.Employe)
                .Where(m => m.Actif)
                .ToListAsync();

            using var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("Materiels");

            // En-têtes
            worksheet.Cell(1, 1).Value = "Code-barres";
            worksheet.Cell(1, 2).Value = "Libellé";
            worksheet.Cell(1, 3).Value = "Type";
            worksheet.Cell(1, 4).Value = "Marque";
            worksheet.Cell(1, 5).Value = "Modèle";
            worksheet.Cell(1, 6).Value = "N° Série";
            worksheet.Cell(1, 7).Value = "État";
            worksheet.Cell(1, 8).Value = "Localisation";
            worksheet.Cell(1, 9).Value = "Employé affecté";
            worksheet.Row(1).Style.Font.Bold = true;

            // Données
            int row = 2;
            foreach (var m in materiels)
            {
                worksheet.Cell(row, 1).Value = m.CodeBarre;
                worksheet.Cell(row, 2).Value = m.Libelle;
                worksheet.Cell(row, 3).Value = m.TypeMateriel?.Libelle ?? "";
                worksheet.Cell(row, 4).Value = m.Marque ?? "";
                worksheet.Cell(row, 5).Value = m.Modele ?? "";
                worksheet.Cell(row, 6).Value = m.NumeroSerie ?? "";
                worksheet.Cell(row, 7).Value = m.Etat?.Libelle ?? "";
                worksheet.Cell(row, 8).Value = m.Localisation?.Nom ?? "";
                worksheet.Cell(row, 9).Value = m.Employe != null 
                    ? $"{m.Employe.Nom} {m.Employe.Prenom}" 
                    : "";
                row++;
            }

            worksheet.Columns().AdjustToContents();

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            stream.Position = 0;

            return File(
                stream.ToArray(),
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                $"Materiel_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx"
            );
        }
    }
}