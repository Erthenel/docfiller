using DocFiller.Models;
using System.IO;

namespace DocFiller.Utils
{
    class BinaryConverter
    {
        public static void WriteToBinaryFile<T>(string filePath, T objectToWrite)
        {
            using (Stream stream = File.Open(filePath, FileMode.Create))
            {
                var binaryFormatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
                binaryFormatter.Serialize(stream, objectToWrite);
            }
        }

        public static object ReadFromBinaryFile(Stream stream)
        {
            var binaryFormatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
            return binaryFormatter.Deserialize(stream);
        }
    }
}
