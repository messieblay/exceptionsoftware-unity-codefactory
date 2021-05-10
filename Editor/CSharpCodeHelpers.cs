using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

////REVIEW: this seems like it should be #if UNITY_EDITOR

namespace ExceptionSoftware.CodeFactory
{
    public static class CSharpCodeHelpers
    {

        public static bool IsProperIdentifier(string name)
        {
            if (string.IsNullOrEmpty(name))
                return false;

            if (char.IsDigit(name[0]))
                return false;

            for (var i = 0; i < name.Length; ++i)
            {
                var ch = name[i];
                if (!char.IsLetterOrDigit(ch) && ch != '_')
                    return false;
            }

            return true;
        }

        public static bool IsEmptyOrProperIdentifier(string name)
        {
            if (string.IsNullOrEmpty(name))
                return true;

            return IsProperIdentifier(name);
        }

        public static bool IsEmptyOrProperNamespaceName(string name)
        {
            if (string.IsNullOrEmpty(name))
                return true;

            return name.Split('.').All(IsProperIdentifier);
        }

        ////TODO: this one should add the @escape automatically so no other code has to worry
        public static string MakeIdentifier(string name, string suffix = "", bool firstCharacterLow = false)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException(nameof(name));

            if (char.IsDigit(name[0]))
                name = "_" + name;

            if (firstCharacterLow)
            {
                name = char.ToLower(name[0]) + name.Substring(1);
            }

            // See if we have invalid characters in the name.
            var nameHasInvalidCharacters = false;
            for (var i = 0; i < name.Length; ++i)
            {
                var ch = name[i];
                if (!char.IsLetterOrDigit(ch) && ch != '_')
                {
                    nameHasInvalidCharacters = true;
                    break;
                }
            }

            // If so, create a new string where we remove them.
            if (nameHasInvalidCharacters)
            {
                var buffer = new StringBuilder();
                for (var i = 0; i < name.Length; ++i)
                {
                    var ch = name[i];
                    if (char.IsLetterOrDigit(ch) || ch == '_')
                        buffer.Append(ch);
                }

                name = buffer.ToString();
            }

            return name + suffix;
        }

        public static string MakeTypeName(string name, string suffix = "")
        {
            var symbolName = MakeIdentifier(name, suffix);
            if (char.IsLower(symbolName[0]))
                symbolName = char.ToUpper(symbolName[0]) + symbolName.Substring(1);
            return symbolName;
        }



        public static ClassReaded GetNestedClassCode(Type t)
        {
            string path = FileUtils.LocateFile($"{t.Name}.cs");
            string log = t.Name + "\n";
            log += t + "\n";
            log += path + "\n";

            string fileText = File.ReadAllText(path);

            ClassReaded classReaded = new ClassReaded();

            //Namespaces
            classReaded.namespaces = fileText.Split('\n').Where(s => s.Trim().StartsWith("using")).ToList();

            //Nested classes
            List<NestedClass> nestedClasses = new List<NestedClass>();
            foreach (var nt in t.GetNestedTypes())
            {
                log += nt.Name + "\n";
                int index = fileText.IndexOf($"class {nt.Name}");
                int contador = 0;
                int indexContenido = fileText.IndexOf("{", index);
                string result = string.Empty;
                for (int x = indexContenido; x < fileText.Length; x++)
                {
                    result += fileText[x];
                    if (fileText[x] == '{')
                    {
                        contador++;
                    }
                    if (fileText[x] == '}')
                    {
                        contador--;

                        if (contador == 0)
                        {
                            result = result.Substring(1);
                            result = result.Remove(result.Length - 1);
                            result = result.Trim('\r', '\n').Trim();
                            //if (result.StartsWith("\n")) { 
                            //result=result.
                            //}
                            nestedClasses.Add(new NestedClass(nt.Name, result));
                            break;
                        }
                    }

                }
            }

            nestedClasses.TrimExcess();
            classReaded.NestedClasses = nestedClasses;
            return classReaded;
        }

    }

    public class ClassReaded
    {
        public List<string> namespaces = new List<string>();
        public List<NestedClass> NestedClasses = new List<NestedClass>();
    }
    public class NestedClass
    {
        public string name;
        public string code;

        public NestedClass(string name, string code)
        {
            this.name = name;
            this.code = code;
        }
    }



    public struct Writer
    {
        public const int kSpacesPerIndentLevel = 4;
        public StringBuilder buffer;
        public int indentLevel;

        public void BeginBlock()
        {
            WriteIndent();
            buffer.Append("{\n");
            ++indentLevel;
        }

        public void EndBlock()
        {
            --indentLevel;
            WriteIndent();
            buffer.Append("}\n");
        }

        public void WriteLine()
        {
            buffer.Append('\n');
        }

        public void WriteLine(string text)
        {
            WriteIndent();
            buffer.Append(text);
            buffer.Append('\n');
        }

        public void Write(string text)
        {
            buffer.Append(text);
        }

        public void WriteIndent()
        {
            for (var i = 0; i < indentLevel; ++i)
            {
                for (var n = 0; n < kSpacesPerIndentLevel; ++n)
                    buffer.Append(' ');
            }
        }
    }
}
