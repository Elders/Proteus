using System.Collections.Generic;

namespace Elders.Cronus.DomainModeling
{
    public interface ICanRestoreStateFromEvents<out TState>
    {
        TState State { get; }
        void ReplayEvents(List<IEvent> events, int currentRevision);
    }
}