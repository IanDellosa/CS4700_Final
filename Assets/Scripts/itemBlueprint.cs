using UnityEngine;

public class itemBlueprint
{
    public string itemName, req1, req2;

    public int req1Amt, req2Amt;

    public int numOfReqs;

    public itemBlueprint(string itemName, string req1, string req2, int req1Amt, int req2Amt, int numOfReqs)
    {
        this.itemName = itemName;
        this.req1 = req1;
        this.req2 = req2;
        this.req1Amt = req1Amt;
        this.req2Amt = req2Amt;
        this.numOfReqs = numOfReqs;
    }
}
