using StardewValley.Menus;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NLua;
using static StardewValley.Menus.NumberSelectionMenu;
using static StardewValley.LocationRequest;
using static System.Net.Mime.MediaTypeNames;
using static StardewValley.Menus.NamingMenu;
using static StardewValley.Menus.ConfirmationDialog;
using static StardewValley.Menus.ChooseFromListMenu;
using System.Reflection.Metadata.Ecma335;

namespace LuaValley.API.Interface
{
    internal class UIAPI: LuaAPI
    {
        public UIAPI(APIManager api) : base(api, "UIAPI") { }

        public void CreateNumberMenu(string text, LuaFunction callback, int price = -1, int minValue = 0, int maxValue = 99, int defaultNumber = 0)
        {
            behaviorOnNumberSelect onSelect = (int number, int price, Farmer who) =>
            {
                api.lua.CallSafe(callback, who, number, price);
            };
            Game1.activeClickableMenu = new NumberSelectionMenu(text, onSelect, 50, 0, 999);
        }

        public void CreateNamingMenu(string title, LuaFunction callback, string defaultName = "")
        {
            doneNamingBehavior onSelect = (string name) =>
            {
                api.lua.CallSafe(callback, name);
            };
            Game1.activeClickableMenu = new NamingMenu(onSelect, title, defaultName);
        }

        public void CreateTextInputMenu(string title, LuaFunction callback, string defaultName = "")
        {
            doneNamingBehavior onSelect = (string name) =>
            {
                api.lua.CallSafe(callback, name);
            };
            Game1.activeClickableMenu = new TitleTextInputMenu(title, onSelect, defaultName);
        }

        public void CreateConfirmationMenu(string message, LuaFunction onAccept, LuaFunction onCancel)
        {
            behavior acceptFn = (Farmer who) =>
            {
                api.lua.CallSafe(onAccept, who);
            };
            behavior denyFn = (Farmer who) =>
            {
                api.lua.CallSafe(onCancel, who);
            };
            Game1.activeClickableMenu = new ConfirmationDialog(message, acceptFn, denyFn);
        }

        public void CreateMailMenu(string title, string text)
        {
            Game1.activeClickableMenu = new LetterViewerMenu(text, title);
        }

        public void CreateChoiceMenu(LuaTable options, LuaFunction chooseAction)
        {
            var optionList = api.lua.ToList<string>(options);
            actionOnChoosingListOption onChosen = (string option) =>
            {
                api.lua.CallSafe(chooseAction, option);
            };
            var newMenu = new ChooseFromListMenu(optionList, onChosen);
            Game1.activeClickableMenu = newMenu;
        }

        public void CreateItemGrabMenu(LuaTable itemsTable, bool essential = false)
        {
            var items = new List<Item>();
            foreach (object item in itemsTable.Values)
            {
                if (item is string)
                {
                    items.Add(ItemRegistry.Create((string)item));
                }
                if (item is Item)
                {
                    items.Add((Item)item);
                }
            }
            ItemGrabMenu itemGrabMenu = new ItemGrabMenu(items);
            itemGrabMenu.setEssential(essential);
            itemGrabMenu.inventory.showGrayedOutSlots = false;
            Game1.activeClickableMenu = itemGrabMenu;
        }

        public void CreateItemListMenu(string title, LuaTable itemsTable)
        {
            var items = new List<Item>();
            foreach (object item in itemsTable.Values)
            {
                if (item is string)
                {
                    items.Add(ItemRegistry.Create((string)item));
                }
                if (item is Item)
                {
                    items.Add((Item)item);
                }
            }
            Game1.activeClickableMenu = new ItemListMenu(title, items);
        }

        public void CreateShopMenu(LuaTable inventory)
        {
            var stock = new Dictionary<ISalable, ItemStockInformation>();
            foreach (LuaTable set in inventory.Values)
            {
                List<object> kv = api.lua.ToList<object>(set);
                if (kv[0] is string && kv[1] is long && kv[2] is long)
                {
                    var item = ItemRegistry.Create<StardewValley.Object>((string)kv[0]);
                    stock[item] = new ItemStockInformation(Convert.ToInt32(kv[1]), Convert.ToInt32(kv[2]));
                }
            }
            var shop = new ShopMenu("LuaValleyShop", stock);
            Game1.activeClickableMenu = shop;
        }

        public void CreateAnimationPreviewTool()
        {
            Game1.activeClickableMenu = new AnimationPreviewTool();
        }

        public void CloseMenu()
        {
            Game1.activeClickableMenu = null;
        }

        public void SetCursor(string cursor = "default")
        {
            int cursorType = Game1.cursor_default;
            switch (cursor)
            {
                case "none": cursorType = Game1.cursor_none; break;
                case "wait": cursorType = Game1.cursor_wait; break;
                case "grab": cursorType = Game1.cursor_grab; break;
                case "gift": cursorType = Game1.cursor_gift; break;
                case "talk": cursorType = Game1.cursor_talk; break;
                case "look": cursorType = Game1.cursor_look; break;
                case "harvest": cursorType = Game1.cursor_harvest; break;
            }
            Game1.mouseCursor = cursorType;
        }
    }
}
