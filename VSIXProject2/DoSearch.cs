using EnvDTE;
using Microsoft.VisualStudio.Shell;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;

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
                        var CsName = fileitem.Name;
                        if (fileitem.Name.EndsWith(".cs"))
                        {


                            var rrr = fileitem.FileCodeModel;
                            foreach (CodeElement element in rrr.CodeElements)
                            {

                                if (element.Kind == vsCMElement.vsCMElementNamespace)
                                {
                                    foreach (CodeElement Child in element.Children)
                                    {
                                        var yyyy = Child.Name;
                                        ///اینجا تازه ما میرسیم به روی کلاس باید بریم داخل کلاس که چند تا متد داره و روی اون ها سرچ کنه 
                                        var findtochild = yyyy.FirstOrDefault(x => yyyy.Contains("yy"));

                                        foreach (CodeElement child2 in Child.Children)
                                        {
                                            var bbbb = child2.Name;
                                            ///برای اینکه بخوای یک متد رو نمایش بده باید از codefunction استفاده بشه
                                            ///دیدگاه شی کونه به هر متدی با استفاده از codefunction
                                            if (child2 is CodeFunction method)
                                            {
                                                EditPoint start = method.GetStartPoint().CreateEditPoint();
                                                string body = start.GetText(method.GetEndPoint());

                                                ///هر جا که رسیدی به علامن /n بریزش توی یک آرایه دیگه 
                                                string[] lines = body.Split('\n');
                                                for (int i = 0; i < lines.Length; i++)
                                                {
                                                    /////اینجا گیر کردم
                                                    if (lines[0]!=  lines.Contains("[") && lines.Contains("]"))
                                                    {
                                                        continue;

                                                    }
                                                    string line = lines[i];
                                                    if (line.Trim().Contains("[") && line.Trim().Contains("]"))
                                                    {
                                                        // ✅ داخل این if فقط وقتی میاد که هم [ و هم ] وجود دارن
                                                        int starts = line.IndexOf('[');
                                                        int end = line.IndexOf(']');
                                                        string between = line.Substring(starts + 1, end - starts - 1);
                                                      
                                                        lines[i] = between;
                                                    }
                                                   if(i==lines.Length-1)
                                                    { 
                                                    string Filename = fileitem.Name + " " + child2.Name + Guid.NewGuid();
                                                    string finalText = string.Join(Environment.NewLine, lines);
                                                    File.WriteAllText($@"D:\New folder\{Filename}.txt", finalText);
                                                    }
                                                }

                                                ///هر وقت به آخرین اندیس lines رسیدی بعدش ذخیره کن



                                                /// برای زمانی هست که میخواد تغییرات رو خود پروژه اعمال بشه 
                                                //TextDocument textDoc = (TextDocument)fileitem.Document.Object("TextDocument");
                                                //EditPoint startpoint = textDoc.StartPoint.CreateEditPoint();
                                                //EditPoint endpoint = textDoc.EndPoint.CreateEditPoint();
                                                //startpoint.Delete(endpoint);
                                                //startpoint.Insert(lines.ToString());

                                            }


                                            foreach (CodeElement child3 in child2.Children)
                                            {
                                                var ccc = child3.Name;
                                            }

                                        }

                                    }

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
