using StardewValley.Menus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LuaValley.UI
{
    internal class Menu: IClickableMenu
    {
        List<ColorPicker> colorPickers = new List<ColorPicker>();
        List<ClickableComponent> clickableComponents = new List<ClickableComponent>();
        public List<OptionsElement> options = new List<OptionsElement>();
        //List<ClickableAnimatedComponent>
    }
}
