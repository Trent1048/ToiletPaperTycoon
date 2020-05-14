using System.Collections.Generic;
using UnityEngine;

public class TreeController : MonoBehaviour {

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

    private void Grow() {
        // put the grow stuff here
    }
}
