using DirektoryAnalyzer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DirektoryAnalyzer.Controllers
{
    public class OperationOverFolderController : Controller
    {
        private readonly OperationOverFolder operationOverFolder;

        public OperationOverFolderController()
        {
            operationOverFolder = new OperationOverFolder();
        }

        public ActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Index(string path)
        {
            FolderResultModel result = operationOverFolder.ChangesInDirektory(path);

            return View(result);

        }

    }
}