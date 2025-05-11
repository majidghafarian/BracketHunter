using EnvDTE;
using Microsoft.VisualStudio.Shell;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;

namespace VSIXProject2
{
    public static class MyCache
    {
        //    public static Dictionary<string, object> Data = new Dictionary<string, object>();

        //    public static void SaveToCache(string key, string value)
        //    {
        //        if (Data.ContainsKey(key))
        //        {
        //            Data[key] = value;

        //        }
        //        else
        //        {
        //            Data.Add(key, value);
        //        }
        //    }
        //    public static string GetFile()
        //    {
        //        StringBuilder sb = new StringBuilder();
        //        foreach (var res in Data)
        //        {
        //            sb.AppendLine(res.Value.ToString());
        //            sb.AppendLine("\n");

        //        }
        //        return sb.ToString();
        //    }
        //}
        //public static class Save
        //{

        //    public static void SaveTofile(string key, string value)
        //    {

        //        File.WriteAllText($@"D:\New folder\{key}", value);

        //    }

        //}

        private static void CsFiles(ProjectItem item, List<ProjectItem> collect)
        {
            if (item.Name.EndsWith(".cs"))
            {
                collect.Add(item);
            }
            foreach (ProjectItem child in item.ProjectItems)
            {
                CsFiles(child, collect);

            }
        }
        public static class DoSearch
        {

            public static string Solotions()
            {

                DTE dte = Package.GetGlobalService(typeof(DTE)) as DTE;
                if (dte != null && dte.Solution != null)
                {


                    foreach (Project item in dte.Solution.Projects)
                    {
                        ////یک کش موقت برای ریختن همه کلاس های که آخرش با .cs تموم میشه 
                        List<ProjectItem> itemssss = new List<ProjectItem>();

                        foreach (ProjectItem searchinproject in item.ProjectItems)
                        {
                            ////برای فایل های .cs تو در تو 
                            CsFiles(searchinproject, itemssss);


                        }

                        List<string> allCsFiles = new List<string>();

                        foreach (var itemfile in itemssss)
                        {
                            var child = itemfile.Name;
                            if (itemfile.FileCodeModel == null) continue;
                            foreach (CodeElement element in itemfile.FileCodeModel.CodeElements)
                            {
                                if (element.Kind == vsCMElement.vsCMElementNamespace)
                                {
                                    foreach (CodeElement children in element.Children)
                                    {
                                        ///اینجا میرسه به کلاس تازه
                                        var name = children.Name;
                                        ///اینجا میریم توی کلاس 
                                        foreach (CodeElement children2 in children.Children)
                                        {
                                            var namechildren2 = children2.Name;
                                            if (children2 is CodeFunction method)
                                            {
                                                EditPoint start = method.GetStartPoint().CreateEditPoint();
                                                string body = start.GetText(method.GetEndPoint());
                                                string[] lines = body.Split('\n');

                                                for (int i = 0; i < lines.Length; i++)
                                                {
                                                    var matches = Regex.Matches(lines[i], @"\[(.*?)\]");
                                                    foreach (Match match in matches)
                                                    {
                                                        allCsFiles.Add(match.Groups[1].Value);
                                                    }



                                                }


                                            }

                                        }


                                    }

                                }
                                if (allCsFiles.Count > 0)
                                {
                                    string filename = $"{itemfile.Name}_{Guid.NewGuid()}.txt";
                                    string fullText = string.Join(Environment.NewLine, allCsFiles);
                                    File.WriteAllText($@"D:\New folder\{filename}", fullText);
                                }

                            }
                 
                        }
                    }
                }
                return "Done";
                //foreach (ProjectItem fileitem in item.ProjectItems)
                //{

                //    ////اینجا باید بگم هر موقع fileitem تموم شد بریزه توی یک فایل و استخراج منه
                //    var CsName = fileitem.Name;
                //    if (fileitem.Name.EndsWith(".cs"))
                //    {


                //        var rrr = fileitem.FileCodeModel;
                //        if (rrr == null)
                //        {
                //            continue;
                //        }
                //        foreach (CodeElement element in rrr.CodeElements)
                //        {

                //            if (element.Kind == vsCMElement.vsCMElementNamespace)
                //            {
                //                foreach (CodeElement Child in element.Children)
                //                {
                //                    var yyyy = Child.Name;
                //                    ///اینجا تازه ما میرسیم به روی کلاس باید بریم داخل کلاس که چند تا متد داره و روی اون ها سرچ کنه 
                //                    var findtochild = yyyy.FirstOrDefault(x => yyyy.Contains("yy"));

                //                    foreach (CodeElement child2 in Child.Children)
                //                    {
                //                        var bbbb = child2.Name;
                //                        ///برای اینکه بخوای یک متد رو نمایش بده باید از codefunction استفاده بشه
                //                        ///دیدگاه شی کونه به هر متدی با استفاده از codefunction
                //                        if (child2 is CodeFunction method)
                //                        {
                //                            EditPoint start = method.GetStartPoint().CreateEditPoint();
                //                            string body = start.GetText(method.GetEndPoint());

                //                            ///هر جا که رسیدی به علامن /n بریزش توی یک آرایه دیگه 
                //                            string[] lines = body.Split('\n');
                //                            List<string> extracted = new List<string>(); // فقط مقادیر داخل [ ] میرن اینجا
                //                            for (int i = 0; i < lines.Length; i++)
                //                            {

                //                                ///اینجا گیر کردم
                //                                //if (!lines[i] lines.Contains("[") && lines.Contains("]"))
                //                                //{
                //                                //    continue;

                //                                //}
                //                                string line = lines[i];
                //                                var matches = Regex.Matches(line, @"\[(.*?)\]");
                //                                foreach (Match match in matches)
                //                                {
                //                                    extracted.Add(match.Groups[1].Value);
                //                                }
                //                                if (i == lines.Length - 1)
                //                                {

                //                                    string Filename = $"{fileitem.Name}_{child2.Name}_{Guid.NewGuid()}.txt";
                //                                    string finalText = string.Join(Environment.NewLine, extracted);
                //                                    MyCache.SaveToCache(Filename, finalText);
                //                                    //File.WriteAllText($@"D:\New folder\{Filename}", finalText);

                //                                }
                //                            }

                //                            ///هر وقت به آخرین اندیس lines رسیدی بعدش ذخیره کن



                //                            /// برای زمانی هست که میخواد تغییرات رو خود پروژه اعمال بشه 
                //                            //TextDocument textDoc = (TextDocument)fileitem.Document.Object("TextDocument");
                //                            //EditPoint startpoint = textDoc.StartPoint.CreateEditPoint();
                //                            //EditPoint endpoint = textDoc.EndPoint.CreateEditPoint();
                //                            //startpoint.Delete(endpoint);
                //                            //startpoint.Insert(lines.ToString());

                //                        }


                //                        foreach (CodeElement child3 in child2.Children)
                //                        {
                //                            var ccc = child3.Name;
                //                        }

                //                    }

                //                }

                //            }

                //        }

                //    }

                //}




                //    }



                //}


            }

        }
    }
}
