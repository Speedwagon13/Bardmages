using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Data
{
    public class RoundHandler : MonoBehaviour
    {
        /// <summary> Reference to the canvas for the count down timer. </summary>
        [SerializeField]
        [Tooltip("Reference to the canvas for the count down timer.")]
        private GameObject canvas;
        /// <summary> Reference to the textbox for the timer. </summary>
        [SerializeField]
        [Tooltip("Reference to the textbox for the timer.")]
        private Text timerText;
        /// <summary> Spawn Points for this stage. </summary>
        [SerializeField]
        [Tooltip("Spawn Points for this stage.  5 expected. 5th is the hill spawn")]
        private Transform[] spawnPoints;
        /// <summary> Prefab for the king of the hill hill. </summary>
        [SerializeField]
        [Tooltip("Prefab for the king of the hill hill.")]
        private GameObject hill;

        public delegate void OpenMidRoundUI();
        private OpenMidRoundUI uiCaller;

        /// <summary> internal reference for ensuring singleton. </summary>
        private static RoundHandler instance;
        public static RoundHandler Instance
        {
            get
            {
                if(instance == null)
                {
                    instance = FindObjectOfType<RoundHandler>();
                    if (instance == null)
                        return null;
                    instance.Init();
                }
                return instance;
            }
        }
        /// <summary> Count down timer for round start. </summary>
        private float countDown;
        /// <summary> Reference to all the bards. </summary>
        private PlayerLife[] bards;
        public PlayerLife[] Bards
        {
            get
            {
                if (bards == null)
                    Init();
                return bards;
            }
        }
        /// <summary> Number of players that have died this round. </summary>
        private int deathCount;
        /// <summary> The score for each bard. </summary>
        private int[] scores;
        public int[] Scores { get { return scores; } }
        /// <summary> Bool array for tracking who is dead. </summary>
        private bool[] isDead;
        /// <summary> Vector for resetting camera postion. </summary>
        private Vector3 cameraPos;
        /// <summary> Quaternion for resetting camera rotation. </summary>
        private Quaternion cameraRot;
        /// <summary> Pointer to the camera to reset it. </summary>
        private CameraMovement cm;
        private GameObject hillObject;

		private bool roundActive;
        
        /// <summary> Initializes this object. </summary>
        void Init()
        {
            bards = new PlayerLife[Data.Instance.numOfPlayers];
            for (int i = 0; i < Bards.Length; i++)
            {
                Bards[i] = Data.Instance.Spawn(i, spawnPoints[i]).GetComponent<PlayerLife>();
				LevelControllerManager.instance.AddPlayer((PlayerID)(i+1), Bards[i].GetComponent<BaseControl>());
            }
        }

        void Start()
        {
            instance = this;
            timerText.text = "3";
            canvas.SetActive(true);
            countDown = 3f;
            scores = new int[Data.Instance.numOfPlayers];
            isDead = new bool[Data.Instance.numOfPlayers];
            if (bards == null)
                Init();
            Transform[] targets = new Transform[Data.Instance.numOfPlayers];
            for (int i = 0; i < targets.Length; i++)
            {
                targets[i] = Bards[i].transform;
            }
            cm = GameObject.FindObjectOfType<CameraMovement>();
            cm.targets = targets;
            cameraPos = cm.transform.position;
            cameraRot = cm.transform.localRotation;
            ResetRound();
            if (!Data.Instance.IsElimination)
            {
                hillObject = Instantiate(hill);
                hillObject.transform.position = spawnPoints[4].transform.position;
            }
        }

        void Update()
        {
			if(deathCount >= (Data.Instance.NumOfPlayers-1))
            {
                ResetRound();
                deathCount = 0;
                for(int i = 0; i < isDead.Length; i++)
                {
                    if (!isDead[i])
                        AddScore((PlayerID)(i+1));
                    isDead[i] = false;
                }
                cm.transform.position = cameraPos;
                cm.transform.localRotation = cameraRot;
                if (uiCaller != null)
                    uiCaller();
                //Debug.Log(scores[0] + " " + scores[1] + " " + scores[2] + " " + scores[3]);
            }
            if (countDown > -2)
            {
                countDown -= Time.deltaTime/2f;
				timerText.transform.localScale = Vector3.one*(Mathf.Abs(Mathf.Sin(countDown*3f))+Mathf.Abs(3-countDown));
				timerText.GetComponent<RectTransform>().anchoredPosition = new Vector2((0.5f-Random.value)*(3-countDown),(0.5f-Random.value)*(3-countDown))*timerText.transform.localScale.magnitude;
                if (countDown > 2)
                {
                    timerText.text = "3";
					timerText.color = Color.white;
                    //text effects
                }
                else if (countDown > 1)
                {
                    timerText.text = "2";
                    //text effects
                }
                else if (countDown > 0)
                {
                    timerText.text = "1";
                    //text effects
                }
                else if (countDown > -1)
                {
					if(!roundActive) StartRound();
                    timerText.text = "Start!";
					timerText.color = Color.red;
					timerText.CrossFadeAlpha(0f,1f,false);
                    //text effects
                }
                else
                {
                    canvas.SetActive(false);
                    countDown = -5000;
                }
            }
            bool done = false;
            for(int i = 0; i < scores.Length; i++)
            {
                if(Data.Instance.IsElimination)
                {
                    if (scores[i] > 10)
                        done = true;
                }
                else
                {
                    if (scores[i] > 29)
                        done = true;
                }
            }
            if (done)
            {
                countDown = -5000;
                canvas.SetActive(true);
                ResetRound();
                timerText.text = "Finish!";
            }
        }

        /// <summary> Callback for the mid round UI to start the next round. </summary>
        public void BeginRound()
        {
			canvas.SetActive(true);
			GetComponent<AudioSource>().Play();
            countDown = 3f;
        }

        /// <summary> Register the function to turn the mid round score display. </summary>
        /// <param name="function"> A function that turn the UI on. </param>
        public void RegisterUI(OpenMidRoundUI function)
        {
            uiCaller = function;
        }

        /// <summary> Increments the death count for this round or sets the given player to respawn. </summary>
        /// <param name="id"> The player that died. </param>
        public void AddDeath(PlayerID id)
        {
            if (id != PlayerID.None)
            {
                if (Data.Instance.IsElimination)
                {
                    deathCount++;
                    isDead[((int)id) - 1] = true;
                }
                else
                {
                    StartCoroutine(Respawn(((int)id) - 1));
                }
            }
        }

        /// <summary> Increments the score of the given player. </summary>
        /// <param name="id"> The player to increment. </param>
        public void AddScore(PlayerID id)
        {
            if (id != PlayerID.None)
            {
                scores[((int)id) - 1]++;
            }
        }

        /// <summary> Co-routine for respawning players in king of the hill. </summary>
        /// <param name="player"> The player to respawn. </param>
        /// <returns> Nothing. </returns>
        private IEnumerator Respawn(int player)
        {
            Bards[player].GetComponent<BaseControl>().enabled = false;
            Bards[player].GetComponent<BaseBard>().enabled = false;
            Bards[player].GetComponent<CharacterController>().enabled = false;
            if (Bards[player].GetComponent<NavMeshAgent>())
                Bards[player].GetComponent<NavMeshAgent>().enabled = false;
            yield return new WaitForSeconds(3);
            Bards[player].GetComponent<BaseControl>().enabled = true;
            Bards[player].GetComponent<BaseBard>().enabled = true;
            Bards[player].GetComponent<CharacterController>().enabled = true;
            if (Bards[player].GetComponent<NavMeshAgent>())
                Bards[player].GetComponent<NavMeshAgent>().enabled = true;
            Bards[player].Respawn();
            Bards[player].transform.position = spawnPoints[player].position;
            yield return null;
        }

        /// <summary> Reset all of the bards. </summary>
        private void ResetRound()
        {
			roundActive = false;
			LevelManager.instance.music.Stop();
            for (int i = 0; i < Bards.Length; i++)
            {
                Bards[i].GetComponent<BaseControl>().enabled = false;
                Bards[i].GetComponent<BaseBard>().enabled = false;
                Bards[i].GetComponent<CharacterController>().enabled = false;
                if(Bards[i].GetComponent<NavMeshAgent>())
                    Bards[i].GetComponent<NavMeshAgent>().enabled = false;
                Bards[i].transform.position = spawnPoints[i].position;
            }
        }

        /// <summary> Spawn all of the bards so they can begin. </summary>
        private void StartRound()
        {
			roundActive = true;
			LevelManager.instance.music.Play();
            for (int i = 0; i < Bards.Length; i++)
            {
                Bards[i].GetComponent<BaseControl>().enabled = true;
                Bards[i].GetComponent<BaseBard>().enabled = true;
                Bards[i].GetComponent<CharacterController>().enabled = true;
                if (Bards[i].GetComponent<NavMeshAgent>())
                    Bards[i].GetComponent<NavMeshAgent>().enabled = true;
                Bards[i].Respawn();
                //Bards[i].transform.position = spawnPoints[i].position;
            }
            if (!Data.Instance.IsElimination)
            {
                hill.transform.parent = null;
                hill.transform.position = spawnPoints[4].transform.position;
            }
        }
        
        /// <summary> Used to retrieve all non-AI bards. </summary>
        /// <returns> an array of non-AI bards. </returns>
        public PlayerControl[] PlayerControl()
        {
            System.Collections.Generic.List<PlayerControl> bc = new System.Collections.Generic.List<PlayerControl>();
            for (int i = 0; i < Bards.Length; i++)
                if (Bards[i].GetComponent<PlayerControl>() != null)
                    bc.Add(Bards[i].GetComponent<PlayerControl>());
            return bc.Count > 0 ? bc.ToArray() : null;
        }

        /// <summary> Used to retrieve all the bards in the game. </summary>
        /// <returns> An array of all the bards. </returns>
        public BaseControl[] Control()
        {
            BaseControl[] bc = new BaseControl[Data.Instance.numOfPlayers];
            for (int i = 0; i < Bards.Length; i++)
                bc[i] = Bards[i].GetComponent<BaseControl>();
            return bc;
        }
    }
}
