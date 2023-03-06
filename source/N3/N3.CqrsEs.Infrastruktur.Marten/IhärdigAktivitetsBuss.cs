using Marten;
using N3.CqrsEs.Ramverk.Jobs;
using N3.CqrsEs.SkrivModell.Exceptions;
using N3.CqrsEs.SkrivModell.JobbPaket;

namespace N3.CqrsEs.Infrastruktur.Marten
{
    public class IhärdigAktivitetsBuss : IJobQueue
    {
        private readonly IDocumentStore _store;

        public IhärdigAktivitetsBuss(IDocumentStore store)
        {
            _store = store;
        }

        public async Task<IEnumerable<JobStatus>> GetStatus<T>(
            JobStatusFiltering filtrering = JobStatusFiltering.Pending
        )
            where T : IJob
        {
            var session = _store.LightweightSession();
            var status = session.Query<T>();
            var data = await status.ToListAsync();
            return data.Select(x => new JobStatus(x.Id, false));
        }

        public async Task<JobStatus> GetStatus<T>(string jobbId)
            where T : IJob
        {
            var session = _store.LightweightSession();
            var status = session.Query<T>().Where(x => x.Id == jobbId);
            var data = await status.SingleAsync();
            return new JobStatus(jobbId, false);
        }

        public async Task Queue<T>(T jobb)
            where T : IJob
        {
            var session = _store.LightweightSession();

            var status = session.Query<T>().FirstOrDefault(x => x.Id == jobb.Id);
            if (status is not null)
            {
                throw new GenerellKonfliktException("Finns redan");
            }

            session.Store(jobb);
            await session.SaveChangesAsync();
        }

        public async Task<ReservationReceipt> Reserve<T>(string jobbId)
            where T : IJob
        {
            var session = _store.LightweightSession(System.Data.IsolationLevel.Serializable);
            var post = await session.Query<T>().SingleAsync(x => x.Id == jobbId);
            if (post.ReservationsTidsstämpel is DateTimeOffset dt)
            {
                // konfigg
                if (DateTimeOffset.Now - dt < TimeSpan.FromMinutes(10))
                {
                    throw new GenerellKonfliktException();
                }
            }
            post.ReservationsId = UnikIdentifierare.Skapa();
            post.ReservationsTidsstämpel = DateTimeOffset.Now;
            session.Update(post);
            await session.SaveChangesAsync();
            return new ReservationReceipt(jobbId, post.ReservationsId);
        }

        public async Task Dequeue<T>(string jobbId, ReservationReceipt kvitto)
            where T : IJob
        {
            var session = _store.LightweightSession();
            var post = await session.Query<T>().SingleAsync(x => x.Id == jobbId);
            if (post.ReservationsId == kvitto.ReservationsId)
            {
                session.Delete(post);
            }
            else
            {
                throw new InvalidOperationException("Fel reservation");
            }
            await session.SaveChangesAsync();
        }
    }
}
