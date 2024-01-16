using UnityEngine;
namespace RoR2
{
    public class HitBoxGroup : MonoBehaviour
    {
        // Token: 0x04002A04 RID: 10756
        [Tooltip("The name of this hitbox group.")]
        public string groupName;

        // Token: 0x04002A05 RID: 10757
        [Tooltip("The hitbox objects in this group.")]
        public HitBox[] hitBoxes;
    }
}