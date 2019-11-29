using System.Collections.Generic;

namespace NeonTDS
{
    public class MessageQueue
    {
        private readonly Queue<Message> messages = new Queue<Message>();

        public void Enqueue(Message message)
        {
            messages.Enqueue(message);
        }

        public void Enqueue(List<Message> incomingMessages)
        {
            foreach (Message message in incomingMessages) {
                messages.Enqueue(message);
            }
        }

        public List<Message> RetrieveMessages()
        {
            List<Message> ret = new List<Message>(messages.ToArray());
            messages.Clear();
            return ret;
        }
    }
}