# GAME DESIGN DOCUMENT
## 💥 Monster Cannon

**Version :** 3.1  
**Dernière mise à jour :** Février 2026  
**Nom de code :** Cannon Strike

---

| Info | Détail |
|------|--------|
| **Genre** | Arcade Roguelike Hypercasual |
| **Plateforme** | Mobile (Android - Google Play) |
| **Moteur** | Unity |
| **Inspiration** | Angry Birds + Bowling + Billard + Roguelike |
| **Session dev** | 30 min/jour max |
| **Modèle économique** | Free-to-Play + Rewarded Ads Only |

---

# 🎯 SCOPE V1 vs V2+

## V1 - Google Play Launch (Must Have)

| Feature | Priorité | Status |
|---------|----------|--------|
| Vagues infinies (scaling) | 🔴 Critical | ⬜ |
| Tutoriel (7 étapes) | 🔴 Critical | ⬜ |
| 5 Boss (vague 10-50) | 🔴 Critical | ⬜ |
| 5 Mécaniques (Obstacles → Division) | 🔴 Critical | ⬜ |
| 10+ Upgrades | 🔴 Critical | ⬜ |
| Système Coins + Gems | 🔴 Critical | ⬜ |
| 5 Skins (Boss rewards) | 🟡 Important | ⬜ |
| Achievements (15 de base) | 🟡 Important | ⬜ |
| Daily Login (7 jours) | 🟡 Important | ⬜ |
| Rewarded Ads (Revive, x2 Coins) | 🟡 Important | ⬜ |
| Sauvegarde (PlayerPrefs) | 🔴 Critical | ⬜ |
| Audio (SFX + Music) | 🟡 Important | ⬜ |
| Settings (Son, Langue) | 🟢 Nice | ⬜ |

## V2+ - Post-Launch (Future Updates)

| Feature | Update |
|---------|--------|
| Boss 60-100 (Ghost → Overlord) | V1.1 |
| 5 Mécaniques avancées | V1.1 |
| 5 Skins supplémentaires | V1.1 |
| Leaderboard | V1.2 |
| Events saisonniers | V1.3 |
| Nouveaux types d'ennemis | V1.2 |
| Battle Pass | V2.0 |

---

# TABLE DES MATIÈRES

1. [Concept du Jeu](#1-concept-du-jeu)
2. [Tutoriel](#2-tutoriel)
3. [Boucle de Gameplay](#3-boucle-de-gameplay)
4. [Mécaniques de Gameplay](#4-mécaniques-de-gameplay)
5. [Entités du Jeu](#5-entités-du-jeu)
6. [Système de Vagues Infinies](#6-système-de-vagues-infinies)
7. [Système d'Upgrades](#7-système-dupgrades-roguelike)
8. [Économie & Progression](#8-économie--progression)
9. [Système de Skins](#9-système-de-skins)
10. [Achievements](#10-achievements)
11. [Daily Login](#11-daily-login)
12. [Monétisation](#12-monétisation-rewarded-ads-only)
13. [Interface Utilisateur](#13-interface-utilisateur)
14. [Audio](#14-audio)
15. [Roadmap V1](#15-roadmap-v1)
16. [Notes Techniques](#16-notes-techniques)

---

# 1. Concept du Jeu

Monster Cannon est un jeu arcade roguelike où le joueur utilise un **canon** pour lancer des **boulets** qui **rebondissent** sur les murs et traversent les monstres. L'objectif est de survivre le plus longtemps possible à travers des **vagues infinies** de difficulté croissante.

## Pitch en une phrase

> *"Vise, tire, fais rebondir ton boulet et élimine tous les monstres avant qu'ils ne détruisent ton canon !"*

## Ce qui rend le jeu unique

| Feature | Description |
|---------|-------------|
| **Rebonds stratégiques** | Murs font rebondir, ennemis sont traversés |
| **Vagues infinies** | Difficulté progressive sans fin |
| **Boss = Professeurs** | Chaque boss enseigne une nouvelle mécanique |
| **Mécaniques cumulatives** | Les mécaniques se mélangent après déblocage |
| **Progression roguelike** | Upgrades in-run (reset au game over) |
| **Méta-progression** | Skins, achievements (persistent) |

---

# 2. Tutoriel

## 2.1 Principes (V1)

- **Court** : < 60 secondes
- **Interactif** : Apprendre en jouant
- **Non-skippable** : Première fois seulement
- **Progressif** : Une mécanique à la fois

## 2.2 Les 7 Étapes avec Mockups

### Étape 1 : Viser (5 secondes)
```
┌─────────────────────────────────────────────┐
│                                             │
│    ☁️ "TOUCHE ET GLISSE POUR VISER"        │
│                                             │
│              👆                             │
│         (main animée)                       │
│                                             │
│                                             │
│              ══╦══  ← Canon surligné        │
│                ║                            │
│           ─────╩─────                       │
└─────────────────────────────────────────────┘
```
- **Action requise :** Touch + drag
- **Validation :** Trajectoire visible → Étape 2

### Étape 2 : Trajectoire (5 secondes)
```
┌─────────────────────────────────────────────┐
│                                             │
│    ☁️ "LA LIGNE MONTRE OÙ IRA              │
│         TON BOULET"                         │
│                                             │
│         · · · · · ·                         │
│                    · ← Trajectoire pulse    │
│                     ·                       │
│              ══╦══                          │
│                ║                            │
│           ─────╩─────                       │
└─────────────────────────────────────────────┘
```
- **Action requise :** Continuer à viser
- **Validation :** 2 secondes → Étape 3

### Étape 3 : Tirer (5 secondes)
```
┌─────────────────────────────────────────────┐
│                                             │
│    ☁️ "RELÂCHE POUR TIRER !"               │
│                                             │
│              🟢  ← Monstre cible            │
│                                             │
│         · · · · ·                           │
│              ══╦══                          │
│                ║                            │
│           ─────╩─────                       │
└─────────────────────────────────────────────┘
```
- **Action requise :** Relâcher le doigt
- **Validation :** Boulet tiré → Étape 4

### Étape 4 : Rebonds (5 secondes)
```
┌─────────────────────────────────────────────┐
│    ☁️ "LE BOULET REBONDIT SUR LES MURS !"  │
│                                             │
│  │                                       │  │
│  │  🟢              🟢                   │  │
│  │         ●→                            │  │
│  │              ←── Rebond surligné      │  │
│  │                                       │  │
│              ══╦══                          │
│                ║                            │
│           ─────╩─────                       │
└─────────────────────────────────────────────┘
```
- **Action requise :** Observer le rebond
- **Validation :** Rebond effectué → Étape 5

### Étape 5 : Objectif (5 secondes)
```
┌─────────────────────────────────────────────┐
│                                             │
│    ☁️ "ÉLIMINE TOUS LES MONSTRES           │
│         POUR PASSER À LA VAGUE SUIVANTE !" │
│                                             │
│         🟢    🔵    🟢                      │
│              🟢                             │
│                                             │
│  ❤️ 20    💎 0    🔫 3  ← UI expliquée     │
│              ══╦══                          │
│           ─────╩─────                       │
│                                             │
│            [ COMPRIS ! ]                    │
└─────────────────────────────────────────────┘
```
- **Explication rapide UI :**
  - ❤️ = Tes points de vie
  - 💎 = Gemmes (pour upgrades)
  - 🔫 = Boulets restants
- **Validation :** Tap bouton → Jeu commence

### Étape 6 : Contre-attaque (Après 1er dégât reçu)
```
┌─────────────────────────────────────────────┐
│                                             │
│    ☁️ "ATTENTION ! LES MONSTRES            │
│         SURVIVANTS T'ATTAQUENT !"          │
│                                             │
│              💥                             │
│         🟢 →→→ ══╦══                        │
│                  ║                          │
│             ─────╩─────                     │
│                                             │
│    ❤️ 19/20  ← Clignote rouge              │
│                                             │
└─────────────────────────────────────────────┘
```
- **Trigger :** Premier dégât reçu
- **Durée :** 3 secondes, auto-dismiss

### Étape 7 : Upgrades (Fin de vague 1)
```
┌─────────────────────────────────────────────┐
│                                             │
│    ☁️ "CHOISIS UN UPGRADE POUR             │
│         T'AMÉLIORER !"                      │
│                                             │
│         VAGUE 1 TERMINÉE !                  │
│            + 12 💎                          │
│                                             │
│  ┌─────────┐ ┌─────────┐ ┌─────────┐       │
│  │   ⚔️    │ │   📦    │ │   🔵    │       │
│  │ POINTE  │ │CARQUOIS │ │ BOULET  │       │
│  │ ACÉRÉE  │ │ ÉLARGI  │ │ ROBUSTE │ ← Highlight
│  │         │ │         │ │         │       │
│  │+1 dégât │ │+1 boulet│ │+1 trav. │       │
│  │  15 💎  │ │  12 💎  │ │  10 💎  │       │
│  └─────────┘ └─────────┘ └─────────┘       │
│                                             │
│            [ PASSER ]                       │
└─────────────────────────────────────────────┘
```
- **Action requise :** Choisir un upgrade OU passer
- **Validation :** Choix fait → Tutoriel terminé ✅

## 2.3 Textes Multilingues (V1 : FR + EN)

| Étape | Français | English |
|-------|----------|---------|
| 1 | "Touche et glisse pour viser" | "Touch and drag to aim" |
| 2 | "La ligne montre où ira ton boulet" | "The line shows where your ball will go" |
| 3 | "Relâche pour tirer !" | "Release to fire!" |
| 4 | "Le boulet rebondit sur les murs !" | "The ball bounces off walls!" |
| 5 | "Élimine tous les monstres pour passer à la vague suivante !" | "Destroy all monsters to advance!" |
| 6 | "Attention ! Les monstres survivants t'attaquent !" | "Watch out! Surviving monsters attack!" |
| 7 | "Choisis un upgrade pour t'améliorer !" | "Choose an upgrade to power up!" |

## 2.4 Règles du Tutoriel

| Règle | Détail |
|-------|--------|
| **Affichage** | Première partie uniquement |
| **Skip** | Non-skippable (première fois) |
| **Sauvegarde** | `PlayerPrefs.SetInt("TutorialDone", 1)` |
| **Rejouable** | Option dans Settings : "Revoir le tutoriel" |
| **Overlay** | Semi-transparent, zone active highlight |

## 2.5 Implémentation Complète

```csharp
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class TutorialManager : MonoBehaviour
{
    public static TutorialManager Instance;
    
    [Header("UI Elements")]
    public GameObject tutorialOverlay;
    public Text tutorialText;
    public Image handIcon;
    public Button continueButton;
    
    [Header("Localization")]
    public string[] textsFR;
    public string[] textsEN;
    
    public enum TutorialStep
    {
        None,
        Aim,           // Étape 1
        Trajectory,    // Étape 2
        Fire,          // Étape 3
        Bounce,        // Étape 4
        Objective,     // Étape 5
        CounterAttack, // Étape 6
        Upgrades,      // Étape 7
        Complete
    }
    
    public TutorialStep currentStep = TutorialStep.None;
    private bool isTutorialActive = false;
    
    void Awake()
    {
        Instance = this;
    }
    
    void Start()
    {
        if (PlayerPrefs.GetInt("TutorialDone", 0) == 0)
        {
            StartTutorial();
        }
    }
    
    public void StartTutorial()
    {
        isTutorialActive = true;
        currentStep = TutorialStep.Aim;
        ShowStep(currentStep);
    }
    
    public void RestartTutorial()
    {
        PlayerPrefs.SetInt("TutorialDone", 0);
        StartTutorial();
    }
    
    void ShowStep(TutorialStep step)
    {
        tutorialOverlay.SetActive(true);
        
        string[] texts = GetLocalizedTexts();
        int index = (int)step - 1;
        
        if (index >= 0 && index < texts.Length)
        {
            tutorialText.text = texts[index];
        }
        
        // Configure UI based on step
        switch (step)
        {
            case TutorialStep.Aim:
                handIcon.gameObject.SetActive(true);
                continueButton.gameObject.SetActive(false);
                StartCoroutine(AnimateHand());
                break;
                
            case TutorialStep.Trajectory:
                handIcon.gameObject.SetActive(false);
                continueButton.gameObject.SetActive(false);
                break;
                
            case TutorialStep.Fire:
                handIcon.gameObject.SetActive(false);
                continueButton.gameObject.SetActive(false);
                break;
                
            case TutorialStep.Bounce:
                handIcon.gameObject.SetActive(false);
                continueButton.gameObject.SetActive(false);
                break;
                
            case TutorialStep.Objective:
                handIcon.gameObject.SetActive(false);
                continueButton.gameObject.SetActive(true);
                break;
                
            case TutorialStep.CounterAttack:
                handIcon.gameObject.SetActive(false);
                continueButton.gameObject.SetActive(false);
                StartCoroutine(AutoDismiss(3f));
                break;
                
            case TutorialStep.Upgrades:
                handIcon.gameObject.SetActive(false);
                continueButton.gameObject.SetActive(false);
                break;
        }
    }
    
    string[] GetLocalizedTexts()
    {
        string lang = PlayerPrefs.GetString("Language", "FR");
        return lang == "EN" ? textsEN : textsFR;
    }
    
    IEnumerator AnimateHand()
    {
        while (currentStep == TutorialStep.Aim)
        {
            // Animate hand up and down
            handIcon.transform.localPosition += Vector3.up * 20f;
            yield return new WaitForSeconds(0.3f);
            handIcon.transform.localPosition -= Vector3.up * 20f;
            yield return new WaitForSeconds(0.3f);
        }
    }
    
    IEnumerator AutoDismiss(float delay)
    {
        yield return new WaitForSeconds(delay);
        AdvanceStep();
    }
    
    // === CALLED BY GAME EVENTS ===
    
    public void OnAimStarted()
    {
        if (currentStep == TutorialStep.Aim)
        {
            AdvanceStep();
        }
    }
    
    public void OnTrajectoryVisible()
    {
        if (currentStep == TutorialStep.Trajectory)
        {
            StartCoroutine(DelayedAdvance(2f));
        }
    }
    
    public void OnBulletFired()
    {
        if (currentStep == TutorialStep.Fire)
        {
            AdvanceStep();
        }
    }
    
    public void OnBulletBounced()
    {
        if (currentStep == TutorialStep.Bounce)
        {
            AdvanceStep();
        }
    }
    
    public void OnContinueButtonClicked()
    {
        if (currentStep == TutorialStep.Objective)
        {
            AdvanceStep();
            tutorialOverlay.SetActive(false); // Hide, wait for damage
        }
    }
    
    public void OnFirstDamageTaken()
    {
        if (currentStep == TutorialStep.CounterAttack || 
            currentStep == TutorialStep.Objective)
        {
            currentStep = TutorialStep.CounterAttack;
            ShowStep(currentStep);
        }
    }
    
    public void OnUpgradeScreenShown()
    {
        if (isTutorialActive && currentStep != TutorialStep.Complete)
        {
            currentStep = TutorialStep.Upgrades;
            ShowStep(currentStep);
        }
    }
    
    public void OnUpgradeSelected()
    {
        if (currentStep == TutorialStep.Upgrades)
        {
            CompleteTutorial();
        }
    }
    
    IEnumerator DelayedAdvance(float delay)
    {
        yield return new WaitForSeconds(delay);
        AdvanceStep();
    }
    
    void AdvanceStep()
    {
        currentStep++;
        
        if (currentStep == TutorialStep.Complete)
        {
            CompleteTutorial();
        }
        else
        {
            ShowStep(currentStep);
        }
    }
    
    void CompleteTutorial()
    {
        currentStep = TutorialStep.Complete;
        isTutorialActive = false;
        
        PlayerPrefs.SetInt("TutorialDone", 1);
        PlayerPrefs.Save();
        
        tutorialOverlay.SetActive(false);
        
        Debug.Log("Tutorial completed!");
    }
    
    public bool IsTutorialActive() => isTutorialActive;
    public bool IsTutorialComplete() => PlayerPrefs.GetInt("TutorialDone", 0) == 1;
}
```

## 2.6 Intégration avec le Gameplay

```csharp
// Dans CannonController.cs
public class CannonController : MonoBehaviour
{
    void OnTouchStart()
    {
        // ... code existant ...
        TutorialManager.Instance?.OnAimStarted();
    }
    
    void UpdateTrajectory()
    {
        // ... code existant ...
        TutorialManager.Instance?.OnTrajectoryVisible();
    }
    
    void Fire()
    {
        // ... code existant ...
        TutorialManager.Instance?.OnBulletFired();
    }
}

// Dans BulletController.cs
public class BulletController : MonoBehaviour
{
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Wall"))
        {
            TutorialManager.Instance?.OnBulletBounced();
        }
    }
}

// Dans CannonHealth.cs
public class CannonHealth : MonoBehaviour
{
    private bool firstDamageTaken = false;
    
    public void TakeDamage(int damage)
    {
        // ... code existant ...
        
        if (!firstDamageTaken)
        {
            firstDamageTaken = true;
            TutorialManager.Instance?.OnFirstDamageTaken();
        }
    }
}

// Dans UpgradeManager.cs
public class UpgradeManager : MonoBehaviour
{
    public void ShowUpgradeScreen()
    {
        // ... code existant ...
        TutorialManager.Instance?.OnUpgradeScreenShown();
    }
    
    public void SelectUpgrade(int index)
    {
        // ... code existant ...
        TutorialManager.Instance?.OnUpgradeSelected();
    }
}
```

---

# 3. Boucle de Gameplay

```
┌────────────────────────────────────────────────────────────┐
│                      MENU PRINCIPAL                         │
│                            │                                │
│     ┌──────────────────────┼──────────────────────┐        │
│     │                      │                      │        │
│  [SKINS]              [JOUER]            [ACHIEVEMENTS]    │
│     │                      │                      │        │
│     └──────────────────────┼──────────────────────┘        │
│                            ▼                                │
│  ┌─────────────────────────────────────────────────────┐   │
│  │                    GAMEPLAY                          │   │
│  │                                                      │   │
│  │   ┌────────────────────────────────────────────┐    │   │
│  │   │              BOUCLE VAGUE                  │    │   │
│  │   │                                            │    │   │
│  │   │  1. VISER (touch + trajectoire)            │    │   │
│  │   │  2. TIRER (boulet avec rebonds)            │    │   │
│  │   │  3. DÉGÂTS (boulet traverse monstres)      │    │   │
│  │   │  4. CONTRE-ATTAQUE (monstres → canon)      │    │   │
│  │   │  5. RÉPÉTER jusqu'à fin                    │    │   │
│  │   │                                            │    │   │
│  │   └────────────────────────────────────────────┘    │   │
│  │                         │                            │   │
│  │            ┌────────────┴────────────┐              │   │
│  │            │                         │              │   │
│  │      [VAGUE OK]               [HP = 0]              │   │
│  │            │                         │              │   │
│  │      UPGRADE SCREEN            GAME OVER            │   │
│  │            │                    │    │              │   │
│  │      VAGUE +1              [REVIVE] [FIN]           │   │
│  │                              (Ad)                    │   │
│  └─────────────────────────────────────────────────────┘   │
│                            │                                │
│                     RETOUR MENU                             │
│              (Coins ajoutés, Achievements check)            │
└────────────────────────────────────────────────────────────┘
```

---

# 4. Mécaniques de Gameplay

## 4.1 Contrôles

| Action | Input | Feedback |
|--------|-------|----------|
| Viser | Touch + drag | Trajectoire pointillée |
| Tirer | Relâcher | Boulet part + vibration |
| Pause | Bouton ⏸️ | Menu pause |

## 4.2 Physique du Boulet

| Surface | Effet |
|---------|-------|
| **Murs** | Rebond (angle inversé) |
| **Obstacles** | Rebond (angle inversé) |
| **Monstres** | Traverse + dégâts + durabilité -1 |

## 4.3 Trajectoire Prédictive

- Ligne pointillée pendant le touch
- Affiche **3 rebonds** maximum
- S'arrête au premier monstre
- Disparaît au tir

---

# 5. Entités du Jeu

## 5.1 Le Canon (Joueur)

| Stat | Valeur | Note |
|------|--------|------|
| HP | 20 | Restauré 100% fin de vague |
| Position | Fixe, bas de l'écran | - |
| Rotation | 0° - 180° | Suit le doigt |

## 5.2 Le Boulet

| Stat | Base | Upgradeable |
|------|------|-------------|
| Durabilité | 3 | ✅ +1 par upgrade |
| Dégâts | 1 | ✅ +1 par upgrade |
| Quantité/vague | 3 | ✅ +1 par upgrade |

## 5.3 Les Monstres (V1)

| Type | HP | Dégâts | Drop 💎 | Apparition |
|------|-----|--------|---------|------------|
| **Blob Vert** | 1 | 1 | 1-2 | Vague 1+ |
| **Blob Bleu** | 2 | 2 | 2-4 | Vague 1+ |
| **Blob Rouge** | 3 | 3 | 4-6 | Vague 15+ |
| **Blob Violet** | 5 | 4 | 8-12 | Vague 25+ |

---

# 6. Système de Vagues Infinies

## 6.1 Formules de Scaling

```csharp
int GetEnemyCount(int wave)
{
    return 3 + (wave * 2);  // Vague 1=5, Vague 10=23, Vague 50=103
}

float GetEnemyHPMultiplier(int wave)
{
    return 1f + (wave * 0.08f);  // Vague 1=1x, Vague 50=5x
}

float GetGemMultiplier(int wave)
{
    return 1f + (wave * 0.1f);  // Plus de gems = plus d'upgrades possibles
}
```

## 6.2 Boss & Mécaniques (Toutes les 10 vagues)

### 🎓 Concept : Boss = Professeur

Chaque boss **introduit une nouvelle mécanique**. Une fois battu, cette mécanique apparaît dans les vagues normales. Le joueur a **10 vagues** pour maîtriser avant la prochaine.

```
Vague 1-9:    Gameplay de base (pas de mécanique spéciale)
     ↓
Vague 10:     BOSS → Introduit OBSTACLES
     ↓
Vague 11-19:  Obstacles présents dans les vagues
     ↓
Vague 20:     BOSS → Introduit DÉPLACEMENT
     ↓
Vague 21-29:  Obstacles + Ennemis mobiles
     ↓
     ... Les mécaniques s'accumulent ...
```

### V1 - Les 5 Boss (Vague 10 → 50)

| Vague | Boss | HP | Pattern | Mécanique Introduite | Skin Reward |
|-------|------|-----|---------|---------------------|-------------|
| **10** | **Blob King** 👑 | 15 | Se cache derrière blocs | 🧱 **OBSTACLES** | 🎨 Bronze |
| **20** | **Speedy** ⚡ | 25 | Se déplace rapidement | 🏃 **DÉPLACEMENT** | 🎨 Silver |
| **30** | **Guardian** 🛡️ | 35 | Bouclier d'un côté | 🛡️ **BOUCLIER** | 🎨 Gold |
| **40** | **Splitter** 💥 | 45 | Se divise à 50% HP | ✂️ **DIVISION** | 🎨 Diamond |
| **50** | **Overlord** 💀 | 60 | Toutes mécaniques | 🔄 **CONSOLIDATION** | 🎨 Legendary |

### V2+ - Boss Futurs (Vague 60 → 100)

| Vague | Boss | Mécanique | Skin |
|-------|------|-----------|------|
| 60 | **Ghost** 👻 | 👻 INVISIBILITÉ | 🎨 Phantom |
| 70 | **Berserker** 😡 | 😡 ENRAGÉ | 🎨 Fury |
| 80 | **Mirror** 🪞 | 🪞 CLONES | 🎨 Crystal |
| 90 | **Shaman** 💚 | 💚 HEAL | 🎨 Nature |
| 100 | **Chaos** 🌀 | 🎲 TOUT RANDOM | 🎨 Chaos |

## 6.3 Détail des 5 Mécaniques V1

### 🧱 OBSTACLES (Vague 10+)

```
  🟢    ▓▓    🟢
       🟢
  ▓▓       🟢   ▓▓
```

| Propriété | Valeur |
|-----------|--------|
| Effet | Blocs statiques, rebond comme murs |
| Spawn | 2-4 obstacles par vague |
| Taille | 1x1 à 2x1 blocs |
| Stratégie | Utiliser rebonds pour atteindre ennemis cachés |

### 🏃 DÉPLACEMENT (Vague 20+)

```
  🟢 →          ← 🔵
        🟢 ↔
  🔵 →              🟢
```

| Propriété | Valeur |
|-----------|--------|
| Effet | Ennemis se déplacent horizontalement |
| Vitesse | Lente (1), augmente avec vagues |
| % Ennemis mobiles | 20% (vague 20) → 50% (vague 40+) |
| Stratégie | Anticiper le mouvement, viser devant |

### 🛡️ BOUCLIER (Vague 30+)

```
  [🟢       🔵]
  
  Côté [ = invulnérable
```

| Propriété | Valeur |
|-----------|--------|
| Effet | Ennemi invulnérable d'un côté |
| Côté | Gauche OU droite (aléatoire) |
| Visuel | Demi-cercle brillant |
| % Ennemis bouclier | 15% (vague 30) → 30% (vague 50+) |
| Stratégie | Rebondir pour toucher le côté vulnérable |

### ✂️ DIVISION (Vague 40+)

```
     💥
  🟣 → 🔵 + 🔵
  
  À 50% HP, se divise en 2 petits
```

| Propriété | Valeur |
|-----------|--------|
| Effet | Se divise en 2 ennemis à 50% HP |
| HP des petits | 25% du parent |
| Dégâts des petits | 50% du parent |
| % Ennemis diviseurs | 10% (vague 40) → 25% (vague 60+) |
| Stratégie | One-shot si possible, sinon gérer les petits |

### 🔄 CONSOLIDATION (Vague 50 - Boss)

Le boss de la vague 50 utilise **TOUTES les mécaniques** :
- Se cache derrière obstacles
- Se déplace
- A un bouclier rotatif
- Se divise à 50% HP

C'est le **test final** de toutes les mécaniques apprises !

## 6.4 Tableau des Mécaniques par Vague

| Vague | Obstacles | Déplacement | Bouclier | Division |
|-------|:---------:|:-----------:|:--------:|:--------:|
| 1-9 | ❌ | ❌ | ❌ | ❌ |
| 10-19 | ✅ | ❌ | ❌ | ❌ |
| 20-29 | ✅ | ✅ | ❌ | ❌ |
| 30-39 | ✅ | ✅ | ✅ | ❌ |
| 40-49 | ✅ | ✅ | ✅ | ✅ |
| 50+ | ✅ | ✅ | ✅ | ✅ |

---

# 7. Système d'Upgrades (Roguelike)

## 7.1 Fonctionnement

- Entre chaque vague : **3 upgrades aléatoires**
- Le joueur choisit **1** (coûte 💎) OU **passe** (gratuit)
- **Reset au Game Over**

## 7.2 Upgrades V1 (12 upgrades)

### Offensifs

| Upgrade | Effet | Coût 💎 | Icône |
|---------|-------|---------|-------|
| **Pointe Acérée** | +1 dégât | 15 | ⚔️ |
| **Critique** | 20% chance x2 dégâts | 25 | ⭐ |
| **Explosion** | Dégâts de zone petit rayon | 35 | 💥 |

### Défensifs

| Upgrade | Effet | Coût 💎 | Icône |
|---------|-------|---------|-------|
| **Canon Blindé** | +5 HP max | 12 | 🛡️ |
| **Régénération** | +2 HP par vague | 18 | ❤️‍🩹 |
| **Vampirisme** | +1 HP par kill | 40 | 🧛 |

### Utilitaires

| Upgrade | Effet | Coût 💎 | Icône |
|---------|-------|---------|-------|
| **Carquois Élargi** | +1 boulet/vague | 12 | 📦 |
| **Boulet Robuste** | +1 traverse | 10 | 🔵 |
| **Magnétisme** | Gems attirées auto | 25 | 🧲 |
| **Vision** | +2 rebonds prédiction | 15 | 👁️ |

### Économiques

| Upgrade | Effet | Coût 💎 | Icône |
|---------|-------|---------|-------|
| **Prospecteur** | +25% gems drop | 20 | 💎 |
| **Chanceux** | +10% drop rare | 22 | 🍀 |

---

# 8. Économie & Progression

## 8.1 Deux Monnaies

| Monnaie | Usage | Persistance |
|---------|-------|-------------|
| 💎 **Gems** | Upgrades (in-run) | Reset au game over |
| 🪙 **Coins** | Skins (permanent) | Sauvegardé |

## 8.2 Gains de Coins

| Source | Montant |
|--------|---------|
| Fin de run | `vague × 2` |
| Rewarded Ad | `× 1.5` (x2 coins) |
| Achievement | 50 - 500 🪙 |
| Daily Login | 50 - 500 🪙 |

## 8.3 Sauvegarde (PlayerPrefs)

```csharp
// Données sauvegardées
PlayerPrefs.SetInt("TotalCoins", coins);
PlayerPrefs.SetInt("HighScore", bestWave);
PlayerPrefs.SetInt("TutorialDone", 1);
PlayerPrefs.SetString("UnlockedSkins", "default,bronze");
PlayerPrefs.SetString("EquippedSkin", "default");
PlayerPrefs.SetString("Achievements", jsonData);
PlayerPrefs.SetString("DailyLogin", jsonData);
```

---

# 9. Système de Skins

## 9.1 Skins V1 (7 skins)

| Skin | Visuel | Déblocage Principal | Prix 🪙 alternatif |
|------|--------|---------------------|-------------------|
| **Default** | Canon gris | Gratuit | - |
| **Bronze** 🥉 | Canon cuivré | Boss Vague 10 | 500 |
| **Silver** 🥈 | Canon argenté | Boss Vague 20 | 1000 |
| **Gold** 🥇 | Canon doré | Boss Vague 30 | 2500 |
| **Diamond** 💎 | Canon diamant | Boss Vague 40 | 5000 |
| **Legendary** ⭐ | Canon épique | Boss Vague 50 | 10000 |
| **Rainbow** 🌈 | Canon arc-en-ciel | Daily Login 7j | - (exclusif) |

## 9.2 Skins V2+ (Post-launch)

| Skin | Déblocage |
|------|-----------|
| **Fire** 🔥 | Achievement: 1000 kills |
| **Ice** ❄️ | Achievement: Combo x20 |
| **Dark** 🖤 | Achievement: 100 runs |
| **Phantom** 👻 | Boss Vague 60 |
| **Fury** 😡 | Boss Vague 70 |

## 9.3 États d'un Skin

| État | Visuel | Action disponible |
|------|--------|-------------------|
| 🔒 **Locked (Boss)** | Grisé + "Vague X" | Acheter OU jouer |
| 🔒 **Locked (Daily)** | Grisé + "Login 7j" | Aucune (exclusif) |
| ✅ **Unlocked** | Couleur normale | Équiper |
| ⭐ **Equipped** | Bordure dorée + "✓" | Déjà équipé |

## 9.4 UI Page Skins - Vue Principale

```
┌─────────────────────────────────────────────┐
│  ← RETOUR              🎨 SKINS             │
│                                             │
│                    🪙 2,450                 │
│                                             │
│  ┌─────────────────────────────────────┐    │
│  │                                     │    │
│  │           ╔═══════════╗             │    │
│  │           ║  ══╦══    ║             │    │
│  │           ║    ║      ║  PREVIEW    │    │
│  │           ║ ───╩───   ║             │    │
│  │           ╚═══════════╝             │    │
│  │                                     │    │
│  │         "BRONZE" 🥉                 │    │
│  │         Équipé ✓                    │    │
│  │                                     │    │
│  └─────────────────────────────────────┘    │
│                                             │
│  ═══════════ COLLECTION (3/7) ═══════════  │
│                                             │
│  ┌───────┐ ┌───────┐ ┌───────┐ ┌───────┐   │
│  │░░░░░░░│ │▓▓▓▓▓▓▓│ │░░░░░░░│ │░░░░░░░│   │
│  │░═══╦══│ │▓═══╦══│ │░═══╦══│ │░═══╦══│   │
│  │░   ║ ░│ │▓   ║ ▓│ │░   ║ ░│ │░   ║ ░│   │
│  │       │ │  ⭐   │ │  🔒   │ │  🔒   │   │
│  │DEFAULT│ │BRONZE │ │SILVER │ │ GOLD  │   │
│  │  ✓    │ │ÉQUIPÉ │ │Vague20│ │Vague30│   │
│  └───────┘ └───────┘ └───────┘ └───────┘   │
│                                             │
│  ┌───────┐ ┌───────┐ ┌───────┐             │
│  │  🔒   │ │  🔒   │ │  🔒   │             │
│  │DIAMOND│ │LEGEND.│ │RAINBOW│             │
│  │Vague40│ │Vague50│ │Login7j│             │
│  └───────┘ └───────┘ └───────┘             │
│                                             │
└─────────────────────────────────────────────┘
```

## 9.5 UI Skin Locked (Boss) - Avec Option Achat

```
┌─────────────────────────────────────────────┐
│  ← RETOUR              🎨 SKINS             │
│                                             │
│  ┌─────────────────────────────────────┐    │
│  │                                     │    │
│  │           ╔═══════════╗             │    │
│  │           ║  ══╦══ 🔒 ║  LOCKED     │    │
│  │           ║    ║      ║             │    │
│  │           ╚═══════════╝             │    │
│  │                                     │    │
│  │         "SILVER" 🥈                 │    │
│  │                                     │    │
│  │    ┌─────────────────────────┐      │    │
│  │    │  🏆 Battre le boss      │      │    │
│  │    │     de la VAGUE 20      │      │    │
│  │    └─────────────────────────┘      │    │
│  │                                     │    │
│  │             - OU -                  │    │
│  │                                     │    │
│  │    ┌─────────────────────────┐      │    │
│  │    │  🪙 ACHETER - 1000      │      │    │
│  │    └─────────────────────────┘      │    │
│  │                                     │    │
│  └─────────────────────────────────────┘    │
└─────────────────────────────────────────────┘
```

## 9.6 UI Skin Locked (Daily) - Exclusif

```
┌─────────────────────────────────────────────┐
│  ← RETOUR              🎨 SKINS             │
│                                             │
│  ┌─────────────────────────────────────┐    │
│  │                                     │    │
│  │           ╔═══════════╗             │    │
│  │           ║  ══╦══ 🔒 ║  LOCKED     │    │
│  │           ║    ║      ║             │    │
│  │           ╚═══════════╝             │    │
│  │                                     │    │
│  │         "RAINBOW" 🌈                │    │
│  │                                     │    │
│  │    ┌─────────────────────────┐      │    │
│  │    │  📅 Connecte-toi        │      │    │
│  │    │     7 JOURS CONSÉCUTIFS │      │    │
│  │    │                         │      │    │
│  │    │     Progression: 3/7    │      │    │
│  │    │     ████████░░░░░░░░    │      │    │
│  │    └─────────────────────────┘      │    │
│  │                                     │    │
│  │         ⚠️ SKIN EXCLUSIF           │    │
│  │      Non achetable avec coins       │    │
│  │                                     │    │
│  └─────────────────────────────────────┘    │
└─────────────────────────────────────────────┘
```

## 9.7 UI Skin Unlocked - Équiper

```
┌─────────────────────────────────────────────┐
│  ← RETOUR              🎨 SKINS             │
│                                             │
│  ┌─────────────────────────────────────┐    │
│  │                                     │    │
│  │           ╔═══════════╗             │    │
│  │           ║  ══╦══    ║  DÉBLOQUÉ   │    │
│  │           ║    ║      ║             │    │
│  │           ╚═══════════╝             │    │
│  │                                     │    │
│  │         "DEFAULT"                   │    │
│  │                                     │    │
│  │    ┌─────────────────────────┐      │    │
│  │    │      ⭐ ÉQUIPER         │      │    │
│  │    └─────────────────────────┘      │    │
│  │                                     │    │
│  └─────────────────────────────────────┘    │
└─────────────────────────────────────────────┘
```

## 9.8 UI Popup Confirmation Achat

```
┌─────────────────────────────────────────────┐
│                                             │
│     ┌───────────────────────────────┐       │
│     │                               │       │
│     │      ACHETER "SILVER" ?       │       │
│     │                               │       │
│     │         ══╦══                 │       │
│     │           ║                   │       │
│     │        ───╩───                │       │
│     │                               │       │
│     │        Prix: 1000 🪙          │       │
│     │                               │       │
│     │   Ton solde: 2,450 🪙         │       │
│     │   Après achat: 1,450 🪙       │       │
│     │                               │       │
│     │  ┌─────────┐  ┌─────────┐     │       │
│     │  │ ANNULER │  │ ACHETER │     │       │
│     │  └─────────┘  └─────────┘     │       │
│     │                               │       │
│     └───────────────────────────────┘       │
│                                             │
└─────────────────────────────────────────────┘
```

## 9.9 UI Popup Coins Insuffisants

```
┌─────────────────────────────────────────────┐
│                                             │
│     ┌───────────────────────────────┐       │
│     │                               │       │
│     │      ⚠️ COINS INSUFFISANTS    │       │
│     │                               │       │
│     │        Prix: 5000 🪙          │       │
│     │   Ton solde: 2,450 🪙         │       │
│     │                               │       │
│     │      Il te manque 2,550 🪙    │       │
│     │                               │       │
│     │   💡 Joue plus pour gagner    │       │
│     │      des coins !              │       │
│     │                               │       │
│     │       ┌─────────────┐         │       │
│     │       │   COMPRIS   │         │       │
│     │       └─────────────┘         │       │
│     │                               │       │
│     └───────────────────────────────┘       │
│                                             │
└─────────────────────────────────────────────┘
```

## 9.10 UI Popup Skin Débloqué

```
┌─────────────────────────────────────────────┐
│                                             │
│     ┌───────────────────────────────┐       │
│     │                               │       │
│     │     🎉 NOUVEAU SKIN ! 🎉      │       │
│     │                               │       │
│     │           ══╦══               │       │
│     │             ║                 │       │
│     │          ───╩───              │       │
│     │                               │       │
│     │       "SILVER" 🥈             │       │
│     │                               │       │
│     │    Tu as battu le boss de     │       │
│     │        la vague 20 !          │       │
│     │                               │       │
│     │  ┌─────────┐  ┌─────────┐     │       │
│     │  │  SUPER  │  │ ÉQUIPER │     │       │
│     │  └─────────┘  └─────────┘     │       │
│     │                               │       │
│     └───────────────────────────────┘       │
│                                             │
└─────────────────────────────────────────────┘
```

## 9.11 Flow Navigation Skins

```
MENU PRINCIPAL
      │
      └── [SKINS] 🎨
              │
              ▼
      ┌─────────────────┐
      │   PAGE SKINS    │
      │  - Preview      │
      │  - Collection   │
      └────────┬────────┘
               │
       ┌───────┴───────┐
       │               │
       ▼               ▼
  [SKIN LOCKED]   [SKIN UNLOCKED]
       │               │
       │               └──→ [ÉQUIPER] ──→ Skin appliqué
       │
       ├── Boss unlock? ──→ "Battre boss vague X"
       │                         │
       │                         └──→ [ACHETER] ──→ Confirmation
       │
       └── Daily unlock? ──→ "Login 7j" (pas achetable)


  CONFIRMATION ACHAT
       │
       ├── Assez de coins? ──→ ACHAT OK ──→ Popup Succès
       │
       └── Pas assez? ──→ Popup Erreur
```

## 9.12 Implémentation

### SkinData.cs (ScriptableObject)
```csharp
using UnityEngine;

[CreateAssetMenu(fileName = "Skin", menuName = "MonsterCannon/Skin")]
public class SkinData : ScriptableObject
{
    public string skinId;
    public string skinName;
    public Sprite skinSprite;
    public Sprite skinPreview;
    
    [Header("Unlock")]
    public UnlockType unlockType;
    public int bossWaveRequired;
    public int coinPrice;
    public bool isDailyExclusive;
    
    public enum UnlockType { Free, Boss, Daily }
}
```

### SkinManager.cs
```csharp
using UnityEngine;
using System.Collections.Generic;

public class SkinManager : MonoBehaviour
{
    public static SkinManager Instance;
    
    public List<SkinData> allSkins;
    public SpriteRenderer cannonRenderer;
    
    private HashSet<string> unlockedSkins = new HashSet<string>();
    private string equippedSkinId = "default";
    
    void Awake()
    {
        Instance = this;
        LoadData();
    }
    
    // === UNLOCK ===
    
    public void OnBossDefeated(int waveNumber)
    {
        var skin = allSkins.Find(s => 
            s.unlockType == SkinData.UnlockType.Boss && 
            s.bossWaveRequired == waveNumber);
            
        if (skin != null && !IsUnlocked(skin.skinId))
        {
            UnlockSkin(skin.skinId);
            SkinShopUI.Instance?.ShowUnlockPopup(skin);
        }
    }
    
    public void OnDailyLoginComplete()
    {
        var skin = allSkins.Find(s => s.isDailyExclusive);
        if (skin != null && !IsUnlocked(skin.skinId))
        {
            UnlockSkin(skin.skinId);
            SkinShopUI.Instance?.ShowUnlockPopup(skin);
        }
    }
    
    public void UnlockSkin(string skinId)
    {
        unlockedSkins.Add(skinId);
        SaveData();
    }
    
    // === PURCHASE ===
    
    public enum PurchaseResult { Success, NotEnoughCoins, AlreadyOwned, NotPurchasable }
    
    public PurchaseResult TryPurchaseSkin(string skinId)
    {
        var skin = GetSkin(skinId);
        
        if (skin == null || IsUnlocked(skinId))
            return PurchaseResult.AlreadyOwned;
            
        if (skin.isDailyExclusive || skin.coinPrice <= 0)
            return PurchaseResult.NotPurchasable;
            
        if (CoinManager.Instance.GetCoins() < skin.coinPrice)
            return PurchaseResult.NotEnoughCoins;
        
        CoinManager.Instance.SpendCoins(skin.coinPrice);
        UnlockSkin(skinId);
        return PurchaseResult.Success;
    }
    
    // === EQUIP ===
    
    public bool EquipSkin(string skinId)
    {
        if (!IsUnlocked(skinId)) return false;
        
        equippedSkinId = skinId;
        SaveData();
        ApplyEquippedSkin();
        return true;
    }
    
    void ApplyEquippedSkin()
    {
        var skin = GetSkin(equippedSkinId);
        if (skin != null && cannonRenderer != null)
            cannonRenderer.sprite = skin.skinSprite;
    }
    
    // === GETTERS ===
    
    public SkinData GetSkin(string skinId) => allSkins.Find(s => s.skinId == skinId);
    public SkinData GetEquippedSkin() => GetSkin(equippedSkinId);
    public bool IsUnlocked(string skinId) => unlockedSkins.Contains(skinId);
    public bool IsEquipped(string skinId) => equippedSkinId == skinId;
    public int GetUnlockedCount() => unlockedSkins.Count;
    public int GetTotalCount() => allSkins.Count;
    
    public string GetUnlockRequirement(SkinData skin)
    {
        return skin.unlockType switch
        {
            SkinData.UnlockType.Free => "Gratuit",
            SkinData.UnlockType.Boss => $"Vague {skin.bossWaveRequired}",
            SkinData.UnlockType.Daily => "Login 7j",
            _ => ""
        };
    }
    
    // === SAVE/LOAD ===
    
    void SaveData()
    {
        PlayerPrefs.SetString("UnlockedSkins", string.Join(",", unlockedSkins));
        PlayerPrefs.SetString("EquippedSkin", equippedSkinId);
        PlayerPrefs.Save();
    }
    
    void LoadData()
    {
        string saved = PlayerPrefs.GetString("UnlockedSkins", "default");
        unlockedSkins = new HashSet<string>(saved.Split(','));
        unlockedSkins.Add("default");
        
        equippedSkinId = PlayerPrefs.GetString("EquippedSkin", "default");
        if (!unlockedSkins.Contains(equippedSkinId))
            equippedSkinId = "default";
    }
}
```

### SkinShopUI.cs
```csharp
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class SkinShopUI : MonoBehaviour
{
    public static SkinShopUI Instance;
    
    [Header("Main UI")]
    public GameObject skinShopPanel;
    public Text coinsText;
    public Text collectionText;
    
    [Header("Preview")]
    public Image previewImage;
    public Text previewNameText;
    public Text previewStatusText;
    
    [Header("Action Buttons")]
    public GameObject equipButton;
    public GameObject buyButton;
    public Text buyButtonPriceText;
    public GameObject lockedBossInfo;
    public Text lockedBossText;
    public GameObject lockedDailyInfo;
    public Slider lockedDailyProgressBar;
    
    [Header("Skin Grid")]
    public Transform skinGridParent;
    public GameObject skinItemPrefab;
    
    [Header("Popups")]
    public GameObject confirmPopup;
    public GameObject notEnoughPopup;
    public GameObject unlockPopup;
    
    private SkinData selectedSkin;
    
    void Awake() => Instance = this;
    
    public void Open()
    {
        skinShopPanel.SetActive(true);
        RefreshUI();
        SelectSkin(SkinManager.Instance.GetEquippedSkin());
    }
    
    public void Close() => skinShopPanel.SetActive(false);
    
    void RefreshUI()
    {
        coinsText.text = CoinManager.Instance.GetCoins().ToString();
        collectionText.text = $"COLLECTION ({SkinManager.Instance.GetUnlockedCount()}/{SkinManager.Instance.GetTotalCount()})";
        RefreshGrid();
    }
    
    void RefreshGrid()
    {
        // Clear and rebuild grid
        foreach (Transform child in skinGridParent)
            Destroy(child.gameObject);
            
        foreach (var skin in SkinManager.Instance.allSkins)
        {
            var item = Instantiate(skinItemPrefab, skinGridParent);
            var ui = item.GetComponent<SkinItemUI>();
            ui.Setup(skin, () => SelectSkin(skin));
        }
    }
    
    void SelectSkin(SkinData skin)
    {
        selectedSkin = skin;
        previewImage.sprite = skin.skinPreview ?? skin.skinSprite;
        previewNameText.text = $"\"{skin.skinName.ToUpper()}\"";
        
        bool unlocked = SkinManager.Instance.IsUnlocked(skin.skinId);
        bool equipped = SkinManager.Instance.IsEquipped(skin.skinId);
        
        // Hide all buttons
        equipButton.SetActive(false);
        buyButton.SetActive(false);
        lockedBossInfo.SetActive(false);
        lockedDailyInfo.SetActive(false);
        
        if (equipped)
        {
            previewStatusText.text = "Équipé ✓";
        }
        else if (unlocked)
        {
            previewStatusText.text = "Débloqué";
            equipButton.SetActive(true);
        }
        else
        {
            previewStatusText.text = "Verrouillé 🔒";
            
            if (skin.unlockType == SkinData.UnlockType.Boss)
            {
                lockedBossInfo.SetActive(true);
                lockedBossText.text = $"🏆 Battre boss VAGUE {skin.bossWaveRequired}";
                if (skin.coinPrice > 0)
                {
                    buyButton.SetActive(true);
                    buyButtonPriceText.text = $"🪙 ACHETER - {skin.coinPrice}";
                }
            }
            else if (skin.unlockType == SkinData.UnlockType.Daily)
            {
                lockedDailyInfo.SetActive(true);
                int progress = DailyLoginManager.Instance?.GetCurrentStreak() ?? 0;
                lockedDailyProgressBar.value = progress / 7f;
            }
        }
    }
    
    public void OnEquipClicked()
    {
        SkinManager.Instance.EquipSkin(selectedSkin.skinId);
        RefreshUI();
        SelectSkin(selectedSkin);
    }
    
    public void OnBuyClicked()
    {
        int coins = CoinManager.Instance.GetCoins();
        if (coins >= selectedSkin.coinPrice)
            confirmPopup.SetActive(true);
        else
            notEnoughPopup.SetActive(true);
    }
    
    public void OnConfirmPurchase()
    {
        SkinManager.Instance.TryPurchaseSkin(selectedSkin.skinId);
        confirmPopup.SetActive(false);
        ShowUnlockPopup(selectedSkin);
        RefreshUI();
        SelectSkin(selectedSkin);
    }
    
    public void ShowUnlockPopup(SkinData skin)
    {
        unlockPopup.SetActive(true);
        // Configure popup with skin info
    }
}
```

---

# 10. Achievements

## 10.1 Achievements V1 (15 achievements)

### Progression (5)

| Achievement | Condition | Reward |
|-------------|-----------|--------|
| Premier pas | Vague 5 | 50 🪙 |
| Apprenti | Vague 10 | 100 🪙 |
| Survivant | Vague 25 | 250 🪙 |
| Vétéran | Vague 40 | 400 🪙 |
| Légende | Vague 50 | 500 🪙 |

### Combat (5)

| Achievement | Condition | Reward |
|-------------|-----------|--------|
| Premier sang | 1 kill | 25 🪙 |
| Chasseur | 100 kills total | 100 🪙 |
| Exterminateur | 500 kills total | 250 🪙 |
| Tueur de boss | 1 boss tué | 100 🪙 |
| Chasseur de boss | 5 boss tués | 300 🪙 |

### Skill (3)

| Achievement | Condition | Reward |
|-------------|-----------|--------|
| Combo x5 | Combo de 5 | 75 🪙 |
| Combo x10 | Combo de 10 | 150 🪙 |
| Perfectionniste | Vague sans dégât | 100 🪙 |

### Rétention (2)

| Achievement | Condition | Reward |
|-------------|-----------|--------|
| Fidèle | Daily 7 jours | Skin Rainbow |
| Accro | 50 parties jouées | 200 🪙 |

---

# 11. Daily Login

## 11.1 Rewards 7 Jours

| Jour | Reward |
|------|--------|
| 1 | 50 🪙 |
| 2 | 100 🪙 |
| 3 | Upgrade gratuit (prochain run) |
| 4 | 150 🪙 |
| 5 | 200 🪙 |
| 6 | 300 🪙 |
| 7 | **Skin Rainbow** 🌈 |

**Total : 800 🪙 + 1 upgrade + 1 skin**

## 11.2 Règles

- Reset du cycle après jour 7
- Miss 1 jour = retour jour 1
- Rainbow skin : une seule fois

---

# 12. Monétisation (Rewarded Ads Only)

## 12.1 Philosophie

**ZÉRO pub forcée.** Le joueur choisit TOUJOURS de regarder une pub.

| Type | Utilisé |
|------|---------|
| Rewarded | ✅ Oui |
| Interstitial | ❌ Non |
| Banner | ❌ Non |

## 12.2 Placements Rewarded V1

| Placement | Quand | Reward | Limite |
|-----------|-------|--------|--------|
| **Revive** | Game Over | 50% HP, continuer | 1x/run |
| **Double Coins** | Game Over | x2 coins du run | 1x/run |
| **Bonus Boulets** | Menu | +2 boulets prochain run | 3x/jour |

## 12.3 UI Game Over

```
┌─────────────────────────────────────────────┐
│              💀 GAME OVER                   │
│                                             │
│           Vague atteinte: 23                │
│           Meilleur: 35                      │
│                                             │
│           Coins gagnés: 46 🪙               │
│                                             │
│  ┌─────────────────────────────────────┐    │
│  │  🎬 REVIVE (Regarder pub)           │    │
│  │  Reprendre avec 50% HP              │    │
│  └─────────────────────────────────────┘    │
│                                             │
│  ┌─────────────────────────────────────┐    │
│  │  🎬 x2 COINS (Regarder pub)         │    │
│  │  46 🪙 → 92 🪙                       │    │
│  └─────────────────────────────────────┘    │
│                                             │
│        [MENU]           [REJOUER]           │
└─────────────────────────────────────────────┘
```

---

# 13. Interface Utilisateur

## 13.1 Menu Principal

```
┌─────────────────────────────────────────────┐
│                                             │
│            💥 MONSTER CANNON                │
│                                             │
│              Meilleur: Vague 35             │
│                                             │
│           ┌─────────────────┐               │
│           │     JOUER       │               │
│           └─────────────────┘               │
│                                             │
│       🎨           🏆           📅          │
│      SKINS    ACHIEVEMENTS   DAILY          │
│                                             │
│   🪙 2,450                    ⚙️            │
└─────────────────────────────────────────────┘
```

## 13.2 HUD Gameplay

```
┌─────────────────────────────────────────────┐
│  ⏸️                          VAGUE 23       │
│                                             │
│  ❤️ 15/20    💎 89    🔫 2/3               │
│                                             │
│        Monstres: 8        COMBO x4!        │
│                                             │
│     🟢    ▓▓    [🔵                        │
│          🟢 →                               │
│     ▓▓        🟢    🔵                     │
│                                             │
│           · · · · ·                         │
│              · · ·                          │
│              ══╦══                          │
│           ─────╩─────                       │
└─────────────────────────────────────────────┘
```

## 13.3 Settings

```
┌─────────────────────────────────────────────┐
│               ⚙️ PARAMÈTRES                 │
│                                             │
│  🔊 Son                          [ON] OFF   │
│  🎵 Musique                      [ON] OFF   │
│  📳 Vibration                    [ON] OFF   │
│  🌐 Langue                       [FR] EN    │
│                                             │
│  📖 Revoir tutoriel                      →  │
│  🔒 Politique de confidentialité         →  │
│                                             │
│              [ ← RETOUR ]                   │
└─────────────────────────────────────────────┘
```

---

# 14. Audio

## 14.1 SFX (V1)

| Son | Event |
|-----|-------|
| Cannon Fire | Tir |
| Wall Bounce | Rebond mur/obstacle |
| Monster Hit | Touche monstre |
| Monster Death | Monstre meurt |
| Player Damage | Canon touché |
| Gem Collect | Ramasse gem |
| Upgrade Select | Choisit upgrade |
| Button Click | UI |
| Wave Complete | Fin vague |
| Game Over | Défaite |
| Boss Appear | Boss spawn |

## 14.2 Music (V1)

| Piste | Écran |
|-------|-------|
| Menu Theme | Menu (chill, loop) |
| Combat Theme | Gameplay (energetic, loop) |
| Boss Theme | Vagues boss (intense, loop) |

## 14.3 Sources CC0

- **Kenney.nl** : SFX complets
- **OpenGameArt** : Creatures
- **Soundimage.org** : Music

---

# 15. Roadmap V1

## Timeline (30 min/jour)

| Semaine | Focus | Livrables |
|---------|-------|-----------|
| **1-2** | Core infini | Vagues infinies, scaling, 4 types monstres |
| **3-4** | Boss & Mécaniques | 5 boss, obstacles, déplacement, bouclier, division |
| **5** | Tutoriel | 7 étapes, FR/EN |
| **6** | Économie | Coins, gems, sauvegarde |
| **7** | Progression | 7 skins, 15 achievements, daily login |
| **8** | Monétisation | Unity Ads (rewarded only) |
| **9** | Audio & Polish | SFX, music, effets FEEL |
| **10** | Publication | Icon, screenshots, store listing, submit |

## Checklist V1

### Core ✅
- [ ] Vagues infinies avec scaling
- [ ] 4 types de monstres
- [ ] 12 upgrades
- [ ] HP restauré entre vagues
- [ ] Game Over + Revive

### Boss & Mécaniques ✅
- [ ] Boss vague 10: Blob King + Obstacles
- [ ] Boss vague 20: Speedy + Déplacement
- [ ] Boss vague 30: Guardian + Bouclier
- [ ] Boss vague 40: Splitter + Division
- [ ] Boss vague 50: Overlord (consolidation)
- [ ] Mécaniques se mélangent après déblocage

### Progression ✅
- [ ] Tutoriel 7 étapes
- [ ] 7 skins (5 boss + daily + default)
- [ ] 15 achievements
- [ ] Daily login 7 jours
- [ ] Sauvegarde PlayerPrefs

### Monétisation ✅
- [ ] Rewarded Ad: Revive
- [ ] Rewarded Ad: Double Coins
- [ ] Rewarded Ad: Bonus Boulets

### Polish ✅
- [ ] Audio (SFX + Music)
- [ ] Effets FEEL (screenshake, flash)
- [ ] FR + EN

### Publication ✅
- [ ] Icône 512x512
- [ ] Feature Graphic 1024x500
- [ ] Screenshots (5-8)
- [ ] Description FR + EN
- [ ] Privacy Policy
- [ ] Google Play Console

---

# 16. Notes Techniques

## 16.1 Architecture

```
Assets/
├── Scripts/
│   ├── Core/
│   │   ├── GameManager.cs
│   │   ├── WaveManager.cs
│   │   └── SaveManager.cs
│   ├── Player/
│   │   ├── CannonController.cs
│   │   └── BulletController.cs
│   ├── Enemies/
│   │   ├── Monster.cs
│   │   ├── Boss.cs
│   │   └── MonsterSpawner.cs
│   ├── Mechanics/
│   │   ├── ObstacleManager.cs
│   │   ├── MovementBehavior.cs
│   │   ├── ShieldBehavior.cs
│   │   └── SplitBehavior.cs
│   ├── Progression/
│   │   ├── UpgradeManager.cs
│   │   ├── SkinManager.cs
│   │   ├── AchievementManager.cs
│   │   └── DailyLoginManager.cs
│   ├── Economy/
│   │   ├── GemManager.cs
│   │   └── CoinManager.cs
│   ├── Monetization/
│   │   └── AdsManager.cs
│   ├── Tutorial/
│   │   └── TutorialManager.cs
│   └── UI/
│       └── [All UI scripts]
├── Prefabs/
├── ScriptableObjects/
├── Scenes/
│   └── Game.unity (single scene)
├── Audio/
└── Art/
```

## 16.2 Performance

| Métrique | Target |
|----------|--------|
| FPS | 60 stable |
| Load time | < 3s |
| APK size | < 100MB |

## 16.3 Assets

| Asset | Usage |
|-------|-------|
| FEEL | Effets juice |
| All In 1 Sprite Shader | Effets visuels |
| CraftPix Merge Shooter | Sprites |
| Unity Ads | Monétisation |
| Kenney Assets | Audio (CC0) |

---

# Changelog

| Version | Date | Notes |
|---------|------|-------|
| 1.0 | Fév 2026 | Arrow Strike initial |
| 2.0 | Fév 2026 | Pivot vers Canon |
| 3.0 | Fév 2026 | Monster Cannon complet |
| **3.1** | **Fév 2026** | **V1 Scope défini** |
| | | - Boss toutes les 10 vagues |
| | | - 5 mécaniques progressives |
| | | - Rewarded ads only |
| | | - V1 vs V2+ clairement séparés |

---

> 🎯 **FOCUS V1 : Sortir sur Google Play !**
> 
> Les features V2+ viendront APRÈS la publication.
> Un jeu publié > un jeu parfait jamais sorti.

---

**Monster Cannon - V1 Scope**  
**Développeur : RDH**  
**Février 2026**
