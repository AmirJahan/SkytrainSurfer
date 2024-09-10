using UnityEngine;
using UnityEngine.SceneManagement;

namespace Passer {
	using Humanoid;

	public class StartScene : MonoBehaviour {

		public string sceneName;

		private string thisSceneName;

		protected void Awake() {
			Scene activeScene = UnityEngine.SceneManagement.SceneManager.GetActiveScene();
			thisSceneName = activeScene.name;

			UnityEngine.SceneManagement.SceneManager.sceneLoaded += OnSceneLoad;

			HumanoidControl pawn = FindObjectOfType<HumanoidControl>();
			if (pawn == null) {
				UnityEngine.SceneManagement.SceneManager.LoadScene(sceneName);
			}
			DontDestroyOnLoad(this.gameObject);

		}

		private void OnSceneLoad(UnityEngine.SceneManagement.Scene _, LoadSceneMode _1) {
#if UNITY_EDITOR
			SiteNavigator siteNavigator = FindObjectOfType<SiteNavigator>();
			if (siteNavigator != null) {
				siteNavigator.startScene = thisSceneName;
			}
#endif
		}
	}
}