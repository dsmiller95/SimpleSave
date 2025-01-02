using System;

namespace Dman.SimpleJson
{
    public class SaveDataException : Exception
    {
        public SaveDataException(string message) : base(message) { }
        public SaveDataException(string message, Exception innerException) : base(message, innerException) { }
    }
}