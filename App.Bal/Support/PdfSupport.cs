using PdfSharp.Drawing;
using PdfSharp.Pdf.IO;
using PdfSharp.Pdf;
using App.Entity.Dto;
using App.Foundation.Extensions;

namespace App.Bal.Support
{
    public class PdfSupport
    {
        public static Stream? AddQrToPdf(IPdfParam dto)
        {
            string imageData = dto.QR.Replace("data:image/png;base64,", "");
            Stream? stream = dto.File?.OpenReadStream();
            if (stream != null)
            {
                stream.Position = 0;
                PdfDocument pdfDocument = PdfReader.Open(stream, PdfDocumentOpenMode.Modify);
                double top = dto.DY - (dto.CanvasHeight * (dto.PageNumber - 1));
                if(top == 0)
                {
                    top += 20;
                }
                if(dto.DX == 0) { dto.DX += 20; }
                PdfPage page = pdfDocument.Pages[dto.PageNumber - 1];
                double ratio = dto.CanvasWidth / page.Width;

                MemoryStream imageStream = (MemoryStream)imageData.Base64ImageToBitmap();
                imageStream.Position = 0;
                if (top != 0)
                {
                    XGraphics graphics = XGraphics.FromPdfPage(page);
                    XImage xImage = XImage.FromStream(imageStream);
                    graphics.DrawImage(xImage, dto.DX / ratio, top / ratio, dto.DW / ratio, dto.DH / ratio);
                    //graphics.DrawImage(xImage, dto.DX, top, dto.DW, dto.DH);
                }
                imageStream.Close();
                imageStream.Dispose();
                imageStream = new MemoryStream();
                pdfDocument.Save(imageStream);
                return imageStream;
            }
            return null;
        }

    }
}
