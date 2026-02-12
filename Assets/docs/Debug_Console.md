# Debug Console

## Setup
- Script: `Assets/Scripts/Debug/DebugConsole.cs` (composant sur GameManager)
- Toggle: **F1** (clavier)
- Interface: Canvas UI avec TMP_InputField (PAS IMGUI)
- Fleche haut/bas pour historique des commandes

## Commandes

| Commande | Description |
|----------|-------------|
| `wave N` | Sauter a la vague N |
| `boss 1-5` | Boss direct (1=W10, 2=W20, 3=W30, 4=W40, 5=W50) |
| `dmg N` | Ajouter +N damage |
| `dur N` | Ajouter +N durability |
| `arr N` | Ajouter +N arrows |
| `coins N` | Ajouter N coins |
| `god` | Toggle god mode (invincibilite) |
| `kill` | Tuer tous les ennemis |
| `heal` | Soigner bow a fond |
| `arrows` | Recharger toutes les fleches |
| `reset` | Reset tous les upgrades |
| `status` | Voir etat actuel (wave, upgrades, HP, god) |
| `clear` | Vider la console |
| `help` | Afficher la liste des commandes |

## Notes techniques
- Utilise `Keyboard.current.f1Key.wasPressedThisFrame` (New Input System)
- IMGUI (OnGUI) ne recoit PAS les clics avec le New Input System
- `DebugPanel.cs` existe aussi mais n'est PAS utilise (IMGUI abandonne)
