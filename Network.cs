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
    public enum MessageType { join, updatePlayer, updateTrail, gameStart}
    class Network
    {
        Thread receiver;
        public static UdpClient client = new UdpClient(12500);
        public static IPEndPoint clientEP = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 12500);
        private IPEndPoint storedEP;
        public bool isHost;
        private int sendTimer;
        private bool sendTrail = false;
        public Network(bool isHost)
        {
            this.isHost = isHost;
            receiver = new Thread(Receiver);
            receiver.Start();
            if (!isHost)
            {
                var data = new GameStart() { };
                //SendMsg(data, MessageType.gameStart, ip);
            }
        }
        /// <summary>
        /// Non hosting client will send updates. Will receive echo with new data from host.
        /// </summary>
        public void Update()
        {
            if (!isHost) //If NOT host
            {
                sendTimer--;
                //sendTimer[1]--;
                if (sendTimer < 1) //Send Player data timer
                {
                    SendPlayerData(new IPEndPoint(IPAddress.Any, 12500));
                    sendTimer = 2; //Wait 1 frame before sending again (30hz update)
                }
                //if (sendTimer[1] < 1)
                //{
                //    SendTrailData(new IPEndPoint(IPAddress.Any, 12500));
                //    sendTimer[1] = 4;//Wait 3 frames before sending again (15hz update)
                //}
            }
            if (sendTrail)
            {
                SendTrailData(storedEP);
                sendTrail = false;
            }
        }
        private void SendPlayerData(IPEndPoint ip)
        {
            var data = new UpdatePlayer()
            {
                dir = Tron.thisRider.GetDir(),
                pos = Tron.thisRider.GetPos()
            };
            SendMsg(data, MessageType.updatePlayer, ip);
        }
        private void SendTrailData(IPEndPoint ip)
        {
            List<GameObject> temp = new List<GameObject>(Tron.gameObjects);
            temp.RemoveRange(0, 2); //Remove Riders from list
            var data = new UpdateTrail()
            {
                trails = new List<GameObject>(temp)
            };
            SendMsg(data, MessageType.updateTrail, ip);
        }
        private void IncommingPlayer(UpdatePlayer msg, IPEndPoint ip)
        {
            Tron.otherRider.SetDir(msg.dir);
            Tron.otherRider.SetPos(msg.pos);
            if (isHost) //IF HOST - Echo back relevant info
            {
                SendPlayerData(ip); //Echo HOST rider to CLIENT
                //SendTrailData(ip); //Echo ALL trail data to CLIENT
                sendTrail = true; //Delay sending until next pass (Monogame is 60hz, thereby delaying by 1hz)
                storedEP = ip;
            }
        }
        private void IncommingTrail(UpdateTrail msg)
        {
            Tron.HandleNewObjects(msg.trails, true);
        }
        public void MessageDecoder(byte[] data, IPEndPoint ip)
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
                            IncommingPlayer(msg1, ip);
                            break;
                        case MessageType.updateTrail:
                            UpdateTrail msg2 = complexMsg["message"].ToObject<UpdateTrail>();
                            IncommingTrail(msg2);
                            break;
                        case MessageType.gameStart:
                            Tron.gameStart = true;
                            Tron.NumberOfPlayers++;
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
        private void HandleJoin(JoinMsg msg, IPEndPoint ip)
        {
            Tron.NumberOfPlayers++;
            var data = new UpdatePlayer()
            {
                dir = Tron.thisRider.GetDir(),
                pos = Tron.thisRider.GetPos()
            };
            SendMsg(data, MessageType.updatePlayer, ip);
        }
        public void SendMsg(NetworkMsgBase msgBase, MessageType msgType, IPEndPoint ip)
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
        private void Receiver()
        {
            while (true)
            {
                var data = client.Receive(ref clientEP);
                MessageDecoder(data, clientEP);
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
        public Vector2 pos;
        public Vector2 dir;
    }
    [Serializable]
    public class UpdateTrail : NetworkMsgBase
    {
        public List<GameObject> trails;
    }
    [Serializable]
    public class GameStart : NetworkMsgBase
    {

    }
}
