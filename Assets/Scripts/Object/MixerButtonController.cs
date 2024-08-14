using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MixerButtonController : MonoBehaviour
{
    public Putable leftInput;
    public Putable rightInput;
    public Transform leftPutItemPosition;
    public Transform rightPutItemPosition;
    public GameObject Kerosene;
    public GameObject RocketFuel;
    public MixerCoverController cover;
    public Renderer Alert;

    private float range = 5f;

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, range))
            {
                if (hit.transform == transform)
                {
                    CheckValidate();
                }
            }
        }
    }

    public void CheckValidate()
    {
        if (cover.isClosed)
        {
            if ((leftInput.putItem.Name == "분자 6" && rightInput.putItem.Name == "분자 3") || (leftInput.putItem.Name == "분자 3" && rightInput.putItem.Name == "분자 6"))
            {
                MixComplete(Kerosene);
            }
            else if ((leftInput.putItem.Name == "등유" && rightInput.putItem.Name == "액화 산소") || (leftInput.putItem.Name == "액화 산소" && rightInput.putItem.Name == "등유"))
            {
                MixComplete(RocketFuel);
            }
            else
            {
                Alert.material.color = Color.red;
            }
        }
        else
        {
            Alert.material.color = Color.red;
        }
    }

    private void MixComplete(GameObject result)
    {
        Alert.material.color = Color.green;

        if (leftPutItemPosition.childCount > 0)
        {
            int leftItemIndex = leftPutItemPosition.childCount - 1;
            Transform leftPutItem = leftPutItemPosition.GetChild(leftItemIndex);
            Destroy(leftPutItem.gameObject);
        }
        leftInput.RemovePutItem();

        if (rightPutItemPosition.childCount > 0)
        {
            int rightItemIndex = rightPutItemPosition.childCount - 1;
            Transform rightPutItem = rightPutItemPosition.GetChild(rightItemIndex);
            Destroy(rightPutItem.gameObject);
        }
        leftInput.RemovePutItem();

        result.SetActive(true);
    }
}
