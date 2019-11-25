using System.Collections.Generic;

namespace NeonTDS
{
    public class MessageQueue
    {
        private readonly Queue<Message> messages = new Queue<Message>();

        public void Enqueue(Message message)
        {
            lock (messages)
            {
                messages.Enqueue(message);
            }
        }

        public List<Message> GetMessages()
        {
            lock (messages)
            {
                List<Message> ret = new List<Message>(messages.ToArray());
                messages.Clear();
                return ret;
            }
        }
    }
}