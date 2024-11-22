using System.Reflection;
using UnityEngine;
using UnityEditor;

public class LightProbeTest : MonoBehaviour
{
    private int m_counter = 0;

    void Start()
    {
    }

    void Update()
    {
    }

    private void OnGUI()
    {
        if (GUI.Button(new Rect(0, 0, 200, 100), "Change Light"))
        {
            ChangeLight();
        }
    }

    private void ChangeLight()
    {
        m_counter++;
        m_counter %= 2;

        LightingDataAsset asset = null;
        switch (m_counter)
        {
            case 0:
                asset = AssetDatabase.LoadAssetAtPath<LightingDataAsset>("Assets/SwapLightmap/Scene1/LightingData.asset");
                break;
            case 1:
                asset = AssetDatabase.LoadAssetAtPath<LightingDataAsset>("Assets/SwapLightmap/Scene2/LightingData.asset");
                break;
        }

        SerializedObject so = new SerializedObject(asset);
        
        // 设置为 Debug 模式 //
        // 参考自 https://discussions.unity.com/t/how-get-lightprobes-from-a-lightningdata-asset/947990/4 //
        typeof(SerializedObject).GetProperty("inspectorMode", BindingFlags.Instance | BindingFlags.NonPublic).SetValue(so, InspectorMode.DebugInternal);

        // LightProbes //
        LightProbes probes = so.FindProperty("m_LightProbes").objectReferenceValue as LightProbes;
        LightmapSettings.lightProbes = probes;

        // Lightmaps //
        LightmapData lightmapData = new LightmapData();
        SerializedProperty lightmapsPty = so.FindProperty("m_Lightmaps");
        if (lightmapsPty != null)
        {
            for (int i = 0; i < lightmapsPty.arraySize; i++)
            {
                SerializedProperty lightmapDataPty = lightmapsPty.GetArrayElementAtIndex(i);
                if (lightmapDataPty != null)
                {
                    SerializedProperty light = lightmapDataPty.FindPropertyRelative("m_Lightmap");
                    if (light != null)
                    {
                        lightmapData.lightmapColor = light.objectReferenceValue as Texture2D;
                    }
                }
            }
        }
        LightmapData[] lightmaps = LightmapSettings.lightmaps;
        lightmaps[0] = lightmapData;
        LightmapSettings.lightmaps = lightmaps;
    }
}
