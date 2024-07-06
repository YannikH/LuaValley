using NLua;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Inventories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LuaValley.API.Game
{
    public enum DialogueEmotion
    {
        Happy,
        Sad,
        Unique,
        Neutral,
        Love,
        Angry,
        End
    }

    internal class DialogueAPI : LuaAPI
    {
        private struct DialogueRequest
        {
            public DialogueRequest() { }
            public DialogueRequest(string content, NPC character, string emotion = "Neutral")
            {
                this.content = content;
                this.character = character;
                this.emotion = emotion;
            }
            public NPC character = null;
            public string content = "";
            public string emotion = "Neutral";
            public LuaTable responses = null;
            public LuaFunction callback = null;
        }

        public DialogueAPI(APIManager api) : base(api, "DialogueAPI")
        {
            api.mod.Helper.Events.Display.MenuChanged += OnMenuChanged;
        }
        private LuaFunction callbackLua;
        private List<DialogueRequest> DialogueQueue = new List<DialogueRequest>();

        private void OnMenuChanged(object? sender, MenuChangedEventArgs e)
        {
            if (e.NewMenu == null && DialogueQueue.Count > 0)
            {
                DialogueRequest req = DialogueQueue[0];
                Create(req);
                DialogueQueue.RemoveAt(0);
            }
        }

        public void Create(string content)
        {
            if (Game1.dialogueUp)
            {
                DialogueQueue.Add(new DialogueRequest(content, null));
                return;
            }
            Game1.drawObjectDialogue(content);
        }

        public void Create(string content, NPC character, string emotion = "Neutral")
        {
            if (Game1.dialogueUp)
            {
                DialogueQueue.Add(new DialogueRequest(content, character, emotion));
                return;
            }
            string emotionString = GetEmotionString(emotion) + content;
            character.CurrentDialogue.Push(new StardewValley.Dialogue(character, "", emotionString));
            Game1.drawDialogue(character);
        }

        private void Create(DialogueRequest req)
        {
            if (req.character != null)
            {
                Create(req.content, req.character, req.emotion);
            }
            else
            {
                Create(req.content);
            }
        }

        public void Create(string content, LuaTable responses, LuaFunction callback)
        {
            List<Response> responseObjects = new List<Response>();
            foreach (LuaTable set in responses.Values)
            {
                List<object> kv = api.lua.ToList<object>(set);
                if (kv[0] is string && kv[1] is string)
                {
                    responseObjects.Add(new Response((string)kv[0], (string)kv[1]));
                }
            }
            callbackLua = callback;
            Game1.player.currentLocation.createQuestionDialogue(content, responseObjects.ToArray(), OnResponse);
        }

        private string GetEmotionString(string emotion)
        {
            DialogueEmotion e = DialogueEmotion.Neutral;
            Enum.TryParse<DialogueEmotion>(emotion, true, out e);
            switch (e)
            {
                case DialogueEmotion.Happy:
                    return StardewValley.Dialogue.dialogueHappy;
                case DialogueEmotion.Sad:
                    return StardewValley.Dialogue.dialogueSad;
                case DialogueEmotion.Unique:
                    return StardewValley.Dialogue.dialogueUnique;
                case DialogueEmotion.Neutral:
                    return "";
                case DialogueEmotion.Love:
                    return StardewValley.Dialogue.dialogueLove;
                case DialogueEmotion.Angry:
                    return StardewValley.Dialogue.dialogueAngry;
                case DialogueEmotion.End:
                    return StardewValley.Dialogue.dialogueEnd;
            }
            return StardewValley.Dialogue.dialogueNeutral;
        }

        private void OnResponse(Farmer who, string response)
        {
            api.lua.CallSafe(callbackLua, response);
        }
    }
}
