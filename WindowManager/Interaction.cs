/// <summery>
/// The whole Mbox replacment was writen by "Stefan Steiger" found here [https://stackoverflow.com/a/55827338/368648]
/// </summery>
namespace SystemX.WindowManager
{
    public static class Interaction
    {
        private static string GetTitleFromAssembly(System.Reflection.Assembly callingAssembly)
        {
            try
            {
                return callingAssembly.GetName().Name;
            }
            catch (System.Security.SecurityException)
            {
                string fullName = callingAssembly.FullName;
                int index = fullName.IndexOf(',');
                if (index >= 0)
                {
                    return fullName.Substring(0, index);
                }
                return "";
            }
        }
        
        public static MsgBoxResult MsgBox(string text, string caption, MsgBoxStyle options)
        {
            if (string.IsNullOrEmpty(caption))
                caption = GetTitleFromAssembly(System.Reflection.Assembly.GetCallingAssembly());

            if (System.Environment.OSVersion.Platform != System.PlatformID.Unix)
                return UnsafeNativeMethods.MessageBox(System.IntPtr.Zero, text, caption, options);

            text = text.Replace("\"", @"\""");
            caption = caption.Replace("\"", @"\""");

            using (System.Diagnostics.Process p = System.Diagnostics.Process.Start("notify-send", "\"" + caption + "\" \"" + text + "\""))
            {
                p.WaitForExit();
            }

            return MsgBoxResult.Ok;
        }

        public static MsgBoxResult MsgBox(string text, string caption)
        {
            return MsgBox(text, caption, MsgBoxStyle.OkOnly);
        }

        public static MsgBoxResult MsgBox(string text)
        {
            return MsgBox(text, null);
        }

        public static MsgBoxResult MsgBox(object objText, object objCaption)
        {
            string text = System.Convert.ToString(objText, System.Globalization.CultureInfo.InvariantCulture);
            string caption = System.Convert.ToString(objCaption, System.Globalization.CultureInfo.InvariantCulture);

            return MsgBox(text, caption);
        }

        public static MsgBoxResult MsgBox(object objText)
        {
            return MsgBox(objText, null);
        }
    }
}