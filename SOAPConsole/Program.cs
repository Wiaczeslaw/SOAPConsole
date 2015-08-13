using System;
using System.Data;
using System.IO;
using System.Net;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Xml;
using Microsoft.Office.Interop.Excel;
using SOAPConsole.WebReference;

namespace SOAPConsole
{
    internal class Program
    {
        //для генерации WebReference
        //private const string url = "http://62.192.35.162:8088/test1c/ws/stationlightport.1cws?wsdl";
        private const string Login = "takebus";
        private const string Password = "takebus";

        private static void Main(string[] args)
        {
            Console.WriteLine("Test connection with SOAP Service...");

            var client = new stationlightport
            {
                Credentials = new NetworkCredential(Login, Password),
                PreAuthenticate = true
            };

            try
            {
                Console.WriteLine("Try get data from service... \n");
                var responseData = client.GetBusStops();
                Console.WriteLine("Success! \n");

                Console.WriteLine("Try save data as excel file... \n");
                SaveXmlToXlsFile(XmlReader.Create(new StringReader(responseData)));
                Console.WriteLine("Success! \n File xmlToExcel.xls saved in your documents directory\n");
            }
            catch (Exception e)
            {
                Console.WriteLine("Error! Please read exception message: \n\n {0} \n", e.Message);
            }
        }

        private static void SaveXmlToXlsFile(XmlReader reader)
        {
            object misValue = Missing.Value;
            var dataSet = new DataSet();

            var excelApplication = new Application();
            var excelWorkBook = excelApplication.Workbooks.Add(misValue);
            var excelWorkSheet = (Worksheet)excelWorkBook.Worksheets.Item[1];

            dataSet.ReadXml(reader);

            for (var i = 0; i <= dataSet.Tables[0].Rows.Count - 1; i++)
            {
                int j;
                for (j = 0; j <= dataSet.Tables[0].Columns.Count - 1; j++)
                {
                    excelWorkSheet.Cells[i + 1, j + 1] = dataSet.Tables[0].Rows[i].ItemArray[j].ToString();
                }
            }

            excelWorkBook.SaveAs("xmlToExcel.xls", XlFileFormat.xlWorkbookNormal, misValue, misValue, misValue, misValue, XlSaveAsAccessMode.xlExclusive, misValue, misValue, misValue, misValue, misValue);
            excelWorkBook.Close(true, misValue, misValue);
            excelApplication.Quit();

            ReleaseObject(excelApplication);
            ReleaseObject(excelWorkBook);
            ReleaseObject(excelWorkSheet);
        }

        private static void ReleaseObject(object obj)
        {
            try
            {
                Marshal.ReleaseComObject(obj);
            }
            finally
            {
                GC.Collect();
            }
        } 
    }
}
