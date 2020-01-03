using Microsoft.Office.Interop.Outlook;
using Outlook = Microsoft.Office.Interop.Outlook;

namespace GitPatchExtractor
{
    public partial class ThisAddIn
    {
        private Outlook.Inspectors allInspectors;
        private ContextMenus contextMenus;
        protected override Microsoft.Office.Core.IRibbonExtensibility CreateRibbonExtensibilityObject()
        {
            contextMenus = new ContextMenus();
            return contextMenus;
        }
        private void ThisAddIn_Startup(object sender, System.EventArgs e)
        {
            allInspectors = Application.Inspectors;
            allInspectors.NewInspector += Inspectors_NewInspector;
        }

        private void Inspectors_NewInspector(Outlook.Inspector Inspector)
        {
            // Cause Ribbon.GetVisible() be called everytime ReadMail inspector pops up.
            if (Inspector.CurrentItem is Outlook.MailItem)
            {
                contextMenus.Invalidate();
            }

        }

        private void ThisAddIn_Shutdown(object sender, System.EventArgs e)
        {
            // Note: Outlook no longer raises this event. If you have code that 
            //    must run when Outlook shuts down, see http://go.microsoft.com/fwlink/?LinkId=506785
        }

        #region VSTO generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InternalStartup()
        {
            this.Startup += new System.EventHandler(ThisAddIn_Startup);
            this.Shutdown += new System.EventHandler(ThisAddIn_Shutdown);
        }

        #endregion
    }
}
