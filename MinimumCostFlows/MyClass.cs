public class Store
{
    public string Name { get; set; }
    public double LatY { get; set; }
    public double LongX { get; set; }
    public int Supplies { get; set; }
    public Store(string name, double latY, double longX, int supplies)
    {
        this.Name = name;
        this.LatY = latY;
        this.LongX = longX;
        this.Supplies = supplies;
    }
}
public class Node
{
    public int Id { get; set; }
    public int Num { get; set; }
    public Node(int id, int num)
    {
        this.Id = id;
        this.Num = num;
    }
}
public class LinkCase
{
    public int IdSupply { get; set; }
    public int NumSupply { get; set; }
    public int IdDemand { get; set; }
    public int NumDemand { get; set; }
    public int Distance { get; set; }
    public LinkCase(int idSupply, int numSupply, int idDemand, int numDemand, int distance)
    {
        this.IdSupply = idSupply;
        this.NumSupply = numSupply;
        this.IdDemand = idDemand;
        this.NumDemand = numDemand;
        this.Distance = distance;
    }
}
public class ResultLine
{
    public int From { get; set; }
    public int To { get; set; }
    public int FLow { get; set; }
    public int UnitCost { get; set; }
    public int Cost { get; set; }

    public ResultLine(int from, int to, int flow, int unitCost, int cost)
    {
        this.From = from;
        this.To = to;
        this.FLow = flow;
        this.UnitCost = unitCost;
        this.Cost = cost;
    }
}