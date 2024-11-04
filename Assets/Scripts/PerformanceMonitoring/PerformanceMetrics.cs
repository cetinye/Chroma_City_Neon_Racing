// MIT License

// Copyright (c) 2022 anarkila - https://github.com/anarkila

// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:

// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.

// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.

using System.Collections.Generic;
using Unity.Profiling;
using UnityEngine.UI;
using UnityEngine;
using System;
using TMPro;

// Mostly garbage free* performance metrics for Unity 2022.1 or later.
// *garbage free after caching strings and as long as value is within bounds of cached string array!

// This uses Unity's ProfilerRecorder API
// https://docs.unity3d.com/ScriptReference/Unity.Profiling.ProfilerRecorder.html

// Setup:
// 1. Create empty GameObject
// 2. Add this script to it
// 3. Press Play

public class PerformanceMetrics : MonoBehaviour {

    [Range(0.2f, 1.0f)][SerializeField] private float updateFrequency = 0.3f;
    private GameObject textPrefab; // Alternatively make this public and drag your text prefab here!

    private List<PerformanceMetric> metrics = new List<PerformanceMetric>();
    private Transform parentTransform;
    private float nextUpdate = 0.0f;

    private void Awake() {
        if (TryGetComponent(out Canvas canvas)) {
            canvas.enabled = true;
        }
        parentTransform = gameObject.transform.GetChild(0).transform;
        CreateTextPrefab(new Vector2(400, 25), Color.white, 20);
        CreateStatistics();
    }

    private void OnEnable() {
        EnableProfilerRecorders();
    }

    private void OnDisable() {
        DisposeProfilerRecorders();
    }

    private void EnableProfilerRecorders() {
        for (int i = 0; i < metrics.Count; i++) {
            if (metrics[i] == null) continue;
            metrics[i].StartProfilerRecorder();
        }
    }

    private void DisposeProfilerRecorders() {
        for (int i = 0; i < metrics.Count; i++) {
            if (metrics[i] == null) continue;
            metrics[i].Dispose();
        }
    }

    private void LateUpdate() {
        if (Time.time > nextUpdate) {
            nextUpdate += updateFrequency;
            UpdateStatistics();
        }
    }

    private void UpdateStatistics() {
        for (int i = 0; i < metrics.Count; i++) {
            if (metrics[i] == null) continue;
            metrics[i].UpdateText();
        }
    }

    private void CreateTextPrefab(Vector2 rectWidthHeight, Color textColor, int fontsize = 20) {
        if (textPrefab != null && textPrefab.GetComponent<TextMeshProUGUI>() != null) {
            // return if TextPrefab provided and it has textPrefab / TMP_Text component
            return;
        }

        GameObject generatedPrefab = new GameObject("textprefab");

        var textComponent = generatedPrefab.AddComponent<TextMeshProUGUI>();
        textComponent.fontSize = fontsize;
        textComponent.color = textColor;

        var rectComp = generatedPrefab.GetComponent<RectTransform>();
        rectComp.sizeDelta = rectWidthHeight;

        textPrefab = generatedPrefab;
    }

    private void CreateStatistics() {
        if (textPrefab == null) {
#if UNITY_EDITOR
            Debug.LogError("Text textPrefab is null!");
#endif
            return;
        }

        // https://docs.unity3d.com/Manual/ProfilerCPU.html
        CreateTitle("CPU Statistics:", false);
        float maxStoredMilliseconds = 90.0f; // max stored millisecods from 0.00
        int maxMsUpdates = (int)(maxStoredMilliseconds * 100);

#if UNITY_2022_1_OR_NEWER
        metrics.Add(CreateNew(NumericType.FloatType, ProfilerCategory.Internal, "CPU Total Frame Time", maxMsUpdates, "CPU Total Frame: ", " ms", 0.0f, 0.01f, 100.0f));
        metrics.Add(CreateNew(NumericType.FloatType, ProfilerCategory.Internal, "CPU Main Thread Frame Time", maxMsUpdates, "CPU Main Thread: ", " ms", 0.0f, 0.01f, 100.0f));
        metrics.Add(CreateNew(NumericType.FloatType, ProfilerCategory.Internal, "CPU Render Thread Frame Time", maxMsUpdates, "CPU Render Thread: ", " ms", 0.0f, 0.01f, 100.0f));
#endif

        metrics.Add(CreateNew(NumericType.FloatType, ProfilerCategory.Internal, "FixedUpdate.PhysicsFixedUpdate", maxMsUpdates, "PhysicsFixedUpdate: ", " ms", 0.0f, 0.01f, 100.0f));
        metrics.Add(CreateNew(NumericType.FloatType, ProfilerCategory.Internal, "BehaviourUpdate", maxMsUpdates, "Update: ", " ms", 0.0f, 0.01f, 100.0f));
        metrics.Add(CreateNew(NumericType.FloatType, ProfilerCategory.Internal, "LateBehaviourUpdate", maxMsUpdates, "LateUpdate: ", " ms", 0.0f, 0.01f, 100.0f));
        metrics.Add(CreateNew(NumericType.FloatType, ProfilerCategory.Internal, "FixedBehaviourUpdate", maxMsUpdates, "FixedUpdate: ", " ms", 0.0f, 0.01f, 100.0f));


#if UNITY_2022_1_OR_NEWER
        // https://docs.unity3d.com/Manual/ProfilerRendering.html
        CreateTitle("Rendering Statistics:");
        metrics.Add(CreateNew(NumericType.FloatType, ProfilerCategory.Internal, "GPU Frame Time", maxMsUpdates, "GPU: ", " ms", 0.0f, 0.01f, 100.0f));

#if UNITY_EDITOR
        // these seem to work only in Editor!
        metrics.Add(CreateNew(NumericType.IntegerWithSkip, ProfilerCategory.Render, "Triangles Count", 1001, "Triangles: ", "k", 0.0f, 0.1f, 100.0f, 1, true, 101, "M"));
        metrics.Add(CreateNew(NumericType.IntegerWithSkip, ProfilerCategory.Render, "Vertices Count", 1001, "Vertices: ", "k", 0.0f, 0.1f, 100.0f, 1, true, 101, "M"));

        metrics.Add(CreateNew(NumericType.IntegerType, ProfilerCategory.Render, "Draw Calls Count", 3000, "Draw Calls: ", ""));
        metrics.Add(CreateNew(NumericType.IntegerType, ProfilerCategory.Render, "Batches Count", 3000, "Batches: ", ""));

        metrics.Add(CreateNew(NumericType.IntegerType, ProfilerCategory.Render, "SetPass Calls Count", 3000, "SetPass Calls: ", ""));
        metrics.Add(CreateNew(NumericType.IntegerType, ProfilerCategory.Render, "Shadow Casters Count", 3000, "Shadow Casters: ", ""));

        //metrics.Add(CreateNew(NumericType.IntegerType, ProfilerCategory.Render, "Dynamic Batched Draw Calls Count", 3000, "Dynamic Batched Draw Calls: ", ""));
        //metrics.Add(CreateNew(NumericType.IntegerType, ProfilerCategory.Render, "Static Batched Draw Calls Count", 3000, "Static Batched Draw Calls: ", ""));
        //metrics.Add(CreateNew(NumericType.IntegerType, ProfilerCategory.Render, "Instanced Batched Draw Calls Count", 3000, "Instanced Batched Draw Calls: ", ""));
#endif
#endif

        Destroy(textPrefab);
    }

    private void CreateTitle(string text, bool createEmpty = true) {
        if (createEmpty) Instantiate(textPrefab, parentTransform);
        var title = Instantiate(textPrefab, parentTransform);
        if (title.TryGetComponent(out TMP_Text textComp)) {
            textComp.fontStyle = FontStyles.Bold | FontStyles.Underline;
            textComp.text = text;
        }
    }

    private PerformanceMetric CreateNew(NumericType type, ProfilerCategory category, string profileMarkerName, int arrayLength, string prefix, string suffix, 
        float startVa = 0.0f, float floatInterval = 0.01f, float multiplier = 10.0f, int skipCount = 1, bool createSecondaryArray = false, 
        int secondaryArraylength = 100, string secondaryprefix = "M") {

        var newGO = Instantiate(textPrefab, parentTransform);
        PerformanceMetric newMetric = new PerformanceMetric();

        newMetric.go = newGO;
        newMetric.textComponent = newGO.GetComponent<TMP_Text>();
        newMetric.type = type;

        switch (type) {
            case NumericType.FloatType: 
                newMetric.cachedStrings = CreateStringArrayWithInterval(startVa, floatInterval, arrayLength, prefix, suffix, "0.00"); 
                break;
            case NumericType.IntegerType: 
                newMetric.cachedStrings = CreateStringArrayWithIntegers(0, arrayLength + 1, prefix, suffix); 
                break;
            case NumericType.IntegerWithSkip:
                newMetric.cachedStrings = CreateStringArrayWithSkip(0, arrayLength, prefix, suffix, skipCount);
                if (createSecondaryArray) newMetric.secondaryCachedStrings = CreateStringArrayWithSkip(0, secondaryArraylength, prefix, secondaryprefix, skipCount);
                break;
        }
        newMetric.statName = profileMarkerName;
        newMetric.category = category;
        newMetric.suffix = suffix;
        newMetric.prefix = prefix;
        newMetric.multiplier = multiplier;

        return newMetric;
    }

    private string[] CreateStringArrayWithIntegers(int startIndex, int endIndex, string prefix = "", string suffix = "") {
        var stringArray = new string[endIndex];
        for (int i = startIndex; i < endIndex; ++i) {
            stringArray[i] = prefix + i.ToString() + suffix;
        }
        return stringArray;
    }

    private string[] CreateStringArrayWithSkip(int startIndex, int endIndex, string prefix = "", string suffix = "", int count = 0, bool skipFirst = false) {
        int total = 0;
        var stringArray = new string[endIndex];
        for (int i = startIndex; i < endIndex; ++i) {
            if (!skipFirst && i != 0) total += count;
            stringArray[i] = prefix + total.ToString() + suffix;
        }
        return stringArray;
    }

    private string[] CreateStringArrayWithInterval(float startVal, float step, int endIndex, string preFix = "", string suffix = "", string decimalCount = "0.0") {
        var stringArray = new string[endIndex];
        for (int i = 0; i < stringArray.Length; i++) {
            var index = startVal.ToString(decimalCount);
            stringArray[i] = preFix + index + suffix;
            startVal += step;
        }
        return stringArray;
    }


#if UNITY_EDITOR
    private void OnValidate() {
        // Setup GameObject in Editor
        var canvas = GetComponent<Canvas>();
        if (canvas == null) {
            var canvass = this.gameObject.AddComponent<Canvas>();
            canvass.renderMode = RenderMode.ScreenSpaceOverlay;
            this.gameObject.transform.SetParent(null);
            this.gameObject.name = "PerformanceMetrics";
        }

        GameObject obj = null;
        var childObj = gameObject.transform.childCount;
        if (childObj == 0) {
            obj = new GameObject("Vertical");
            obj.transform.SetParent(this.transform);
        }
        else {
            obj = this.gameObject.transform.GetChild(0).gameObject;
        }

        if (obj.GetComponent<VerticalLayoutGroup>() == null) {
            obj.AddComponent<VerticalLayoutGroup>();
            var rect = obj.GetComponent<RectTransform>();
            rect.anchorMin = new Vector2(0, 1);
            rect.anchorMax = new Vector2(0, 1);
            rect.sizeDelta = new Vector2(100, 100);
            rect.anchoredPosition = new Vector2(65, -65);
        }
    }
#endif

    public class PerformanceMetric {
        public GameObject go;
        public TMP_Text textComponent;
        public string[] cachedStrings, secondaryCachedStrings;
        public ProfilerRecorder profilerRecorder;
        public ProfilerCategory category;
        public string prefix, suffix = "";
        public float multiplier = 10.0f;
        public NumericType type;
        public string statName;

        public void StartProfilerRecorder() {
            // https://docs.unity3d.com/ScriptReference/Unity.Profiling.ProfilerRecorder.StartNew.html
            profilerRecorder = ProfilerRecorder.StartNew(category, statName);
        }

        public void Dispose() {
            // https://docs.unity3d.com/ScriptReference/Unity.Profiling.ProfilerRecorder.Dispose.html
            profilerRecorder.Dispose();
        }

        public long GetCurrentValue() {
            if (!profilerRecorder.Valid || !profilerRecorder.IsRunning) StartProfilerRecorder();

            return profilerRecorder.CurrentValue;
        }

        public void UpdateText() {
            var value = GetCurrentValue();

            switch (type) {
                case NumericType.FloatType:
                    var ms = value / 1000000f;
                    float rounded = Mathf.Round(ms * multiplier);
                    int index = (int)rounded;
                    if (InBounds(index, cachedStrings.Length)) textComponent.text = cachedStrings[index];
                    else textComponent.text = prefix + ms.ToString("0.00") + suffix;
                    break;

                case NumericType.IntegerWithSkip:
                    if (value >= 1000000 && secondaryCachedStrings.Length != 0) {
                        int idx1 = Convert.ToInt32(value / 1000000);
                        if (InBounds(idx1, cachedStrings.Length)) textComponent.text = secondaryCachedStrings[idx1];
                        else textComponent.text = prefix + value + suffix;
                    }
                    else {
                        int idx2 = (int)Math.Ceiling((double)value / 1000);
                        if (InBounds(idx2, cachedStrings.Length)) textComponent.text = cachedStrings[idx2];
                        else textComponent.text = prefix + value + suffix;
                    }
                    break;

                case NumericType.IntegerType:
                    int idx = (int)value;
                    if (InBounds(idx, cachedStrings.Length)) textComponent.text = cachedStrings[idx];
                    else textComponent.text = prefix + value + suffix;
                    break;
            }
        }

        private bool InBounds(int index, int totalLength) {
            return (index >= 0) && (index < totalLength);
        }
    }

    public enum NumericType {
        IntegerType,            // 0, 1, 2, 3..
        FloatType,              // 0.01, 0.02, 0.03..
        IntegerWithSkip         // 0 - 1k - 2k - 3k...
    }
}