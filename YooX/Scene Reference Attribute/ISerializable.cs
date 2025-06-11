using System;

namespace YooX.SceneReferenceAttribute {
    public interface ISerializable {
        Type RefType { get; }
        object SerializedObject { get; }
        bool HasSerializedObject { get; }

        /// <summary>
        /// Callback for serialization.
        /// </summary>
        /// <param name="value">Object to serialize.</param>
        /// <returns>True if the value has changed.</returns>
        bool OnSerialize(object value);
        void Clear();
    }

    public interface ISerializable<T> : ISerializable where T : class {
        T Value { get; }
    }

}
