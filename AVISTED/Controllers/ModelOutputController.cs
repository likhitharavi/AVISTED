using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using AVISTED.Models;
using System.Text.RegularExpressions;
using System.IO;
using System.Text;
using System.Xml.Serialization;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System.Net.Http;
using Microsoft.Extensions.Logging;

namespace AVISTED.Controllers
{
    public class ModelOutputController : Controller
    {
        IConfiguration _configuration;
        private readonly ILogger<ModelOutputController> _logger;
        public ModelOutputController(IConfiguration configuration, ILogger<ModelOutputController> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        //Page that provides Extract options for the selected Database
        public IActionResult Extract(int? id)
        {
            string path = _configuration["AppSettings:Metadata"] + id.ToString() + "\\Load.xml";
            var xml = System.IO.File.ReadAllText(path);
            xml = string.Join(" ", Regex.Split(xml, @"(?:\r\n|\n|\r|\t)"));
            DatasetInfo data;
            using (MemoryStream stream = new MemoryStream(Encoding.UTF8.GetBytes(xml)))
            {
                XmlSerializer serializer = new XmlSerializer(typeof(DatasetInfo));
                data = (DatasetInfo)serializer.Deserialize(stream);
            }
            foreach (var parameter in data.parameters)
            {

                if (string.Compare(parameter.type, "DateTime") == 0)
                {
                    data.startDateField = Convert.ToDateTime(parameter.max);
                    data.endDateField = Convert.ToDateTime(parameter.max);

                }
            }
            data.isPostback = false;
            ViewBag.datasetid = id.ToString();
            HttpContext.Session.SetString("SelectedModel", JsonConvert.SerializeObject(data));

            return View(data);
        }

        [HttpPost]
        public IActionResult Extract()
        {
            var selectModel = JsonConvert.DeserializeObject<DatasetInfo>(HttpContext.Session.GetString("SelectedModel"));
            ExtractInfo info = new ExtractInfo();
            info.parameters = (string)Request.Form["parameter"];
            selectModel.isPostback = true;

            if (info.parameters == null || info.parameters == "")
                ModelState.AddModelError("parameter", "Parameter is required.");
            else
                selectModel.parameterField = (string)Request.Form["parameter"];

            //Statistics
            info.stat = (string)Request.Form["stat"];
            if (info.stat == null || info.stat == "")
                ModelState.AddModelError("stat", "Statistics is required.");
            else
                selectModel.statField = (string)Request.Form["stat"];
            //StartDate
            try
            {

                info.startDate = DateTime.Parse(Request.Form["startDate"]);
                selectModel.startDateField = Convert.ToDateTime(Request.Form["startDate"]);
            }
            catch (FormatException e)
            {
                if (info.startDate == null)
                    ModelState.AddModelError("startDate", "Start Date is required.");
                else
                    ModelState.AddModelError("startDate", e.Message + "-Start Date");
            }

            //End Date
            try
            {

                info.endDate = DateTime.Parse(Request.Form["endDate"]);
                selectModel.endDateField = Convert.ToDateTime(Request.Form["endDate"]);
            }
            catch (FormatException e)
            {
                if (info.endDate == null)
                    ModelState.AddModelError("lastDate", "End Date is required.");
                else
                    ModelState.AddModelError("lastDate", e.Message + "-End Date");
            }

            if (!(info.startDate == null && info.endDate == null))
            {
                int result = DateTime.Compare(info.startDate, info.endDate);
                if (result > 0)
                    ModelState.AddModelError("Date", "Start Date is later than End Date");
            }


            //Latitude
            try
            {
                info.latmin = double.Parse(Request.Form["latmin"]);
               

            }
            catch (FormatException e)
            {

                if (Request.Form["latmin"] == "")
                    ModelState.AddModelError("latmin", "latmin is required.");
                else
                    ModelState.AddModelError("latmin", e.Message + "-latmin");
                
            }


            //Longitude
            try
            {
                info.lonmin = double.Parse(Request.Form["lonmin"]);
               }
            catch (FormatException e)
            {

                if (Request.Form["lonmin"] != "")
                    ModelState.AddModelError("lonmin", e.Message + "-lonmin");

                else
                    ModelState.AddModelError("lonmin", "lonmin is required.");
            }

            //Redirect page for the desired output
            var decision = Request.Form["decision"];
            if (Request.Form["outFormat"] == "")
            {
                if (decision == "Download")
                {
                    ModelState.AddModelError("outFormat", "Output Format is required.");
                }
            }
            else
            //outFormat
            {
                info.outFormat = (string)Request.Form["outFormat"];
                if (Request.Form["save"].Equals("on"))
                {
                    info.saveDownload = true;
                }
                else
                {
                    info.saveDownload = false;

                }
                selectModel.saveDownload = info.saveDownload;
                selectModel.outFormatField = info.outFormat;

            }
            info.format = (string)Request.Form["format"];
            info.path = (string)Request.Form["path"];
            if (!ModelState.IsValid)
            {
                return View(selectModel);
            }
            else
            {

                //check if it is the same request.Dont call WebApi if it is same
                if (string.IsNullOrEmpty(HttpContext.Session.GetString("info")))
                {
                    HttpContext.Session.SetString("info", JsonConvert.SerializeObject(info));
                    String Status = GetDatafromWebApi(selectModel);
                }
                else
                {
                    string oldrequest = HttpContext.Session.GetString("info");
                    string newrequest = JsonConvert.SerializeObject(info);
                    if (!(oldrequest.Equals(newrequest)))
                    {
                        HttpContext.Session.SetString("info", JsonConvert.SerializeObject(info));
                        String Status =  GetDatafromWebApi(selectModel);
                    }
                }


                HttpContext.Session.SetString("parameters", info.parameters.ToString());
                HttpContext.Session.SetString("period", selectModel.startDateField.Year.ToString() + "," + selectModel.endDateField.Year.ToString());
                if (decision == "Visualize")
                {
                    return RedirectToAction("Visualize", "Visualize");
                }
                else if (decision == "View")
                {
                    return RedirectToAction("ViewData");
                }
                else
                {
                    return RedirectToAction("Download", new { @type = info.outFormat });
                }
            }
        }

        //Call the webAPI to extract data from the Dataset 
        private string GetDatafromWebApi(DatasetInfo model)
        {
            string info = HttpContext.Session.GetString("info");
            string apiUrl = _configuration["WebApis:AVISTEDDataExtractor"] + info;

            try
            {
                using (HttpClient client = new HttpClient())
                {
                    client.BaseAddress = new Uri(apiUrl);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

                    HttpResponseMessage response = client.GetAsync(apiUrl).GetAwaiter().GetResult();
                    string reader = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                    
                    if (response.IsSuccessStatusCode)
                    {
                        var data = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                        List<Dictionary<string, string>> ls = JsonConvert.DeserializeObject<List<Dictionary<string, string>>>(data);
                        HttpContext.Session.SetString("Data", JsonConvert.SerializeObject(ls));
                        return "success";
                    }
                    else
                    {
                        return "Data Not found.";
                    }
                }
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }


        //Call the WebAPI for converting the data to NetCDF and HDF5 formats
        private string DownloadDatafromWebApi(string type, bool infoDownload)
        {
           string data = HttpContext.Session.GetString("Data");
            string randomlyGeneratedFolderNamePart = Path.GetFileNameWithoutExtension(Path.GetRandomFileName());

            string timeRelatedFolderNamePart = DateTime.Now.Year.ToString()
                                             + DateTime.Now.Month.ToString()
                                             + DateTime.Now.Day.ToString()
                                             + DateTime.Now.Hour.ToString()
                                             + DateTime.Now.Minute.ToString()
                                             + DateTime.Now.Second.ToString()
                                             + DateTime.Now.Millisecond.ToString();

            string processRelatedFolderNamePart = System.Diagnostics.Process.GetCurrentProcess().Id.ToString();
            string copypath = _configuration["AppSettings:Converters"];
            string temporaryDirectoryName = Path.Combine(copypath
                                                        , timeRelatedFolderNamePart
                                                        + processRelatedFolderNamePart
                                                        + randomlyGeneratedFolderNamePart);
            Directory.CreateDirectory(temporaryDirectoryName);
            string fileUri = Path.Combine(temporaryDirectoryName, "data" + ".txt");
            System.IO.File.WriteAllText(fileUri, data);
            string apiUrl = "";
            if (type.Equals("NetCDF"))
            {
                apiUrl = _configuration["WebApis:AVISTEDNetCDFConverter"] + fileUri + "&outfolder=" + infoDownload;
            }
            else
            {
                apiUrl = _configuration["WebApis:AVISTEDHDF5Converter"] + fileUri + "&outfolder=" + infoDownload;
            }
            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri(apiUrl);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

                HttpResponseMessage response = client.GetAsync(apiUrl).GetAwaiter().GetResult();

                if (response.IsSuccessStatusCode)
                {
                    string folder = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                    return folder;
                }
                else
                    return "ERROR";

            }
        }
        //Download Results
        public ActionResult Download(string type)
        {
            var info = JsonConvert.DeserializeObject<ExtractInfo>(HttpContext.Session.GetString("info"));

            if (type.Equals("ASCII"))
            {
                string filename = "results" + Guid.NewGuid().ToString() + ".txt";
                ConvertersController obj = new ConvertersController();
                string[] datacsvs = obj.spaceSeparatedValues(HttpContext.Session.GetString("Data"));
                string dataString = "";
                foreach (string s in datacsvs)
                {
                    dataString = dataString + s + Environment.NewLine;
                }
                var data = Encoding.UTF8.GetBytes(dataString);
                if (info.saveDownload)
                {
                    HttpContext.Session.SetString("type", type);
                    HttpContext.Session.SetString("dataString", dataString);
                    return RedirectToAction("Create", "ArchivesDwnds");
                }
                else
                    return File(data, "text/txt", filename);

            }
            else if (type.Equals("NetCDF"))
            {
                string directoryPath = DownloadDatafromWebApi("NetCDF", info.saveDownload);
                string result = RemoveDirtyCharsFromString(directoryPath);
                string path = Path.GetFullPath(result);
                DirectoryInfo dir1 = new DirectoryInfo(path);

                FileInfo[] DispatchFiles = dir1.GetFiles();
                if (DispatchFiles.Length > 0)
                {
                    foreach (FileInfo aFile in DispatchFiles)
                    {
                        byte[] doc = System.IO.File.ReadAllBytes(Path.Combine(dir1.FullName, aFile.Name));
                        if (info.saveDownload)
                        {
                            HttpContext.Session.SetString("type", type);
                            HttpContext.Session.SetString("URI", Path.Combine(dir1.FullName, aFile.Name));
                            return RedirectToAction("Create", "ArchivesDwnds");
                        }
                        else
                        {
                            return File(doc, "application/netcdf", "result.nc");
                        }
                    }
                    return File("Data not found", "text/txt", "Result.txt");
                }
                else
                {
                    return File("Data not found", "text/txt", "Result.txt");
                }

            }
            else if (type.Equals("HDF5"))
            {
                string directoryPath = DownloadDatafromWebApi("HDF5", info.saveDownload);
                string result = RemoveDirtyCharsFromString(directoryPath);
                string path = Path.GetFullPath(result);


                DirectoryInfo dir1 = new DirectoryInfo(path);
                FileInfo[] DispatchFiles = dir1.GetFiles();
                if (DispatchFiles.Length > 0)
                {
                    foreach (FileInfo aFile in DispatchFiles)
                    {
                        byte[] doc = System.IO.File.ReadAllBytes(Path.Combine(dir1.FullName, aFile.Name));
                        if (info.saveDownload)
                        {
                            HttpContext.Session.SetString("type", type);
                            HttpContext.Session.SetString("URI", Path.Combine(dir1.FullName, aFile.Name));
                            return RedirectToAction("Create", "ArchivesDwnds");
                        }
                        else
                            return File(doc, "application/hdf5", "result.h5");
                    }
                    return File("Data not found", "text/txt", "Result.txt");
                }
                else
                {
                    return File("Data not found", "text/txt", "Result.txt");
                }
            }
            else
            {
                string filename = "results" + Guid.NewGuid().ToString() + ".csv";
                ConvertersController obj = new ConvertersController();
                string[] datacsvs = obj.commaSeparatedValues(HttpContext.Session.GetString("Data"));
                string dataString = "";
                foreach (string s in datacsvs)
                {
                    dataString = dataString + s + Environment.NewLine;
                }
                var data = Encoding.UTF8.GetBytes(dataString);
                if (info.saveDownload)
                {
                    HttpContext.Session.SetString("type", type);
                    HttpContext.Session.SetString("dataString", dataString);
                    return RedirectToAction("Create", "ArchivesDwnds");
                }
                else
                    return File(data, "text/csv", filename);

            }
        }
        public new ActionResult ViewData()
        {

            ConvertersController obj = new ConvertersController();
            ViewBag.Results = obj.commaSeparatedValues(HttpContext.Session.GetString("Data"));
            return View();
        }

        private static string RemoveDirtyCharsFromString(string in_string)
        {
            int index = 0;
            int removed = 0;

            byte[] in_array = Encoding.UTF8.GetBytes(in_string);

            foreach (byte element in in_array)
            {
                if ((element == '"') ||
                    (element == '-')
                    )
                {
                    removed++;
                }
                else
                {
                    in_array[index] = element;
                    index++;
                }
            }

            Array.Resize<byte>(ref in_array, (in_array.Length - removed));
            return (Encoding.UTF8.GetString(in_array, 0, in_array.Length));
        }
    }
    public static class Extensions
    {
        public static StringContent AsJson(this object o)
            => new StringContent(JsonConvert.SerializeObject(o), Encoding.UTF8, "application/json");
    }
}