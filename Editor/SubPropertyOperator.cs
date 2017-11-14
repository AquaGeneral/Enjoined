using System.Collections;
using System.Collections.Generic;
using UnityEditor;

public class SubPropertyOperator : IEnumerator<SerializedProperty> {
    private SerializedProperty firstProperty;
    internal SerializedProperty iterator;
    private SerializedProperty lastProperty;

    public SubPropertyOperator(SerializedProperty serializedProperty) {
        firstProperty = serializedProperty.Copy();
        iterator = serializedProperty.Copy();
        lastProperty = serializedProperty.GetEndProperty();
    }

    public SerializedProperty Current {
        get {
            if(iterator == null) throw new System.InvalidOperationException();
            return iterator;
        }
    }

    object IEnumerator.Current {
        get {
            return Current;
        }
    }

    public void Dispose() {
        throw new System.NotImplementedException();
    }
    
    public bool MoveNext() {
        return iterator.NextVisible(true) && !SerializedProperty.EqualContents(iterator, lastProperty);
    }

    public void Reset() {
        iterator = firstProperty.Copy();
    }

    //public IEnumerator<SerializedProperty> GetEnumerator() {
    //    return new SubPropertyOperator()
    //}

    //IEnumerator IEnumerable.GetEnumerator() {
    //    return GetEnumerator();
    //}
}
