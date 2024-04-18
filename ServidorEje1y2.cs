using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;

class Program
{
    const int PUERTO = 12345;

    static void Main(string[] args)
    {
        var serverSocket = new TcpListener(IPAddress.Any, PUERTO);
        serverSocket.Start();
        Console.WriteLine("Servidor iniciado. Esperando conexiones...");

        while (true)
        {
            using (var clienteSocket = serverSocket.AcceptTcpClient())
            {
                Console.WriteLine("Cliente conectado desde " + clienteSocket.Client.RemoteEndPoint);
                using (var writer = new StreamWriter(clienteSocket.GetStream(), Encoding.UTF8))
                {
                    writer.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                    writer.Flush();
                }
            }
        }
    }
}