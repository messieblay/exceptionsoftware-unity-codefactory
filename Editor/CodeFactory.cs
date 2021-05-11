using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace ExceptionSoftware.CodeFactory
{
    public static class CodeFactory
    {

        public static void CreateScripts<T>(params T[] templates) where T : Template
        {
            foreach (T template in templates)
            {

                if (template == null)
                {
                    Debug.LogError($"[CodeFactory] Template {typeof(T).Name} not created by: template is null");
                    continue;
                }

                if (!template.relativePath.StartsWith("Assets/"))
                {
                    Debug.LogError($"[CodeFactory] Template {typeof(T).Name} not created by: relativePath not begins with Assets/");
                    continue;
                }

                string finalPath = FileUtils.ConvertRelativePathToAbsolute(template.relativePath);

                //Creacion de directorio
                Directory.CreateDirectory(finalPath);

                List<TemplateFile> files = new List<TemplateFile>();
                template.GetFiles(files);

                string filesCreated = string.Empty;
                foreach (var file in files)
                {
                    //Reemplazar fichero
                    string finalFilePath = FileUtils.ConcatPaths(finalPath, file.className + ".cs");
                    filesCreated += file.className + "\n";
                    File.WriteAllText(finalFilePath, file.BuildCode());
                }


                Debug.Log($"[CodeFactory] Template {typeof(T).Name} created in {template.relativePath}\n{filesCreated}");
            }

            UnityEditor.AssetDatabase.Refresh();
        }
    }
}
