using Oculus.Interaction;
using Oculus.Interaction.Surfaces;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class InventoryManager : MonoBehaviour
{
    [Header("Inventory Slot")]
    public List<Collectable> initialItems;
    public GameObject[] slots;
    public Dictionary<int, Collectable> collectables;

    [Header("Item Pickup")]
    public int pickedItemIndex = -1;
    public GameObject pickedItemPosition;
    private GameObject pickedItem;

    [Header("Item Zoom In")]
    public bool isItemZoomed = false;
    public Transform zoomedItemPosition;
    public Transform originalCameraPosition;
    public Camera mainCamera;

    private GameObject player;

    private void Start()
    {
        player = FindAnyObjectByType<Player>().gameObject;
        pickedItemPosition = player.transform.Find("PickedItemPosition").gameObject;
        zoomedItemPosition = player.transform.Find("ZoomedItemPosition");
        originalCameraPosition = player.transform.Find("OriginalCameraPosition");

        collectables = new Dictionary<int, Collectable>();

        for (int i = 0; i < slots.Length; i++) 
        {
            Image itemImage = slots[i].transform.Find("Item").GetComponent<Image>();
            if (i < initialItems.Count)
            {
                Collectable item = initialItems[i];
                itemImage.sprite = item.Icon;
                itemImage.enabled = true;
                collectables[i] = item;
            }
            else
            {
                itemImage.sprite = null;
                itemImage.enabled = false;
            }
        }
    }

    public void AddItem(Collectable item, GameObject obj)
    {
        bool itemAdded = false;

        for (int i = 0;i < slots.Length;i++)
        {
            Image itemImage = slots[i].transform.Find ("Item").GetComponent<Image>();
            if(itemImage.sprite == null)
            {
                itemImage.sprite = item.Icon;
                collectables[i] = item;
                itemImage.enabled = true;
                itemAdded = true;
                break;
            }
        }

        if (itemAdded)
        {
            Debug.Log("아이템 추가 완료.");
            obj.SetActive(false);
        }
        else
        {
            Debug.Log("아이템 창이 가득 찼습니다.");
        }
    }

    public void RemoveItem(int index)
    {
        if (index >= 0 && index < slots.Length)
        {
            Image itemImage = slots[index].transform.Find("Item").GetComponent<Image>();
            itemImage.sprite = null;
            itemImage.enabled = false;

            if (collectables.ContainsKey(index))
            {
                collectables.Remove(index);
            }

            for (int i = index; i < slots.Length - 1; i++)
            {
                Image currentItemImage = slots[i].transform.Find("Item").GetComponent<Image>();
                Image nextItemImage = slots[i + 1].transform.Find("Item").GetComponent<Image>();

                if (nextItemImage.sprite != null)
                {
                    currentItemImage.sprite = nextItemImage.sprite;
                    currentItemImage.enabled = true;
                    
                    if (collectables.ContainsKey(i + 1))
                    {
                        collectables[i] = collectables[i + 1];
                    }
                    else
                    {
                        collectables.Remove(i);
                        break;
                    }
                }
                else
                {
                    currentItemImage.sprite = null;
                    currentItemImage.enabled = false;
                    collectables.Remove(i);
                    break;
                }
            }

            Image lastItemImage = slots[^1].transform.Find("Item").GetComponent<Image>();
            lastItemImage.sprite = null;
            lastItemImage.enabled = false;
            collectables.Remove(slots.Length - 1);
        }
    }

    public void RemoveItemByName(string itemName)
    {
        for (int i = 0; i < slots.Length; i++)
        {
            Image itemImage = slots[i].transform.Find("Item").GetComponent<Image>();
            if (itemImage.sprite != null && itemImage.sprite.name == itemName)
            {
                itemImage.sprite = null;
                itemImage.enabled = false;
                if (collectables.ContainsKey(i))
                {
                    collectables.Remove(i);
                }
                break;
            }
        }
    }

    public void PickItem(int index)
    {
        if (index < 0 || index >= collectables.Count)
        {
            Debug.LogError("Index Error: Inventory Manager Pick Item Index Error.");
            return;
        }
        // 프리팹 가져오기
        Collectable prefab = collectables[index];

        if (prefab == null)
        {
            Debug.LogError("Reference Error: Inventory Manager Pick Item Prefab is Null.");
            return;
        }
        // 기존 아이템 삭제
        if (pickedItem != null)
        {
            Destroy(pickedItem);
        }
        // 생성
        pickedItem = Instantiate(prefab.gameObject, pickedItemPosition.transform);
        pickedItem.transform.SetPositionAndRotation(pickedItemPosition.transform.position, Quaternion.identity);
        pickedItem.name = prefab.name;
        // 속성 제거
        Rigidbody rb = pickedItem.GetComponent<Rigidbody>();
        InteractableUnityEventWrapper IUEW = pickedItem.GetComponent<InteractableUnityEventWrapper>();
        ColliderSurface colliderSurface = pickedItem.GetComponent<ColliderSurface>();
        RayInteractable rayInteractable = pickedItem.GetComponent<RayInteractable>();
        Collider[] colliders = pickedItem.GetComponents<Collider>();

        Destroy(rb);
        Destroy(IUEW);
        Destroy(colliderSurface);
        Destroy(rayInteractable);
        foreach (Collider collider in colliders)
        {
            Destroy(collider);
        }
        // 활성화
        pickedItemIndex = index;
        pickedItem.SetActive(true);
        pickedItemPosition.SetActive(true);
    }

    public Collectable PutItem(Transform putItemPosition)
    {
        
        Collectable obj = collectables[pickedItemIndex];
        
        GameObject putItem = Instantiate(obj.gameObject, putItemPosition);
        putItem.transform.SetPositionAndRotation(putItemPosition.position, Quaternion.identity);
        putItem.name = obj.name;

        Rigidbody rb = putItem.GetComponent<Rigidbody>();
        rb.isKinematic = true;

        putItem.SetActive(true);

        DestroyItem(pickedItemIndex);
        DeselectItem();

        return putItem.GetComponent<Collectable>();
    }

    public void DropItem(int index, Vector3 dropPosition)
    {
        if (index >= 0 && index < slots.Length)
        {
            Collectable dropItem = collectables[index];
            
            if (dropItem != null)
            {
                dropItem.gameObject.SetActive(true);

                dropItem.transform.position = dropPosition;
                Rigidbody rb = dropItem.GetComponent<Rigidbody>();
                rb.isKinematic = false;

                RemoveItem(index);
                DeselectItem();
            }
        }
    }

    public void DeselectItem()
    {
        pickedItemIndex = -1;
        Destroy(pickedItem);
        pickedItemPosition.SetActive(false);
    }

    public void DestroyItem(int index)
    {
        if (index >= 0 && index < slots.Length)
        {
            Collectable item = collectables[index];

            if (item != null)
            {   
                Destroy(item.gameObject);

                RemoveItem(index);
            }
        }
    }

    public void ZoomItem()
    {
        isItemZoomed = true;
        mainCamera.transform.position = zoomedItemPosition.position;
    }

    public void UnzoomItem()
    {
        isItemZoomed = false;
        mainCamera.transform.position = originalCameraPosition.position;
    }

    public Collectable GetItemByIndex(int index)
    {
        return collectables[index];
    }

    public int GetItemCount()
    {
        return collectables.Count;
    }
}
