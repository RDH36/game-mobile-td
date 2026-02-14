# MONSTER CANNON — V1 TASK LIST (Google Play Launch)

**Version :** 1.0
**Ref :** Monster_Cannon_GDD_v3.1.md
**Objectif :** Publication V1 sur Google Play
**Base :** MVP 98% complete (gameplay core, UI, audio, VFX)

---

## Legende

- [ ] A faire
- [x] Fait (herite du MVP)
- **P0** = Bloquant (le jeu ne peut pas etre publie sans)
- **P1** = Core V1 (experience complete)
- **P2** = Important (retention, monetisation)
- **P3** = Polish (qualite publication)

---

## 1. VAGUES INFINIES + SCALING (P0) ✅

> ~~Actuellement : 4 vagues fixes via WaveData ScriptableObjects~~
> Fait : Vagues 1-4 fixes (SO), puis generation infinie avec scaling

### 1.1 Systeme de generation infinie
- [x] Refactorer `WaveManager.cs` : hybride SO (1-4) + generation infinie (5+)
- [x] Implementer `InfiniteWaveGenerator.cs` : formules de scaling
  - `GetEnemyCount(wave)` = min(6 + wave*0.4, 25) — courbe douce, cap 25
  - `GetHPMultiplier(wave)` = 1f + (wave * 0.08f)
  - `GetCoinMultiplier(wave)` = 1f + (wave * 0.1f)
- [x] Generer la composition d'ennemis par type selon la vague :
  - Vague 1-14 : Weak + Medium
  - Vague 15-24 : + Strong (20%→35%)
  - Vague 25+ : + Elite (15%→30%)
- [x] Garder les 4 WaveData SO comme waves 1-4, puis generation infinie
- [x] Waves fixes reequilibrees (Wave 4 : 2 Weak + 5 Medium, pas de Strong)
- [ ] Tester le scaling jusqu'a vague 100 (equilibrage)

### 1.2 Nouveaux types de monstres
- [x] EnemyData_Weak (Blob Vert) : 1 HP, 1 dmg, 1-2 coins
- [x] EnemyData_Medium (Blob Bleu) : 2 HP, 2 dmg, 2-4 coins
- [x] EnemyData_Strong (Blob Rouge) : 3 HP, 3 dmg, 4-6 coins — mis a jour
- [x] EnemyData_Elite (Blob Violet) : 5 HP, 4 dmg, 8-12 coins — cree
- [x] Sprites assignes via VisualVariant[] (pas de prefabs separes, 1 seul Enemy.prefab)
- [x] `EnemySpawner.cs` supporte les 4 types + multipliers HP/coin
- [x] Tous les EnemyData deplaces dans `Resources/EnemyData/`

### 1.3 Variete visuelle des monstres
> Sprites disponibles : Monster01-10 (Png + Spine) — 10 visuels differents

- [x] Modifier `EnemyData.cs` : champ `VisualVariant[]` pour variantes aleatoires
- [x] Modifier `Enemy.cs` : `Init()` pioche un variant aleatoire au spawn
- [x] Creer AnimatorControllers pour Monster02, 04, 06, 07, 08, 09, 10 (editor script)
- [x] Assigner les variants aux EnemyData :
  - Weak : Monster01 (vert) + Monster06 (rose)
  - Medium : Monster03 (cyan) + Monster07 (bleu 3 yeux) + Monster09 (orange)
  - Strong : Monster05 (violet piques) + Monster04 (violet raye)
  - Elite : Monster02 (bleu piquant) + Monster08 (rose raye) + Monster10 (vert fonce)

### 1.4 Scaling HP monstres
- [x] Multiplier HP applique aux ennemis selon la vague (`Enemy.Init(data, hpMult, coinMult)`)
- [x] Coins drops augmentent aussi avec le multiplier
- [x] HUD affiche "VAGUE {n}" (plus de total, mode infini)

---

## 2. BOSS SYSTEM (P0) ✅

> ~~Actuellement : Aucun boss~~
> Fait : 5 boss aux vagues 10, 20, 30, 40, 50 avec patterns uniques

### 2.1 Systeme de boss
> Sprites disponibles : Bos01-05 (Png animes, 25 frames chacun)

- [x] Creer `BossData.cs` (ScriptableObject) : nom, HP, pattern, mecanique introduite, skin reward
- [x] Creer `Boss.cs` (MonoBehaviour) : logique de boss generique (phases, patterns)
- [x] Adapter `EnemySpawner.cs` pour les vagues boss (SpawnBoss method)
- [x] Detecter vague boss dans `WaveManager.cs` (Dictionary bossByWave)
- [x] Boss seul dans sa vague (pas d'ennemis normaux)
- [x] UI "BOSS!" annonce au debut de la vague boss (HUDManager + Feel spring)
- [x] Musique boss differente (SFXManager crossfade)
- [x] Creer AnimatorControllers pour Bos01-05 (editor script, 25 frames @ 12fps)
- [x] Shield bloque les degats dans ArrowCollisionHandler (TryBlockDamage)

### 2.2 Boss Vague 10 — Blob King (Obstacles)
- [x] Creer BossData_BlobKing : 15 HP, pattern Obstacle
- [x] Sprite Bos01, anim controller assigne
- [x] Pattern IA : spawn 3 blocs obstacles (Wall layer, bouncy material)
- [x] Skin Bronze deblocable (skinRewardId configure)

### 2.3 Boss Vague 20 — Speedy (Deplacement)
- [x] Creer BossData_Speedy : 25 HP, pattern Speed
- [x] Sprite Bos02, anim controller assigne
- [x] Pattern IA : mouvement horizontal rapide, rebond aux bords
- [x] Skin Silver deblocable

### 2.4 Boss Vague 30 — Guardian (Bouclier)
- [x] Creer BossData_Guardian : 35 HP, pattern Shield
- [x] Sprite Bos03, anim controller assigne
- [x] Pattern IA : cycle 3s shield/3s vulnerable, sprite cercle procedural, pulse
- [x] Skin Gold deblocable

### 2.5 Boss Vague 40 — Splitter (Division)
- [x] Creer BossData_Splitter : 45 HP, pattern Split
- [x] Sprite Bos04, anim controller assigne
- [x] Pattern IA : a 50% HP, spawn 2 mini-bosses (half HP, half dmg)
- [x] Skin Diamond deblocable

### 2.6 Boss Vague 50 — Overlord (Consolidation)
- [x] Creer BossData_Overlord : 60 HP, pattern Overlord
- [x] Sprite Bos05, anim controller assigne
- [x] Pattern IA : mouvement (0.7x) + bouclier + division a 50%
- [x] Skin Legendary deblocable

---

## 3. MECANIQUES DE GAMEPLAY (P0)

> Actuellement : Gameplay de base (rebonds + traverse monstres)
> Cible : 5 mecaniques progressives deverrouillees par les boss

### 3.1 Systeme de mecaniques
- [ ] Creer `MechanicManager.cs` : tracker les mecaniques deblocees
- [ ] Debloquer une mecanique a chaque boss vaincu
- [ ] Les mecaniques se cumulent dans les vagues normales apres deblocage
- [ ] Pourcentage progressif d'ennemis avec mecanique (voir tableau GDD 6.3)

### 3.2 Obstacles — Boules orbitales (Vague 10+) ✅
> Adapte du pattern boss BlobKing : boule orbitale sur ennemis normaux (one-shot)
- [x] Creer `EnemyOrbitBall.cs` : boule qui orbite autour d'un ennemi normal
- [x] Auto-lancement apres 2-5s (aleatoire) vers le bow, inflige 1 degat
- [x] Sprite bullet aleatoire (Resources/Sprites/Bullets), CircleCollider2D layer Wall
- [x] Probabilite progressive : `GetOrbitBallChance(wave)` dans `InfiniteWaveGenerator.cs`
  - W11=8%, W15=40%, W20=80%, W23+=100%
- [x] Integration dans `EnemySpawner.SpawnWave()` : ajout composant selon probabilite

### 3.3 Deplacement (Vague 20+)
- [ ] Creer `MovementBehavior.cs` : comportement de deplacement horizontal
- [ ] Ajouter sur un % d'ennemis (20% vague 20 → 50% vague 40+)
- [ ] Vitesse de base lente, augmente avec les vagues
- [ ] Rebond aux bords de la zone de jeu
- [ ] Indicateur visuel (trail ou fleche directionnelle)

### 3.4 Bouclier (Vague 30+)
- [ ] Creer `ShieldBehavior.cs` : cote invulnerable (gauche ou droite aleatoire)
- [ ] Visuel demi-cercle brillant du cote protege
- [ ] Les balles rebondissent sur le bouclier sans faire de degats
- [ ] 15% vague 30 → 30% vague 50+
- [ ] Feedback visuel quand la balle touche le bouclier (etincelles)

### 3.5 Division (Vague 40+)
- [ ] Creer `SplitBehavior.cs` : a 50% HP, l'ennemi se divise en 2 petits
- [ ] Petits : 25% HP parent, 50% dmg parent
- [ ] Animation de division (sprite scale down + spawn 2)
- [ ] 10% vague 40 → 25% vague 60+
- [ ] Les petits peuvent avoir d'autres mecaniques (bouclier, deplacement)

---

## 4. UPGRADES COMPLETS (P1)

> Actuellement : 3 upgrades (Durabilite, ArrowCount, Damage)
> Cible : 11 upgrades (3 offensifs, 3 defensifs, 3 utilitaires, 2 economiques)
> Monnaie : les upgrades coutent des **COINS** (🪙 monnaie in-run)

### 4.1 Nouveaux upgrades offensifs
- [x] Pointe Aceree : +1 degat (existe deja comme Upgrade_Damage)
- [ ] Critique : 20% chance x2 degats — `CriticalUpgrade` logic dans `ArrowCollisionHandler.cs`
- [ ] Explosion : degats de zone petit rayon — `ExplosionUpgrade` + VFX cercle d'impact

### 4.2 Nouveaux upgrades defensifs
- [ ] Canon Blinde : +5 HP max — modifier `BowHealth.cs` maxHP
- [ ] Regeneration : +2 HP par vague — ajouter heal dans `WaveManager.cs` NextWaveSequence
- [ ] Vampirisme : +1 HP par kill — listener dans `EnemyHealth.cs` OnDeath

### 4.3 Nouveaux upgrades utilitaires
- [x] Carquois Elargi : +1 balle/vague (existe deja comme Upgrade_ArrowCount)
- [x] Boulet Robuste : +1 traverse (existe deja comme Upgrade_Durability)
- [ ] Vision : +2 rebonds prediction dans `TrajectoryLine.cs`
> Note : pas de MagnetUpgrade — les coins sont deja auto-collectes

### 4.4 Nouveaux upgrades economiques
- [ ] Prospecteur : +25% coins drop — modifier calcul dans drop monstres
- [ ] Chanceux : +10% chance drop rare — bonus coins calculation

### 4.5 Scaling des prix d'upgrade ✅
- [x] Les prix d'upgrade augmentent a certains paliers de vague :
  - Vague 1-9 : prix de base (x1)
  - Vague 10-19 : prix x1.5
  - Vague 20-29 : prix x2
  - Vague 30-39 : prix x3
  - Vague 40+ : prix x4
- [x] `UpgradeManager.cs` : `GetScaledCost()` + `GetPriceMultiplier(wave)` statique
- [x] `UpgradeScreenUI.cs` : affiche le prix scale, verifie affordability sur prix scale

### 4.6 Configuration des upgrades
- [ ] Creer 8 nouveaux UpgradeData ScriptableObjects (un par nouvel upgrade)
- [ ] Icones pour chaque upgrade (sprites simples)
- [ ] Equilibrer les couts de base en coins (voir GDD 7.2)
- [ ] Adapter `UpgradeScreenUI.cs` pour afficher les icones

---

## 5. TUTORIEL (P0) ✅

> ~~Actuellement : Aucun tutoriel~~
> Fait : 7 etapes interactives, pause a chaque etape, UI Canvas programmatique

### 5.1 Systeme de tutoriel
- [x] Creer `TutorialManager.cs` (singleton) : gere les 7 etapes avec enum TutorialStep
- [x] UI overlay semi-transparent (65% noir) avec panel message, texte bold + outline
- [x] Premiere fois seulement via `SaveManager.TutorialDone` (PlayerPrefs)
- [x] Non-skippable (premiere fois), lance au `GameManager.StartGame()`
- [x] Canvas sortOrder 50 (au-dessus de tout), CanvasScaler 1080x1920

### 5.2 Les 7 etapes
- [x] Etape 1 — Viser : "Glisse pour viser !" — pause + tap to continue
- [x] Etape 2 — Trajectoire : "La ligne montre la trajectoire" — auto 2s (WaitForSecondsRealtime)
- [x] Etape 3 — Tirer : "Relache pour tirer !" — tap to continue, puis unpause
- [x] Etape 4 — Rebonds : cache, attend ArrowCollisionHandler.OnWallBounce → pause + affiche 2s
- [x] Etape 5 — Contre-attaque : cache, attend BowHealth.OnDamageTaken → pause + affiche 2.5s
- [x] Etape 6 — Explication UI : pause + bouton "COMPRIS" (vert style)
- [x] Etape 7 — Upgrades : cache, attend UpgradeManager.OnUpgradesOffered → affiche 3s (pas de pause)

### 5.3 Integration gameplay
- [x] Event `OnAimStarted` dans `BowController.cs` (+ tracking `_aimFired`)
- [x] Event static `OnWallBounce` dans `ArrowCollisionHandler.cs`
- [x] Event `OnDamageTaken` dans `BowHealth.cs` (existait deja)
- [x] Event `OnUpgradesOffered` dans `UpgradeManager.cs` (existait deja)
- [x] Lancement tutorial dans `GameManager.StartGame()` (AddComponent si TutorialDone=false)

### 5.4 Localisation FR/EN
- [x] Textes FR pour les 7 etapes (GetText helper)
- [x] Textes EN pour les 7 etapes
- [x] Selection langue basee sur `SaveManager.Language` ("FR"/"EN")
- [x] Rich text pour etape UI (couleurs HP rouge, Coins or, Fleches bleu)

---

## 6. ECONOMIE : COINS + GEMS (P0) ✅

> ~~Actuellement : `GemManager.cs` gere la monnaie in-run~~
> Fait : `CoinManager.cs` (in-run), `SaveManager` gere gems permanents

### 6.1 Renommer GemManager → CoinManager (in-run)
- [x] Renommer `GemManager.cs` → `CoinManager.cs` (COINS in-run)
- [x] Renommer toutes les refs "gem" → "coin" : HUD, UpgradeScreen, VictoryScreen, GameOver, Effects
- [x] Coins reset au debut de chaque run (CoinManager.ResetRun())
- [x] Coin multiplier par vague deja implemente (InfiniteWaveGenerator.GetCoinMultiplier)

### 6.2 Systeme de gems permanent
- [x] `SaveManager.cs` gere les gems permanents (AddGems, SpendGems, TotalGems via PlayerPrefs)
- [x] Gains de gems en fin de run — premium, petits drops :
  - Vague 1-9 : 1 gem, 10-19 : 2, 20-29 : 3, 30-39 : 5, 40+ : 8
- [ ] Gains de gems : achievements, daily login (sections 9 et 10)
- [ ] Afficher le total gems dans le menu principal (section 15)

### 6.3 Integration UI
- [ ] Menu principal : afficher gems (💎) en haut (section 15)
- [x] HUD gameplay : affiche coins (🪙) in-run
- [x] Upgrade screen : couts en coins (🪙)
- [ ] Skin shop : prix en gems (💎) (section 8)
- [x] Game Over : affiche "Gems gagnes: +X" (bleu)
- [ ] Game Over : bouton x2 gems (rewarded ad, section 11)

---

## 7. SAUVEGARDE (P0) ✅

> ~~Actuellement : Aucun systeme de sauvegarde centralise~~
> Fait : SaveManager singleton avec PlayerPrefs, high score sur Game Over

### 7.1 SaveManager
- [x] Creer `SaveManager.cs` (singleton) : centraliser toutes les sauvegardes
- [x] Sauvegarder : TotalGems, HighScore (best wave), TutorialDone
- [x] Sauvegarder : UnlockedSkins, EquippedSkin
- [x] Sauvegarder : Achievements (JSON string)
- [x] Sauvegarder : DailyLogin data (JSON string)
- [x] Sauvegarder : Settings (son, musique, vibration, langue)
- [x] Load au demarrage, Save apres chaque changement important
- [x] Methode ResetAll() pour debug

### 7.2 HighScore
- [x] Tracker la meilleure vague atteinte
- [ ] Afficher "Meilleur: Vague X" dans le menu principal
- [x] Afficher "Meilleur: X" sur l'ecran Game Over (comparaison) + "NOUVEAU RECORD !" si nouveau best

---

## 8. SKINS (P2)

> Actuellement : Aucun systeme de skins
> Cible : 7 skins (default + 5 boss rewards + 1 daily)

### 8.1 Systeme de skins
- [ ] Creer `SkinData.cs` (ScriptableObject) : id, nom, sprite, preview, unlock type, boss wave, prix gems
- [ ] Creer `SkinManager.cs` (singleton) : unlock, equip, purchase, save/load
- [ ] 3 types de deblocage : Free, Boss, Daily
- [ ] Appliquer le skin equipe au SpriteRenderer du cannon

### 8.2 Les 7 skins V1
> Monnaie d'achat : **GEMS** (💎 permanent)

- [ ] Creer SkinData : Default (gratuit)
- [ ] Creer SkinData : Bronze (boss vague 10, OU 500 💎)
- [ ] Creer SkinData : Silver (boss vague 20, OU 1000 💎)
- [ ] Creer SkinData : Gold (boss vague 30, OU 2500 💎)
- [ ] Creer SkinData : Diamond (boss vague 40, OU 5000 💎)
- [ ] Creer SkinData : Legendary (boss vague 50, OU 10000 💎)
- [ ] Creer SkinData : Rainbow (daily login 7j, exclusif)
- [ ] Sprites pour chaque skin (cannon colore — utiliser Gun01-10 sprites)

### 8.3 UI Skin Shop
- [ ] Creer `SkinShopUI.cs` : page skins accessible depuis le menu principal
- [ ] Preview du skin selectionne (grand sprite centre)
- [ ] Grille de collection (toutes les skins, locked/unlocked/equipped)
- [ ] Bouton "Equiper" pour les skins debloquees
- [ ] Bouton "Acheter" pour les skins boss (avec gems 💎)
- [ ] Info "Login 7j" pour le skin daily (pas achetable)
- [ ] Popup confirmation achat
- [ ] Popup gems insuffisants
- [ ] Popup "Nouveau skin debloque!" (apres boss ou achat)

### 8.4 Integration boss → skin
- [ ] Appeler `SkinManager.OnBossDefeated(wave)` quand un boss est vaincu
- [ ] Afficher la popup de skin debloque immediatement

---

## 9. ACHIEVEMENTS (P2)

> Actuellement : Aucun systeme d'achievements
> Cible : 15 achievements de base

### 9.1 Systeme d'achievements
- [ ] Creer `AchievementData.cs` (ScriptableObject) : id, nom, description, condition, reward gems
- [ ] Creer `AchievementManager.cs` (singleton) : tracker progression, debloquer, sauvegarder
- [ ] Tracker les stats persistentes : total kills, total runs, best wave, total bosses killed
- [ ] Check achievements apres chaque evenement (kill, wave complete, game over)
- [ ] Notification toast quand un achievement est debloque

### 9.2 Les 15 achievements V1
> Rewards en **GEMS** (💎 permanent)

**Progression (5)**
- [ ] Premier pas : Atteindre vague 5 → 50 💎
- [ ] Apprenti : Atteindre vague 10 → 100 💎
- [ ] Survivant : Atteindre vague 25 → 250 💎
- [ ] Veteran : Atteindre vague 40 → 400 💎
- [ ] Legende : Atteindre vague 50 → 500 💎

**Combat (5)**
- [ ] Premier sang : 1 kill → 25 💎
- [ ] Chasseur : 100 kills total → 100 💎
- [ ] Exterminateur : 500 kills total → 250 💎
- [ ] Tueur de boss : 1 boss tue → 100 💎
- [ ] Chasseur de boss : 5 boss tues → 300 💎

**Skill (3)**
- [ ] Combo x5 : Atteindre un combo de 5 → 75 💎
- [ ] Combo x10 : Atteindre un combo de 10 → 150 💎
- [ ] Perfectionniste : Terminer une vague sans degat → 100 💎

**Retention (2)**
- [ ] Fidele : Daily login 7 jours → Skin Rainbow
- [ ] Accro : 50 parties jouees → 200 💎

### 9.3 UI Achievements
- [ ] Creer `AchievementUI.cs` : page achievements depuis le menu principal
- [ ] Liste scrollable de tous les achievements (icone, nom, description, progression, reward)
- [ ] Etat : locked (grise + barre progression) / unlocked (colore + "Recompense recue")
- [ ] Toast notification in-game quand un achievement est debloque (Feel spring)

---

## 10. DAILY LOGIN (P2)

> Actuellement : Aucun systeme de daily login
> Cible : 7 jours de rewards, cycle reset

### 10.1 Systeme daily login
- [ ] Creer `DailyLoginManager.cs` (singleton) : tracker les connexions
- [ ] Detecter si c'est un nouveau jour (comparer date sauvegardee vs aujourd'hui)
- [ ] Streak consecutive : miss 1 jour = retour jour 1
- [ ] Sauvegarder via PlayerPrefs (LastLoginDate, CurrentStreak, TotalClaimed)

### 10.2 Rewards 7 jours
> Rewards en **GEMS** (💎 permanent)

- [ ] Jour 1 : 50 💎
- [ ] Jour 2 : 100 💎
- [ ] Jour 3 : Upgrade gratuit (prochain run)
- [ ] Jour 4 : 150 💎
- [ ] Jour 5 : 200 💎
- [ ] Jour 6 : 300 💎
- [ ] Jour 7 : Skin Rainbow (une seule fois, sinon 500 💎)
- [ ] Reset du cycle apres jour 7

### 10.3 UI Daily Login
- [ ] Creer `DailyLoginUI.cs` : popup au lancement du jeu
- [ ] Afficher les 7 jours avec icones de reward
- [ ] Jour courant highlight (animatoin)
- [ ] Jours passes = coches
- [ ] Jours futurs = grise
- [ ] Bouton "Reclamer" pour le jour courant
- [ ] Bouton accessible depuis le menu principal (icone calendrier)

---

## 11. MONETISATION — REWARDED ADS (P2)

> Actuellement : Aucun systeme de pubs
> Cible : 3 placements rewarded ads (zero pub forcee)

### 11.1 Integration Unity Ads
- [ ] Installer Unity Ads package
- [ ] Creer `AdsManager.cs` (singleton) : initialiser, charger, afficher ads
- [ ] Configurer les ad unit IDs (Android)
- [ ] Precharger les ads au demarrage
- [ ] Callback : OnAdCompleted, OnAdFailed, OnAdSkipped

### 11.2 Placement : Revive (Game Over)
- [ ] Bouton "Revive (Regarder pub)" sur l'ecran Game Over
- [ ] Si ad completee : restaurer 50% HP, continuer le run
- [ ] Limite : 1x par run
- [ ] Cacher le bouton si deja utilise dans ce run

### 11.3 Placement : Double Gems (Game Over)
- [ ] Bouton "x2 Gems (Regarder pub)" sur l'ecran Game Over
- [ ] Si ad completee : doubler les gems gagnes dans ce run
- [ ] Limite : 1x par run
- [ ] Afficher le calcul : "46 💎 → 92 💎"

### 11.4 Placement : Bonus Boulets (Menu)
- [ ] Bouton "Bonus boulets (Pub)" dans le menu principal
- [ ] Si ad completee : +2 boulets au prochain run
- [ ] Limite : 3x par jour
- [ ] Afficher le nombre restant "2/3"

### 11.5 UI Game Over revisee
- [ ] Redesign l'ecran Game Over avec les 2 boutons ad (revive + x2 gems)
- [ ] Afficher : vague atteinte, meilleur score, gems gagnes
- [ ] Boutons Menu + Rejouer en bas

---

## 12. SETTINGS (P3)

> Actuellement : Aucun ecran settings
> Cible : Son, Musique, Vibration, Langue

### 12.1 Systeme de settings
- [ ] Creer `SettingsManager.cs` (singleton) : gerer les preferences
- [ ] SFX on/off → `SFXManager.cs`
- [ ] Music on/off → `SFXManager.cs`
- [ ] Vibration on/off → Handheld.Vibrate
- [ ] Langue FR/EN → PlayerPrefs, recharger textes
- [ ] Sauvegarder dans PlayerPrefs

### 12.2 UI Settings
- [ ] Creer `SettingsUI.cs` : page settings
- [ ] Toggle Son (ON/OFF)
- [ ] Toggle Musique (ON/OFF)
- [ ] Toggle Vibration (ON/OFF)
- [ ] Toggle Langue (FR/EN)
- [ ] Bouton "Revoir tutoriel"
- [ ] Bouton "Politique de confidentialite" (ouvre URL)
- [ ] Accessible depuis menu principal (icone engrenage)
- [ ] Accessible depuis l'ecran pause

---

## 13. AUDIO COMPLET (P1)

> Actuellement : SFXManager avec 6 SFX + 2 musiques
> Cible : 11 SFX + 3 musiques

### 13.1 SFX manquants
- [x] Cannon Fire — `cannon-fire.mp3`
- [x] Wall Bounce — `hit-wall.wav`
- [x] Monster Hit — `moster-hit.wav`
- [x] Player Damage — `cannon-damage-receive.wav`
- [x] Gem Collect — `coins.wav`
- [x] Button Click — `click2.ogg`
- [ ] Monster Death (pop/explosion) — trouver/creer son CC0
- [ ] Upgrade Select (level up) — trouver/creer son CC0
- [ ] Wave Complete (fanfare courte) — trouver/creer son CC0
- [ ] Game Over (son triste) — trouver/creer son CC0
- [ ] Boss Appear (son dramatique) — trouver/creer son CC0

### 13.2 Musiques manquantes
- [x] Menu Theme — `menu-bg-music.wav`
- [x] Combat Theme — `action-bg-music.wav`
- [ ] Boss Theme (intense, loop) — trouver/creer musique CC0

### 13.3 Integration audio
- [ ] Ajouter les nouveaux sons dans `SFXManager.cs`
- [ ] Trigger Monster Death son dans `EnemyHealth.cs`
- [ ] Trigger Upgrade Select dans `UpgradeScreenUI.cs`
- [ ] Trigger Wave Complete dans `WaveManager.cs`
- [ ] Trigger Game Over dans `GameManager.cs`
- [ ] Trigger Boss Appear dans `BossSpawner.cs`
- [ ] Crossfade vers Boss Theme pendant les vagues boss

---

## 14. LOCALISATION FR/EN (P1)

> Actuellement : Textes en dur (francais)
> Cible : FR + EN pour tout le jeu

### 14.1 Systeme de localisation
- [ ] Creer `LocalizationManager.cs` : charger les textes selon la langue
- [ ] Format simple : dictionnaire cle → valeur (JSON ou SO)
- [ ] Langue par defaut : FR
- [ ] Changeable dans Settings

### 14.2 Textes a localiser
- [ ] Menu principal : titre, boutons, labels
- [ ] HUD : "VAGUE", labels
- [ ] Upgrade screen : noms et descriptions des 12 upgrades
- [ ] Game Over : textes, labels
- [ ] Victory : textes
- [ ] Tutoriel : 7 textes (GDD 2.3)
- [ ] Achievements : noms et descriptions des 15 achievements
- [ ] Settings : labels
- [ ] Skin Shop : labels, popups
- [ ] Daily Login : labels

---

## 15. UI POLISH + MENU PRINCIPAL COMPLET (P1)

> Actuellement : Menu basique (titre + bouton JOUER)
> Cible : Menu avec acces Skins, Achievements, Daily, Settings

### 15.1 Menu principal etendu
- [ ] Ajouter bouton Skins (icone palette) → ouvre SkinShopUI
- [ ] Ajouter bouton Achievements (icone trophee) → ouvre AchievementUI
- [ ] Ajouter bouton Daily Login (icone calendrier) → ouvre DailyLoginUI
- [ ] Ajouter bouton Settings (icone engrenage) → ouvre SettingsUI
- [ ] Afficher "Meilleur: Vague X" sous le titre
- [ ] Afficher gems (💎) en haut (monnaie permanente pour skins)
- [ ] Notification badge sur Daily si reward non reclamee
- [ ] Notification badge sur Achievements si nouveau debloque

### 15.2 Transitions & animations
- [ ] Transitions entre ecrans (Feel spring slide in/out)
- [ ] Boutons avec Feel feedback (scale bounce au clic)
- [ ] Fond anime (particules ou monstres qui passent)

---

## 16. PUBLICATION GOOGLE PLAY (P0)

> Actuellement : Build Android fonctionne
> Cible : App publiee sur Google Play

### 16.1 Assets de publication
- [ ] Icone app 512x512 (hi-res icon)
- [ ] Feature Graphic 1024x500
- [ ] Screenshots 5-8 (gameplay, menu, upgrades, boss, skins)
- [ ] Logo/titre du jeu pour les screenshots

### 16.2 Store listing
- [ ] Description courte FR (80 chars max)
- [ ] Description courte EN
- [ ] Description longue FR (4000 chars max)
- [ ] Description longue EN
- [ ] Tags/categories : Arcade, Casual
- [ ] Content rating questionnaire

### 16.3 Configuration build
- [ ] Package name definitif : `com.rdh.monstercannon`
- [ ] Version code + version name
- [ ] Minimum API level (Android 7.0 / API 24)
- [ ] Target API level (Android 14 / API 34)
- [ ] Keystore de signature (release)
- [ ] Proguard/R8 minification
- [ ] AAB (Android App Bundle) au lieu de APK

### 16.4 Legal & Privacy
- [ ] Privacy Policy (page web ou Google Doc)
- [ ] Terms of Service (optionnel pour le lancement)
- [ ] URL de la privacy policy dans le store listing

### 16.5 Performance & QA
- [ ] Tester sur 3+ appareils Android (bas, moyen, haut de gamme)
- [ ] FPS stable 60 sur device moyen
- [ ] Temps de chargement < 3s
- [ ] Taille APK/AAB < 100MB
- [ ] Aucun crash sur 10 runs complets
- [ ] Tester le flow complet : menu → tuto → vague 1-10 → boss → upgrade → game over → revive → coins

### 16.6 Google Play Console
- [ ] Creer la fiche app sur Google Play Console
- [ ] Uploader l'AAB en internal testing
- [ ] Test internal → closed testing → open testing → production
- [ ] Publier

---

## ECONOMIE — RECAP

| Monnaie | Icone | Usage | Persistance |
|---------|-------|-------|-------------|
| **COINS** | 🪙 | Droppes par monstres, achat upgrades in-run | **Reset au game over** |
| **GEMS** | 💎 | Skins, achievements rewards, daily login | **Permanent (PlayerPrefs)** |

## SPRITES DISPONIBLES — RECAP

| Type | Sprites | Frames | Usage |
|------|---------|--------|-------|
| Monstres | Monster01-10 | 20 frames | Ennemis normaux (variants par tier) |
| Boss | Bos01-05 | 25 frames | 5 boss (vague 10-50) |
| Canons | Gun01-10 | Spine | Skins cannon joueur |

## RESUME V1

| Categorie | Taches | Priorite | Complexite |
|-----------|--------|----------|------------|
| 1. Vagues Infinies + Scaling + Variants | 18 | P0 | Moyenne |
| 2. Boss System (5 boss, sprites Bos01-05) | 23 | P0 | Haute |
| 3. Mecaniques Gameplay | 17 | P0 | Haute |
| 4. Upgrades Complets (11 upgrades) | 14 | P1 | Moyenne |
| 5. Tutoriel ✅ | 16 | P0 | Moyenne |
| 6. Economie Coins + Gems (inversee) | 12 | P0 | Moyenne |
| 7. Sauvegarde | 8 | P0 | Faible |
| 8. Skins (7 skins, prix en 💎) | 18 | P2 | Moyenne |
| 9. Achievements (15, rewards en 💎) | 22 | P2 | Moyenne |
| 10. Daily Login (rewards en 💎) | 12 | P2 | Moyenne |
| 11. Rewarded Ads | 13 | P2 | Moyenne |
| 12. Settings | 10 | P3 | Faible |
| 13. Audio Complet | 13 | P1 | Faible |
| 14. Localisation FR/EN | 12 | P1 | Moyenne |
| 15. UI Polish + Menu | 10 | P1 | Moyenne |
| 16. Publication Google Play | 18 | P0 | Moyenne |
| **TOTAL** | **~236 taches** | | |

## ORDRE DE DEVELOPPEMENT RECOMMANDE

### Phase 1 — Core Infinite (Semaines 1-2)
1. Vagues Infinies (section 1)
2. Sauvegarde (section 7)
3. Economie Coins (section 6)

### Phase 2 — Boss & Mecaniques (Semaines 3-4)
4. Boss System (section 2)
5. Mecaniques Gameplay (section 3)

### Phase 3 — Contenu (Semaine 5)
6. Tutoriel (section 5)
7. Upgrades Complets (section 4)

### Phase 4 — Progression & Retention (Semaines 6-7)
8. Skins (section 8)
9. Achievements (section 9)
10. Daily Login (section 10)

### Phase 5 — Monetisation & Polish (Semaines 8-9)
11. Rewarded Ads (section 11)
12. Audio Complet (section 13)
13. Localisation (section 14)
14. Settings (section 12)
15. UI Polish (section 15)

### Phase 6 — Publication (Semaine 10)
16. Publication Google Play (section 16)

---

> **FOCUS : Un jeu publie > un jeu parfait jamais sorti.**
> Couper les features si necessaire. La V2 existe pour ca.
