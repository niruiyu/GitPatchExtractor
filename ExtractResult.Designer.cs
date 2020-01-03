namespace GitPatchExtractor
{
  partial class ExtractResult
  {
    /// <summary>
    /// Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    /// <summary>
    /// Clean up any resources being used.
    /// </summary>
    /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
    protected override void Dispose(bool disposing)
    {
      if (disposing && (components != null))
      {
        components.Dispose();
      }
      base.Dispose(disposing);
    }

    #region Windows Form Designer generated code

    /// <summary>
    /// Required method for Designer support - do not modify
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
      this.btnLeft = new System.Windows.Forms.Button();
      this.btnRight = new System.Windows.Forms.Button();
      this.textMessage = new System.Windows.Forms.RichTextBox();
      this.SuspendLayout();
      // 
      // btnLeft
      // 
      this.btnLeft.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this.btnLeft.DialogResult = System.Windows.Forms.DialogResult.Yes;
      this.btnLeft.Location = new System.Drawing.Point(339, 363);
      this.btnLeft.Name = "btnLeft";
      this.btnLeft.Size = new System.Drawing.Size(186, 32);
      this.btnLeft.TabIndex = 1;
      this.btnLeft.Text = "&Left";
      this.btnLeft.UseVisualStyleBackColor = true;
      // 
      // btnRight
      // 
      this.btnRight.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this.btnRight.DialogResult = System.Windows.Forms.DialogResult.No;
      this.btnRight.Location = new System.Drawing.Point(531, 363);
      this.btnRight.Name = "btnRight";
      this.btnRight.Size = new System.Drawing.Size(186, 32);
      this.btnRight.TabIndex = 1;
      this.btnRight.Text = "&Right";
      this.btnRight.UseVisualStyleBackColor = true;
      // 
      // textMessage
      // 
      this.textMessage.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.textMessage.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.textMessage.Location = new System.Drawing.Point(14, 14);
      this.textMessage.Margin = new System.Windows.Forms.Padding(5);
      this.textMessage.Name = "textMessage";
      this.textMessage.ReadOnly = true;
      this.textMessage.Size = new System.Drawing.Size(701, 341);
      this.textMessage.TabIndex = 2;
      this.textMessage.Text = "";
      // 
      // ExtractResult
      // 
      this.AcceptButton = this.btnLeft;
      this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.CancelButton = this.btnRight;
      this.ClientSize = new System.Drawing.Size(729, 407);
      this.ControlBox = false;
      this.Controls.Add(this.textMessage);
      this.Controls.Add(this.btnRight);
      this.Controls.Add(this.btnLeft);
      this.KeyPreview = true;
      this.MaximizeBox = false;
      this.MinimizeBox = false;
      this.MinimumSize = new System.Drawing.Size(441, 250);
      this.Name = "ExtractResult";
      this.ShowIcon = false;
      this.ShowInTaskbar = false;
      this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
      this.Text = "Title";
      this.ResumeLayout(false);

    }

    #endregion
    private System.Windows.Forms.Button btnLeft;
    private System.Windows.Forms.Button btnRight;
    private System.Windows.Forms.RichTextBox textMessage;
  }
}