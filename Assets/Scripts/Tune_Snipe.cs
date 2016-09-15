using UnityEngine;
using System.Collections;

public class Tune_Snipe : Tune {

    /// <summary>
    /// What should happen when the tune completes?
    /// </summary>
    /// <param name="crit">Was the tune played perfectly?</param>
	public override void TuneComplete (bool crit)
	{
		base.TuneComplete (crit);

		GameObject temp = (GameObject)GameObject.Instantiate(spawnObject,ownerTransform.position + ownerTransform.forward*2f, ownerTransform.rotation);
		temp.GetComponent<Attack>().agressor = ownerTransform.GetComponent<BaseControl>().player;
		temp.GetComponent<TrailRenderer>().material.color = Color.red;
		temp.transform.GetChild(1).GetComponent<ParticleSystem>().startColor = Color.red;
		temp.transform.GetChild(0).GetComponent<ParticleSystem>().startColor = Color.red;
		if(crit) {
			temp.GetComponent<TrailRenderer>().material.color = Color.blue;
			temp.transform.GetChild(1).GetComponent<ParticleSystem>().startColor = Color.blue;
			temp.transform.GetChild(0).GetComponent<ParticleSystem>().startColor = Color.blue;
			temp.GetComponent<Attack>().damage *= 2f;
		}
		Destroy(temp.transform.GetChild(0).gameObject,2f);
		Destroy(temp.transform.GetChild(1).gameObject,2f);

		temp.transform.GetChild(0).parent = null;
		temp.transform.GetChild(0).parent = null;
	}

}
