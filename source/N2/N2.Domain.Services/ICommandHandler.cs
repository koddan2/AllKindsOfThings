namespace N2.Domain.Services;

public interface ICommandHandler<TAggregate, TCommand>
{
	Task Hydrate(TAggregate aggregate, ExpectedStateOfStream expectedState = ExpectedStateOfStream.Exist);
	Task Handle(TAggregate aggregate, TCommand command);
}
