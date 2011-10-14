﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.UI;
using Autodesk.Revit.DB;
using System.Diagnostics;
using Microsoft.Scripting;
using System.Threading;
using System.Windows.Threading;

namespace RevitPythonShell
{
    /// <summary>
    /// An object of this class is instantiated every time the user clicks on the
    /// button for opening the shell.
    /// </summary>
    /// 
    [Regeneration(RegenerationOption.Manual)]
    [Transaction(TransactionMode.Manual)]
    public class IronPythonConsoleCommand : IExternalCommand
    {
        /// <summary>
        /// Open a window to let the user enter python code.
        /// </summary>
        /// <returns></returns>
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {            
            var messageCopy = message;
            var gui = new IronPythonConsole();
            gui.consoleControl.WithConsoleHost((host) =>
            {
                // now that the console is created and initialized, the script scope should
                // be accessible...
                new ScriptExecutor(commandData, messageCopy, elements).SetupEnvironment(host.Engine, host.Console.ScriptScope);

                // run the initscript
                var initScript = RevitPythonShellApplication.GetInitScript();
                if (initScript != null)
                {
                    var scriptSource = host.Engine.CreateScriptSourceFromString(initScript, SourceCodeKind.Statements);
                    scriptSource.Execute(host.Console.ScriptScope);
                }                
            });
            gui.consoleControl.SetDispatcherWindow(gui);
            gui.ShowDialog();
            return Result.Succeeded;
        }
    }    
}
