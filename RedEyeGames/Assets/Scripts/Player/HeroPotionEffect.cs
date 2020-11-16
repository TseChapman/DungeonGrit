using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Health doesn't count
public enum PotionEffect
{
    SPEED_EFFECT = 0,
    ARMOR_EFFECT = 1,
    GOD_EFFECT = 2,
    NUM_EFFECT = 3
}

public class HeroPotionEffect : MonoBehaviour
{
    private HeroMovement mHeroMovement;
    private Health mHealth;
    private InventoryManager mInventoryManager;

    [SerializeField] private PotionEffect mCurrentEffect;
    private bool mIsActive = false;
    private float mEffectTimer = 0f;

    public void SetPotionEffect(PotionEffect effect)
    {
        mCurrentEffect = effect;
    }

    public void SetActive(bool isActive)
    {
        mIsActive = isActive;
    }

    public void SetEffectTimer(float timer)
    {
        mEffectTimer = timer;
    }

    // Start is called before the first frame update
    private void Start()
    {
        mInventoryManager = GameObject.FindObjectOfType<InventoryManager>();
        mHealth = GetComponent<Health>();
    }

    // Update is called once per frame
    private void Update()
    {
        mEffectTimer -= Time.smoothDeltaTime;
    }

    private void CheckEffect()
    {
        if (mIsActive is false)
        {
            // Reset all hero parameter and return
            mHeroMovement.ResetParameter();
            return;
        }

        if (mEffectTimer <= 0)
        {
            mIsActive = false;
            return;
        }
        if (mCurrentEffect == PotionEffect.SPEED_EFFECT)
        {
            // Add speed
            mHeroMovement.SetSpeed(mHeroMovement.GetSpeed() * 1.25f);
        }
        else if (mCurrentEffect == PotionEffect.ARMOR_EFFECT)
        {
            // Visualize armor
        }
        else if (mCurrentEffect == PotionEffect.GOD_EFFECT)
        {
            // Keep updating health = max health
            mHealth.setGodMode();
        }
    }
}
