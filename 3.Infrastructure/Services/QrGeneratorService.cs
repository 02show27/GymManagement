using QRCoder;

namespace GymManagement.Infrastructure.Services;

public interface IQrGeneratorService
{
    string GenerarQrBase64(string contenido);
}

public class QrGeneratorService : IQrGeneratorService
{
    public string GenerarQrBase64(string contenido)
    {
        using QRCodeGenerator qrGenerator = new QRCodeGenerator();
        using QRCodeData qrCodeData = qrGenerator.CreateQrCode(contenido, QRCodeGenerator.ECCLevel.Q);
        using BitmapByteQRCode qrCode = new BitmapByteQRCode(qrCodeData);
        byte[] qrCodeImage = qrCode.GetGraphic(20);
        return $"data:image/png;base64,{Convert.ToBase64String(qrCodeImage)}";
    }
}