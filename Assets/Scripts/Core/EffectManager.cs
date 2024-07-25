using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class EffectManager : ScriptableObject
{
    // 폭발 오브젝트 넣기

    [SerializeField] private GameObject[] effectObjects;

    private Dictionary<int, GameObject> freeParticle = new Dictionary<int, GameObject>();

    public GameObject PlayParticle(int id, Vector3 position)
    {
        GameObject result;

        if (freeParticle.TryGetValue(id, out GameObject obj))
        {
            result = obj;
            freeParticle.Remove(id);
        }
        else
        {
            result = Instantiate(effectObjects[id]);
        }

        EffectObject effectObject = result.GetComponent<EffectObject>();
        effectObject.SetData(id, this);
        effectObject.Play(position);

        return result;
    }

    public void ClearParticles()
    {
        freeParticle.Clear();
    }

    public void RecycleParticle(int id, GameObject obj)
    {
        if(!freeParticle.ContainsKey(id))
        {
            freeParticle.Add(id, obj);
        }
    }
}