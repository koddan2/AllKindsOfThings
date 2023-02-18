namespace N3.CqrsEs.Ramverk
{
    public interface IKommandoHanterare<T>
    {
        Task Hantera(T kommando);
    }
}
