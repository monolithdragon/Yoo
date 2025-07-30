using UnityEngine;

namespace YooX.SceneReferenceAttribute {
    public sealed class ValidatedMonoBehaviour : MonoBehaviour {
#if UNITY_EDITOR
        protected virtual void OnValidate()
        {
            this.ValidateRefs();
        }
#else
	    private void OnValidate() { }
#endif
    }
}
