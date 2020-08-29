using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameUISystem : SceneSingleton<GameUISystem>
{
    public Slider healthSlider;
    public Slider fuelSlider;
    public Transform homePointer;

    // Move this to separate script 
    public GameObject resultScreenPopup;
    public GameObject resultScreenContent;
    IEnumerator AddInOrderCouroutine;
    float time = 1f;

    void Start()
    {
        
    }

    public void UpdateHealthUI(float health)
    {
        healthSlider.value = health / 100;
    }

    public void UpdateFuelUI(float fuel)
    {
        fuelSlider.value = fuel / 100;
    }

    public void OpenResultScreenPopup(List<ItemWorld> items)
    {
        resultScreenPopup.SetActive(true);

        AddInOrderCouroutine = AddInOrder(items);
        StartCoroutine(AddInOrderCouroutine);
    }

    IEnumerator AddInOrder(List<ItemWorld> items)
    {
        //Debug.Log("moving?");
        foreach (ItemWorld i in items)
        {
            GameObject go = new GameObject();
            go.name = i.name;
            Image t = go.AddComponent<Image>();
            t.sprite = i.icon;
            go.GetComponent<RectTransform>().SetParent(resultScreenContent.transform);
            go.transform.localScale = new Vector3(1, 1, 1);
            go.SetActive(true);

            yield return new WaitForSeconds(time);
        }

        yield return null;
        
    }
}
