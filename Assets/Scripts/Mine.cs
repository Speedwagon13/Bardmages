using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Mine : Attack {
	protected override void Start () {
        transform.GetChild(0).GetComponent<ParticleSystem>().Stop();

        base.Start();

        Renderer renderer = GetComponent<Renderer>();
        Color mineColor;
        if (this.crit)
        {
            mineColor = Color.magenta;
        }
        else
        {
            mineColor = LevelManager.instance.playerDict[agressor].GetRobeMaterial().color;
        }
        renderer.material.color = mineColor;
        renderer.material.SetColor("_EmissionColor", mineColor);
        transform.GetChild(0).GetComponent<ParticleSystem>().startColor = mineColor;
    }

    protected override void OnTriggerEnter(Collider other)
    {
        //Player won't detonate the mine
		if (other.gameObject.GetComponent<BaseControl>() 
			&& other.gameObject.GetComponent<BaseControl>().playerOwner != agressor)
        {
            base.OnTriggerEnter(other);

            transform.GetChild(0).GetComponent<ParticleSystem>().Play();
            Destroy(this.gameObject, .1f);
        }
    }
}
