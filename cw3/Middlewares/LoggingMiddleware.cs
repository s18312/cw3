using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using cw3.DAL;

namespace cw3.Middlewares
{
    public class LoggingMiddleware
    {
        private readonly RequestDelegate _next;

        public LoggingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext httpContext)
        {
            httpContext.Request.EnableBuffering();

            if (httpContext.Request != null)
            {
                string sciezka = httpContext.Request.Path;
                string querystring = httpContext.Request?.QueryString.ToString();
                string metoda = httpContext.Request.Method.ToString();
                string bodyStr = "";

                bodyStr += "Sciezka: " + sciezka + " QueryString: " + querystring + " Metoda: " + metoda + " ";
                

                using (StreamReader reader = new StreamReader(httpContext.Request.Body, Encoding.UTF8, true, 1024, true))
                {
                    bodyStr += " Cialo: " +   await reader.ReadToEndAsync();
                }

                //logowanie do pliku
                var logPath = @Environment.CurrentDirectory + @"\logRequest.txt";
                using (System.IO.StreamWriter file = new System.IO.StreamWriter(logPath, true))

                {
                    file.WriteLine(bodyStr);
                }
            }

            await _next(httpContext);
        }


    }
}
