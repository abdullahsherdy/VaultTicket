using QRCoder;

namespace API.Helpers
{
    public class QRCodeHelper
    {

        public static string GenerateQrCodeBase64(string content)
        {
            using var qrGenerator = new QRCodeGenerator();
            using var qrCodeData = qrGenerator.CreateQrCode(content, QRCodeGenerator.ECCLevel.Q);
            using var qrCode = new PngByteQRCode(qrCodeData);
            var qrBytes = qrCode.GetGraphic(20); // 20 = pixel size per module

            return $"data:image/png;base64,{Convert.ToBase64String(qrBytes)}";
        }
    }
}
