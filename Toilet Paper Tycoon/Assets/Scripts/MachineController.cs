using UnityEngine;


public class MachineController : ConveyorController
{
  public GameObject wood;
  public GameObject toiletPaper;
  private int machineProcess;
  public Sprite brokeMachine;
  private bool stop;
  
    void Update()
    {
        if(stop == false)
        {
            if (storedObject) CollectWood();
        }
    }


    public void CollectWood()
    {

        if (storedObject.CompareTag("Wood"))
        {
            Destroy(storedObject);
            storedObject = Instantiate(toiletPaper, transform);
            machineProcess++;
            Debug.Log(machineProcess);
            MachineDegrade();
        }
    }

    public void MachineDegrade()
    {
        //randomly breaks after certain number of processes, range can be adjusted
        if (machineProcess > Random.Range(2,4)) 
        {

            Debug.Log("broke");
             //spriteRenderer.sprite = brokenMachine; can add sprite l8r
             machineProcess = 0;
             stop = true;
        }
    }
    
}