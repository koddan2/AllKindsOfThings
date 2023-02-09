using Microsoft.Extensions.Logging;
using Stateless;
using Stateless.Reflection;

namespace N2.Story
{
	class PaymentAgg
	{
		readonly int _a = 0;
		public void RegisterPaymentAllocationIntent(string correlationId, decimal amount)
		{
			Console.WriteLine(correlationId, amount, _a);
		}
		public void ResolvePaymentAllocationIntent(string correlationId, decimal amount)
		{
			Console.WriteLine(correlationId, amount, _a);
		}
	}
	class InvoiceAgg
	{
		readonly int _a = 0;
		public void AllocatePayment(string correlationId, decimal amount)
		{
			Console.WriteLine(correlationId, amount, _a);
		}
	}

	interface ICorrelatable
	{
		string CorrelationId { get; }
	}
	record RetryState(uint TryCount, float Delay);
	enum Result
	{
		Uninitialized,
		Pending,
		Success,
		Error,
		Reverted,
	}
	readonly record struct State(
		string CorrelationId,
		decimal Amount,
		Result IntentRegistered = Result.Uninitialized,
		Result PaymentAllocatedToInvoice = Result.Uninitialized,
		Result IntentResolved = Result.Uninitialized) : ICorrelatable
	{
		public override string ToString()
		{
			return $"{IntentRegistered}|{PaymentAllocatedToInvoice}|{IntentResolved}";
		}
	}

	enum FsmTrigger
	{
		TryStep1,
		ResolveStep1,
		RejectStep1,
		TryStep2,
		ResolveStep2,
		RejectStep2,
		TryStep3,
		ResolveStep3,
		RejectStep3,
	}

	public class Story
	{
		private readonly ILogger _log;
		readonly PaymentAgg _paymentAgg = new();
		readonly InvoiceAgg _invoiceAgg = new();

		public Story(ILogger log)
		{
			_log = log;
		}

		public void Run()
		{
			var state = new State(Guid.NewGuid().ToString("N"), 231.00m);
			state = TryThenStore(state,
				(state) => _paymentAgg.RegisterPaymentAllocationIntent(state.CorrelationId, state.Amount),
				(state) => state with { IntentRegistered = Result.Success },
				(state) => state with { IntentRegistered = Result.Error });
			if (state.IntentRegistered == Result.Success)
			{
				state = TryThenStore(state,
					(state) => _invoiceAgg.AllocatePayment(state.CorrelationId, state.Amount),
					(state) => state with { PaymentAllocatedToInvoice = Result.Success },
					(state) => state with { PaymentAllocatedToInvoice = Result.Error });
			}

			if (state.PaymentAllocatedToInvoice == Result.Success)
			{
				state = TryThenStore(state,
					(state) => _paymentAgg.ResolvePaymentAllocationIntent(state.CorrelationId, state.Amount),
					(state) => state with { IntentResolved = Result.Success },
					(state) => state with { IntentResolved = Result.Error });
			}
			else if (state.PaymentAllocatedToInvoice == Result.Error)
			{
				// revert intent
			}
		}

		public static StateMachineInfo FsmTest()
		{
			var state = new State(Guid.NewGuid().ToString("N"), 231.00m);
			var fsm = new StateMachine<State, FsmTrigger>(state);
			fsm.Configure(state)
				.Permit(FsmTrigger.TryStep1, state with { IntentRegistered = Result.Pending });
			fsm.Configure(state with { IntentRegistered = Result.Pending })
				.Permit(FsmTrigger.ResolveStep1, state with { IntentRegistered = Result.Success });
			fsm.Configure(state with { IntentRegistered = Result.Pending })
				.Permit(FsmTrigger.RejectStep1, state with { IntentRegistered = Result.Error });

			fsm.Configure(state with { IntentRegistered = Result.Success })
				.Permit(FsmTrigger.TryStep2, state with { IntentRegistered = Result.Success, PaymentAllocatedToInvoice = Result.Pending });
			fsm.Configure(state with { IntentRegistered = Result.Success, PaymentAllocatedToInvoice = Result.Pending })
				.Permit(FsmTrigger.ResolveStep2, state with { IntentRegistered = Result.Success, PaymentAllocatedToInvoice = Result.Success });
			fsm.Configure(state with { IntentRegistered = Result.Success, PaymentAllocatedToInvoice = Result.Pending })
				.Permit(FsmTrigger.RejectStep2, state with { IntentRegistered = Result.Success, PaymentAllocatedToInvoice = Result.Error });
			return fsm.GetInfo();
		}

		/// <summary>
		/// May never throw exception.
		/// </summary>
		/// <param name="toStore"></param>
		/// <param name="retryState"></param>
		private static void Persist(ICorrelatable toStore, RetryState? retryState = null)
		{
			Console.WriteLine("{0}{1}", toStore, retryState);
		}

		private TState TryThenStore<TState>(
			TState beginState,
			Action<TState> commandInvoker,
			Func<TState, TState> onSuccess,
			Func<TState, TState> onError)
			where TState : ICorrelatable
		{
			try
			{
				commandInvoker(beginState);
				var newState = onSuccess(beginState);
				Persist(newState);
				return newState;
			}
			catch (Exception e)
			{
				_log.LogError(e, "Exception during story execution");
				var newState = onError(beginState);
				Persist(newState);
				return newState;
			}
		}
	}
}