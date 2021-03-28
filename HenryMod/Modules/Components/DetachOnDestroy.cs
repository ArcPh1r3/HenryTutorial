using UnityEngine;

namespace HenryMod.Modules.Components
{
    public class DetachOnDestroy : MonoBehaviour
    {
        private ParticleSystem main;
        private GameObject root;
        private bool destroying;

        private void Awake()
        {
            this.main = this.GetComponent<ParticleSystem>();
            this.root = this.transform.root.gameObject;
            this.transform.parent = null;
        }

        private void FixedUpdate()
        {
            if (this.root)
            {
                this.transform.position = this.root.transform.position;
                this.transform.rotation = this.root.transform.rotation;
            }
            else
            {
                if (!this.destroying)
                {
                    this.destroying = true;
                    Destroy(this.gameObject, 8f);
                    this.main.Stop();
                }
            }
        }
    }
}