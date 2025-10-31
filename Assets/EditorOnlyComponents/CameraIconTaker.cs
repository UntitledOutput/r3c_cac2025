using System.Collections;
using System.IO;
using MyBox;
using ScriptableObj;
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

    public string ExportPath = "Sprites/camera_screenshot.png";

#if UNITY_EDITOR

    [ButtonMethod]
    public void TakeAll()
    {
        var clothing = Resources.LoadAll<ClothingObject>("Settings/Clothing");

        var head_mannequin = GameObject.Find("Model").transform.Find("head_mannequin");
        var full_mannequin = GameObject.Find("Model").transform.Find("full_mannequin");

        IEnumerator cycle()
        {
            foreach (var clothingObject in clothing)
            {
                ClothingController c;
                if (clothingObject.Type == ClothingObject.ClothingType.Hair ||
                    clothingObject.Type == ClothingObject.ClothingType.Hat)
                {
                    head_mannequin.gameObject.SetActive(true);
                    full_mannequin.gameObject.SetActive(false);
                    c = head_mannequin.GetComponentInChildren<ClothingController>();
                }
                else
                {
                    full_mannequin.gameObject.SetActive(true);
                    head_mannequin.gameObject.SetActive(false);
                    c = full_mannequin.GetComponentInChildren<ClothingController>();
                }
                

                c.clothing.Clear();
                c.clothing.Add(clothingObject);
                c.FitClothing();

                for (int i = 0; i < 3; i++)
                {
                    yield return new WaitForEndOfFrame();
                }
                
                ExportPath = $"Sprites/Clothing/{clothingObject.name}.png";
                
                Take();


                for (int i = 0; i < 3; i++)
                {
                    yield return new WaitForEndOfFrame();
                }
                // clothingObject.Colors.Clear();
                // var renderers = clothingObject.Prefab.GetComponentsInChildren<Renderer>();
                // foreach (var renderer1 in renderers)
                // {
                //     foreach (var renderer1SharedMaterial in renderer1.sharedMaterials)
                //     {
                //         if (renderer1SharedMaterial.HasColor("_BaseColor"))
                //             clothingObject.Colors.Add(renderer1SharedMaterial.GetColor("_BaseColor"));
                //     }
                // }
                
                clothingObject.Icon = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/"+ExportPath);
                
                EditorUtility.SetDirty(clothingObject);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
                
                for (int i = 0; i < 3; i++)
                {
                    yield return new WaitForEndOfFrame();
                }

            }
        }

        StartCoroutine(cycle());
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
        string filePath = Path.Combine(Application.dataPath, ExportPath);
        File.WriteAllBytes(filePath, bytes);
        AssetDatabase.Refresh();
    }
#endif
}
