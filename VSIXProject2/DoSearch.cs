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
                                

                                ////فقط میره نام namespace رو سرچ میکنه که به کار ما نمیاد ما باید بریم داخل nampcespace
                                var find = element.Name.FirstOrDefault(x => element.Name.Contains("test"));
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
                                              
         
                                                if (body.Contains("C"))
                                                {
                                                    body = body.Replace("c", "");
                                                    ///"TextDocument" یک مقدار شناخته شده هست برای ویژوال که یک فایل رو میگیره 
                                                    TextDocument textDoc = (TextDocument)fileitem.Document.Object("TextDocument");

                                                    EditPoint startpoint = textDoc.StartPoint.CreateEditPoint();
                                                    EditPoint endpoint = textDoc.EndPoint.CreateEditPoint();
                                                    start.Delete(endpoint);
                                                    start.Insert(body);
                                                }
                                            }


                                                foreach (CodeElement child3 in child2.Children)
                                                {
                                                    var ccc = child3.Name;
                                                }

                                            }

                                        }

                                    }
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
