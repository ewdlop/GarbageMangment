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

## 🔬 Krebs Cycle 隱喻

**Krebs Cycle（克雷布斯循環）**是細胞代謝中的關鍵循環過程，就像 GC 的世代循環一樣：

* **代謝循環** → 物件在世代間的循環
* **能量回收** → 記憶體的回收與重用
* **持續運作** → GC 的自動化管理

這個類比強調了 GC 作為一個持續、循環的記憶體管理系統的本質。

---

## 📄 授權

本專案為教育示範用途。
