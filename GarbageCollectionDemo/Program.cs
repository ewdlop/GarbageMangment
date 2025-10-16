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

            // Demonstrate Span<T> and Memory<T>
            DemonstrateSpan();

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

        static void DemonstrateSpan()
        {
            Console.WriteLine("--- Span<T> and Memory<T> Demonstration ---");
            Console.WriteLine("📌 Demonstrating modern .NET memory-efficient types\n");

            // Demonstrate stackalloc with Span<T> - no heap allocation, no GC pressure
            Console.WriteLine("🔹 Stack Allocation with Span<T> (stackalloc):");
            Console.WriteLine("   • Allocated on the stack, NOT the heap");
            Console.WriteLine("   • No GC pressure - automatically cleaned when method exits");
            Console.WriteLine("   • Cannot escape method scope\n");

            var gen0BeforeStack = GC.CollectionCount(0);
            var memoryBeforeStack = GC.GetTotalMemory(false);

            unsafe
            {
                // Stack allocation - no heap, no GC involvement
                Span<int> stackSpan = stackalloc int[1000];
                for (int i = 0; i < stackSpan.Length; i++)
                {
                    stackSpan[i] = i;
                }

                Console.WriteLine($"  Created Span<int> with {stackSpan.Length} elements on stack");
                Console.WriteLine($"  First element: {stackSpan[0]}, Last element: {stackSpan[^1]}");
            }

            var memoryAfterStack = GC.GetTotalMemory(false);
            var gen0AfterStack = GC.CollectionCount(0);

            Console.WriteLine($"  Memory change: {memoryAfterStack - memoryBeforeStack:N0} bytes (minimal/zero heap allocation)");
            Console.WriteLine($"  Gen 0 collections: {gen0AfterStack - gen0BeforeStack} (no GC triggered)");

            // Compare with heap allocation
            Console.WriteLine("\n🔹 Heap Allocation comparison (array):");
            var gen0BeforeHeap = GC.CollectionCount(0);
            var memoryBeforeHeap = GC.GetTotalMemory(false);

            var heapArray = new int[1000];
            for (int i = 0; i < heapArray.Length; i++)
            {
                heapArray[i] = i;
            }

            var memoryAfterHeap = GC.GetTotalMemory(false);
            var gen0AfterHeap = GC.CollectionCount(0);

            Console.WriteLine($"  Created int[] with {heapArray.Length} elements on heap");
            Console.WriteLine($"  Memory change: {memoryAfterHeap - memoryBeforeHeap:N0} bytes (heap allocation)");
            Console.WriteLine($"  Gen 0 collections: {gen0AfterHeap - gen0BeforeHeap}");

            // Demonstrate Span<T> slicing - no allocation
            Console.WriteLine("\n🔹 Span<T> Slicing (zero-allocation):");
            var sourceArray = new int[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 };
            Span<int> fullSpan = sourceArray;
            Span<int> slice = fullSpan.Slice(2, 5); // Elements [2..7]

            Console.WriteLine($"  Original array: [{string.Join(", ", sourceArray)}]");
            Console.WriteLine($"  Slice (index 2-6): [{string.Join(", ", slice.ToArray())}]");
            Console.WriteLine("  ✓ No memory allocation - just a view over existing memory");

            // Demonstrate Memory<T> - can be stored in fields, passed to async methods
            Console.WriteLine("\n🔹 Memory<T> vs Span<T>:");
            Console.WriteLine("  • Span<T>: Stack-only, very fast, cannot cross async boundaries");
            Console.WriteLine("  • Memory<T>: Can be stored in heap, supports async/await");

            Memory<int> memory = new int[100];
            Console.WriteLine($"  Created Memory<int> with {memory.Length} elements");
            
            // Convert to Span for manipulation
            Span<int> memorySpan = memory.Span;
            for (int i = 0; i < memorySpan.Length; i++)
            {
                memorySpan[i] = i * 2;
            }

            Console.WriteLine($"  First 5 elements: [{string.Join(", ", memory.Span.Slice(0, 5).ToArray())}]");

            // Demonstrate benefits
            Console.WriteLine("\n💡 Key Benefits of Span<T> and Memory<T>:");
            Console.WriteLine("   • Reduced GC pressure - stack allocation when possible");
            Console.WriteLine("   • Zero-copy slicing - no new allocations for sub-ranges");
            Console.WriteLine("   • Type-safe - compile-time checks, better than pointers");
            Console.WriteLine("   • Performance - nearly as fast as unsafe code, but safe");
            Console.WriteLine("   • Modern .NET idiom - used extensively in .NET Core/5+");
            Console.WriteLine();
        }
    }
}
