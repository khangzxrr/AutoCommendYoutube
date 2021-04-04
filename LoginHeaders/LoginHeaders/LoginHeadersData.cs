using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoginHeaders
{
    public class LoginHeadersData
    {
        public Dictionary<string, string> headers { get; set; }

        public LoginHeadersData()
        {
            headers = new Dictionary<string, string>();
        }

        public string Serialize()
        {
            return JsonConvert.SerializeObject(this);
        }

        public static LoginHeadersData Deserialize(string data)
        {
            try
            {
                return JsonConvert.DeserializeObject<LoginHeadersData>(data);
            }catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return null;
        }



    }
}
