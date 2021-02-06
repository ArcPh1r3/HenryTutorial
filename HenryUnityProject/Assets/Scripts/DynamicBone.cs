using System;
using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("Dynamic Bone/Dynamic Bone")]
public class DynamicBone : MonoBehaviour
{

    private void Start()
    {
        this.SetupParticles();
    }

    private void FixedUpdate()
    {
        if (this.m_UpdateMode == DynamicBone.UpdateMode.AnimatePhysics)
        {
            this.PreUpdate();
        }
    }

    private void Update()
    {
        if (this.m_UpdateMode != DynamicBone.UpdateMode.AnimatePhysics)
        {
            this.PreUpdate();
        }
    }

    private void LateUpdate()
    {
        if (this.m_DistantDisable)
        {
            this.CheckDistance();
        }
        if (this.m_Weight > 0f && (!this.m_DistantDisable || !this.m_DistantDisabled))
        {
            float deltaTime = Time.deltaTime;
            this.UpdateDynamicBones(deltaTime);
        }
    }

    private void PreUpdate()
    {
        if (this.m_Weight > 0f && (!this.m_DistantDisable || !this.m_DistantDisabled))
        {
            this.InitTransforms();
        }
    }

    private void CheckDistance()
    {
        Transform transform = this.m_ReferenceObject;
        if (transform == null && Camera.main != null)
        {
            transform = Camera.main.transform;
        }
        if (transform != null)
        {
            bool flag = (transform.position - base.transform.position).sqrMagnitude > this.m_DistanceToObject * this.m_DistanceToObject;
            if (flag != this.m_DistantDisabled)
            {
                if (!flag)
                {
                    this.ResetParticlesPosition();
                }
                this.m_DistantDisabled = flag;
            }
        }
    }

    private void OnEnable()
    {
        this.ResetParticlesPosition();
    }

    private void OnDisable()
    {
        this.InitTransforms();
    }

    private void OnValidate()
    {
        this.m_UpdateRate = Mathf.Max(this.m_UpdateRate, 0f);
        this.m_Damping = Mathf.Clamp01(this.m_Damping);
        this.m_Elasticity = Mathf.Clamp01(this.m_Elasticity);
        this.m_Stiffness = Mathf.Clamp01(this.m_Stiffness);
        this.m_Inert = Mathf.Clamp01(this.m_Inert);
        this.m_Radius = Mathf.Max(this.m_Radius, 0f);
        if (Application.isEditor && Application.isPlaying)
        {
            this.InitTransforms();
            this.SetupParticles();
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (!base.enabled || this.m_Root == null)
        {
            return;
        }
        if (Application.isEditor && !Application.isPlaying && base.transform.hasChanged)
        {
            this.InitTransforms();
            this.SetupParticles();
        }
        Gizmos.color = Color.white;
        for (int i = 0; i < this.m_Particles.Count; i++)
        {
            DynamicBone.Particle particle = this.m_Particles[i];
            if (particle.m_ParentIndex >= 0)
            {
                DynamicBone.Particle particle2 = this.m_Particles[particle.m_ParentIndex];
                Gizmos.DrawLine(particle.m_Position, particle2.m_Position);
            }
            if (particle.m_Radius > 0f)
            {
                Gizmos.DrawWireSphere(particle.m_Position, particle.m_Radius * this.m_ObjectScale);
            }
        }
    }

    public void SetWeight(float w)
    {
        if (this.m_Weight != w)
        {
            if (w == 0f)
            {
                this.InitTransforms();
            }
            else if (this.m_Weight == 0f)
            {
                this.ResetParticlesPosition();
            }
            this.m_Weight = w;
        }
    }

    public float GetWeight()
    {
        return this.m_Weight;
    }

    private void UpdateDynamicBones(float t)
    {
        if (this.m_Root == null)
        {
            return;
        }
        this.m_ObjectScale = Mathf.Abs(base.transform.lossyScale.x);
        this.m_ObjectMove = base.transform.position - this.m_ObjectPrevPosition;
        this.m_ObjectPrevPosition = base.transform.position;
        int num = 1;
        if (this.m_UpdateRate > 0f)
        {
            float num2 = 1f / this.m_UpdateRate;
            this.m_Time += t;
            num = 0;
            while (this.m_Time >= num2)
            {
                this.m_Time -= num2;
                if (++num >= 3)
                {
                    this.m_Time = 0f;
                    break;
                }
            }
        }
        if (num > 0)
        {
            for (int i = 0; i < num; i++)
            {
                this.UpdateParticles1();
                this.UpdateParticles2();
                this.m_ObjectMove = Vector3.zero;
            }
        }
        else
        {
            this.SkipUpdateParticles();
        }
        this.ApplyParticlesToTransforms();
    }

    private void SetupParticles()
    {
        this.m_Particles.Clear();
        if (this.m_Root == null)
        {
            return;
        }
        this.m_LocalGravity = this.m_Root.InverseTransformDirection(this.m_Gravity);
        this.m_ObjectScale = Mathf.Abs(base.transform.lossyScale.x);
        this.m_ObjectPrevPosition = base.transform.position;
        this.m_ObjectMove = Vector3.zero;
        this.m_BoneTotalLength = 0f;
        this.AppendParticles(this.m_Root, -1, 0f);
        for (int i = 0; i < this.m_Particles.Count; i++)
        {
            DynamicBone.Particle particle = this.m_Particles[i];
            particle.m_Damping = this.m_Damping;
            particle.m_Elasticity = this.m_Elasticity;
            particle.m_Stiffness = this.m_Stiffness;
            particle.m_Inert = this.m_Inert;
            particle.m_Radius = this.m_Radius;
            if (this.m_BoneTotalLength > 0f)
            {
                float time = particle.m_BoneLength / this.m_BoneTotalLength;
                if (this.m_DampingDistrib != null && this.m_DampingDistrib.keys.Length != 0)
                {
                    particle.m_Damping *= this.m_DampingDistrib.Evaluate(time);
                }
                if (this.m_ElasticityDistrib != null && this.m_ElasticityDistrib.keys.Length != 0)
                {
                    particle.m_Elasticity *= this.m_ElasticityDistrib.Evaluate(time);
                }
                if (this.m_StiffnessDistrib != null && this.m_StiffnessDistrib.keys.Length != 0)
                {
                    particle.m_Stiffness *= this.m_StiffnessDistrib.Evaluate(time);
                }
                if (this.m_InertDistrib != null && this.m_InertDistrib.keys.Length != 0)
                {
                    particle.m_Inert *= this.m_InertDistrib.Evaluate(time);
                }
                if (this.m_RadiusDistrib != null && this.m_RadiusDistrib.keys.Length != 0)
                {
                    particle.m_Radius *= this.m_RadiusDistrib.Evaluate(time);
                }
            }
            particle.m_Damping = Mathf.Clamp01(particle.m_Damping);
            particle.m_Elasticity = Mathf.Clamp01(particle.m_Elasticity);
            particle.m_Stiffness = Mathf.Clamp01(particle.m_Stiffness);
            particle.m_Inert = Mathf.Clamp01(particle.m_Inert);
            particle.m_Radius = Mathf.Max(particle.m_Radius, 0f);
        }
    }

    private void AppendParticles(Transform b, int parentIndex, float boneLength)
    {
        DynamicBone.Particle particle = new DynamicBone.Particle();
        particle.m_Transform = b;
        particle.m_ParentIndex = parentIndex;
        if (b != null)
        {
            particle.m_Position = (particle.m_PrevPosition = b.position);
            particle.m_InitLocalPosition = b.localPosition;
            particle.m_InitLocalRotation = b.localRotation;
        }
        else
        {
            Transform transform = this.m_Particles[parentIndex].m_Transform;
            if (this.m_EndLength > 0f)
            {
                Transform parent = transform.parent;
                if (parent != null)
                {
                    particle.m_EndOffset = transform.InverseTransformPoint(transform.position * 2f - parent.position) * this.m_EndLength;
                }
                else
                {
                    particle.m_EndOffset = new Vector3(this.m_EndLength, 0f, 0f);
                }
            }
            else
            {
                particle.m_EndOffset = transform.InverseTransformPoint(base.transform.TransformDirection(this.m_EndOffset) + transform.position);
            }
            particle.m_Position = (particle.m_PrevPosition = transform.TransformPoint(particle.m_EndOffset));
        }
        if (parentIndex >= 0)
        {
            boneLength += (this.m_Particles[parentIndex].m_Transform.position - particle.m_Position).magnitude;
            particle.m_BoneLength = boneLength;
            this.m_BoneTotalLength = Mathf.Max(this.m_BoneTotalLength, boneLength);
        }
        int count = this.m_Particles.Count;
        this.m_Particles.Add(particle);
        if (b != null)
        {
            for (int i = 0; i < b.childCount; i++)
            {
                bool flag = false;
                if (this.m_Exclusions != null)
                {
                    for (int j = 0; j < this.m_Exclusions.Count; j++)
                    {
                        if (this.m_Exclusions[j] == b.GetChild(i))
                        {
                            flag = true;
                            break;
                        }
                    }
                }
                if (!flag)
                {
                    this.AppendParticles(b.GetChild(i), count, boneLength);
                }
            }
            if (b.childCount == 0 && (this.m_EndLength > 0f || this.m_EndOffset != Vector3.zero))
            {
                this.AppendParticles(null, count, boneLength);
            }
        }
    }

    private void InitTransforms()
    {
        for (int i = 0; i < this.m_Particles.Count; i++)
        {
            DynamicBone.Particle particle = this.m_Particles[i];
            if (particle.m_Transform != null)
            {
                particle.m_Transform.localPosition = particle.m_InitLocalPosition;
                particle.m_Transform.localRotation = particle.m_InitLocalRotation;
            }
        }
    }

    private void ResetParticlesPosition()
    {
        for (int i = 0; i < this.m_Particles.Count; i++)
        {
            DynamicBone.Particle particle = this.m_Particles[i];
            if (particle.m_Transform != null)
            {
                particle.m_Position = (particle.m_PrevPosition = particle.m_Transform.position);
            }
            else
            {
                Transform transform = this.m_Particles[particle.m_ParentIndex].m_Transform;
                particle.m_Position = (particle.m_PrevPosition = transform.TransformPoint(particle.m_EndOffset));
            }
        }
        this.m_ObjectPrevPosition = base.transform.position;
    }

    private void UpdateParticles1()
    {
        Vector3 vector = this.m_Gravity;
        Vector3 normalized = this.m_Gravity.normalized;
        Vector3 lhs = this.m_Root.TransformDirection(this.m_LocalGravity);
        Vector3 b = normalized * Mathf.Max(Vector3.Dot(lhs, normalized), 0f);
        vector -= b;
        vector = (vector + this.m_Force) * this.m_ObjectScale;
        for (int i = 0; i < this.m_Particles.Count; i++)
        {
            DynamicBone.Particle particle = this.m_Particles[i];
            if (particle.m_ParentIndex >= 0)
            {
                Vector3 a = particle.m_Position - particle.m_PrevPosition;
                Vector3 b2 = this.m_ObjectMove * particle.m_Inert;
                particle.m_PrevPosition = particle.m_Position + b2;
                particle.m_Position += a * (1f - particle.m_Damping) + vector + b2;
            }
            else
            {
                particle.m_PrevPosition = particle.m_Position;
                particle.m_Position = particle.m_Transform.position;
            }
        }
    }

    private void UpdateParticles2()
    {
        Plane plane = default(Plane);
        for (int i = 1; i < this.m_Particles.Count; i++)
        {
            DynamicBone.Particle particle = this.m_Particles[i];
            DynamicBone.Particle particle2 = this.m_Particles[particle.m_ParentIndex];
            float magnitude;
            if (particle.m_Transform != null)
            {
                magnitude = (particle2.m_Transform.position - particle.m_Transform.position).magnitude;
            }
            else
            {
                magnitude = particle2.m_Transform.localToWorldMatrix.MultiplyVector(particle.m_EndOffset).magnitude;
            }
            float num = Mathf.Lerp(1f, particle.m_Stiffness, this.m_Weight);
            if (num > 0f || particle.m_Elasticity > 0f)
            {
                Matrix4x4 localToWorldMatrix = particle2.m_Transform.localToWorldMatrix;
                localToWorldMatrix.SetColumn(3, particle2.m_Position);
                Vector3 a;
                if (particle.m_Transform != null)
                {
                    a = localToWorldMatrix.MultiplyPoint3x4(particle.m_Transform.localPosition);
                }
                else
                {
                    a = localToWorldMatrix.MultiplyPoint3x4(particle.m_EndOffset);
                }
                Vector3 a2 = a - particle.m_Position;
                particle.m_Position += a2 * particle.m_Elasticity;
                if (num > 0f)
                {
                    a2 = a - particle.m_Position;
                    float magnitude2 = a2.magnitude;
                    float num2 = magnitude * (1f - num) * 2f;
                    if (magnitude2 > num2)
                    {
                        particle.m_Position += a2 * ((magnitude2 - num2) / magnitude2);
                    }
                }
            }
            if (this.m_Colliders != null)
            {
                float particleRadius = particle.m_Radius * this.m_ObjectScale;
                for (int j = 0; j < this.m_Colliders.Count; j++)
                {
                    DynamicBoneCollider dynamicBoneCollider = this.m_Colliders[j];
                    if (dynamicBoneCollider != null && dynamicBoneCollider.enabled)
                    {
                        dynamicBoneCollider.Collide(ref particle.m_Position, particleRadius);
                    }
                }
            }
            if (this.m_FreezeAxis != DynamicBone.FreezeAxis.None)
            {
                switch (this.m_FreezeAxis)
                {
                    case DynamicBone.FreezeAxis.X:
                        plane.SetNormalAndPosition(particle2.m_Transform.right, particle2.m_Position);
                        break;
                    case DynamicBone.FreezeAxis.Y:
                        plane.SetNormalAndPosition(particle2.m_Transform.up, particle2.m_Position);
                        break;
                    case DynamicBone.FreezeAxis.Z:
                        plane.SetNormalAndPosition(particle2.m_Transform.forward, particle2.m_Position);
                        break;
                }
                particle.m_Position -= plane.normal * plane.GetDistanceToPoint(particle.m_Position);
            }
            Vector3 a3 = particle2.m_Position - particle.m_Position;
            float magnitude3 = a3.magnitude;
            if (magnitude3 > 0f)
            {
                particle.m_Position += a3 * ((magnitude3 - magnitude) / magnitude3);
            }
        }
    }

    private void SkipUpdateParticles()
    {
        for (int i = 0; i < this.m_Particles.Count; i++)
        {
            DynamicBone.Particle particle = this.m_Particles[i];
            if (particle.m_ParentIndex >= 0)
            {
                particle.m_PrevPosition += this.m_ObjectMove;
                particle.m_Position += this.m_ObjectMove;
                DynamicBone.Particle particle2 = this.m_Particles[particle.m_ParentIndex];
                float magnitude;
                if (particle.m_Transform != null)
                {
                    magnitude = (particle2.m_Transform.position - particle.m_Transform.position).magnitude;
                }
                else
                {
                    magnitude = particle2.m_Transform.localToWorldMatrix.MultiplyVector(particle.m_EndOffset).magnitude;
                }
                float num = Mathf.Lerp(1f, particle.m_Stiffness, this.m_Weight);
                if (num > 0f)
                {
                    Matrix4x4 localToWorldMatrix = particle2.m_Transform.localToWorldMatrix;
                    localToWorldMatrix.SetColumn(3, particle2.m_Position);
                    Vector3 a;
                    if (particle.m_Transform != null)
                    {
                        a = localToWorldMatrix.MultiplyPoint3x4(particle.m_Transform.localPosition);
                    }
                    else
                    {
                        a = localToWorldMatrix.MultiplyPoint3x4(particle.m_EndOffset);
                    }
                    Vector3 a2 = a - particle.m_Position;
                    float magnitude2 = a2.magnitude;
                    float num2 = magnitude * (1f - num) * 2f;
                    if (magnitude2 > num2)
                    {
                        particle.m_Position += a2 * ((magnitude2 - num2) / magnitude2);
                    }
                }
                Vector3 a3 = particle2.m_Position - particle.m_Position;
                float magnitude3 = a3.magnitude;
                if (magnitude3 > 0f)
                {
                    particle.m_Position += a3 * ((magnitude3 - magnitude) / magnitude3);
                }
            }
            else
            {
                particle.m_PrevPosition = particle.m_Position;
                particle.m_Position = particle.m_Transform.position;
            }
        }
    }

    private static Vector3 MirrorVector(Vector3 v, Vector3 axis)
    {
        return v - axis * (Vector3.Dot(v, axis) * 2f);
    }

    private void ApplyParticlesToTransforms()
    {
        for (int i = 1; i < this.m_Particles.Count; i++)
        {
            DynamicBone.Particle particle = this.m_Particles[i];
            DynamicBone.Particle particle2 = this.m_Particles[particle.m_ParentIndex];
            if (particle2.m_Transform.childCount <= 1)
            {
                Vector3 direction;
                if (particle.m_Transform != null)
                {
                    direction = particle.m_Transform.localPosition;
                }
                else
                {
                    direction = particle.m_EndOffset;
                }
                Vector3 toDirection = particle.m_Position - particle2.m_Position;
                Quaternion lhs = Quaternion.FromToRotation(particle2.m_Transform.TransformDirection(direction), toDirection);
                particle2.m_Transform.rotation = lhs * particle2.m_Transform.rotation;
            }
            if (particle.m_Transform != null)
            {
                particle.m_Transform.position = particle.m_Position;
            }
        }
    }

    public Transform m_Root;
    public float m_UpdateRate = 60f;
    public DynamicBone.UpdateMode m_UpdateMode;
    [Range(0f, 1f)]
    public float m_Damping = 0.1f;
    public AnimationCurve m_DampingDistrib;
    [Range(0f, 1f)]
    public float m_Elasticity = 0.1f;
    public AnimationCurve m_ElasticityDistrib;
    [Range(0f, 1f)]
    public float m_Stiffness = 0.1f;
    public AnimationCurve m_StiffnessDistrib;
    [Range(0f, 1f)]
    public float m_Inert;
    public AnimationCurve m_InertDistrib;
    public float m_Radius;
    public AnimationCurve m_RadiusDistrib;
    public float m_EndLength;
    public Vector3 m_EndOffset = Vector3.zero;
    public Vector3 m_Gravity = Vector3.zero;
    public Vector3 m_Force = Vector3.zero;
    public List<DynamicBoneCollider> m_Colliders;
    public List<Transform> m_Exclusions;
    public DynamicBone.FreezeAxis m_FreezeAxis;
    public bool m_DistantDisable;
    public Transform m_ReferenceObject;
    public float m_DistanceToObject = 20f;
    private Vector3 m_LocalGravity = Vector3.zero;
    private Vector3 m_ObjectMove = Vector3.zero;
    private Vector3 m_ObjectPrevPosition = Vector3.zero;
    private float m_BoneTotalLength;
    private float m_ObjectScale = 1f;
    private float m_Time;
    private float m_Weight = 1f;
    private bool m_DistantDisabled;
    private List<DynamicBone.Particle> m_Particles = new List<DynamicBone.Particle>();

    public enum UpdateMode
    {
        Normal,
        AnimatePhysics,
        UnscaledTime
    }

    public enum FreezeAxis
    {
        None,
        X,
        Y,
        Z
    }

    private class Particle
    {
        public Transform m_Transform;
        public int m_ParentIndex = -1;
        public float m_Damping;
        public float m_Elasticity;
        public float m_Stiffness;
        public float m_Inert;
        public float m_Radius;
        public float m_BoneLength;
        public Vector3 m_Position = Vector3.zero;
        public Vector3 m_PrevPosition = Vector3.zero;
        public Vector3 m_EndOffset = Vector3.zero;
        public Vector3 m_InitLocalPosition = Vector3.zero;
        public Quaternion m_InitLocalRotation = Quaternion.identity;
    }
}
