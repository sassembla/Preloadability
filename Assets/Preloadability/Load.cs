using UnityEngine;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class Load : MonoBehaviour {
	public static AssetList onMemoryAssetList = new AssetList();

	private bool preloadIsDone = false;


	private List<string> preloadList = new List<string>();
	private int preloadCount;


	public const string RESOURCE_URLBASE = "https://dl.dropboxusercontent.com/u/36583594/outsource/Preloadability/";
	public const int FIXED_VERSION_NUM = 1;// forcely, 1.


	private Texture2D onDemandTexture;


	void Start () {
		// delete all cache before running.
		Caching.CleanCache();

		// get whole asset list & preload for "Load" scene.
		StartCoroutine(GetScenePreloadList());
	}

	private IEnumerator GetScenePreloadList () {
		
		// 1. load whole asset list from web.
		var www1 = new WWW(RESOURCE_URLBASE + "assetlist.json");
		yield return www1;

		// deserialize & allocate in memory(should save into local in actual use.)
		JsonUtility.FromJsonOverwrite(www1.text, onMemoryAssetList);

		// 2. load "Load" scene preloadList from web.
		var www2 = new WWW(RESOURCE_URLBASE + "preload_load.json");
		yield return www2;

		// got preload list for "Load" scene. let's preload it.
		var preloadListForLoad = JsonUtility.FromJson<PreloadList>(www2.text);
		preloadList = new List<string>(preloadListForLoad.preloadBundleNames);
		preloadCount = preloadList.Count;

		// 3. start preload bundles.
		foreach (var bundleName in preloadListForLoad.preloadBundleNames) {
			StartCoroutine(PreloadBundle(bundleName));
		}
	}

	private IEnumerator PreloadBundle (string preloadingBundleName) {
		var preloadingBundleUrl = RESOURCE_URLBASE + "bundles/" + preloadingBundleName;

		var crc = onMemoryAssetList.bundles.Where(bundle => bundle.bundleName == preloadingBundleName).FirstOrDefault().crc;

		// 4. download & cache AssetBundle.
		var www = WWW.LoadFromCacheOrDownload(preloadingBundleUrl, FIXED_VERSION_NUM, crc);
		yield return www;

		while (!Caching.IsVersionCached(preloadingBundleUrl, FIXED_VERSION_NUM)) {
			yield return null;
		}

		Debug.LogError("preloaded asset is just cached. " + preloadingBundleUrl);

		preloadList.Remove(preloadingBundleName);
		if (!preloadList.Any()) preloadIsDone = true;
	}

	private IEnumerator DownloadOndemandBundle (string resourceName, AssetData onDemandLoadingBundle) {
		var onDemandLoadingBundleName = onDemandLoadingBundle.bundleName;
		var crc = onDemandLoadingBundle.crc;

		var onDemandLoadingBundleUrl = RESOURCE_URLBASE + "bundles/" + onDemandLoadingBundleName;

		// 5. download & cache AssetBundle.
		var www = WWW.LoadFromCacheOrDownload(onDemandLoadingBundleUrl, FIXED_VERSION_NUM, crc);
		yield return www;

		Debug.LogError("onDemand asset is just cached. " + onDemandLoadingBundleUrl);

		onDemandTexture = www.assetBundle.LoadAsset(resourceName) as Texture2D;
	}

	void OnGUI () {
		if (!preloadIsDone) {
			GUI.Button(new Rect(0,0,200,100), "preloading... " + preloadList.Count + " of " + preloadCount);
			return;
		}

		
		if (onDemandTexture == null) {
			if (GUI.Button(new Rect(0,0,400,100), "start downloading the texture resource for this button on demand.")) {
				var resourceName = "a1.png";
				var targetBundle = onMemoryAssetList.bundles.Where(bundle => bundle.resources.Contains(resourceName)).FirstOrDefault();
				StartCoroutine(DownloadOndemandBundle(resourceName, targetBundle));
			}
			return;
		}

		if (GUI.Button(new Rect(0,0,300,300), onDemandTexture)) {
			Application.LoadLevelAsync("Game");
		}
			
	}
}
