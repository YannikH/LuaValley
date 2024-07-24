using StardewValley.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LuaValley.Helpers
{
    internal class FileLogger : IGameLogger
    {
        string fileName = "log.txt";
        public FileLogger(string fileName) {
            this.fileName = fileName;
            using (StreamWriter outputFile = new StreamWriter(fileName))
            {
                outputFile.WriteLine("Start of log file " + fileName);
            }
        }

        private void writeLine(string content)
        {
            using (StreamWriter outputFile = new StreamWriter(fileName, true))
            {
                outputFile.WriteLine(content);
            }
        }
        public void Verbose(string msg) { writeLine(msg); }
        public void Debug(string msg) { writeLine(msg); }
        public void Info(string msg) { writeLine(msg); }
        public void Warn(string msg) { writeLine(msg); }
        public void Error(string msg, Exception e) { writeLine(msg); }
    }
}
