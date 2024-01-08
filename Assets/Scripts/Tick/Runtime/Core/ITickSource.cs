namespace Tick.Core {

    public interface ITickSource {
        void Subscribe(IUpdatable updatable);
        void Unsubscribe(IUpdatable updatable);
    }

}
