using Avalonia.Controls;
using Avalonia.Media;

namespace newUI.Helpers;

public class ColorModel
{
    public ColorModel(
        Color mainBgColour,
        Color secondaryBgColour,
        Color textColour,
        Color buttonColour)
    {
        MainBgColour = mainBgColour;
        SecondaryBgColour = secondaryBgColour;
        TextColour = textColour;
        ButtonColour = buttonColour;
    }

    public Color MainBgColour { get; }
    public Color SecondaryBgColour { get; }
    public Color TextColour { get; }
    public Color ButtonColour { get; }
}