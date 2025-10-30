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
    public List<ClothingObject> clothing = new List<ClothingObject>();

    private List<Renderer> baseRenderers;
    public Color SkinColor;
    public Color HairColor;
    
    public class ClothingPiece
    {
        public GameObject root;
        private List<Transform> bones;
        public ClothingObject obj;
        public Renderer[] renderers;
 
        public ClothingPiece(GameObject r, ClothingObject obj)
        {
            root = r;
            bones = r.transform.Find("Armature").RecursiveGetAllChildren();
            bones.Add(r.transform.Find("Armature"));
            renderers = root.GetComponentsInChildren<Renderer>();
            this.obj = obj;
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
        clothing = newC;
        FitClothing();
    }

    public void FitClothing(ClothingObject clothingObject)
    {
        foreach (var clothingPiece in Pieces)
        {
            if (clothingPiece.obj.Type == clothingObject.Type)
            {
                clothingPiece.Destroy();
                Pieces.Remove(clothingPiece);
                clothing.Remove(clothingPiece.obj);
                break;
            }

        }
        if (clothingObject && clothingObject.Prefab)
        {


            var o = Instantiate(clothingObject.Prefab, transform);
            var renderers = o.GetComponentsInChildren<Renderer>();
            foreach (var componentsInChild in renderers)
            {
                foreach (var material in componentsInChild.materials)
                {
                    material.SetColor(BaseColor,clothingObject.Color);
                }
            }
            var piece = new ClothingPiece(o,clothingObject);

            AlignBones(o);
            
            Pieces.Add(piece);
            if (clothingObject)
                clothing.Add(clothingObject);
            
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
                var renderers = o.GetComponentsInChildren<Renderer>();
                foreach (var componentsInChild in renderers)
                {
                    foreach (var material in componentsInChild.materials)
                    {
                        material.SetColor(BaseColor,clothingObject.Color);
                    }
                }
                var piece = new ClothingPiece(o, clothingObject);

                AlignBones(o);

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
            HairColor = DataController.saveData.HairColor;
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
            if (GetComponentInParent<ActorBehavior>()) 
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
        
        foreach (var clothingPiece in Pieces)
        {
            if (clothingPiece.obj.Type == ClothingObject.ClothingType.Hair)
            {
                foreach (var clothingPieceRenderer in clothingPiece.renderers)
                {
                    foreach (var material in clothingPieceRenderer.materials)
                    {
                        material.SetColor(BaseColor, HairColor);
                    }
                }
            }
        }
    }
}
