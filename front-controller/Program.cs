using System;
using System.Net;
using System.Text.Unicode;
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
                var context = await _listener.GetContextAsync();
                await new FrontController().ProcessRequestAsync(context);
                
            }
        }
    }

    public static class ResponseHelper
    {
        public static async Task WriteTextAsync(HttpListenerResponse response, string text, string contentType = "text/plain")
        {
            response.ContentType = $"{contentType}; charset=utf-8";
            byte[] buffer = System.Text.Encoding.UTF8.GetBytes(text);
            await response.OutputStream.WriteAsync(buffer);
            response.Close();
        }
    }

    public class FrontController
    {
        public async Task ProcessRequestAsync(HttpListenerContext context)
        {
            string path = context.Request.Url!.AbsolutePath.ToLower();

            // Lógica comum a todas as rotas
            Console.WriteLine($"[LOG] Requisição para {path}");

            // Dispatcher
            IController controller = path switch
            {
                "/users" => new UserController(),
                "/products" => new ProductController(),
                _ => new NotFoundController()
            };

            await controller.HandleAsync(context);
        }
    }

    public interface IController
    {
        Task HandleAsync(HttpListenerContext context);
    }

    public class UserController : IController
    {
        public async Task HandleAsync(HttpListenerContext context)
        {
            await ResponseHelper.WriteTextAsync(context.Response, "Listando usuários...");
        }
    }

    public class ProductController : IController
    {
        public async Task HandleAsync(HttpListenerContext context)
        {
            await ResponseHelper.WriteTextAsync(context.Response, "Listando produtos...");
        }
    }

    public class NotFoundController : IController
    {
        public async Task HandleAsync(HttpListenerContext context)
        {
            context.Response.StatusCode = 404;
            await ResponseHelper.WriteTextAsync(context.Response, "404 - Página não encontrada...");
        }
    }
}