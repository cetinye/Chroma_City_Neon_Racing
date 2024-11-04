using Unity.Profiling;
using UnityEngine;

public class SimpleProfilerRecorderExample : MonoBehaviour {

    private ProfilerRecorder UpdateBehaviour;

    private void OnEanble() {
        // https://docs.unity3d.com/ScriptReference/Unity.Profiling.ProfilerRecorder.StartNew.html
        UpdateBehaviour = ProfilerRecorder.StartNew(ProfilerCategory.Internal, "BehaviourUpdate");
    }
    
     public void OnDisable() {
        // https://docs.unity3d.com/ScriptReference/Unity.Profiling.ProfilerRecorder.Dispose.html
        UpdateBehaviour.Dispose();
    }

    private void Update() {
        // https://docs.unity3d.com/ScriptReference/Unity.Profiling.ProfilerRecorder.CurrentValue.html
        var currentValue = UpdateBehaviour.CurrentValue;

        // for 'BehaviourUpdate' stat ProfilerRecorder.CurrentValue returns in nanoseconds so this makes it in milliseconds
        var toMilliseconds = currentValue / 100000f;

        // Log the result that Unity Engine takes to execute *all* Update() functions.
        Debug.Log("Update() takes " + toMilliseconds + " ms");
    }
}