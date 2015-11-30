using UnityEngine;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;


public class Boot : MonoBehaviour {
	
	// flag for waiting preload load.
	private bool preloadIsDone = false;
	
	// flag for waiting on-demand load,
	private enum OnDemandLoadingState : int {
		STATE_READY,
		STATE_BOOTING,
		STATE_BOOTED
	};
	private OnDemandLoadingState onDemandLoadingState;


	// list of preloading Asset.
	private List<string> preloadList = new List<string>();
	// integer for counting max preloading assets.
	private int preloadCount;


	/*
		Resources for this scene.
	*/

	// texture of loading promotion. load this texture from web before user control.
	private Texture2D preloadCharaTexture;

	// texture of button. load this texture from web on-demand.
	private Texture2D onDemandTexture;


	void Start () {
		// delete all cache before running.
		Caching.CleanCache();

		// get whole asset list & preload list for this "Boot" scene.
		StartCoroutine(GetAllAssetListAndPreloadList());
	}

	public IEnumerator GetAllAssetListAndPreloadList () {
		
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
		preloadCount = preloadList.Count;

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

						// このへんで指モデルのローディング
					}
				}
			));
		}
	}

	void OnGUI () {
		if (!preloadIsDone) {
			GUI.Button(new Rect(0,0,200,100), "preloading... " + preloadList.Count + " of " + preloadCount);
			return;
		}

		/*
			preloading is done. show preloded resources.
		*/
		ShowOndemandLoadingPromotion();

		switch (onDemandLoadingState) {
			case OnDemandLoadingState.STATE_READY:{
				GUI.Button(new Rect(0,0,400,100), "downloading texture...");

				/*
					start on-demand loading.
					texture appears when this loading is over.
				*/

				// set state to loading.
				onDemandLoadingState = OnDemandLoadingState.STATE_BOOTING;

				// use this resource name for this button's texture.
				var resourceName = "a1.png";

				// get resource contained bundle name from shared onMemoryAsstList.
				var containedBundleData = AssetBundleLoader.onMemoryBundleList.bundles
					.Where(bundle => bundle.resources.Contains(resourceName))
					.FirstOrDefault();
				
				StartCoroutine(AssetBundleLoader.DownloadBundleThenLoadAsset(
					resourceName,
					containedBundleData,
					(Texture2D t) => {
						onDemandTexture = t;

						// on-demand loading is done.
						onDemandLoadingState = OnDemandLoadingState.STATE_BOOTED;
					}
				));
				break;
			}
			case OnDemandLoadingState.STATE_BOOTING: {
				if (GUI.Button(new Rect(0,0,400,100), "downloading texture...")) {}
				break;
			}
			case OnDemandLoadingState.STATE_BOOTED: {
				/*
					on-demand loading is done. use loaded resources.
				*/
				if (GUI.Button(new Rect(0,0,300,300), onDemandTexture)) {
					Application.LoadLevelAsync("Title");
				}
				break;
			}
		}
			
	}


	private void ShowOndemandLoadingPromotion () {
		Debug.Log("プリロードが終わったらどうしようかな、その素材に「次の素材がOn-demand loadされるよ」って書くか。ブランクのボタンを指差してるモデルでも動かそう。");
	}
}
