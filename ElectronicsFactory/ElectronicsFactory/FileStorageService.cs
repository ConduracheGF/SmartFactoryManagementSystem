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
            catch (Exception e)
            {
                Logger.Error($"File storage error while saving {fileName}: {e.Message}");
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
