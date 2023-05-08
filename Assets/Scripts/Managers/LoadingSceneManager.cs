using System.Collections;
using Scriptables;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Managers
{
    public class LoadingSceneManager : MonoBehaviour
    {
        [Header("Data")] [SerializeField] private GameConfigs gameConfigs;
   
        [Header("Dependencies")]
        [SerializeField] private Slider progressBar;
        [SerializeField] private Image gameIconImage;
        [SerializeField] private TextMeshProUGUI gameNameTMP;
    
        private Coroutine _loadLevelCoroutine;
        
        private void Awake()
        {
            if (gameConfigs == null)
            {
                Debug.LogWarning(string.Format("{0} has no game configs", gameObject.name));
            }
            else
            {
              
                InitalSceneConfigurations();
                ApplicationConfigurations();
            }
            
            progressBar.value = 0f;
            _loadLevelCoroutine = StartCoroutine(LoadLevelAsyncCoroutine());
        }

        private void OnValidate()
        {
            if (gameConfigs != null)
            {
                InitalSceneConfigurations();
            }
        }

        private void InitalSceneConfigurations()
        {
            gameIconImage.sprite = gameConfigs.gameIcon;
            gameNameTMP.text = gameConfigs.gameName;
            gameNameTMP.color = gameConfigs.gameNameColor;
        }

        private void ApplicationConfigurations()
        {
            Application.targetFrameRate = gameConfigs.targetFrameRate;
            QualitySettings.vSyncCount = gameConfigs.vSyncCount;
        }

        private IEnumerator LoadLevelAsyncCoroutine()
        {
            progressBar.value = 0.1f;

            yield return new WaitForSeconds(0.75f);
        
            var currentScene = SceneManager.GetActiveScene();
            int nextSceneIndex = currentScene.buildIndex + 1;
            //SceneManager.LoadScene(nextSceneIndex);
            var sceneLoadOperation = SceneManager.LoadSceneAsync(nextSceneIndex, LoadSceneMode.Additive);
        
            while (sceneLoadOperation.progress < 1)
            {
                progressBar.value = sceneLoadOperation.progress;

                yield return new WaitForEndOfFrame();
            }

            SceneManager.UnloadSceneAsync(currentScene);
        
            Debug.Log("Async Game Scene loading job is done..");

            yield return new WaitForEndOfFrame();
        }
    }
}

