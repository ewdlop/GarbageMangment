using System;
using System.Runtime;

namespace GarbageCollectionDemo
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("=== .NET Garbage Collection Demonstration ===\n");

            if (args.Length > 0 && args[0] == "--advanced")
            {
                RunAdvancedDemos();
            }
            else
            {
                RunBasicDemos();
                Console.WriteLine("\n💡 Tip: Run with --advanced flag to see more advanced GC demonstrations");
                Console.WriteLine("   Example: dotnet run -- --advanced");
            }

            Console.WriteLine("\n=== Demo Complete ===");
        }

        static void RunBasicDemos()
        {
            // Display GC settings
            DisplayGCSettings();

            // Demonstrate generations
            DemonstrateGenerations();

            // Demonstrate Large Object Heap
            DemonstrateLargeObjectHeap();

            // Demonstrate GC collection levels
            DemonstrateCollectionLevels();
        }

        static void RunAdvancedDemos()
        {
            Console.WriteLine("🚀 Running Advanced GC Demonstrations\n");

            // Basic demos first
            DisplayGCSettings();
            DemonstrateGenerations();

            // Advanced demos
            AdvancedGCDemo.DemonstrateMemoryPressure();
            AdvancedGCDemo.DemonstrateLatencyModes();
            AdvancedGCDemo.DemonstrateWeakReferences();
            AdvancedGCDemo.DemonstrateFinalization();
        }

        static void DisplayGCSettings()
        {
            Console.WriteLine("--- GC Settings ---");
            Console.WriteLine($"Is Server GC: {GCSettings.IsServerGC}");
            Console.WriteLine($"Latency Mode: {GCSettings.LatencyMode}");
            Console.WriteLine($"LOH Compaction Mode: {GCSettings.LargeObjectHeapCompactionMode}");
            Console.WriteLine($"Max Generation: {GC.MaxGeneration}");
            Console.WriteLine();
        }

        static void DemonstrateGenerations()
        {
            Console.WriteLine("--- Generation Demonstration ---");

            // Create a new object (will be in Gen 0)
            var youngObject = new byte[1000];
            Console.WriteLine($"New object generation: {GC.GetGeneration(youngObject)} (Gen 0 - 年輕代)");

            // Force Gen 0 collection - object survives and moves to Gen 1
            GC.Collect(0);
            GC.WaitForPendingFinalizers();
            Console.WriteLine($"After Gen 0 GC: {GC.GetGeneration(youngObject)} (Gen 1 - 中生代)");

            // Force Gen 1 collection - object survives and moves to Gen 2
            GC.Collect(1);
            GC.WaitForPendingFinalizers();
            Console.WriteLine($"After Gen 1 GC: {GC.GetGeneration(youngObject)} (Gen 2 - 老年代)");

            // Display collection counts
            Console.WriteLine($"\nGen 0 Collections: {GC.CollectionCount(0)}");
            Console.WriteLine($"Gen 1 Collections: {GC.CollectionCount(1)}");
            Console.WriteLine($"Gen 2 Collections: {GC.CollectionCount(2)}");
            Console.WriteLine();
        }

        static void DemonstrateLargeObjectHeap()
        {
            Console.WriteLine("--- Large Object Heap (LOH) Demonstration ---");

            // Small object (< 85,000 bytes) - goes to regular heap
            var smallObject = new byte[80_000];
            Console.WriteLine($"Small object (80KB) generation: {GC.GetGeneration(smallObject)}");

            // Large object (≥ 85,000 bytes) - goes to LOH (Gen 2)
            var largeObject = new byte[90_000];
            Console.WriteLine($"Large object (90KB) generation: {GC.GetGeneration(largeObject)} (LOH - Large Object Heap)");

            Console.WriteLine("\n⚠️ Large objects (≥85,000 bytes) are allocated directly to Gen 2 (LOH)");
            Console.WriteLine("   LOH is not compacted by default (可在 .NET 4.5+ 選擇壓縮)");
            Console.WriteLine();
        }

        static void DemonstrateCollectionLevels()
        {
            Console.WriteLine("--- GC Collection Levels ---");

            var gen0Before = GC.CollectionCount(0);
            var gen1Before = GC.CollectionCount(1);
            var gen2Before = GC.CollectionCount(2);

            // GC.Collect(0) - Only Gen 0
            Console.WriteLine("\nCalling GC.Collect(0) - 僅收集 Generation 0");
            GC.Collect(0);
            GC.WaitForPendingFinalizers();
            Console.WriteLine($"  Gen 0: {GC.CollectionCount(0) - gen0Before} collection(s)");
            Console.WriteLine($"  Gen 1: {GC.CollectionCount(1) - gen1Before} collection(s)");
            Console.WriteLine($"  Gen 2: {GC.CollectionCount(2) - gen2Before} collection(s)");

            gen0Before = GC.CollectionCount(0);
            gen1Before = GC.CollectionCount(1);
            gen2Before = GC.CollectionCount(2);

            // GC.Collect(1) - Gen 0 and Gen 1
            Console.WriteLine("\nCalling GC.Collect(1) - 收集 Generation 0 與 1");
            GC.Collect(1);
            GC.WaitForPendingFinalizers();
            Console.WriteLine($"  Gen 0: {GC.CollectionCount(0) - gen0Before} collection(s)");
            Console.WriteLine($"  Gen 1: {GC.CollectionCount(1) - gen1Before} collection(s)");
            Console.WriteLine($"  Gen 2: {GC.CollectionCount(2) - gen2Before} collection(s)");

            gen0Before = GC.CollectionCount(0);
            gen1Before = GC.CollectionCount(1);
            gen2Before = GC.CollectionCount(2);

            // GC.Collect(2) or GC.Collect() - All generations including LOH
            Console.WriteLine("\nCalling GC.Collect(2) - 收集全部（包含 LOH）");
            GC.Collect(2);
            GC.WaitForPendingFinalizers();
            Console.WriteLine($"  Gen 0: {GC.CollectionCount(0) - gen0Before} collection(s)");
            Console.WriteLine($"  Gen 1: {GC.CollectionCount(1) - gen1Before} collection(s)");
            Console.WriteLine($"  Gen 2: {GC.CollectionCount(2) - gen2Before} collection(s)");

            Console.WriteLine("\n⚠️ 一般建議不要手動呼叫 GC.Collect()，讓 CLR 自行判斷最佳時機即可。");
            Console.WriteLine();
        }
    }
}
