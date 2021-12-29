namespace SystemX.WindowManager
{
    // https://docs.microsoft.com/en-us/windows/desktop/api/winuser/nf-winuser-messagebox
    internal class UnsafeNativeMethods 
    {
        [System.Runtime.InteropServices.DllImport("user32.dll")]
        internal static extern MsgBoxResult MessageBox(System.IntPtr hWnd, string text, string caption, MsgBoxStyle options);
    }
}