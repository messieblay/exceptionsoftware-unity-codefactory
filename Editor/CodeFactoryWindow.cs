using ExceptionSoftware.ExEditor;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace ExceptionSoftware.CodeFactory
{
    public class CodeFactoryWindow : ExWindow<CodeFactoryWindow>
    {
        [MenuItem("Tools/Code factory", priority = 3000)]
        public static CodeFactoryWindow GetWindow()
        {
            var window = EditorWindow.GetWindow<CodeFactoryWindow>();
            window.titleContent = new GUIContent("Code Factory");
            window.Focus();
            window.Repaint();
            return window;
        }

        public override void DoGUI()
        {
            DrawToolbar();
            DrawBody();
        }

        protected override void DoEnable()
        {
            templatesTypes = ExReflect.GetDerivedClassesAllAsseblys<Template>().OrderBy(s => s.FullName).ToList();
            templatesNames = templatesTypes?.Select(s => s.Name.Replace("Template", "")).ToArray();
            templateSelected = templatesTypes.First();
        }

        protected override void DoRecompile() => DoEnable();

        #region Layout
        Rect[] _lv = null;
        Rect _rtoolbar;
        Rect _rbody;

        protected override void DoLayout()
        {
            _lv = position.CopyToZero().Split(SplitMode.Vertical, 20, -1);
            _rtoolbar = _lv[0];
            _rbody = _lv[1];
        }
        #endregion

        [SerializeField] string rootPath = string.Empty;
        [SerializeField] List<Type> templatesTypes = null;
        [SerializeField] List<Template> templates = new List<Template>();
        [SerializeField] string[] templatesNames = null;
        [SerializeField] int templateIndex = 0;
        [SerializeField] Type templateSelected = null;
        void DrawToolbar()
        {
            GUILayout.BeginArea(_rtoolbar, EditorStyles.toolbar);
            {
                EditorGUILayout.BeginHorizontal();
                {
                    if (templatesNames != null)
                    {
                        EditorGUI.BeginChangeCheck();
                        templateIndex = EditorGUILayout.Popup(templateIndex, templatesNames, EditorStyles.toolbarPopup);
                        if (EditorGUI.EndChangeCheck())
                        {
                            templateSelected = templatesTypes[templateIndex];
                            templates = new List<Template>();
                        }
                    }

                    GUILayout.FlexibleSpace();

                    GUI.enabled = templates.Count > 0;
                    if (GUILayout.Button("Create scripts", EditorStyles.toolbarButton))
                    {
                        CodeFactory.CreateScripts(templates.ToArray());
                    }
                    GUI.enabled = true;
                }
                EditorGUILayout.EndHorizontal();
            }
            GUILayout.EndArea();
        }


        void DrawRootPath()
        {
            EditorGUILayout.LabelField(Application.dataPath);
            EditorGUILayout.BeginHorizontal();
            {
                rootPath = EditorGUILayout.DelayedTextField("Root path", rootPath);
                if (GUILayout.Button("Explore", GUILayout.Width(70)))
                {
                    rootPath = EditorUtility.OpenFolderPanel("Root path", rootPath, "");
                }
            }
            EditorGUILayout.EndHorizontal();
        }
        Vector2 _scroll;
        void DrawBody()
        {
            GUILayout.BeginArea(_rbody);
            {
                using (new EditorGUILayout.ScrollViewScope(_scroll))
                {
                    DrawRootPath();
                    ExGUI.Separator();
                    GUI.enabled = rootPath != string.Empty;
                    EditorGUILayout.BeginHorizontal();
                    {
                        EditorGUILayout.LabelField("Templates");
                        if (GUILayout.Button("+", GUILayout.Width(40)))
                        {
                            CreateNewTemplate();
                        }
                    }
                    EditorGUILayout.EndHorizontal();
                    GUI.enabled = true;

                    foreach (var t in templates)
                    {
                        EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                        {
                            DrawTemplate(t);

                            GUI.color = Color.red;
                            if (GUILayout.Button("Remove"))
                            {
                                templates.Remove(t);
                                break;
                            }
                            GUI.color = Color.white;
                        }
                        EditorGUILayout.EndVertical();
                    }
                }
            }
            GUILayout.EndArea();
        }

        void CreateNewTemplate()
        {
            templates.Add((Template)System.Activator.CreateInstance(templateSelected, rootPath));
        }

        void DrawTemplate(Template t)
        {

            foreach (var f in t.GetType().GetFields())
            {
                if (f.Name.ToLower().Contains("rootpat")) continue;
                if (f.FieldType.IsArray)
                {
                    EditorGUI.BeginChangeCheck();
                    EditorGUILayout.LabelField(f.Name);
                    var valueArray = f.GetValue(t) as string[];

                    var text = EditorGUILayout.TextArea(ConvertToString(valueArray), GUILayout.Height(60));
                    if (EditorGUI.EndChangeCheck())
                    {
                        f.SetValue(t, text.Split('\n'));
                    }
                }
                else if (f.FieldType == typeof(string))
                {
                    EditorGUI.BeginChangeCheck();
                    var text = EditorGUILayout.DelayedTextField(f.Name, f.GetValue(t) as string);
                    if (EditorGUI.EndChangeCheck())
                    {
                        f.SetValue(t, text);
                    }
                }

            }



            string ConvertToString(string[] array)
            {
                string val = string.Empty;
                if (array != null)
                {
                    for (int i = 0; i < array.Length; i++)
                    {
                        val += array[i];
                        if (i < array.Length - 1)
                        {
                            val += "\n";
                        }
                    }
                }
                return val;
            }
        }
    }
}
