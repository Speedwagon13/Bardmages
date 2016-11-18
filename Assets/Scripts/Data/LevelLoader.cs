using UnityEngine;
using UnityEngine.SceneManagement;

namespace Assets.Scripts.Data
{
    class LevelLoader : MonoBehaviour
    {
        /// <summary> The minimum amount of time in seconds to wait before loading the next scene. </summary>
        [SerializeField]
        [Tooltip("The minimum amount of time in seconds to wait before loading the next scene.")]
        private float waitTime;

        /// <summary> Gameobject to use as a loading bar. </summary>
        [SerializeField]
        [Tooltip("Gameobject to use as a loading bar.")]
        private GameObject loadBar;

        /// <summary> Reference to the data class with all of the properties needed. </summary>
        private Data data;
        /// <summary> Reference to the load operation to track it. </summary>
        private AsyncOperation op;
        /// <summary> Simple timer. </summary>
        private float time;
        /// <summary> The position of the bar at 100% </summary>
        private float xOrig;
        /// <summary> The scale of the bar at 100% </summary>
        private float xMax;

        void Start()
        {
            data = Data.Instance;
            op = SceneManager.LoadSceneAsync(data.level);
            op.allowSceneActivation = false;
            time = waitTime;
            xOrig = loadBar.transform.position.x;
            xMax = loadBar.transform.localScale.x;
			loadBar.transform.localScale = new Vector3(0f, 1f, 1f);
        }

        void Update()
        {
            float t = op.progress;
			loadBar.transform.position = new Vector3(Mathf.MoveTowards(loadBar.transform.position.x, xOrig - (1 - t), Time.deltaTime*(t*10f+2f)), loadBar.transform.position.y, loadBar.transform.position.z);
			loadBar.transform.localScale = new Vector3(Mathf.MoveTowards(loadBar.transform.localScale.x, xMax * t, Time.deltaTime*(t*10f+2f)), loadBar.transform.localScale.y, loadBar.transform.localScale.z);
            for (int i = 0; i < data.clips.Length; i++)
            {
                bool soundPlayed = false;
                foreach (ControllerInputWrapper.Buttons b in new ControllerInputWrapper.Buttons[] { ControllerInputWrapper.Buttons.A, ControllerInputWrapper.Buttons.B, ControllerInputWrapper.Buttons.X, ControllerInputWrapper.Buttons.Y })
                {
                    if (ControllerManager.instance.GetButtonDown(b, (PlayerID)(i+1)))
                    {
                        if (!soundPlayed)
                        {
                            GetComponent<AudioSource>().pitch = LevelManager.instance.buttonPitchMap[b];
                            GetComponent<AudioSource>().PlayOneShot(data.clips[i], .4f);
                            soundPlayed = true;
                        }
                    }
                }
            }
            if ((time -= Time.deltaTime) < 0)
                op.allowSceneActivation = true;
        }
    }
}
