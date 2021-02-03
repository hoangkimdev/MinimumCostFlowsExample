//############################################################
// Siêu thị - siêu thị: cân bằng hàng tồn                 ####
//############################################################
// Store ~ Siêu thị: Tên, tọa độ, lượng hàng tồn (+ dư hàng, - thiếu hàng)
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
//############################################################
// Kho - Khu vực: chia hàng từ kho                        ####
//############################################################
// Node ~ một kho/ một khu vực: mã, lượng hàng
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
// LinkCase ~ một link giữa Node - Node: 
// Mã kho, lượng hàng tồn, mã khu vực, lượng nhu cầu, khoảng cách giữa 2 Node
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
// ResultLine ~ output: mã Node cung hàng, mã Node cầu hàng, 
// lượng hàng chuyển, chi phí vận chuyển (khoảng cách), tổng chi phí của lượt chuyển
public class ResultLine
{
    public int From { get; set; }
    public int To { get; set; }
    public int FLow { get; set; }
    public int Distance { get; set; }
    public int RealCost { get; set; }

    public ResultLine(int from, int to, int flow, int distance, int realCost)
    {
        this.From = from;
        this.To = to;
        this.FLow = flow;
        this.Distance = distance;
        this.RealCost = realCost;
    }
}