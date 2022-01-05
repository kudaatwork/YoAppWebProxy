using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using YoAppWebProxy.Models.AuditLog;
using YoAppWebProxy.ViewModels;

namespace YoAppWebProxy.Controllers
{
    public class AdminController : Controller
    {
        // GET: Admin
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public ActionResult AuditTrail()
        {
            AuditTrailViewModel auditTrail = new AuditTrailViewModel();

            auditTrail.ServiceProviders = new List<string>();
            auditTrail.SubDirectories = new List<string>();

            // Populate ServiceProviders
            var serviceProviders = GetServiceProviders();

            ViewBag.ServiceProviders = serviceProviders.Select(x => new SelectListItem { Value = x, Text = x }).ToList();

            if (serviceProviders != null)
            {
                auditTrail.ServiceProviders.AddRange(serviceProviders);
            }

            // Populate ServiceSubCategories
            var subCategories = GetSubDirectories(auditTrail.ServiceProviders[0]);

            ViewBag.SubFolders = subCategories.Select(x => new SelectListItem { Value = x, Text = x }).ToList();

            if (subCategories != null)
            {
                auditTrail.SubDirectories.AddRange(subCategories);
            }

            //var logfiles = GetLogFiles(auditTrail.ServiceProviders[0] + "/" + auditTrail.SubDirectories[0]);

            //auditTrail.Logs = new List<Models.AuditLog.AuditTrail>();

            //foreach (var item in logfiles)
            //{
            //    var fileLines = System.IO.File.ReadAllLines(item);

            //    foreach (var fileLine in fileLines)
            //    {
            //        if (!fileLine.StartsWith("=="))
            //        {
            //            var line = JsonConvert.DeserializeObject<AuditTrail>(fileLine);

            //            auditTrail.Logs.Add(line);
            //        }
            //    }
            //}

            var logfiles = GetLogFiles(auditTrail.ServiceProviders[0] + "/" + auditTrail.SubDirectories[0]);

            var fileLines = System.IO.File.ReadAllLines(logfiles.LastOrDefault());

            auditTrail.Logs = new List<Models.AuditLog.AuditTrail>();

            foreach (var fileLine in fileLines)
            {
                if (!fileLine.StartsWith("=="))
                {
                    var line = JsonConvert.DeserializeObject<AuditTrail>(fileLine);

                    auditTrail.Logs.Add(line);
                }
            }

            auditTrail.StartDate = DateTime.Now.Date.ToString("dd/MM/yyyy");
            auditTrail.EndDate = DateTime.Now.Date.ToString("dd/MM/yyyy");

            return View(auditTrail);
        }

        [HttpPost]
        public ActionResult AuditTrail(string serviceProvider)
        {
            bool status = false;

            AuditTrailViewModel auditTrail = new AuditTrailViewModel();

            auditTrail.ServiceProviders = new List<string>();
            auditTrail.SubDirectories = new List<string>();
           
            // Populate ServiceProviders
            var serviceProviders = GetServiceProviders();

            ViewBag.ServiceProviders = serviceProviders.Select(x => new SelectListItem { Value = x, Text = x }).ToList();

            if (serviceProviders != null)
            {
                auditTrail.ServiceProviders.AddRange(serviceProviders);
            }

            // Populate ServiceSubCategories
            var subCategories = GetSubDirectories(serviceProvider);

            ViewBag.SubFolders = subCategories.Select(x => new SelectListItem { Value = x, Text = x }).ToList();

            if (subCategories != null)
            {
                auditTrail.SubDirectories.AddRange(subCategories);
            }

            var logfiles = GetLogFiles(serviceProvider + "/" + auditTrail.SubDirectories[0]);

            //auditTrail.Logs = new List<Models.AuditLog.AuditTrail>();

            //foreach (var item in logfiles)
            //{
            //    var fileLines = System.IO.File.ReadAllLines(item);

            //    foreach (var fileLine in fileLines)
            //    {
            //        if (!fileLine.StartsWith("=="))
            //        {
            //            var line = JsonConvert.DeserializeObject<AuditTrail>(fileLine);

            //            auditTrail.Logs.Add(line);
            //        }
            //    }
            //}

            var fileLines = System.IO.File.ReadAllLines(logfiles.LastOrDefault());

            auditTrail.Logs = new List<Models.AuditLog.AuditTrail>();

            foreach (var fileLine in fileLines)
            {
                if (!fileLine.StartsWith("=="))
                {
                    var line = JsonConvert.DeserializeObject<AuditTrail>(fileLine);

                    auditTrail.Logs.Add(line);
                }
            }

            return PartialView("AuditTrailPartial", auditTrail);
        }

        [HttpPost]
        public ActionResult DateRangeFilter(string startDate, string endDate, string serviceProvider, string subFolder)
        {
            AuditTrailViewModel auditTrail = new AuditTrailViewModel();

            auditTrail.Logs = new List<Models.AuditLog.AuditTrail>();

            if (!string.IsNullOrEmpty(startDate) && !string.IsNullOrEmpty(endDate) && !string.IsNullOrEmpty(serviceProvider) && !string.IsNullOrEmpty(subFolder))
            {
                var convertedStartDate = Convert.ToDateTime(startDate);
                var convertedEndDate = Convert.ToDateTime(endDate);

                var logfiles = GetLogFiles(serviceProvider + "/" + subFolder);

                foreach (var item in logfiles)
                {
                    var fileName = Path.GetFileName(item);

                    var parts = fileName.Split(new char[] { '.' }, StringSplitOptions.RemoveEmptyEntries);

                    if (parts.Length == 4)
                    {
                        var formatedFileName = parts[0] + "/" + parts[1] + "/" + parts[2];

                        var datedFile = DateTime.ParseExact(formatedFileName, "MM/dd/yyyy", System.Globalization.CultureInfo.InvariantCulture);

                        if (convertedStartDate <= datedFile && datedFile <= convertedEndDate)
                        {
                            var fileLines = System.IO.File.ReadAllLines(item);

                            foreach (var fileLine in fileLines)
                            {
                                if (!fileLine.StartsWith("=="))
                                {
                                    var line = JsonConvert.DeserializeObject<AuditTrail>(fileLine);

                                    auditTrail.Logs.Add(line);
                                }
                            }
                        }
                    }
                }
            }

            return PartialView("AuditTrailPartial", auditTrail);
        }

        [NonAction]
        public List<string> GetServiceProviders()
        {
            string root = Server.MapPath("~/App_Data/");

            var subdirectoryEntries = Directory.GetDirectories(root);

            List<string> svcProviders = new List<string>();

            if (subdirectoryEntries != null)
            {
                List<string> serviceProvidersList = new List<string>(subdirectoryEntries);

                foreach (var serviceProvider in serviceProvidersList)
                {
                    var serviceProviderName = new DirectoryInfo(Path.GetDirectoryName(serviceProvider + "/App_Data")).Name;

                    svcProviders.Add(serviceProviderName);
                }
            }
            else
            {
                svcProviders = null;
            }

            return svcProviders;
        }

        [NonAction]
        public List<string> GetSubDirectories(string directory)
        {
            string root = Server.MapPath("~/App_Data/" + directory);

            // Get all subdirectories            

            var subdirectoryEntries = Directory.GetDirectories(root);

            List<string> subDirectoriesList = new List<string>(subdirectoryEntries);

            List<string> subDirectories = new List<string>();

            foreach (var subDirectory in subDirectoriesList)
            {
                var serviceProviderName = new DirectoryInfo(Path.GetDirectoryName(subDirectory + "/" + directory)).Name;

                if (serviceProviderName.Contains("ProxyLog"))
                {
                    subDirectories.Add(serviceProviderName);
                }
            }

            return subDirectories;
        }

        [NonAction]
        public List<string> GetLogFiles(string folderPath)
        {
            string root = Server.MapPath("~/App_Data/" + folderPath);

            string[] files = Directory.GetFiles(root, "*.txt");

            List<string> logFiles = new List<string>(files);

            return logFiles;
        }
    }
}