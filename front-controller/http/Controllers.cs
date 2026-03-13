using System.Net;
using System.Text;
using System.Text.Json;
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

    public class MessageController : IController 
    {
        private readonly MessageRepository _repo = new MessageRepository();

        public async Task HandleAsync(HttpListenerContext context) 
        {
            if (context.Request.HttpMethod != "POST")
            {
                context.Response.StatusCode = 405;
                await ResponseHelper.WriteTextAsync(context.Response, "ONLY POST.");
                return;
            }

            using var reader = new StreamReader(context.Request.InputStream, context.Request.ContentEncoding);
            string body = await reader.ReadToEndAsync();

            var payload = JsonSerializer.Deserialize<MessageRequest>(body);

            if (payload == null || string.IsNullOrWhiteSpace(payload.Data))
            {
                context.Response.StatusCode = 400;
                await ResponseHelper.WriteTextAsync(context.Response, "Campo 'data' obrigatório.");
                return;
            }

            string decoded;

            try 
            {
                decoded = Encoding.UTF8.GetString(Convert.FromBase64String(payload.Data));
            }
            catch
            {
                context.Response.StatusCode = 400;
                await ResponseHelper.WriteTextAsync(context.Response, "Base64 inválido.");
                return;
            }

            _repo.SaveMessage(decoded);

            await ResponseHelper.WriteTextAsync(context.Response, "Mensagem salva.");
        }

        private class MessageRequest 
        {
            public string Data { get; set; } = "";
        }
    }

    public class MessageListController : IController
    {
        private readonly MessageRepository _repo = new MessageRepository();

        public async Task HandleAsync(HttpListenerContext context) 
        {
            if (context.Request.HttpMethod != "GET")
            {
                context.Response.StatusCode = 405;
                await ResponseHelper.WriteTextAsync(context.Response, "ONLY GET.");
                return;
            }

            var messages = _repo.GetAllMessages();

            // converter cada mensagem para base64
            var base64List = messages.Select(msg =>
                Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(msg))
            ).ToList();

            var json = JsonSerializer.Serialize(base64List);

            await ResponseHelper.WriteTextAsync(context.Response, json, "application/json");
        }
    }

    public class DownloadController : IController
    {
        public async Task HandleAsync(HttpListenerContext context)
        {
            const string FilePath = "caminho//do//arquivo//arquivo.zip";
            const string FileName = "file.zip";

            if (!System.IO.File.Exists(FilePath))
            {
                context.Response.StatusCode = 404;
                await ResponseHelper.WriteTextAsync(context.Response, "Arquivo nao encontrado no servidor.", "text/plain");
                Logger.Error("Nenhum arquivo de download encontrado");
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
                Logger.Success("Arquivo baixado com sucesso");
            }
            catch (Exception ex)
            {
                Logger.Error($"Erro no download: {ex.Message}");

                await ResponseHelper.WriteTextAsync(context.Response, "Erro ao processar o arquivo para download." + ex.Message);
            }
        }
    }
}
