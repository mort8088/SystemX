 using Microsoft.Xna.Framework;

namespace SystemX.GUI.Controls {
    public class CheckBox : Control {
        public override string KeyRef {
            get {
                return "CHECKBOX";
            }
        }

        public bool Checked { get; set; }

        public override void OnLeftClick(Point pos) {
            Checked = !Checked;

            base.OnLeftClick(pos);
        }
    }
}