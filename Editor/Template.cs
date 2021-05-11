using System.Collections.Generic;

namespace ExceptionSoftware.CodeFactory
{
    [System.Serializable]
    public abstract class Template
    {
        public string rootPath;
        public Template(string rootPath)
        {
            this.rootPath = rootPath;
        }
        public abstract void GetFiles(List<TemplateFile> files);

    }
}
