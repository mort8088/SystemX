using System;
using System.Runtime.InteropServices;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace SystemX.Input
{
    public delegate void CharEnteredHandler(object sender, CharacterEventArgs e);
    public delegate void KeyEventHandler(object sender, KeyEventArgs e);

    public static class EventInput
    {
        /// <summary>
        /// Event raised when a character has been entered.
        /// </summary>
        public static event CharEnteredHandler CharEntered;

        /// <summary>
        /// Event raised when a key has been pressed down. May fire multiple times due to keyboard repeat.
        /// </summary>
        public static event KeyEventHandler KeyDown;

        /// <summary>
        /// Event raised when a key has been released.
        /// </summary>
        public static event KeyEventHandler KeyUp;

        delegate IntPtr WndProc(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam);

        static bool _initialized;
        static IntPtr _prevWndProc;
        static WndProc _hookProcDelegate;
        static IntPtr _hImc;

        //various Win32 constants that we need
        const int GWL_WNDPROC = -4;
        const int WM_KEYDOWN = 0x100;
        const int WM_KEYUP = 0x101;
        const int WM_CHAR = 0x102;
        const int WM_IME_SETCONTEXT = 0x0281;
        const int WM_INPUTLANGCHANGE = 0x51;
        const int WM_GETDLGCODE = 0x87;
        const int WM_IME_COMPOSITION = 0x10f;
        const int DLGC_WANTALLKEYS = 4;

        //Win32 functions that we're using
        [DllImport("Imm32.dll", CharSet = CharSet.Unicode)]
        static extern IntPtr ImmGetContext(IntPtr hWnd);

        [DllImport("Imm32.dll", CharSet = CharSet.Unicode)]
        static extern IntPtr ImmAssociateContext(IntPtr hWnd, IntPtr hImc);

        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        static extern IntPtr CallWindowProc(IntPtr lpPrevWndFunc, IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);


        /// <summary>
        /// Initialize the TextInput with the given GameWindow.
        /// </summary>
        /// <param name="window">The XNA window to which text input should be linked.</param>
        public static void Initialize(GameWindow window)
        {
            if (_initialized)
                throw new InvalidOperationException("TextInput.Initialize can only be called once!");

            _hookProcDelegate = HookProc;
            _prevWndProc = (IntPtr)SetWindowLong(window.Handle, GWL_WNDPROC, (int)Marshal.GetFunctionPointerForDelegate(_hookProcDelegate));

            _hImc = ImmGetContext(window.Handle);
            _initialized = true;
        }

        static IntPtr HookProc(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam)
        {
            IntPtr returnCode = CallWindowProc(_prevWndProc, hWnd, msg, wParam, lParam);

            switch (msg)
            {
                case WM_GETDLGCODE:
                    returnCode = (IntPtr)(returnCode.ToInt32() | DLGC_WANTALLKEYS);
                    break;

                case WM_KEYDOWN:
                    if (KeyDown != null)
                        KeyDown(null, new KeyEventArgs((Keys)wParam));
                    break;

                case WM_KEYUP:
                    if (KeyUp != null)
                        KeyUp(null, new KeyEventArgs((Keys)wParam));
                    break;

                case WM_CHAR:
                    if (CharEntered != null)
                        CharEntered(null, new CharacterEventArgs((char)wParam, lParam.ToInt32()));
                    break;

                case WM_IME_SETCONTEXT:
                    if (wParam.ToInt32() == 1)
                        ImmAssociateContext(hWnd, _hImc);
                    break;

                case WM_INPUTLANGCHANGE:
                    ImmAssociateContext(hWnd, _hImc);
                    returnCode = (IntPtr)1;
                    break;
            }

            return returnCode;
        }
    }
}