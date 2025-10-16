# .NET GC vs Java HotSpot GC 比較

## 世代對比

### .NET CLR GC

| 世代 | 名稱 | 說明 |
|-----|------|------|
| Gen 0 | 年輕代 | 新建立的物件 |
| Gen 1 | 中生代 | 從 Gen 0 存活的物件 |
| Gen 2 | 老年代 | 長期存活的物件 |
| LOH | Large Object Heap | 大型物件（≥85,000 bytes） |

### Java HotSpot GC

| 世代 | 名稱 | 說明 |
|-----|------|------|
| Young Generation | 年輕代 | 新建立的物件 |
| ├─ Eden Space | 伊甸園空間 | 物件初始分配區域 |
| └─ Survivor Space | 存活區 | 包含 S0 和 S1 兩個區域 |
| Old Generation (Tenured) | 老年代 | 長期存活的物件 |
| ~~PermGen~~ / Metaspace | 永久代/元空間 | 類別元資料（Java 8+ 使用 Metaspace） |

---

## 主要差異

### 1. 世代數量

- **.NET**: 3 個主要世代 (0, 1, 2) + LOH
- **Java**: 2 個主要世代 (Young, Old) + Metaspace

### 2. 年輕代結構

- **.NET Gen 0**: 單一空間，所有新物件都在這裡
- **Java Young**: 細分為 Eden + 2 個 Survivor 空間

### 3. 物件晉升機制

**.NET:**
```
新物件 → Gen 0 → Gen 1 → Gen 2
```

**Java:**
```
新物件 → Eden → Survivor S0 ⇄ S1 → Old Generation
```

### 4. 大型物件處理

- **.NET**: LOH (Large Object Heap) - 85,000 bytes 以上
- **Java**: 直接分配到 Old Generation（具體閾值依 GC 實作而定）

### 5. 類別元資料

- **.NET**: 存放在 Gen 2 或特殊的 Loader Heap
- **Java**: 
  - Java 7 及之前: PermGen (Permanent Generation)
  - Java 8 及之後: Metaspace (使用原生記憶體)

---

## GC 演算法比較

### .NET GC 模式

| 模式 | 說明 |
|------|------|
| Workstation GC | 桌面應用，單執行緒或並行 |
| Server GC | 伺服器應用，多執行緒並行 |
| Concurrent GC | 背景執行以減少停頓 |

### Java HotSpot GC 收集器

| 收集器 | 說明 |
|--------|------|
| Serial GC | 單執行緒，適合小型應用 |
| Parallel GC | 多執行緒，追求吞吐量 |
| CMS (Concurrent Mark Sweep) | 並行標記清除，追求低延遲 |
| G1 GC (Garbage First) | 區域化收集，平衡吞吐量與延遲 |
| ZGC / Shenandoah | 新一代低延遲收集器 |

---

## 效能調整參數

### .NET

```xml
<!-- 在 .csproj 或 runtimeconfig.json 中設定 -->
<PropertyGroup>
  <ServerGarbageCollection>true</ServerGarbageCollection>
  <ConcurrentGarbageCollection>true</ConcurrentGarbageCollection>
  <RetainVMGarbageCollection>true</RetainVMGarbageCollection>
</PropertyGroup>
```

### Java

```bash
# 選擇 GC 收集器
-XX:+UseG1GC              # 使用 G1 GC
-XX:+UseParallelGC        # 使用 Parallel GC
-XX:+UseConcMarkSweepGC   # 使用 CMS

# 記憶體設定
-Xms512m                  # 初始堆大小
-Xmx2g                    # 最大堆大小
-XX:NewRatio=2            # Old/Young 比例
```

---

## 相似點

1. **世代假說 (Generational Hypothesis)**
   - 兩者都基於「大多數物件很快死亡」的假設
   - 使用世代劃分來優化效能

2. **標記-清除-壓縮 (Mark-Sweep-Compact)**
   - 兩者都使用類似的基礎演算法

3. **並行收集**
   - 都支援多執行緒並行收集

4. **背景收集**
   - 都支援在背景執行 GC 以減少停頓

---

## 使用建議

### 選擇 .NET 的情境
- Windows 生態系統整合
- 需要與 Microsoft 技術棧緊密整合
- 追求更簡單的 GC 調優體驗

### 選擇 Java 的情境
- 需要跨平台支援
- 需要更細粒度的 GC 控制
- 已有大量 Java 生態系統依賴

---

## 參考資料

### .NET
- [Fundamentals of Garbage Collection](https://learn.microsoft.com/en-us/dotnet/standard/garbage-collection/fundamentals)
- [Workstation and Server Garbage Collection](https://learn.microsoft.com/en-us/dotnet/standard/garbage-collection/workstation-server-gc)

### Java
- [Java Garbage Collection Basics](https://www.oracle.com/webfolder/technetwork/tutorials/obe/java/gc01/index.html)
- [Getting Started with the G1 Garbage Collector](https://www.oracle.com/technetwork/tutorials/tutorials-1876574.html)
- [HotSpot Virtual Machine Garbage Collection Tuning Guide](https://docs.oracle.com/en/java/javase/17/gctuning/)
