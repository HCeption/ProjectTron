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

namespace ProjectTron
{
    public enum MessageType { join, update }
    class Networking
    {
        public static UdpClient client = new UdpClient(12500);
        public static IPEndPoint clientEP = new IPEndPoint(IPAddress.Any, 12500);
        public void MessageDecoder(byte[] data, IPEndPoint ip)
        {
            try
            {
                var jsonMsg = Encoding.UTF8.GetString(data);
                JObject complexMsg = JObject.Parse(jsonMsg);
                JToken complexMsgType = complexMsg["Type"];
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
                            HandleJoin(msg);
                            break;
                        case MessageType.update:
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
        private void HandleJoin(JoinMsg msg)
        {
            Tron.NumberOfPlayers++;
            var data = new UpdatePlayer()
            {
                otherPlayerDir = Tron.thisRider.GetDir(),
                otherPlayerPos = Tron.thisRider.GetPos()
            };
            SendMsg(data,MessageType.update);
        }
        public static void SendMsg(NetworkMsgBase msgBase, MessageType msgType)
        {
            var msg = new NetworkMsg()
            {
                type = msgType,
                message = msgBase
            };
            var serializedMsg = JsonConvert.SerializeObject(msg);
            byte[] byteMsg = Encoding.UTF8.GetBytes(serializedMsg);
            client.Send(byteMsg, byteMsg.Length, clientEP);
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
        public List<Rectangle> rect;
        public List<Vector2> position;
    }
}
