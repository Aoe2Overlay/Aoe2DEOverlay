namespace Aoe2DEOverlay
{
    public delegate void ReceiveMessage (Message message);
    
    public class MessageBus
    {
        public static MessageBus Instance { get; } = new MessageBus();
        
        public ReceiveMessage Subscriber;
        
        static MessageBus()
        {
        }

        private MessageBus()
        {
        }

    }

    public abstract class Message
    {
        public string Id => this.GetType().Name;
    }
}