using Microsoft.Office.Interop.Outlook;
using System;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using Office = Microsoft.Office.Core;

// TODO:  Follow these steps to enable the Ribbon (XML) item:

// 1: Copy the following code block into the ThisAddin, ThisWorkbook, or ThisDocument class.

//  protected override Microsoft.Office.Core.IRibbonExtensibility CreateRibbonExtensibilityObject()
//  {
//      return new Ribbon1();
//  }

// 2. Create callback methods in the "Ribbon Callbacks" region of this class to handle user
//    actions, such as clicking a button. Note: if you have exported this Ribbon from the Ribbon designer,
//    move your code from the event handlers to the callback methods and modify the code to work with the
//    Ribbon extensibility (RibbonX) programming model.

// 3. Assign attributes to the control tags in the Ribbon XML file to identify the appropriate callback methods in your code.  

// For more information, see the Ribbon XML documentation in the Visual Studio Tools for Office Help.


namespace GitPatchExtractor
{
    [ComVisible(true)]
    public class ContextMenus : Office.IRibbonExtensibility
    {
        private Bitmap bmpExtract;
        private Office.IRibbonUI ribbonThis;

        public ContextMenus()
        {
            bmpExtract = Properties.Resources.Extract;
            bmpExtract.MakeTransparent(Color.White);
        }
        public void Invalidate()
        {
            if (ribbonThis != null)
            {
                ribbonThis.Invalidate();
            }
        }

        #region IRibbonExtensibility Members
        public string GetCustomUI(string ribbonID)
        {
            string xmlResource = "";
            switch (ribbonID)
            {
                case "Microsoft.Outlook.Mail.Read":
                    xmlResource = "GitPatchExtractor.Resources.Ribbon.xml";
                    break;
                case "Microsoft.Outlook.Explorer":
                    xmlResource = "GitPatchExtractor.Resources.ContextMenus.xml";
                    break;
            }

            return GetResourceText(xmlResource);
        }
        #endregion

        #region Ribbon Callbacks
        //Create callback methods here. For more information about adding callback methods, visit http://go.microsoft.com/fwlink/?LinkID=271226
        public bool GetVisible(Office.IRibbonControl control)
        {
            foreach (MailItem mail in Globals.ThisAddIn.Application.ActiveExplorer().Selection)
            {
                if (PatchExtractor.IsPatch(mail))
                {
                    return true;
                }
            }
            return false;
        }

        public void OnLoad(Office.IRibbonUI ribbon)
        {
            ribbonThis = ribbon;
        }
        public void OnExtractPatch(Office.IRibbonControl control)
        {
            PatchExtractor.Extract(Globals.ThisAddIn.Application.ActiveExplorer().Selection);
        }

        public Bitmap GetCustomImage(Office.IRibbonControl control)
        {
            return bmpExtract;
        }
        #endregion

        #region Helpers
        private static string GetResourceText(string resourceName)
        {
            Assembly asm = Assembly.GetExecutingAssembly();
            string[] resourceNames = asm.GetManifestResourceNames();
            for (int i = 0; i < resourceNames.Length; ++i)
            {
                if (string.Compare(resourceName, resourceNames[i], StringComparison.OrdinalIgnoreCase) == 0)
                {
                    using (StreamReader resourceReader = new StreamReader(asm.GetManifestResourceStream(resourceNames[i])))
                    {
                        if (resourceReader != null)
                        {
                            return resourceReader.ReadToEnd();
                        }
                    }
                }
            }
            return null;
        }
        #endregion
    }
}
