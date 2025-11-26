using UnityEngine;

public class ExperienceDrop : MonoBehaviour
{
    [SerializeField] private int totalExperience;
    [SerializeField] private int experiencePerOrb = 20;
    [SerializeField] private GameObject experienceOrbPrefab;

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
        Vector2 randomVelocity = new Vector2(Random.Range(-8, 8), Random.Range(15, 20));

        if (DroppedItemManager.instance != null)
            DroppedItemManager.instance.SpawnExperience(experiencePerOrb, transform.position, randomVelocity);
    }

    public void SetExperienceAmount(int amount)
    {
        totalExperience = amount;
    }
}
