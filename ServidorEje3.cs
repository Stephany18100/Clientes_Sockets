
// Servidor
using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;

class Program
{
    const int PUERTO = 12345;
    static string directorioImagenes = @"C:\Users\steph\OneDrive\Documents\Practica 6 Unidad 2\Images"; // Asegúrate de cambiar esto por el directorio real

    static void Main(string[] args)
    {
        var serverSocket = new TcpListener(IPAddress.Any, PUERTO);
        serverSocket.Start();
        Console.WriteLine("Servidor iniciado. Esperando conexiones...");

        while (true)
        {
            using (var clienteSocket = serverSocket.AcceptTcpClient())
            using (var stream = clienteSocket.GetStream())
            using (var writer = new StreamWriter(stream, Encoding.UTF8))
            using (var reader = new StreamReader(stream, Encoding.UTF8))
            {
                // Enviar lista de nombres de imágenes
                var archivos = Directory.GetFiles(directorioImagenes, "*.jpeg");
                var nombresImagenes = string.Join(",", Array.ConvertAll(archivos, Path.GetFileName));
                writer.WriteLine(nombresImagenes);
                writer.Flush();

                // Recibir selección del cliente
                var seleccionCliente = reader.ReadLine();
                var rutaImagenSeleccionada = Path.Combine(directorioImagenes, seleccionCliente);

                // Verificar si la imagen existe
                if (File.Exists(rutaImagenSeleccionada))
                {
                    // Enviar imagen
                    var bytesImagen = File.ReadAllBytes(rutaImagenSeleccionada);
                    writer.WriteLine("OK");
                    writer.WriteLine(bytesImagen.Length);
                    writer.Flush();
                    stream.Write(bytesImagen, 0, bytesImagen.Length);
                }
                else
                {
                    // Enviar mensaje de error
                    writer.WriteLine("ERROR");
                    writer.Flush();
                }
            }
        }
    }
}