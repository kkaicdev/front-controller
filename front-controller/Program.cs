using System;
using System.Net;
using System.Threading.Tasks;

namespace FrontController
{
    public class Program
    {
        static async Task Main()
        {
            var server = new SimpleHttpServer();
            await server.StartAsync("http://localhost:8080/");
        }
    }

    public class SimpleHttpServer
    {
        private readonly HttpListener _listener = new HttpListener();

        public async Task StartAsync(string prefix)
        {
            _listener.Prefixes.Add(prefix);
            _listener.Start();
            Console.WriteLine($"Servidor iniciado em {prefix}");

            while (true)
            {
                HttpListenerContext context;
                try
                {
                    context = await _listener.GetContextAsync();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[ERRO DE INFRA] {ex.Message}");
                    break;
                }

                Task.Run(async () =>
                {
                    try
                    {
                        await new FrontController().ProcessRequestAsync(context);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"[ERRO DE PROCESSAMENTO] {ex.Message}");
                    }
                });
            }
        }
    }
}