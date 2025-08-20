using UnityEngine;

public enum CategoryType { Math, Literature, Science }

public class Trashcan : MonoBehaviour
{
    [Header("Assign the category of this trashcan in Inspector")] // waay dun pulos hehe
    public CategoryType category;
}
