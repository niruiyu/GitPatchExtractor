using GitPatchExtractor.Properties;
using Microsoft.Office.Interop.Outlook;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace GitPatchExtractor
{
    class PatchExtractor
    {

        private static string GetDestinationFolder()
        {
            FolderBrowserDialog folderBrowser = new FolderBrowserDialog();
            folderBrowser.Description = Resources.SpecifyDestinationFolderPrompt;
            folderBrowser.RootFolder = System.Environment.SpecialFolder.MyComputer;
            folderBrowser.SelectedPath = Properties.Settings.Default.LastSavedFolder;

            while (folderBrowser.ShowDialog() != DialogResult.OK)
            {
                if (MessageBox.Show(
                  Resources.QuestionAbortPatchExtraction, Resources.QuestionAbort, MessageBoxButtons.YesNo, MessageBoxIcon.Question
                  ) == DialogResult.Yes)
                {
                    return null;
                }
            }

            Properties.Settings.Default.LastSavedFolder = folderBrowser.SelectedPath;
            Properties.Settings.Default.Save();

            return folderBrowser.SelectedPath;

        }
        public static bool IsPatch(MailItem mail)
        {
            if (mail.BodyFormat == OlBodyFormat.olFormatPlain)
            {
                return true;
            }

            return false;
        }
        public static void Extract(System.Collections.IEnumerable mails)
        {
            string destinationFolder = GetDestinationFolder();
            if (destinationFolder == null)
            {
                return;
            }

            // Dictionary to hold the patch file name and accordingly mail item.
            Dictionary<string, MailItem> patchMailDictionary = new Dictionary<string, MailItem>();
            // List to hold the existing patch files which may be overwritten.
            List<string> existingFileList = new List<string>();

            foreach (MailItem mail in mails)
            {
                if (mail.BodyFormat == OlBodyFormat.olFormatPlain)
                {
                    // If the mail subjects of multiple selections are the same,
                    //  put number suffix to avoid extracted patches overwrite each other.
                    for (int patchFilePathIndex = 0; ; patchFilePathIndex++)
                    {
                        string patchFileName = GetPatchFileName(mail, patchFilePathIndex);

                        if (!patchMailDictionary.ContainsKey(patchFileName))
                        {
                            // Save the patch file name and mail item for later patch extraction.
                            patchMailDictionary.Add(patchFileName, mail);

                            // Save the existing patch file paths for later overwrite confirmation.
                            if (File.Exists(Path.Combine(destinationFolder, patchFileName)))
                            {
                                existingFileList.Add(patchFileName);
                            }
                            break;
                        }
                    }
                }
            }

            // Confirm overwrite.
            if (existingFileList.Count > 0)
            {
                if (new ExtractResult(
                  "Confirm Overwrite",
                  string.Format("Overwrite the following patch{0} in {1}?",
                                existingFileList.Count == 1 ? "" : "es",
                                destinationFolder
                                ),
                  existingFileList.ToArray(),
                  null,
                  "&Yes", "&No"
                  ).ShowDialog() == DialogResult.No)
                {
                    return;
                }
            }

            // Extract patches.
            List<bool> success = new List<bool>();
            List<string> filePaths = new List<string>();
            foreach (var kv in patchMailDictionary)
            {
                success.Add(
                  Extract(
                    new BufferedStream(
                      new FileStream(Path.Combine(destinationFolder, kv.Key), FileMode.Create)
                    ),
                    kv.Value
                    )
                  );
                filePaths.Add(kv.Key);
            }

            // Show the extraction result.
            if (new ExtractResult(
              "Patch Extracted", string.Format(
              "Patch{0} extracted to {1}:{2}",
              patchMailDictionary.Count == 1 ? " was" : "es were",
              destinationFolder,
              success.IndexOf(false) != -1 ? "\r\nNote: Patch colored in red was extracted with warnings!" : ""
              ),
              filePaths.ToArray(),
              success.ToArray(),
              "&Open Destination Folder", "&Close"
              ).ShowDialog() == DialogResult.Yes)
            {
                new OpenFolderAndSelectItems(
                  destinationFolder,
                  patchMailDictionary.Keys
                  );
            }
        }

        // diff --git a/filepath_1 b/filepath_2
        private static Regex regexPatchFileHeader = new Regex(
          @"^diff\s+--git\s+a(\/.*)\s+b(\/.*)$",
          RegexOptions.Compiled
          );

        //@@ -347,7 +347,8 @@ Field(GNVS,AnyAcc,Lock,Preserve)
        //@@ -1 +1,3 @@ xx
        private static Regex regexPatchHunkAsciiHeader = new Regex(
          @"^@@\s+-(?:\d+,)?(\d+)\s+\+(?:\d+,)?(\d+)\s+@@.*$",
          RegexOptions.Compiled
          );

        //delta ####
        //literal ####
        private static Regex regexPatchHunkBinarySubHeader = new Regex(
          @"^(delta|literal)\s+[0-9]+\s*$",
          RegexOptions.Compiled
          );

        private static Regex regexPatchHunkBinary = new Regex(
          @"^[a-zA-Z]\S+$",
          RegexOptions.Compiled
          );

        private enum PatchState
        {
            PatchStart,
            PatchFileHeader,
            PatchHunkHeader,
            PatchHunk,
            PatchHunkEnd,
            PatchEnd
        }

        private static bool Extract(Stream patchFileStream, MailItem mail)
        {
            bool success = true;
            string mailAddress;
            if (patchFileStream != null)
            {
                if (mail.Sender.AddressEntryUserType == OlAddressEntryUserType.olExchangeUserAddressEntry)
                {
                    mailAddress = mail.Sender.GetExchangeUser().PrimarySmtpAddress;
                }
                else
                {
                    mailAddress = mail.Sender.Address;
                }
                byte[] mailHeader = Encoding.ASCII.GetBytes(
                  string.Format(
                    "From: {0} <{1}>\nDate: {2}\nSubject: {3}\n\n",
                    mail.Sender.Name, mailAddress,
                    mail.SentOn.ToString(), mail.Subject
                    ));
                patchFileStream.Write(mailHeader, 0, mailHeader.Length);
            }

            int addedLines = 0, deletedLines = 0;
            bool ascii = false;
            PatchState patchState = PatchState.PatchStart;
            string filePath = null;
            StringReader sr = new StringReader(mail.Body);
            while (true)
            {
                string line = sr.ReadLine();
                if (line == null)
                {
                    break;
                }

                bool unixEol = true;
                Match m;

                if ((patchState == PatchState.PatchStart || patchState == PatchState.PatchHunkEnd) &&
                    (m = regexPatchFileHeader.Match(line)).Success)
                {
                    // meet "diff --git"
                    patchState = PatchState.PatchFileHeader;
                    unixEol = true;

                    //Debug.Assert (m.Groups[1].ToString () == m.Groups[2].ToString ());
                    //success = (success && m.Groups[1].ToString () == m.Groups[2].ToString ());

                    filePath = m.Groups[2].ToString();

                }
                else if ((patchState == PatchState.PatchFileHeader || patchState == PatchState.PatchHunkEnd) &&
                         ((m = regexPatchHunkAsciiHeader.Match(line)).Success || line == "GIT binary patch"))
                {
                    // meet "@@ ..." or "GIT binary patch"
                    unixEol = true;
                    patchState = PatchState.PatchHunkHeader;

                    if (m.Success)
                    {
                        ascii = true;
                        deletedLines = int.Parse(m.Groups[1].ToString());
                        addedLines = int.Parse(m.Groups[2].ToString());
                    }
                    else
                    {
                        ascii = false;
                    }

                }
                else if ((patchState == PatchState.PatchHunkHeader || patchState == PatchState.PatchHunkEnd) &&
                         regexPatchHunkBinarySubHeader.Match(line).Success)
                {
                    // meet "delta ###" or "literal ###"
                    Debug.Assert(!ascii);
                    success = success && !ascii;

                    patchState = PatchState.PatchHunkHeader;
                    unixEol = true;

                }
                else if (patchState == PatchState.PatchHunkHeader || patchState == PatchState.PatchHunk)
                {
                    patchState = PatchState.PatchHunk;
                    if (ascii)
                    {
                        // meet " xxyy", "+xxyy" "-xxyy" or "\ No newline at end of file"
                        unixEol = FileUseUnixEol(filePath);

                        if (line != @"\ No newline at end of file" && line != "")
                        {
                            if (line[0] == ' ' || line[0] == '+')
                            {
                                addedLines--;
                            }
                            if (line[0] == ' ' || line[0] == '-')
                            {
                                deletedLines--;
                            }
                        }

                        if (addedLines == 0 && deletedLines == 0)
                        {
                            patchState = PatchState.PatchHunkEnd;
                        }
                        success = (success && addedLines >= 0 && deletedLines >= 0);

                    }
                    else
                    {
                        unixEol = true;

                        if (!regexPatchHunkBinary.Match(line).Success)
                        {
                            patchState = PatchState.PatchHunkEnd;
                        }
                    }

                }
                else if (patchState == PatchState.PatchHunkEnd && line == "-- ")
                {
                    unixEol = true;
                    patchState = PatchState.PatchEnd;
                }

                // Seems Outlook has a bug which adds extra empty line in the hunk
                // when the previous line is a long while space line.
                if (patchState == PatchState.PatchHunk || patchState == PatchState.PatchHunkEnd)
                {
                    if (ascii && line == "")
                    {
                        continue;
                    }
                }
                if (patchFileStream != null)
                {
                    byte[] mailBody = Encoding.ASCII.GetBytes(line + (unixEol ? "\n" : "\r\n"));
                    patchFileStream.Write(mailBody, 0, mailBody.Length);
                }
            }

            if (patchFileStream != null)
            {
                WriteVersionInfo(patchFileStream);
                patchFileStream.Flush();
                patchFileStream.Close();
            }

            Debug.Assert(patchState == PatchState.PatchEnd);
            success = (success && patchState == PatchState.PatchEnd);
            return success;
        }

        private static void WriteVersionInfo(Stream patchFileStream)
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            AssemblyName name = assembly.GetName();
            Version version = name.Version;

            byte[] versionInfo = Encoding.ASCII.GetBytes(
              string.Format(
                "{0} {1}.{2}\n",
                name.Name, version.Major, version.Minor
                ));
            patchFileStream.Write(versionInfo, 0, versionInfo.Length);
        }

        private static bool FileUseUnixEol(string filePath)
        {
            switch (Path.GetExtension(filePath))
            {
                case ".sh":
                    return true;
            }

            return false;
        }

        // [EDK2] [PATCH] xxx
        // [EDK2] [Patch V2] xxx
        // [EDK2] [PATCH 2/5] xxx
        // [EDK2] [Patch V2 2/5] xxx
        // [EDK2] [Patch V2 2/5] xxx.
        private static Regex regexSubject = new Regex(
          @"(?:\[EDK2\]\s*)?\[PATCH\s*(?:V\d+\s+)?(?:(\d+)/\d+)?\]\s*(.*?)[.\s]*$",
          RegexOptions.IgnoreCase | RegexOptions.Compiled
          );

        // Convert "[EDK2] [Patch V2 2/5] xxx." to "0002-xxx.patch"
        // Convert "[EDK2] [Patch V2] xxx" to "0001-xxx.patch"
        private static string GetPatchFileName(MailItem mail, int suffixIndex)
        {
            string mailSubject = mail.Subject;
            // Extract the patch index and patch title
            Match match = regexSubject.Match(mailSubject);
            if (match.Success)
            {
                int patchIndex;
                if (!int.TryParse(match.Groups[1].ToString(), out patchIndex))
                {
                    patchIndex = 1;
                }
                mailSubject = string.Format("{0:D4}-{1}", patchIndex, match.Groups[2].ToString());
            }

            // Replace invalid file name char with ' '
            StringBuilder mailSubjectSb = new StringBuilder(mailSubject);
            foreach (char c in Path.GetInvalidFileNameChars())
            {
                mailSubjectSb.Replace(c, ' ');
            }

            // Replace multiple ' ' with single ' '
            for (int i = 1; i < mailSubjectSb.Length;)
            {
                if (mailSubjectSb[i] == ' ' && mailSubjectSb[i - 1] == ' ')
                {
                    mailSubjectSb.Remove(i, 1);
                }
                else
                {
                    i++;
                }
            }

            // Replace ' ' with '-'
            mailSubjectSb.Replace(' ', '-');

            // Append "warn" suffix when Extract() warns.
            if (!Extract(null, mail))
            {
                mailSubjectSb.Append("-warn");
            }

            // Append "-##" to avoid file name confliction
            if (suffixIndex != 0)
            {
                mailSubjectSb.AppendFormat("-{0}", suffixIndex);
            }

            // Append file extension
            mailSubjectSb.Append(".patch");

            return mailSubjectSb.ToString();
        }

    }
}
