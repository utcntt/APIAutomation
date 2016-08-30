using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//using OfficeOpenXml;
using TestLibrary.Logging.LogModel;
using System.IO;

namespace NUnitReportToExcel
{
    /// <summary>
    /// Print a test result to an excel file 
    /// </summary>
    public class ExcelHelper
    {
        //public static void GenerateExcelFile(TestResult result, string filePath)
        //{
        //    try
        //    {
        //        FileInfo newFile = new FileInfo(filePath);
        //        if (newFile.Exists)
        //        {
        //            newFile.Delete();  // ensures we create a new workbook
        //            newFile = new FileInfo(filePath);
        //        }

        //        using (ExcelPackage package = new ExcelPackage(newFile))
        //        {
        //            //Add the Content sheet
        //            var ws = package.Workbook.Worksheets.Add("Content");
        //            ws.View.ShowGridLines = true;
        //            ws.Column(1).Width = 40;

        //            //Print the header
        //            PrintTestResultHeader(ws, result);

        //            //Print the detail
        //            PrintTestCaseList(ws, result.TestSuite, 10);

        //            // set some document properties
        //            package.Workbook.Properties.Title = "Public API Test Report";
        //            package.Workbook.Properties.Author = "Truong Pham";
        //            package.Workbook.Properties.Comments = "This sample demonstrates how to create an Excel 2007 workbook using EPPlus";

        //            // set some extended property values
        //            package.Workbook.Properties.Company = "Porters";

        //            // save our new workbook and we are done!
        //            package.Save();
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine("There is an error when writing the Excel file.");
        //        Console.WriteLine("Message: {0}", ex.Message);
        //        Console.WriteLine("Stack trace:  {0}", ex.StackTrace);
        //        Console.WriteLine("Source:  {0}", ex.Source);
        //    }
        //}

        //internal static void PrintTestResultHeader(ExcelWorksheet ws, TestResult result)
        //{
            
        //    ws.Cells["A1"].Value = "Assembly";
        //    ws.Cells["B1"].Value = result.Name;
        //    ws.Cells["A2"].Value = "Time";
        //    ws.Cells["B2"].Value = result.Date + " " + result.Time;
        //    ws.Cells["A3"].Value = "Machine name";
        //    ws.Cells["B3"].Value = result.Environment.MachineName;
        //    ws.Cells["A4"].Value = "OS version";
        //    ws.Cells["B4"].Value = result.Environment.OsVersion;
        //    ws.Cells["A5"].Value = "NUnit version";
        //    ws.Cells["B5"].Value = result.Environment.NunitVersion;

        //    ws.Cells["A6"].Value = "Total";
        //    ws.Cells["A6"].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
        //    ws.Cells["A7"].Value = result.Total;
        //    ws.Cells["A7"].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
        //    ws.Cells["B6"].Value = "Errors";
        //    ws.Cells["B6"].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
        //    ws.Cells["B7"].Value = result.Errors;
        //    ws.Cells["B7"].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
        //    ws.Cells["C6"].Value = "Failures";
        //    ws.Cells["C6"].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
        //    ws.Cells["C7"].Value = result.Failures;
        //    ws.Cells["C7"].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
        //    ws.Cells["D6"].Value = "Not run";
        //    ws.Cells["D6"].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
        //    ws.Cells["D7"].Value = result.NotRun;
        //    ws.Cells["D7"].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
        //    ws.Cells["E6"].Value = "Inconclusive";
        //    ws.Cells["E6"].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
        //    ws.Cells["E7"].Value = result.Inconclusive;
        //    ws.Cells["E7"].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
        //    ws.Cells["F6"].Value = "Ignored";
        //    ws.Cells["F6"].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
        //    ws.Cells["F7"].Value = result.Ignored;
        //    ws.Cells["F7"].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
        //    ws.Cells["G6"].Value = "Skipped";
        //    ws.Cells["G6"].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
        //    ws.Cells["G7"].Value = result.Skipped;
        //    ws.Cells["G7"].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);

        //    ws.Cells["A9"].Value = "Test case";
        //    ws.Cells["A9"].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
        //    //ws.Cells["B9"].Value = "Executed";
        //    //ws.Cells["B9"].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
        //    ws.Cells["B9"].Value = "Description";
        //    ws.Cells["B9"].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
        //    ws.Cells["C9"].Value = "Result";
        //    ws.Cells["C9"].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
        //    //ws.Cells["D9"].Value = "Success";
        //    //ws.Cells["D9"].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
        //    ws.Cells["D9"].Value = "Message";
        //    ws.Cells["D9"].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
        //    ws.Cells["E9"].Value = "Stack trace";
        //    ws.Cells["E9"].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
        //}

        //internal static int PrintTestCaseList(ExcelWorksheet ws, TestSuite result, int startRow)
        //{
        //    int startRowIndex = startRow;
        //    if (result.TestCases != null && result.TestCases.Count > 0)
        //    {
        //        foreach (TestCase testCase in result.TestCases)
        //        {
        //            PrintTestCase(ws, testCase, startRowIndex);
        //            startRowIndex++;
        //        }
        //    }
        //    if (result.TestSuites != null && result.TestSuites.Count > 0)
        //    {
        //        foreach (TestSuite testSuite in result.TestSuites)
        //        {
        //            startRowIndex = PrintTestCaseList(ws, testSuite, startRowIndex);
        //        }
        //    }
        //    return startRowIndex;
        //}

        //internal static void PrintTestCase(ExcelWorksheet ws, TestCase result, int startRow)
        //{
        //    ws.Cells["A" + startRow.ToString()].Value = result.Name;
        //    ws.Cells["A" + startRow.ToString()].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
        //    ws.Cells["B" + startRow.ToString()].Value = 
        //        (result.ParentTestSuite != null? result.ParentTestSuite.Description + ". " : string.Empty)  + result.Description;
        //    ws.Cells["B" + startRow.ToString()].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
        //    ws.Cells["C" + startRow.ToString()].Value = result.Result;
        //    ws.Cells["C" + startRow.ToString()].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
        //    //ws.Cells["D" + startRow.ToString()].Value = result.Success;
        //    //ws.Cells["D" + startRow.ToString()].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
        //    ws.Cells["D" + startRow.ToString()].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
        //    ws.Cells["E" + startRow.ToString()].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
        //    if (result.Failure != null)
        //    {
        //        ws.Cells["D" + startRow.ToString()].Value = result.Failure.Message;
        //        ws.Cells["E" + startRow.ToString()].Value = result.Failure.StackTrace;
        //    }
        // }
    }
}
