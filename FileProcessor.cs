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

        public void ProcessCsvFile(string file)
        {
            FileStream fs = File.Open(file, FileMode.Open, FileAccess.Read, FileShare.None);
            using (StreamReader sr = new StreamReader(fs, Encoding.UTF8))
            {
                string line = sr.ReadLine();
                ProcessLine(line);
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
