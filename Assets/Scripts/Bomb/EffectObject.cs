using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectObject : MonoBehaviour
{
    private EffectManager manager;
    private ParticleSystem ps;
    private ParticleSystem afterPs;

    private int id;
    private float time = float.MaxValue;

    private void Awake()
    {
        ps = GetComponent<ParticleSystem>();
        afterPs = GetComponentInChildren<ParticleSystem>();
    }

    private void OnDisable()
    {
        ps.Stop();
        if(afterPs != null) afterPs.Stop();
    }

    private void Update()
    {
        time -= Time.deltaTime;

        if (time < ps.time && gameObject.activeSelf)
        {
            manager.RecycleParticle(id, this.gameObject);
            gameObject.SetActive(false);
        }
    }

    public void SetData(int id, EffectManager manager)
    {
        this.manager = manager;
        this.id = id;
    }

    public void Play(Vector3 position)
    {
        // 활성화 전 초기화
        time = ps.main.duration;
        transform.position = position;

        // 활성화
        gameObject.SetActive(true);
        ps.Play();
        if (afterPs != null) afterPs.Play();
    }
}