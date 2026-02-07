# MONSTER CANNON — POLISH TASK LIST

**Ancien nom :** Arrow Strike
**Nouveau nom :** Monster Cannon
**Assets :** `Assets/Art/Sprites/Png/`
**Objectif :** Remplacer tous les placeholders par les vrais sprites, ajouter animations + FX + UI

---

## Legende

- [ ] A faire
- [x] Fait
- **P1** = Indispensable (le jeu a l'air fini)
- **P2** = Nice to have (meilleure experience)
- **P3** = Bonus (wow factor)

---

## PHASE 1 — Sprites de base (P1)

### 1.1 Renommer le jeu
- [x] Titre Main Menu : "ARROW STRIKE" → "MONSTER CANNON"
- [x] Mettre a jour les textes UI (references a "fleche" → "tir")
- [x] Mettre a jour le GDD et les docs (GDD + MVP Tasks renommes Monster Cannon)

### 1.2 Cannon (remplace le Bow)
- [x] Sprite Gun01 Idle comme sprite du cannon (remplace le placeholder jaune)
- [x] Configurer le SpriteRenderer du Bow avec le sprite Gun01
- [x] Ajuster la taille/position du cannon dans la scene (scale 0.75)
- [x] Le cannon pointe vers la direction du swipe (rotation -90, inchange)

### 1.3 Bullet (remplace la fleche/Arrow)
- [x] Sprite Bullet (1.png) comme projectile
- [x] Remplacer le sprite jaune du prefab Arrow par le sprite bullet
- [x] Ajuster la taille du collider au nouveau sprite (radius 0.3)
- [x] Ajuster la rotation du bullet dans la direction de vol (inchange, -90)

### 1.4 Monstres (remplace les ennemis placeholder)
- [x] Monster01 (vert) → Ennemi Faible (1 PV)
- [x] Monster03 (bleu) → Ennemi Moyen (2 PV)
- [x] Monster05 (violet) → Ennemi Fort (3 PV)
- [x] Ajuster les colliders aux sprites des monstres (radius 0.7)
- [x] Ajuster les tailles/positions dans le spawner (scale 0.7)
- [x] Ajout champ `sprite` dans EnemyData + Enemy.Init applique le sprite

### 1.5 Background / Gameplay Area
- [x] Utiliser "game area.png" comme fond de jeu (12x23.7 units, sortOrder -100)
- [x] ~~Utiliser "wall.png" pour les murs~~ — supprime, murs invisibles (colliders only)
- [ ] Utiliser "top gradient.png" comme overlay en haut

---

## PHASE 2 — Animations (P1)

### 2.1 Animation du Cannon
- [x] Cannon_Idle.anim (Gun01/Idle 1 frame, looping) — `Assets/Animations/Cannon/`
- [x] Cannon_Shoot.anim (Gun01/Shoot 10 frames @ 20fps, one-shot) — `Assets/Animations/Cannon/`
- [x] Cannon.controller (Idle → Shoot via Trigger → retour Idle) — assigne au Bow
- [x] ArrowManager.PlayCannonShootAnimation() declenche le trigger "Shoot"

### 2.2 Animations des Monstres
- [x] Monster01_Idle.anim (20 frames @ 12fps, looping) — `Assets/Animations/Monsters/`
- [x] Monster03_Idle.anim (20 frames @ 12fps, looping) — `Assets/Animations/Monsters/`
- [x] Monster05_Idle.anim (20 frames @ 12fps, looping) — `Assets/Animations/Monsters/`
- [x] AnimatorController par monstre assigne via EnemyData.animController
- [x] Enemy.Init() ajoute un Animator component avec le controller

### 2.3 Effets visuels animes
- [x] ShootFX.anim + ShootFX.controller (15 frames @ 24fps) — `Assets/Animations/FX/`
- [x] DeathFX.anim + DeathFX.controller (20 frames @ 20fps, scale = enemy) — `Assets/Animations/FX/`
- [x] Prefabs ShootFX + DeathFX avec Animator + DestroyAfterAnimation — `Assets/Prefabs/Effects/`
- [x] SetupAnimations.cs editor script cree tous les assets .anim/.controller

---

## PHASE 3 — UI avec les vrais assets (P2)

### 3.1 UI Sprites
- [x] HP bar cannon avec sprites `enemy hp bar bg/fg.png` (HUD)
- [x] Landing screen / Main Menu avec `cover background.png` + `game title.png` + `start game btn.png`
- [x] Dark overlays (GameOver, Victory, Pause, Upgrade) avec `dark background.png`
- [x] Upgrade popup boutons avec `btn05.png` / `btn05 pressed.png`
- [x] Gameplay area UI — money bar, wave bg, arrow bg avec `money bar.png`

### 3.2 Theme UI
- [x] Boutons avec sprites du pack + SpriteSwap transition (normal/pressed)
- [x] Couleurs Image = white pour afficher les sprites dans leurs couleurs originales
- [x] Font ARCO SDF appliquee manuellement a tous les textes UI
- [ ] Texte stylise (ombres, outlines) — a faire si necessaire

### 3.3 Bugfixes
- [x] HP bar fill ne change plus de couleur (garde le sprite original, pas de tint vert/rouge)
- [x] HP fill padding (6px H, 4px V) pour rester dans le cadre du bg sprite
- [x] Spawn adaptatif — espacement reduit dynamiquement selon le nombre d'ennemis (fix chevauchement wave 4)

---

## PHASE 4 — Juice & Polish (P2-P3)

### 4.1 Effets visuels (P2)
- [x] Screenshake a l'impact bullet/monstre (CameraShake via Feel — 0.12f hit, 0.3f kill)
- [x] Screenshake augmente quand le joueur recoit des degats (0.25f + dmg*0.1f)
- [x] Knockback ennemi quand touche mais pas tue (0.3f push arriere)
- [x] Trail du bullet pendant le vol (TrailRenderer jaune-orange dans ArrowController.Awake)
- [x] Flash blanc quand un monstre est touche (EnemyFlash.cs — overbright Color(10,10,10))
- [x] Squash & stretch du cannon pendant la visee (BowController — elastic snap-back au tir)

### 4.2 Audio SFX (P3)
- [ ] Son de tir cannon (boom)
- [ ] Son de rebond mur (thud)
- [ ] Son d'impact monstre (hit)
- [ ] Son de destruction monstre (pop/explosion)
- [ ] Son de degat au cannon (crack)
- [ ] Son de collecte gemme (ding)
- [ ] Son d'upgrade (level up)

---

## Assets disponibles

| Categorie | Dossier | Contenu |
|-----------|---------|---------|
| Canons | `Png/Guns/Gun01-10/` | 10 canons avec Idle + Shoot frames |
| Bullets | `Png/Bullets/1-4.png` | 4 types de projectiles |
| Monstres | `Png/Monster/Monster01-10/` | 10 monstres avec ~20 frames anim |
| Shoot FX | `Png/Shoot fx/` | 15 frames animation muzzle flash |
| Death FX | `Png/Dead fx/` | 20 frames animation explosion |
| Background | `Png/Gameplay Area/` | game area, wall, top gradient |
| UI | `Png/User interfaces/` | HP bar, popups, landing, wave cleared |
| Shield | `Png/Guns/Shield.png` | Bouclier (bonus?) |

## Mapping Monstre → Difficulte

| Monster | Couleur | Role | PV |
|---------|---------|------|-----|
| Monster01 | Vert | Faible | 1 |
| Monster03 | Bleu | Moyen | 2 |
| Monster05 | Violet | Fort | 3 |
| Monster02,04,06-10 | Varies | Reserve pour futur | - |
