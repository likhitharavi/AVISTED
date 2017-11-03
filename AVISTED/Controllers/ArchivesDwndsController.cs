using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using AVISTED.Data;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Http;
using System.IO;

namespace AVISTED.Models
{
    public class ArchivesDwndsController : Controller
    {
        private readonly ApplicationDbContext _context;
        IConfiguration _configuration;

        public ArchivesDwndsController(ApplicationDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        // GET: ArchivesDwnds
        public async Task<IActionResult> Index()
        {
            string username = HttpContext.Session.GetString("userName");
            var checkImg = _context.ArchivesDownload.Where(m => m.UserName == username).Any(p => p.ImgDown == true);
            var checkDown = _context.ArchivesDownload.Where(m => m.UserName == username).Any(p => p.ImgDown == false);
            ViewBag.checkDown = checkDown;
            ViewBag.checkImg = checkImg;
            return View(_context.ArchivesDownload.ToList().Where(m => m.UserName == username));
        }

        // GET: ArchivesDwnds/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var archivesDwnd = await _context.ArchivesDownload
                .SingleOrDefaultAsync(m => m.ID == id);
            if (archivesDwnd == null)
            {
                return NotFound();
            }

            return View(archivesDwnd);
        }

        // GET: ArchivesDwnds/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: ArchivesDwnds/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ID,UserName,FileName,Date,Path,ImgDown,FileType")] ArchivesDownload archivesDwnd)
        {
            var type = HttpContext.Session.GetString("type");
            if (type.Equals("Image"))
                {
                    ArchivesDownload aD = new ArchivesDownload();
                    aD.Date = DateTime.Now;
                    aD.UserName = HttpContext.Session.GetString("userName");

                    aD.ImgDown = true;
                    if (archivesDwnd.FileName == "")
                    {
                        aD.FileName = "AVISTEDImage";
                    }
                    else
                    {
                        aD.FileName = archivesDwnd.FileName;
                    }
                    aD.FileType = type;
                    aD.Path = HttpContext.Session.GetString("URI");
                    _context.Add(aD);
                    await _context.SaveChangesAsync();
                    return RedirectToAction("Index");
                }

                if (type.Equals("NetCDF") || type.Equals("HDF5"))
                {
                    ArchivesDownload aD = new ArchivesDownload();
                    aD.Date = DateTime.Now;
                    aD.UserName = HttpContext.Session.GetString("userName");
                    aD.FileType = type;
                    aD.ImgDown = false;
                    if (archivesDwnd.FileName == "")
                    {
                        aD.FileName = "AVISTEDImage";
                    }
                    else
                    {
                        aD.FileName = archivesDwnd.FileName;
                    }
                    aD.Path = HttpContext.Session.GetString("URI");
                    _context.Add(aD);
                    await _context.SaveChangesAsync();
                    return RedirectToAction("Index");

                }
                else
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
                    string copypath = _configuration["AppSettings:Save_Downloads"];
                    string temporaryDirectoryName = Path.Combine(copypath
                                                                , timeRelatedFolderNamePart
                                                                + processRelatedFolderNamePart
                                                                + randomlyGeneratedFolderNamePart);
                    Directory.CreateDirectory(temporaryDirectoryName);
                    ArchivesDownload aD = new ArchivesDownload();
                    aD.Date = DateTime.Now;
                    aD.UserName = HttpContext.Session.GetString("userName");

                    aD.ImgDown = false;
                    // string filename = "data";
                    if (type.Equals("ASCII"))
                    {
                        aD.FileType = type;
                        if (archivesDwnd.FileName == "")
                        {
                            aD.FileName = "result";
                        }
                        else
                        {
                            aD.FileName = archivesDwnd.FileName;
                        }
                        string fileUri = Path.Combine(temporaryDirectoryName, archivesDwnd.FileName + ".txt");
                        string data = HttpContext.Session.GetString("dataString");
                        System.IO.File.WriteAllText(fileUri, data);
                        aD.Path = fileUri;
                        //var status = saveDownloadinDatabase(fileName, fileUri, 1);
                    }
                    else
                    {
                        aD.FileType = type;

                        if (archivesDwnd.FileName == "")
                        {
                            aD.FileName = "result";
                        }
                        else
                        {
                            aD.FileName = archivesDwnd.FileName;
                        }
                        string fileUri = Path.Combine(temporaryDirectoryName, archivesDwnd.FileName + ".csv");
                        string dataString = HttpContext.Session.GetString("dataString");
                        //var data = Encoding.UTF8.GetBytes(dataString);
                        System.IO.File.WriteAllText(fileUri, dataString);
                        aD.Path = fileUri;
                        //var status = saveDownloadinDatabase(fileName, fileUri, 1);
                    }
                    _context.Add(aD);
                    await _context.SaveChangesAsync();
                    return RedirectToAction("Index");
                }
           
        }

        // GET: ArchivesDwnds/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var archivesDwnd = await _context.ArchivesDownload.SingleOrDefaultAsync(m => m.ID == id);
            if (archivesDwnd == null)
            {
                return NotFound();
            }
            return View(archivesDwnd);
        }

        // POST: ArchivesDwnds/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ID,UserName,FileName,Date,Path,ImgDown,FileType")] ArchivesDownload archivesDwnd)
        {
            if (id != archivesDwnd.ID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(archivesDwnd);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ArchivesDwndExists(archivesDwnd.ID))
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
            return View(archivesDwnd);
        }

        // GET: ArchivesDwnds/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var archivesDwnd = await _context.ArchivesDownload
                .SingleOrDefaultAsync(m => m.ID == id);
            if (archivesDwnd == null)
            {
                return NotFound();
            }

            return View(archivesDwnd);
        }

        // POST: ArchivesDwnds/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var archivesDwnd = await _context.ArchivesDownload.SingleOrDefaultAsync(m => m.ID == id);
            _context.ArchivesDownload.Remove(archivesDwnd);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ArchivesDwndExists(int id)
        {
            return _context.ArchivesDownload.Any(e => e.ID == id);
        }
        //Download file
        public async Task<IActionResult> Download(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var archivesDwnd = await _context.ArchivesDownload.SingleOrDefaultAsync(m => m.ID == id);
            // get type

            if (archivesDwnd == null)
            {
                return NotFound();
            }
            string type = Path.GetExtension(archivesDwnd.Path);
            string path = Path.GetFullPath(archivesDwnd.Path);
            string filename = Path.GetFileName(path);
            if (type.Equals(".nc"))
            {
                byte[] doc = System.IO.File.ReadAllBytes(path);
                return File(doc, "application/netcdf", filename);

            }
            else if (type.Equals(".h5"))
            {
                byte[] doc = System.IO.File.ReadAllBytes(path);
                return File(doc, "application/hdf5", filename);
            }
            else if (type.Equals(".txt"))
            {
                byte[] doc = System.IO.File.ReadAllBytes(path);
                return File(doc, "text/txt", filename);

            }
            else if (type.Equals(".csv"))
            {
                byte[] doc = System.IO.File.ReadAllBytes(path);
                return File(doc, "text/csv", filename);

            }
            else if (type.Equals(".svg"))
            {
                byte[] doc = System.IO.File.ReadAllBytes(path);
                return File(doc, "text/png", filename);
            }

            return View(archivesDwnd);
        }
    }
}
