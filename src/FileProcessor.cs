using System;
using System.Text;
using System.IO;
using System.Configuration;

namespace Cymbal
{
    public class FileProcessor
    {
        private string _connectionString;
        
        public FileProcessor()
        {
            _connectionString = GetConnectionString();
        }

        public bool ProcessCsvFile(string path)
        {
            FileStream fs = null;
            try
            {
                if (!FileOpener.TryOpen(path, ref fs))
                    return false;
                
                using (StreamReader sr = new StreamReader(fs, Encoding.UTF8))
                {
                    while (!sr.EndOfStream)
                    {
                        string line = sr.ReadLine();
                        ProcessLine(line);
                    }
                }

                return true;
            }
            finally
            {
                if (fs != null)
                {
                    fs.Dispose();
                }
            }
        }

        private void ProcessLine(string line)
        {
            string[] values = line.Split(',');
            string name = values[0];
            string color = values[1];
            int age = int.Parse(values[2]);

            if (age < 18)
                throw new ApplicationException("All participants must be 18 or older.");

            Save(name, color, age);
        }

        private void Save(string name, string color, int age)
        {
            //
        }

        private string GetConnectionString()
        {
            return ConfigurationManager.AppSettings["DbConnectionString"];
        }
    }
}
