using System.Threading.Tasks;
using System.Net.Http;
using System.Text;
using System;

namespace dctool
{
    internal class Program
    {
        private static readonly HttpClient client = new HttpClient();

        static void Banner()
        {
            Console.WriteLine("""
                                              _           _      
                                             | |         | |     
                __      _____  _ __ _ __ ___ | |__   ___ | | ___ 
                \ \ /\ / / _ \| '__| '_ ` _ \| '_ \ / _ \| |/ _ \
                 \ V  V / (_) | |  | | | | | | | | | (_) | |  __/
                  \_/\_/ \___/|_|  |_| |_| |_|_| |_|\___/|_|\___|
                """);
        }

        public static async Task SendMessage(string url, string message, int amount)
        {
            for (int i = 0; i < amount; i++)
            {
                try
                {
                    var json = $"{{\"content\": \"{message}\"}}";
                    var content = new StringContent(json, Encoding.UTF8, "application/json");

                    HttpResponseMessage response = await client.PostAsync(url, content);

                    if (response.IsSuccessStatusCode)
                    {
                        Console.WriteLine($"Message {i + 1} sent successfully.");
                    }
                    else
                    {
                        Console.WriteLine($"Error sending message {i + 1}. Status code: {response.StatusCode}");
                    }
                }
                catch (HttpRequestException e)
                {
                    Console.WriteLine($"Request error on message {i + 1}: {e.Message}");
                }
            }
        }

        static async Task Main(string[] args)
        {
            Console.Title = "w0rmh0l3";
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.DarkGreen;
            Banner();
            Console.WriteLine();

            Console.WriteLine("[I] Webhook info grabber");
            Console.WriteLine("[W] Webhook spammer");
            Console.WriteLine("[Q] Quit\n");

            Console.Write("option >> ");
            string choice = Console.ReadLine();

            if (choice?.ToLower() == "w")
            {
                Console.Write("webhook url >> ");
                string url = Console.ReadLine();

                int amount;
                Console.Write("amount >> ");
                while (!int.TryParse(Console.ReadLine(), out amount))
                {
                    Console.Write("invalid number, try again >> ");
                }

                Console.Write("message >> ");
                string message = Console.ReadLine();

                await SendMessage(url, message, amount);
            }

            if (choice?.ToLower() == "i")
            {
                Console.Write("webhook url >> ");
                string url = Console.ReadLine();
                Console.Clear();

                try
                {
                    HttpResponseMessage response = await client.GetAsync(url);
                    if (response.IsSuccessStatusCode)
                    {
                        string json = await response.Content.ReadAsStringAsync();
                        var parsed = System.Text.Json.JsonDocument.Parse(json);
                        var pretty = System.Text.Json.JsonSerializer.Serialize(parsed, new System.Text.Json.JsonSerializerOptions { WriteIndented = true });
                        Console.WriteLine("\nWebhook info:\n" + pretty);
                    }
                    else
                    {
                        Console.WriteLine($"Failed to fetch webhook info. Status code: {response.StatusCode}");
                    }
                }
                catch (HttpRequestException e)
                {
                    Console.WriteLine($"Request error: {e.Message}");
                }
            }

            if (choice?.ToLower() == "q")
            {
                Environment.Exit(0);
            }

            Console.WriteLine("\nPress anything to return..");
            Console.ReadKey();
            await Main(args);
        }
    }
}
