using System.IO;
using Dman.Utilities.Logger;
using UnityEngine;

namespace Dman.SimpleJson
{
    public class FileSystemPersistence : IPersistText
    {
        private readonly string _directoryPath;

        private FileSystemPersistence(string absolutePath)
        {
            _directoryPath = absolutePath;
        }

        public static IPersistText CreateAtAbsoluteFolderPath(string path)
        {
            return new FileSystemPersistence(path);
        }
        
        public TextWriter WriteTo(string contextKey)
        {
            string filePath = EnsureSaveFilePath(contextKey);
            Log.Info($"Saving to {filePath}");
            return new StreamWriter(filePath, append: false);
        }

        public TextReader ReadFrom(string contextKey)
        {
            var filePath = EnsureSaveFilePath(contextKey);
            if (!File.Exists(filePath))
            {
                Log.Warning($"No file found at {filePath}");
                return null;
            }
            Log.Info($"Reading from {filePath}");
            return new StreamReader(filePath);
        }

        public void Delete(string contextKey)
        {
            var filePath = EnsureSaveFilePath(contextKey);
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
        }

        
        public void DeleteAll()
        {
            if (Directory.Exists(_directoryPath))
            {
                var files = Directory.EnumerateFiles(_directoryPath, "*.json", SearchOption.TopDirectoryOnly);
                foreach (var file in files)
                {
                    File.Delete(file);
                }
            }
        }

        private string EnsureSaveFilePath(string contextKey)
        {
            var fileName = $"{contextKey}.json";
            if (!Directory.Exists(_directoryPath))
            {
                Directory.CreateDirectory(_directoryPath);
            }
            
            var saveFile = Path.Combine(_directoryPath, fileName);
            return saveFile;
        }
    }
}