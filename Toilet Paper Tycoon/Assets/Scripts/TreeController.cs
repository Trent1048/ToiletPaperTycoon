using System.Collections.Generic;
using UnityEngine;




public class TreeController : MonoBehaviour {

    public GameObject[] plantStage;

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
        float startTime = -5.0f;
        float endTime = 0f;

        float growth = Mathf.Lerp(0, 1, (endTime - Time.time) / (endTime - startTime));
        int treeForm = (int)Mathf.Floor(growth * 4);
        for (int i = 0; i < 4; i++) plantStage[i].SetActive(false);
        plantStage[treeForm].SetActive(true);

    }
}
