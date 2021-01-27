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

            Store A = new Store("TP Ho Chi Minh", 10.823099, 106.629662, 10);
            Store B = new Store("TP Di An", 10.896476, 106.752739, -13);
            Store C = new Store("Thu Dau Mot", 10.981680, 106.650490, -16);
            Store D = new Store("Bien Hoa", 10.957413, 106.842690, 20);
            Store E = new Store("Long An", 10.713440, 106.124840, 0);
            Store F = new Store("Ba Ria VT", 10.493348, 107.168194, -13);
            Store G = new Store("TX Long Khanh", 10.938091, 107.240153, -8);
            Store H = new Store("Trang Bang", 11.033403, 106.358219, 20);

            Store e1 = new Store("Cua hang 1", 10.523099, 107.639462, 15);
            Store e2 = new Store("Cua hang 2", 10.422099, 106.679262, 23);
            Store e3 = new Store("Cua hang 3", 10.328099, 107.619262, -23);
            Store e4 = new Store("Cua hang 4", 10.222099, 106.649162, 34);
            Store e5 = new Store("Cua hang 5", 10.121099, 107.659062, -24);
            Store e6 = new Store("Cua hang 6", 10.927099, 106.669062, -53);
            Store e7 = new Store("Cua hang 7", 10.426099, 107.619262, 19);
            Store e8 = new Store("Cua hang 8", 10.323099, 106.609162, 12);
            Store e9 = new Store("Cua hang 9", 10.228099, 107.609062, 5);
            Store e0 = new Store("Cua hang 0", 10.722099, 106.619962, -8);

            List<Store> listStore = new List<Store> { A, B, C, D, E, F, G, H,
                /*e1, e2, e3, e4, e5, e6, e7, e8, e9, e0*/ };

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
            if (IsBalanced(listStore))
            {
                // 1. Define Data
                // Số node ~ cửa hàng
                int numNodes = listStore.Count();
                Console.WriteLine("Số cửa hàng: " + numNodes);

                // Tổng số liên kết có thể giữa các node
                int numArcs = numNodes * numNodes - numNodes;
                Console.WriteLine("Số link: " + numArcs);

                // lượng hàng tồn kho của cửa hàng
                List<int> supplies = new List<int> { };

                // startNode vận chuyển đến endNode
                List<int> startNodes = new List<int> { };
                List<int> endNodes = new List<int> { };

                // chi phí vận chuyển (startNode -> endNode tốn chi phí là unitCosts)
                List<int> unitCosts = new List<int> { };

                // 2. Declare the solver and Loop to add data
                MinCostFlow minCostFlow = new MinCostFlow();
                for (int i = 0; i < numNodes; ++i)
                {
                    supplies.Add(listStore[i].Supplies);

                    // Add node supplies.
                    minCostFlow.SetNodeSupply(i, supplies[i]);

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
                    // => mặc định max (vận chuyển bao nhiêu cũng được)
                    int arc = minCostFlow.AddArcWithCapacityAndUnitCost(
                        startNodes[i], endNodes[i], 99999999999999, unitCosts[i]);
                    if (arc != i) throw new Exception("Internal error");
                }

                // 3. Invoke the solver and display the results
                // Find the min cost flow.
                int solveStatus = (int)minCostFlow.Solve();
                if (solveStatus == 1)
                {
                    long optimalCost = minCostFlow.OptimalCost();
                    Console.WriteLine("Minimum cost: " + optimalCost);
                    Console.WriteLine("");
                    Console.WriteLine(String.Format("{0}\t\t\t{1}\t\t{2}\t{3}\t{4}",
                        "From", "To", "UnitCost", "Flow", "Cost"));
                    long total7 = 0;    // ### test
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
                            if (minCostFlow.Tail(i) < 8) total7 += cost; // ### test
                        }
                    }
                    Console.WriteLine("");  // ### test
                    Console.WriteLine("Total cost (0-7): " + total7); // ### test
                }
                else
                {
                    Console.WriteLine("Solving the min cost flow problem failed. "
                        + "Solver status: " + solveStatus);
                }
            } //end check balanced
            watch.Stop();
            var elapsedMs = watch.ElapsedMilliseconds;
            Console.WriteLine("");
            Console.WriteLine("Time exec: " + elapsedMs + " (Milliseconds)");
        }
        public static bool IsBalanced(List<Store> listStore)
        {
            int numBalanced = 0;
            for (int i = 0; i < listStore.Count(); ++i)
            {
                numBalanced += listStore[i].Supplies;
            }

            if (numBalanced == 0)
            {
                return true;
            }
            else if (numBalanced < 0)
            {
                Console.WriteLine("Thiếu hàng, số lượng: " + (-1) * numBalanced);
                return false;
            }
            else
            {
                Console.WriteLine("Thừa hàng, số lượng: " + numBalanced);
                return false;
            }
        }
        public static int GetDistance(double LatA, double LongA, double LatB, double LongB)
        {
            var sCoord = new GeoCoordinate(LatA, LongA);
            var eCoord = new GeoCoordinate(LatB, LongB);
            return (int)(sCoord.GetDistanceTo(eCoord) / 1000);
        }
    }
}