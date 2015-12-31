using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class Shop : MonoBehaviour {
    
    private List<string> preloadList = new List<string>();
    
    private bool preloadIsDone;


    // Use this for initialization
    void Start () {
        // get whole asset list & preload for "Game" scene.
		StartCoroutine(GetScenePreloadList());
    }
    
    private IEnumerator GetScenePreloadList () {
        var www2 = new WWW(Settings.RESOURCE_URLBASE + "preload_shop.json");
		yield return www2;

		var preloadListForShop = JsonUtility.FromJson<PreloadList>(www2.text);
		preloadList = new List<string>(preloadListForShop.preloadBundleNames);

		foreach (var bundleName in preloadListForShop.preloadBundleNames) {
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
                        
                        Debug.Log("んーと、アイテムの画像とかが取得できたんで、それらを表示する");		
					}
				}
			));
		}
	}
	
	// Update is called once per frame
	void Update () {
	   
	}
    
    public void GetMaguro () {
        Debug.Log("押したら、このアイテムの詳細を取得しようとして、詳細の取得完了時にオンデマンドでDL、完了したら表示を行う。");
    }
    public void GetSalmon () {
        
    }
    public void GetTamago () {
        
    }
    public void GetEbi () {
        
    }
    
}
