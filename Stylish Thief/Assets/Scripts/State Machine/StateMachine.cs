using System.Collections.Generic;
using UnityEngine;

namespace HSM { 

    public class StateMachine
    {
        public readonly State Root;
        public readonly TransitionSequencer Sequencer;

        bool started;

        public StateMachine(State root)
        {
            Root = root;
            Sequencer = new TransitionSequencer(this);
        }

        public void Start()
        {
            if (started) { return; }
            started = true;

            Root.Enter();
        }

        public void Update(float deltaTime)
        {
            if (!started) { Start(); }
            InternalTick(deltaTime);
        }

        internal void InternalTick(float deltaTime) => Root.Update(deltaTime);

        public void ChangeState(State from, State to)
        {
            if (from == to || from == null || to == null) return;
            State lca = TransitionSequencer.Lca(from, to);

            // Exit current branch up to (but not including) LCA
            for (State s = from; s != lca; s = s.Parent) { s.Exit(); }

            var stack = new Stack<State>();
            for (State s = to; s != lca; s = s.Parent) { stack.Push(s); }
            while (stack.Count > 0) { stack.Pop().Enter(); }

        }
    }
}
