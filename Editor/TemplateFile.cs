using System.Text;

namespace ExceptionSoftware.CodeFactory
{
    public sealed class TemplateFile
    {
        public string[] usings = null;
        public string namespaces = string.Empty;
        public string[] classAttributes = null;
        public string classVisibility = "public";
        public string classType = "class";
        public string className;
        public string extensions = string.Empty;
        public System.Action<Writer> body = null;

        Writer _writer;

        public TemplateFile()
        {
            _writer = new Writer
            {
                buffer = new StringBuilder()
            };
        }
        public string BuildCode()
        {
            //Usings
            if (usings != null)
            {
                foreach (var u in usings)
                {
                    if (u.IsNullOrEmpty()) continue;
                    _writer.WriteLine($"using {u};");
                }
                _writer.WriteLine("");
            }

            //Namespace
            bool havenamespace = !namespaces.IsNullOrEmpty();
            if (havenamespace)
            {
                _writer.WriteLine($"namespace {namespaces}");
                _writer.BeginBlock();
                _writer.WriteLine();
            }

            //Class Atributtes
            if (classAttributes != null)
            {
                foreach (var u in classAttributes)
                {
                    if (u.IsNullOrEmpty()) continue;
                    _writer.WriteLine($"[{u}]");
                }
            }

            //Class header
            string classHeader = $"{classVisibility} {classType} {className}";
            if (!extensions.IsNullOrEmpty())
            {
                classHeader += $" {extensions}";
            }

            _writer.WriteLine(classHeader);

            _writer.BeginBlock();
            {
                if (body != null) body(_writer);
            }
            _writer.EndBlock();
            if (havenamespace)
            {
                _writer.EndBlock();
            }

            return _writer.buffer.ToString();
        }
    }
}
