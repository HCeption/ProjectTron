using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectTron
{
    class Chat
    {
        private static bool chatBoxActive;
        private static Vector2 position = new Vector2(Tron.gameWindow.X + 10, Tron.screen.Y - 50);
        private static Rectangle rect = new Rectangle((int)Tron.gameWindow.X + 10, (int)Tron.screen.Y - 50, (int)(Tron.screen.X - Tron.gameWindow.X - 20), 40);
        public static List<string> log = new List<string>() { "This is a dumb chat,","please type slowly as it will","register multiple inputs otherwise"};
        private static Keys oldKs = Keys.None;
        private static string currentMsg = "";
        public static void NewEntry(string s)
        {
            log.Add(s);
        }
        public static List<string> GetChatLog()
        {
            return log;
        }
        public static void Draw(SpriteBatch sb, SpriteFont txt)
        {
            int x = 0;
            for (int i = log.Count - 1; i > -1; i--)
            {
                sb.DrawString(txt, log[i], new Vector2(Tron.gameWindow.X + 10, Tron.screen.Y - 80 - x), Color.Black, 0, Vector2.Zero, 0.5f, 0, 0);
                x += 13;
            }
            sb.Draw(Tron.ct, position, rect, Color.LightGray, 0f, Vector2.Zero, 1, SpriteEffects.None, 0);
            if (currentMsg == "")
            {
                if(!chatBoxActive)sb.DrawString(txt, "Click here to begin chatting", new Vector2(Tron.gameWindow.X + 10, Tron.screen.Y - 50), Color.Black, 0, Vector2.Zero, 0.5f, 0, 0);
                else sb.DrawString(txt, "Ready for message!", new Vector2(Tron.gameWindow.X + 10, Tron.screen.Y - 50), Color.Black, 0, Vector2.Zero, 0.5f, 0, 0);
            }
            else
            {
                sb.DrawString(txt, currentMsg, new Vector2(Tron.gameWindow.X + 10, Tron.screen.Y - 50), Color.Black, 0, Vector2.Zero, 0.5f, 0, 0);
            }
        }
        public static void Update(GameTime gameTime)
        {
            MouseState ms = Mouse.GetState();
            if (ms.X > Tron.gameWindow.X + 10 && ms.X < Tron.screen.X - 10) //Within X of chatbox
            {
                if (ms.Y > Tron.screen.Y - 50 && ms.Y < Tron.screen.Y - 10) //Within Y of chatbox
                {
                    if (ms.LeftButton == ButtonState.Pressed) //Mouse clicked
                    {
                        chatBoxActive = true;
                    }
                }
            }
            if (chatBoxActive) KeyboardListener();
        }
        private static void KeyboardListener()
        {
            KeyboardState ks = Keyboard.GetState();
            KeyCheck(ks, Keys.A, 'a'); //No fancry way of doing this worked. Big sad.
            KeyCheck(ks, Keys.B, 'b');
            KeyCheck(ks, Keys.C, 'c');
            KeyCheck(ks, Keys.D, 'd');
            KeyCheck(ks, Keys.E, 'e');
            KeyCheck(ks, Keys.F, 'f');
            KeyCheck(ks, Keys.G, 'g');
            KeyCheck(ks, Keys.H, 'h');
            KeyCheck(ks, Keys.I, 'i');
            KeyCheck(ks, Keys.J, 'j');
            KeyCheck(ks, Keys.K, 'k');
            KeyCheck(ks, Keys.L, 'l');
            KeyCheck(ks, Keys.M, 'm');
            KeyCheck(ks, Keys.N, 'n');
            KeyCheck(ks, Keys.O, 'o');
            KeyCheck(ks, Keys.P, 'p');
            KeyCheck(ks, Keys.Q, 'q');
            KeyCheck(ks, Keys.R, 'r');
            KeyCheck(ks, Keys.S, 's');
            KeyCheck(ks, Keys.T, 't');
            KeyCheck(ks, Keys.U, 'u');
            KeyCheck(ks, Keys.V, 'v');
            KeyCheck(ks, Keys.W, 'w');
            KeyCheck(ks, Keys.X, 'x');
            KeyCheck(ks, Keys.Y, 'y');
            KeyCheck(ks, Keys.Z, 'z');
            KeyCheck(ks, Keys.Enter, ' ');
            KeyCheck(ks, Keys.Space, ' ');
        }
        private static void KeyCheck(KeyboardState ks, Keys k, char c)
        {
            if (ks.IsKeyDown(k) && oldKs != k)
            {
                oldKs = k;
                if (k == Keys.Enter)
                {
                    NewEntry(currentMsg);
                    if (Tron.network != null)
                    {
                        Tron.network.SendMsg(new ChatMsg { msg = currentMsg }, MessageType.chat, Tron.network.storedClient);
                    }
                    currentMsg = "";
                    chatBoxActive = false;
                    return;
                }
                currentMsg += c;
                Console.WriteLine(c);
            }
            if (ks.IsKeyUp(k) && oldKs == k)
            {
                oldKs = Keys.None;
            }
        }
    }
}
