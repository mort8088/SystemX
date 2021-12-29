using System;
using Microsoft.Xna.Framework.Input;

namespace SystemX.Input
{
    public class KeyEventArgs : EventArgs
    {
        private readonly Keys _keyCode;

        public KeyEventArgs(Keys keyCode)
        {
            this._keyCode = keyCode;
        }

        public Keys KeyCode
        {
            get { return _keyCode; }
        }
    }
}