using System.Collections.Generic;
using UnityEngine;

public enum AfterImageType
{
    Dash,
    Assassinate,
    Jump
}

public class AfterImageManager : MonoBehaviour
{
    public static AfterImageManager instance { get; private set; }

    public GameObject dashAfterImagePrefab;
    public GameObject assassinateAfterImagePrefab;
    public GameObject jumpAfterImagePrefab;

    public int poolSizePerType = 20;

    private Dictionary<AfterImageType, Queue<GameObject>> afterImagePools = new Dictionary<AfterImageType, Queue<GameObject>>();

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
    }

    void Start()
    {
        // 初始化不同类型的对象池
        InitializePool(AfterImageType.Dash, dashAfterImagePrefab);
        InitializePool(AfterImageType.Assassinate, assassinateAfterImagePrefab);
        InitializePool(AfterImageType.Jump, jumpAfterImagePrefab);
    }

    private void InitializePool(AfterImageType type, GameObject prefab)
    {
        Queue<GameObject> pool = new Queue<GameObject>();
        for (int i = 0; i < poolSizePerType; i++)
        {
            GameObject afterImage = Instantiate(prefab);
            afterImage.SetActive(false);
            pool.Enqueue(afterImage);
        }
        afterImagePools[type] = pool;
    }

    public void CreateAfterImage(Sprite sprite, Vector3 position, bool facingRight, AfterImageType type)
    {
        if (afterImagePools.ContainsKey(type) && afterImagePools[type].Count > 0)
        {
            GameObject afterImage = afterImagePools[type].Dequeue();
            afterImage.SetActive(true);
            afterImage.GetComponent<AfterImage>().Initialize(sprite, position, facingRight);

            // 延迟回收到池中
            StartCoroutine(ReturnToPool(afterImage, 0.1f, type));
        }
    }

    System.Collections.IEnumerator ReturnToPool(GameObject obj, float delay, AfterImageType type)
    {
        yield return new WaitForSeconds(delay);
        obj.SetActive(false);
        afterImagePools[type].Enqueue(obj);
    }
}

