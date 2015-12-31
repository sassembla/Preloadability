using System.Linq;
using UnityEngine;

public class SushiboxController : MonoBehaviour {
    
    private void OnCollisionEnter2D (Collision2D coll) {
        
        switch (coll.gameObject.name) {
            case "chara(Clone)": {
                var rigid2D = GetComponent<Rigidbody2D>();
                rigid2D.AddForce(Vector2.up * 200);
                
                var collider = GetComponent<Collider2D>();
                Destroy(collider);
                
                var spriteRenderer = GetComponent<SpriteRenderer>();
                spriteRenderer.sortingOrder = 1;
                
                var resourceName = "Assets/BundledResources/Resources/PreloadOnShop/";
                
                var itemKind = Random.Range(0, 4);
                
                switch (itemKind) {
                    case 0:{
                        resourceName = resourceName + "maguro.png";
                        break;
                    }
                    case 1:{
                        resourceName = resourceName + "tamago.png";
                        break;
                    }
                    case 2:{
                        resourceName = resourceName + "salmon.png";
                        break;
                    }
                    case 3:{
                        resourceName = resourceName + "ebi.png";
                        break;
                    }
                }

                // get resource contained bundle name from shared onMemoryAsstList.
                var containedBundleData = AssetBundleLoader.onMemoryBundleList.bundles
                    .Where(bundle => bundle.resources.Contains(resourceName))
                    .FirstOrDefault();
                    
                StartCoroutine(AssetBundleLoader.DownloadBundleThenLoadAsset(
                    resourceName,
                    containedBundleData,
                    (Sprite t) => {
                        spriteRenderer.sprite = t;
                        spriteRenderer.transform.localScale = new Vector2(0.4f, 0.4f);
                    }
                ));
                break;
            }
        }
    }
    
    private int frame;
    
    private void Update () {
        if (transform.position.y < 158f) {
            transform.position = new Vector2(transform.position.x, 158f);
            
            if (false) frame++;
            
            if (300 < frame) Destroy(gameObject);
        }
    }
}