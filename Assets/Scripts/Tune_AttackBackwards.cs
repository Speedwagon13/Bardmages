using UnityEngine;
using System.Collections;

public class Tune_AttackBackwards : Tune
{
    public override void TuneComplete(bool crit)
    {
        base.TuneComplete(crit);

        GameObject temp = (GameObject)GameObject.Instantiate(spawnObject, ownerTransform.position + -ownerTransform.forward * 2f, ownerTransform.rotation);
        temp.GetComponent<Attack>().agressor = ownerTransform.GetComponent<PlayerControl>().player;
        temp.GetComponent<Spawnable>().Crit(crit);
    }
}