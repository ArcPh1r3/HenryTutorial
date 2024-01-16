using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
namespace RoR2
{
    public class HurtBoxGroup : MonoBehaviour
    {
        public HurtBox[] hurtBoxes;

        [Tooltip("The most important hurtbox in this group, usually a good center-of-mass target like the chest.")]
        public HurtBox mainHurtBox;

        public void OnValidate()
        {
            int num = 0;
            if (this.hurtBoxes == null)
            {
                this.hurtBoxes = Array.Empty<HurtBox>();
            }
            short num2 = 0;
            while ((int)num2 < this.hurtBoxes.Length)
            {
                if (!this.hurtBoxes[(int)num2])
                {
                    Debug.LogWarning(string.Format("Object {0} HurtBoxGroup hurtbox #{1} is missing.", new object[]
                    {
                        base.gameObject,
                        num2
                    }, base.gameObject));
                }
                else
                {
                    this.hurtBoxes[(int)num2].hurtBoxGroup = this;
                    this.hurtBoxes[(int)num2].indexInGroup = num2;
                    if (this.hurtBoxes[(int)num2].isBullseye)
                    {
                        num++;
                    }
                }
                num2 += 1;
            }
            if (!this.mainHurtBox)
            {
                IEnumerable<HurtBox> source = from v in this.hurtBoxes
                                              where v
                                              where v.isBullseye
                                              select v;
                IEnumerable<HurtBox> source2 = from v in source
                                               where v.transform.parent.name.ToLowerInvariant() == "chest"
                                               select v;
                this.mainHurtBox = (source2.FirstOrDefault<HurtBox>() ?? source.FirstOrDefault<HurtBox>());
            }
        }
    }
}