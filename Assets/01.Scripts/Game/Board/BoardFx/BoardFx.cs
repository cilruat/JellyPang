using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using System;

public class BoardFx : MonoBehaviour
{
    public enum TYPE
    {
        GEM_EXPLOSION = 0,
        FEVER_EXPLOSION,
        FEVER_FIRE_TOP,
        FEVER_FIRE_BOT,

        DIAMOND_PROJECTILE,
        DIAMOND_EXPLOSION,

        //Gem Object Effect
        COLORBOMB_EXPLOSION = 500,
        ENERGYBOMB_EXPLOSION,
    };

    public TYPE Type;
    public BoardFxPool Pool { get; set; }

    private ParticleSystem _ps;
    protected IDisposable _observer;

    new Transform transform;

    public virtual void Awake()
    {
        transform = GetComponent<Transform>();
        _ps = GetComponentInChildren<ParticleSystem>();
    }

    public virtual void Play()
    {
        _observer = Observable.EveryGameObjectUpdate()
            .Select(_ => _ps.IsAlive(true))
            .DistinctUntilChanged()
            .Where(isAlive => isAlive == false)
            .Subscribe(_ => 
            {                
                Pool.Return(this); 
                _observer.Dispose();

                transform.rotation = Quaternion.Euler(Vector3.zero);
            })
            .AddTo(gameObject);
    }

    public virtual void Play(float lifeTime)
    {
        _observer = Observable.Timer(System.TimeSpan.FromSeconds(lifeTime))
            .Subscribe(_ => 
            {                
                Pool.Return(this); 
                _observer.Dispose();

                transform.rotation = Quaternion.Euler(Vector3.zero);
            })
            .AddTo(gameObject);
    }
}
