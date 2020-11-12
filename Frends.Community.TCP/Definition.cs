#pragma warning disable 1591

using Newtonsoft.Json.Linq;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Frends.Community.TCP
{
    public class Parameters
    {
        [DisplayFormat(DataFormatString = "Text")]
        public string[] Command { get; set; }

        [DisplayFormat(DataFormatString = "Text")]
        public string IpAddress { get; set; }

        [DisplayFormat(DataFormatString = "Text")]
        [DefaultValue(2202)]
        public int Port { get; set; }
    }


    public class Options
    {
        /// <summary>
        /// Read timeout
        /// </summary>
        [DefaultValue(15)]
        public int Timeout { get; set; }

    }

    public class Result
    {
        /// <summary>
        /// Responses in JArray
        /// </summary>
        [DisplayFormat(DataFormatString = "Text")]
        public JArray Responses;
    }
}
