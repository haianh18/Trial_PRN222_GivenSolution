using ServerAppLib;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;


public class Program
{
    private static void Main(string[] args)
    {
        ServerAppLib.ServerApp.MainProcess();
    }


}

//public class Star
//{
//    public string Name { get; set; }
//    public DateTime? Dob { get; set; }
//    public string? Description { get; set; }
//    public bool? Male { get; set; }
//    public string? Nationality { get; set; }
//}

//class Program
//{
//    static async Task Main(string[] args)
//    {
//        var listener = new TcpListener(IPAddress.Parse("127.0.0.1"), 5000);
//        listener.Start();
//        Console.WriteLine("Listening for connections on 127.0.0.1:5000");

//        while (true)
//        {
//            var client = await listener.AcceptTcpClientAsync();
//            _ = Task.Run(async () =>
//            {
//                using (var stream = client.GetStream())
//                {
//                    var buffer = new byte[1024];
//                    var bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length);
//                    var request = Encoding.UTF8.GetString(buffer, 0, bytesRead);
//                    var stars = JsonSerializer.Deserialize<List<Star>>(request);

//                    if (stars != null)
//                    {
//                        // Process the list of stars (e.g., save to database, log, etc.)
//                        Console.WriteLine("Received stars:");
//                        foreach (var star in stars)
//                        {
//                            Console.WriteLine($"{star.Name} {star.Dob?.ToString("d")} {star.Description} {star.Male} {star.Nationality}");
//                        }

//                        var responseString = "accepted";
//                        var responseData = Encoding.UTF8.GetBytes(responseString);
//                        await stream.WriteAsync(responseData, 0, responseData.Length);
//                    }
//                    else
//                    {
//                        var responseString = "error";
//                        var responseData = Encoding.UTF8.GetBytes(responseString);
//                        await stream.WriteAsync(responseData, 0, responseData.Length);
//                    }
//                }
//            });
//        }
//    }
//}