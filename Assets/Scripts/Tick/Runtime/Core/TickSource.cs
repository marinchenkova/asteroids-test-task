using System.Collections.Generic;

namespace Tick.Core {

    public sealed class TickSource : ITickSource {

        private readonly HashSet<IUpdatable> _subscribers = new HashSet<IUpdatable>();
        private readonly HashSet<IUpdatable> _subscribersToAdd = new HashSet<IUpdatable>();
        private readonly HashSet<IUpdatable> _subscribersToRemove = new HashSet<IUpdatable>();

        public void Subscribe(IUpdatable updatable) {
            _subscribersToAdd.Add(updatable);
            _subscribersToRemove.Remove(updatable);
        }

        public void Unsubscribe(IUpdatable updatable) {
            _subscribersToAdd.Remove(updatable);
            _subscribersToRemove.Add(updatable);
        }

        public void Tick(float dt) {
            InvokeUpdate(dt);
            CheckSubscriberListChanges();
        }

        public void Clear() {
            _subscribers.Clear();
            _subscribersToAdd.Clear();
            _subscribersToRemove.Clear();
        }

        private void InvokeUpdate(float dt) {
            foreach (var subscriber in _subscribers) {
                subscriber.OnUpdate(dt);
            }
        }

        private void CheckSubscriberListChanges() {
            foreach (var subscriber in _subscribersToAdd) {
                _subscribers.Add(subscriber);
            }

            foreach (var subscriber in _subscribersToRemove) {
                _subscribers.Remove(subscriber);
            }

            _subscribersToAdd.Clear();
            _subscribersToRemove.Clear();
        }
    }

}
