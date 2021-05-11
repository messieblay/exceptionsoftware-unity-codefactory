using System.Collections.Generic;

namespace ExceptionSoftware.CodeFactory
{
    [System.Serializable]
    public abstract class Template
    {
        /// <summary>
        /// Relative path. Must starts with Assets/
        /// </summary>
        public string relativePath;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="relativePath">Must starts with Assets/</param>
        public Template(string relativePath)
        {
            this.relativePath = relativePath;
        }
        public abstract void GetFiles(List<TemplateFile> files);

    }
}
