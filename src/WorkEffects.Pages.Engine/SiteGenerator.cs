using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace WorkEffects.Pages.Engine
{
    public class SiteGenerator
    {
        private string _contentPath, _outputPath;
        public List<string> menu_field = new List<string>();
        public SiteGenerator(string contentPath, string outputPath)
        {
            _contentPath = contentPath;
            _outputPath = outputPath;
        }

        public void Run()
        {
            var sourcePath = new DirectoryInfo(Path.Combine(_contentPath, "content"));
            var outputPath = new DirectoryInfo(_outputPath);

            if (!sourcePath.Exists)
            {
                EmitMessage("Source Path Does Not Exist");
                return;
            }

            EmitMessage(string.Format("Getting Content From: {0}", sourcePath.Name));
            EmitMessage(string.Format("Writing Site To     : {0}", outputPath.Name));

            
            var templatePath = new DirectoryInfo(Path.Combine(_contentPath, "templates"));

            var templateFile = templatePath.GetFiles("*.html").First();
            EmitMessage(string.Format("Using template: {0}", templateFile.Name));
            string templateContent;

            using (var stream = templateFile.OpenText())
            {
                templateContent = stream.ReadToEnd();
            }

            var folders = ProcessFolders(sourcePath);
            folders.Add(sourcePath);

            var nodes = new Dictionary<string,FileInfo>();

            foreach (var folder in folders)
            {

                foreach (var file in folder.GetFiles())
                {
                    string folder_path = folder.FullName.Replace(sourcePath.FullName,"");
                    nodes.Add(folder_path, file);
                }
            }

            foreach (var d in nodes)
            {
                string result;
                
                using (var stream = d.Value.OpenText())
                {
                    var streamContent = stream.ReadToEnd();
                    result = processHtml(templateContent, streamContent);
                }

                if (!outputPath.Exists) outputPath.Create();
                var outputPathFile = outputPath.FullName + d.Key;
                var path = new System.IO.DirectoryInfo(outputPathFile);
                
                if (!path.Exists) path.Create();

                var filePath = Path.Combine(outputPathFile, d.Value.Name);
                using (var stream = new FileStream(filePath, FileMode.OpenOrCreate, FileAccess.Write))
                using (var writer = new StreamWriter(stream))
                {
                    writer.Write(result);
                }
            }



        }
        static List<DirectoryInfo> folders = new List<DirectoryInfo>();
        
        public List<DirectoryInfo> ProcessFolders(DirectoryInfo path, string outputpath)
        {
            foreach(var d in path.GetDirectories())
            {
                folders.Add(d);
                menu_field.Add("<li>" + d.Name + "<ul>");
                ProcessFolders(d, outputpath);
                menu_field.Add("</ul>");
                menu_field.Add("</li>");
            }
            return folders;
        }

        public string processHtml(string templateContent, string streamContent)
        {
            string result = templateContent;
            result = result.Replace("@menu", string.Join("\n",menu_field.ToArray()));
            result = result.Replace("@body", streamContent);
            return result;
        }

        public Action<String> EmitMessage { get; set; }
    }
}
