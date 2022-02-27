namespace SystemX.GUI.Controls
{
    public class Slideshow : Control
    {
        private int _curSlide;

        public override string KeyRef
        {
            get
            {
                return "SLIDESHOW";
            }
        }

        public bool CanAdvance
        {
            get
            {
                return _curSlide < (Visuals.Count - 1);
            }
        }

        public bool CanGoBack
        {
            get
            {
                return _curSlide > 0;
            }
        }

        public override void RegisterVisuals(RenderEngine renderer)
        {
            for (int i = 0; i < Visuals.Count; i++)
            {
                Visuals[i].Owner = this;
                renderer.Visuals.Add(Visuals[i]);

                if (!string.IsNullOrEmpty(Visuals[i].Name) &&
                    !NamedVisuals.ContainsKey(Visuals[i].Name))
                    NamedVisuals.Add(Visuals[i].Name, Visuals[i]);

                Visuals[i].Visibility = (i == _curSlide ? Visibility.EnabledDisabled : Visibility.Disabled);
            }
        }

        public void NextSlide()
        {
            _curSlide += _curSlide == (Visuals.Count - 1) ? 0 : 1;
            UpdateVisuals();
        }

        private void UpdateVisuals()
        {
            for (int i = 0; i < Visuals.Count; i++)
                Visuals[i].Visibility = (i == _curSlide ? Visibility.EnabledDisabled : Visibility.Disabled);
        }

        public void PrevSlide()
        {
            _curSlide -= _curSlide > 0 ? 1 : 0;
            UpdateVisuals();
        }

        public void Reset()
        {
            _curSlide = 0;
        }
    }
}