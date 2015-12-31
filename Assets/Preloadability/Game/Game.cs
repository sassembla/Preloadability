using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class Game : MonoBehaviour {

    private bool preloadIsDone;
    
    private GameObject hero;
    
    private GameObject sushiBox;
    
    private List<string> preloadList;
    
    
	// Use this for initialization
	void Start () {
        preloadIsDone = false;
        
        StartCoroutine(GetScenePreloadList());
	}
    
    
    private IEnumerator GetScenePreloadList () {
        var www2 = new WWW(Settings.RESOURCE_URLBASE + "preload_game.json");
		yield return www2;

		var preloadListForGame = JsonUtility.FromJson<PreloadList>(www2.text);
		preloadList = new List<string>(preloadListForGame.preloadBundleNames);

        var preloadedCount = 0;
        
		foreach (var bundleName in preloadListForGame.preloadBundleNames) {
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
						
                        // create character.
                        var resourceName0 = "Assets/BundledResources/Resources/PreloadOnGame/chara.prefab";
                        var containedBundleData0 = AssetBundleLoader.onMemoryBundleList.bundles
                            .Where(bundle => bundle.resources.Contains(resourceName0))
                            .FirstOrDefault();
                            
                        StartCoroutine(AssetBundleLoader.DownloadBundleThenLoadAsset(
                            resourceName0,
                            containedBundleData0,
                            (GameObject p) => {
                                hero = Instantiate(p) as GameObject;
                                hero.AddComponent<MyCharaController>();
                                preloadedCount++;
                                if (preloadedCount == 3) {
                                    preloadIsDone = true;
                                }
                            }
                        ));
                        
                        // set stage images.
                        var resourceName1 =  "Assets/BundledResources/Resources/PreloadOnGame/block.png";
                        var containedBundleData1 = AssetBundleLoader.onMemoryBundleList.bundles
                            .Where(bundle => bundle.resources.Contains(resourceName1))
                            .FirstOrDefault();
                            
                        StartCoroutine(AssetBundleLoader.DownloadBundleThenLoadAsset(
                            resourceName1,
                            containedBundleData1,
                            (Sprite t) => {
                                var stage = GameObject.Find("stage");
                                stage.GetComponent<SpriteRenderer>().sprite = t;
                                
                                var stage1 = GameObject.Find("stage (1)");
                               stage1.GetComponent<SpriteRenderer>().sprite = t;
                                
                                var stage2 = GameObject.Find("stage (2)");
                                stage2.GetComponent<SpriteRenderer>().sprite = t;
                                
                                preloadedCount++;
                                if (preloadedCount == 3) {
                                    preloadIsDone = true;
                                }
                            }
                        ));
                        
                        // get sushibox gameobject.
                        
                        var resourceName2 =  "Assets/BundledResources/Resources/PreloadOnGame/box.prefab";
                        var containedBundleData2 = AssetBundleLoader.onMemoryBundleList.bundles
                            .Where(bundle => bundle.resources.Contains(resourceName2))
                            .FirstOrDefault();
                            
                        StartCoroutine(AssetBundleLoader.DownloadBundleThenLoadAsset(
                            resourceName2,
                            containedBundleData2,
                            (GameObject b) => {
                                sushiBox = b;
                                
                                preloadedCount++;
                                if (preloadedCount == 3) {
                                    preloadIsDone = true;
                                }
                            }
                        ));
					}
				}
			));
		}
	}
	
    private int frame;
	// Update is called once per frame
	void Update () {
        if (!preloadIsDone) return;
        
        
        if (frame % (60 * 3) == 0) {
            if (sushiBox == null) return;
             
            var newSushiBox = Instantiate(sushiBox);
            newSushiBox.AddComponent<SushiboxController>();
            newSushiBox.transform.position = hero.transform.position + new Vector3(0, 100f);
        }
        
        frame++;
	}
    
    public void BackToTitle () {
        Application.LoadLevelAsync("Title");
    }
    
}