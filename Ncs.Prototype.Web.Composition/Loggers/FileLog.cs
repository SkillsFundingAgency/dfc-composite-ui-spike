using Microsoft.Extensions.Hosting;
using System;
using System.IO;

namespace Ncs.Prototype.Web.Composition.Loggers
{
    public class FileLog : ILog
    {
        private readonly IHostingEnvironment _hostingEnvironment;

        public FileLog(IHostingEnvironment hostingEnvironment)
        {
            _hostingEnvironment = hostingEnvironment;
        }

        public void Log(string data)
        {
            var fullData = $"{DateTime.Now} {data} {Environment.NewLine}";
            File.AppendAllText($"{_hostingEnvironment.ContentRootPath}\\applog.txt", fullData);
        }

        private void DoLog(string file, string data)
        {
            var fullData = $"{DateTime.Now} {data} {Environment.NewLine}";
            File.AppendAllText(file, fullData);
        }
    }
}
