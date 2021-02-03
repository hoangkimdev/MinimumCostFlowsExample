using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Google.OrTools.Graph;
using System.Device.Location;

namespace MinimumCostFlows
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = Encoding.GetEncoding("UTF-8");
            //############################################################
            // input sample || Sieu thi - sieu thi                    ####
            //############################################################
            Store A = new Store("TP Hồ Chí Minh", 10.823099, 106.629662, 12);
            Store B = new Store("TP Dĩ An", 10.896476, 106.752739, -13);
            Store C = new Store("Thủ Dầu Một", 10.981680, 106.650490, -6);
            Store D = new Store("Biên Hoà", 10.957413, 106.842690, 8);
            Store E = new Store("TP Long An", 10.713440, 106.124840, -6);
            Store F = new Store("TP Bà Rịa VT", 10.493348, 107.168194, -3);
            Store G = new Store("TX Long Khánh", 10.938091, 107.240153, -8);
            Store H = new Store("Trảng Bàng", 11.033403, 106.358219, 16);
            Store I = new Store("TX Thuận An", 10.950260, 106.742140, 15);
            Store K = new Store("TX Tân Uyên", 11.060420, 106.796341, 3);
            Store M = new Store("TX Bến Cát", 11.151430, 106.593513, -13);
            Store N = new Store("TT Long Thành", 10.783620, 106.950699, 14);
            Store O = new Store("H Trảng Bom", 10.953290, 107.005341, -4);
            Store Q = new Store("TT Bến Lức", 10.635149, 106.491248, -13);

            List<Store> listStore = new List<Store> { A, B, C, D, E, F, G, H, I, K, M, N, O, Q };
            StoreSolveMinCost(listStore);

            //############################################################
            // input sample || Kho - Khu Vuc                          ####
            //############################################################
            Node A0 = new Node(0, 3000);    // 0 - "Kho Long Bình"
            Node B1 = new Node(11, 500);    // 1 - "Kho Cam Ranh"
            Node C2 = new Node(22, 500);    // 2 - "Kho Nhà Bè"
            Node D3 = new Node(33, 500);    // 3 - "Kho Long Bình 2"

            Node E4 = new Node(44, 2000);   // 4 - "KV Long Bình"
            Node F5 = new Node(55, 1000);   // 5 - "Kv Cam Ranh"
            Node G6 = new Node(66, 2500);   // 6 - "Kv Nhà Bè"

            LinkCase l0 = new LinkCase(A0.Id, A0.Num, E4.Id, E4.Num, 1);
            LinkCase l1 = new LinkCase(A0.Id, A0.Num, F5.Id, F5.Num, 999);
            LinkCase l2 = new LinkCase(A0.Id, A0.Num, G6.Id, G6.Num, 99);
            LinkCase l3 = new LinkCase(B1.Id, B1.Num, F5.Id, F5.Num, 1);
            LinkCase l4 = new LinkCase(B1.Id, B1.Num, G6.Id, G6.Num, 999);
            LinkCase l5 = new LinkCase(C2.Id, C2.Num, E4.Id, E4.Num, 99);
            LinkCase l6 = new LinkCase(C2.Id, C2.Num, G6.Id, G6.Num, 1);
            LinkCase l7 = new LinkCase(D3.Id, D3.Num, E4.Id, E4.Num, 1);
            LinkCase l8 = new LinkCase(D3.Id, D3.Num, F5.Id, F5.Num, 999);
            List<LinkCase> listLinkCase = new List<LinkCase> { l0, l1, l2, l3, l4, l5, l6, l7, l8 };

            double distancePercent = 0.1;

            // input: list các liên kết kho - khu vực, trọng số của chi phí khoảng cách (<1), bool check 1-1
            Console.WriteLine("\n___ 1 - n ____\n");
            List<ResultLine> resultLines = SupplyDemandSolveMinCost(listLinkCase, distancePercent, false);
            PrintResult(resultLines);

            Console.WriteLine("\n___ 1 - 1 ____\n");
            List<ResultLine> resultLines2 = SupplyDemandSolveMinCost(listLinkCase, distancePercent, true);
            PrintResult(resultLines2);

            Console.WriteLine("\n__end");
            Console.ReadKey();
        }
        //############################################################
        // Siêu thị - siêu thị: cân bằng hàng tồn                 ####
        //############################################################
        private static void StoreSolveMinCost(List<Store> listStore)
        {
            var watch = System.Diagnostics.Stopwatch.StartNew();

            // 1. Define Data // Số node ~ cửa hàng
            int numNodes = listStore.Count();
            Console.WriteLine("Số cửa hàng: " + numNodes);

            // Số liên kết giữa các node (tối đa)
            int numArcs = numNodes * numNodes - numNodes;
            Console.WriteLine("Tổng số link: " + numArcs);

            // startNode vận chuyển đến endNode
            List<int> startNodes = new List<int> { };
            List<int> endNodes = new List<int> { };

            // chi phí vận chuyển (từ startNode -> endNode tốn chi phí là unitCosts)
            List<int> unitCosts = new List<int> { };

            // 2. Declare the solver and Loop to add data
            MinCostFlow minCostFlow = new MinCostFlow(numNodes, numArcs);
            for (int i = 0; i < numNodes; ++i)
            {
                // Add node supplies.
                minCostFlow.SetNodeSupply(i, listStore[i].Supplies);

                for (int j = 0; j < numNodes; ++j)
                {
                    if (i == j) continue; // bỏ qua chính nó ()
                    else
                    {
                        startNodes.Add(i);
                        endNodes.Add(j);
                        unitCosts.Add(GetDistance(listStore[i].LatY, listStore[i].LongX,
                            listStore[j].LatY, listStore[j].LongX));
                    }
                }
            }
            // Add each arc.
            for (int i = 0; i < numArcs; ++i)
            {
                // mặc định capacities[i] = 99999 || lưu lượng (số lượng xe, đường kẹt xe?) 
                // => mặc định max (vận chuyển bao nhiêu cũng được) (tạm thời)
                int arc = minCostFlow.AddArcWithCapacityAndUnitCost(
                    startNodes[i], endNodes[i], 99999999999, unitCosts[i]);
                if (arc != i) throw new Exception("Internal error");
            }

            // 3. Invoke the solver and display the results
            // Find the min cost flow.
            int solveStatus = (int)minCostFlow.SolveMaxFlowWithMinCost();
            if (solveStatus == 1)
            {
                long optimalCost = minCostFlow.OptimalCost();
                Console.WriteLine("\nTổng chi phí tối ưu: " + optimalCost);
                long maximumFlow = minCostFlow.MaximumFlow();
                Console.WriteLine("\nTổng lượng hàng hóa vận chuyển: " + maximumFlow);

                Console.WriteLine(String.Format("{0}\t\t\t{1}\t\t{2}\t{3}\t{4}",
                    "\nFrom", "To", "UnitCost", "Flow", "Cost"));
                for (int i = 0; i < numArcs; ++i)
                {
                    long cost = minCostFlow.Flow(i) * minCostFlow.UnitCost(i);
                    if (minCostFlow.Flow(i) != 0)
                    {
                        listStore[minCostFlow.Tail(i)].Supplies -= (int)minCostFlow.Flow(i);
                        listStore[minCostFlow.Head(i)].Supplies += (int)minCostFlow.Flow(i);
                        Console.WriteLine(String.Format("{0}\t{1}\t{2}\t{3}\t\t{4}\t{5}",
                            listStore[minCostFlow.Tail(i)].Name,
                            " -> ",
                            listStore[minCostFlow.Head(i)].Name,
                            minCostFlow.UnitCost(i),
                            minCostFlow.Flow(i),
                            cost));
                    }
                }
                Console.WriteLine("");
                for (int i = 0; i < numNodes; ++i)
                {
                    if (listStore[i].Supplies == 0) continue;
                    else if (listStore[i].Supplies > 0)
                    {
                        Console.WriteLine("'" + listStore[i].Name + "' dư hàng. Số lượng: " + listStore[i].Supplies);
                    }
                    else
                    {
                        Console.WriteLine("'" + listStore[i].Name + "' thiếu hàng, cần mua thêm. Số lượng: " + (-1) * listStore[i].Supplies);
                    }
                }
            }
            else
            {
                Console.WriteLine("Solving the min cost flow problem failed. "
                    + "Solver status: " + solveStatus);
            }

            watch.Stop();
            var elapsedMs = watch.ElapsedMilliseconds;
            Console.WriteLine("\nTime exec: " + elapsedMs + " (Milliseconds)");
        }
        public static int GetDistance(double LatA, double LongA, double LatB, double LongB)
        {
            var sCoord = new GeoCoordinate(LatA, LongA);
            var eCoord = new GeoCoordinate(LatB, LongB);
            return (int)(sCoord.GetDistanceTo(eCoord) / 1000);
        }
        //############################################################
        // Kho - Khu vực: chia hàng từ kho                        ####
        //############################################################
        private static List<ResultLine> SupplyDemandSolveMinCost(List<LinkCase> listLinkCase, double distancePercent, bool is11)
        {
            if (distancePercent > 1)
            {
                Console.WriteLine("Trọng số chi phí khoảng cách không được phép > 1");
                return null;
            }
            else
            {
                List<int> NodeIds = new List<int> { };
                List<int> NodeSupplies = new List<int> { };
                // độ lệch số lượng hàng (phần trăm)
                List<double> subNum = new List<double> { };
                // chi phí chuyển hàng (tính trên 1 đơn vị hàng)
                List<int> unitCosts = new List<int> { };
                // output
                List<ResultLine> resultLines = new List<ResultLine> { };

                // Số liên kết
                int numArcs = listLinkCase.Count();

                // tính max cost -> khởi tạo temp ResultLine
                int maxDistance = 0;
                int maxSupply = 0;
                for (int i = 0; i < numArcs; ++i)
                {
                    if (listLinkCase[i].Distance > maxDistance)
                    {
                        maxDistance = listLinkCase[i].Distance;
                    }
                    if (listLinkCase[i].NumSupply > maxSupply)
                    {
                        maxSupply = listLinkCase[i].NumSupply;
                    }
                }
                int maxCost = maxDistance * maxSupply;

                for (int i = 0; i < numArcs; ++i)
                {
                    // tính các chi phí, nhân trọng số -> unitCost
                    // tính sub theo % độ lệch
                    subNum.Add(Math.Abs((double)(listLinkCase[i].NumSupply - listLinkCase[i].NumDemand)
                        / Math.Min(listLinkCase[i].NumSupply, listLinkCase[i].NumDemand) * 100));

                    //Console.WriteLine("sub " + listLinkCase[i].IdSupply + " - " + listLinkCase[i].IdDemand + ": " + subNum[i]); // test

                    unitCosts.Add((int)(distancePercent * listLinkCase[i].Distance + (1 - distancePercent) * subNum[i]));
                    // lấy danh sách startNode (kho / bên cung cấp)
                    if (!NodeIds.Contains(listLinkCase[i].IdSupply))
                    {
                        NodeIds.Add(listLinkCase[i].IdSupply);
                        NodeSupplies.Add(listLinkCase[i].NumSupply);
                        // khởi tạo mặc định 1 luồng vận chuyển từ mỗi startNode() (để so sánh sau này)
                        if (is11)
                        {
                            ResultLine result = new ResultLine(
                                    listLinkCase[i].IdSupply,   // From
                                    listLinkCase[i].IdDemand,   // To
                                    0,                          // Flow
                                    listLinkCase[i].Distance,
                                    maxCost); // max cost
                            resultLines.Add(result);
                        }
                    }
                }
                // lấy danh sách endNode (khu vực / bên có nhu cầu)
                for (int i = 0; i < numArcs; ++i)
                {
                    if (!NodeIds.Contains(listLinkCase[i].IdDemand))
                    {
                        NodeIds.Add(listLinkCase[i].IdDemand);
                        NodeSupplies.Add((-1) * listLinkCase[i].NumDemand);
                    }
                }

                // Số node ~ (kho + khu vực)
                int numNodes = NodeIds.Count();

                // 2. Declare the solver and Loop to add data
                MinCostFlow minCostFlow = new MinCostFlow(numNodes, numArcs);
                for (int i = 0; i < numNodes; ++i)
                {
                    minCostFlow.SetNodeSupply(i, NodeSupplies[i]);
                }
                // Add each arc.
                for (int i = 0; i < numArcs; ++i)
                {
                    int arc = minCostFlow.AddArcWithCapacityAndUnitCost(
                        NodeIds.IndexOf(listLinkCase[i].IdSupply),
                        NodeIds.IndexOf(listLinkCase[i].IdDemand),
                        999999999999,    //(băng thông) capacities[i] -> max
                        unitCosts[i]);
                    if (arc != i) throw new Exception("Internal error");
                }
                // 3. Invoke the solver - Find the min cost flow.
                int solveStatus = (int)minCostFlow.SolveMaxFlowWithMinCost();
                if (solveStatus == 1)
                {
                    for (int i = 0; i < numArcs; ++i)
                    {
                        if (minCostFlow.Flow(i) != 0)
                        {
                            int realCost = (int)(minCostFlow.Flow(i) * listLinkCase[i].Distance);
                            ResultLine result = new ResultLine(
                                        NodeIds[minCostFlow.Tail(i)],
                                        NodeIds[minCostFlow.Head(i)],
                                        (int)minCostFlow.Flow(i),
                                        listLinkCase[i].Distance,
                                        realCost);
                            if (!is11)
                            {
                                resultLines.Add(result);
                                NodeSupplies[minCostFlow.Tail(i)] -= (int)minCostFlow.Flow(i);
                                NodeSupplies[minCostFlow.Head(i)] += (int)minCostFlow.Flow(i);
                            }
                            else // is11 is true
                            {
                                for (int index = 0; index < resultLines.Count(); ++index)
                                {
                                    // chọn lượt vận chuyển có cost min
                                    if (NodeIds[minCostFlow.Tail(i)] == resultLines[index].From
                                        && realCost <= resultLines[index].RealCost)
                                    {
                                        // -> cập nhật lại result tương ứng
                                        resultLines[index] = result;
                                        NodeSupplies[minCostFlow.Tail(i)] -= (int)minCostFlow.Flow(i);
                                        NodeSupplies[minCostFlow.Head(i)] += (int)minCostFlow.Flow(i);
                                    }
                                }
                            }
                        }
                    }
                    // Tính lại lượng hàng thừa thiếu sau chia
                    if (resultLines.Count() != 0)
                    {
                        // đẩy hàng từ kho dư -> khu vực thiếu
                        if (is11)
                        {
                            for (int i = 0; i < numNodes; ++i)
                            {
                                // kho dư hàng
                                if (NodeSupplies[i] > 0)
                                {
                                    for (int index = 0; index < resultLines.Count(); ++index)
                                    {
                                        // Kho dư hàng & khu vực thiếu hàng -> tiến hành chuyển hàng
                                        if (NodeIds[i] == resultLines[index].From
                                            && NodeSupplies[NodeIds.IndexOf(resultLines[index].To)] < 0)
                                        {
                                            int numFrom = NodeSupplies[i]; // hàng thừa
                                            int numTo = NodeSupplies[NodeIds.IndexOf(resultLines[index].To)]; // hàng thiếu 
                                            int addFlow = numFrom + numTo;
                                            // cập nhật lại thông tin chuyển hàng, thừa, thiếu sau cùng
                                            if (addFlow < 0)
                                            {
                                                resultLines[index].FLow += numFrom;
                                                resultLines[index].RealCost = resultLines[index].FLow * resultLines[index].Distance;
                                                NodeSupplies[i] = 0;
                                                NodeSupplies[NodeIds.IndexOf(resultLines[index].To)] += numFrom;
                                            }
                                            else
                                            {
                                                resultLines[index].FLow += (-1) * numTo;
                                                resultLines[index].RealCost = resultLines[index].FLow * resultLines[index].Distance;
                                                NodeSupplies[i] += numTo;
                                                NodeSupplies[NodeIds.IndexOf(resultLines[index].To)] = 0;
                                            }
                                        }
                                    }
                                }
                            }
                            // loại bỏ những temp resultLine khởi tạo nhưng chưa được cập nhật
                            for (int i = 0; i < resultLines.Count(); ++i)
                            {
                                if (resultLines[i].FLow == 0)
                                {
                                    resultLines.Remove(resultLines[i]);
                                }
                            }
                        }
                        int totalCost = 0;
                        // tính lại optimalCost
                        for (int i = 0; i < resultLines.Count(); ++i)
                        {
                            totalCost += resultLines[i].RealCost;
                        }
                        // in optimalCost + lượng thừa, thiếu
                        for (int i = 0; i < numNodes; ++i)
                        {
                            if (NodeSupplies[i] == 0) continue;
                            else if (NodeSupplies[i] > 0)
                            {
                                Console.WriteLine("'" + NodeIds[i] + "' dư hàng. Số lượng: " + NodeSupplies[i]);
                            }
                            else
                            {
                                Console.WriteLine("'" + NodeIds[i] + "' thiếu hàng, cần mua thêm. Số lượng: " + (-1) * NodeSupplies[i]);
                            }
                        }
                        Console.WriteLine("\nTổng chi phí chuyển hàng: " + totalCost + "\n");
                    }
                    return resultLines;
                }
                else
                {
                    Console.WriteLine("Solving the min cost flow problem failed. " + "Solver status: " + solveStatus);
                    return null;
                }
            }
        }
        public static void PrintResult(List<ResultLine> resultLines)
        {
            if (resultLines != null && resultLines.Count() != 0)
            {
                Console.WriteLine(String.Format("{0}\t\t{1}\t{2}\t{3}\t{4}",
                "\nFrom", "To", "Flow", "Distance", "RealCost"));
                for (int i = 0; i < resultLines.Count(); ++i)
                {
                    Console.WriteLine(String.Format("{0}\t{1}\t{2}\t{3}\t{4}\t\t{5}",
                               resultLines[i].From,
                                " -> ",
                                resultLines[i].To,
                                resultLines[i].FLow,
                                resultLines[i].Distance,
                                resultLines[i].RealCost));
                }
            }
        }
    }
}