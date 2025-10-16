# Garbage Collection (GC) - Krebs Cycle 🔄

![80e62f42a351d2cc26f023b279779457](https://github.com/user-attachments/assets/b66bf2e1-b1ad-4abd-b1f0-014762f257b4)

在 .NET 或其他具備自動記憶體管理的語言中，**Garbage Collector（GC）層級**（或稱「世代」Generations）是為了提升效能而設計的機制。本專案以 **.NET CLR (Common Language Runtime)** 為例詳細說明與示範。

---

## 🧠 GC 的三個世代（Generations）

### **Generation 0（年輕代）**

* **特點**：存放新建立的物件。
* **清除頻率**：最頻繁。
* **設計理念**：大多數物件「很快死亡」（例如暫時性變數、短期緩存）。
* **觸發時機**：當記憶體不足時，GC 會先嘗試清除 Gen 0。
* **清除後**：
  * 若物件仍在使用，會被「升級」到 **Generation 1**。

---

### **Generation 1（中生代）**

* **特點**：存放從 Gen 0 存活下來的物件。
* **清除頻率**：比 Gen 0 少，但比 Gen 2 多。
* **設計理念**：有些物件比短期變數壽命更長，但不會長期存在。
* **清除後**：
  * 若物件仍然存活，則升級到 **Generation 2**。

---

### **Generation 2（老年代）**

* **特點**：存放長期存在的物件，例如：
  * 應用程式啟動時建立的單例
  * 大型緩存資料結構
* **清除頻率**：最少，因為通常存活率高。
* **清除時機**：只有在記憶體壓力很大時才觸發。

---

## 📦 Large Object Heap (LOH)

* **特別區域**：存放大於 **85,000 bytes** 的物件（例如大型陣列）。
* **不屬於 Gen 0~2**，但在 .NET 中視為 Gen 2 的一部分。
* **特性**：
  * 不會經常被壓縮（compacted）。
  * 在 .NET 4.5 之後可以選擇壓縮 LOH。

---

## 🔁 收集層級（GC Collection Levels）

| 方法              | 說明                    | 對應世代  |
| --------------- | --------------------- | ----- |
| `GC.Collect(0)` | 僅收集 Generation 0      | Gen 0 |
| `GC.Collect(1)` | 收集 Generation 0 與 1   | Gen 1 |
| `GC.Collect(2)` | 收集全部（包含 LOH）          | Gen 2 |
| `GC.Collect()`  | 預設等同於 `GC.Collect(2)` | 全部    |

> ⚠️ 一般建議 **不要手動呼叫 `GC.Collect()`**，讓 CLR 自行判斷最佳時機即可。

---

## ⚙️ GC 模式（補充）

除了世代外，.NET GC 也有「模式」設定：

| 模式                             | 說明                  |
| ------------------------------ | ------------------- |
| **Workstation GC**             | 適用於桌面應用程式，優先互動流暢度。  |
| **Server GC**                  | 適用於伺服器環境，平行化、提升吞吐量。 |
| **Concurrent / Background GC** | 在背景執行，以減少應用程式停頓時間。  |

---

## 🚀 執行示範程式

本專案包含一個完整的 .NET 示範程式，展示所有 GC 概念：

```bash
cd GarbageCollectionDemo
dotnet run
```

### 示範內容

1. **GC 設定顯示** - 顯示當前的 GC 模式和設定
2. **世代示範** - 展示物件如何在世代間晉升
3. **Large Object Heap** - 展示 LOH 的特殊行為
4. **收集層級** - 展示不同的 GC.Collect() 層級效果
5. **短期記憶示範** - 展示「短期注意力」(Short Attention Memory Span) 的物件生命週期
6. **Span<T> 與 Memory<T>** - 展示現代 .NET 記憶體高效型別

---

## 📁 專案結構

```
GarbageMangement/
├── README.md                           # 本文件
└── GarbageCollectionDemo/              # 示範程式
    ├── GarbageCollectionDemo.csproj    # 專案檔
    └── Program.cs                      # 主程式
```

---

## 📚 參考來源

* Microsoft Docs: [Fundamentals of Garbage Collection](https://learn.microsoft.com/en-us/dotnet/standard/garbage-collection/fundamentals)
* Jeffrey Richter, *CLR via C# (4th Edition)*, Chapter 21: Garbage Collection
* Maoni Stephens (Microsoft GC Team Blog): [GC Performance and Internals](https://devblogs.microsoft.com/dotnet/tag/gc/)

---

## 💭 短期記憶 (Short Attention Memory Span)

**「短期注意力」概念** 是指大多數物件在記憶體中的生命週期都很短暫，就像人類的短期記憶一樣。

### 核心特性

* **瞬時性物件（Ephemeral Objects）**：大部分物件在建立後很快就不再被使用
* **快速失去引用**：當變數離開作用域或被設為 null，物件即「被遺忘」
* **Gen 0 最佳化**：因為多數物件都有「短期注意力」，Gen 0 設計為快速且頻繁回收

### 實際應用

```csharp
// 短期注意力 - 物件立即失去引用
for (int i = 0; i < 1000; i++)
{
    var temp = new byte[1000];  // 建立
    // 離開迴圈 → 失去引用 → Gen 0 回收
}

// 長期注意力 - 物件持續被引用
var keepAlive = new byte[1000][];
for (int i = 0; i < 1000; i++)
{
    keepAlive[i] = new byte[1000];  // 維持引用
}
```

### 為什麼重要？

* 理解「短期注意力」有助於寫出更高效的程式碼
* 避免不必要的長期引用可以減少記憶體壓力
* Gen 0 的快速回收機制正是為了處理這些「健忘」的物件

---

## 🔬 Krebs Cycle 隱喻

**Krebs Cycle（克雷布斯循環）**是細胞代謝中的關鍵循環過程，就像 GC 的世代循環一樣：

* **代謝循環** → 物件在世代間的循環
* **能量回收** → 記憶體的回收與重用
* **持續運作** → GC 的自動化管理

這個類比強調了 GC 作為一個持續、循環的記憶體管理系統的本質。

---

## 🚀 Span<T> 與 Memory<T> - 現代記憶體管理

.NET Core 2.1+ 引入了 `Span<T>` 和 `Memory<T>`，這些是現代高效能記憶體管理的核心型別。

### **Span<T> - 堆疊型記憶體視圖**

* **特點**：
  * ref struct 型別，僅能存在於堆疊上
  * 零配置（zero-allocation）的記憶體切片
  * 可以指向堆疊記憶體、堆積記憶體或原生記憶體
  * 無法跨越 async/await 邊界

* **優勢**：
  * 完全無 GC 壓力（使用 stackalloc 時）
  * 接近原生指標的效能，但型別安全
  * 零成本抽象（zero-cost abstraction）

```csharp
// 堆疊配置 - 無堆積分配，無 GC 壓力
Span<int> stackSpan = stackalloc int[100];

// 切片 - 無配置
var slice = stackSpan.Slice(10, 20);
```

### **Memory<T> - 可儲存的記憶體視圖**

* **特點**：
  * 一般 struct 型別，可以儲存在堆積上
  * 支援 async/await
  * 可以轉換為 Span<T> 進行操作

* **用途**：
  * 需要儲存在欄位或屬性中時
  * 需要跨越 async 邊界時
  * 需要較長生命週期時

```csharp
// 可以儲存在欄位中
Memory<byte> buffer = new byte[1000];

// 轉換為 Span 進行操作
Span<byte> span = buffer.Span;
```

### **與傳統陣列的比較**

| 特性 | Array | Span<T> | Memory<T> |
|------|-------|---------|-----------|
| GC 壓力 | 是（堆積配置） | 否（stackalloc） | 是（若堆積配置） |
| 切片成本 | 高（新配置） | 零（視圖） | 零（視圖） |
| 跨越 async | 是 | **否** | 是 |
| 儲存在欄位 | 是 | **否** | 是 |
| 效能 | 良好 | **極佳** | 良好 |

### **何時使用**

* **Span<T>**：
  * 需要最高效能
  * 短期、局部處理
  * 可以使用 stackalloc

* **Memory<T>**：
  * 需要儲存或傳遞
  * async/await 方法中
  * 較長生命週期

* **Array**：
  * 需要可變大小
  * 需要實作 IEnumerable
  * 舊版 .NET Framework

---

## 📄 授權

本專案為教育示範用途。
