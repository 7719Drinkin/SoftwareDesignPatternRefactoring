using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chest : MonoBehaviour
{
    private Animator anim;
    public string chestId;
    public bool opened;

    [SerializeField] private int amountOfCurrency;
    [SerializeField] private ItemDrop itemDrop;
    [SerializeField] private int amountOfExperience;
    [SerializeField] private ExperienceDrop experienceDrop;

    private void Awake()
    {
        anim = GetComponent<Animator>();
    }

    [ContextMenu("Generate new id")]
    public void GenerateId()
    {
        chestId = System.Guid.NewGuid().ToString();
    }

    public void OpenChest()
    {
        if (opened) return;
        SetChestOpen();

        AudioManager.instance.PlaySFX(45);

        itemDrop.GenerateDrop();

        experienceDrop.SetExperienceAmount(amountOfExperience);
        experienceDrop.GenerateExperienceDrop();

        PlayerManager.instance.currency += amountOfCurrency;
    }

    public void SetChestOpen()
    {
        opened = true;
        anim.SetTrigger("Open");
    }
}
