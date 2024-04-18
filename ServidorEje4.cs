using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
// Asegúrate de agregar la referencia necesaria para usar Microsoft.Office.Interop.Word
using Microsoft.Office.Interop.Word;

class Program
{
    private const int Port = 12345;

    static void Main()
    {
        var listener = new TcpListener(IPAddress.Any, Port);
        listener.Start();

        Console.WriteLine("Servidor iniciado...");

        while (true)
        {
            using (var client = listener.AcceptTcpClient())
            using (var stream = client.GetStream())
            using (var ms = new MemoryStream())
            {
                // Recibir archivo del cliente
                stream.CopyTo(ms);
                var wordData = ms.ToArray();
                var wordPath = Path.GetTempFileName();
                File.WriteAllBytes(wordPath, wordData);

                // Convertir a PDF
                var pdfPath = Path.ChangeExtension(wordPath, ".pdf");
                ConvertToPdf(wordPath, pdfPath);

                // Enviar archivo PDF al cliente
                var pdfData = File.ReadAllBytes(pdfPath);
                stream.Write(pdfData, 0, pdfData.Length);

                // Limpiar archivos temporales
                File.Delete(wordPath);
                File.Delete(pdfPath);
            }
        }
    }

    static void ConvertToPdf(string inputPath, string outputPath)
    {
        var app = new Application();
        var doc = app.Documents.Open(inputPath);
        doc.ExportAsFixedFormat(outputPath, WdExportFormat.wdExportFormatPDF);
        doc.Close();
        app.Quit();
    }
}
