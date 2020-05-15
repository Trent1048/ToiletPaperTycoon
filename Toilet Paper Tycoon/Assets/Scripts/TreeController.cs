using System.Collections.Generic;
using UnityEngine;




public class TreeController : MonoBehaviour {

    public Sprite[] plantStage;
    public int growCount = 0;
    


    protected static List<TreeController> treeControllers;

    public static void GrowTrees() {
        if (treeControllers != null) {
            foreach (TreeController tree in treeControllers) {
                tree.Grow();
                
                

            }
        }
    }

    private void Start() {
        if (treeControllers == null) {
            treeControllers = new List<TreeController>();
        }
        treeControllers.Add(this);
    }

    private void OnDestroy() {
        treeControllers.Remove(this);
    }

    public void Grow()
    {
        
        
        if (growCount >= 0 && growCount < 5)
        {

            this.gameObject.GetComponent<SpriteRenderer>().sprite=plantStage[0];
        }
        if (growCount >=5 && growCount<10)
        {
            
            this.gameObject.GetComponent<SpriteRenderer>().sprite = plantStage[1];
        }
        if (growCount >= 10 && growCount < 15)
        {
            
            this.gameObject.GetComponent<SpriteRenderer>().sprite = plantStage[2]; 
        }
        if (growCount >= 15 )
        {
            
            this.gameObject.GetComponent<SpriteRenderer>().sprite = plantStage[3];
        }

        growCount++;
    }

}
