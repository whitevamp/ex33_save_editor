# EX33 Save Editor

A powerful and evolving **save file editor for Clair Obscur: Expedition 33**, built in Python with a modern UI using [CustomTkinter](https://github.com/TomSchimansky/CustomTkinter).

---

### ⚠️ Alpha Notice

> **This is an early alpha release**  
> Features are still being implemented, and bugs are expected. Use at your own risk and always back up your saves.

---

## 🔧 What It Does

- 🔍 **Load and edit** decrypted `.json` save files.
- 💾 **Convert** `.sav` ↔ `.json` with the help of `uesave-rs`.
- 🧩 **Auto-maps** inventory, weapons, outfits, tints, haircuts, pictos, and more.
- 📁 **Creates backups** of `.sav` and `.json` files before saving.
- 📚 **Multi-level category filtering** (e.g., `Weapons > Lune`).
- ✅ **Search and highlight** by item name or save key.
- 📅 **Log generation** for items missing subcategories.
- 🧠 **Remembers your settings** with `config.yaml`.

---

## 🧷 Requirements

- Python 3.10 or later
- [uesave-rs](https://github.com/trumank/uesave-rs/releases) binary (place or link to `uesave.exe`)
- Python libraries from `requirements.txt`

---

## 🛆 Installation

1. Download or clone this repository.
2. Download `uesave.exe` from [uesave-rs releases](https://github.com/trumank/uesave-rs/releases).
3. Place `uesave.exe` in the project folder or configure path on first run.
4. Install Python dependencies:

```bash
pip install -r requirements.txt
```

5. Run the editor:

```bash
python ex33_save_editor.py
```

---

## 📁 Files

- `ex33_save_editor.py` — Main application script.
- `ex33_mapping_full.yaml` — Save key mappings.
- `pictos.txt` — Optional master item list.
- `Save_Backup/` — Auto-created backups of `.sav` and `.json` files.
- `config.yaml` — Auto-generated config storing preferences.
- `missing_subcategories.log` — Created if mappings are missing subcategories.

---

## ❌ Limitations

- Frame transparency and background blending is platform- and theme-dependent.
- Only supports inventory and related mappings for now.
- Requires decrypted `.json` files using `uesave-rs`.

---

## 📢 Contributing

Feel free to fork, submit pull requests, or suggest improvements via issues.
If you create new mappings or patches, open a PR to share them!

---

## 🚨 Disclaimer

This tool is fan-made and not affiliated with the developers or publishers of *Clair Obscur: Expedition 33*.
No game content is included.

---

## 💼 License

MIT License.

---

## 📆 Version

**Alpha v0.1**  
Future versions may include expanded item types, and full theme customization.
