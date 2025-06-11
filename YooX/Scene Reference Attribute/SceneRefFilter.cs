namespace YooX.SceneReferenceAttribute {
    public abstract class SceneRefFilter {
        public abstract bool IncludeSceneRef(object obj);
    }

    public abstract class SceneRefFilter<T> : SceneRefFilter where T : class {

        public override bool IncludeSceneRef(object obj)
            => IncludeSceneRef((T)obj);

        /// <summary>
        /// Returns true if the given object should be included as a reference.
        /// </summary>
        public abstract bool IncludeSceneRef(T obj);
    }

}
