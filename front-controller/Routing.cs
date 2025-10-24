using System;
using System.Net;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace FrontController
{
    public class FrontController
    {
        public async Task ProcessRequestAsync(HttpListenerContext context)
        {
            string path = context.Request.Url!.AbsolutePath;
            Logger.Info($"[LOG] requisição para: {path} | método: {context.Request.HttpMethod}");

            IController controller = Router.GetController(path);

            await controller.HandleAsync(context);
        }
    }


    public static class Router
    {
        private static readonly Dictionary<string, Func<IController>> Routes = new(StringComparer.OrdinalIgnoreCase)
        {
            {"/health", () => new HealthController() },
            {"/download", () => new DownloadController() }
        };

        public static IController GetController(string path)
        {
            string normalizedPath = path.ToLower();

            if (Routes.TryGetValue(normalizedPath, out var controllerFactory))
            {
                return controllerFactory();
            }

            return new NotFoundController();
        }
    }
}