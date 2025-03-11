using Newtonsoft.Json;
using System.IO;

namespace RestoreMonarchy.UconomyBalanceUI.Storages
{
    public class DataStorage<T> where T : class
    {
        public string DataPath { get; private set; }
        public DataStorage(string path)
        {
            DataPath = path;
        }

        public void Save(T obj)
        {
            string objData = JsonConvert.SerializeObject(obj, Formatting.Indented);

            using (StreamWriter stream = new StreamWriter(DataPath, false))
            {
                stream.Write(objData);
            }
        }

        public T Read()
        {
            if (File.Exists(DataPath))
            {
                string dataText;
                using (StreamReader stream = File.OpenText(DataPath))
                {
                    dataText = stream.ReadToEnd();
                }
                return JsonConvert.DeserializeObject<T>(dataText);
            }
            else
            {
                return null;
            }
        }
    }
}
