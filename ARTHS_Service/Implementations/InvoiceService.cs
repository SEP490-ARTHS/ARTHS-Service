using ARTHS_Data;
using ARTHS_Data.Repositories.Interfaces;
using ARTHS_Service.Interfaces;
using AutoMapper;
using PdfSharp.Drawing.Layout;
using PdfSharp.Drawing;
using PdfSharp.Pdf;
using System.Globalization;
using Microsoft.EntityFrameworkCore;
using ARTHS_Data.Entities;

namespace ARTHS_Service.Implementations
{
    public class InvoiceService : BaseService, IInvoiceService
    {
        private readonly IOrderRepository _orderRepository;
        public InvoiceService(IUnitOfWork unitOfWork, IMapper mapper) : base(unitOfWork, mapper)
        {
            _orderRepository = unitOfWork.Order;
        }

        public async Task<string> GenerateInvoice(string orderId, string folderPath)
        {
            try
            {
                var order = await _orderRepository.GetMany(order => order.Id.Equals(orderId))
                    .Include(order => order.OrderDetails)
                        .ThenInclude(detail => detail.MotobikeProduct)
                    .Include(order => order.OrderDetails)
                        .ThenInclude(detail => detail.RepairService)
                    .Include(order => order.Teller)
                    .FirstOrDefaultAsync();

                if (order == null)
                {
                    return "Lỗi: Đơn hàng không tồn tại.";
                }
                if (!Directory.Exists(folderPath))
                {
                    Directory.CreateDirectory(folderPath);
                }

                PdfDocument document = new PdfDocument();
                PdfPage page = document.AddPage();
                XGraphics gfx = XGraphics.FromPdfPage(page);

                // 1. Draw Header
                DrawHeader(gfx, page);

                // 2. Draw Shop and Customer Info
                DrawShopAndCustomerInfo(gfx, page, order);

                double startY = 180;
                // 3. Draw Product Table
                if (order.OrderDetails.Any(detail => detail.MotobikeProductId.HasValue))
                {
                    startY = DrawProductTable(gfx, page, order, startY);
                }
                if (order.OrderDetails.Any(detail => detail.RepairServiceId.HasValue))
                {
                    startY = DrawServiceTable(gfx, page, order, startY);
                }

                // 4. Draw Service Table

                // 5. Draw Total Amount
                startY = DrawTotalAmount(gfx, page, order, startY);

                // 6. Draw Warranty
                startY = DrawWarrantyTable(gfx, page, order, startY);

                // 7. Signature
                DrawSignatureSection(gfx, page);



                string filePath = SaveDocument(document, orderId, folderPath);
                return "Thành công, file được lưu tại: " + filePath;
            }
            catch (Exception ex)
            {
                return "Lỗi: " + ex.Message;
            }
        }



        private void DrawHeader(XGraphics gfx, PdfPage page)
        {
            // Fonts
            XFont headerFont = new XFont("Verdana", 20, XFontStyle.Bold, new XPdfFontOptions(PdfFontEncoding.Unicode));
            XFont detailsFont = new XFont("Verdana", 10, XFontStyle.Regular, new XPdfFontOptions(PdfFontEncoding.Unicode));
            XFont contentFont = new XFont("Verdana", 10, XFontStyle.Regular, new XPdfFontOptions(PdfFontEncoding.Unicode));

            // Formats
            XStringFormat centerFormat = new XStringFormat { Alignment = XStringAlignment.Center, LineAlignment = XLineAlignment.Center };
            XStringFormat rightAlignFormat = new XStringFormat { Alignment = XStringAlignment.Far, LineAlignment = XLineAlignment.Center };

            // Heading
            double marginTop = 20;
            double marginRight = 20;

            // Tiêu đề "Thanh Huy Motobike Shop" căn phải
            gfx.DrawString("Thanh Huy Motobike Shop", detailsFont, XBrushes.Black,
                new XRect(0, marginTop, page.Width - marginRight, 40), rightAlignFormat);

            marginTop += 20; // Điều chỉnh khoảng cách từ hàng trước

            // Lấy ngày và giờ hiện tại
            DateTime now = DateTime.Now;

            // Định dạng ngày và giờ theo dạng "HCM, Ngày dd, tháng MM, năm yyyy
            string formattedDate = $"HCM, Ngày {now.Day}, tháng {now.Month}, năm {now.Year}";
            // Vẽ chuỗi ngày và giờ lên PDF, căn phải
            gfx.DrawString(formattedDate, detailsFont, XBrushes.Black,
                new XRect(0, marginTop, page.Width - marginRight, 40), rightAlignFormat);


            // Position "Hóa đơn sửa chữa - bán hàng" in the center
            double titleMarginTop = 70; // Adjust this value based on your layout preferences
            gfx.DrawString("Hóa đơn sửa chữa - bán hàng", headerFont, XBrushes.Black, new XRect(0, titleMarginTop, page.Width, 40), centerFormat);
        }

        private void DrawShopAndCustomerInfo(XGraphics gfx, PdfPage page, Order order)
        {
            // Fonts
            XFont headerFont = new XFont("Verdana", 20, XFontStyle.Bold, new XPdfFontOptions(PdfFontEncoding.Unicode));
            XFont detailsFont = new XFont("Verdana", 10, XFontStyle.Regular, new XPdfFontOptions(PdfFontEncoding.Unicode));
            XFont contentFont = new XFont("Verdana", 10, XFontStyle.Regular, new XPdfFontOptions(PdfFontEncoding.Unicode));

            // Formats
            XStringFormat centerFormat = new XStringFormat { Alignment = XStringAlignment.Center, LineAlignment = XLineAlignment.Center };
            XStringFormat rightAlignFormat = new XStringFormat { Alignment = XStringAlignment.Far, LineAlignment = XLineAlignment.Center };

            // Shop address
            gfx.DrawString("Địa chỉ: D4, khu Công Nghệ Cao, phường Long Thạnh Mỹ, Quận 9, TP.Hồ Chí Minh", detailsFont, XBrushes.Black, new XPoint(20, 130));


            // Info
            double midX = page.Width / 2;
            double textWidth = gfx.MeasureString($"Số điện thoại: {order.CustomerPhoneNumber}", detailsFont).Width;
            double adjustedX = midX - (textWidth / 2);

            gfx.DrawString($"Khách hàng: {order.CustomerName}", detailsFont, XBrushes.Black, new XPoint(20, 150));
            gfx.DrawString($"Số điện thoại: {order.CustomerPhoneNumber}", detailsFont, XBrushes.Black, new XPoint(adjustedX, 150));

            gfx.DrawString($"Nhân viên tạo đơn: {order.Teller!.FullName}", detailsFont, XBrushes.Black, new XPoint(20, 165));
            gfx.DrawString($"Mã hóa đơn: {order.Id}", detailsFont, XBrushes.Black, new XPoint(adjustedX, 165));
        }

        private double DrawProductTable(XGraphics gfx, PdfPage page, Order order, double startY)
        {
            // Fonts
            XFont headerFont = new XFont("Verdana", 10, XFontStyle.Bold, new XPdfFontOptions(PdfFontEncoding.Unicode));
            XFont detailsFont = new XFont("Verdana", 10, XFontStyle.Regular, new XPdfFontOptions(PdfFontEncoding.Unicode));
            XFont contentFont = new XFont("Verdana", 10, XFontStyle.Regular, new XPdfFontOptions(PdfFontEncoding.Unicode));

            // Formats
            XStringFormat centerFormat = new XStringFormat { Alignment = XStringAlignment.Center, LineAlignment = XLineAlignment.Center };
            XStringFormat rightAlignFormat = new XStringFormat { Alignment = XStringAlignment.Far, LineAlignment = XLineAlignment.Center };

            //double startY = 180;
            // Drawing the table
            double columnMargin = 20;
            double tableWidth = page.Width - (2 * columnMargin);
            double[] columnWidths = {
            0.05 * tableWidth,  // STT
            0.4 * tableWidth,   // Product Name
            0.1 * tableWidth,   // Quantity
            0.2 * tableWidth,   // Price
            0.25 * tableWidth   // Total
        };

            // Table Headers
            string[] headers = { "STT", "Tên thiết bị", "SL", "Đơn giá", "Thành tiền" };
            XStringFormat[] headerFormats = { centerFormat, centerFormat, centerFormat, centerFormat, centerFormat };

            double currentX = columnMargin;
            for (int i = 0; i < headers.Length; i++)
            {
                gfx.DrawRectangle(XPens.Black, currentX, startY, columnWidths[i], 25);  // Drawing cell borders
                gfx.DrawString(headers[i], headerFont, XBrushes.Black, new XRect(currentX, startY, columnWidths[i], 25), headerFormats[i]);
                currentX += columnWidths[i];
            }

            startY += 25;

            // Order details
            var detailsList = order.OrderDetails.Where(detail => detail.MotobikeProductId.HasValue).ToList();
            XTextFormatter tf = new XTextFormatter(gfx);
            double rowHeight = 30;
            double totalAmount = 0;

            for (int i = 0; i < detailsList.Count; i++)
            {
                var detail = detailsList[i];
                double rowStartY = startY;
                currentX = columnMargin;

                gfx.DrawString((i + 1).ToString(), contentFont, XBrushes.Black, new XRect(currentX, rowStartY, columnWidths[0], rowHeight), centerFormat);
                currentX += columnWidths[0];

                double cellPadding = 5; // adjust as needed

                // Handle text wrapping
                XRect productNameRect = new XRect(currentX + cellPadding, rowStartY + cellPadding, columnWidths[1] - (2 * cellPadding), rowHeight - (2 * cellPadding));
                string productName = detail.MotobikeProduct?.Name ?? "";

                List<string> wrappedLines = WrapText(productName, columnWidths[1] - (2 * cellPadding), contentFont, gfx);
                double currentLineY = rowStartY + cellPadding;
                foreach (var line in wrappedLines)
                {
                    tf.DrawString(line, contentFont, XBrushes.Black, new XRect(currentX + cellPadding, currentLineY, columnWidths[1] - (2 * cellPadding), rowHeight), XStringFormats.TopLeft);
                    currentLineY += gfx.MeasureString(line, contentFont).Height;
                }

                currentX += columnWidths[1];

                gfx.DrawString(detail.Quantity.ToString(), contentFont, XBrushes.Black, new XRect(currentX, rowStartY, columnWidths[2], rowHeight), centerFormat);
                currentX += columnWidths[2];

                string formattedPrice = string.Format("{0:N0}", detail.Price);
                gfx.DrawString(formattedPrice, contentFont, XBrushes.Black, new XRect(currentX, rowStartY, columnWidths[3], rowHeight), centerFormat);
                currentX += columnWidths[3];

                double? total = (double?)(detail.Price * detail.Quantity);
                totalAmount += total.GetValueOrDefault(); // Cộng dồn tổng tiền
                string formattedTotal = string.Format("{0:N0}", total);
                gfx.DrawString(formattedTotal, contentFont, XBrushes.Black, new XRect(currentX, rowStartY, columnWidths[4], rowHeight), centerFormat);

                // Drawing cell borders
                currentX = columnMargin;
                for (int j = 0; j < columnWidths.Length; j++)
                {
                    gfx.DrawRectangle(XPens.Black, currentX, rowStartY, columnWidths[j], rowHeight);
                    currentX += columnWidths[j];
                }

                startY += rowHeight;
            }

            return startY;
        }

        private double DrawServiceTable(XGraphics gfx, PdfPage page, Order order, double startY)
        {
            startY += 20;

            // Fonts
            XFont headerFont = new XFont("Verdana", 10, XFontStyle.Bold, new XPdfFontOptions(PdfFontEncoding.Unicode));
            XFont detailsFont = new XFont("Verdana", 10, XFontStyle.Regular, new XPdfFontOptions(PdfFontEncoding.Unicode));
            XFont contentFont = new XFont("Verdana", 10, XFontStyle.Regular, new XPdfFontOptions(PdfFontEncoding.Unicode));

            // Formats
            XStringFormat centerFormat = new XStringFormat { Alignment = XStringAlignment.Center, LineAlignment = XLineAlignment.Center };
            XStringFormat rightAlignFormat = new XStringFormat { Alignment = XStringAlignment.Far, LineAlignment = XLineAlignment.Center };

            double columnMargin = 20;
            double tableWidth = page.Width - (2 * columnMargin);
            double[] serviceColumnWidths = {
            0.1 * tableWidth,  // STT
            0.6 * tableWidth,  // Service Name
            0.3 * tableWidth   // Price
        };
            double currentX = columnMargin;
            string[] serviceHeaders = { "STT", "Tên Dịch vụ", "Giá" };
            for (int i = 0; i < serviceHeaders.Length; i++)
            {
                gfx.DrawRectangle(XPens.Black, currentX, startY, serviceColumnWidths[i], 25);  // Drawing cell borders
                gfx.DrawString(serviceHeaders[i], headerFont, XBrushes.Black, new XRect(currentX, startY, serviceColumnWidths[i], 25), centerFormat);
                currentX += serviceColumnWidths[i];
            }

            var detailsList = order.OrderDetails.Where(detail => detail.RepairServiceId.HasValue).ToList();
            // Move startY for the first service row
            startY += 25;
            double rowHeight = 30;
            XTextFormatter tf = new XTextFormatter(gfx);
            // Draw the services list
            int serviceIndex = 1;  // Initialize service index (STT)
            for (int i = 0; i < detailsList.Count; i++)  // Assuming detailsList contains the services
            {
                var service = detailsList[i];

                // Skip if RepairService is null
                if (service.RepairService == null) continue;

                double rowStartY = startY;
                currentX = columnMargin;

                // Drawing the index (STT)
                gfx.DrawString(serviceIndex.ToString(), contentFont, XBrushes.Black, new XRect(currentX, rowStartY, serviceColumnWidths[0], rowHeight), centerFormat);
                currentX += serviceColumnWidths[0];

                // Drawing the service name
                gfx.DrawString(service.RepairService.Name, contentFont, XBrushes.Black, new XRect(currentX, rowStartY, serviceColumnWidths[1], rowHeight), centerFormat);
                currentX += serviceColumnWidths[1];

                // Drawing the service price
                string formattedPrice = string.Format("{0:N0}", service.Price);
                gfx.DrawString(formattedPrice, contentFont, XBrushes.Black, new XRect(currentX, rowStartY, serviceColumnWidths[2], rowHeight), centerFormat);

                // Drawing cell borders
                currentX = columnMargin;
                for (int j = 0; j < serviceColumnWidths.Length; j++)
                {
                    gfx.DrawRectangle(XPens.Black, currentX, rowStartY, serviceColumnWidths[j], rowHeight);
                    currentX += serviceColumnWidths[j];
                }

                // Move startY for the next service row
                startY += rowHeight;

                // Increment the service index (STT)
                serviceIndex++;
            }

            return startY;

        }

        private double DrawTotalAmount(XGraphics gfx, PdfPage page, Order order, double startY)
        {
            // Fonts
            XFont boldFont = new XFont("Verdana", 12, XFontStyle.Bold, new XPdfFontOptions(PdfFontEncoding.Unicode));

            // Formats
            XStringFormat rightAlignFormat = new XStringFormat { Alignment = XStringAlignment.Far, LineAlignment = XLineAlignment.Center };

            // Calculate the total amount
            int totalAmount = order.TotalAmount;

            // Convert totalAmount to string with VNĐ format
            CultureInfo vietnameseCulture = new CultureInfo("vi-VN");
            string totalAmountStr = string.Format(vietnameseCulture, "{0:C}", totalAmount);

            // Calculate the position for the total amount
            double startX = page.Width - 20; // Subtracting 20 for margin

            // Draw the total amount
            gfx.DrawString("Tổng tiền: " + totalAmountStr, boldFont, XBrushes.Black, new XRect(40, startY, startX - 40, 30), rightAlignFormat);

            return startY + 35;  // Return updated Y coordinate, incremented by the height of the text plus some padding
        }

        private void DrawSignatureSection(XGraphics gfx, PdfPage page)
        {
            // Fonts
            XFont signatureFont = new XFont("Verdana", 10, XFontStyle.Regular, new XPdfFontOptions(PdfFontEncoding.Unicode));

            // Formats
            XStringFormat centerAlignFormat = new XStringFormat { Alignment = XStringAlignment.Center, LineAlignment = XLineAlignment.Center };

            double pageWidth = page.Width;
            double startY = page.Height - 100; // Adjust the starting Y-coordinate as per your needs

            double sectionWidth = pageWidth / 2 - 60;  // Dividing the page width into two sections with some margin

            // Draw line for store signature
            gfx.DrawLine(XPens.Black, 40, startY, 40 + sectionWidth, startY);
            gfx.DrawString("Chữ ký của cửa hàng", signatureFont, XBrushes.Black, new XRect(40, startY + 10, sectionWidth, 20), centerAlignFormat);

            // Draw line for customer signature
            gfx.DrawLine(XPens.Black, pageWidth / 2 + 20, startY, pageWidth - 40, startY);
            gfx.DrawString("Chữ ký của khách hàng", signatureFont, XBrushes.Black, new XRect(pageWidth / 2 + 20, startY + 10, sectionWidth, 20), centerAlignFormat);
        }

        private double DrawWarrantyTable(XGraphics gfx, PdfPage page, Order order, double startY)
        {
            // Fonts
            XFont headerFont = new XFont("Verdana", 14, XFontStyle.Regular, new XPdfFontOptions(PdfFontEncoding.Unicode));
            XFont contentFont = new XFont("Verdana", 10, XFontStyle.Regular, new XPdfFontOptions(PdfFontEncoding.Unicode));

            // Formats
            XStringFormat leftAlignFormat = new XStringFormat { Alignment = XStringAlignment.Near, LineAlignment = XLineAlignment.Center };
            XStringFormat rightAlignFormat = new XStringFormat { Alignment = XStringAlignment.Far, LineAlignment = XLineAlignment.Center };

            // Maximum width for wrapping (using full page width now)
            double maxWidth = page.Width - 80;

            // Draw the main title
            gfx.DrawString("Thông Tin Bảo Hành", headerFont, XBrushes.Black, new XRect(40, startY, maxWidth, 30), leftAlignFormat);
            startY += 35;

            // Draw titles for product name and warranty date
            gfx.DrawString("Tên Sản Phẩm", contentFont, XBrushes.Black, new XRect(40, startY, maxWidth / 2, 20), leftAlignFormat);
            gfx.DrawString("Bảo Hành Đến", contentFont, XBrushes.Black, new XRect(maxWidth / 2 + 40, startY, maxWidth / 2, 20), rightAlignFormat);
            startY += 20;

            // Draw line under titles
            gfx.DrawLine(XPens.Black, 40, startY, 40 + maxWidth, startY);
            startY += 10;

            foreach (var detail in order.OrderDetails)
            {
                // Drawing product name aligned to the left
                string productName = detail.MotobikeProduct?.Name! ?? detail.RepairService?.Name!;
                gfx.DrawString(productName, contentFont, XBrushes.Black, new XRect(40, startY, maxWidth / 2, 20), leftAlignFormat);

                // Drawing warranty period aligned to the right
                string warrantyPeriodStr = detail.WarrantyEndDate.HasValue
    ? detail.WarrantyEndDate.Value.ToString("dd-MM-yyyy")
    : "N/A"; // or some default value or handling
                gfx.DrawString(warrantyPeriodStr, contentFont, XBrushes.Black, new XRect(maxWidth / 2 + 40, startY, maxWidth / 2, 20), rightAlignFormat);

                startY += 20;

            }

            return startY; // Return updated Y coordinate
        }

        private string SaveDocument(PdfDocument document, string orderId, string folderPath)
        {
            string fileName = $"invoice_{orderId}_{DateTime.Now:yyyyMMddHHmmss}.pdf";
            string filePath = Path.Combine(folderPath, fileName);
            document.Save(filePath);
            return filePath;
        }

        private List<string> WrapText(string text, double maxWidth, XFont font, XGraphics gfx)
        {
            List<string> wrapped = new List<string>();
            string[] words = text.Split(' ');
            string line = "";
            foreach (var word in words)
            {
                XSize size = gfx.MeasureString(line + " " + word, font);
                if (size.Width > maxWidth)
                {
                    if (!string.IsNullOrEmpty(line))
                    {
                        wrapped.Add(line);
                    }
                    line = word;
                }
                else
                {
                    if (string.IsNullOrEmpty(line))
                    {
                        line = word;
                    }
                    else
                    {
                        line += " " + word;
                    }
                }
            }
            if (!string.IsNullOrEmpty(line))
            {
                wrapped.Add(line);
            }
            return wrapped;
        }
    }
}
