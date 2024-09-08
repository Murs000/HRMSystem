using ClosedXML.Excel;
using HRMSystem.Business.Services.Interfaces;
using HRMSystem.DataAccess.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace HRMSystem.Business.Services.Implementations
{
    public class ExelService(IUserRepository userRepository) : IExelService
    {
        public async Task<byte[]> ExportWorkers()
        {
            var users = await userRepository.GetAllAsync();
            byte[] excelFile;

            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("Workers");

                // Add title with fancy font and border
                var title = worksheet.Cell(1, 1).SetValue("Workers Report");
                title.Style.Font.SetBold(true).Font.SetFontSize(18).Font.SetFontColor(XLColor.Black).Font.SetFontName("Arial");
                title.Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                worksheet.Range(1, 1, 1, 3).Merge().Style.Border.OutsideBorder = XLBorderStyleValues.Thick;

                // Headers
                var headers = new[] { "Name", "Surname", "Years In Company" };
                for (int i = 0; i < headers.Length; i++)
                {
                    worksheet.Cell(3, i + 1).SetValue(headers[i]);
                }

                // Style headers
                var headerRange = worksheet.Range(3, 1, 3, 3);
                headerRange.Style.Font.SetBold(true).Font.SetFontSize(12).Font.SetFontColor(XLColor.White);
                headerRange.Style.Fill.SetBackgroundColor(XLColor.BlueGray);
                headerRange.Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                headerRange.Style.Border.OutsideBorder = XLBorderStyleValues.Medium;

                // Apply auto-filter
                worksheet.Range(3, 1, users.Count() + 3, 3).SetAutoFilter();

                // Data rows
                int row = 4;
                foreach (var user in users)
                {
                    worksheet.Cell(row, 1).SetValue(user.Name);
                    worksheet.Cell(row, 2).SetValue(user.Surname);
                    worksheet.Cell(row, 3).SetValue(user.ExperienceInCompany);

                    // Row styling
                    List<IXLCell> currentCells = [worksheet.Cell(row,1), worksheet.Cell(row, 2), worksheet.Cell(row, 3)];

                    foreach (IXLCell currentCell in currentCells)
                    {
                        currentCell.Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                        currentCell.Style.Alignment.SetVertical(XLAlignmentVerticalValues.Center);
                        currentCell.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

                        // Alternating row colors for better readability
                        if (row % 2 == 0)
                        {
                            currentCell.Style.Fill.SetBackgroundColor(XLColor.LightBlue);
                        }
                        else
                        {
                            currentCell.Style.Fill.SetBackgroundColor(XLColor.WhiteSmoke);
                        }
                    }

                    row++;
                }

                // Adjust column widths to content
                worksheet.Columns(1, 3).AdjustToContents();

                // Set minimum column widths for better formatting
                worksheet.Column(1).Width = Math.Max(worksheet.Column(1).Width, 15);  // Name
                worksheet.Column(2).Width = Math.Max(worksheet.Column(2).Width, 15);  // Surname
                worksheet.Column(3).Width = Math.Max(worksheet.Column(3).Width, 25);  // Years In Company

                // Final touches: add borders around the entire data range
                var dataRange = worksheet.Range(3, 1, row - 1, 3);
                dataRange.Style.Border.OutsideBorder = XLBorderStyleValues.Medium;

                // Convert to byte array
                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    stream.Position = 0;
                    excelFile = stream.ToArray();
                }
            }

            return excelFile;
        }
    }
}
