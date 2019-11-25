using System.Collections.Generic;
using System.IO;
using System.Numerics;
using System.Text;

namespace NeonTDS
{
    public class MessageUtils
    {
        public static void ToPackedBytes(Stream stream, List<Message> messages)
        {
            BinaryWriter writer = new BinaryWriter(stream);
            foreach (Message message in messages) {
                message.ToBytes(writer);
            }
        }

        public static void StreamToMessageQueue(Stream stream, MessageQueue messageQueue)
        {
            BinaryReader reader = new BinaryReader(stream);
            while (stream.Position != stream.Length)
            {
                messageQueue.Enqueue(Message.GetFromBytes(reader));
            }
        }

        public static void StreamFromMessageQueue(Stream stream, MessageQueue messageQueue)
        {
            ToPackedBytes(stream, messageQueue.GetMessages());
        }
    }

    public enum MessageTypes
    {
        Unknown,
        Clock,
        EntityCreate,
        EntityDestroy,
        ShapeCreate,
        PlayerState,
        Damage,
        PlayerRespawned,
        PlayerPoweredUp,
        Connect,
        ConnectResponse,
        PlayerInput,
    }
}
