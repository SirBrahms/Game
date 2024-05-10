using Terminal.Gui;

namespace Game.Types;

class EButton : Button
{
    public new event Action<object>? Clicked;

    public override void OnClicked()
    {
        Clicked?.Invoke(this);
    }
}