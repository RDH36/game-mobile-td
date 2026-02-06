# GAME DESIGN DOCUMENT
## 🏹 Arrow Strike

**Version :** 2.0  
**Dernière mise à jour :** Février 2026

---

| Info | Détail |
|------|--------|
| **Genre** | Arcade Roguelike Hypercasual |
| **Plateforme** | Mobile (Android) |
| **Moteur** | Unity |
| **Inspiration** | Angry Birds + Bowling + Billard + Roguelike |
| **Session dev** | 30 min/jour |
| **Modèle économique** | Free-to-Play + Ads + IAP |

---

# TABLE DES MATIÈRES

1. [Concept du Jeu](#1-concept-du-jeu)
2. [Boucle de Gameplay](#2-boucle-de-gameplay)
3. [Mécaniques de Gameplay](#3-mécaniques-de-gameplay)
4. [Entités du Jeu](#4-entités-du-jeu)
5. [Système de Vagues](#5-système-de-vagues)
6. [Système d'Upgrades (Roguelike)](#6-système-dupgrades-roguelike)
7. [Économie & Méta-progression](#7-économie--méta-progression)
8. [Monétisation](#8-monétisation)
9. [Interface Utilisateur](#9-interface-utilisateur)
10. [Effets Visuels (Juice/FEEL)](#10-effets-visuels-juicefeel)
11. [Audio](#11-audio)
12. [Conditions de Victoire / Défaite](#12-conditions-de-victoire--défaite)
13. [Roadmap de Développement](#13-roadmap-de-développement)
14. [Notes Techniques](#14-notes-techniques)

---

# 1. Concept du Jeu

Arrow Strike est un jeu arcade roguelike où le joueur utilise un arc pour lancer des flèches qui **rebondissent** sur les murs, obstacles et ennemis. L'objectif est de toucher un maximum d'ennemis avec chaque flèche tout en survivant à leurs contre-attaques.

## Pitch en une phrase

> *"Vise, tire, fais rebondir ta flèche et élimine tous les ennemis avant qu'ils ne détruisent ton arc !"*

## Ce qui rend le jeu unique

| Feature | Description |
|---------|-------------|
| **Rebonds partout** | Murs, obstacles ET ennemis font rebondir la flèche |
| **Durabilité flèche** | Chaque contact ennemi use la flèche |
| **Combat bidirectionnel** | Les ennemis survivants contre-attaquent |
| **Progression roguelike** | Upgrades entre chaque vague |
| **Méta-progression** | Boutique, arcs, résistance |

---

# 2. Boucle de Gameplay

## 2.1 Vue d'ensemble

```
┌─────────────────────────────────────────────────────────┐
│                      MÉTA-BOUCLE                        │
│                                                         │
│  BOUTIQUE ←──────────────────────────────────────┐      │
│     │                                            │      │
│     ▼                                            │      │
│  ┌─────────────────────────────────────────────────┐    │
│  │              BOUCLE D'UN RUN                    │    │
│  │                                                 │    │
│  │  ┌───────────────────────────────────────────┐  │    │
│  │  │           BOUCLE D'UNE VAGUE              │  │    │
│  │  │                                           │  │    │
│  │  │  1. VISER (Swipe + Trajectoire)           │  │    │
│  │  │  2. TIRER (Flèche avec rebonds)           │  │    │
│  │  │  3. DÉGÂTS (Flèche → Ennemis)             │  │    │
│  │  │  4. CONTRE-ATTAQUE (Ennemis → Arc)        │  │    │
│  │  │  5. RÉPÉTER jusqu'à 0 flèches/ennemis     │  │    │
│  │  │                                           │  │    │
│  │  └───────────────────────────────────────────┘  │    │
│  │                      ↓                          │    │
│  │              VAGUE TERMINÉE ?                   │    │
│  │         OUI → UPGRADE + VAGUE SUIVANTE          │    │
│  │         NON → GAME OVER ─────────────────────────────┘
│  │                                                 │
│  └─────────────────────────────────────────────────┘
│                        ↓
│                  FIN DU RUN
│              RETOUR À LA BOUTIQUE
│
└─────────────────────────────────────────────────────────┘
```

## 2.2 Déroulement d'un tir

```
SWIPE → TRAJECTOIRE VISIBLE → RELÂCHER → FLÈCHE PART
                                              ↓
                    ┌─────────────────────────────────────┐
                    │         BOUCLE DE REBONDS          │
                    │                                     │
                    │   FLÈCHE TOUCHE QUELQUE CHOSE ?     │
                    │           ↓                         │
                    │   ┌─────────────────────────────┐   │
                    │   │ MUR/OBSTACLE → Rebond       │   │
                    │   │ ENNEMI → Rebond + Dégâts    │   │
                    │   │          Flèche -1 PV       │   │
                    │   │          Ennemi -X PV       │   │
                    │   └─────────────────────────────┘   │
                    │           ↓                         │
                    │   FLÈCHE PV > 0 ?                   │
                    │   OUI → Continue rebonds            │
                    │   NON → Flèche disparaît            │
                    │                                     │
                    └─────────────────────────────────────┘
                                    ↓
                        CONTRE-ATTAQUE DES ENNEMIS
                        (Ennemis survivants → Arc)
```

---

# 3. Mécaniques de Gameplay

## 3.1 Contrôles

| Action | Input | Feedback |
|--------|-------|----------|
| Viser | Swipe depuis l'arc | Ligne trajectoire apparaît |
| Ajuster puissance | Longueur du swipe | Ligne plus longue |
| Ajuster angle | Direction du swipe | Ligne suit le doigt |
| Tirer | Relâcher | Flèche part + vibration |

## 3.2 Système de rebonds

**TOUT fait rebondir la flèche :**

| Surface | Effet sur flèche | Effet sur surface |
|---------|------------------|-------------------|
| Murs (4 bords écran) | Rebond gratuit | Aucun |
| Obstacles | Rebond gratuit | Aucun (ou destructible) |
| Ennemis | Rebond + Flèche -1 PV | Ennemi -X dégâts |

## 3.3 Trajectoire prédictive

- Ligne pointillée pendant le swipe
- Montre les premiers rebonds (2-3 max pour garder du skill)
- Disparaît au moment du tir

## 3.4 Fin de trajectoire

La flèche s'arrête quand :
- ❌ Durabilité = 0 PV
- ❌ Sort de l'écran (impossible si murs)
- ❌ Timer max atteint (anti-boucle infinie)

---

# 4. Entités du Jeu

## 4.1 L'Arc (Joueur)

| Stat | Description | MVP | Futur |
|------|-------------|-----|-------|
| **PV** | Points de vie | 20 | Variable selon l'arc |
| **Position** | Emplacement | Fixe en bas | Fixe en bas |
| **Résistance** | Usure de l'arc | ❌ Non | ✅ Oui |
| **Restauration PV** | Quand | Fin de vague | Fin de vague |

### Arcs disponibles (Futur)

| Arc | PV | Résistance | Bonus | Prix 💎 |
|-----|-----|------------|-------|---------|
| **Basique** | 20 | 10 | - | Gratuit |
| **Renforcé** | 25 | 15 | - | 500 |
| **Élite** | 30 | 20 | +5% dégâts | 1000 |
| **Légendaire** | 35 | 25 | +1 flèche/vague | 2500 |
| **Mythique** | 40 | 30 | +10% dégâts, +1 flèche | 5000 |

## 4.2 La Flèche

| Stat | Description | MVP | Futur |
|------|-------------|-----|-------|
| **Durabilité (PV)** | Nombre de touches | 4 | Upgradeable (5-10) |
| **Dégâts** | Dégâts par touche | 1 | Upgradeable (1-5) |
| **Quantité/vague** | Flèches disponibles | 3 | Upgradeable (3-7) |
| **Effets spéciaux** | Bonus | ❌ Non | ✅ Oui |

### Types de flèches (Futur)

| Type | Effet spécial | Déblocage |
|------|---------------|-----------|
| **Normale** | Aucun | Base |
| **Perforante** | Traverse ennemis 1 PV | Upgrade |
| **Explosive** | AoE au dernier rebond | Upgrade rare |
| **Glaciale** | Ralentit ennemis | Upgrade |
| **Électrique** | Chaîne entre ennemis proches | Upgrade légendaire |

## 4.3 Les Ennemis

| Type | PV | Dégâts à l'arc | Drop 💎 | MVP | Visuel |
|------|-----|----------------|---------|-----|--------|
| **Faible** | 1 | 1 | 1-2 | ✅ | Petit, vert |
| **Moyen** | 2 | 2 | 3-5 | ✅ | Moyen, bleu |
| **Fort** | 3 | 3 | 5-10 | ❌ | Grand, rouge |
| **Tank** | 5 | 4 | 10-15 | ❌ | Très grand, violet |
| **Boss** | 15+ | 6 | 50+ | ❌ | Énorme, or |

### Comportements ennemis (Futur)

| Type | Comportement |
|------|--------------|
| **Statique** | Ne bouge pas (MVP) |
| **Mobile** | Se déplace lentement |
| **Tireur** | Tire des projectiles |
| **Bouclier** | Invulnérable d'un côté |
| **Diviseur** | Se divise en 2 petits ennemis |

## 4.4 Les Obstacles

| Type | Effet | MVP | Visuel |
|------|-------|-----|--------|
| **Mur simple** | Rebond | ✅ | Rectangle gris |
| **Mur destructible** | Rebond, peut être détruit (3 PV) | ❌ | Rectangle fissuré |
| **Bumper** | Rebond amplifié (+50% vitesse) | ❌ | Cercle orange |
| **Portail** | Téléporte la flèche | ❌ | Ovale bleu |
| **Miroir** | Rebond parfait (angle = angle) | ❌ | Rectangle brillant |

---

# 5. Système de Vagues

## 5.1 Structure MVP (4 vagues)

| Vague | Ennemis | Types | PV ennemis | Dégâts | Difficulté |
|-------|---------|-------|------------|--------|------------|
| 1 | 4-5 | Faible | 1 | 1 | ⭐ |
| 2 | 6-7 | Faible + Moyen | 1-2 | 1-2 | ⭐⭐ |
| 3 | 7-8 | Moyen | 2 | 2 | ⭐⭐⭐ |
| 4 | 8-10 | Moyen + Fort | 2-3 | 2-3 | ⭐⭐⭐⭐ |

## 5.2 Scaling procédural (Futur)

| Vague | Formule ennemis | Formule PV |
|-------|-----------------|------------|
| N | 4 + (N × 1.5) | 1 + (N / 3) |

**Boss toutes les 5 vagues**

## 5.3 Entre chaque vague

1. ✅ **Calcul des gemmes** récoltées
2. ✅ **PV Arc restauré** à 100%
3. ⏳ **Résistance Arc -1** (futur)
4. ✅ **Écran d'upgrade** apparaît
5. ✅ Joueur choisit **1 upgrade parmi 3**
6. ➡️ **Vague suivante** commence

---

# 6. Système d'Upgrades (Roguelike)

## 6.1 Fonctionnement

- Entre chaque vague : **3 upgrades proposés**
- Le joueur en choisit **1**
- Coûte des **gemmes 💎**
- Les upgrades **s'accumulent** pendant le run
- **Reset au prochain run**

## 6.2 Upgrades MVP

| Upgrade | Effet | Coût 💎 | Icône |
|---------|-------|---------|-------|
| **+1 Durabilité** | Flèche 4 → 5 PV | 10 | 🛡️ |
| **+1 Flèche** | 3 → 4 flèches/vague | 15 | 🏹 |
| **+1 Dégât** | 1 → 2 dégâts/touche | 20 | ⚔️ |

## 6.3 Upgrades Futur

### Catégorie Flèche 🏹

| Upgrade | Effet | Rareté | Coût 💎 |
|---------|-------|--------|---------|
| +1 Durabilité | Flèche +1 PV | ⚪ Commun | 10 |
| +1 Dégât | +1 dégât/touche | ⚪ Commun | 20 |
| Pénétration | Traverse ennemis 1 PV | 🔵 Rare | 50 |
| Rebond+ | +2 rebonds gratuits | 🔵 Rare | 40 |
| Multi-shot | Tire 2 flèches | 🟣 Épique | 100 |
| Explosion | AoE au dernier rebond | 🟣 Épique | 120 |
| Chaîne | Dégâts aux ennemis proches | 🟡 Légendaire | 200 |

### Catégorie Arc 🎯

| Upgrade | Effet | Rareté | Coût 💎 |
|---------|-------|--------|---------|
| +2 PV Arc | Arc +2 PV max | ⚪ Commun | 15 |
| Armure | -1 dégât reçu | 🔵 Rare | 60 |
| Régénération | +1 PV/vague | 🔵 Rare | 50 |
| Bouclier | Ignore 1 attaque/vague | 🟣 Épique | 100 |
| Épines | Renvoie 1 dégât | 🟡 Légendaire | 150 |

### Catégorie Économie 💎

| Upgrade | Effet | Rareté | Coût 💎 |
|---------|-------|--------|---------|
| +10% Gemmes | Drop +10% | ⚪ Commun | 25 |
| Aimant | Collecte auto gemmes | 🔵 Rare | 40 |
| Jackpot | Chance double drop | 🟣 Épique | 80 |

## 6.4 Système de raretés

| Rareté | Couleur | Chance apparition | Puissance |
|--------|---------|-------------------|-----------|
| ⚪ Commun | Gris | 60% | Faible |
| 🔵 Rare | Bleu | 25% | Moyenne |
| 🟣 Épique | Violet | 12% | Forte |
| 🟡 Légendaire | Or | 3% | Très forte |

---

# 7. Économie & Méta-progression

## 7.1 Gemmes 💎

### Sources de gemmes

| Source | Quantité | Condition |
|--------|----------|-----------|
| Ennemi faible | 1-2 💎 | Kill |
| Ennemi moyen | 3-5 💎 | Kill |
| Ennemi fort | 5-10 💎 | Kill |
| Ennemi tank | 10-15 💎 | Kill |
| Boss | 50-100 💎 | Kill |
| Bonus vague parfaite | +20% 💎 | 0 dégât reçu |
| Bonus combo | +5 💎/kill | 3+ kills 1 flèche |
| Pub rewarded | 50-100 💎 | Regarder pub |

### Utilisation des gemmes

| Usage | Coût 💎 | Quand |
|-------|---------|-------|
| Upgrades (run) | 10-200 | Entre vagues |
| Acheter arc | 500-5000 | Boutique |
| Réparer arc | 100-300 | Boutique |
| Cosmétiques | 100-500 | Boutique |

## 7.2 Boutique

### Onglet Arcs

| Arc | PV | Résistance | Bonus | Prix 💎 |
|-----|-----|------------|-------|---------|
| Basique | 20 | 10 | - | Gratuit |
| Renforcé | 25 | 15 | - | 500 |
| Élite | 30 | 20 | +5% dégâts | 1000 |
| Légendaire | 35 | 25 | +1 flèche | 2500 |
| Mythique | 40 | 30 | +10% dégâts, +1 flèche | 5000 |

### Onglet Réparation

| État arc | Coût réparation |
|----------|-----------------|
| Résistance > 50% | 100 💎 |
| Résistance 25-50% | 200 💎 |
| Résistance < 25% | 300 💎 |
| Résistance 0 (cassé) | 400 💎 |

### Onglet Cosmétiques (Futur)

| Type | Exemples | Prix 💎 |
|------|----------|---------|
| Skin Arc | Néon, Pixel, Dragon | 200-500 |
| Skin Flèche | Feu, Glace, Étoile | 100-300 |
| Trail | Arc-en-ciel, Fumée | 150-400 |
| Effet kill | Explosion, Confettis | 200-500 |

## 7.3 Système de Résistance (Futur)

```
DÉBUT RUN → Résistance = MAX (selon arc)

VAGUE 1 TERMINÉE → Résistance -1
VAGUE 2 TERMINÉE → Résistance -1
VAGUE 3 TERMINÉE → Résistance -1
...

RÉSISTANCE = 0 → Arc cassé
                 → Run terminé
                 → Doit réparer ou changer d'arc
```

**Exemple avec Arc Basique (10 résistance) :**
- Peut faire **10 vagues max** avant de casser
- Après : réparer (100-400 💎) ou acheter nouvel arc

## 7.4 Matériaux (Futur avancé)

| Matériau | Source | Utilisation | Rareté |
|----------|--------|-------------|--------|
| Fer | Drop vagues 1-5 | Craft basique | ⚪ |
| Acier | Drop vagues 5-10 | Craft renforcé | 🔵 |
| Mithril | Drop boss | Craft élite | 🟣 |
| Adamantium | Drop boss rare | Craft légendaire | 🟡 |

---

# 8. Monétisation

## 8.1 Philosophie

> **Règle d'or : PAS de Pay-to-Win**
> 
> Les joueurs F2P doivent pouvoir tout débloquer, juste plus lentement.
> L'argent achète du **temps** et du **confort**, pas de la **puissance**.

## 8.2 Sources de revenus

| Source | MVP | Futur | % Revenu estimé |
|--------|-----|-------|-----------------|
| Rewarded Ads | ✅ | ✅ | 50-60% |
| IAP Gemmes | ✅ | ✅ | 20-30% |
| Remove Ads | ✅ | ✅ | 10% |
| Cosmétiques | ❌ | ✅ | 5-10% |
| Battle Pass | ❌ | ✅ | 10-15% |

## 8.3 Publicités (Rewarded Ads)

**Principe : Le joueur CHOISIT de regarder pour une récompense**

| Moment | Récompense | Fréquence max |
|--------|------------|---------------|
| Fin de run | Doubler gemmes récoltées | 1x/run |
| Game Over | Revive (PV arc = 50%) | 1x/run |
| Entre vagues | Upgrade gratuit aléatoire | 1x/run |
| Boutique | 50 gemmes gratuites | 3x/jour |
| Écran accueil | Bonus journalier x2 | 1x/jour |

### Pubs NON intrusives

| ❌ À éviter | ✅ Acceptable |
|-------------|---------------|
| Pub forcée toutes les 30s | Pub opt-in rewarded |
| Pub avant chaque vague | Pub entre runs (skip possible) |
| Pub plein écran surprise | Banner discrète (si remove ads non acheté) |

## 8.4 Achats In-App (IAP)

### Packs de gemmes

| Pack | Prix | Gemmes | Bonus | Valeur/€ |
|------|------|--------|-------|----------|
| Petit sac | 0.99€ | 500 💎 | - | 505 💎/€ |
| Sac moyen | 2.99€ | 1800 💎 | +20% | 602 💎/€ |
| Grand sac | 4.99€ | 3500 💎 | +40% | 701 💎/€ |
| Coffre | 9.99€ | 8000 💎 | +60% | 800 💎/€ |
| Trésor | 19.99€ | 20000 💎 | +100% | 1000 💎/€ |

### Packs spéciaux

| Pack | Prix | Contenu | Limite |
|------|------|---------|--------|
| **Starter Pack** | 0.99€ | 500 💎 + Arc Renforcé | 1x/compte |
| **Premium Pack** | 4.99€ | 2000 💎 + Arc Élite + 3 cosmétiques | 1x/compte |
| **Remove Ads** | 2.99€ | Supprime toutes les pubs bannières | Permanent |

## 8.5 Battle Pass (Futur)

### Structure

| Niveau | XP requis | Récompense Free | Récompense Premium |
|--------|-----------|-----------------|-------------------|
| 1 | 0 | 50 💎 | + Skin flèche |
| 5 | 500 | 100 💎 | + 200 💎 |
| 10 | 1500 | Upgrade rare | + Skin arc |
| 15 | 3000 | 200 💎 | + 500 💎 |
| 20 | 5000 | Arc Renforcé | + Arc Exclusif |
| 25 | 7500 | 500 💎 | + Effet kill exclusif |
| 30 | 10000 | 1000 💎 | + Titre exclusif |

### Prix et durée

| Type | Prix | Durée |
|------|------|-------|
| Free Pass | Gratuit | 30 jours |
| Premium Pass | 4.99€ | 30 jours |
| Premium + 10 niveaux | 9.99€ | 30 jours |

### Gain d'XP

| Action | XP |
|--------|-----|
| Terminer 1 vague | 10 XP |
| Terminer 1 run | 50 XP |
| Kill 100 ennemis | 25 XP |
| Quête journalière | 50-100 XP |

## 8.6 Cosmétiques (Futur)

**Principe : Aucun avantage gameplay, juste visuel**

### Skins d'arc

| Skin | Apparence | Prix 💎 | Source |
|------|-----------|---------|--------|
| Néon | Brillant fluo | 200 | Boutique |
| Pixel | Style rétro | 250 | Boutique |
| Dragon | Écailles rouges | 400 | Boutique |
| Cristal | Transparent brillant | 500 | Battle Pass |
| Or | Doré luxueux | 500 | Boutique |
| Galaxie | Étoiles animées | 750 | Exclusif événement |

### Skins de flèche

| Skin | Apparence | Prix 💎 |
|------|-----------|---------|
| Feu | Flammes | 150 |
| Glace | Cristaux bleus | 150 |
| Étoile | Scintillante | 200 |
| Plasma | Énergie verte | 300 |

### Trails (traînée)

| Trail | Effet | Prix 💎 |
|-------|-------|---------|
| Fumée | Traînée grise | 150 |
| Arc-en-ciel | Multicolore | 250 |
| Sparkles | Étincelles | 200 |
| Fantôme | Images rémanentes | 350 |

### Effets de kill

| Effet | Animation | Prix 💎 |
|-------|-----------|---------|
| Explosion | Boom classique | 200 |
| Confettis | Fête | 250 |
| Pixels | Désintégration | 300 |
| Étoiles | Pop étoilé | 200 |

## 8.7 Ce qu'on NE fait PAS (Anti-P2W)

| ❌ Interdit | Pourquoi |
|-------------|----------|
| Vendre stats directement | Pay-to-win |
| Arcs achetables uniquement en € | Paywall |
| Énergie/Vies limitées | Frustrant, old school |
| Lootboxes avec stats | Gambling + P2W |
| Upgrades exclusifs payants | Avantage injuste |
| Temps de construction/cooldown | Manipulation |

---

# 9. Interface Utilisateur

## 9.1 HUD In-Game

```
┌─────────────────────────────────────────────────────────┐
│                                                         │
│  💎 125                              VAGUE 2/4          │
│                                                         │
│  ❤️❤️❤️❤️❤️❤️❤️❤️❤️❤️ Arc: 20/20 PV                     │
│                                                         │
│  ═══════════════════════════════════════════════════    │
│  ║                                                 ║    │
│  ║        👾      👾                               ║    │
│  ║                     ▓▓▓▓▓▓                     ║    │
│  ║    👾         👾           👾      👾          ║    │
│  ║                                                 ║    │
│  ║         👾              ▓▓▓▓        👾         ║    │
│  ║                                                 ║    │
│  ║                   . . . .                       ║    │
│  ║                 .                               ║    │
│  ║               .   (trajectoire)                 ║    │
│  ║             .                                   ║    │
│  ║           🏹 (Arc)                              ║    │
│  ║                                                 ║    │
│  ═══════════════════════════════════════════════════    │
│                                                         │
│  🏹 x3                              ⏸️ PAUSE            │
│  Flèches restantes                                      │
│                                                         │
└─────────────────────────────────────────────────────────┘
```

## 9.2 Écran d'Upgrade

```
┌─────────────────────────────────────────────────────────┐
│                                                         │
│              ✨ VAGUE 2 TERMINÉE ! ✨                   │
│                                                         │
│                  💎 +45 gemmes                          │
│                  🏆 Combo x3 !                          │
│                                                         │
│            ═══════════════════════════                  │
│                                                         │
│               CHOISIS UN UPGRADE                        │
│                                                         │
│   ┌─────────────┐ ┌─────────────┐ ┌─────────────┐      │
│   │     🛡️      │ │     🏹      │ │     ⚔️      │      │
│   │             │ │             │ │             │      │
│   │ +1 DURABI-  │ │ +1 FLÈCHE   │ │ +1 DÉGÂT    │      │
│   │   LITÉ      │ │             │ │             │      │
│   │             │ │             │ │             │      │
│   │  ⚪ Commun   │ │  ⚪ Commun   │ │  ⚪ Commun   │      │
│   │             │ │             │ │             │      │
│   │   10 💎     │ │   15 💎     │ │   20 💎     │      │
│   └─────────────┘ └─────────────┘ └─────────────┘      │
│                                                         │
│                  💎 125 disponibles                     │
│                                                         │
│   ┌─────────────────────────────────────────────┐      │
│   │  📺 REGARDER PUB = UPGRADE GRATUIT          │      │
│   └─────────────────────────────────────────────┘      │
│                                                         │
└─────────────────────────────────────────────────────────┘
```

## 9.3 Écran Game Over

```
┌─────────────────────────────────────────────────────────┐
│                                                         │
│                    💀 GAME OVER 💀                      │
│                                                         │
│                     Vague atteinte: 3                   │
│                                                         │
│                    💎 89 gemmes                         │
│                    👾 23 ennemis tués                   │
│                    🏹 12 flèches tirées                 │
│                                                         │
│            ═══════════════════════════                  │
│                                                         │
│   ┌─────────────────────────────────────────────┐      │
│   │  📺 REVIVRE ? (Regarder pub)                │      │
│   │     Arc restauré à 50% PV                   │      │
│   └─────────────────────────────────────────────┘      │
│                                                         │
│   ┌─────────────────────────────────────────────┐      │
│   │  📺 DOUBLER GEMMES ? (Regarder pub)         │      │
│   │     89 💎 → 178 💎                          │      │
│   └─────────────────────────────────────────────┘      │
│                                                         │
│             ┌───────────────────────┐                   │
│             │    🏠 MENU PRINCIPAL   │                   │
│             └───────────────────────┘                   │
│                                                         │
└─────────────────────────────────────────────────────────┘
```

## 9.4 Écran Boutique (Futur)

```
┌─────────────────────────────────────────────────────────┐
│                                                         │
│  ← RETOUR              🏪 BOUTIQUE         💎 1,250    │
│                                                         │
│  ┌────────┐ ┌────────┐ ┌────────┐ ┌────────┐           │
│  │  ARCS  │ │RÉPARER │ │ SKINS  │ │ GEMMES │           │
│  └────────┘ └────────┘ └────────┘ └────────┘           │
│                                                         │
│  ═══════════════════════════════════════════════════    │
│                                                         │
│   🏹 ARC RENFORCÉ           ✅ POSSÉDÉ                  │
│   PV: 25 | Résistance: 15                              │
│                                                         │
│   🏹 ARC ÉLITE              500 💎                      │
│   PV: 30 | Résistance: 20 | +5% dégâts                 │
│   [ACHETER]                                            │
│                                                         │
│   🏹 ARC LÉGENDAIRE         2,500 💎                    │
│   PV: 35 | Résistance: 25 | +1 flèche                  │
│   [ACHETER]                                            │
│                                                         │
│  ═══════════════════════════════════════════════════    │
│                                                         │
└─────────────────────────────────────────────────────────┘
```

## 9.5 Liste des écrans

| Écran | MVP | Futur |
|-------|-----|-------|
| Splash/Logo | ✅ | ✅ |
| Menu principal | ✅ Simple | ✅ Complet |
| Sélection arc | ❌ | ✅ |
| Gameplay | ✅ | ✅ |
| Pause | ✅ | ✅ |
| Upgrade (entre vagues) | ✅ | ✅ |
| Fin de vague | ✅ | ✅ |
| Game Over | ✅ | ✅ |
| Victoire | ✅ | ✅ |
| Boutique | ❌ | ✅ |
| Collection arcs | ❌ | ✅ |
| Cosmétiques | ❌ | ✅ |
| Battle Pass | ❌ | ✅ |
| Paramètres | ✅ Simple | ✅ Complet |

---

# 10. Effets Visuels (Juice/FEEL)

## 10.1 MVP

| Effet | Quand | Intensité |
|-------|-------|-----------|
| **Screenshake** | Impact flèche/ennemi | Léger |
| **Particules explosion** | Destruction ennemi | Moyen |
| **Trail flèche** | Pendant vol | Subtil |
| **Flash blanc** | Ennemi touché | Court |
| **Squash & stretch** | Arc pendant visée | Subtil |

## 10.2 Futur

| Effet | Quand | Intensité |
|-------|-------|-----------|
| **Slow-motion** | Kill final de vague | 0.3s |
| **Combo text** | Multi-kill (3+) | Pop animé |
| **Screen flash rouge** | Dégât à l'arc | Court |
| **Juice collecte** | Ramasser gemmes | Satisfaisant |
| **Impact freeze** | Gros dégâts | 0.1s |
| **Ripple effect** | Rebond sur mur | Subtil |
| **Anticipation** | Avant tir flèche | Build-up |

## 10.3 Paramètres visuels

| Option | Valeurs | Défaut |
|--------|---------|--------|
| Screenshake | Off / Low / Medium / High | Medium |
| Particules | Off / Low / High | High |
| Slow-motion | On / Off | On |

---

# 11. Audio

## 11.1 Sound Effects (SFX)

| Son | Moment | Priorité MVP |
|-----|--------|--------------|
| Swoosh | Tir flèche | ✅ |
| Boing/Thud | Rebond mur | ✅ |
| Hit | Touche ennemi | ✅ |
| Pop/Explosion | Destruction ennemi | ✅ |
| Ouch/Crack | Dégât à l'arc | ✅ |
| Ding | Collecte gemme | ✅ |
| Level up | Upgrade choisi | ✅ |
| Fanfare | Victoire vague | ⏳ |
| Sad trombone | Game over | ⏳ |
| Combo | Multi-kill | ⏳ |

## 11.2 Musique (Futur)

| Piste | Quand | Style |
|-------|-------|-------|
| Menu theme | Menu principal | Calme, mystérieux |
| Battle theme | Gameplay | Rythmé, tension |
| Boss theme | Combat boss | Épique, intense |
| Victory | Fin de run réussie | Triomphant |
| Shop theme | Boutique | Relaxant |

## 11.3 Paramètres audio

| Option | Valeurs | Défaut |
|--------|---------|--------|
| Musique | 0-100% | 70% |
| SFX | 0-100% | 100% |
| Vibration | On / Off | On |

---

# 12. Conditions de Victoire / Défaite

## 12.1 Victoire

| Condition | Récompense |
|-----------|------------|
| ✅ Terminer vague 4 (MVP) | Gemmes + écran victoire |
| ✅ Terminer run complet | Gemmes + bonus |
| ✅ Battre boss | Gemmes bonus + drop rare |

## 12.2 Défaite (Game Over)

| Condition | Peut revivre ? |
|-----------|----------------|
| ❌ Arc à 0 PV | ✅ Oui (pub) |
| ❌ 0 flèches + ennemis restants | ✅ Oui (pub) |
| ❌ Arc résistance 0 (futur) | ❌ Non (run terminé) |

## 12.3 Statistiques trackées

| Stat | Description |
|------|-------------|
| Vague max atteinte | Record personnel |
| Ennemis tués (total) | Lifetime |
| Ennemis tués (run) | Par run |
| Gemmes récoltées | Total |
| Combo max | Record |
| Runs complétés | Total |
| Temps de jeu | Total |

---

# 13. Roadmap de Développement

## Phase 1 : MVP (Semaine 1) — 30 min/jour

| Jour | Tâche | Livrable |
|------|-------|----------|
| **1** | Setup projet Unity + Arc + Swipe input | Arc visible, détecte swipe |
| **2** | Trajectoire prédictive (ligne pointillée) | Ligne suit le doigt |
| **3** | Flèche + Physique rebonds (murs) | Flèche rebondit |
| **4** | Ennemis + Collision + Durabilité flèche | Ennemis meurent, flèche s'use |
| **5** | Contre-attaque + PV Arc + Game Over | Boucle complète |
| **6** | Système vagues (4) + UI Upgrades | Progression fonctionne |
| **7** | Polish + Effets basiques + Test | MVP jouable |

### ✅ Checklist MVP

- [ ] Arc fixe en bas
- [ ] Swipe pour viser
- [ ] Ligne trajectoire prédictive
- [ ] Flèche avec rebonds
- [ ] Flèche durabilité (4 PV)
- [ ] 2 types d'ennemis (1 PV, 2 PV)
- [ ] Rebond sur ennemis
- [ ] Contre-attaque des ennemis
- [ ] PV Arc (20)
- [ ] 4 vagues
- [ ] 3 flèches/vague
- [ ] 3 upgrades basiques
- [ ] Écran upgrade entre vagues
- [ ] Game Over
- [ ] Écran victoire
- [ ] Screenshake
- [ ] Particules destruction

## Phase 2 : Polish (Semaine 2)

- [ ] Plus d'ennemis (fort, tank)
- [ ] Obstacles variés
- [ ] Plus d'upgrades (5-6)
- [ ] Effets sonores complets
- [ ] Musique placeholder
- [ ] Écran titre
- [ ] Tutoriel basique
- [ ] Équilibrage difficulté

## Phase 3 : Monétisation (Semaine 3)

- [ ] Système de gemmes
- [ ] Rewarded ads (doubler gemmes, revive)
- [ ] IAP gemmes (packs)
- [ ] Remove ads
- [ ] Bannière pub (si pas remove ads)

## Phase 4 : Méta-progression (Semaine 4)

- [ ] Boutique
- [ ] Plusieurs arcs (3-4)
- [ ] Système de résistance
- [ ] Réparation arcs
- [ ] Sauvegarde progression

## Phase 5 : Contenu (Mois 2)

- [ ] Vagues procédurales
- [ ] Système de boss
- [ ] Raretés d'upgrades
- [ ] Nouveaux types d'ennemis (5+)
- [ ] Nouveaux obstacles (4+)

## Phase 6 : Live Ops (Mois 3+)

- [ ] Battle Pass
- [ ] Cosmétiques
- [ ] Événements saisonniers
- [ ] Leaderboard
- [ ] Achievements
- [ ] Daily rewards

---

# 14. Notes Techniques

## 14.1 Architecture suggérée

```
Assets/
├── Scripts/
│   ├── Core/
│   │   ├── GameManager.cs
│   │   ├── WaveManager.cs
│   │   ├── UpgradeManager.cs
│   │   └── SaveManager.cs
│   ├── Player/
│   │   ├── BowController.cs
│   │   ├── BowHealth.cs
│   │   ├── ArrowController.cs
│   │   ├── ArrowDurability.cs
│   │   └── TrajectoryLine.cs
│   ├── Enemies/
│   │   ├── Enemy.cs
│   │   ├── EnemyHealth.cs
│   │   ├── EnemyAttack.cs
│   │   ├── EnemySpawner.cs
│   │   └── EnemyData.cs (ScriptableObject)
│   ├── Obstacles/
│   │   └── Obstacle.cs
│   ├── Input/
│   │   └── SwipeDetector.cs
│   ├── Physics/
│   │   └── BounceHandler.cs
│   ├── Economy/
│   │   ├── GemManager.cs
│   │   ├── ShopManager.cs
│   │   └── UpgradeData.cs (ScriptableObject)
│   ├── Monetization/
│   │   ├── AdsManager.cs
│   │   └── IAPManager.cs
│   ├── Effects/
│   │   ├── ScreenShake.cs
│   │   ├── ParticleManager.cs
│   │   └── TimeManager.cs
│   └── UI/
│       ├── HUDManager.cs
│       ├── UpgradeUI.cs
│       ├── ShopUI.cs
│       └── GameOverUI.cs
├── Prefabs/
│   ├── Arrow.prefab
│   ├── Enemies/
│   └── Effects/
├── ScriptableObjects/
│   ├── EnemyData/
│   ├── UpgradeData/
│   ├── ArcData/
│   └── WaveData/
├── Scenes/
│   ├── MainMenu.unity
│   ├── Game.unity
│   └── Shop.unity
├── Audio/
│   ├── SFX/
│   └── Music/
└── Art/
    ├── Sprites/
    ├── UI/
    └── Effects/
```

## 14.2 Formules clés

### Trajectoire prédictive

```csharp
// Position à l'instant t
Vector2 GetTrajectoryPoint(Vector2 startPos, Vector2 velocity, float t)
{
    return startPos + velocity * t + 0.5f * Physics2D.gravity * t * t;
}

// Générer ligne de prédiction
for (float t = 0; t < maxTime; t += step)
{
    Vector2 point = GetTrajectoryPoint(startPos, velocity, t);
    lineRenderer.SetPosition(i, point);
    
    // Check collision pour rebond
    if (Physics2D.Raycast(...)) { /* calculer nouveau rebond */ }
}
```

### Rebond

```csharp
Vector2 Reflect(Vector2 velocity, Vector2 normal)
{
    return Vector2.Reflect(velocity, normal);
}
```

### Scaling vagues

```csharp
int GetEnemyCount(int waveNumber)
{
    return 4 + Mathf.FloorToInt(waveNumber * 1.5f);
}

int GetEnemyMaxHP(int waveNumber)
{
    return 1 + Mathf.FloorToInt(waveNumber / 3f);
}
```

## 14.3 Assets recommandés

| Asset | Usage | Possédé |
|-------|-------|---------|
| **FEEL** (More Mountains) | Effets juice | ✅ |
| **All In 1 Sprite Shader** | Effets visuels sprites | ✅ |
| **DOTween** | Animations UI | ❌ (gratuit) |
| **Unity Ads** | Publicités | ❌ (gratuit) |
| **Unity IAP** | Achats in-app | ❌ (gratuit) |

## 14.4 Plateformes cibles

| Plateforme | MVP | Futur |
|------------|-----|-------|
| Android | ✅ | ✅ |
| iOS | ❌ | ✅ |
| WebGL | ❌ | ⏳ Peut-être |

## 14.5 Performance targets

| Métrique | Target |
|----------|--------|
| FPS | 60 stable |
| Load time | < 3s |
| RAM | < 200MB |
| APK size | < 50MB |

---

# 15. Annexes

## 15.1 Références visuelles

- Angry Birds (swipe mécanique)
- Bowling (1 tir = max de dégâts)
- Billard (rebonds stratégiques)
- Vampire Survivors (roguelike upgrades)
- Archero (combat + upgrades)

## 15.2 Compétition

| Jeu | Similarité | Notre différence |
|-----|------------|------------------|
| Angry Birds | Swipe to aim | Rebonds multiples + roguelike |
| Archero | Roguelike shooter | Tir manuel + rebonds |
| Bowmasters | Arc & flèche | Système de rebonds |

## 15.3 Glossaire

| Terme | Définition |
|-------|------------|
| **Run** | Une partie complète (vague 1 → game over ou victoire) |
| **Vague** | Un niveau avec X ennemis à éliminer |
| **Durabilité** | PV de la flèche |
| **Résistance** | Usure de l'arc au fil des vagues |
| **Juice/FEEL** | Effets visuels qui rendent le jeu satisfaisant |
| **Rewarded Ad** | Publicité que le joueur choisit de regarder |
| **IAP** | In-App Purchase (achat dans l'app) |
| **F2P** | Free-to-Play |
| **P2W** | Pay-to-Win (ce qu'on évite) |

---

> ⚠️ **Rappel : Ce projet est pour le FUN !**
> 
> MVP en 1 semaine, 30 min/jour.
> Fini ou pas, l'objectif est de s'amuser et d'apprendre.
> 
> La monétisation et le contenu avancé viendront APRÈS avoir un jeu fun.

---

**Document créé pour Arrow Strike**  
**Développeur : RDH**  
**Février 2026**
