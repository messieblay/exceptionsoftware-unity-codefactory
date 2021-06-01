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
                string finalFilePath;
                foreach (var file in files)
                {
                    //Reemplazar fichero
                    finalFilePath = FileUtils.ConcatPaths(finalPath, file.className + ".cs");
                    filesCreated += file.className + "\n";
                    File.WriteAllText(finalFilePath, file.BuildCode());
                }

                Debug.Log($"[CodeFactory] {typeof(T).Name} created in {template.relativePath}\n{filesCreated}");
            }

            UnityEditor.AssetDatabase.Refresh();
        }



        public static string[] GenerateEnumContent(List<string> enumList)
        {
            List<string> content = new List<string>();
            if (enumList.Count > 0)
            {
                for (int i = 0; i < enumList.Count; i++)
                {
                    content.Add(ValidName(enumList[i]));
                }
            }
            return content.ToArray();

            string ValidName(string sceneName)
            {
                return sceneName.Replace(" ", "");
            }
        }
    }
}
