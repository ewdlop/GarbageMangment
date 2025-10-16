using System;
using System.Runtime;

namespace GarbageCollectionDemo
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("=== .NET Garbage Collection Demonstration ===\n");

            // Display GC settings
            DisplayGCSettings();

            // Demonstrate generations
            DemonstrateGenerations();

            // Demonstrate Large Object Heap
            DemonstrateLargeObjectHeap();

            // Demonstrate GC collection levels
            DemonstrateCollectionLevels();

            // Demonstrate short attention memory span
            DemonstrateShortAttentionSpan();

            Console.WriteLine("\n=== Demo Complete ===");
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

        static void DemonstrateShortAttentionSpan()
        {
            Console.WriteLine("--- Short Attention Memory Span Demonstration ---");
            Console.WriteLine("📌 Demonstrating ephemeral objects (短期記憶) - objects with 'short attention span'\n");

            var gen0Before = GC.CollectionCount(0);
            var memoryBefore = GC.GetTotalMemory(false);

            Console.WriteLine($"Initial state:");
            Console.WriteLine($"  Memory allocated: {memoryBefore:N0} bytes");
            Console.WriteLine($"  Gen 0 collections: {gen0Before}");

            // Create many short-lived objects that quickly go out of scope
            // These simulate "short attention span" - quickly forgotten references
            Console.WriteLine("\n🧠 Creating 1000 short-lived objects (objects quickly lose our 'attention')...");
            for (int i = 0; i < 1000; i++)
            {
                // Each object is created and immediately becomes eligible for collection
                // when the loop iterates - simulating very short "attention span"
                var shortLivedObject = new byte[1000];
                // Object loses reference here - "attention span" ends
            }

            var memoryAfter = GC.GetTotalMemory(false);
            var gen0After = GC.CollectionCount(0);

            Console.WriteLine($"\nAfter creating short-lived objects:");
            Console.WriteLine($"  Memory allocated: {memoryAfter:N0} bytes");
            Console.WriteLine($"  Gen 0 collections: {gen0After}");
            Console.WriteLine($"  New Gen 0 collections: {gen0After - gen0Before} (automatic cleanup!)");

            // Now demonstrate the difference with long-lived references
            Console.WriteLine("\n🎯 Creating 1000 objects with longer 'attention span' (kept in array)...");
            gen0Before = GC.CollectionCount(0);
            memoryBefore = GC.GetTotalMemory(false);

            var longLivedObjects = new byte[1000][];
            for (int i = 0; i < 1000; i++)
            {
                // These objects are referenced by the array - we maintain "attention"
                longLivedObjects[i] = new byte[1000];
            }

            memoryAfter = GC.GetTotalMemory(false);
            gen0After = GC.CollectionCount(0);

            Console.WriteLine($"\nWith maintained references:");
            Console.WriteLine($"  Memory allocated: {memoryAfter:N0} bytes (higher - objects still referenced)");
            Console.WriteLine($"  Gen 0 collections: {gen0After}");
            Console.WriteLine($"  New Gen 0 collections: {gen0After - gen0Before}");
            Console.WriteLine($"  Objects generation: {GC.GetGeneration(longLivedObjects[0])}");

            // Release the "attention" - dereference
            Console.WriteLine("\n💭 Losing 'attention' - releasing references...");
            longLivedObjects = null; // "Forgetting" the objects

            GC.Collect(0);
            GC.WaitForPendingFinalizers();

            var memoryFinal = GC.GetTotalMemory(false);
            Console.WriteLine($"\nAfter losing 'attention' (releasing references) and GC:");
            Console.WriteLine($"  Memory allocated: {memoryFinal:N0} bytes (lower - objects collected)");

            Console.WriteLine("\n💡 Key Concept - 'Short Attention Memory Span':");
            Console.WriteLine("   • Short-lived objects = Quick 'forgetting' = Fast Gen 0 collection");
            Console.WriteLine("   • Long-lived references = Maintained 'attention' = Promotion to Gen 1/2");
            Console.WriteLine("   • Most objects have 'short attention span' (ephemeral) → Gen 0 optimized for this!");
            Console.WriteLine();
        }
    }
}
