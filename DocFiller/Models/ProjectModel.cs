using DocFiller.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DocFiller.Models
{
    [Serializable]
    public class ProjectModel
    {
        public string name;

        public HashSet<string> templatePaths;
        public string templatePathText;

        public List<string> templateGroups;
        public Dictionary<string, string> templateMarkSpecGroup;

        public Dictionary<string, string> templateMarks;

        public delegate void ProjectOpenFunctionDelegate(string projectFilePath);

        [NonSerialized]
        public ProjectOpenFunctionDelegate openProjectDelegate;
    }
}
