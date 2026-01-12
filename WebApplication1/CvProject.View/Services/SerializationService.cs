using System.Xml.Serialization;

namespace CvProject.View.Services
{
    public static class SerializationService
    {
        public static string SerializeToXML<T>(T obj)
        {
            var xmlSerializer = new XmlSerializer(typeof(T));

            using(var stringWriter = new StringWriter())
            {
                xmlSerializer.Serialize(stringWriter, obj);
                return stringWriter.ToString();
            }
        }
    }
}
