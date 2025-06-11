using System;
using UnityEngine;

namespace YooX.SceneReferenceAttribute {
    /// <summary>
    /// Allows for serializing Interface types with [SceneRef] attributes.
    /// </summary>
    /// <typeparam name="T">Component type to find and serialize.</typeparam>
    [Serializable]
    public class Interface<T> : ISerializable<T> where T : class {
        [SerializeField] private Component? implementer;

        private bool _hasCast;
        private T? _value;

        /// <summary>
        /// The serialized interface value.
        /// </summary>
        public T Value {
            get {
                if (!_hasCast) {
                    _hasCast = true;
                    _value = implementer as T;
                }
                return _value!;
            }
        }

        object ISerializable.SerializedObject
            => implementer!;

        public Type RefType => typeof(T);

        public bool HasSerializedObject => implementer != null;

        bool ISerializable.OnSerialize(object value) {
            Component c = (Component)value;
            if (c == implementer)
                return false;

            _hasCast = false;
            _value = null;
            implementer = c;
            return true;
        }

        void ISerializable.Clear() {
            _hasCast = false;
            _value = null;
            implementer = null;
        }
    }

}
