using System;
using System.IO;
using System.ServiceProcess;
using System.Configuration;
using System.Diagnostics;

namespace Cymbal
{
    public class FileProcessorService : ServiceBase
    {
        private FileSystemWatcher _fileWatcher;
        private FileProcessor _processor;

        public FileProcessorService()
        {
            this.ServiceName = "Cymbal.FileProcessorService";
            this.AutoLog = true;

            _processor = new FileProcessor();
        }

        protected override void OnStart(string[] args)
        {
            try
            {
                string watchPath = GetWatchPath();

                _fileWatcher = new FileSystemWatcher(watchPath, "*.csv");
                _fileWatcher.Created += OnFileCreated;
                _fileWatcher.EnableRaisingEvents = true;

                EventLog.WriteEntry("OnStart:: Watching directory " + watchPath, 
                    EventLogEntryType.Information);
            }
            catch (Exception e)
            {
                EventLog.WriteEntry("OnStart:: Error starting service " + e.Message,
                    EventLogEntryType.Error);

                this.Stop();
            }
        }

        private void OnFileCreated(object sender, FileSystemEventArgs e)
        {
            string filename = e.FullPath;
            try
            {
                _processor.ProcessCsvFile(filename);
                
                EventLog.WriteEntry("OnFileCreated:: Processed " + filename,
                    EventLogEntryType.Information);
            }
            catch (Exception ex)
            {
                EventLog.WriteEntry("OnFileCreated:: Error processing " + filename + " " + 
                    ex.Message, EventLogEntryType.Warning);
            }
        }

        protected override void OnStop()
        {
            if (_fileWatcher != null)
            {
                _fileWatcher.EnableRaisingEvents = false;
                _fileWatcher.Dispose();
            }
        }

        private string GetWatchPath()
        {
            string watchPath = ConfigurationManager.AppSettings["WatchPath"];
            
            if (string.IsNullOrEmpty(watchPath))
            {
                throw new ApplicationException(
                    "AppSettings missing or does not contain a WatchPath value.");
            }

            if (!Directory.Exists(watchPath))
            {
                throw new ApplicationException(
                    "Configured WatchPath directory does not exist: " + watchPath);
            }
            
            return watchPath;
        }
    }
}
