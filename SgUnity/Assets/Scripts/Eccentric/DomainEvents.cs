namespace Eccentric {
    using System.Collections.Generic;
    using System.Linq;
    using System;

    static class DomainEvents {
        private static Dictionary<Type, List<Delegate>> m_actionsByType;
        public static void Register<T> (Action<T> callback)where T : IDomainEvent {
            var eventType = typeof (T);
            if (m_actionsByType == null) {
                m_actionsByType = new Dictionary<Type, List<Delegate>> ( );
            }
            if (!m_actionsByType.ContainsKey (eventType)) {
                m_actionsByType.Add (eventType, new List<Delegate> ( ));
            }
            var actions = m_actionsByType [eventType];
            if (!actions.Contains (callback))
                actions.Add (callback);
        }

        public static void UnRegister<T> (Action<T> callback)where T : IDomainEvent {
            if (m_actionsByType == null) {
                return;
            }
            var eventType = typeof (T);
            if (m_actionsByType.ContainsKey (eventType)) {
                var actions = m_actionsByType [eventType];
                actions.Remove (callback);
            }
        }
        public static void Clear ( ) {
            if (m_actionsByType != null) {
                m_actionsByType.Clear ( );
                m_actionsByType = null;
            }
        }
        public static void Raise<T> (T args)where T : IDomainEvent {
            if (m_actionsByType != null) {
                Type eventType = typeof (T);
                if (m_actionsByType.ContainsKey (eventType)) {
                    List<Delegate> actions = m_actionsByType [eventType];
                    foreach (var action in actions.Cast<Action<T>> ( ).ToList ( )) {
                        action (args);
                    }
                }
            }
        }
    }
    public abstract class IDomainEvent {

    }

}
