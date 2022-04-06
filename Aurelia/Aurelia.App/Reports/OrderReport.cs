using Aurelia.App.Models;
using iTextSharp.text;
using iTextSharp.text.pdf;

namespace Aurelia.App.Reports
{
    public class OrderReport
    {
        Document _document;
        
        MemoryStream _memoryStream = new MemoryStream();
        Order _detForOrder = new Order();
        List<Product> _productOrd = new List<Product>();
        public byte[] Report(Order detForOrd, List<Product> productOrd)
        {
            BaseFont bfTimes = BaseFont.CreateFont(BaseFont.TIMES_ROMAN, BaseFont.CP1252, false);
            Font tablefont = new Font(bfTimes, 20);
            _detForOrder = detForOrd;
            _productOrd = productOrd;
            _document = new Document();
            PdfWriter docWriter = PdfWriter.GetInstance(_document, _memoryStream);
            _document.Open();
            
            _document.SetPageSize(PageSize.A4);
            _document.SetMargins(5f, 5f, 20f, 5f);

            PdfPTable table = new PdfPTable(2);
            PdfPCell cell = new PdfPCell(new Phrase("Order details", tablefont));
            cell.Colspan = 2;
            cell.HorizontalAlignment = Element.ALIGN_CENTER;
            table.AddCell(cell);

            table.AddCell("Email");
            table.AddCell(_detForOrder.Email);

            table.AddCell("Date placed");
            table.AddCell(_detForOrder.date_placed.ToString());

            table.AddCell("Address");
            table.AddCell(_detForOrder.Address);

            table.AddCell("Payment method");
            table.AddCell(_detForOrder.PaymentMethod);

            table.AddCell("Delivery method");
            table.AddCell(_detForOrder.DeliveryMethod);

            PdfPCell cellProd = new PdfPCell(new Phrase("Products", tablefont));
            cellProd.Colspan = 2;
            cellProd.HorizontalAlignment = Element.ALIGN_CENTER;
            table.AddCell(cellProd);
            foreach (var item in _productOrd) 
            {
                table.AddCell(item.ProductName);
                table.AddCell("Quantity: "+ item.Quantity);
            }
            _document.Add(table);
            _document.Close();

            

            return _memoryStream.ToArray(); 
        }
       
    }
}
