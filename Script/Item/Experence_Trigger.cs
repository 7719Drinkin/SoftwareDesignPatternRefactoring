using UnityEngine;

public class Experence_Trigger : MonoBehaviour
{
    private ExperienceObject experienceObject => GetComponentInParent<ExperienceObject>();

    private bool picked;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponent<Player>() != null && !collision.GetComponent<PlayerStats>().isDead && !picked)
        {
            experienceObject.PickUpItem();
            picked = true;
        }
    }
}
