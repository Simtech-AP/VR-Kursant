using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RobotAudioHandler : MonoBehaviour
{
    [Serializable]
    private class JointData
    {
        public Rigidbody Rigidbody = default;
        public AudioSource AudioSource = default;


        [SerializeField]
        private float fadeTime = default;

        [SerializeField]
        private float jointVolumeMultiplier = default;

        [SerializeField]
        private float linearVolumeMultiplier = default;

        [HideInInspector]
        private float fadeMoment = default;
        public float FadeMoment { get { return fadeMoment; } set { fadeMoment = Mathf.Clamp(value, 0, 1); } }

        private Vector3 prevFrameRotation = default;

        [SerializeField]
        private int prevVolumeCount = default;
        public int PrevVolumeCount { get { return prevVolumeCount; } }

        [SerializeField]
        private float minVolume = default;
        [SerializeField]
        private float maxVolume = default;

        private List<float> prevVolumeList = new List<float>();

        private float volumeJumpIgnoreThreshold = 100.0f;
        private float epsilon = 0.01f;

        public void onAwake()
        {
            AudioSource.velocityUpdateMode = AudioVelocityUpdateMode.Dynamic;

            if (PrevVolumeCount > 0)
            {
                for (int i = 0; i < PrevVolumeCount; i++)
                    prevVolumeList.Add(0.0f);
            }
        }

        public void Update()
        {
            if (TargetVolume == 0)
            {
                FadeMoment -= Time.deltaTime / fadeTime;
            }
            else
            {
                FadeMoment += Time.deltaTime / fadeTime;
            }


            prevFrameRotation = Rigidbody.transform.localEulerAngles;

        }


        public float TargetVolume
        {
            get
            {
                if (RobotData.Instance.IsInJointConfiguration)
                {
                    return getAngularSpeedForJoint() * jointVolumeMultiplier * maxVolume;
                }
                else
                {
                    return Mathf.Clamp((getAngularSpeedForLinear() * linearVolumeMultiplier), 0.0f, 1.0f) * maxVolume;
                }
            }
        }

        private float getAngularSpeedForLinear()
        {
            return Rigidbody.angularVelocity.magnitude;
        }

        private float getAngularSpeedForJoint()
        {
            return Vector3.Distance(Rigidbody.transform.localEulerAngles, prevFrameRotation) / Time.deltaTime;
        }

        public float getInertiaVolume()
        {
            if (prevVolumeCount < 0)
            {
                return TargetVolume;
            }

            if (TargetVolume < volumeJumpIgnoreThreshold * jointVolumeMultiplier)              //using this we are avoiding unexpected volume jump while passing from -180 to +180 degree (end/start of coordinate system)
            {
                prevVolumeList.RemoveAt(0);
                if (TargetVolume <= minVolume && TargetVolume > epsilon)                        //second condition is to ignore really tiny movement (for example while changing coord type)
                {
                    prevVolumeList.Add(minVolume);
                }
                else
                {
                    prevVolumeList.Add(TargetVolume);
                }
            }

            return prevVolumeList.Average();
        }
    }


    [SerializeField]
    private List<JointData> joints = default;

    private void Awake()
    {
        foreach (var joint in joints)
        {
            joint.onAwake();
        }
    }

    void Update()
    {
        ApplyJointVolumes();
    }

    private void ApplyJointVolumes()
    {
        foreach (var joint in joints)
        {
            if (joint.PrevVolumeCount <= 0)
                joint.AudioSource.volume = Mathf.Lerp(0, joint.TargetVolume, joint.FadeMoment);
            else
            {
                joint.AudioSource.volume = joint.getInertiaVolume();
            }

            joint.Update();
        }
    }


}
