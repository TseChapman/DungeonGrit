using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DamagePopUp : MonoBehaviour
{
    private static int sortingOrder;

    private const float DISAPPEAR_TIMER_MAX = 1f;

    private TextMeshPro textMesh;
    private float disappearTimer;
    private Color textColor;

    public static DamagePopUp CreatePlayer(Vector3 position, int damage)
    {
        Transform damagePopUpTransform = Instantiate(GameAssets.i.pfDamagePopUpPlayer, position, Quaternion.identity);
        DamagePopUp damagePopUp = damagePopUpTransform.GetComponent<DamagePopUp>();
        damagePopUp.Setup(damage);

        return damagePopUp;
    }

    public static DamagePopUp CreateEnemy(Vector3 position, int damage)
    {
        Transform damagePopUpTransform = Instantiate(GameAssets.i.pfDamagePopUpEnemy, position, Quaternion.identity);
        DamagePopUp damagePopUp = damagePopUpTransform.GetComponent<DamagePopUp>();
        damagePopUp.Setup(damage);

        return damagePopUp;
    }

    private void Awake()
    {
        textMesh = GetComponent<TextMeshPro>();
    }

    public void Setup(int damage)
    {
        textMesh.SetText(damage.ToString());
        textColor = textMesh.color;
        disappearTimer = DISAPPEAR_TIMER_MAX;

        sortingOrder++;
        textMesh.sortingOrder = sortingOrder;
    }

    private void Update()
    {
        float moveYSpeed = 0.75f;
        transform.position += new Vector3(0, moveYSpeed * Time.smoothDeltaTime);

        if (disappearTimer > DISAPPEAR_TIMER_MAX * 0.5f)
        {
            // first half of popup lifetime
            float increaseScaleAmount = 1f;
            transform.localScale += Vector3.one * increaseScaleAmount * Time.smoothDeltaTime;
        }
        else
        {
            // second half of popup lifetime
            float decreaseScaleAmount = 1f;
            transform.localScale -= Vector3.one * decreaseScaleAmount * Time.smoothDeltaTime;
        }

        disappearTimer -= Time.smoothDeltaTime;
        if (disappearTimer < 0)
        {
            // start disappearing
            float disappearSpeed = 3f;
            textColor.a -= disappearSpeed * Time.smoothDeltaTime;
            textMesh.color = textColor;
            if (textColor.a < 0)
                Destroy(gameObject);
        }
    }
}
