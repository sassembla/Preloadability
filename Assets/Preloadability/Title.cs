using UnityEngine;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

/**
	preload, on-demandの説明は終わってるので、応用編。
	タイトルの画像はpreload、押せるボタンはon-demand
*/
public class Title : MonoBehaviour {
	private bool preloadIsDone = false;


	private List<string> preloadList = new List<string>();
	private int preloadCount;


	void Start () {
		// get whole asset list & preload for "Game" scene.
		StartCoroutine(GetScenePreloadList());
	}

	private IEnumerator GetScenePreloadList () {
		// 1. load "Load" scene preloadList from web.
		var www2 = new WWW(Settings.RESOURCE_URLBASE + "preload_title.json");
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
		var preloadingBundleUrl = Settings.RESOURCE_URLBASE + "bundles/" + preloadingBundleName;

		var crc = AssetBundleLoader.onMemoryBundleList.bundles.Where(bundle => bundle.bundleName == preloadingBundleName).FirstOrDefault().crc;

		// 3. download & cache AssetBundle.
		var www = WWW.LoadFromCacheOrDownload(preloadingBundleUrl, Settings.FIXED_VERSION_NUM, crc);
		yield return www;

		while (!Caching.IsVersionCached(preloadingBundleUrl, Settings.FIXED_VERSION_NUM)) {
			yield return null;
		}

		Debug.LogError("preloaded asset is just cached. " + preloadingBundleUrl);

		preloadList.Remove(preloadingBundleName);
		if (!preloadList.Any()) preloadIsDone = true;
	}

	void OnGUI () {
		if (!preloadIsDone) {
			GUI.Button(new Rect(0,0,200,100), "preloading... " + preloadList.Count + " of " + preloadCount);
			return;
		}

		/*
			preloadが終わったら、on-demand loadingが勝手に発生しまくるような画面を作るか。
		*/
					
	}
}
