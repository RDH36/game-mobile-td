# MONSTER CANNON — MVP TASK LIST

**Version :** 1.0
**Ref :** Arrow_Strike_GDD_v2.md
**Objectif :** MVP jouable en 7 jours (30 min/jour)
**Status :** MVP CORE COMPLETE ✓ (gameplay complet, polish en cours)

---

## Legende

- [ ] A faire
- [x] Fait
- **P0** = Bloquant (rien ne marche sans)
- **P1** = Core gameplay
- **P2** = Gameplay complet
- **P3** = Polish MVP

---

## JOUR 1 — Setup Projet + Cannon + Input

### 1.1 Setup projet (P0)
- [x] Configurer la scene de jeu (camera orthographique 2D, ratio mobile 9:16)
- [x] Creer les murs (4 bords ecran) avec BoxCollider2D — `GameSetup.cs` (dynamique au runtime)
- [x] Definir les layers : Player, Bullet, Enemy, Wall, Obstacle — `SetupLayers.cs` (Editor)
- [x] Creer l'architecture de dossiers (Scripts/, Prefabs/, ScriptableObjects/, Art/, Scenes/)
- [x] Configurer le New Input System pour touch/swipe — `PlayerInputActions.inputactions`

### 1.2 Cannon du joueur (P0)
- [x] Creer le GameObject Cannon (sprite placeholder, position fixe en bas) — Bow @ (0,-4,0)
- [x] Script `BowController.cs` : detecter swipe (direction + puissance)
- [x] Feedback visuel : rotation du cannon selon la direction du swipe
- [x] Script `BowHealth.cs` : PV du cannon (20 PV par defaut)

### 1.3 Input Swipe (P0)
- [x] Script `SwipeDetector.cs` avec le New Input System — InputActions inline (touch + mouse)
- [x] Detecter debut du swipe (touch down)
- [x] Detecter direction du swipe (touch drag)
- [x] Detecter fin du swipe (touch up = tir)
- [x] Calculer la puissance selon la longueur du swipe

---

## JOUR 2 — Trajectoire Predictive

### 2.1 Ligne de trajectoire (P1)
- [x] Script `TrajectoryLine.cs` avec LineRenderer
- [x] Afficher la ligne pointillee pendant le swipe
- [x] Calculer les rebonds sur les murs (Raycast2D) — montre bow → 1er rebond → direction apres rebond (4u)
- [x] Limiter la prediction — 1 rebond visible seulement (plus de skill)
- [x] Faire disparaitre la ligne au moment du tir
- [x] Bloquer les tirs vers le bas (direction.y minimum)
- [x] Mur du bas repositionne au-dessus du bow (balles ne redescendent pas)

---

## JOUR 3 — Balle + Physique des Rebonds

### 3.1 Balle (P0)
- [x] Creer le prefab Arrow (sprite jaune, Rigidbody2D gravite 0, CircleCollider2D) — `Assets/Prefabs/Arrow.prefab`
- [x] Script `ArrowController.cs` : lancement + rotation vers direction de vol
- [x] Script `ArrowDurability.cs` : durabilite de la balle (4 PV par defaut)
- [x] La balle disparait quand durabilite = 0

### 3.2 Systeme de rebonds (P0)
- [x] PhysicsMaterial2D (bounciness=1, friction=0) applique en code sur balle + murs
- [x] Rebond sur les obstacles
- [x] Rebond sur les ennemis (+ degats) — `ArrowCollisionHandler.cs`
- [x] Chaque touche ennemi = balle -1 PV de durabilite
- [x] Timer anti-boucle infinie — `ArrowLifetime.cs` (8s max)

### 3.3 Gestion des balles par vague (P1)
- [x] Limiter le nombre de balles par vague (3 par defaut) — `ArrowManager.cs`
- [x] Bloquer le tir quand plus de balles disponibles
- [x] Attendre la fin de la balle en cours avant de pouvoir tirer la suivante

---

## JOUR 4 — Ennemis + Collisions

### 4.1 Ennemis (P0)
- [x] Creer le ScriptableObject `EnemyData.cs` (PV, degats, sprite, drop gemmes)
- [x] Configurer EnemyData pour "Faible" (1 PV, 1 degat, vert)
- [x] Configurer EnemyData pour "Moyen" (2 PV, 2 degats, bleu)
- [x] Creer le prefab Enemy (sprite, collider, Rigidbody2D kinematic)
- [x] Script `EnemyHealth.cs` : gerer les PV, mourir quand PV = 0
- [x] Script `Enemy.cs` : initialiser depuis EnemyData

### 4.2 Collisions balle/ennemi (P0)
- [x] Detecter la collision balle → ennemi
- [x] Appliquer les degats a l'ennemi (degats de la balle)
- [x] Reduire la durabilite de la balle (-1)
- [x] Faire rebondir la balle apres impact — `EnemyBounce.cs` (PhysicsMaterial2D en code)
- [x] Effet visuel : flash blanc sur l'ennemi touche — `EnemyFlash.cs`

### 4.3 Spawner d'ennemis (P1)
- [x] Script `EnemySpawner.cs` : placer les ennemis dans la zone de jeu
- [x] Positions aleatoires dans la moitie superieure de l'ecran
- [x] Eviter le chevauchement entre ennemis (espacement min 1.2)
- [x] Eviter de spawn sur les obstacles

---

## JOUR 5 — Contre-attaque + PV Arc + Game Over

### 5.1 Contre-attaque des ennemis (P1)
- [x] Script `EnemyCounterAttack.cs` : apres chaque tir du joueur, les ennemis survivants attaquent
- [x] Chaque ennemi survivant inflige ses degats au cannon
- [x] Animation/feedback visuel de la contre-attaque — `EnemyProjectile.cs` (projectile orange-rouge vole de l'ennemi vers le cannon)
- [x] Screen flash rouge quand le cannon prend des degats — `ScreenFlash.cs`

### 5.2 PV du cannon (P1)
- [x] Afficher les PV du cannon dans le HUD (DebugHUD pour l'instant)
- [x] Reduire les PV quand les ennemis contre-attaquent
- [x] Feedback visuel quand le cannon perd des PV (screenshake via Feel MMCameraShaker) — `CameraShake.cs`

### 5.3 Conditions de fin (P0)
- [x] Game Over quand cannon PV = 0
- [x] Game Over quand 0 balles restantes ET ennemis encore vivants
- [x] Victoire quand tous les ennemis de la vague sont elimines → `GameState.WaveComplete`
- [x] Script `GameManager.cs` : gerer les etats du jeu (Playing, WaveComplete, GameOver, Victory) — deja fait Jour 1

---

## JOUR 6 — Systeme de Vagues + Upgrades

### 6.1 Systeme de vagues (P1)
- [x] Script `WaveManager.cs` : gerer la progression des vagues
- [x] Creer ScriptableObject `WaveData.cs` (nombre ennemis, types, PV) — avec `WaveEntry` (enemyData + count)
- [x] Configurer 4 vagues MVP :
  - [x] Vague 1 "Scouts" : 5 faibles
  - [x] Vague 2 "Patrol" : 5 faibles + 1 moyen
  - [x] Vague 3 "Assault" : 3 faibles + 4 moyens
  - [x] Vague 4 "Final Stand" : 2 faibles + 5 moyens + 2 forts
- [x] Restaurer les PV de l'arc a 100% entre chaque vague
- [x] Restaurer les fleches entre chaque vague
- [x] Detruire les fleches en vol entre les vagues (fix bug)
- [x] Fix AliveCount (check IsDead, pas juste null)
- [x] Countdown timer entre les vagues (affiche dans DebugHUD)

### 6.2 Systeme d'upgrades (P1)
- [x] Script `UpgradeManager.cs` : gerer les upgrades du run
- [x] Creer ScriptableObject `UpgradeData.cs` (nom, description, effet, cout)
- [x] Configurer 3 upgrades MVP :
  - [x] +1 Durabilite balle (cout : 10 gemmes)
  - [x] +1 Balle par vague (cout : 15 gemmes)
  - [x] +1 Degat par touche (cout : 20 gemmes)
- [x] Proposer 3 upgrades aleatoires entre chaque vague
- [x] Appliquer l'upgrade choisi aux stats du joueur
- [x] Les upgrades s'accumulent pendant le run et reset au prochain run
- [x] GameState.UpgradeSelection ajoute au flow
- [x] UI selection d'upgrade dans DebugHUD (OnGUI + Pointer input mobile)

### 6.3 Systeme de gemmes (P1)
- [x] Script `GemManager.cs` : gerer les gemmes du run
- [x] Les ennemis drop des gemmes a leur mort (Faible: 1-2, Moyen: 3-5, Fort: 5-10)
- [x] Bonus combo : +5 gemmes si 3+ kills avec 1 fleche
- [x] Bonus vague parfaite : +20% si 0 degat recu dans la vague
- [x] Afficher les gemmes dans le HUD

---

## JOUR 7 — UI + Polish + Effets

### 7.1 HUD In-Game (P1)
- [x] Afficher les PV du cannon (barre de vie en haut)
- [x] Afficher le nombre de balles restantes (en bas a gauche)
- [x] Afficher la vague actuelle (ex: "VAGUE 2/4" en haut a droite)
- [x] Afficher les gemmes collectees (en haut a gauche)
- [x] Afficher les buffs actifs pres du cannon (PV, Balle, Degat)
- [x] Banner "VAGUE X TERMINEE !" (3s pause avant upgrades)
- [x] Bouton Pause

### 7.2 Ecran Upgrade entre vagues (P2)
- [x] UI : "VAGUE X TERMINEE !"
- [x] UI : gemmes recoltees dans la vague
- [x] UI : 3 cartes d'upgrades a choisir
- [x] UI : cout en gemmes de chaque upgrade
- [x] UI : gemmes disponibles du joueur
- [x] Transition vers la vague suivante apres selection
- [x] Canvas dans la scene (editable dans l'editeur sans Play)

### 7.3 Ecran Game Over (P2)
- [x] UI : "GAME OVER"
- [x] UI : vague atteinte
- [x] UI : gemmes recoltees (total du run)
- [x] UI : ennemis tues (total du run)
- [x] UI : fleches tirees (total du run)
- [x] Bouton "Menu Principal" / "Rejouer"

### 7.4 Ecran Victoire (P2)
- [x] UI : "VICTOIRE !" apres la vague 4
- [x] UI : stats du run (gemmes, kills, fleches)
- [x] Bouton "Menu Principal" / "Rejouer"

### 7.5 Menu Principal (P2)
- [x] UI : titre du jeu "Monster Cannon"
- [x] Bouton "JOUER"
- [x] Scene separee ou overlay (overlay Canvas, sort order 30)

### 7.6 Ecran Pause (P2)
- [x] UI : overlay de pause
- [x] Bouton "Reprendre"
- [x] Bouton "Quitter" (retour menu)
- [x] Time.timeScale = 0 en pause

### 7.7 Effets visuels / Juice (P3)
- [x] Screenshake a l'impact balle/ennemi — `CameraShake.cs` via Feel MMCameraShakeEvent
- [x] Particules d'explosion a la destruction d'un ennemi — `deathFXPrefab` dans EnemySpawner
- [x] Trail de la balle pendant le vol (TrailRenderer) — dans `ArrowController.cs`
- [x] Flash blanc quand un ennemi est touche — `EnemyFlash.cs`
- [ ] Squash & stretch du cannon pendant la visee
- [x] Barre de vie ennemis (sprite-based) — `EnemyHealthBar.cs`
- [x] Visual upgrade fleche (AllIn1SpriteShader) — `ArrowVisualUpgrade.cs`
- [x] Popup combo "COMBO xN!" avec Feel spring — `ComboPopup.cs`
- [x] Effet coin collect avec Feel spring — `CoinCollectFX.cs`
- [x] Damage popup "-X" avec Feel spring — `DamagePopup.cs`
- [x] Screen flash rouge (degats cannon) + gold (combo) — `ScreenFlash.cs`
- [x] Feel spring animations sur UI panels + HUD bumps
- [x] Police Showpop sur tous les popups (DamagePopup, ComboPopup, CoinCollectFX)
- [x] Fix boucle rebond infini — perturbation angle ±3° + timeout 5s

### 7.8 Audio SFX placeholder (P3)
- [ ] Son de tir balle (swoosh)
- [ ] Son de rebond mur (boing/thud)
- [ ] Son d'impact ennemi (hit)
- [ ] Son de destruction ennemi (pop/explosion)
- [ ] Son de degat au cannon (ouch/crack)
- [ ] Son de collecte gemme (ding)
- [ ] Son d'upgrade choisi (level up)

---

## RESUME MVP

| Categorie | Taches | Priorite | Status |
|-----------|--------|----------|--------|
| Setup + Cannon + Input | 14 | P0 | ✓ DONE |
| Trajectoire | 5 | P1 | ✓ DONE |
| Balle + Rebonds | 11 | P0 | ✓ DONE |
| Ennemis + Collisions | 11 | P0-P1 | ✓ DONE |
| Contre-attaque + Game Over | 10 | P0-P1 | ✓ DONE |
| Vagues + Upgrades + Gemmes | 20 | P1 | ✓ DONE |
| UI + Polish | 39 | P1-P3 | 32/39 (P3 SFX restant) |
| **TOTAL** | **~110 taches** | | **97% COMPLETE** |

## Scripts a creer

| Script | Dossier | Responsabilite |
|--------|---------|----------------|
| `GameManager.cs` | Scripts/Core/ | Etats du jeu, flow global |
| `WaveManager.cs` | Scripts/Core/ | Progression des vagues |
| `UpgradeManager.cs` | Scripts/Core/ | Gestion des upgrades du run |
| `GemManager.cs` | Scripts/Economy/ | Gestion des gemmes |
| `BowController.cs` | Scripts/Player/ | Visee + tir |
| `BowHealth.cs` | Scripts/Player/ | PV de l'arc |
| `ArrowController.cs` | Scripts/Player/ | Mouvement de la fleche |
| `ArrowDurability.cs` | Scripts/Player/ | Durabilite de la fleche |
| `TrajectoryLine.cs` | Scripts/Player/ | Ligne de trajectoire predictive |
| `SwipeDetector.cs` | Scripts/Input/ | Detection du swipe (New Input System) |
| `BounceHandler.cs` | Scripts/Physics/ | Gestion des rebonds |
| `Enemy.cs` | Scripts/Enemies/ | Initialisation ennemi |
| `EnemyHealth.cs` | Scripts/Enemies/ | PV des ennemis |
| `EnemyAttack.cs` | Scripts/Enemies/ | Contre-attaque |
| `EnemySpawner.cs` | Scripts/Enemies/ | Placement des ennemis |
| `ScreenShake.cs` | Scripts/Effects/ | Effet screenshake |
| `HUDManager.cs` | Scripts/UI/ | HUD in-game |
| `UpgradeUI.cs` | Scripts/UI/ | Ecran selection upgrade |
| `GameOverUI.cs` | Scripts/UI/ | Ecran game over + victoire |

## ScriptableObjects a creer

| SO | Dossier | Donnees |
|----|---------|---------|
| `EnemyData.cs` | ScriptableObjects/EnemyData/ | PV, degats, sprite, drop gemmes |
| `WaveData.cs` | ScriptableObjects/WaveData/ | Nombre ennemis, types, difficulte |
| `UpgradeData.cs` | ScriptableObjects/UpgradeData/ | Nom, effet, cout, icone |

## Prefabs a creer

| Prefab | Dossier | Composants |
|--------|---------|------------|
| `Bow.prefab` | Prefabs/ | SpriteRenderer, BowController, BowHealth |
| `Arrow.prefab` | Prefabs/ | SpriteRenderer, Rigidbody2D, CircleCollider2D, ArrowController, ArrowDurability, BounceHandler, TrailRenderer |
| `Enemy_Weak.prefab` | Prefabs/Enemies/ | SpriteRenderer, Collider2D, Rigidbody2D (kinematic), Enemy, EnemyHealth, EnemyAttack |
| `Enemy_Medium.prefab` | Prefabs/Enemies/ | Idem, stats differentes |
| `Wall.prefab` | Prefabs/ | BoxCollider2D (bords ecran) |
