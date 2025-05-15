# BracketMiner

**BracketMiner** is a Visual Studio extension tool designed to extract all content found inside square brackets `[]` from `.cs` files in a Visual Studio solution. It recursively scans all project files — including nested items like `Form1.Designer.cs` — and generates a clean `.txt` output for each project.

---
Note:
 When using BracketMiner, make sure to close all project items in Visual Studio. Before running BracketMiner, 
 also close all tabs, then go to Tools and click on BracketMiner.

## 🚀 Features

- 🔍 Scans all `.cs` files in all projects in the current solution
- 📂 Supports nested project items (e.g., Windows Forms Designer files)
- 🧠 Extracts only content inside `[ ... ]` using regex
- 📝 Saves extracted results in a `.txt` file per project in a specified folder
- After extracting the words, the repeated values will be removed.

---

## 📁 Output Example

If a file contains:

```csharp
Console.WriteLine("[Hello]");
Console.WriteLine("[World]");
