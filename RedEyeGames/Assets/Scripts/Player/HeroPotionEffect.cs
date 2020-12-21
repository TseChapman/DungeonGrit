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

    private float potionDuration = 30f;

    public void SetPotionEffect(PotionEffect effect)
    {
        SetActive(true);
        SetEffectTimer(potionDuration);
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
        mHeroMovement = GetComponent<HeroMovement>();
    }

    // Update is called once per frame
    private void Update()
    {
        mEffectTimer -= Time.smoothDeltaTime;
        CheckEffect();
    }

    private void CheckEffect()
    {
        if (mIsActive is false)
        {
            // do nothing
            return;
        }

        if (mEffectTimer <= 0)
        {
            // remove effects
            mIsActive = false;
            mHealth.HalfDamage(false);
            mHeroMovement.SpeedBoost(false);
            return;
        }
        if (mCurrentEffect == PotionEffect.SPEED_EFFECT)
        {
            // Add speed
            mHeroMovement.SpeedBoost(true);
            // Remove damage potion
            mHealth.HalfDamage(false);
        }
        else if (mCurrentEffect == PotionEffect.ARMOR_EFFECT)
        {
            // Half damage
            mHealth.HalfDamage(true);
            // Remove speed potion
            mHeroMovement.SpeedBoost(false);
        }
        else if (mCurrentEffect == PotionEffect.GOD_EFFECT)
        {
            // does not take damage
            // effect set in InventoryManager.cs
        }
    }
}
