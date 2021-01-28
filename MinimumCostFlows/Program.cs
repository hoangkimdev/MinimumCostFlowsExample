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

            // input sample
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

            Store e7 = new Store("Cua hang 7", 10.426099, 107.619262, 9);
            Store e8 = new Store("Cua hang 8", 10.323099, 106.609162, 2);
            Store e9 = new Store("Cua hang 9", 10.228099, 107.609062, 5);
            Store e0 = new Store("Cua hang 0", 10.722099, 106.619962, -8);
            Store e11 = new Store("Cua hang 11", 10.129099, 107.139262, 24);
            Store e21 = new Store("Cua hang 21", 10.928099, 106.79212, -12);
            Store e31 = new Store("Cua hang 31", 10.327099, 107.119362, -30);
            Store e41 = new Store("Cua hang 41", 10.426099, 106.749962, 12);
            Store e51 = new Store("Cua hang 51", 10.525099, 107.359162, -21);
            Store e61 = new Store("Cua hang 61", 10.624099, 106.069462, 12);
            Store e71 = new Store("Cua hang 71", 10.726099, 107.219162, -19);
            Store e81 = new Store("Cua hang 81", 10.824099, 106.309162, 19);
            Store e91 = new Store("Cua hang 91", 10.223099, 107.109262, 35);
            Store e01 = new Store("Cua hang 01", 10.029099, 106.819462, 12);

            List<Store> listStore = new List<Store> { A, B, C, D, E, F, G, H, I, K, M, N, O, Q,
                /*e7, e8, e9, e0, e11, e21, e31, e41, e51, e61, e71, e81, e91, e01*/};

            SolveMinCostFlow(listStore);

            Console.ReadKey();
            Console.ReadKey();
            Console.ReadKey();
        }
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
        private static void SolveMinCostFlow(List<Store> listStore)
        {
            var watch = System.Diagnostics.Stopwatch.StartNew();

            // 1. Define Data // Số node ~ cửa hàng
            int numNodes = listStore.Count();
            Console.WriteLine("Số cửa hàng: " + numNodes);

            // Số liên kết giữa các node (tối đa)
            int numArcs = numNodes * numNodes - numNodes;
            Console.WriteLine("Tổng số link: " + numArcs);

            // lượng hàng tồn kho của cửa hàng
            List<int> supplies = new List<int> { };

            // startNode vận chuyển đến endNode
            List<int> startNodes = new List<int> { };
            List<int> endNodes = new List<int> { };

            // chi phí vận chuyển (từ startNode -> endNode tốn chi phí là unitCosts)
            List<int> unitCosts = new List<int> { };

            // kiểm tra đủ hàng / thiếu hàng trong lưu thông
            int numBalanced = 0;

            // 2. Declare the solver and Loop to add data
            MinCostFlow minCostFlow = new MinCostFlow(numNodes, numArcs);
            for (int i = 0; i < numNodes; ++i)
            {
                supplies.Add(listStore[i].Supplies);

                // Add node supplies.
                minCostFlow.SetNodeSupply(i, supplies[i]);

                numBalanced += listStore[i].Supplies;

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
                    startNodes[i], endNodes[i], 999999999999, unitCosts[i]);
                if (arc != i) throw new Exception("Internal error");
            }

            // 3. Invoke the solver and display the results
            // Find the min cost flow.
            int solveStatus = (int)minCostFlow.SolveMaxFlowWithMinCost();
            if (solveStatus == 1)
            {
                long optimalCost = minCostFlow.OptimalCost();
                Console.WriteLine("\nTổng chi phí tối ưu: " + optimalCost);
                Console.WriteLine(String.Format("{0}\t\t\t{1}\t\t{2}\t{3}\t{4}",
                    "\nFrom", "To", "UnitCost", "Flow", "Cost"));
                for (int i = 0; i < numArcs; ++i)
                {
                    long cost = minCostFlow.Flow(i) * minCostFlow.UnitCost(i);
                    if (cost > 0)
                    {
                        Console.WriteLine(String.Format("{0}\t{1}\t{2}\t{3}\t\t{4}\t{5}",
                            listStore[minCostFlow.Tail(i)].Name,
                            " -> ",
                            listStore[minCostFlow.Head(i)].Name,
                            minCostFlow.UnitCost(i),
                            minCostFlow.Flow(i),
                            cost));
                    }
                }
                if (numBalanced == 0) Console.WriteLine("\nĐã chia đủ hàng.");
                else if (numBalanced < 0) Console.WriteLine("\nThiếu hàng, số lượng: " + (-1) * numBalanced);
                else Console.WriteLine("\nThừa hàng, số lượng: " + numBalanced);
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
    }
}