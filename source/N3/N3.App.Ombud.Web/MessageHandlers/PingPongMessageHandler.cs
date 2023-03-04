using N3.CqrsEs.SkrivModell.JobbPaket;
using SlimMessageBus;

namespace N3.App.Ombud.Web.MessageHandlers
{
    public class PingPongMessageHandler : IConsumer<PingPongMessage>
    {
        public async Task OnHandle(PingPongMessage message)
        {
            await ValueTask.CompletedTask;
            Console.WriteLine("Hello: {0}", message.MessageText);
        }
    }
}
