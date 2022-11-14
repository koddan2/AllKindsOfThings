#nullable enable
namespace SAK
{
    public class InvariantFailedException : Exception
    {
        public InvariantFailedException(string msg)
            : base(msg)
        {

        }
    }
}
