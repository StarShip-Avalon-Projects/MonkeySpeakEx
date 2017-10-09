﻿using System.IO;
using System.Text;

namespace Monkeyspeak.Libraries
{
    internal class IO : AbstractBaseLibrary
    {
        private string DefaultAuthorizedPath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal);

        public IO(string authorizedPath = null)
        {
            if (!string.IsNullOrEmpty(authorizedPath)) DefaultAuthorizedPath = authorizedPath;

            // (1:200) and the file {...} exists,
            Add(new Trigger(TriggerCategory.Condition, 200), FileExists,
                "(1:200) and the file {...} exists,");

            // (1:201) and the file {...} does not exist,
            Add(new Trigger(TriggerCategory.Condition, 201), FileNotExists,
                "(1:201) and the file {...} does not exist,");

            // (1:202) and the file {...} can be read from,
            Add(new Trigger(TriggerCategory.Condition, 202), CanReadFile,
                "(1:202) and the file {...} can be read from,");

            // (1:203) and the file {...} can be written to,
            Add(new Trigger(TriggerCategory.Condition, 203), CanWriteFile,
                "(1:203) and the file {...} can be written to,");

            // (5:200) append {...} to file {...}.
            Add(new Trigger(TriggerCategory.Effect, 200), AppendToFile,
                "(5:200) append {...} to file {...}.");

            // (5:201) read from file {...} and put it into variable %Variable.
            Add(new Trigger(TriggerCategory.Effect, 201), ReadFileIntoVariable,
                "(5:201) read from file {...} and put it into variable %Variable.");

            // (5:202) delete file {...}.
            Add(new Trigger(TriggerCategory.Effect, 202), DeleteFile,
                "(5:202) delete file {...}.");

            //(5:203) create file {...}.
            Add(new Trigger(TriggerCategory.Effect, 203), CreateFile,
                "(5:203) create file {...}.");
        }

        public override void OnPageDisposing(Page page)
        {
        }

        private bool AppendToFile(TriggerReader reader)
        {
            string data = reader.ReadString();
            string file = reader.ReadString();

            using (var streamWriter = new StreamWriter(Path.Combine(DefaultAuthorizedPath, file), true))
            {
                streamWriter.WriteLine(data);
            }

            return true;
        }

        private bool CanReadFile(TriggerReader reader)
        {
            string file = reader.ReadString();
            try
            {
                using (var stream = File.Open(Path.Combine(DefaultAuthorizedPath, file), FileMode.Open, FileAccess.Read))
                {
                    return true;
                }
            }
            catch // (UnauthorizedAccessException ex)
            {
                return false;
            }
        }

        private bool CanWriteFile(TriggerReader reader)
        {
            string file = reader.ReadString();
            try
            {
                using (var stream = File.Open(Path.Combine(DefaultAuthorizedPath, file), FileMode.Open, FileAccess.Write))
                {
                    return true;
                }
            }
            catch // (UnauthorizedAccessException ex)
            {
                return false;
            }
        }

        private bool CreateFile(TriggerReader reader)
        {
            if (!reader.PeekString()) return false;
            string file = reader.ReadString();
            File.CreateText(Path.Combine(DefaultAuthorizedPath, file)).Close();
            return true;
        }

        private bool DeleteFile(TriggerReader reader)
        {
            if (!reader.PeekString()) return false;
            string file = reader.ReadString();
            File.Delete(Path.Combine(DefaultAuthorizedPath, file));
            return true;
        }

        private bool FileExists(TriggerReader reader)
        {
            string file = (reader.PeekString()) ? reader.ReadString() : "";
            return File.Exists(file);
        }

        private bool FileNotExists(TriggerReader reader)
        {
            return !FileExists(reader);
        }

        private bool ReadFileIntoVariable(TriggerReader reader)
        {
            string file = reader.ReadString();
            var var = reader.ReadVariable(true);
            StringBuilder sb = new StringBuilder();
            using (var stream = File.Open(Path.Combine(DefaultAuthorizedPath, file), FileMode.Open, FileAccess.Read))
            {
                using (var streamReader = new StreamReader(stream))
                {
                    sb.Append(streamReader.ReadToEnd());
                }
            }
            var.Value = sb.ToString();
            return true;
        }
    }
}