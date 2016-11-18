using UnityEngine;
using System.Collections;

public class Knockback : Attack
{
    public float knockSpeed = 20.0f;
    public float knockValue = 1.0f;

    private Rigidbody rigidbodyTune;
    private BaseControl enemy;

    protected override void Start() {
        base.Start();
        rigidbodyTune = GetComponent<Rigidbody>();
        
    }

    public void Update() {
        rigidbodyTune.velocity = transform.forward * knockSpeed;
    }

    protected override void OnTriggerEnter(Collider other) {
        base.OnTriggerEnter(other);

        if (other.gameObject.GetComponent<BaseControl>()
            && other.gameObject.GetComponent<BaseControl>().player != agressor) {

            //GetComponent<MeshRenderer>().enabled = false;
            //GetComponent<TrailRenderer>().enabled = false;
            //GetComponent<BoxCollider>().enabled = false;

            try {
                transform.GetChild(0).GetComponent<ParticleSystem>().Play();
            } catch (UnityException e) {
                Destroy(this.gameObject);
            }
            
            

            enemy = other.gameObject.GetComponent<BaseControl>();
            Vector3 forward = transform.forward;
            enemy.Knockback(new Vector2(forward.x, forward.z) * knockValue);

            Destroy(transform.GetChild(1).gameObject);
            transform.GetChild(0).GetComponent<ParticleSystem>().transform.parent = null;

            Destroy(this.gameObject);
        }
    }
}