using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine.iOS;

namespace HSM {
    public class TransitionSequencer
    {
        public readonly StateMachine Machine;

        ISequence sequencer; // current phase
        Action nextPhase; // switch structure between phases
        (State from, State to)? pending;
        State lastFrom, lastTo;

        public TransitionSequencer(StateMachine machine)
        {
            Machine = machine;
        }

        public void RequestTransition(State from, State to)
        {
            Machine.ChangeState(from, to);
            //if(to == null || from  == null) { return; };
            //if(sequencer != null) { pending = (from, to); return; }
            //BeginTransition(from, to);
        }

        static List<PhaseStep> GatherPhaseSteps(List<State> chain, bool deactivate)
        {
            var steps = new List<PhaseStep>();
            for (int i = 0; i < chain.Count; i++)
            {
                var acts = chain[i].Activities;
                for (int j = 0; j < acts.Count; j++)
                {
                    var act = acts[j];
                    if (deactivate)
                    {
                        if(act.Mode == ActivityMode.Active) steps.Add(ct => act.DeactivateAsync(ct));
                    }
                    else
                    {
                        if(act.Mode == ActivityMode.Inactive) steps.Add(ct => act.ActivateAsync(ct));
                    }
                }
            }
            return steps;
        }

        static List<State> StatesToExit(State from, State lca)
        {
            var list = new List<State>();
            for (var s = from; s != null && s != lca; s = s.Parent) { list.Add(s); }
            return list;
        }

        static List<State> StatesToEnter(State to, State lca)
        {
            var stack = new Stack<State>();
            for (var s = to; s != null && s != lca; s = s.Parent) { stack.Push(s); }
            return new List<State>(stack);
        }


        CancellationTokenSource cts;
        public readonly bool UseSequential = true;
        private void BeginTransition(State from, State to)
        {
            var lca = Lca(from, to);
            var exitChain = StatesToExit(from, lca);
            var enterChain = StatesToEnter(to, lca);


            // 1. Deactivate the old branch
            var exitSteps = GatherPhaseSteps(exitChain, deactivate: true);
            //sequencer = new NoopPhase();
            sequencer = UseSequential ? new SequentialPhase(exitSteps, cts.Token) : new ParallelPhase(exitSteps, cts.Token);
            sequencer.Start();

            nextPhase = () =>
            {
                Machine.ChangeState(from, to);
                var enterSteps = GatherPhaseSteps(enterChain, deactivate: false);
                //sequencer = new NoopPhase();
                sequencer = UseSequential ? new SequentialPhase(enterSteps, cts.Token) : new ParallelPhase(enterSteps, cts.Token);
                sequencer.Start();
                sequencer.Start();
            };
        }

        private void EndTransition()
        {
            sequencer = null;

            if (pending.HasValue)
            {
                (State from, State to) p = pending.Value;
                pending = null;
                BeginTransition(p.from, p.to);
            }
        }

        public void Tick(float deltaTime)
        {
            if(sequencer != null)
            {
                if (sequencer.Update())
                {
                    if(nextPhase != null)
                    {
                        var n = nextPhase;
                        nextPhase = null;
                        n();
                    }
                    else
                    {
                        EndTransition();
                    }
                }
                return;
            }
            Machine.InternalTick(deltaTime);
        }

        // Returns lowest common ancestor of 2 states
        public static State Lca(State a,  State b)
        {
            var ap = new HashSet<State>();
            for (var s = a; s != null; s = s.Parent) { ap.Add(s); }
            
            for(var s = b; s != null; s = s.Parent)
            {
                if(ap.Contains(s)) return s;
            }
            return null;
        }
    }

    
}
