using System.Collections.Generic;

namespace ExceptionSoftware.CodeFactory
{
    public class EnumTemplate : Template
    {
        public string namespaces = string.Empty;
        public string className;
        public string[] enums;
        public EnumTemplate(string rootPath) : base(rootPath)
        {
        }
        public EnumTemplate(string rootPath, string className, params string[] enums) : this(rootPath)
        {
            this.enums = enums;
            this.className = className;
        }
        public override void GetFiles(List<TemplateFile> files)
        {
            TemplateFile file = new TemplateFile();
            file.classType = "enum";
            file.className = className;
            file.namespaces = namespaces;


            file.body += delegate (Writer writer)
              {
                  string line;
                  for (int i = 0; i < enums.Length; i++)
                  {
                      if (enums[i].IsNullOrEmpty()) continue;
                      line = enums[i];

                      if (i == 0)
                      {
                          line += " =0";
                      }

                      if (i != enums.Length - 1)
                      {
                          line += ",";
                      }

                      writer.WriteLine(line);
                  }
              };

            files.Add(file);
        }
    }
}
