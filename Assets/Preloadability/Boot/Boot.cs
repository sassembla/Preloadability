using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class Boot : MonoBehaviour {
	
	// flag for waiting preload load.
	private bool preloadIsDone = false;
	

	// list of preloading Asset.
	private List<string> preloadList = new List<string>();


	void Start () {
		// get whole asset list & preload list for this "Boot" scene.
		StartCoroutine(GetAllAssetListAndPreloadList());
	}

	public IEnumerator GetAllAssetListAndPreloadList () {
		while (!Caching.ready) {
			yield return null;
		}

		// delete all cache before running.
		// Caching.CleanCache();

		
		// 1. load whole bundle list from web.
		var www1 = new WWW(Settings.RESOURCE_URLBASE + "bundle_list.json");
		yield return www1;

		// deserialize & allocate on memory(should save into local in actual use.)
		JsonUtility.FromJsonOverwrite(www1.text, AssetBundleLoader.onMemoryBundleList);

		// 2. load "Boot" scene PreloadList from web.
		var www2 = new WWW(Settings.RESOURCE_URLBASE + "preload_" + "boot" + ".json");
		yield return www2;

		// got preload list for "Boot" scene. let's preload it.
		var preloadListForLoad = JsonUtility.FromJson<PreloadList>(www2.text);
		preloadList = new List<string>(preloadListForLoad.preloadBundleNames);

		// 3. start preload bundles.
		foreach (var bundleName in preloadListForLoad.preloadBundleNames) {
			// get target bundle's crc from shared onMemoryBundleList.
			var crc = AssetBundleLoader.onMemoryBundleList.bundles
				.Where(bundle => bundle.bundleName == bundleName)
				.FirstOrDefault().crc;

			/*
				start preload.
				set preloadIsDone -> true when all preloads are over.
			*/
			StartCoroutine(AssetBundleLoader.PreloadBundle(
				bundleName, 
				crc,
				(string preloadedBundleName) => {
					preloadList.Remove(preloadedBundleName);
					if (!preloadList.Any()) {
						/*
							all preloaded assets are cached.
						*/
						preloadIsDone = true;

						/*
							ready loading bar.
							instantiate prefab which is already downloaded.
						*/
						var resourceName = "Assets/BundledResources/Resources/PreloadOnBoot/ProgressBar.prefab";

						var containedBundleData = AssetBundleLoader.onMemoryBundleList.bundles
							.Where(bundle => bundle.resources.Contains(resourceName))
							.FirstOrDefault();
						
						StartCoroutine(AssetBundleLoader.DownloadBundleThenLoadAsset(
							resourceName,
							containedBundleData,
							(GameObject loadingBarPrefab) => {
								var loadingBarObj = Instantiate(loadingBarPrefab);
							
								// add loadingBar object to canvas.
								var canvas = GameObject.Find("Canvas");
								loadingBarObj.transform.SetParent(canvas.transform, false);

								// hold progress image.
								loadingBarProgressImage = loadingBarObj.transform.Find("ProgressBar").GetComponent<Image>() as Image;
							}
						));

						/*
							start on-demand loading.
							hand model & button texture.

							model & texture will appear after downloading some AssetBundle.
						*/
						StartOndemandLoading();						
					}
				}
			));
		}
	}

	void OnGUI () {
		if (!preloadIsDone) {
			return;
		}

		/*
			preloading is done. show preloded resources.
		*/
		
		ShowOndemandLoadingBar();
	}

	
	private List<string> onDemandLoadingBundleNames = new List<string>();
	private float onDemandLoadingMax = 0f;

	private void StartOndemandLoading () {
		// use this resource name for this button's texture.
		var canvas = GameObject.Find("Canvas");

		// add button to GUI.
		{
			var buttonObj = new GameObject("DoneButton", typeof(RectTransform));
			buttonObj.transform.SetParent(canvas.transform, false);
			buttonObj.AddComponent<CanvasRenderer>();
			var buttonImage = buttonObj.AddComponent<Image>();
			buttonImage.rectTransform.anchoredPosition = new Vector2(124, -250);
			buttonImage.rectTransform.sizeDelta = new Vector2(420, 116);

			var buttonTextObj = new GameObject("DoneButtonText", typeof(RectTransform));
			buttonTextObj.transform.SetParent(buttonObj.transform, false);
			buttonTextObj.AddComponent<CanvasRenderer>();
			var buttonText = buttonTextObj.AddComponent<Text>();
			buttonText.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
			buttonText.rectTransform.sizeDelta = new Vector2(420, 60);
			buttonText.text = "loading button texture...";
			buttonText.fontSize = 38;
			buttonText.alignment = TextAnchor.MiddleCenter;
			buttonText.color = Color.black;
			
			var button = buttonObj.AddComponent<Button>();
			button.onClick.AddListener(
				delegate {
					Debug.LogError("now loading on demand...");
				}
			);


			var resourceName = "Assets/BundledResources/Resources/OnDemandOnBoot/GoToNextButton/sushi.jpg";

			// get resource contained bundle name from shared onMemoryAsstList.
			var containedBundleData = AssetBundleLoader.onMemoryBundleList.bundles
				.Where(bundle => bundle.resources.Contains(resourceName))
				.FirstOrDefault();
			
			var bundleName = containedBundleData.bundleName;
			onDemandLoadingBundleNames.Add(bundleName);

			// load button image.
			StartCoroutine(AssetBundleLoader.DownloadBundleThenLoadAsset(
				resourceName,
				containedBundleData,
				(Sprite t) => {
					buttonText.text = string.Empty;
					buttonImage.sprite = t;

					button.onClick.RemoveAllListeners();

					button.onClick.AddListener(
						delegate {
							Application.LoadLevelAsync("Title");
						}
					);
					onDemandLoadingBundleNames.Remove(bundleName);
				}
			));
		}


		// load hand model
		{
			var resourceName = "Assets/BundledResources/Resources/OnDemandOnBoot/Hand/hand.prefab";

			var containedBundleData = AssetBundleLoader.onMemoryBundleList.bundles
				.Where(bundle => bundle.resources.Contains(resourceName))
				.FirstOrDefault();
			
			var bundleName = containedBundleData.bundleName;
			onDemandLoadingBundleNames.Add(bundleName);
			onDemandLoadingMax = onDemandLoadingBundleNames.Count;

			// load hand prefab then 
			StartCoroutine(AssetBundleLoader.DownloadBundleThenLoadAsset(
				resourceName,
				containedBundleData,
				(GameObject hand) => {
					var handObj = Instantiate(hand);
					handObj.transform.SetParent(canvas.transform, false);

					handObj.AddComponent<HandRotation>();

					onDemandLoadingBundleNames.Remove(bundleName);
				}
			));
		}
        
        // load title image
        {
            var resourceName = "Assets/BundledResources/Resources/PreloadOnTitle/Image.prefab";
            var containedBundleData = AssetBundleLoader.onMemoryBundleList.bundles
				.Where(bundle => bundle.resources.Contains(resourceName))
				.FirstOrDefault();
			
			var bundleName = containedBundleData.bundleName;
			onDemandLoadingBundleNames.Add(bundleName);
			onDemandLoadingMax = onDemandLoadingBundleNames.Count;

			// load title image prefab then 
			StartCoroutine(AssetBundleLoader.DownloadBundleThenLoadAsset(
				resourceName,
				containedBundleData,
				(GameObject titlePrefab) => {
                    onDemandLoadingBundleNames.Remove(bundleName);
				}
			));
        }
	}

	private Image loadingBarProgressImage;

	private void ShowOndemandLoadingBar () {
		if (loadingBarProgressImage != null) loadingBarProgressImage.fillAmount = ((onDemandLoadingMax - onDemandLoadingBundleNames.Count) * 1.0f) / onDemandLoadingMax;
	}
}
