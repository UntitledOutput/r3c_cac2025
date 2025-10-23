using System.Collections.Generic;
using System.Linq;
using External;
using UnityEngine;

public class ClothingController : MonoBehaviour
{
    public List<GameObject> clothing;
    public Shader OutlineShader;

    public static void AlignBones(GameObject baseObject, GameObject clothingObject)
    {
        var baseChildren = baseObject.transform.Find("Armature").RecursiveGetAllChildren();
        var clothingChildren = clothingObject.transform.Find("Armature").RecursiveGetAllChildren();
        
        foreach (var clothingChild in clothingChildren)
        {
            var corresponding = baseChildren.Where((transform1 => transform1.name == clothingChild.name)).First();
            
            clothingChild.SetParent(corresponding);
            clothingChild.localPosition = Vector3.zero;
            clothingChild.localRotation = Quaternion.identity;
            //clothingChild.localScale = Vector3.one;
        }
    }
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        foreach (var o in clothing)
        {
            AlignBones(gameObject,o);
            o.transform.SetParent(transform);
        }

        /*transform.RemoveAllChildrenIf("OutlineRenderer");
        
        var skinnedMeshRenderers = gameObject.GetComponentsInChildren<SkinnedMeshRenderer>();
        foreach (var skinnedMeshRenderer in skinnedMeshRenderers)
        {
            var outlineClone = Instantiate(skinnedMeshRenderer.gameObject, skinnedMeshRenderer.transform.parent).GetComponent<SkinnedMeshRenderer>();
            outlineClone.name = "OutlineRenderer";
            foreach (var material in outlineClone.materials)
            {
                material.shader = OutlineShader;
            }
        }*/
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
