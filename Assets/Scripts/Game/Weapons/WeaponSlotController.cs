using UnityEngine;

public class WeaponSlotController
{
    protected CharacterController character;

    private readonly float circleRadius;

    private readonly WeaponController[] equippedWeapons = new WeaponController[2];

    private readonly float weaponOffsetAngle = 45f;

    public WeaponSlotController(CharacterController character, float distanceFromCharacter = 0.7f)
    {
        this.character = character;
        circleRadius = distanceFromCharacter;
    }

    public void ManageWeapons()
    {
        if (character is PlayerController player)
        {
            // if a player, move based on mouse position
            Vector2 mousePosition = player.MainCamera.ScreenToWorldPoint(Input.mousePosition);
            MoveWeaponsAtPosition(mousePosition);

            // weapon controls
            if (Input.GetMouseButtonUp(0))
            {
                AttackAtPosition(0);
            }
            else if (Input.GetMouseButtonUp(1))
            {
                AttackAtPosition(1, mousePosition);
            }
        }
    }

    public void AttackAtPosition(int weaponSlotIndex, Vector2? attackPosition = null)
    {
        if (attackPosition == null)
        {
            equippedWeapons[weaponSlotIndex].AttackAtPosition(
                equippedWeapons[weaponSlotIndex].transform.position
            );
        }
        else
        {
            equippedWeapons[weaponSlotIndex].AttackAtPosition((Vector2)attackPosition);
        }
    }

    public void MoveWeaponsAtPosition(Vector2 position)
    {
        for (int ndx = 0; ndx < equippedWeapons.Length; ndx++)
        {
            float offset = ndx * weaponOffsetAngle;
            var weapon = equippedWeapons[ndx];
            if (weapon == null)
            {
                continue;
            }
            // Calculate the direction from the character to the mouse position
            Vector2 direction = (position - (Vector2)character.transform.position).normalized;

            // Calculate the position on the circumference of the circle in the direction of the mouse
            Vector2 circlePosition = CalculatePositionOnCircle(direction, offset);

            // move and rotate each of the weapons equipped
            MoveAndRotateWeapon(circlePosition, ndx);

            if (weapon.ShouldRotateToMousePosition)
            {
                RotateTowardsDirection(weapon, position);
            }
        }
    }

    void MoveAndRotateWeapon(Vector2 circlePosition, int slot)
    {
        equippedWeapons[slot].transform.position = circlePosition;

        // Calculate the rotation angle in degrees
        float angle =
            Mathf.Atan2(
                circlePosition.y - character.transform.position.y,
                circlePosition.x - character.transform.position.x
            ) * Mathf.Rad2Deg
            + 90;

        equippedWeapons[slot].transform.rotation = Quaternion.Euler(0, 0, angle);
    }

    public void AssignWeaponSlot(WeaponController weapon, int slot)
    {
        float angleDegrees = -45f;

        float angleRadians = angleDegrees * Mathf.Deg2Rad;

        // calculate the position on the circumference based off angle: classic trigonometry
        Vector2 startPosition = new Vector2(
            Mathf.Cos(angleRadians) * circleRadius,
            Mathf.Sin(angleRadians) * circleRadius
        );

        weapon.transform.position = (Vector2)character.transform.position + startPosition;
        equippedWeapons[slot] = weapon;
    }

    Vector2 CalculatePositionOnCircle(Vector2 direction, float offsetAngle)
    {
        // Calculate the rotated direction based on the offset angle
        Quaternion rotation = Quaternion.Euler(0, 0, offsetAngle);
        Vector2 rotatedDirection = rotation * direction;

        // Calculate the position on the circumference of the circle
        Vector2 circlePosition =
            (Vector2)character.transform.position + rotatedDirection * circleRadius;

        return circlePosition;
    }

    void RotateTowardsDirection(WeaponController weapon, Vector2 lookAtDirection)
    {
        Vector2 direction = (lookAtDirection - (Vector2)weapon.transform.position).normalized;
        float angle = (Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg) - 90; // subtract 90 to account for tan angle

        weapon.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
    }
}
