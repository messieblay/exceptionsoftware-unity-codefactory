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
                    Debug.Log("[CodeFactory] template is null");
                    return;
                }

                //Creacion de directorio
                Debug.Log("Application.dataPath: " + Application.dataPath);
                Directory.CreateDirectory(template.rootPath);

                List<TemplateFile> files = new List<TemplateFile>();
                template.GetFiles(files);

                string filesCreated = string.Empty;
                foreach (var file in files)
                {
                    //Reemplazar fichero
                    string finalPath = template.rootPath + "/" + file.className + ".cs";
                    filesCreated += file.className + "\n";
                    File.WriteAllText(finalPath, file.BuildCode());
                }


                Debug.Log($"[CodeFactory] Template {typeof(T).Name} created in {template.rootPath}\n{filesCreated}");
            }

            UnityEditor.AssetDatabase.Refresh();
        }
    }
}
