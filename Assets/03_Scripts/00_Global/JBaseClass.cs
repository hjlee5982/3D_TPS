using UnityEngine;

public abstract class JBaseClass : MonoBehaviour
{
    protected abstract void InitializeComponents();
    protected abstract void InitializeTransforms();
    protected abstract void InitializeValues();
}
