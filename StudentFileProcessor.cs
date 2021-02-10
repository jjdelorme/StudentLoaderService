using System;
using System.Text;
using System.IO;
using System.Configuration;
using Cymbal.Data;

namespace Cymbal
{
    public class StudentFileProcessor
    {
        private SchoolContext _schoolContext;

        public StudentFileProcessor()
        {
            _schoolContext = new SchoolContext();
        }

        public int ProcessCsvFile(string path)
        {
            int records = 0;
            FileStream fs = null;
            try
            {
                if (!FileOpener.TryOpen(path, ref fs))
                    return 0;
                
                using (StreamReader sr = new StreamReader(fs, Encoding.UTF8))
                {
                    while (!sr.EndOfStream)
                    {
                        string line = sr.ReadLine();
                        ProcessLine(line);
                        records++;
                    }
                }

                return records;
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
            string firstName = values[0];
            string lastName = values[1];
            DateTime enrollmentDate = DateTime.Parse(values[2]);

            if (enrollmentDate > DateTime.Now)
                throw new ApplicationException("Enrollment.");

            Save(firstName, lastName, enrollmentDate);
        }

        private void Save(string firstName, string lastName, DateTime enrollmentDate)
        {
            Student student = new Student();
            student.LastName = lastName;
            student.FirstMidName = firstName;
            student.EnrollmentDate = enrollmentDate;

            _schoolContext.Students.Add(student);
        }
    }
}
