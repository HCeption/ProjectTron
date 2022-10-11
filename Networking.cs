using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using Microsoft.Xna.Framework;
using System.Threading;

namespace ProjectTron
{
    public enum MessageType { join, updatePlayer, updateTrail }
    class Networking
    {
        Thread receiver = new Thread(Receiver);
        public static UdpClient client = new UdpClient(12500);
        public static IPEndPoint clientEP = new IPEndPoint(IPAddress.Any, 12500);
        public bool isHost;
        private int[] sendTimer = new int[2];
        public Networking(bool isHost)
        {
            this.isHost = isHost;
            if (isHost)
            {
                receiver.Start();
            }
        }
        /// <summary>
        /// Non hosting client will send updates. Will receive echo with new data from host.
        /// </summary>
        public void Update()
        {
            if (!isHost) //If NOT host
            {
                sendTimer[0]--;
                sendTimer[1]--;
                if (sendTimer[0] < 1) //Send Player data timer
                {
                    SendPlayerData(new IPEndPoint(IPAddress.Any, 12500));
                    sendTimer[0] = 2; //Wait 1 frame before sending again (30hz update)
                }
                if (sendTimer[1] < 1)
                {
                    SendTrailData(new IPEndPoint(IPAddress.Any, 12500));
                    sendTimer[1] = 4;//Wait 3 frames before sending again (15hz update)
                }
            }
        }
        private void SendPlayerData(IPEndPoint ip)
        {
            var data = new UpdatePlayer()
            {
                otherPlayerDir = Tron.thisRider.GetDir(),
                otherPlayerPos = Tron.thisRider.GetPos()
            };
            Networking.SendMsg(data, MessageType.updatePlayer, ip);
        }
        private void SendTrailData(IPEndPoint ip)
        {
            List<GameObject> temp = new List<GameObject>(Tron.gameObjects);
            temp.RemoveRange(0, 2); //Remove Riders from list
            var data = new UpdateTrail()
            {
                trails = new List<GameObject>(temp)
            };
            Networking.SendMsg(data, MessageType.updateTrail, ip);
        }
        private static void IncommingPlayer(UpdatePlayer msg)
        {
            Tron.otherRider.SetDir(msg.otherPlayerDir);
            Tron.otherRider.SetPos(msg.otherPlayerPos);
        }
        private static void IncommingTrail(UpdateTrail msg)
        {
            Tron.HandleNewObjects(msg.trails, true);
        }
        public static void MessageDecoder(byte[] data, IPEndPoint ip)
        {
            try
            {
                var jsonMsg = Encoding.UTF8.GetString(data); //Convert JSON into JSON string
                JObject complexMsg = JObject.Parse(jsonMsg); //Convert JSON into JSON Object
                JToken complexMsgType = complexMsg["Type"]; //Fetch Type from JSON object
                if (complexMsg != null && complexMsgType.Type is JTokenType.Integer)
                {
                    //Succesful message, do rest in here
                    MessageType msgType = (MessageType)complexMsgType.Value<int>();
                    JToken nullChecker = complexMsg["message"];
                    if (nullChecker == null) return;
                    switch (msgType)
                    {
                        case MessageType.join:
                            JoinMsg msg = complexMsg["message"].ToObject<JoinMsg>();
                            HandleJoin(msg, ip);
                            break;
                        case MessageType.updatePlayer:
                            UpdatePlayer msg1 = complexMsg["message"].ToObject<UpdatePlayer>();
                            IncommingPlayer(msg1);
                            break;
                        case MessageType.updateTrail:
                            UpdateTrail msg2 = complexMsg["message"].ToObject<UpdateTrail>();
                            IncommingTrail(msg2);
                            break;
                        default:
                            break;
                    }
                }
            }
            catch
            {
                //Breakpoint incase of decode fail
            }
        }
        private static void HandleJoin(JoinMsg msg, IPEndPoint ip)
        {
            Tron.NumberOfPlayers++;
            var data = new UpdatePlayer()
            {
                otherPlayerDir = Tron.thisRider.GetDir(),
                otherPlayerPos = Tron.thisRider.GetPos()
            };
            SendMsg(data, MessageType.updatePlayer, ip);
        }
        public static void SendMsg(NetworkMsgBase msgBase, MessageType msgType, IPEndPoint ip)
        {
            var msg = new NetworkMsg()
            {
                type = msgType,
                message = msgBase
            };
            var serializedMsg = JsonConvert.SerializeObject(msg);
            byte[] byteMsg = Encoding.UTF8.GetBytes(serializedMsg);
            client.Send(byteMsg, byteMsg.Length, ip);
        }
        private static void Receiver()
        {
            try
            {
                while (true)
                {
                    var data = client.Receive(ref clientEP);
                    MessageDecoder(data, clientEP);
                }
            }
            catch
            {
                Console.WriteLine("Message receive failure");
            }
        }
    }
    [Serializable]
    public class NetworkMsg
    {
        public MessageType type;
        public NetworkMsgBase message;
    }
    [Serializable]
    public class NetworkMsgBase { }
    [Serializable]
    public class JoinMsg : NetworkMsgBase
    {
        public string name;
    }
    [Serializable]
    public class UpdatePlayer : NetworkMsgBase
    {
        public Vector2 otherPlayerPos;
        public Vector2 otherPlayerDir;
    }
    [Serializable]
    public class UpdateTrail : NetworkMsgBase
    {
        public List<GameObject> trails;
    }
}
