using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using Microsoft.AspNetCore.Http;
using System.Text;
using Newtonsoft.Json;
using Microsoft.Extensions.Configuration;

namespace AVISTED.Controllers
{
    public class VisualizeController : Controller
    {
        //View the results in browser
        IConfiguration _configuration;
        public VisualizeController(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public JsonResult Load()
        {
            //intialize fields
            string Param = HttpContext.Session.GetString("parameters");
            string ReqParam = HttpContext.Session.GetString("parameter");
            string[] fields = HttpContext.Session.GetString("fields").Split('$');
            List<String> parameters = new List<String>(Param.Split(','));
            List<Dictionary<string, string>> ValueList = JsonConvert.DeserializeObject<List<Dictionary<string, string>>>(HttpContext.Session.GetString("Data"));
            List<string> RemoveParams = new List<string>();
            string[] months, years;
            string type = fields[2];
            string compare = fields[3];
            string period = HttpContext.Session.GetString("period");
            string[] yearsPer = period.Split(',');
            //if month/months selected


            List<Dictionary<string, string>> result = new List<Dictionary<string, string>>();
            List<string> ReqParms = new List<string>();


            //setting the months and years
            months = fields[1].Split(',');
            years = fields[0].Split(',');

            if (months.Length > 0 && months[0] != "")
            {

                if (years.Length > 1 && years[0] != "")
                {
                    //if year not selected set it to the first year

                    years[0] = yearsPer[0];
                }

            }



            if (ReqParam != null)
            {
                ReqParms = new List<string>(ReqParam.Split(','));
            }

            //if month/months selected
            if (months.Length > 1)
            {
                //get the extracted dictionary for month1/year, month2/year
                List<Dictionary<string, string>> list1 = new List<Dictionary<string, string>>();
                List<Dictionary<string, string>> list2 = new List<Dictionary<string, string>>();
                List<Dictionary<string, string>> list = new List<Dictionary<string, string>>();
                Dictionary<string, string> dict1 = new Dictionary<string, string>();
                foreach (Dictionary<string, string> dict in ValueList)
                {

                    DateTime date = Convert.ToDateTime(dict["date"]);
                    if (years[0].Equals(date.Year.ToString()) && months[0].Equals(date.Month.ToString()))
                    {
                        dict1.Add("date", date.Day.ToString());
                        if (ReqParam != null)
                        {

                            foreach (string s in ReqParms)
                            {
                                dict1.Add(s + "-" + date.Month, dict[s]);
                            }

                        }
                        list1.Add(new Dictionary<string, string>(dict1));
                        dict1.Clear();
                    }
                    else if (years[0].Equals(date.Year.ToString()) && months[1].Equals(date.Month.ToString()))
                    {
                        Dictionary<string, string> dict2 = new Dictionary<string, string>();
                        dict2.Add("date", date.Day.ToString());
                        if (ReqParam != null)
                        {

                            foreach (string s in ReqParms)
                            {
                                dict2.Add(s + "-" + date.Month, dict[s]);
                            }

                        }
                        list2.Add(new Dictionary<string, string>(dict2));
                        dict2.Clear();
                    }
                }

                if (list1.Count > 0)
                {
                    if (list2.Count() > 0)
                    {
                        //  Dictionary<string, string> dict3 = new Dictionary<string, string>();
                        foreach (Dictionary<string, string> dict3 in list2)
                        {
                            foreach (Dictionary<string, string> dict4 in list1)
                            {
                                if (dict3["date"].Equals(dict4["date"]))
                                {
                                    var resultdict = dict3.Concat(dict4).GroupBy(d => d.Key)
                                      .ToDictionary(d => d.Key, d => d.First().Value);
                                    result.Add(new Dictionary<string, string>(resultdict));
                                    resultdict.Clear();

                                }
                            }
                        }
                    }
                }

            }
            else if (months.Length == 1 && !(months[0].Equals("")))
            {
                List<Dictionary<string, string>> list1 = new List<Dictionary<string, string>>();
                Dictionary<string, string> dict1 = new Dictionary<string, string>();
                foreach (Dictionary<string, string> dict in ValueList)
                {
                    DateTime date = Convert.ToDateTime(dict["date"]);
                    if (years[0].Equals(date.Year.ToString()) && months[0].Equals(date.Month.ToString()))
                    {
                        dict1.Add("date", date.Day.ToString());


                        if (ReqParam != null)
                        {

                            foreach (string s in ReqParms)
                            {
                                dict1.Add(s + "-" + date.Month, dict[s]);
                            }

                        }
                        list1.Add(new Dictionary<string, string>(dict1));
                        dict1.Clear();
                    }
                }
                if (ReqParam != null)
                {
                    //  ReqParms = new List<string>(ReqParam.Split(','));
                    RemoveParams = parameters.Except(ReqParms).ToList();
                    foreach (Dictionary<string, string> dict in list1)
                    {
                        foreach (string s in RemoveParams) { dict.Remove(s); }

                    }
                }
                result = list1;
            }
            //if years/year selected
            else if (years.Length > 1)
            {
                List<Dictionary<string, string>> ValueList2 = ValueList.Where(dict => DictionaryYear(dict, years[0])).ToList();
                List<Dictionary<string, string>> ValueList3 = ValueList.Where(dict => DictionaryYear(dict, years[1])).ToList();
                List<Dictionary<string, string>> list1 = new List<Dictionary<string, string>>();
                Dictionary<string, string> dict1 = new Dictionary<string, string>();
                foreach (Dictionary<string, string> dict in ValueList2)
                {
                    DateTime date = Convert.ToDateTime(dict["date"]);
                    foreach (Dictionary<string, string> dictt in ValueList3)
                    {
                        DateTime date1 = Convert.ToDateTime(dictt["date"]);
                        if (date.Day == date1.Day && date.Month == date1.Month)
                        {
                            string daymon = date1.ToString("MM-dd");
                            dict1.Add("date", daymon);
                            foreach (string s in ReqParms)
                            {
                                dict1.Add(s + "-" + years[0], dict[s]);
                            }
                            foreach (string s in ReqParms)
                            {
                                dict1.Add(s + "-" + years[1], dictt[s]);
                            }

                        }

                    }
                    list1.Add(new Dictionary<string, string>(dict1));
                    dict1.Clear();
                }
                if (type.Equals("Bar"))
                {
                    int months1 = 1;
                    List<string> keyList = new List<string>(list1[0].Keys);
                    List<Dictionary<string, string>> list2 = GetResutforBarchart(list1, keyList, yearsPer, months1);

                    result = list2;
                }
                else
                {
                    result = list1;
                }


            }
            else if (years.Length == 1 && !(years[0].Equals("")))
            {
                List<Dictionary<string, string>> ValueList2 = ValueList.Where(dict => DictionaryYear(dict, years[0])).ToList();
                if (ReqParam != null)
                {
                    //  ReqParms = new List<string>(ReqParam.Split(','));
                    RemoveParams = parameters.Except(ReqParms).ToList();
                    foreach (Dictionary<string, string> dict in ValueList2)
                    {
                        foreach (string s in RemoveParams) { dict.Remove(s); }

                    }
                }

                if (type.Equals("Bar"))
                {
                    int months1 = 1;
                    List<Dictionary<string, string>> list1 = GetResutforBarchart(ValueList2, ReqParms, yearsPer, months1);
                    result = list1;

                }
                else
                {
                    result = ValueList2;
                }

            }
            else if (months.Length == 1 && years.Length == 1 && months[0].Equals("") && years[0].Equals(""))
            {
                //ReqParms = new List<string>(ReqParam.Split(','));
                //string[] req = ReqParam.Split(',');
                //if parameters are selected discard other parameters
                if (ReqParam != null)
                {

                    RemoveParams = parameters.Except(ReqParms).ToList();
                    foreach (Dictionary<string, string> dict in ValueList)
                    {
                        foreach (string s in RemoveParams) { dict.Remove(s); }

                    }
                }
                if (type.Equals("Bar"))
                {
                    int months1 = 0;
                    List<Dictionary<string, string>> list1 = GetResutforBarchart(ValueList, ReqParms, yearsPer, months1);

                    result = list1;
                }
                else
                {
                    result = ValueList;
                }

            }

            return Json(result);
        }

        List<Dictionary<string, string>> GetResutforBarchart(List<Dictionary<string, string>> ValueList, List<string> ReqParm, string[] years, int months)
        {
            Dictionary<string, string> dict1 = new Dictionary<string, string>();
            List<Dictionary<string, string>> list1 = new List<Dictionary<string, string>>();
            if (months == 0)
            {
                for (int i = int.Parse(years[0]); i <= int.Parse(years[1]); i++)
                {
                    List<Dictionary<string, string>> ValueList3 = ValueList.Where(dict => DictionaryYear(dict, i.ToString())).ToList();
                    dict1.Add("date", i.ToString());
                    float total = 0, count = 0;
                    foreach (string req in ReqParm)
                    {
                        foreach (Dictionary<string, string> dict in ValueList3)
                        {
                            total = total + float.Parse(dict[req]);
                            count = count + 1;
                        }
                        float aver = 0;
                        if (count != 0)
                        {
                            aver = total / count;
                        }
                        dict1.Add(req, aver.ToString());
                    }
                    list1.Add(new Dictionary<string, string>(dict1));
                    dict1.Clear();
                }
            }
            else

            {
                for (int i = 1; i < 13; i++)
                {
                    List<Dictionary<string, string>> ValueList3 = ValueList.Where(dict => DictionaryMonth(dict, i.ToString())).ToList();
                    dict1.Add("date", i.ToString());
                    float total = 0, count = 0;
                    foreach (string req in ReqParm)
                    {
                        if (!req.Equals("date"))
                        {
                            foreach (Dictionary<string, string> dict in ValueList3)
                            {
                                total = total + float.Parse(dict[req]);
                                count = count + 1;
                            }
                            float aver = 0;
                            if (count != 0)
                            {
                                aver = total / count;
                            }

                            dict1.Add(req, aver.ToString());
                        }
                    }
                    list1.Add(new Dictionary<string, string>(dict1));
                    dict1.Clear();
                }

            }
            return list1;
        }
        bool DictionaryMonth(Dictionary<string, string> dictionary, string month)
        {

            DateTime dictDate;


            string key = "date";
            if (dictionary.ContainsKey(key) && dictionary[key] != null)
            {
                dictDate = Convert.ToDateTime(dictionary[key]);
                if (month.Equals(dictDate.Month.ToString()))
                {
                    return true;
                }
            }

            return false;
        }

        bool DictionaryYear(Dictionary<string, string> dictionary, string year)
        {

            DateTime dictDate;


            string key = "date";
            if (dictionary.ContainsKey(key) && dictionary[key] != null)
            {
                dictDate = Convert.ToDateTime(dictionary[key]);
                if (year.Equals(dictDate.Year.ToString()))
                {
                    return true;
                }
            }

            return false;
        }

        bool DictionaryContainsText(Dictionary<string, string> dictionary, string text, string text2)
        {
            DateTime startDate = Convert.ToDateTime(text);
            DateTime lastDate = Convert.ToDateTime(text2);
            DateTime dictDate;
            //   int last, begin;

            string key = "date";
            if (dictionary.ContainsKey(key) && dictionary[key] != null)
            {
                dictDate = Convert.ToDateTime(dictionary[key]);
                // begin = DateTime.Compare(startDate, dictDate);
                //last = DateTime.Compare(lastDate, dictDate);
                //begin <= 0 && last >= 0 to get range of values

                if (startDate.Year == dictDate.Year || lastDate.Year == dictDate.Year)
                {
                    return true;
                }
            }

            return false;
        }
        public ActionResult Visualize()
        {
            string Param = HttpContext.Session.GetString("parameters");
            string period = HttpContext.Session.GetString("period");
            string[] years = period.Split(',');
            List<String> parameters = new List<String>(Param.Split(','));
            ViewBag.StartDate = years[0];
            ViewBag.EndDate = years[1];
            ViewBag.Year = "";
            ViewBag.Month = "";
            ViewBag.Parameter = parameters[0];
            ViewBag.Parameters = Param;
            ViewBag.visTypes = "Line,Area,Bar,Scatter";
            ViewBag.Type = "Line";
            ViewBag.Compare = "0";
            string fields = ViewBag.Year + "$" + ViewBag.Month + "$" + ViewBag.Type + "$" + ViewBag.Compare;
            HttpContext.Session.SetString("fields", fields);
            HttpContext.Session.SetString("parameter", parameters[0]);
            ViewBag.Ispostback = false;
            return View();
        }
        //Visualize the data: POST
        [HttpPost]
        [ActionName("Visualize")]
        public ActionResult VisualizePost()
        {

            //Save image in the server
            string imgsrc = (string)Request.Form["ImgFile"];
            string status = "";
            if (!imgsrc.Equals(""))
            {
                status = SaveSVG(imgsrc);

                if (status.Equals("success"))
                {
                    return RedirectToAction("Create", "ArchivesDwnds");
                }

            }

            //postback value of the form
            String parameter = (string)Request.Form["parameter"];
            string vistype = (string)Request.Form["Type"];
            string year = (string)Request.Form["year"];
            string month = (string)Request.Form["month"];

            //get the year
            string period = HttpContext.Session.GetString("period");
            string[] years = period.Split(',');
            ViewBag.StartDate = years[0];
            ViewBag.EndDate = years[1];
            ViewBag.Compare = 0;
            //error handling 
            if (parameter == null || parameter == "")
                ModelState.AddModelError("parameter", "Parameter is required.");
            if (vistype == null || vistype == "")
                ModelState.AddModelError("Type", "Visulization type is required.");

            //get the postback values for year and month
            if (year == null || year == "")
                ViewBag.Year = "";
            else
            {
                ViewBag.Year = year;

                string[] years1 = year.Split(',');
                if (years1.Length > 1)
                    ViewBag.Compare = 1;
            }
            if (month == null || month == "")
                ViewBag.Month = "";
            else
            {
                ViewBag.Month = month;
                string[] months = month.Split(',');
                if (months.Length > 1)
                    ViewBag.Compare = 1;
            }


            ViewBag.visTypes = "Line,Area,Bar,Scatter";
            string Param = HttpContext.Session.GetString("parameters");
            ViewBag.Parameters = Param;
            ViewBag.Ispostback = true;

            if (!ModelState.IsValid)
            {
                List<String> parameters = new List<String>(Param.Split(','));
                ViewBag.Parameter = parameters[0];
                ViewBag.Type = "Line";
                string fields = ViewBag.Year + "$" + ViewBag.Month + "$" + ViewBag.Type + "$" + ViewBag.Compare;
                HttpContext.Session.SetString("fields", fields);
                HttpContext.Session.SetString("parameter", parameters[0]);
            }
            else
            {
                HttpContext.Session.SetString("parameter", parameter);
                ViewBag.Parameter = parameter;
                ViewBag.Type = (string)Request.Form["Type"];
                string fields = ViewBag.Year + "$" + ViewBag.Month + "$" + ViewBag.Type + "$" + ViewBag.Compare;
                HttpContext.Session.SetString("fields", fields);
            }

            return View();

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
                string[] jsonElements = headers.Zip(fields, (header, field) => string.Format("'{0}': '{1}'", header, field)).ToArray();
                string jsonObject = "{" + string.Format("{0}", string.Join(",", jsonElements)) + "}";
                if (i < lines.Length - 1)
                    jsonObject += ",";
                sb.AppendLine(jsonObject);
            }
            sb.AppendLine("]");
            return sb.ToString();
        }
        public string SaveSVG(string data)
        {
            string randomlyGeneratedFolderNamePart = Path.GetFileNameWithoutExtension(Path.GetRandomFileName());

            string timeRelatedFolderNamePart = DateTime.Now.Year.ToString()
                                             + DateTime.Now.Month.ToString()
                                             + DateTime.Now.Day.ToString()
                                             + DateTime.Now.Hour.ToString()
                                             + DateTime.Now.Minute.ToString()
                                             + DateTime.Now.Second.ToString()
                                             + DateTime.Now.Millisecond.ToString();

            string processRelatedFolderNamePart = System.Diagnostics.Process.GetCurrentProcess().Id.ToString();
            string copypath = _configuration["AppSettings:Save_SVGs"];
            string temporaryDirectoryName = Path.Combine(copypath
                                                        , timeRelatedFolderNamePart
                                                        + processRelatedFolderNamePart
                                                        + randomlyGeneratedFolderNamePart);
            Directory.CreateDirectory(temporaryDirectoryName);
            string fileUri = Path.Combine(temporaryDirectoryName, "data.svg");
            HttpContext.Session.SetString("type", "Image");
            HttpContext.Session.SetString("URI", fileUri);
            System.IO.File.WriteAllText(fileUri, data);
            return "success";

        }
    }
}