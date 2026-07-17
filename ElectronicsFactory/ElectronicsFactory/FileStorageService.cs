using System;
using System.Collections.Generic;
using System.Text;

namespace ElectronicsFactory
{
    internal class FileStorageService
    {
        public void Write(string fileName, IEnumerable<string> text)
        {
            try
            {
                File.WriteAllLines(fileName, text);
            }
            catch (IOException e)
            {
                throw new DataPersistenceException($"Critical I/O error while writing to {fileName}.", e);
            }
            catch (UnauthorizedAccessException e)
            {
                throw new DataPersistenceException($"Access denied to write file {fileName}.", e);
            }
        }

        public IEnumerable<string> Read(string fileName)
        {
            if (!File.Exists(fileName))
            {
                return Array.Empty<string>();
            }

            try
            {
                return File.ReadAllLines(fileName);
            }
            catch (Exception e)
            {
                Logger.Error($"File storage error while loading {fileName}: {e.Message}");
                return Array.Empty<string>();
            }
        }

        public void Append(string fileName, string text)
        {
            try
            {
                File.AppendAllText(fileName, text + Environment.NewLine);
            }
            catch (Exception e)
            {
                Logger.Error($"[CRITICAL] Failed to append line to {fileName}: {e.Message}");
            }
        }
    }
}
