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

        public ActionResult AuditTrail()
        {
            AuditTrailViewModel auditTrail = new AuditTrailViewModel();

            auditTrail.ServiceProviders = new List<string>();
            auditTrail.SubDirectories = new List<string>();

            // Populate ServiceProviders
            var serviceProviders = GetServiceProviders();           
            
            if (serviceProviders != null)
            {
                auditTrail.ServiceProviders.AddRange(serviceProviders);           
            }

            // Populate ServiceSubCategories
            var subCategories = GetSubDirectories(auditTrail.ServiceProviders[0]);

            if (subCategories != null)
            {
                auditTrail.SubDirectories.AddRange(subCategories);
            }

            var logfiles = GetLogFiles(auditTrail.ServiceProviders[0] + "/" + auditTrail.SubDirectories[0]);

            var fileLines = System.IO.File.ReadAllLines(logfiles[0]);

            auditTrail.Logs = new List<Models.AuditLog.AuditTrail>();

            foreach (var fileLine in fileLines)
            {
                if (!fileLine.StartsWith("=="))
                {
                    var line = JsonConvert.DeserializeObject<AuditTrail>(fileLine);

                    auditTrail.Logs.Add(line);
                }                
            }

            return View(auditTrail);
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