using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UI;

public class Shop : MonoBehaviour {
    
    private List<string> preloadList = new List<string>();
    
    private GameObject canvas;
    private GameObject detailButton;


    // Use this for initialization
    void Start () {
        canvas = GameObject.Find("Canvas");
        detailButton = GameObject.Find("DetailButton");
        
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
                        
                        // MenuChara
                        {
                            var resourceName =  "Assets/BundledResources/Resources/PreloadOnShop/menu.png";
                            var containedBundleData = AssetBundleLoader.onMemoryBundleList.bundles
                                .Where(bundle => bundle.resources.Contains(resourceName))
                                .FirstOrDefault();
                                
                            StartCoroutine(AssetBundleLoader.DownloadBundleThenLoadAsset(
                                resourceName,
                                containedBundleData,
                                (Sprite s) => {
                                    var menuBgImage = GameObject.Find("MenuChara").GetComponent<Image>();
                                    menuBgImage.color = Color.white;
                                    menuBgImage.sprite = s;
                                }
                            ));
                        }
                        
                        // buttons images.
                        {
                            var resourceName =  "Assets/BundledResources/Resources/PreloadOnShop/maguro.png";
                            var containedBundleData = AssetBundleLoader.onMemoryBundleList.bundles
                                .Where(bundle => bundle.resources.Contains(resourceName))
                                .FirstOrDefault();
                                
                            StartCoroutine(AssetBundleLoader.DownloadBundleThenLoadAsset(
                                resourceName,
                                containedBundleData,
                                (Sprite s) => {
                                    var buttonImage = GameObject.Find("Button0").GetComponent<Image>();
                                    buttonImage.color = Color.white;
                                    buttonImage.sprite = s;
                                }
                            ));
                        }
                        
                        {
                            var resourceName =  "Assets/BundledResources/Resources/PreloadOnShop/tamago.png";
                            var containedBundleData = AssetBundleLoader.onMemoryBundleList.bundles
                                .Where(bundle => bundle.resources.Contains(resourceName))
                                .FirstOrDefault();
                                
                            StartCoroutine(AssetBundleLoader.DownloadBundleThenLoadAsset(
                                resourceName,
                                containedBundleData,
                                (Sprite s) => {
                                    var buttonImage = GameObject.Find("Button1").GetComponent<Image>();
                                    buttonImage.color = Color.white;
                                    buttonImage.sprite = s;
                                }
                            ));
                        }
                        
                        {
                            var resourceName =  "Assets/BundledResources/Resources/PreloadOnShop/salmon.png";
                            var containedBundleData = AssetBundleLoader.onMemoryBundleList.bundles
                                .Where(bundle => bundle.resources.Contains(resourceName))
                                .FirstOrDefault();
                                
                            StartCoroutine(AssetBundleLoader.DownloadBundleThenLoadAsset(
                                resourceName,
                                containedBundleData,
                                (Sprite s) => {
                                    var buttonImage = GameObject.Find("Button2").GetComponent<Image>();
                                    buttonImage.color = Color.white;
                                    buttonImage.sprite = s;
                                }
                            ));
                        }
                        
                        {
                            var resourceName =  "Assets/BundledResources/Resources/PreloadOnShop/ebi.png";
                            var containedBundleData = AssetBundleLoader.onMemoryBundleList.bundles
                                .Where(bundle => bundle.resources.Contains(resourceName))
                                .FirstOrDefault();
                                
                            StartCoroutine(AssetBundleLoader.DownloadBundleThenLoadAsset(
                                resourceName,
                                containedBundleData,
                                (Sprite s) => {
                                    var buttonImage = GameObject.Find("Button3").GetComponent<Image>();
                                    buttonImage.color = Color.white;
                                    buttonImage.sprite = s;
                                }
                            ));
                        }
					}
				}
			));
		}
	}
    
    
    public void GetMaguro () {
        var buttonImage = GameObject.Find("DetailButton").GetComponent<Image>();
        buttonImage.color = Color.gray;
        
        detailButton.transform.SetParent(canvas.transform, true);
        var resourceName =  "Assets/BundledResources/Resources/OnDemandOnShop/maguro.png";
        var containedBundleData = AssetBundleLoader.onMemoryBundleList.bundles
            .Where(bundle => bundle.resources.Contains(resourceName))
            .FirstOrDefault();
            
        StartCoroutine(AssetBundleLoader.DownloadBundleThenLoadAsset(
            resourceName,
            containedBundleData,
            (Sprite s) => {
                buttonImage.color = Color.white;
                buttonImage.sprite = s;
            }
        ));
    }
    public void GetTamago () {
        var buttonImage = GameObject.Find("DetailButton").GetComponent<Image>();
        buttonImage.color = Color.gray;
        
        detailButton.transform.SetParent(canvas.transform, true);
        var resourceName =  "Assets/BundledResources/Resources/OnDemandOnShop/tamago.png";
        var containedBundleData = AssetBundleLoader.onMemoryBundleList.bundles
            .Where(bundle => bundle.resources.Contains(resourceName))
            .FirstOrDefault();
            
        StartCoroutine(AssetBundleLoader.DownloadBundleThenLoadAsset(
            resourceName,
            containedBundleData,
            (Sprite s) => {
                buttonImage.color = Color.white;
                buttonImage.sprite = s;
            }
        ));
    }
    public void GetSalmon () {
        var buttonImage = GameObject.Find("DetailButton").GetComponent<Image>();
        buttonImage.color = Color.gray;
        
        detailButton.transform.SetParent(canvas.transform, true);
        var resourceName =  "Assets/BundledResources/Resources/OnDemandOnShop/salmon.png";
        var containedBundleData = AssetBundleLoader.onMemoryBundleList.bundles
            .Where(bundle => bundle.resources.Contains(resourceName))
            .FirstOrDefault();
            
        StartCoroutine(AssetBundleLoader.DownloadBundleThenLoadAsset(
            resourceName,
            containedBundleData,
            (Sprite s) => {
                buttonImage.color = Color.white;
                buttonImage.sprite = s;
            }
        ));
    }
    public void GetEbi () {
        var buttonImage = GameObject.Find("DetailButton").GetComponent<Image>();
        buttonImage.color = Color.gray;
        
        detailButton.transform.SetParent(canvas.transform, true);
        var resourceName =  "Assets/BundledResources/Resources/OnDemandOnShop/ebi.png";
        var containedBundleData = AssetBundleLoader.onMemoryBundleList.bundles
            .Where(bundle => bundle.resources.Contains(resourceName))
            .FirstOrDefault();
            
        StartCoroutine(AssetBundleLoader.DownloadBundleThenLoadAsset(
            resourceName,
            containedBundleData,
            (Sprite s) => {
                buttonImage.color = Color.white;
                buttonImage.sprite = s;
            }
        ));
    }
    
    public void BackToTitle () {
        Application.LoadLevelAsync("Title");
    }
    
    public void HideDetail () {
        detailButton.transform.SetParent(this.transform);
    }
}
