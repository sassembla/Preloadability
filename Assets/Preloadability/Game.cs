using UnityEngine;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class Game : MonoBehaviour {
	private AssetList onMemoryAssetList;

	private bool preloadIsDone = false;


	private List<string> preloadList = new List<string>();
	private int preloadCount;


	private Texture2D onDemandTexture;

	void OnEnable () {
		this.onMemoryAssetList = Load.onMemoryAssetList;
	}

	void Start () {
		// get whole asset list & preload for "Game" scene.
		StartCoroutine(GetScenePreloadList());
	}

	private IEnumerator GetScenePreloadList () {
		// 1. load "Load" scene preloadList from web.
		var www2 = new WWW(Load.RESOURCE_URLBASE + "preload_game.json");
		yield return www2;

		// got preload list for "Game" scene. let's preload it.
		var preloadListForGame = JsonUtility.FromJson<PreloadList>(www2.text);
		preloadList = new List<string>(preloadListForGame.preloadBundleNames);
		preloadCount = preloadList.Count;

		// 2. start preload bundles.
		foreach (var bundleName in preloadListForGame.preloadBundleNames) {
			StartCoroutine(PreloadBundle(bundleName));
		}
	}

	private IEnumerator PreloadBundle (string preloadingBundleName) {
		var preloadingBundleUrl = Load.RESOURCE_URLBASE + "bundles/" + preloadingBundleName;

		var crc = onMemoryAssetList.bundles.Where(bundle => bundle.bundleName == preloadingBundleName).FirstOrDefault().crc;

		// 3. download & cache AssetBundle.
		var www = WWW.LoadFromCacheOrDownload(preloadingBundleUrl, Load.FIXED_VERSION_NUM, crc);
		yield return www;

		while (!Caching.IsVersionCached(preloadingBundleUrl, Load.FIXED_VERSION_NUM)) {
			yield return null;
		}

		Debug.LogError("preloaded asset is just cached. " + preloadingBundleUrl);

		preloadList.Remove(preloadingBundleName);
		if (!preloadList.Any()) preloadIsDone = true;
	}

	private IEnumerator DownloadOndemandBundle (string resourceName, AssetData onDemandLoadingBundle) {
		var onDemandLoadingBundleName = onDemandLoadingBundle.bundleName;
		var crc = onDemandLoadingBundle.crc;

		var onDemandLoadingBundleUrl = Load.RESOURCE_URLBASE + "bundles/" + onDemandLoadingBundleName;

		// 4. download & cache AssetBundle.
		var www = WWW.LoadFromCacheOrDownload(onDemandLoadingBundleUrl, Load.FIXED_VERSION_NUM, crc);
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
				var resourceName = "a3.png";
				var targetBundle = onMemoryAssetList.bundles.Where(bundle => bundle.resources.Contains(resourceName)).FirstOrDefault();
				StartCoroutine(DownloadOndemandBundle(resourceName, targetBundle));
			}
			return;
		}

		if (GUI.Button(new Rect(0,0,300,300), onDemandTexture)) {
			// Application.LoadLevelAsync("Game");
			Debug.LogError("何しようかな〜このデモの終わりですみたいなの流すかなあ。");
		}
			
	}
}
