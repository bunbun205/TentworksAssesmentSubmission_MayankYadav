using UnityEngine;

public enum IngredientType
{
    Vegetable,
    Cheese,
    Meat
}

public enum PrepType
{
    None,
    Chop,
    Cook
}

[CreateAssetMenu(fileName = "IngredientData", menuName = "Scriptable Objects/IngredientData")]
public class IngredientData : ScriptableObject
{
    [SerializeField] private IngredientType type;

    [Header("Prep")] 
    [SerializeField] private bool prepRequired;
    [SerializeField] private float prepTime;
    [SerializeField] private PrepType prepType;

    [Header("Scoring")] 
    [SerializeField] private int score;

    [Header("Visual")] 
    [SerializeField] private Material unpreppedMat;
    [SerializeField] private Material preppedMat;
    [SerializeField] private Sprite icon;

    public IngredientType Type => type;
    public bool PrepRequired => prepRequired;
    public float PrepTime => prepTime;
    public PrepType PrepType => prepType;
    public int Score => score;
    public Material UnpreppedMat => unpreppedMat;
    public Material PreppedMat => preppedMat;
    public Sprite Icon => icon;
}
