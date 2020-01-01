using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;
using Sirenix.OdinInspector;

public class Probability : MonoBehaviour
{
    [SerializeField]
    [Probability("OriginObj")]
    float[] probability;
    [SerializeField]
    string[] OriginObj;

    
   
    [Button]
    // Start is called before the first frame update
    void Start()
    {
        bool b;
        string go;
       
        (b, go) = ProbabilityObject(OriginObj, probability);
  
         print(go);
     
    
    }

    // Update is called once per frame
    void Update()
    {

    }
    /// <summary>
    /// 依照機率取得Obj
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="obj">Obj</param>
    /// <param name="prob">機率</param>
    /// <returns></returns>
    (bool, T) ProbabilityObject<T>(T[] obj, float[] prob)
    {
        if (obj.Length == 1) {
            return ((obj[0] != null), obj[0]);
        }
     
        if (obj.Length == prob.Length+1)
        {

            List<int> tempRanges = (from value in prob select (int)(value * 100)).ToList();
            tempRanges.Add(100);
            int randomValue = UnityEngine.Random.Range(0, 100);
         
            if (prob.Length == 0)
            {
                return ((obj[0] != null), default);
            }
          
            for (int i = 0; i < tempRanges.Count; i++)
            {
                if (randomValue <= tempRanges[i]) {

                    return ((obj[i] != null), obj[i]);
                }
              
            }
        }
     
        return (false, default);
    }
}
