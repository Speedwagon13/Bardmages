using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Mine : Attack {
	void Start () {
        transform.GetChild(0).GetComponent<ParticleSystem>().Stop();

        //TODO: Color should depend on the color of the mine's owner

        base.Start();

        if (this.crit)
        {
            GetComponent<Renderer>().material.color = Color.magenta;
            GetComponent<Renderer>().material.SetColor("_EmissionColor", Color.magenta);
            transform.GetChild(0).GetComponent<ParticleSystem>().startColor = Color.magenta;
        }
    }

    protected override void OnTriggerEnter(Collider other)
    {
        //Player won't detonate the mine
        if (other.gameObject.GetComponent<PlayerControl>() 
            && other.gameObject.GetComponent<PlayerControl>().player != agressor)
        {
            base.OnTriggerEnter(other);

            transform.GetChild(0).GetComponent<ParticleSystem>().Play();
            Destroy(this.gameObject, .1f);
        }
    }
}
