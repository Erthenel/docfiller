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

        public List<string> templateGroups = new List<string>() { "Общие закладки"};
        public Dictionary<string, string> templateMarkSpecGroup = new Dictionary<string, string>();

        public Dictionary<string, string> templateMarks = new Dictionary<string, string>();

        public delegate void ProjectOpenFunctionDelegate(string projectFilePath);

        [NonSerialized]
        public ProjectOpenFunctionDelegate openProjectDelegate;
    }
}
