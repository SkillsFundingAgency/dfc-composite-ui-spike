using System;
using System.IO;

namespace TestWebApplication
{
    public class FileLog : ILog
    {

        public void Log(string data)
        {
            try
            {
                var file = @"D:\Clients\Esfa\Code\Ncs\Ncs.Prototype\Branches\Dev\Option2\TestWebApplication\AppLog.txt";
                DoLog(file, data);
            }
            catch (Exception)
            {
                var file = @"D:\home\site\wwwroot\AppLog.txt";
                DoLog(file, data);
            }
        }

        private void DoLog(string file, string data)
        {
            var fullData = $"{DateTime.Now} {data} {Environment.NewLine}";
            File.AppendAllText(file, fullData);
        }
    }
}
