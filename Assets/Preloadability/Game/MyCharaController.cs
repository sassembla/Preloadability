using UnityEngine;
public class MyCharaController : MonoBehaviour {
    private int direction = 1;
    
    public void Update () {
        transform.position = new Vector2(transform.position.x + (0.1f * direction), transform.position.y);
    }
    
    public void OnCollisionEnter2D (Collision2D coll) {
        
        switch (coll.gameObject.name) {
            case "stage": {
                // ignore
                break;
            }
            case "stage (1)": {
                // left wall.
                direction = 1;
                break;
            }
            case "stage (2)": {
                // right wall.
                direction = -1;
                break;
            }
            default: {
                // hit by sushibox.
                direction = direction * -1;
                break;
            }
        }
    }
    
}