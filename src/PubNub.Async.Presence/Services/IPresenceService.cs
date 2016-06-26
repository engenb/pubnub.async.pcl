using System.Threading.Tasks;

namespace PubNub.Async.Presence.Services
{
    public interface IPresenceService
    {
	    Task<TState> GetState<TState>()
			where TState : class;
	    Task SetState<TState>(TState state)
			where TState : class;
    }
}
