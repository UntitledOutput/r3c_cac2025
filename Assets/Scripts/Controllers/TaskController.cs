
public class TaskController : MonoBehaviour {

    public class TaskReward {
        public Sprite Icon;
        public UnityEvent OnAward;
        public int Count;
    }

    public class Task {
        public string name;
        public string description;

        public Sprite icon;

        public List<TaskReward> Rewards;

        public void Complete() {

            TaskController.Instance.OnReward(this);

            foreach (TaskReward reward in Rewards) {
                reward.OnAward.Invoke();
            }
        }
    }

    public List<Task> Tasks;

    public static TaskController Instance;

    public void AddNewTask(Task t) {
        Tasks.add(t);
    }  

    

    protected void OnReward(Task t) {
        Tasks.Remove(t);
    }
    
    /// <summary>
    /// Awake is called when the script instance is being loaded.
    /// </summary>
    void Awake()
    {
        if (Instance) {
            DestroyImmediate(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    /// <summary>
    /// Start is called on the frame when a script is enabled just before
    /// any of the Update methods is called the first time.
    /// </summary>
    void Start()
    {
        
    }
}