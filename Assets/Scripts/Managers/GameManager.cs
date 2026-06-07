using UnityEngine;
using UnityEngine.SceneManagement;

namespace CatWar
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }

        public enum GameState { MainMenu, Playing, Paused, GameOver, Victory }
        
        [Header("State")]
        [SerializeField] private GameState currentState = GameState.Playing;
        public GameState CurrentState => currentState;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                // Keep managers alive between scenes if necessary, 
                // but since it's a simple setup we can let it load per scene.
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void Start()
        {
            SetupLayerCollisions();
            SetState(GameState.Playing);
        }

        private void SetupLayerCollisions()
        {
            int playerUnit = LayerMask.NameToLayer("PlayerUnit");
            int enemyUnit = LayerMask.NameToLayer("EnemyUnit");
            int playerBase = LayerMask.NameToLayer("PlayerBase");
            int enemyBase = LayerMask.NameToLayer("EnemyBase");

            if (playerUnit != -1 && enemyUnit != -1 && playerBase != -1 && enemyBase != -1)
            {
                // Units of the same team pass through each other
                Physics2D.IgnoreLayerCollision(playerUnit, playerUnit, true);
                Physics2D.IgnoreLayerCollision(enemyUnit, enemyUnit, true);
                
                // Units pass through their own bases
                Physics2D.IgnoreLayerCollision(playerUnit, playerBase, true);
                Physics2D.IgnoreLayerCollision(enemyUnit, enemyBase, true);

                // Units collide with enemy units and enemy bases
                Physics2D.IgnoreLayerCollision(playerUnit, enemyUnit, false);
                Physics2D.IgnoreLayerCollision(playerUnit, enemyBase, false);
                Physics2D.IgnoreLayerCollision(enemyUnit, playerBase, false);
                
                Debug.Log("[GameManager] Runtime physics collision layers configured successfully.");
            }
            else
            {
                Debug.LogWarning("[GameManager] Custom layers (PlayerUnit, EnemyUnit, PlayerBase, EnemyBase) not found in the project. Please ensure they are added in Tags and Layers.");
            }
        }

        public void SetState(GameState newState)
        {
            currentState = newState;

            switch (newState)
            {
                case GameState.Playing:
                    Time.timeScale = 1f;
                    break;
                case GameState.Paused:
                    Time.timeScale = 0f;
                    break;
                case GameState.GameOver:
                    Time.timeScale = 0f;
                    break;
                case GameState.Victory:
                    Time.timeScale = 0f;
                    break;
            }

            if (UIManager.Instance != null)
            {
                UIManager.Instance.UpdateUIForState(newState);
            }
        }

        public void OnBaseDestroyed(bool isPlayerBase)
        {
            if (isPlayerBase)
            {
                SetState(GameState.GameOver);
            }
            else
            {
                SetState(GameState.Victory);
            }
        }

        public void PauseGame()
        {
            if (currentState == GameState.Playing)
            {
                SetState(GameState.Paused);
            }
        }

        public void ResumeGame()
        {
            if (currentState == GameState.Paused)
            {
                SetState(GameState.Playing);
            }
        }

        public void RestartLevel()
        {
            Time.timeScale = 1f;
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

        public void NextLevel()
        {
            Time.timeScale = 1f;
            if (LevelManager.Instance != null)
            {
                LevelManager.Instance.GoToNextLevel();
            }
            else
            {
                // Fallback: Reload same scene if level manager doesn't exist
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            }
        }

        public void QuitGame()
        {
            Application.Quit();
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#endif
        }
    }
}
