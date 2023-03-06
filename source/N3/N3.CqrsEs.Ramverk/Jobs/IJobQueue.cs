namespace N3.CqrsEs.Ramverk.Jobs
{
    public interface IJobQueue
    {
        Task<ReservationReceipt> Reserve<T>(string jobId)
            where T : IJob;

        Task Dequeue<T>(string jobbId, ReservationReceipt receipt)
            where T : IJob;

        Task<IEnumerable<JobStatus>> GetStatus<T>(
            JobStatusFiltering filtering = JobStatusFiltering.Pending
        )
            where T : IJob;

        Task<JobStatus> GetStatus<T>(string jobId)
            where T : IJob;

        Task Queue<T>(T job)
            where T : IJob;
    }
}
