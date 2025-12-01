using UnityEngine;

public class ExperienceDrop : MonoBehaviour
{
    [SerializeField] private int totalExperience;
    [SerializeField] private int experiencePerOrb = 20;
    [SerializeField] private GameObject experienceOrbPrefab;
    
    private IDroppedItemManager droppedItemManager;

    public void GenerateExperienceDrop()
    {
        int orbCount = Mathf.CeilToInt((float)totalExperience / experiencePerOrb);

        for (int i = 0; i < orbCount; i++)
        {
            DropExperienceOrb();
        }
    }

    private void DropExperienceOrb()
    {
        if (droppedItemManager == null)
            droppedItemManager = ServiceLocator.Instance.Get<IDroppedItemManager>();
            
        Vector2 randomVelocity = new Vector2(Random.Range(-8, 8), Random.Range(15, 20));

        if (droppedItemManager != null)
            droppedItemManager.SpawnExperience(experiencePerOrb, transform.position, randomVelocity);
    }

    public void SetExperienceAmount(int amount)
    {
        totalExperience = amount;
    }
}
