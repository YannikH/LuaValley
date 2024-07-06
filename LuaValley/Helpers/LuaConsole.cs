using LuaValley.API;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Characters;
using StardewValley.Menus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static StardewValley.LocalizedContentManager;

namespace LuaValley.Helpers
{
    internal class LuaConsole : IClickableMenu
    {
        ChatTextBox chatBox;
        private List<ChatMessage> log = new List<ChatMessage>();
        private KeyboardState oldKBState;
        APIManager api;
        public LuaConsole(APIManager api)
        {
            this.api = api;
            Texture2D chatboxTexture = Game1.content.Load<Texture2D>("LooseSprites\\chatBox");
            chatBox = new ChatTextBox(chatboxTexture, null, Game1.smallFont, Color.White);
            chatBox.TitleText = "Lua Console";
            chatBox.OnEnterPressed += onSend;
            chatBox.X = 0;
            chatBox.Y = 0;
            chatBox.Width = 500;
            chatBox.Height = 500;
            api.mod.Helper.Events.Display.RenderingActiveMenu += (object? sender, RenderingActiveMenuEventArgs args) => {
                chatBox.Draw(args.SpriteBatch);
            };
        }

        public ChatTextBox GetChatBox()
        {
            return chatBox;
        }

        public void Show(bool visible)
        {
            Game1.activeClickableMenu = this;
            Game1.keyboardDispatcher.Subscriber = chatBox;
        }

        public void onSend(TextBox sender)
        {
            //api.lua.Execute(sender.Text);
            var msg = new ChatMessage();
            msg.message = new List<ChatSnippet>();
            msg.message.Add(new ChatSnippet(sender.Text, LanguageCode.en));
            log.Add(msg);
            chatBox.setText("");
        }

        public override void draw(SpriteBatch b)
        {
            if (!this.chatBox.Selected) return;
            int heightSoFar = 0;
            bool drawBG = false;
            for (int j = this.log.Count - 1; j >= 0; j--)
            {
                ChatMessage message2 = this.log[j];
                if (this.chatBox.Selected || message2.alpha > 0.01f)
                {
                    heightSoFar += message2.verticalSize;
                    drawBG = true;
                }
            }
            if (drawBG)
            {
                IClickableMenu.drawTextureBox(b, Game1.mouseCursors, new Rectangle(301, 288, 15, 15), base.xPositionOnScreen, base.yPositionOnScreen - heightSoFar - 20 + ((!this.chatBox.Selected) ? this.chatBox.Height : 0), this.chatBox.Width, heightSoFar + 20, Color.White, 4f, drawShadow: false);
            }
            heightSoFar = 0;
            for (int i = this.log.Count - 1; i >= 0; i--)
            {
                ChatMessage message = this.log[i];
                heightSoFar += message.verticalSize;
                message.draw(b, base.xPositionOnScreen + 12, base.yPositionOnScreen - heightSoFar - 8 + ((!this.chatBox.Selected) ? this.chatBox.Height : 0));
            }
            if (this.chatBox.Selected)
            {
                this.chatBox.Draw(b, drawShadow: false);
                if (this.isWithinBounds(Game1.getMouseX(), Game1.getMouseY()) && !Game1.options.hardwareCursor)
                {
                    Game1.mouseCursor = (Game1.options.gamepadControls ? 44 : 0);
                }
            }
        }

        public override void update(GameTime time)
        {
            KeyboardState keyState = Game1.input.GetKeyboardState();
            Keys[] pressedKeys = keyState.GetPressedKeys();
            foreach (Keys key in pressedKeys)
            {
                if (!this.oldKBState.IsKeyDown(key))
                {
                    this.receiveKeyPress(key);
                }
            }
            this.oldKBState = keyState;
        }
    }
}
