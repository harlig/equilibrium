using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TextCore.Text;

public class MeleeWeapon : WeaponController
{
    // private bool isSwinging = false;
    private WeaponAnimator weaponAnimator;

    public override DamageType damageType
    {
        get
        {
            // TODO: change this to be specifc to actual weapon
            return DamageType.FIRE;
        }
    }

    public override float baseDamageAmount
    {
        get
        {
            // TODO: change to be specific to actual weapon
            return 20.0f;
        }
    }

    void Awake()
    {
        weaponAnimator = GetComponent<WeaponAnimator>();
    }

    public override void AttackAtPosition(Vector2 position)
    {
        // TODO: apply animations and make the weapon degree dynamic
        weaponAnimator.DoSwing(position);
        // StartCoroutine(DoStrikeAnimation(0.1f, position));
    }

    private IEnumerator DoStrikeAnimation(float waitTime, Vector2 position)
    {
        // strike with the weapon (crude animation)
        var originalRot = transform.rotation;
        var originalPos = transform.position;
        RotateByDegrees(90, position);

        yield return new WaitForSeconds(waitTime);
        // return to original position
        transform.rotation = originalRot;
        transform.position = originalPos;
    }

    private void RotateByDegrees(float degrees, Vector2 pivotPoint)
    {
        Vector3 pivotPoint3d = new(pivotPoint.x, pivotPoint.y, 0f);
        Vector3 objectPosition = transform.position;

        // calculate the rotated position of the object
        Vector3 rotatedPos =
            Quaternion.Euler(0, 0, degrees) * (objectPosition - pivotPoint3d) + pivotPoint3d;

        // apply the new position and rotate the weapon
        transform.position = rotatedPos;
        transform.rotation = Quaternion.Euler(0, 0, degrees);
    }

    private void ApplyCharacterDamage(CharacterController character, float damageModifier)
    {
        character.OnDamageTaken(damageType, baseDamageAmount + damageModifier);

        // TODO: base this off something and think about how this interacts with status effects on player
        bool shouldApplyDamageTypeToCharacter = true;
        if (shouldApplyDamageTypeToCharacter)
        {
            character.ApplyDamageOverTime(DamageType.FIRE, 5.0f);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        // the weapon has collided with some other game object, do damage if character
        CharacterController otherChar = other.GetComponent<CharacterController>();

        // means we have collided with a character, apply damage, and no friendly fire on self
        if (otherChar != null && otherChar.transform != transform.parent)
        {
            ApplyCharacterDamage(otherChar, GetDamageModifierOfParentCharacter());
        }
    }
}
