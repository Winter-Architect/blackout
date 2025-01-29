using UnityEditor;
using UnityEngine;


[CustomEditor(typeof(SensorDetector))]
public class SensorDetectorEditor : Editor
{
    private Color tangerine = new Color(0.9764705882f, 0.5058823529f, 0.1647058824f);
    private Color bronze = new Color(0.6941176471f, 0.337254902f, 0.15f);

    private void OnSceneGUI()
    {
        SensorDetector sensorDetector = (SensorDetector)target;
        Handles.color = tangerine;
        Handles.DrawWireArc(sensorDetector.transform.position, Vector3.up, Vector3.forward, 360, sensorDetector.range);


        if (sensorDetector.Detected)
        {
            Debug.Log("here");
            Handles.color = bronze;
            Handles.DrawWireArc(sensorDetector.transform.position, Vector3.up, Vector3.forward, 360,
                sensorDetector.range);
        }
    }
}