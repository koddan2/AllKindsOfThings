using N3.CqrsEs.Messages;
using Rebus.Handlers;

namespace N3.App.Ombud.Web.MessageHandlers
{
    public class PingPongMessageHandler : IHandleMessages<PingPongMessage>
    {
        public async Task Handle(PingPongMessage message)
        {
            await ValueTask.CompletedTask;
            Console.WriteLine("Hello: {0}", message.MessageText);
        }
    }
}
