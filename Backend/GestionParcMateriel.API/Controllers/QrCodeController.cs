using GestionParcMateriel.API.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QRCoder;

namespace GestionParcMateriel.API.Controllers
{
    [ApiController]
    [Route("api/materiel")]
    public class QrCodeController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public QrCodeController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("{id}/qrcode")]
        public async Task<IActionResult> GetQrCode(int id)
        {
            var materiel = await _context.Materiels.FindAsync(id);
            if (materiel == null)
                return NotFound();

            // Le QR code encode l'URL vers la fiche détail du matériel
            var frontendUrl = $"http://localhost:5173/materiels/{id}";

            using var qrGenerator = new QRCodeGenerator();
            using var qrCodeData = qrGenerator.CreateQrCode(frontendUrl, QRCodeGenerator.ECCLevel.Q);
            using var qrCode = new PngByteQRCode(qrCodeData);
            byte[] qrCodeBytes = qrCode.GetGraphic(20);

            return File(qrCodeBytes, "image/png");
        }
    }
}