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
    public enum MessageType { join, updatePlayer, collision}
    public class Network
    {
        Thread receiver;
        private int port = 12481;
        public UdpClient client;
        public IPEndPoint clientEP;
        public bool isHost;
        private int sendTimer;
        public Network(bool isHost)
        {
            this.isHost = isHost;
            
            if (!isHost)//If client
            {
                clientEP = new IPEndPoint(IPAddress.Parse("127.0.0.1"), port);
                client = new UdpClient();
                client.Connect(clientEP);
                SendMsg(new JoinMsg { name = Tron.thisRider.GetName() }, MessageType.join, clientEP);
            }
            else //If IS host
            {
                client = new UdpClient(port);
                clientEP = new IPEndPoint(IPAddress.Any, port);
            }
            receiver = new Thread(Receiver);
            receiver.Start();
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
        private void IncommingPlayer(UpdatePlayer msg, IPEndPoint ip)
        {
            Tron.otherRider.DirectionChange(msg.dir, msg.pos);
            if (isHost) //IF HOST - Echo back relevant info
            {
                SendPlayerData(ip); //Echo HOST rider to CLIENT
            }
        }
        public void MessageDecoder(byte[] data, IPEndPoint ip)
        {
            try
            {
                var jsonMsg = Encoding.UTF8.GetString(data); //Convert JSON into JSON string
                JObject complexMsg = JObject.Parse(jsonMsg); //Convert JSON into JSON Object
                JToken complexMsgType = complexMsg["type"]; //Fetch Type from JSON object
                //if (complexMsg != null && complexMsgType.Type is JTokenType.Integer)
                if (complexMsg != null)
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
            Tron.otherRider.SetName(msg.name);
            var data = new JoinMsg()
            {
                name = Tron.thisRider.GetName()
            };
            SendMsg(data, MessageType.join, ip);
            Tron.gameStart = true;
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
            if(isHost)client.Send(byteMsg, byteMsg.Length, ip);
            else client.Send(byteMsg, byteMsg.Length);
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
}
