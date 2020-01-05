using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DirektoryAnalyzer.Models
{
    public class FolderResultModel
    {
        public List<DirectoryModel> Files { get; set; } = new List<DirectoryModel>();
        public string Message { get; set; }
        public bool Result { get; set; }
    }
}