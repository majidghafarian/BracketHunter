using EnvDTE;
using Microsoft.VisualStudio.Shell;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace VSIXProject2
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

    ///برای فرم های ویندوز که design.cs داخلش هست

    public static class DoSearch
    {

        private static void CsFiles(ProjectItem item, List<ProjectItem> collect)
        {
            if (item.Name.EndsWith(".cs"))
            {
                collect.Add(item);
            }
            if (item.ProjectItems != null)
            {
                foreach (ProjectItem child in item.ProjectItems)
                {
                    CsFiles(child, collect);

                }
            }
        }

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
                    List<string> SortData = new List<string>();
                    foreach (var itemfile in itemssss)
                    {
                        var child = itemfile.Name;
                        if (itemfile.FileCodeModel == null) continue;
                        foreach (CodeElement element in itemfile.FileCodeModel.CodeElements)
                        {
                            ///بورو پیدا کن nmacespace 
                            if (element.Kind == vsCMElement.vsCMElementNamespace)
                            {
                                foreach (CodeElement children in element.Children)
                                {
                                    ///اینجا میرسه به کلاس تازه
                                    //var name = children.Name;
                                    ///اینجا میریم توی کلاس 
                                    foreach (CodeElement children2 in children.Children)
                                    {

                                        //var namechildren2 = children2.Name;
                                        if (children2 is CodeFunction method)
                                        {

                                            //////اینجارو گزاشتم فقط ارور نده تا ببینم بعدش چی میشه
                                            //if (method == null && method.StartPoint == null)
                                            //{
                                            //    continue;
                                            //}

                                            EditPoint start = method.GetStartPoint().CreateEditPoint();
                                            string body = start.GetText(method.GetEndPoint());
                                            string[] lines = body.Split('\n');

                                            for (int i = 0; i < lines.Length; i++)
                                            {
                                                var matches = Regex.Matches(lines[i], @"\[(.*?)\]");
                                                foreach (Match match in matches)
                                                {
                                                    string value = match.Groups[1].Value;
                                                    if (!value.EndsWith(":"))
                                                        continue;
                                                    else
                                                    {
                                                        value.TrimEnd(';');
                                                        string newword = "[" + value + "]:";



                                                    }

                                                    allCsFiles.Add(match.Groups[1].Value);
                                                }
                                            }
                                            ///ذخیره نکردن مقادیر  تکراری 
                                            for (int i = 0; i < allCsFiles.Count; i++)
                                            {
                                                var res = allCsFiles[i].Trim();
                                                if (SortData.Contains(res) || string.IsNullOrWhiteSpace(res))
                                                {
                                                    continue;
                                                }
                                                //SortData.Add(res.Trim());
                                                //string selectedword = SortData.Last();
                                                //if (selectedword.EndsWith(":"))
                                                //{
                                                //    selectedword = selectedword.TrimEnd(':');
                                                //    string newword = "[" + selectedword + "]:";

                                                //    SortData.Add(newword);
                                                //}
                                                else
                                                {
                                                    SortData.Add(allCsFiles[i].Trim());
                                                }
                                            }
                                        }
                                    }
                                }
                            }

                        }

                    }
                    if (SortData.Count > 0)
                    {
                        string filename = $"{item.Name}_{Guid.NewGuid()}.txt";
                        string fullText = string.Join(Environment.NewLine, SortData);
                        File.WriteAllText($@"D:\New folder\{filename}", fullText);
                    }
                }
            }
            return "با موفقیت انجام شد.";

        }


        public static string Replace()
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
                                    if (children.Kind == vsCMElement.vsCMElementClass)
                                    {
                                        foreach (CodeElement children2 in children.Children)
                                        {
                                            if (children2 is CodeFunction method)
                                            {
                                                EditPoint start = method.GetStartPoint().CreateEditPoint();
                                                EditPoint end = method.GetEndPoint().CreateEditPoint();

                                                string body = start.GetText(end);

                                                // ویرایش محتوای body
                                                var matches = Regex.Matches(body, @"\[(.*?)\]");
                                                foreach (Match match in matches)
                                                {
                                                    var value = match.Groups[1].Value;

                                                    if (!value.EndsWith(":"))
                                                        continue;

                                                    string replaced = "[" + value.TrimEnd(':') + "]:";
                                                    body = body.Replace(match.Value, replaced);
                                                }

                                                // پاک کردن محتوای قدیمی و نوشتن محتوای جدید
                                                start.Delete(end);
                                                start.Insert(body);
                                            }
                                        }
                                    }
                                }
                            }
                        }


                    }

                }
            }
            return "با موفقیت انجام شد.";

        }


    }
}


