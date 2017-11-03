using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Http;
using System.IO;
using System.Text;

namespace AVISTED.Controllers
{
    public class ConvertersController : Controller
    {
        public string[] commaSeparatedValues(string data)
        {
            List<Dictionary<string, string>> ValueList = JsonConvert.DeserializeObject<List<Dictionary<string, string>>>(data);
            string[] results = new string[ValueList.Count +1];
            int i = 0,j=0;
            foreach (Dictionary<string, string> dict in ValueList)
            {
                if (j==0)
                {
                    results[i++] = string.Join(",", dict.Keys.ToList());
                    j = 1;
                }
                results[i] = string.Join(",", dict.Values.ToList());
                i++;

            }
            return results;
        }
        public string[] spaceSeparatedValues(string data)
        {
            List<Dictionary<string, string>> ValueList = JsonConvert.DeserializeObject<List<Dictionary<string, string>>>(data);
            string[] results = new string[ValueList.Count + 1];
            int i = 0, j = 0;
            foreach (Dictionary<string, string> dict in ValueList)
            {
                if (j == 0)
                {
                    results[i++] = string.Join(" ", dict.Keys.ToList());
                    j = 1;
                }
                results[i] = string.Join(" ", dict.Values.ToList());
                i++;

            }
            return results;
        }
        public string CsvToJson(string value)
        {
            // Get lines.
            if (value == null) return null;
            string[] lines = value.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
            if (lines.Length < 2) throw new InvalidDataException("Must have header line.");

            // Get headers.
            string[] headers = lines.First().Split(',');

            // Build JSON array.
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("[");
            for (int i = 1; i < lines.Length; i++)
            {
                string[] fields = lines[i].Split(',');
                if (fields.Length != headers.Length) throw new InvalidDataException("Field count must match header count.");
                var jsonElements = headers.Zip(fields, (header, field) => string.Format("{0}: {1}", header, field)).ToArray();
                string jsonObject = "{" + string.Format("{0}", string.Join(",", jsonElements)) + "}";
                if (i < lines.Length - 1)
                    jsonObject += ",";
                sb.AppendLine(jsonObject);
            }
            sb.AppendLine("]");
            return sb.ToString();
        }
    }

}