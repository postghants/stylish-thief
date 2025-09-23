using System.Collections.Generic;
using UnityEngine;

namespace HSM
{
    public abstract class State
    {
        public StateMachine Machine;
        public State Parent;
        public State ActiveChild;
        public State(StateMachine machine, State parent = null)
        {
            Machine = machine;
            Parent = parent;
        }

        protected virtual State GetInitialState() => null;
        protected virtual State GetTransition() => null;

        // Lifecycle hooks
        protected virtual void OnEnter() { }
        protected virtual void OnExit() { }
        protected virtual void OnUpdate() { }

        internal void Enter()
        {
            if (Parent != null) { Parent.ActiveChild = this; }
            OnEnter();
            State init = GetInitialState();
            if (init != null)
            {
                init.Enter();
            }
        }

        internal void Exit()
        {
            if (ActiveChild != null) { ActiveChild.Exit(); }
            ActiveChild = null;
            OnExit();
        }

        internal void Update(float deltaTime)
        {
            State t = GetTransition();
            if (t != null)
            {
                Machine.Sequencer.RequestTransition(this, t);
                return;
            }
            ActiveChild?.Update(deltaTime);
            OnUpdate();
        }

        // Returns the deepest currently-active child state
        public State Leaf()
        {
            State s = this;
            while (s.ActiveChild != null) s = s.ActiveChild;
            return s;
        }

        // Yields this state and then each ancestor up to the root state
        public IEnumerable<State> PathToRoot()
        {
            for (State s = this; s != null; s = s.Parent) yield return s;
        }
    }
}
