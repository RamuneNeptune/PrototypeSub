using UnityEngine;

namespace PrototypeSubMod.UI.AbilitySelection;

public class IconDistributor : MonoBehaviour
{
    [SerializeField] private Sprite[] icons;
    [SerializeField] private GameObject radialIconPrefab;
    [SerializeField] private float distanceFromCenter;

    private float increment;

    private void Start()
    {
        increment = 360f / icons.Length;
        for (int i = 0; i < icons.Length; i++)
        {
            var icon = Instantiate(radialIconPrefab, transform);
            icon.GetComponent<RadialIcon>().SetSprite(icons[i]);
            float x = Mathf.Cos(increment * i * Mathf.Deg2Rad) * distanceFromCenter;
            float y = Mathf.Sin(increment * i * Mathf.Deg2Rad) * distanceFromCenter;
            icon.transform.localPosition = new Vector2(x, y);
        }
    }

    public float GetIncrement()
    {
        return increment;
    }

    public GameObject GetIconClosestToAngle(float angle)
    {
        float incrementIndex = Mathf.RoundToInt(angle / increment);
        return transform.GetChild((int)incrementIndex % transform.childCount).gameObject;
    }
}
