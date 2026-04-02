using UnityEngine;

namespace Utilitys
{
    /// <summary>
    /// 모노 오브젝트를 싱글톤으로 설정 시켜주는 클래스입니다.
    /// </summary>
    public class SingletonMonoBehaviour<T> : MonoBehaviour where T : MonoBehaviour
    {
        [Header("씬 전환 유지 설정")]
        [SerializeField] private bool useDontDestroyOnLoad = true;

        private static T _instance;

        public static T Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = FindAnyObjectByType<T>();
                    if (_instance == null)
                    {
                        Debug.LogError($"현재 씬에 {typeof(T).Name}가 없습니다.");
                    }
                }
                return _instance;
            }
        }

        private void Awake()
        {
            if (_instance == null)
            {
                _instance = this as T;

                if (useDontDestroyOnLoad)
                    if (transform.parent == null)
                        DontDestroyOnLoad(gameObject);
                    else
                        Debug.LogWarning($"{typeof(T).Name}는 부모가 있어 DontDestroyOnLoad가 적용되지 않습니다.");

                OnSingletonAwake();
            }
            else if (_instance != this)
            {
                Destroy(gameObject);
            }
        }

        /// <summary>
        /// SingletoneMonoBehaivour에서 인스턴스 설정이 완료된 경우에만 호출되는 Awake
        /// </summary>
        protected virtual void OnSingletonAwake() { }

        private void OnDestroy()
        {
            if (_instance == this)
                _instance = null;
        }
    }
}
