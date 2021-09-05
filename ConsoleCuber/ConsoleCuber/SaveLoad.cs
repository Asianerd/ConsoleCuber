using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Xml.Serialization;

namespace ConsoleCuber
{
    class SaveLoad
    {
        public static bool Save(string fileName, object savedObj)
        {
            var xmlS = new XmlSerializer(savedObj.GetType());
            using (TextWriter streamW = new StreamWriter(fileName))
            {
                xmlS.Serialize(streamW, savedObj);
                streamW.Close();
            }
            return File.Exists(fileName);
        }

        public static object Load(string fileName, Type objectType)
        {
            var xmlS = new XmlSerializer(objectType);

            using (TextReader streamR = new StreamReader(fileName))
            {
                var finalObj = xmlS.Deserialize(streamR);
                streamR.Close();
                return finalObj;
            }
        }
    }
}
