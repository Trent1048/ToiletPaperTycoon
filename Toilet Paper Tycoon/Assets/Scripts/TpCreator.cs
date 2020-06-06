using UnityEngine;

public class TpCreator : ConveyorController
{

    public GameObject tp;
    public int howMuchTp;
    private int machineProcess;
    public Sprite brokeMachine;
    private int paperNum = 0;
    private bool stop;

    // Update is called once per frame
    void Update()
    {
        if (stop == false)
        {
            if (storedObject) CountPaper();
        }
    }


    public void CountPaper()
    {
        if (storedObject.CompareTag("Single Ply Tp"))
        {
            paperNum++;
            Destroy(storedObject);
            if (paperNum == howMuchTp)
            {
                paperNum = 0;
                storedObject = Instantiate(tp, transform);
            }
            MachineDegrade();
        }

    }
    public void MachineDegrade()
    {
        //randomly breaks after certain number of processes, range can be adjusted
        if (machineProcess > Random.Range(2, 4))
        {

            Debug.Log("broke");
            //spriteRenderer.sprite = brokenMachine; can add sprite l8r
            machineProcess = 0;
            stop = true;
        }
    }
}
