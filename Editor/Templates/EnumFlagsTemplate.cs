using System.Collections.Generic;

namespace ExceptionSoftware.CodeFactory
{
    public class EnumFlagsTemplate : Template
    {
        public string namespaces = string.Empty;
        public string className;
        public string[] enums;
        public EnumFlagsTemplate(string rootPath) : base(rootPath)
        {
        }
        public EnumFlagsTemplate(string rootPath, string className, params string[] enums) : this(rootPath)
        {
            this.enums = enums;
            this.className = className;
        }
        public override void GetFiles(List<TemplateFile> files)
        {
            TemplateFile file = new TemplateFile();
            file.classAttributes = new string[] { "Flags" };
            file.classType = "enum";
            file.className = className;
            file.namespaces = namespaces;
            file.usings = new string[] { "System" };

            file.body += delegate (Writer writer)
            {
                string line;
                writer.WriteLine("None = 0,");

                for (int i = 0; i < enums.Length; i++)
                {
                    if (enums[i].IsNullOrEmpty()) continue;

                    writer.WriteLine($"{enums[i]} = 1<<{i},");
                }
                writer.WriteLine("All = ~0");
            };

            files.Add(file);
        }
    }
}
