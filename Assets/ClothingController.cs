using System;
using System.Collections.Generic;
using System.Linq;
using DefaultNamespace;
using External;
using NUnit.Framework;
using ScriptableObj;
using UnityEngine;

public class ClothingController : MonoBehaviour
{
    private static readonly int BaseColor = Shader.PropertyToID("_BaseColor");

    [SerializeField]
    private List<ClothingObject> clothing = new List<ClothingObject>();

    private List<Renderer> baseRenderers;
    public Color SkinColor;
    
    public class ClothingPiece
    {
        private GameObject root;
        private List<Transform> bones;
        public ClothingObject.ClothingType type;

        public ClothingPiece(GameObject r, ClothingObject.ClothingType type)
        {
            root = r;
            bones = r.transform.Find("Armature").RecursiveGetAllChildren();
            bones.Add(r.transform.Find("Armature"));
            this.type = type;
        }

        public void Destroy()
        {
            GameObject.Destroy(root);
            foreach (var bone in bones)
            {
                if (bone)
                {
                    GameObject.Destroy(bone.gameObject);
                }
            }
        }
    }

    private List<ClothingPiece> Pieces = new List<ClothingPiece>();
    private bool _needsToRecalculate = false;
    private Dictionary<string, Transform> baseChildren = new Dictionary<string, Transform>();

    public void AlignBones(GameObject o)
    {
        var clothingChildren = o.transform.Find("Armature").RecursiveGetAllChildren();

        foreach (var clothingChild in clothingChildren)
        {
            //Debug.Log($"Aligning child {clothingChild}");
            var corresponding = baseChildren[clothingChild.name];

            clothingChild.SetParent(corresponding);
            clothingChild.localPosition = Vector3.zero;
            clothingChild.localRotation = Quaternion.identity;
            //clothingChild.localScale = Vector3.one;
        }
    }

    public void ChangeClothing(List<ClothingObject> newC)
    {
        var new_elements = newC.Except(clothing);
        
        foreach (var newElement in new_elements)
        {
            FitClothing(newElement);
        }

        clothing = newC;
    }

    public void FitClothing(ClothingObject clothingObject)
    {
        if (clothingObject && clothingObject.Prefab)
        {
            var pre_remove = DateTime.UtcNow;
            foreach (var clothingPiece in Pieces)
            {
                if (clothingPiece.type == clothingObject.Type)
                {
                    clothingPiece.Destroy();
                    Pieces.Remove(clothingPiece);
                    break;
                }

            }

            var pre_instantiate = DateTime.UtcNow;
            
            Debug.Log($"removed in {(pre_instantiate - pre_remove).TotalSeconds}");
            
            var o = Instantiate(clothingObject.Prefab, transform);

            var pre_piece = DateTime.UtcNow;
            Debug.Log($"instantiated in {(pre_piece - pre_instantiate).TotalSeconds}");

            var piece = new ClothingPiece(o,clothingObject.Type);
            
            var pre_align = DateTime.UtcNow;
            Debug.Log($"pieced in {(pre_align - pre_piece).TotalSeconds}");
            
            AlignBones(o);
            
            var pre_add = DateTime.UtcNow;
            Debug.Log($"aligned in {(pre_add - pre_align).TotalSeconds}");
            
            Pieces.Add(piece);
            
            _needsToRecalculate = true;
        }

    }

    public void FitClothing()
    {
        foreach (var clothingPiece in Pieces)
        {
            clothingPiece.Destroy();
        }

        Pieces.Clear();

        foreach (var clothingObject in clothing)
        {
            if (clothingObject && clothingObject.Prefab)
            {
                var o = Instantiate(clothingObject.Prefab, transform);
                AlignBones(o);
                var piece = new ClothingPiece(o,clothingObject.Type);
                Pieces.Add(piece);
            }
        }

        _needsToRecalculate = true;
    }

// Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        var baseChildren = gameObject.transform.Find("Armature").RecursiveGetAllChildren();
        
        foreach (var baseChild in baseChildren)
        {
            this.baseChildren.TryAdd(baseChild.name, baseChild);
        }
        baseRenderers = GetComponentsInChildren<Renderer>().ToList();

        if (GetComponentInParent<PlayerController>())
        {
            clothing = DataController.saveData.BuildListOfClothing();
            SkinColor = DataController.saveData.SkinColor;
        }

        FitClothing();

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
        if (_needsToRecalculate)
        {
            GetComponentInParent<ActorBehavior>().RecaptureRenderers();
            _needsToRecalculate = false;
        }

        foreach (var baseRenderer in baseRenderers)
        {
            foreach (var baseRendererMaterial in baseRenderer.materials)
            {
                baseRendererMaterial.SetColor(BaseColor, SkinColor);
            }
        }
    }
}
