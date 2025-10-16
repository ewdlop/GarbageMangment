using System;
using System.Runtime;

namespace GarbageCollectionDemo
{
    /// <summary>
    /// Advanced GC concepts and scenarios demonstration
    /// 進階 GC 概念與情境示範
    /// </summary>
    public class AdvancedGCDemo
    {
        /// <summary>
        /// Demonstrates memory pressure and GC behavior under load
        /// 示範記憶體壓力與 GC 在負載下的行為
        /// </summary>
        public static void DemonstrateMemoryPressure()
        {
            Console.WriteLine("--- Memory Pressure Demonstration ---");
            
            long memoryBefore = GC.GetTotalMemory(false);
            Console.WriteLine($"Memory before allocation: {memoryBefore:N0} bytes");

            // Allocate many small objects (Gen 0 stress)
            var objects = new object[10000];
            for (int i = 0; i < objects.Length; i++)
            {
                objects[i] = new byte[1000];
            }

            long memoryAfter = GC.GetTotalMemory(false);
            Console.WriteLine($"Memory after allocation: {memoryAfter:N0} bytes");
            Console.WriteLine($"Memory increase: {(memoryAfter - memoryBefore):N0} bytes");

            // Check GC activity
            Console.WriteLine($"Gen 0 Collections: {GC.CollectionCount(0)}");
            Console.WriteLine($"Gen 1 Collections: {GC.CollectionCount(1)}");
            Console.WriteLine($"Gen 2 Collections: {GC.CollectionCount(2)}");
            Console.WriteLine();
        }

        /// <summary>
        /// Demonstrates GC latency modes
        /// 示範 GC 延遲模式
        /// </summary>
        public static void DemonstrateLatencyModes()
        {
            Console.WriteLine("--- GC Latency Modes ---");
            
            Console.WriteLine($"Current Latency Mode: {GCSettings.LatencyMode}");
            Console.WriteLine("\nAvailable Latency Modes:");
            Console.WriteLine("  - Batch: 批次模式，優先吞吐量");
            Console.WriteLine("  - Interactive: 互動模式（預設），平衡效能與回應");
            Console.WriteLine("  - LowLatency: 低延遲模式，短期使用");
            Console.WriteLine("  - SustainedLowLatency: 持續低延遲模式");
            Console.WriteLine("  - NoGCRegion: 無 GC 區域（需明確設定）");

            // Temporarily change to LowLatency mode
            var oldMode = GCSettings.LatencyMode;
            try
            {
                GCSettings.LatencyMode = GCLatencyMode.LowLatency;
                Console.WriteLine($"\n✓ Changed to: {GCSettings.LatencyMode}");
                Console.WriteLine("  ⚠️ LowLatency mode should only be used for short periods");
            }
            finally
            {
                GCSettings.LatencyMode = oldMode;
                Console.WriteLine($"✓ Restored to: {GCSettings.LatencyMode}");
            }
            Console.WriteLine();
        }

        /// <summary>
        /// Demonstrates WeakReference for cache scenarios
        /// 示範 WeakReference 用於快取情境
        /// </summary>
        public static void DemonstrateWeakReferences()
        {
            Console.WriteLine("--- WeakReference Demonstration ---");
            
            // Strong reference - object won't be collected
            var strongRef = new byte[1000];
            Console.WriteLine($"Strong reference generation: {GC.GetGeneration(strongRef)}");

            // Weak reference - object can be collected
            var weakRef = new WeakReference(new byte[1000]);
            Console.WriteLine($"Weak reference target exists: {weakRef.IsAlive}");

            // Force collection
            GC.Collect();
            GC.WaitForPendingFinalizers();
            
            Console.WriteLine($"After GC, strong ref still exists: {strongRef != null}");
            Console.WriteLine($"After GC, weak ref target exists: {weakRef.IsAlive}");
            Console.WriteLine("\n✓ WeakReference 適用於快取，允許 GC 在需要時回收記憶體");
            Console.WriteLine();

            GC.KeepAlive(strongRef); // Prevent optimization
        }

        /// <summary>
        /// Demonstrates finalization and IDisposable pattern
        /// 示範終結器與 IDisposable 模式
        /// </summary>
        public static void DemonstrateFinalization()
        {
            Console.WriteLine("--- Finalization & IDisposable ---");
            
            Console.WriteLine("Creating disposable resource...");
            using (var resource = new DisposableResource())
            {
                Console.WriteLine("Using resource...");
            } // Dispose called here
            
            Console.WriteLine("\nCreating finalizable resource (without dispose)...");
            CreateFinalizableResource();
            
            Console.WriteLine("Forcing GC to run finalizers...");
            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();
            
            Console.WriteLine("✓ 使用 IDisposable 而非依賴 Finalizer 可獲得更好的效能");
            Console.WriteLine();
        }

        private static void CreateFinalizableResource()
        {
            var resource = new FinalizableResource();
            // Resource goes out of scope and becomes eligible for collection
        }
    }

    /// <summary>
    /// Example of proper IDisposable implementation
    /// 正確的 IDisposable 實作範例
    /// </summary>
    public class DisposableResource : IDisposable
    {
        private bool disposed = false;

        public DisposableResource()
        {
            Console.WriteLine("  → Resource acquired");
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this); // Important: prevent finalizer from running
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    Console.WriteLine("  → Resource disposed (managed cleanup)");
                }
                disposed = true;
            }
        }
    }

    /// <summary>
    /// Example of a finalizable resource (not recommended pattern)
    /// 可終結資源範例（不建議的模式）
    /// </summary>
    public class FinalizableResource
    {
        public FinalizableResource()
        {
            Console.WriteLine("  → Finalizable resource created");
        }

        ~FinalizableResource()
        {
            Console.WriteLine("  → Finalizer called (expensive and unpredictable)");
        }
    }
}
