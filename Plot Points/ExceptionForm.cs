using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Net.Mail;
using System.Net;
using Autodesk.Revit.UI;

namespace Plot_Points {
  public partial class ExceptionForm : Form {
    protected string report;
    protected bool sending = false;
    protected SmtpClient smtp;
    protected System.Windows.Forms.TextBox reportTextBox;

    public ExceptionForm(Exception e) {
      InitializeComponent();
      this.FormBorderStyle = FormBorderStyle.FixedSingle;

      this.report = e.ToString();
    }

    private void viewReportButton_Click(object sender, EventArgs e) {
      if(reportTextBox == null) {
        this.Height += 156;

        this.reportTextBox = new System.Windows.Forms.TextBox();
        this.reportTextBox.AcceptsReturn = true;
        this.reportTextBox.ReadOnly = true;
        this.reportTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom |
          System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right));
        this.reportTextBox.Location = new System.Drawing.Point(12, this.viewReportButton.Location.Y + this.viewReportButton.Size.Height + 6);
        this.reportTextBox.Multiline = true;
        this.reportTextBox.Name = "reportTextBox";
        this.reportTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
        this.reportTextBox.Size = new System.Drawing.Size(240, 150);
        this.reportTextBox.TabIndex = 8;

        this.viewReportButton.Text = "Hide Report";

        this.reportTextBox.Text = this.report;

        this.Controls.Add(this.reportTextBox);
      }
      else {
        this.Controls.Remove(this.reportTextBox);
        this.Height -= 156;
        this.reportTextBox = null;
        this.viewReportButton.Text = "View Report";
      }
    }

    private void dontSendButton_Click(object sender, EventArgs e) {
      if(sending) {
        this.smtp.SendAsyncCancel();
        this.smtp = null;

        this.sending = false;
        this.sendButton.Enabled = true;
        this.sendButton.Text = "Send";
        this.dontSendButton.Text = "Don't Send";
      }
      else
        this.Close();
    }

    private void sendButton_Click(object sender, EventArgs e) {
      this.sending = true;
      this.sendButton.Enabled = false;
      this.sendButton.Text = "Sending...";
      this.dontSendButton.Text = "Cancel";

      MailMessage message = new MailMessage();
      message.From = new MailAddress("software@ellumus.com", "Ellumus Software");
      message.To.Add(new MailAddress("support@ellumus.com", "Ellumus Support"));
      message.Subject = "Rename Elements Exception";
      if(this.commentsTextBox.Text != "")
        message.Body = this.report + "\n\nAdditional comments:\n" + this.commentsTextBox.Text;
      else
        message.Body = this.report;


      const string fromPassword = "{DD589BAA-E005-4677-8AD3-60B1532FAF23}";

      this.smtp = new SmtpClient {
        Host = "smtp.gmail.com",
        Port = 587,
        EnableSsl = true,
        DeliveryMethod = SmtpDeliveryMethod.Network,
        UseDefaultCredentials = false,
        Credentials = new NetworkCredential(message.From.Address, fromPassword),
      };
      this.smtp.SendCompleted += new SendCompletedEventHandler(smtp_SendCompleted);
      this.smtp.SendAsync(message, null);
    }

    void smtp_SendCompleted(object sender, AsyncCompletedEventArgs e) {
      if(e.Cancelled)
        TaskDialog.Show("Rename Elements", "Sending of error report cancelled");
      else {
        if(e.Error != null)
          TaskDialog.Show("Rename Elements", "Error occurred when sending error report:\n" + e.Error.Message);

        this.sending = false;
        this.smtp = null;

        this.Close();
      }
    }

    private void ExceptionForm_FormClosing(object sender, FormClosingEventArgs e) {
      if(sending) {
        TaskDialogResult result = TaskDialog.Show("Rename Elements", "Cancel sending report?", TaskDialogCommonButtons.Yes | TaskDialogCommonButtons.Cancel);
        if(result != TaskDialogResult.Yes)
          e.Cancel = true;
        else {
          if(this.smtp != null) {
            this.smtp.SendAsyncCancel();
            this.smtp = null;
          }
        }
      }
    }
  }
}
