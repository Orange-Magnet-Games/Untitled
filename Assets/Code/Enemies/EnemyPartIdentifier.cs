using UnityEngine;

public class EnemyPartIdentifier : MonoBehaviour
{
    public enum BodyPart { HEAD, BODY, LEFT_ARM, RIGHT_ARM, LEFT_LEG, RIGHT_LEG }

    public BodyPart bodyPart;

    private EnemyController parent;

    private void Start() {
        parent = GetComponentInParent<EnemyController>();
    }

    public void TakeDamage(int damage) {
        parent.TakeDamage(bodyPart switch {
            BodyPart.HEAD => damage * 2,
            BodyPart.BODY => damage,
            BodyPart.LEFT_ARM => damage / 4 * 3,
            BodyPart.RIGHT_ARM => damage / 4 * 3,
            BodyPart.LEFT_LEG => damage / 2,
            BodyPart.RIGHT_LEG => damage / 2,
            _ => 0
        });
    }
}
