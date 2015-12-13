using UnityEngine;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class AssetBundleLoader {
	/*
		shared list of all AssetBundle data for this App.
		static, initialize once, shared by every scene.
	*/
	public static BundleList onMemoryBundleList = new BundleList();

	/**
		Download specific AssetBundle before control.
	*/
	public static IEnumerator PreloadBundle (
		string preloadingBundleName,
		uint crc,
		Action<string> Succeeded
	) {
		var preloadingBundleUrl = Settings.RESOURCE_URLBASE + "bundles/" + preloadingBundleName;

		// download & cache AssetBundle.
		var www = WWW.LoadFromCacheOrDownload(preloadingBundleUrl, Settings.FIXED_VERSION_NUM, crc);
		yield return www;

		while (!Caching.IsVersionCached(preloadingBundleUrl, Settings.FIXED_VERSION_NUM)) {
			yield return null;
		}

		www.Dispose();

		Debug.Log("preloaded asset is just cached. " + preloadingBundleUrl);

		Succeeded(preloadingBundleName);
	}

	/**
		download specific AssetBundle then return asset from AssetBundle.
	*/
	public static IEnumerator DownloadBundleThenLoadAsset <T> (
		string resourceName, 
		BundleData onDemandLoadingBundle, 
		Action<T> Succeeded
	) where T : UnityEngine.Object {
		var onDemandLoadingBundleName = onDemandLoadingBundle.bundleName;
		var crc = onDemandLoadingBundle.crc;

		var loadingBundleUrl = Settings.RESOURCE_URLBASE + "bundles/" + onDemandLoadingBundleName;

		var www = WWW.LoadFromCacheOrDownload(loadingBundleUrl, Settings.FIXED_VERSION_NUM, crc);
		yield return www;

		while (!Caching.IsVersionCached(loadingBundleUrl, Settings.FIXED_VERSION_NUM)) {
			yield return null;
		}

		var assetBundle = www.assetBundle;
		www.Dispose();

		var loadedResource = (T)assetBundle.LoadAsset(resourceName, typeof(T));
		Succeeded(loadedResource);
	}
}