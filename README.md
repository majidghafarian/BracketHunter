# BracketMiner

**BracketMiner** is a Visual Studio extension tool designed to extract all content found inside square brackets `[]` from `.cs` files in a Visual Studio solution. It recursively scans all project files — including nested items like `Form1.Designer.cs` — and generates a clean `.txt` output for each project.

---

## 🚀 Features

- 🔍 Scans all `.cs` files in all projects in the current solution
- 📂 Supports nested project items (e.g., Windows Forms Designer files)
- 🧠 Extracts only content inside `[ ... ]` using regex
- 📝 Saves extracted results in a `.txt` file per project in a specified folder

---

## 📁 Output Example

If a file contains:

```csharp
Console.WriteLine("[Hello]");
Console.WriteLine("[World]");
