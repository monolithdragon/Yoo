using UnityEngine;

namespace YooX.StateMachine {
    public abstract class State<T> : IState {
        protected readonly T controller;
        protected readonly Animator animator;

        protected const float crossFadeDuration = 0.1f;

        protected State(T controller, Animator animator) {
            this.controller = controller;
            this.animator = animator;
        }

        public virtual void OnEnter() { }
        public virtual void Update() { }
        public virtual void FixedUpdate() { }
        public virtual void OnExit() { }
    }

}
