using EnvDTE;
using Microsoft.VisualStudio.Shell;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace VSIXProject2
{
    public static class MineWordFarsi
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
                    List<ProjectItem> itemssss = new List<ProjectItem>();

                    foreach (ProjectItem searchinproject in item.ProjectItems)
                    {
                        CsFiles(searchinproject, itemssss); // پر کردن لیست فایل‌ها
                    }

                    HashSet<string> SortData = new HashSet<string>(); // برای جلوگیری از مقادیر تکراری
                    Regex regex = new Regex("\"([^\"]+)\"");
                    //Regex regex = new Regex(@"\[(.*?)\]");
                    Regex persianRegex = new Regex(@"[\u0600-\u06FF]");
                    foreach (var itemfile in itemssss)
                    {
                        if (itemfile.FileCodeModel == null) continue;

                        foreach (CodeElement element in itemfile.FileCodeModel.CodeElements)
                        {
                            if (element.Kind == vsCMElement.vsCMElementNamespace)
                            {
                                foreach (CodeElement children in element.Children)
                                {
                                    if (children is CodeFunction method)
                                    {
                                        ProcessCodeBody(method.GetStartPoint(), method.GetEndPoint(), regex, SortData, persianRegex);
                                    }
                                    else if (children is CodeEnum enumtype)
                                    {
                                        ProcessCodeBody(enumtype.GetStartPoint(), enumtype.GetEndPoint(), regex, SortData, persianRegex);
                                    }
                                    else if (children is CodeClass cls)
                                    {
                                        foreach (CodeElement member in cls.Members)
                                        {
                                            if (member is CodeFunction m)
                                            {
                                                ProcessCodeBody(m.GetStartPoint(), m.GetEndPoint(), regex, SortData, persianRegex);
                                            }
                                        }
                                    }
                                }
                            }
                            else if (element.Kind == vsCMElement.vsCMElementEnum)
                            {
                                var enumtype = (CodeEnum)element;
                                ProcessCodeBody(enumtype.GetStartPoint(), enumtype.GetEndPoint(), regex, SortData, persianRegex);
                            }

                        }
                    }

                    // ذخیره در فایل متنی
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
        static void ProcessCodeBody(TextPoint start, TextPoint end, Regex regex, HashSet<string> SortData, Regex persianRegex)
        {
            EditPoint editStart = start.CreateEditPoint();
            string body = editStart.GetText(end);
            string[] lines = body.Split('\n');
            Regex regexForBraket = new Regex(@"\[(.*?)\]");


            foreach (string line in lines)
            {
                var matches = regex.Matches(line);
                foreach (Match match in matches)
                {
                    string value = match.Groups[1].Value.Replace("ـ","");
                    if (!string.IsNullOrWhiteSpace(value) && persianRegex.IsMatch(value))
                    {
                        if (value.Contains("[") && value.Contains("]"))
                        {
                            foreach (Match m in regexForBraket.Matches(value))
                            {
                                SortData.Add(m.Groups[1].Value);
                            }
                        }
                        else { 
                            SortData.Add(value);
                        }
                    }
                }
            }
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
                                                    var value = match.Groups[1].Value.Trim();

                                                    if (!value.EndsWith(":"))
                                                        continue;

                                                    string replaced = "[" + value.TrimEnd(':').Trim() + "]:";
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
