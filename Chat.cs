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
        public static List<string> log = new List<string>() { "This is a dumb chat,","please type slowly as it will","register multiple inputs otherwise",
            "","To connect to host: Find HostIP.txt and input-","The ip from hosts ipconfig","","Use psuedo REST using 'rest/get/index' or","'rest/delete/index' (extra / dont matter)",
            "","Move Riders using arrow keys"};
        private static Keys oldKs = Keys.None;
        private static string userMsg = "";
        private static bool userShiftDown;
        public static void NewEntry(string s)
        {
            log.Add(s);
        }
        private static void DeleteAt(int index)
        {
            try
            {
                string s = $"Removed entry {index}: " + log[index];
                log.RemoveAt(index);
                log.Add(s);
                Tron.network.SendMsg(new ChatMsg { msg = s }, MessageType.chat, Tron.network.storedClient);
            }
            catch
            {
                Tron.network.SendMsg(new ChatMsg { msg = $"Out of index or failed: {index}/{log.Count}" }, MessageType.chat, Tron.network.storedClient);
            }
        }
        private static void GetAt(int index)
        {
            try
            {
                Tron.network.SendMsg(new ChatMsg { msg = log[index] }, MessageType.chat, Tron.network.storedClient);
            }
            catch
            {
                Tron.network.SendMsg(new ChatMsg { msg = $"Out of index: {index}/{log.Count}" }, MessageType.chat, Tron.network.storedClient);
            }
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
            if (userMsg == "")
            {
                if (!chatBoxActive) sb.DrawString(txt, "Click here to begin chatting", new Vector2(Tron.gameWindow.X + 10, Tron.screen.Y - 50), Color.Black, 0, Vector2.Zero, 0.5f, 0, 0);
                else sb.DrawString(txt, "Ready for message!", new Vector2(Tron.gameWindow.X + 10, Tron.screen.Y - 50), Color.Black, 0, Vector2.Zero, 0.5f, 0, 0);
            }
            else
            {
                sb.DrawString(txt, userMsg, new Vector2(Tron.gameWindow.X + 10, Tron.screen.Y - 50), Color.Black, 0, Vector2.Zero, 0.5f, 0, 0);
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
            KeyCheck(ks, Keys.D0, '0');
            KeyCheck(ks, Keys.D1, '1');
            KeyCheck(ks, Keys.D2, '2');
            KeyCheck(ks, Keys.D3, '3');
            KeyCheck(ks, Keys.D4, '4');
            KeyCheck(ks, Keys.D5, '5');
            KeyCheck(ks, Keys.D6, '6');
            KeyCheck(ks, Keys.D7, '7');
            KeyCheck(ks, Keys.D8, '8');
            KeyCheck(ks, Keys.D9, '9');
            KeyCheck(ks, Keys.LeftShift, 'ø');
            KeyCheck(ks, Keys.Enter, ' ');
            KeyCheck(ks, Keys.Space, ' ');
        }
        public static void RestDecoder(string s)
        {
            string e = s.Substring(s.LastIndexOf('/') - 1);
            if (e[0] == 't') //REST GET
            {
                int temp = Int32.Parse(s.Substring(s.LastIndexOf('/') + 1));
                GetAt(temp);
            }
            else if (e[0] == 'e')//REST DELETE
            {
                int temp = Int32.Parse(s.Substring(s.LastIndexOf('/') + 1));
                DeleteAt(temp);
            }
        }
        private static void KeyCheck(KeyboardState ks, Keys k, char c)
        {
            if (ks.IsKeyDown(k) && oldKs != k)
            {
                oldKs = k;
                if (k == Keys.Enter)
                {
                    SendMsg();
                }
                if (!userShiftDown)
                {
                    if (c != 'ø') userMsg += c; //ø is empty char used to check for SHIFT key, needed the char for method to work.
                    else userShiftDown = true;
                }
                else if (c == '7') userMsg += '/';
                Console.WriteLine(c);
            }
            if (ks.IsKeyUp(k) && oldKs == k)
            {
                if (c == 'ø')
                {
                    userShiftDown = false;
                }
                oldKs = Keys.None;
            }
        }
        private static void SendMsg()
        {

            if (userMsg[0] == 'r' && userMsg[1] == 'e' && userMsg[2] == 's' && userMsg[3] == 't' && userMsg[4] == '/')
            {
                RestDecoder(userMsg); //This wont enter? WTF?
                if (Tron.network != null)
                {
                    Tron.network.SendMsg(new PsuedoREST { msg = userMsg }, MessageType.REST, Tron.network.storedClient);
                    userMsg = "";
                    chatBoxActive = false;
                    return;
                }
            }
            else NewEntry(userMsg);
            if (Tron.network != null)
            {
                Tron.network.SendMsg(new ChatMsg { msg = userMsg }, MessageType.chat, Tron.network.storedClient);
            }
            userMsg = "";
            chatBoxActive = false;
        }
    }
}
