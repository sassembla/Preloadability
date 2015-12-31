using UnityEngine;
using UnityEngine.UI;

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


    private Texture2D titleImage;
    
    
    private GameObject preloadingProgressBar;
    

	void Start () {
        // set black screen.
        var canvasPanel = GameObject.Find("Panel"); 
        canvasPanel.GetComponent<Image>().color = Color.black;
        
        // set loading indicator.
        preloadingProgressBar = null; 
        
        
		// get whole asset list & preload for "Game" scene.
		StartCoroutine(GetScenePreloadList());
	}

	private IEnumerator GetScenePreloadList () {
		// 1. load " Title" scene preloadList from web.
		var www2 = new WWW(Settings.RESOURCE_URLBASE + "preload_title.json");
		yield return www2;

		// got preload list for "Title" scene. let's preload it.
		var preloadListForTitle = JsonUtility.FromJson<PreloadList>(www2.text);
		preloadList = new List<string>(preloadListForTitle.preloadBundleNames);

		// 2.start preload.
		foreach (var bundleName in preloadListForTitle.preloadBundleNames) {
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
                        
                        
                        var resourceName = "Assets/BundledResources/Resources/PreloadOnTitle/Image.prefab";
                        var containedBundleData = AssetBundleLoader.onMemoryBundleList.bundles
                            .Where(bundle => bundle.resources.Contains(resourceName))
                            .FirstOrDefault();
                            
                        // use cached asset for title.
                        StartCoroutine(AssetBundleLoader.DownloadBundleThenLoadAsset(
                            resourceName,
                            containedBundleData,
                            (GameObject titleImagePrefab) => {
                                var prefabImageObj = Instantiate(titleImagePrefab);
                                
                                var canvas = GameObject.Find("Canvas");
                                
                                // set canvas lighter.
                                var canvasPanel = canvas.transform.FindChild("Panel");
                                canvasPanel.GetComponent<Image>().color = Color.white;
                                
                                // add buttons.
                                {
                                    var goToGameButton = Instantiate(Resources.Load<GameObject>("goToGame"));
                                    goToGameButton.transform.SetParent(canvas.transform, false);
                                    goToGameButton.GetComponent<Button>().onClick.AddListener(
                                        delegate {
                                            GoToGame();
                                        }
                                    );
                                    
                                    var goToShopButton = Instantiate(Resources.Load<GameObject>("goToShop"));
                                    goToShopButton.transform.SetParent(canvas.transform, false);
                                    goToShopButton.GetComponent<Button>().onClick.AddListener(
                                        delegate {
                                            GoToShop();
                                        }
                                    );
                                }
                                
                                
                                prefabImageObj.transform.SetParent(canvas.transform, false);
                            }
                        ));
					}
				}
			));
		}    
	}
    
    
    private void GoToGame () {
        Application.LoadLevelAsync("Preloadability/Game/Game");
    }
    
    private void GoToShop () {
        Application.LoadLevelAsync("Preloadability/Shop/Shop");
    }
    
    
}
