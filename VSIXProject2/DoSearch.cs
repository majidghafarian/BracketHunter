using EnvDTE;
using Microsoft.VisualStudio.Shell;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace VSIXProject2
{
    public static class DoSearch
    {
        public static string Solotions()
        {
            List<string> change = new List<string>();
            DTE dte = Package.GetGlobalService(typeof(DTE)) as DTE;
            if (dte != null && dte.Solution != null)
            {

                foreach (Project item in dte.Solution.Projects)
                {
                    foreach (ProjectItem fileitem in item.ProjectItems)
                    {
                        if (fileitem.Name.EndsWith(".cs"))
                        {
                            var rrr = fileitem.FileCodeModel;
                            foreach (CodeElement element in rrr.CodeElements)
                            {
                                var zzzzz = element.Name;
                                var find = zzzzz.FirstOrDefault(x =>zzzzz.StartsWith("t"));
                                if (find != null)
                                {
                                
                                }

                            }
                        }

                    }

                }
                string res = string.Join(" ", change);

                return res;
            }
            return "nul";

        }

    }
}
