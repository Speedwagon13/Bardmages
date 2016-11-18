using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

namespace Assets.Scripts.Data
{
    class Data : MonoBehaviour
    {
        /// <summary> Reference to the player prefabs for each character. </summary>
        [SerializeField]
        [Tooltip("Reference to the player prefabs for each character. 4 expected")]
        private BaseBard[] characters;
        /// <summary> Reference to the AI prefabs for each character. </summary>
        [SerializeField]
        [Tooltip("Reference to the AI prefabs for each character. 4 expected")]
        private BaseBard[] aiPrefabs;
        /// <summary> Reference to the instrument prefabs. </summary>
        [SerializeField]
        [Tooltip("Reference to the instrument prefabs. 4 expected")]
        private GameObject[] instrumentPrefabs;

        /// <summary> Default tunes for testing. </summary>
        [SerializeField]
        [Tooltip("Default tunes for testing. 3 expected")]
        private Tune[] defaultTunes;

        /// <summary> Default sounds for testing. </summary>
        [SerializeField]
        [Tooltip("Default sounds for testing. 4 expected")]
        private AudioClip[] defaultClips;
        /// <summary> Maps audio clips to their array indices. </summary>
        private Dictionary<AudioClip, int> clipInstrumentMap;

        /// <summary> internal reference fo ensuring singleton. </summary>
        private static Data instance;
        public static Data Instance { get { return instance; } }

        /// <summary> The tunes for each bard. </summary>
        private Tune[][] tunes;
        /// <summary> The sound each bard makes. </summary>
        internal AudioClip[] clips;
        /// <summary> The instrument model to spawn for each bard. </summary>
        private GameObject[] instruments;
        /// <summary> Count of the number of players to spawn. </summary>
        internal int numOfPlayers;
        public int NumOfPlayers { get { return numOfPlayers; } set { numOfPlayers = value; } }

		/// <summary> Whether each of the players is computer-controlled. </summary>
		[HideInInspector]
		[Tooltip("Whether each of the players is computer-controlled.")]
		public bool[] isAIPlayer;

        /// <summary> The level to async load. </summary>
        internal string level;

        private bool isElimination;
        public bool IsElimination { get { return isElimination; } set { isElimination = value; } }

        void Awake()
        {
            if (Instance == null)
			{
                DontDestroyOnLoad(this.gameObject);
                instance = this;
            }
            else if (this != Instance)
            {
                Destroy(this.gameObject);
                return;
            }
            isElimination = true;
            level = "Test";
            tunes = new Tune[4][];
            numOfPlayers = 4;
            for (int i = 0; i < tunes.Length; i++)
            {
                tunes[i] = new Tune[3];
                for (int j = 0; j < tunes[i].Length; j++)
                    tunes[i][j] = defaultTunes[j];
            }
            clips = new AudioClip[4];
            for (int i = 0; i < clips.Length; i++)
                clips[i] = defaultClips[i];
            clipInstrumentMap = new Dictionary<AudioClip, int>(defaultClips.Length);
            for (int i = 0; i < defaultClips.Length; i++)
            {
                clipInstrumentMap.Add(defaultClips[i], i);
            }
            instruments = new GameObject[4];
            DontDestroyOnLoad(this.gameObject);
			isAIPlayer = new bool[4];
        }

        // TODO TEST CODE REMOVE
        void Update()
        {
            if (Input.GetKeyUp(KeyCode.P))
                loadScene("Test");
        }

        /// <summary> Loads a level. </summary>
        /// <param name="name"> The level to go to. </param>
        public void loadScene(string name)
        {
            level = name;
            SceneManager.LoadScene("Load");
        }

        /// <summary> Spawns a bard in the scene. </summary>
        public GameObject Spawn(int character, Transform spawn)
        {
			BaseBard[] prefabArray = isAIPlayer[character] ? aiPrefabs : characters;
            BaseBard b = Instantiate(prefabArray[character]);
            b.GetComponent<BaseControl>().player = (PlayerID)(character + 1);
            b.transform.position = spawn.position;
            b.tunes = tunes[character];
            b.instrumentSound = clips[character];

            GameObject instrumentPrefab = instrumentPrefabs[clipInstrumentMap[clips[character]]];
            Transform modelParent = b.transform.FindChild("bardmage_export");
            GameObject instrument = (GameObject)Instantiate(instrumentPrefab, modelParent.position, modelParent.rotation);
            instrument.transform.parent = modelParent;
            instrument.transform.localScale = Vector3.one;

            return b.gameObject;
        }

        /// <summary> Clear the saved bard data. </summary>
        public void Reset()
        {
            tunes = new Tune[4][];
            for (int i = 0; i < tunes.Length; i++)
                tunes[i] = new Tune[3];
            clips = new AudioClip[4];
            instruments = new GameObject[4];
        }

        /// <summary> Associate a tune with a specific bard. </summary>
        /// <param name="id"> The bard to add it to. </param>
        /// <param name="tune"> The tune to add. </param>
        /// <param name="index"> The index of this tune. 1-3. Determines order in UI. </param>
        public void AddTuneToPlayer(PlayerID id, Tune tune, int index)
        {
            tunes[(int)id - 1][index] = tune;
        }

        /// <summary> Associate a specific sound with a bard. </summary>
        /// <param name="id"> The bard to add it to. </param>
        /// <param name="instrumentSound"> The sound to add. </param>
        public void AddSoundToPlayer(PlayerID id, AudioClip instrumentSound)
        {
            clips[(int)id - 1] = instrumentSound;
        }

        /// <summary> Associate an instrument model with a bard. </summary>
        /// <param name="id"> The bard to add it to. </param>
        /// <param name="instrument"> The model to add. </param>
        public void AddInstrumentToPlayer(PlayerID id, GameObject instrument)
        {
            instruments[(int)id - 1] = instrument;
        }

		public Tune[] GetAllTunes() {
			return defaultTunes;
		}

		public Tune[] GetPlayerTunes(PlayerID player) {
			return tunes[((int)player)-1];
		}


    }
}
