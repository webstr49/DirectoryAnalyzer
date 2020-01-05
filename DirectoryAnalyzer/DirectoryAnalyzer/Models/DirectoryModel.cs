using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DirektoryAnalyzer.Models
{
    public class DirectoryModel
    {
        public string Name { get; set; }
        public string Path { get; set; }
        public DateTime LastWrite { get; set; }
        public FileType Type { get; set; }
        public int Version { get; set; }
    }

    public enum FileType
    {
        None,
        Created,
        Edited,
        Deleted
    }
}