using System.Collections.Generic;
using System.Linq;
using DefaultNamespace;
using External;
using NUnit.Framework;
using ScriptableObj;
using UnityEngine;

public class ClothingController : MonoBehaviour
{
    [SerializeField]
    private List<ClothingObject> clothing = new List<ClothingObject>();

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
            foreach (var clothingPiece in Pieces)
            {
                if (clothingPiece.type == clothingObject.Type)
                {
                    Pieces.Remove(clothingPiece);
                    clothingPiece.Destroy();
                    break;
                }

            }
            var o = Instantiate(clothingObject.Prefab, transform);
            AlignBones(gameObject, o);
            var piece = new ClothingPiece(o,clothingObject.Type);
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
                AlignBones(gameObject, o);
                var piece = new ClothingPiece(o,clothingObject.Type);
                Pieces.Add(piece);
            }
        }

        _needsToRecalculate = true;
    }

// Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (GetComponentInParent<PlayerController>())
            clothing = DataController.saveData.BuildListOfClothing();
        
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
    }
}
