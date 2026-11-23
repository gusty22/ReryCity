using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace ProjetoGrau2.Core
{
    /// <summary>
    /// Gerencia carregamento assíncrono real de cenas em RERY CITY.
    /// Monitora o progresso de inicialização com interpolação suave e eventos de HUD.
    /// </summary>
    [AddComponentMenu("Rery City/Core/Asynchronous Scene Loader")]
    public class AsynchronousSceneLoader : MonoBehaviour
    {
        public static AsynchronousSceneLoader Instance { get; private set; }

        [Header("Configurações de Transição")]
        [SerializeField] private float progressSmoothingSpeed = 3.0f;
        [SerializeField] private float minimumLoadingTime = 0.5f;

        [Header("Estado")]
        [SerializeField] private bool isLoading = false;
        [SerializeField] private float displayProgress = 0f;
        [SerializeField] private string targetSceneName = string.Empty;

        public event Action<string> OnLoadStarted;
        public event Action<float> OnProgressChanged;
        public event Action<string> OnLoadCompleted;

        public bool IsLoading => isLoading;
        public float DisplayProgress => displayProgress;
        public string TargetSceneName => targetSceneName;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        /// <summary>
        /// Inicia o carregamento assíncrono de uma cena por nome.
        /// </summary>
        public void LoadSceneAsync(string sceneName, bool allowSceneActivation = true)
        {
            if (isLoading)
            {
                Debug.LogWarning($"[AsynchronousSceneLoader] Carregamento já em andamento para '{targetSceneName}'.");
                return;
            }

            StartCoroutine(LoadSceneRoutine(sceneName, allowSceneActivation));
        }

        private IEnumerator LoadSceneRoutine(string sceneName, bool allowSceneActivation)
        {
            isLoading = true;
            targetSceneName = sceneName;
            displayProgress = 0f;
            OnLoadStarted?.Invoke(sceneName);

            AsyncOperation asyncOp = SceneManager.LoadSceneAsync(sceneName);
            if (asyncOp == null)
            {
                Debug.LogError($"[AsynchronousSceneLoader] Falha ao carregar cena '{sceneName}'.");
                isLoading = false;
                yield break;
            }

            asyncOp.allowSceneActivation = false;
            float startTime = Time.realtimeSinceStartup;

            while (!asyncOp.isDone)
            {
                // Unity reporta progresso de 0.0 até 0.9 enquanto carrega os recursos
                float realProgress = Mathf.Clamp01(asyncOp.progress / 0.9f);
                float elapsedTime = Time.realtimeSinceStartup - startTime;

                displayProgress = Mathf.MoveTowards(displayProgress, realProgress, Time.unscaledDeltaTime * progressSmoothingSpeed);
                OnProgressChanged?.Invoke(displayProgress);

                if (asyncOp.progress >= 0.9f && elapsedTime >= minimumLoadingTime)
                {
                    displayProgress = 1.0f;
                    OnProgressChanged?.Invoke(displayProgress);

                    if (allowSceneActivation)
                    {
                        asyncOp.allowSceneActivation = true;
                    }
                    break;
                }

                yield return null;
            }

            isLoading = false;
            OnLoadCompleted?.Invoke(sceneName);
        }

        /// <summary>
        /// Utilitário para testes e validação de cálculo de progresso.
        /// </summary>
        public static float NormalizeUnityProgress(float rawProgress)
        {
            return Mathf.Clamp01(rawProgress / 0.9f);
        }
    }
}
