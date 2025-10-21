using System.Net;
using System.Threading.Tasks;

namespace FrontController
{
    public interface IController
    {
        Task HandleAsync(HttpListenerContext context);
    }

    public class NotFoundController : IController
    {
        public async Task HandleAsync(HttpListenerContext context)
        {
            context.Response.StatusCode = 404;
            await ResponseHelper.WriteTextAsync(context.Response, "404 - Página não encontrada...");
        }    
    }

    public class HealthController : IController
    {
        public async Task HandleAsync(HttpListenerContext context)
        {
            await ResponseHelper.WriteTextAsync(context.Response, "/Health OK");
        }
    }

    public class DownloadController : IController
    {
        public async Task HandleAsync(HttpListenerContext context)
        {
            const string FilePath = "caminho//do//arquivo//arquivo.zip";
            const string FileName = "caminho//do//arquivo//arquivo.zip";

            if (!System.IO.File.Exists(FilePath))
            {
                context.Response.StatusCode = 404;
                await ResponseHelper.WriteTextAsync(context.Response, "Arquivo não encontrado no servidor.", "text/plain");
                return;
            }

            try
            {
                await ResponseHelper.WriteFileAsync(
                    context.Response,
                    FilePath,
                    "application/zip",
                    FileName
                );
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro no download: {ex.Message}");

                await ResponseHelper.WriteTextAsync(
                    context.Response,
                    "Erro ao processar o arquivo para download." + ex.Message
                );
            }
        }
    }

}
