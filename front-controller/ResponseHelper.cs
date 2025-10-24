using System.Net;
using System.IO;
using System.Threading.Tasks;

namespace FrontController
{
    public static class ResponseHelper
    {
        // Codificação de caracteres
        public static async Task WriteTextAsync(HttpListenerResponse response, string text, string contentType = "text/plain")
        {
            response.ContentType = $"{contentType}; charset=utf-8";
            byte[] buffer = System.Text.Encoding.UTF8.GetBytes(text);
            await response.OutputStream.WriteAsync(buffer);
            response.Close();
        }

        // Arquivo para download
        public static async Task WriteFileAsync(HttpListenerResponse response, string filePath, string contentType, string downloadFilename)
        {
            FileInfo fileInfo = new FileInfo(filePath);

            // MIME Type
            response.ContentType = contentType;

            // Attachment 
            response.Headers.Add("Content-Disposition", $"attachment; filename=\"{downloadFilename}\"");

            response.ContentLength64 = fileInfo.Length;

            try
            {
                // Abre o arquivo para leitura
                using (FileStream fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read, 4096, useAsync: true))
                {
                    // Copia o conteúdo para o stream de saída
                    await fileStream.CopyToAsync(response.OutputStream);
                }
            }
            catch (Exception)
            {
                response.Abort();
                throw;
            }
            finally
            {
                response.Close();
            }
        }
    }
}

