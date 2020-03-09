using System;
using Newtonsoft.Json;

namespace Bizingo
{
    public class Pacote
    {
        [JsonProperty("comando")]
        public string Comando { get; set; }

        [JsonProperty("mensagem")]
        public string Mensagem { get; set; }

        public Pacote(string comando = "", string mensagem = "")
        {
            Comando = comando;
            Mensagem = mensagem;
        }

        public override string ToString()
        {
            return string.Format(
                "[Packet:\n" +
                "  Comando=`{0}`\n" +
                "  Mensagem=`{1}`]",
                Comando, Mensagem);
        }

        // Serialize to Json
        public string ToJson()
        {
            return JsonConvert.SerializeObject(this);
        }

        // Deserialize
        public static Pacote FromJson(string jsonData)
        {
            return JsonConvert.DeserializeObject<Pacote>(jsonData);
        }
    }
}
