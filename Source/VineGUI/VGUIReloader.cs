using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace RVCRestructured.VineGUI
{
    /// <summary>
    /// Build UI in .cs files, and reload them using this class.
    /// </summary>
    public static class VGUIReloadable
    {
        private static string dirPath;
        private static bool initalized = false;
        public static void Init(string path)
        {
            if(initalized) return;
            initalized = true;
            dirPath = $"{path}/../VGUI/";

        }
        private static Dictionary<string,KeyValuePair<MethodInfo, object>> windowInfo = new Dictionary<string, KeyValuePair<MethodInfo, object>>();
        
        /// <summary>
        /// Attempt to load and call a gui class written in the directiory Rimworld/VGUI/
        /// NOTE: ONLY RUN CODE YOU WROTE USING THIS METHOD.
        /// </summary>
        /// <param name="name">The file name. Do NOT include the .cs ending.</param>
        /// <param name="guiArea">The GUI rect.</param>
        public static void OnGUI(string name, Rect guiArea)
        {
            if (!windowInfo.ContainsKey(name))
            {
                object instance;
                MethodInfo info = LoadGUIInfo(name,out instance);
                if (info == null) return;
                windowInfo[name] = new KeyValuePair<MethodInfo, object>(info,instance);
            }
            KeyValuePair<MethodInfo,object> pair = windowInfo[name];
            pair.Key.Invoke(pair.Value, new object[] { guiArea });
        }
        
        
        private static MethodInfo LoadGUIInfo(string name, out object instance)
        {
            instance = null;
            Assembly asm = GetFile(name);
            if(asm == null) return null;
            Type window = asm.GetType("Window");
            instance=Activator.CreateInstance(window);
            MethodInfo info = window.GetMethod("OnGUI", BindingFlags.Public | BindingFlags.Instance);

            return info;
        }




        private static Assembly GetFile(string name)
        {
            if(!Directory.Exists(dirPath)) Directory.CreateDirectory(dirPath);
            FileInfo file = new FileInfo($"{dirPath}{name}.cs");
            string compiledName = $"{dirPath}{name}.dll";
            bool compiled = Compiler(file, compiledName);
            if (!compiled) return null;

            Assembly asm = Assembly.LoadFrom(compiledName);
            return asm;

        }

        private static bool Compiler(FileInfo file, string outputName)
        {
            CompilerParameters parameters = new CompilerParameters()
            {
                GenerateExecutable = true,
                GenerateInMemory = false,
                OutputAssembly = outputName
            };

            CodeDomProvider codeDomProvider = CodeDomProvider.CreateProvider("CSharp");
            CompilerResults results = codeDomProvider.CompileAssemblyFromFile(parameters, file.FullName);
            if (results.Errors.Count > 0)
            {
                foreach (var error in results.Errors)
                {
                    Log.Error($"Error compiling: {error}");
                }
                return false;
            }
            return true;
        }
    }
}
