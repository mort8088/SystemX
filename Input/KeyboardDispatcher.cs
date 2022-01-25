using System;
using System.Threading;
using System.Windows;
//using System.Windows.Forms;
using Microsoft.Xna.Framework;

namespace SystemX.Input
{
    public interface IKeyboardDispatcher {
        IKeyboardSubscriber Subscriber { get; set; }
    }

    public class KeyboardDispatcher : IKeyboardDispatcher {
        public KeyboardDispatcher(GameWindow window)
        {
            EventInput.Initialize(window);
            EventInput.CharEntered += EventInput_CharEntered;
            EventInput.KeyDown += EventInput_KeyDown;
        }

        private void EventInput_KeyDown(object sender, KeyEventArgs e)
        {
            if (_subscriber == null)
                return;

            _subscriber.RecieveSpecialInput(e.KeyCode);
        }

        private void EventInput_CharEntered(object sender, CharacterEventArgs e)
        {
            if (_subscriber == null)
                return;

            if (char.IsControl(e.Character))
            {
                //ctrl-v
                if (e.Character == 0x16)
                {
                    //XNA runs in Multiple Thread Apartment state, which cannot receive clipboard
                    Thread thread = new Thread(PasteThread);
                    thread.SetApartmentState(ApartmentState.STA);
                    thread.Start();
                    thread.Join();
                    _subscriber.RecieveTextInput(_pasteResult);
                }
                else
                {
                    _subscriber.RecieveCommandInput(e.Character);
                }
            }
            else
            {
                _subscriber.RecieveTextInput(e.Character);
            }
        }

        private IKeyboardSubscriber _subscriber;
        public IKeyboardSubscriber Subscriber
        {
            get { return _subscriber; }
            set
            {
                if (_subscriber != null)
                    _subscriber.Selected = false;

                _subscriber = value;

                if (value != null)
                    value.Selected = true;
            }
        }

        //Thread has to be in Single Thread Apartment state in order to receive clipboard
        private string _pasteResult = "";

        [STAThread]
        private void PasteThread()
        {
            // TODO: Paste not working yet.
            _pasteResult = "";//Clipboard.ContainsText() ? Clipboard.GetText() : "";
        }
    }
}