using UnityEngine;

class DeathTune : MonoBehaviour, Spawnable
{
    private PlayerID owner;
    private bool crit;

    /// <summary> The tune that spawned this object. </summary>
    private Tune _tune;
    /// <summary> The tune that spawned this object. </summary>
    public Tune tune {
        get { return _tune; }
        set { _tune = value; }
    }

    public void Crit(bool crit)
    {
        this.crit = crit;
    }

    public void Owner(PlayerID owner)
    {
        this.owner = owner;
    }

    // Use this for initialization
    void Start()
    {
        PlayerLife[] lifes = FindObjectsOfType<PlayerLife>() as PlayerLife[];
        foreach (PlayerLife l in lifes)
        {
            if (crit)
            {
                if (l.gameObject.GetComponent<BaseControl>().player != owner)
                    l.DealDamage(1f, tune, owner);
            }
            else
            {
                if (l.gameObject.GetComponent<BaseControl>().player == owner)
                    l.DealDamage(.75f, tune, owner);
            }
        }
        Destroy(transform.root.gameObject);
		transform.GetChild(0).parent = null;
    }
}
