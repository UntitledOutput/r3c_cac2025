using System.IO;
using MyBox;
using UnityEditor;
using UnityEngine;
using UnityEngine.Experimental.Rendering;

public class CameraIconTaker : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    [ButtonMethod]
    public void Take()
    {
        var targetCamera = GetComponent<Camera>();
        var width = 256;
        var height = 256;
        
        RenderTexture rt = new RenderTexture(width, height, 24);
        targetCamera.targetTexture = rt;
        Texture2D screenShot = new Texture2D(width, height, TextureFormat.RGBA32, false);
        targetCamera.Render(); // Render the camera's view to the Render Texture
        RenderTexture.active = rt; // Set the active Render Texture
        screenShot.ReadPixels(new Rect(0, 0, width, height), 0, 0); // Read pixels from the Render Texture
        targetCamera.targetTexture = null; // Clear the target texture
        RenderTexture.active = null; // Clear the active Render Texture
        Destroy(rt); // Destroy the temporary Render Texture

        byte[] bytes = screenShot.EncodeToPNG();
        string filePath = Path.Combine(Application.dataPath, "Sprites/camera_screenshot.png");
        File.WriteAllBytes(filePath, bytes);
        AssetDatabase.Refresh();
    }
}
