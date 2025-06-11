using System;
using System.Collections.Generic;

namespace YooX.StateMachine {
    /// <summary>
    /// This class manages states and transitions in a state machine.
    /// It handles state updates, transitions between states, and allows the addition
    /// of transitions with specific conditions.
    /// </summary>
    public class StateMachine {
        // The current state node that the state machine is in.
        private StateNode? current;

        // A dictionary mapping state types to their corresponding state nodes.
        private readonly Dictionary<Type, StateNode> nodes = new Dictionary<Type, StateNode>();

        // A set of transitions that can be triggered from any state.
        private readonly HashSet<ITransition> anyTransitions = new HashSet<ITransition>();

        /// <summary>
        /// Updates the state machine. If a valid transition is found,
        /// it changes to the target state. Otherwise, it updates the current state.
        /// </summary>
        public void Update() {
            var transition = GetTransition();
            if (transition != null) {
                ChangeState(transition.To);
            }

            current?.State?.Update();
        }

        /// <summary>
        /// Performs physics-related updates on the current state.
        /// Called at a fixed interval.
        /// </summary>
        public void FixedUpdate() {
            current?.State?.FixedUpdate();
        }

        /// <summary>
        /// Sets the state machine to the specified state and triggers the state's OnEnter method.
        /// </summary>
        /// <param name="state">The state to transition to.</param>
        public void SetState(IState state) {
            current = nodes[state.GetType()];
            current.State?.OnEnter();
        }

        /// <summary>
        /// Adds a transition from one state to another with a specified condition.
        /// </summary>
        /// <param name="from">The state from which the transition originates.</param>
        /// <param name="to">The target state of the transition.</param>
        /// <param name="condition">The condition that must be met for the transition to occur.</param>
        public void AddTransition(IState from, IState to, IPredicate condition) {
            GetOrAddNode(from).AddTransition(GetOrAddNode(to).State, condition);
        }

        /// <summary>
        /// Adds a transition that can occur from any state to a specified state, given a condition.
        /// </summary>
        /// <param name="to">The target state of the transition.</param>
        /// <param name="condition">The condition that must be met for the transition to occur.</param>
        public void AddAnyTransition(IState to, IPredicate condition) {
            anyTransitions.Add(new Transition(GetOrAddNode(to).State, condition));
        }

        /// <summary>
        /// Retrieves the <see cref="StateNode"/> for the given state, adding it to the state machine if it doesn't already exist.
        /// </summary>
        /// <param name="state">The state for which the corresponding <see cref="StateNode"/> is required.</param>
        /// <returns>The <see cref="StateNode"/> corresponding to the specified state.</returns>
        private StateNode GetOrAddNode(IState state) {
            if (!nodes.TryGetValue(state.GetType(), out var node)) {
                node = new StateNode(state);
                nodes.Add(state.GetType(), node);
            }

            return node;
        }

        /// <summary>
        /// Finds and returns the first valid transition based on the current state's transitions and any transitions.
        /// </summary>
        /// <returns>The first transition whose condition evaluates to true, or null if no valid transition is found.</returns>
        private ITransition? GetTransition() {
            foreach (var transition in anyTransitions) {
                if (transition.Condition.Evaluate())
                    return transition;
            }

            foreach (var transition in current!.Transitions) {
                if (transition.Condition.Evaluate())
                    return transition;
            }

            return null;
        }

        /// <summary>
        /// Changes the current state to the specified state if it's different from the current state.
        /// Triggers the OnExit method of the previous state and the OnEnter method of the new state.
        /// </summary>
        /// <param name="state">The new state to transition to.</param>
        private void ChangeState(IState state) {
            if (state == current?.State)
                return;

            var previousState = current?.State;
            var nextState = nodes[state.GetType()].State;

            previousState?.OnExit();
            nextState?.OnEnter();
            current = nodes[state.GetType()];
        }

        /// <summary>
        /// Represents a node in the state machine, holding a state and its associated transitions.
        /// </summary>
        /// <remarks>
        /// Initializes a new instance of the StateNode class with the specified state.
        /// </remarks>
        /// <param name="state">The state associated with this node.</param>
        private class StateNode {
            /// <summary>
            /// Gets the state associated with this node.
            /// </summary>
            public IState State { get; }

            /// <summary>
            /// Gets the set of transitions originating from this state.
            /// </summary>
            public HashSet<ITransition> Transitions { get; }

            public StateNode(IState state) {
                State = state;
                Transitions = new HashSet<ITransition>();
            }

            /// <summary>
            /// Adds a transition from this state to another state with a specified condition.
            /// </summary>
            /// <param name="to">The target state of the transition.</param>
            /// <param name="condition">The condition that must be met for the transition to occur.</param>
            public void AddTransition(IState to, IPredicate condition) {
                Transitions.Add(new Transition(to, condition));
            }
        }
    }

}
