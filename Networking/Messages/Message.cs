﻿using System.IO;
using System.Net;

namespace NeonTDS
{
    public class Message
    {
        public MessageTypes Type { get; set; }
        public IPEndPoint RemoteEndPoint { get; set; }

        protected Message(MessageTypes type)
        {
            Type = type;
        }

        public static Message GetFromBytes(BinaryReader reader)
        {
            Message message = new Message(MessageTypes.Unknown);
            switch ((MessageTypes)reader.ReadByte())
            {
                case MessageTypes.Clock:
                    message = new ClockMessage();
                    break;
                case MessageTypes.Connect:
                    message = new ConnectMessage();
                    break;
                case MessageTypes.ConnectResponse:
                    message = new ConnectResponseMessage();
                    break;
                case MessageTypes.Damage:
                    message = new DamageMessage();
                    break;
                case MessageTypes.EntityCreate:
                    message = new EntityCreateMessage();
                    break;
                case MessageTypes.EntityDestroy:
                    message = new EntityDestroyMessage();
                    break;
                case MessageTypes.PlayerInput:
                    message = new PlayerInputMessage();
                    break;
                case MessageTypes.PlayerPoweredUp:
                    message = new PlayerPoweredUpMessage();
                    break;
                case MessageTypes.PlayerRespawned:
                    message = new PlayerRespawnedMessage();
                    break;
                case MessageTypes.PlayerState:
                    message = new PlayerStateMessage();
                    break;
                case MessageTypes.ShapeCreate:
                    message = new ShapeCreateMessage();
                    break;
            }

            message.FromBytes(reader);
            return message;
        }

        public virtual void FromBytes(BinaryReader reader)
        {
        }

        public virtual void ToBytes(BinaryWriter writer)
        {
            writer.Write((byte)Type);
        }
    }
}
