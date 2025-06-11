namespace YooX.StateMachine {
    /// <summary>
    /// This interface defines a structure for managing different states in a state machine.
    /// It provides four methods to handle the lifecycle of a state, including initialization,
    /// per-frame updates, fixed-timestep updates, and cleanup on exit.
    /// </summary>
    public interface IState {
        /// <summary>
        /// Called when the state is first entered.
        /// Use this method to initialize or reset any necessary variables or components specific to this state.
        /// </summary>
        void OnEnter();

        /// <summary>
        /// Called every frame while the state is active.
        /// Implement this method to handle any state-specific logic that needs to be checked or executed every frame.
        /// </summary>
        void Update();

        /// <summary>
        /// Called at fixed time intervals, usually for physics updates.
        /// Use this method for consistent physics-related logic within the state.
        /// </summary>
        void FixedUpdate();

        /// <summary>
        /// Called when the state is exited.
        /// Implement this method to clean up or finalize any activities specific to this state before transitioning to another state.
        /// </summary>
        void OnExit();
    }

}
