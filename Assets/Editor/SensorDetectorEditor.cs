using UnityEditor;
using UnityEngine;


[CustomEditor(typeof(SensorDetector))]
public class SensorDetectorEditor : Editor
{
    private Color tangerine = new Color(0.9764705882f, 0.5058823529f, 0.1647058824f, 0.2f);
    private Color bronze = new Color(0.6941176471f, 0.337254902f, 0.15f, 0.2f);
    private Color lk = new Color(0.9764705882f, 0.5058823529f, 0.37058824f, 0.2f);
    private Color k = new Color(0.3176471f, 0.2902f, 0.15f, 0.1f);
    private void OnSceneGUI()
    {
        SensorDetector sensorDetector = (SensorDetector)target;
        Handles.color = new Color(0.9764705882f, 0.5058823529f, 0.1647058824f, 0.15f);
        Handles.DrawSolidDisc(sensorDetector.transform.position, Vector3.up, sensorDetector.range);

        Handles.color = new Color(0.9764705882f, 0.5058823529f, 0.37058824f, 0.15f);;
        Handles.DrawSolidDisc(sensorDetector.transform.position, Vector3.up, sensorDetector.range / 2);
        
        Handles.color = new Color(0.9764705882f, 0.058823529f, 0.37058824f, 0.15f);
        Handles.DrawSolidDisc(sensorDetector.transform.position, Vector3.up, sensorDetector.range * 1.5f);

        if (sensorDetector.Detected)
        {
            if (sensorDetector.IsWalking)
            {
                Handles.color = new Color(0.9764705882f, 0.5058823529f, 0.1647058824f, 0.25f);
                Handles.DrawSolidDisc(sensorDetector.transform.position, Vector3.up, sensorDetector.range);
            }
            else if (sensorDetector.IsRunning)
            {
                Handles.color = new Color(0.9764705882f, 0.058823529f, 0.37058824f, 0.25f);
                Handles.DrawSolidDisc(sensorDetector.transform.position, Vector3.up, sensorDetector.range * 1.5f);
            }
            else if (sensorDetector.IsSneaking)
            {
                Handles.color = new Color(0.9764705882f, 0.5058823529f, 0.37058824f, 0.25f);;
                Handles.DrawSolidDisc(sensorDetector.transform.position, Vector3.up, sensorDetector.range / 2);
            }
        }
    }
}