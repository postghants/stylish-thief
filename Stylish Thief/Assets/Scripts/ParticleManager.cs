using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ParticleManagement;

    public class ParticleManager : MonoBehaviour
    {
        public List<ParticleSystemGroup> groups = new();

        private void Start()
        {
            foreach (ParticleSystemGroup group in groups) { group.Init(); }
        }

        public void StartGroup(int index)
        {
            if (index >= groups.Count) { return; }
            foreach (ParticleSystemData data in groups[index].systems)
            {
                if (data.system == null) { continue; }
                if (data.delay > 0)
                {
                    CoroutineBox box = new();
                    box.c = StartCoroutine(DelayStart(data, box));
                    groups[index].activeDelayedRoutines.Add(box);
                }
                else
                {
                    data.system.Play();
                }
            }
        }

    public void StartGroup(string name)
    {
        foreach(ParticleSystemGroup group in groups)
        {
            if(group.name == name)
            {
                StartGroup(groups.IndexOf(group));
            }
        }
    }

        public void StopGroup(int index)
        {
            if (index >= groups.Count) { return; }
            foreach (ParticleSystemData data in groups[index].systems)
            {
                if(data.system == null) { continue; }
                data.system.Stop();
            }
            foreach (CoroutineBox box in groups[index].activeDelayedRoutines)
            {
                StopCoroutine(box.c);
            }
            groups[index].activeDelayedRoutines.Clear();
        }

        private IEnumerator DelayStart(ParticleSystemData data, CoroutineBox box)
        {
            yield return new WaitForSeconds(data.delay);
            data.system.Play();
            data.group.activeDelayedRoutines.Remove(box);
        }

    }

namespace ParticleManagement
{

    [Serializable]
    public class ParticleSystemGroup
    {
        public string name;
        public List<ParticleSystemData> systems = new();
        [HideInInspector] public List<CoroutineBox> activeDelayedRoutines = new();

        public void Init()
        {
            foreach (ParticleSystemData data in systems)
            {
                data.group = this;
            }
        }
    }

    [Serializable]
    public class ParticleSystemData
    {
        public string name;
        public ParticleSystem system;
        public float delay;
        [NonSerialized] public ParticleSystemGroup group;
    }


}

public class CoroutineBox
{
    public Coroutine c;
}
