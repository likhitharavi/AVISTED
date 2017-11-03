using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using AVISTED.Data;
using AVISTED.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System.Globalization;
using System.IO;
using System.Net.Http.Headers;
using System.Text.RegularExpressions;

namespace AVISTED.Controllers
{
    public class MdlOptMngmtController : Controller
    {
        private readonly ApplicationDbContext _context;
        IConfiguration _configuration;

        public MdlOptMngmtController(ApplicationDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        // GET: MdlOptMngmt
        public async Task<IActionResult> Index()
        {
           // System.Security.Claims.ClaimsPrincipal currentUser = this.User;
            if (User.IsInRole("Admin"))
            {
                ViewBag.Delete = "True";
            }
            else
            {
                ViewBag.Delete = "False";
            }
            ViewBag.EmailId = HttpContext.Session.GetString("userName");
            return View(await _context.Dataset.ToListAsync());
        }

        // GET: MdlOptMngmt/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var dataset = await _context.Dataset
                .SingleOrDefaultAsync(m => m.ID == id);
            if (dataset == null)
            {
                return NotFound();
            }

            return View(dataset);
        }

        // GET: MdlOptMngmt/Create
        public IActionResult Create()
        {
            return View();
        }

        public IActionResult Guest()
        {
            int id1 = 1;
            return RedirectToAction("Extract", "ModelOutput", new { id = id1 });

        }


        // POST: MdlOptMngmt/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ID,Name,Description,Format,Size,Author,StartDate,Parameters")] Dataset dataset, ICollection<IFormFile> files)
        {
            if (ModelState.IsValid)
            {
                Regex re = new Regex("[\\/:*?\"<>|]");
                string dsname = re.Replace(dataset.Name, " ");
                string foldername =   User.Identity.Name + DateTime.Now.ToString("yyyyMMddHHmmss") + dataset.Name;
                var uploads = Path.Combine(_configuration["AppSettings:Uploads"], foldername);

                if (!Directory.Exists(uploads))
                {
                    // Try to create the directory.
                    DirectoryInfo di = Directory.CreateDirectory(uploads);
                }


                foreach (var file in files)
                {
                    if (file.Length > 0)
                    {
                        using (var fileStream = new FileStream(Path.Combine(uploads, file.FileName), FileMode.Create))
                        {
                            await file.CopyToAsync(fileStream);
                        }
                    }
                }
                 dataset.EmailId = HttpContext.Session.GetString("userName");
                dataset.UploadDate = DateTime.Now;
                dataset.Status = "MODEL-UNDER-VALIDATION";
                _context.Add(dataset);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(dataset);
        }

        // GET: MdlOptMngmt/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var dataset = await _context.Dataset.SingleOrDefaultAsync(m => m.ID == id);
            if (dataset == null)
            {
                return NotFound();
            }
            return View(dataset);
        }

        // POST: MdlOptMngmt/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ID,Name,Description,Format,Size,Author,StartDate,Parameters")] Dataset dataset)
        {
            if (id != dataset.ID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(dataset);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DatasetExists(dataset.ID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(dataset);
        }

        // GET: MdlOptMngmt/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var dataset = await _context.Dataset
                .SingleOrDefaultAsync(m => m.ID == id);
            if (dataset == null)
            {
                return NotFound();
            }

            return View(dataset);
        }

        //Reroute to the Model Output Page after selecting a dataset
        public async Task<IActionResult> Select(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var dataset = await _context.Dataset.SingleOrDefaultAsync(m => m.ID == id);
            if (dataset == null)
            {
                return NotFound();
            }

            return RedirectToAction("Extract", "ModelOutput", new { id = dataset.ID });
        }

      
        public IActionResult Upload(IFormFile File1)
        {
            var filePath = string.Empty;
            DateTime startDateField = DateTime.Now, endDateField= DateTime.Now;

            string content;
            using (var reader = new StreamReader(File1.OpenReadStream()))
            {
                var fileContent = reader.ReadToEnd();
                var parsedContentDisposition = ContentDispositionHeaderValue.Parse(File1.ContentDisposition);

                string Filename = parsedContentDisposition.FileName;
                content = fileContent;

            }
            List<Dictionary<string, string>> list = new List<Dictionary<string, string>>();
            Dictionary<string, string> Dict = new Dictionary<string, string>();

            string[] rows = content.Split('\n');
            // HttpContext.Session.SetString("parameters", rows[0].Trim());
            rows[0] = rows[0].Trim();
            string p = rows[0];
            p = p.Replace("date,", string.Empty);
            HttpContext.Session.SetString("parameters", p);
            string[] parameters = rows[0].Split(',');
            string[] values; 
            int j = 0;
            
            bool first = true;
            for (int i = 1; i < rows.Length - 1; i++)
            {
                rows[i] = rows[i].Trim();
                values = rows[i].Split(',');
                foreach (string param in parameters)
                {

                    if (param.Equals("date"))
                    {
                        DateTime dt2 = DateTime.Parse(values[j++], CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.AssumeLocal);
                        Dict.Add(param, dt2.ToString("yyyy-MM-dd"));
                        if (first)
                        {
                            startDateField = dt2;
                            first = false;
                        }
                        endDateField = dt2;
                    }
                    else
                    {
                        Dict.Add(param, values[j++]);
                    }
                }
                list.Add(new Dictionary<string, string>(Dict));
                Dict.Clear();
                j = 0;
            }
            HttpContext.Session.SetString("period", startDateField.Year.ToString() + "," + endDateField.Year.ToString());

            var data = list;
            HttpContext.Session.SetString("Data", JsonConvert.SerializeObject(list));

            // List<Dictionary<string,string>> dict = JsonConvert.des()
            return RedirectToAction("Visualize", "visualize");
        }

        // POST: MdlOptMngmt/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var dataset = await _context.Dataset.SingleOrDefaultAsync(m => m.ID == id);
            _context.Dataset.Remove(dataset);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool DatasetExists(int id)
        {
            return _context.Dataset.Any(e => e.ID == id);
        }
    }
}
