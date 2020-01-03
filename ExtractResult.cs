using System.Drawing;
using System.Windows.Forms;

namespace GitPatchExtractor
{
    public partial class ExtractResult : Form
    {

        private void AppendText(string text, Color color)
        {
            int start = textMessage.Text.Length;
            textMessage.AppendText(text);
            textMessage.Select(start, text.Length);
            textMessage.SelectionColor = color;
        }
        public ExtractResult(
          string title, string description,
          string[] filePaths, bool[] success, string left, string right
          )
        {
            InitializeComponent();

            Text = title;
            btnLeft.Text = left;
            btnRight.Text = right;
            Font defaultFont = textMessage.Font;
            AppendText(description + "\r\n\r\n", Color.Black);

            for (int i = 0; i < filePaths.Length; i++)
            {
                AppendText(
                  string.Format("  {0}\r\n", filePaths[i]),
                  (success == null || success[i]) ? Color.Black : Color.Red
                  );
            }
        }
    }
}
