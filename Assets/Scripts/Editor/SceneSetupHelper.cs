#if UNITY_EDITOR
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

namespace CatWar.Editor
{
    public static class SceneSetupHelper
    {
        [MenuItem("Cat War/Build Game Scene and Assets")]
        public static void BuildProject()
        {
            Debug.Log("[Cat War Setup] Beginning automated project setup...");

            // 1. Create Directories
            CreateRequiredFolders();

            // 2. Setup Layers
            SetupProjectLayers();

            // 3. Generate Sprite Placeholders
            Sprite knightSprite = CreateTextureAsset("Sprites/KnightCat.png", new Color(0.2f, 0.4f, 0.8f));      // Blue
            Sprite rogueSprite = CreateTextureAsset("Sprites/RogueCat.png", new Color(0.1f, 0.7f, 0.6f));        // Teal
            Sprite paladinSprite = CreateTextureAsset("Sprites/PaladinCat.png", new Color(0.5f, 0.5f, 0.6f));    // Gray
            Sprite crossbowSprite = CreateTextureAsset("Sprites/CrossbowCat.png", new Color(0.1f, 0.6f, 0.2f));  // Green
            Sprite clericSprite = CreateTextureAsset("Sprites/ClericCat.png", new Color(0.9f, 0.8f, 0.2f));      // Yellow
            
            Sprite enemyKnightSprite = CreateTextureAsset("Sprites/EnemyKnight.png", new Color(0.8f, 0.2f, 0.2f)); // Red
            Sprite enemyGoblinSprite = CreateTextureAsset("Sprites/EnemyGoblin.png", new Color(0.5f, 0.1f, 0.1f)); // Dark Red

            Sprite playerBaseSprite = CreateTextureAsset("Sprites/PlayerCastle.png", new Color(0.3f, 0.3f, 0.35f), 128, 256);
            Sprite enemyBaseSprite = CreateTextureAsset("Sprites/EnemyFortress.png", new Color(0.2f, 0.15f, 0.15f), 128, 256);
            Sprite boltSprite = CreateTextureAsset("Sprites/CrossbowBolt.png", new Color(0.7f, 0.5f, 0.2f), 32, 8);

            // 4. Create Projectile Prefab
            GameObject boltPrefab = CreateProjectilePrefab(boltSprite);

            // 5. Create HealthBar Prefab
            GameObject healthBarPrefab = CreateHealthBarPrefab();

            // 6. Create UnitData ScriptableObjects
            UnitData knightData = CreateUnitData("Knight Cat", 50, 6f, 120, 15, 1.2f, 1.0f, 1.8f, false, false, 0, knightSprite, null, boltPrefab);
            UnitData rogueData = CreateUnitData("Rogue Cat", 75, 4f, 80, 12, 1.0f, 0.4f, 3.2f, false, false, 0, rogueSprite, null, boltPrefab);
            UnitData paladinData = CreateUnitData("Paladin Cat", 150, 10f, 350, 10, 1.0f, 1.5f, 0.8f, false, false, 0, paladinSprite, null, boltPrefab);
            UnitData crossbowData = CreateUnitData("Crossbow Cat", 100, 8f, 70, 20, 5.5f, 1.8f, 1.4f, true, false, 0, crossbowSprite, null, boltPrefab);
            UnitData clericData = CreateUnitData("Cleric Cat", 125, 8f, 90, 0, 3.5f, 2.2f, 1.5f, false, true, 25, clericSprite, null, boltPrefab);

            UnitData enemyKnightData = CreateUnitData("Dark Knight", 60, 8f, 130, 12, 1.2f, 1.2f, 1.5f, false, false, 0, enemyKnightSprite, null, null);
            UnitData enemyGoblinData = CreateUnitData("Goblin Thief", 40, 4f, 60, 8, 1.0f, 0.6f, 2.8f, false, false, 0, enemyGoblinSprite, null, null);

            // 7. Create Unit Prefabs
            GameObject knightPref = CreateUnitPrefab(knightData, healthBarPrefab, false);
            GameObject roguePref = CreateUnitPrefab(rogueData, healthBarPrefab, false);
            GameObject paladinPref = CreateUnitPrefab(paladinData, healthBarPrefab, false);
            GameObject crossbowPref = CreateUnitPrefab(crossbowData, healthBarPrefab, false);
            GameObject clericPref = CreateUnitPrefab(clericData, healthBarPrefab, false);

            GameObject enemyKnightPref = CreateUnitPrefab(enemyKnightData, healthBarPrefab, true);
            GameObject enemyGoblinPref = CreateUnitPrefab(enemyGoblinData, healthBarPrefab, true);

            // Assign prefabs to ScriptableObjects
            knightData.prefab = knightPref;
            rogueData.prefab = roguePref;
            paladinData.prefab = paladinPref;
            crossbowData.prefab = crossbowPref;
            clericData.prefab = clericPref;
            enemyKnightData.prefab = enemyKnightPref;
            enemyGoblinData.prefab = enemyGoblinPref;

            EditorUtility.SetDirty(knightData);
            EditorUtility.SetDirty(rogueData);
            EditorUtility.SetDirty(paladinData);
            EditorUtility.SetDirty(crossbowData);
            EditorUtility.SetDirty(clericData);
            EditorUtility.SetDirty(enemyKnightData);
            EditorUtility.SetDirty(enemyGoblinData);

            // 8. Build Battlefield Scene
            BuildBattlefieldScene(
                playerBaseSprite, enemyBaseSprite,
                new UnitData[] { knightData, rogueData, paladinData, crossbowData, clericData },
                new UnitData[] { enemyGoblinData, enemyKnightData }
            );

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            Debug.Log("[Cat War Setup] Automated project setup complete! Open Scenes/Battlefield to play.");
        }

        private static void CreateRequiredFolders()
        {
            string[] folders = { "Scenes", "Prefabs", "Prefabs/Units", "ScriptableObjects", "Sprites" };
            foreach (var folder in folders)
            {
                string path = Path.Combine(Application.dataPath, folder);
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
            }
            AssetDatabase.Refresh();
        }

        private static void SetupProjectLayers()
        {
            SerializedObject tagManager = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset")[0]);
            SerializedProperty layers = tagManager.FindProperty("layers");

            string[] customLayers = { "PlayerUnit", "EnemyUnit", "PlayerBase", "EnemyBase" };
            int[] targetIndices = { 8, 9, 10, 11 };

            for (int i = 0; i < customLayers.Length; i++)
            {
                int index = targetIndices[i];
                SerializedProperty layerProp = layers.GetArrayElementAtIndex(index);
                if (string.IsNullOrEmpty(layerProp.stringValue))
                {
                    layerProp.stringValue = customLayers[i];
                    Debug.Log($"[Cat War Setup] Added layer {customLayers[i]} at index {index}");
                }
            }
            tagManager.ApplyModifiedProperties();
        }

        private static Sprite CreateTextureAsset(string relativePath, Color color, int width = 64, int height = 64)
        {
            string fullPath = Path.Combine(Application.dataPath, relativePath);
            string dir = Path.GetDirectoryName(fullPath);
            if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);

            string assetPath = "Assets/" + relativePath;
            if (File.Exists(fullPath))
            {
                return AssetDatabase.LoadAssetAtPath<Sprite>(assetPath);
            }

            Texture2D tex = new Texture2D(width, height);
            Color[] colors = new Color[width * height];
            for (int i = 0; i < colors.Length; i++) colors[i] = color;
            tex.SetPixels(colors);
            tex.Apply();

            byte[] bytes = tex.EncodeToPNG();
            File.WriteAllBytes(fullPath, bytes);
            Object.DestroyImmediate(tex);
            AssetDatabase.ImportAsset(assetPath);

            TextureImporter importer = AssetImporter.GetAtPath(assetPath) as TextureImporter;
            if (importer != null)
            {
                importer.textureType = TextureImporterType.Sprite;
                importer.spritePivot = new Vector2(0.5f, 0.0f); // Set pivot to bottom by default for ground standing
                importer.SaveAndReimport();
            }

            return AssetDatabase.LoadAssetAtPath<Sprite>(assetPath);
        }

        private static UnitData CreateUnitData(
            string name, int cost, float cooldown, float hp, float dmg, float range, float atkCooldown, float speed, 
            bool isRanged, bool isHealer, float heal, Sprite sprite, GameObject prefab, GameObject projPrefab)
        {
            string assetPath = $"Assets/ScriptableObjects/{name.Replace(" ", "")}.asset";
            UnitData data = AssetDatabase.LoadAssetAtPath<UnitData>(assetPath);

            if (data == null)
            {
                data = ScriptableObject.CreateInstance<UnitData>();
                AssetDatabase.CreateAsset(data, assetPath);
            }

            data.unitName = name;
            data.cost = cost;
            data.spawnCooldown = cooldown;
            data.maxHealth = hp;
            data.attackDamage = dmg;
            data.attackRange = range;
            data.attackCooldown = atkCooldown;
            data.movementSpeed = speed;
            data.isRanged = isRanged;
            data.isHealer = isHealer;
            data.healAmount = heal;
            data.unitSprite = sprite;
            data.prefab = prefab;
            data.projectilePrefab = projPrefab;

            EditorUtility.SetDirty(data);
            return data;
        }

        private static GameObject CreateProjectilePrefab(Sprite sprite)
        {
            string path = "Assets/Prefabs/CrossbowBolt.prefab";
            GameObject go = new GameObject("CrossbowBolt");
            
            SpriteRenderer sr = go.AddComponent<SpriteRenderer>();
            sr.sprite = sprite;

            BoxCollider2D col = go.AddComponent<BoxCollider2D>();
            col.isTrigger = true;
            col.size = new Vector2(0.4f, 0.1f);

            go.AddComponent<Projectile>();

            GameObject prefab = PrefabUtility.SaveAsPrefabAsset(go, path);
            Object.DestroyImmediate(go);
            return prefab;
        }

        private static GameObject CreateHealthBarPrefab()
        {
            string path = "Assets/Prefabs/HealthBar.prefab";
            GameObject canvasGo = new GameObject("HealthBarCanvas");
            Canvas canvas = canvasGo.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.WorldSpace;
            
            RectTransform rect = canvasGo.GetComponent<RectTransform>();
            rect.sizeDelta = new Vector2(1.2f, 0.15f);

            canvasGo.AddComponent<CanvasScaler>();

            // Slider background
            GameObject sliderGo = new GameObject("Slider");
            sliderGo.transform.SetParent(canvasGo.transform, false);
            Slider slider = sliderGo.AddComponent<Slider>();
            slider.transition = Selectable.Transition.None;

            RectTransform sliderRect = sliderGo.GetComponent<RectTransform>();
            sliderRect.anchorMin = Vector2.zero;
            sliderRect.anchorMax = Vector2.one;
            sliderRect.sizeDelta = Vector2.zero;

            // Background Image
            GameObject bgGo = new GameObject("Background");
            bgGo.transform.SetParent(sliderGo.transform, false);
            Image bgImg = bgGo.AddComponent<Image>();
            bgImg.color = Color.black;
            RectTransform bgRect = bgGo.GetComponent<RectTransform>();
            bgRect.anchorMin = Vector2.zero;
            bgRect.anchorMax = Vector2.one;
            bgRect.sizeDelta = Vector2.zero;

            // Fill Area
            GameObject fillArea = new GameObject("Fill Area");
            fillArea.transform.SetParent(sliderGo.transform, false);
            RectTransform fillAreaRect = fillArea.AddComponent<RectTransform>();
            fillAreaRect.anchorMin = Vector2.zero;
            fillAreaRect.anchorMax = Vector2.one;
            fillAreaRect.sizeDelta = Vector2.zero;

            GameObject fillGo = new GameObject("Fill");
            fillGo.transform.SetParent(fillArea.transform, false);
            Image fillImg = fillGo.AddComponent<Image>();
            fillImg.color = Color.green;
            RectTransform fillRect = fillGo.GetComponent<RectTransform>();
            fillRect.anchorMin = new Vector2(0, 0);
            fillRect.anchorMax = new Vector2(1, 1);
            fillRect.sizeDelta = Vector2.zero;

            slider.targetGraphic = fillImg;
            slider.fillRect = fillRect;

            // Connect script
            HealthBar hpBar = canvasGo.AddComponent<HealthBar>();
            
            // Accessing internal private Slider using reflection or editing slider serialized property
            SerializedObject obj = new SerializedObject(hpBar);
            obj.FindProperty("slider").objectReferenceValue = slider;
            obj.ApplyModifiedProperties();

            GameObject prefab = PrefabUtility.SaveAsPrefabAsset(canvasGo, path);
            Object.DestroyImmediate(canvasGo);
            return prefab;
        }

        private static GameObject CreateUnitPrefab(UnitData data, GameObject hpBarPrefab, bool isEnemy)
        {
            string path = $"Assets/Prefabs/Units/{data.unitName.Replace(" ", "")}.prefab";
            GameObject go = new GameObject(data.unitName.Replace(" ", ""));

            // Set Layer
            go.layer = LayerMask.NameToLayer(isEnemy ? "EnemyUnit" : "PlayerUnit");

            SpriteRenderer sr = go.AddComponent<SpriteRenderer>();
            sr.sprite = data.unitSprite;
            sr.sortingOrder = 5;

            BoxCollider2D col = go.AddComponent<BoxCollider2D>();
            col.size = new Vector2(0.8f, 1f);
            col.offset = new Vector2(0f, 0.5f); // Standing pivot at bottom

            Rigidbody2D rb = go.AddComponent<Rigidbody2D>();
            rb.bodyType = RigidbodyType2D.Kinematic;

            Unit unit = go.AddComponent<Unit>();
            unit.data = data;
            unit.isEnemy = isEnemy;

            // Setup masks via SerializedObject
            SerializedObject obj = new SerializedObject(unit);
            obj.FindProperty("healthBarPrefab").objectReferenceValue = hpBarPrefab;
            obj.FindProperty("spriteRenderer").objectReferenceValue = sr;
            
            LayerMask enemyMaskVal = 1 << LayerMask.NameToLayer(isEnemy ? "PlayerUnit" : "EnemyUnit");
            enemyMaskVal |= 1 << LayerMask.NameToLayer(isEnemy ? "PlayerBase" : "EnemyBase");
            obj.FindProperty("enemyMask").intValue = enemyMaskVal.value;

            LayerMask friendlyMaskVal = 1 << LayerMask.NameToLayer(isEnemy ? "EnemyUnit" : "PlayerUnit");
            obj.FindProperty("friendlyMask").intValue = friendlyMaskVal.value;

            // Add Projectile fire point if ranged
            if (data.isRanged)
            {
                GameObject fp = new GameObject("FirePoint");
                fp.transform.SetParent(go.transform, false);
                fp.transform.localPosition = new Vector3(0.5f, 0.5f, 0);
                obj.FindProperty("projectileFirePoint").objectReferenceValue = fp.transform;
            }

            obj.ApplyModifiedProperties();

            GameObject prefab = PrefabUtility.SaveAsPrefabAsset(go, path);
            Object.DestroyImmediate(go);
            return prefab;
        }

        private static void BuildBattlefieldScene(
            Sprite playerBaseSprite, Sprite enemyBaseSprite, 
            UnitData[] playerUnits, UnitData[] enemyUnits)
        {
            // Create New Scene
            var scene = EditorSceneManager.NewScene(NewSceneSetup.DefaultGameObjects, NewSceneMode.Single);

            // 1. Camera setup
            Camera mainCam = Camera.main;
            if (mainCam != null)
            {
                mainCam.orthographic = true;
                mainCam.orthographicSize = 5f;
                mainCam.transform.position = new Vector3(0, 0, -10f);
                mainCam.gameObject.AddComponent<CameraController>();
            }

            // 2. Ground setup
            GameObject ground = new GameObject("Ground");
            ground.transform.position = new Vector3(0, -3f, 0);
            BoxCollider2D groundCol = ground.AddComponent<BoxCollider2D>();
            groundCol.size = new Vector2(30f, 1f);

            // 3. Player Base Setup
            GameObject pBase = new GameObject("Player Castle");
            pBase.layer = LayerMask.NameToLayer("PlayerBase");
            pBase.transform.position = new Vector3(-8f, -2.5f, 0);
            
            SpriteRenderer psr = pBase.AddComponent<SpriteRenderer>();
            psr.sprite = playerBaseSprite;
            psr.sortingOrder = 2;

            BoxCollider2D pcol = pBase.AddComponent<BoxCollider2D>();
            pcol.size = new Vector2(2f, 4f);
            pcol.offset = new Vector2(0f, 2f);

            Base pBaseScript = pBase.AddComponent<Base>();
            pBaseScript.isPlayerBase = true;
            pBaseScript.maxHealth = 1000f;

            GameObject pSpawnPoint = new GameObject("SpawnPoint");
            pSpawnPoint.transform.SetParent(pBase.transform, false);
            pSpawnPoint.transform.localPosition = new Vector3(1.5f, 0.5f, 0);

            // 4. Enemy Base Setup
            GameObject eBase = new GameObject("Enemy Fortress");
            eBase.layer = LayerMask.NameToLayer("EnemyBase");
            eBase.transform.position = new Vector3(8f, -2.5f, 0);
            
            SpriteRenderer esr = eBase.AddComponent<SpriteRenderer>();
            esr.sprite = enemyBaseSprite;
            esr.sortingOrder = 2;

            BoxCollider2D ecol = eBase.AddComponent<BoxCollider2D>();
            ecol.size = new Vector2(2f, 4f);
            ecol.offset = new Vector2(0f, 2f);

            Base eBaseScript = eBase.AddComponent<Base>();
            eBaseScript.isPlayerBase = false;
            eBaseScript.maxHealth = 1000f;

            GameObject eSpawnPoint = new GameObject("SpawnPoint");
            eSpawnPoint.transform.SetParent(eBase.transform, false);
            eSpawnPoint.transform.localPosition = new Vector3(-1.5f, 0.5f, 0);

            EnemySpawner eSpawner = eBase.AddComponent<EnemySpawner>();
            
            SerializedObject spawnerObj = new SerializedObject(eSpawner);
            spawnerObj.FindProperty("spawnPoint").objectReferenceValue = eSpawnPoint.transform;
            spawnerObj.ApplyModifiedProperties();

            // 5. Setup Managers GameObject
            GameObject managersGo = new GameObject("Managers");
            GameManager gm = managersGo.AddComponent<GameManager>();
            ResourceManager rm = managersGo.AddComponent<ResourceManager>();
            LevelManager lm = managersGo.AddComponent<LevelManager>();
            AudioManager am = managersGo.AddComponent<AudioManager>();
            UIManager um = managersGo.AddComponent<UIManager>();

            // Setup level configuration details
            SerializedObject lmObj = new SerializedObject(lm);
            SerializedProperty levelsProp = lmObj.FindProperty("levels");
            levelsProp.arraySize = 3;

            string[] names = { "Level 1: Castle Outskirts", "Level 2: Enchanted Forest", "Level 3: Dark Fortress Gate" };
            float[] diffs = { 1.0f, 1.4f, 2.0f };
            float[] enemyCooldownsMin = { 10f, 7f, 5f };
            float[] enemyCooldownsMax = { 18f, 12f, 9f };

            for (int i = 0; i < 3; i++)
            {
                SerializedProperty level = levelsProp.GetArrayElementAtIndex(i);
                level.FindPropertyRelative("levelName").stringValue = names[i];
                level.FindPropertyRelative("baseHealthPlayer").floatValue = 1000f;
                level.FindPropertyRelative("baseHealthEnemy").floatValue = 1000f * diffs[i];
                level.FindPropertyRelative("enemySpawnIntervalMin").floatValue = enemyCooldownsMin[i];
                level.FindPropertyRelative("enemySpawnIntervalMax").floatValue = enemyCooldownsMax[i];
                level.FindPropertyRelative("difficultyMultiplier").floatValue = diffs[i];
                
                // Add enemies to pool
                SerializedProperty pool = level.FindPropertyRelative("enemyPool");
                pool.arraySize = enemyUnits.Length;
                for (int u = 0; u < enemyUnits.Length; u++)
                {
                    pool.GetArrayElementAtIndex(u).objectReferenceValue = enemyUnits[u];
                }
            }
            lmObj.ApplyModifiedProperties();

            // 6. Setup Canvas UI
            BuildCanvasUI(um, pSpawnPoint.transform, playerUnits);

            // Save Scene
            EditorSceneManager.SaveScene(scene, "Assets/Scenes/Battlefield.unity");

            // Register in Build Settings
            EditorBuildSettingsScene[] newScenes = new EditorBuildSettingsScene[1];
            newScenes[0] = new EditorBuildSettingsScene("Assets/Scenes/Battlefield.unity", true);
            EditorBuildSettings.scenes = newScenes;
        }

        private static void BuildCanvasUI(UIManager um, Transform playerSpawnPoint, UnitData[] playerUnits)
        {
            GameObject canvasGo = new GameObject("UI Canvas");
            Canvas canvas = canvasGo.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvasGo.AddComponent<CanvasScaler>();
            canvasGo.AddComponent<GraphicRaycaster>();

            // Event System
            GameObject eventSystem = new GameObject("EventSystem");
            eventSystem.AddComponent<UnityEngine.EventSystems.EventSystem>();
#if ENABLE_INPUT_SYSTEM
            eventSystem.AddComponent<UnityEngine.InputSystem.UI.InputSystemUIInputModule>();
#else
            eventSystem.AddComponent<UnityEngine.EventSystems.StandaloneInputModule>();
#endif

            // HUD Panel
            GameObject hud = new GameObject("HUD Panel");
            hud.transform.SetParent(canvasGo.transform, false);
            RectTransform hudRect = hud.AddComponent<RectTransform>();
            hudRect.anchorMin = new Vector2(0f, 0f);
            hudRect.anchorMax = new Vector2(1f, 1f);
            hudRect.sizeDelta = Vector2.zero;

            // HUD Background color (dark tint)
            Image hudImg = hud.AddComponent<Image>();
            hudImg.color = new Color(0, 0, 0, 0.1f);

            // Gold Text
            GameObject goldGo = new GameObject("Gold Text");
            goldGo.transform.SetParent(hud.transform, false);
            Text goldTxt = goldGo.AddComponent<Text>();
            goldTxt.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            goldTxt.fontSize = 24;
            goldTxt.color = Color.yellow;
            goldTxt.alignment = TextAnchor.UpperLeft;
            RectTransform gRect = goldGo.GetComponent<RectTransform>();
            gRect.anchorMin = new Vector2(0f, 1f);
            gRect.anchorMax = new Vector2(0f, 1f);
            gRect.pivot = new Vector2(0f, 1f);
            gRect.anchoredPosition = new Vector2(20f, -20f);
            gRect.sizeDelta = new Vector2(300f, 50f);

            // Level Text
            GameObject lvlGo = new GameObject("Level Text");
            lvlGo.transform.SetParent(hud.transform, false);
            Text lvlTxt = lvlGo.AddComponent<Text>();
            lvlTxt.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            lvlTxt.fontSize = 28;
            lvlTxt.color = Color.white;
            lvlTxt.alignment = TextAnchor.UpperCenter;
            RectTransform lRect = lvlGo.GetComponent<RectTransform>();
            lRect.anchorMin = new Vector2(0.5f, 1f);
            lRect.anchorMax = new Vector2(0.5f, 1f);
            lRect.pivot = new Vector2(0.5f, 1f);
            lRect.anchoredPosition = new Vector2(0f, -15f);
            lRect.sizeDelta = new Vector2(400f, 50f);

            // Miner Upgrade button panel
            GameObject minerGo = new GameObject("Miner Upgrade Button");
            minerGo.transform.SetParent(hud.transform, false);
            Button minerBtn = minerGo.AddComponent<Button>();
            Image minerImg = minerGo.AddComponent<Image>();
            minerImg.color = new Color(0.7f, 0.7f, 0.2f);
            RectTransform mRect = minerGo.GetComponent<RectTransform>();
            mRect.anchorMin = new Vector2(1f, 1f);
            mRect.anchorMax = new Vector2(1f, 1f);
            mRect.pivot = new Vector2(1f, 1f);
            mRect.anchoredPosition = new Vector2(-20f, -20f);
            mRect.sizeDelta = new Vector2(180f, 60f);

            // Text inside miner upgrade
            GameObject minerTextGo = new GameObject("Text");
            minerTextGo.transform.SetParent(minerGo.transform, false);
            Text minerTxt = minerTextGo.AddComponent<Text>();
            minerTxt.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            minerTxt.fontSize = 16;
            minerTxt.color = Color.black;
            minerTxt.alignment = TextAnchor.MiddleCenter;
            minerTxt.text = "Upgrade Miner\nCost: 90";
            RectTransform mtRect = minerTextGo.GetComponent<RectTransform>();
            mtRect.anchorMin = Vector2.zero;
            mtRect.anchorMax = Vector2.one;
            mtRect.sizeDelta = Vector2.zero;

            // Spawn Button Panel at bottom
            GameObject spawnPanel = new GameObject("Spawn Buttons Panel");
            spawnPanel.transform.SetParent(hud.transform, false);
            RectTransform sPanelRect = spawnPanel.AddComponent<RectTransform>();
            sPanelRect.anchorMin = new Vector2(0.5f, 0f);
            sPanelRect.anchorMax = new Vector2(0.5f, 0f);
            sPanelRect.pivot = new Vector2(0.5f, 0f);
            sPanelRect.anchoredPosition = new Vector2(0f, 30f);
            sPanelRect.sizeDelta = new Vector2(600f, 100f);

            HorizontalLayoutGroup hlg = spawnPanel.AddComponent<HorizontalLayoutGroup>();
            hlg.spacing = 15f;
            hlg.childAlignment = TextAnchor.MiddleCenter;
            hlg.childControlHeight = true;
            hlg.childControlWidth = true;

            // Create buttons
            foreach (var unit in playerUnits)
            {
                GameObject btnGo = new GameObject($"{unit.unitName} Button");
                btnGo.transform.SetParent(spawnPanel.transform, false);
                Button btn = btnGo.AddComponent<Button>();
                
                Image img = btnGo.AddComponent<Image>();
                img.color = Color.white; // Background container

                // Icon image
                GameObject iconGo = new GameObject("Icon");
                iconGo.transform.SetParent(btnGo.transform, false);
                Image iconImg = iconGo.AddComponent<Image>();
                iconImg.sprite = unit.unitSprite;
                RectTransform iconRect = iconGo.GetComponent<RectTransform>();
                iconRect.anchorMin = Vector2.zero;
                iconRect.anchorMax = Vector2.one;
                iconRect.sizeDelta = Vector2.zero;

                // Cooldown overlay
                GameObject coolGo = new GameObject("Cooldown Overlay");
                coolGo.transform.SetParent(btnGo.transform, false);
                Image coolImg = coolGo.AddComponent<Image>();
                coolImg.color = new Color(0, 0, 0, 0.6f);
                coolImg.type = Image.Type.Filled;
                coolImg.fillMethod = Image.FillMethod.Radial360;
                coolImg.fillOrigin = 2; // Top
                RectTransform coolRect = coolGo.GetComponent<RectTransform>();
                coolRect.anchorMin = Vector2.zero;
                coolRect.anchorMax = Vector2.one;
                coolRect.sizeDelta = Vector2.zero;

                // Cost Text
                GameObject costGo = new GameObject("Cost Text");
                costGo.transform.SetParent(btnGo.transform, false);
                Text costTxt = costGo.AddComponent<Text>();
                costTxt.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
                costTxt.fontSize = 14;
                costTxt.color = Color.yellow;
                costTxt.alignment = TextAnchor.LowerCenter;
                costTxt.text = $"{unit.cost} G";
                RectTransform costRect = costGo.GetComponent<RectTransform>();
                costRect.anchorMin = new Vector2(0, 0);
                costRect.anchorMax = new Vector2(1, 0.3f);
                costRect.sizeDelta = Vector2.zero;

                // Wire script
                SpawnButton sb = btnGo.AddComponent<SpawnButton>();
                SerializedObject sbObj = new SerializedObject(sb);
                sbObj.FindProperty("unitData").objectReferenceValue = unit;
                sbObj.FindProperty("spawnPoint").objectReferenceValue = playerSpawnPoint;
                sbObj.FindProperty("button").objectReferenceValue = btn;
                sbObj.FindProperty("iconImage").objectReferenceValue = iconImg;
                sbObj.FindProperty("cooldownImage").objectReferenceValue = coolImg;
                sbObj.FindProperty("costText").objectReferenceValue = costTxt;
                sbObj.ApplyModifiedProperties();
            }

            // Pause Overlay Panel
            GameObject pause = CreateFullscreenPanel(canvasGo, "Pause Panel", new Color(0, 0, 0, 0.6f), um, "ResumeGame", "RestartLevel", "QuitGame");
            // Game Over Panel
            GameObject lose = CreateFullscreenPanel(canvasGo, "Game Over Panel", new Color(0.6f, 0, 0, 0.7f), um, null, "RestartLevel", "QuitGame");
            // Victory Panel
            GameObject victory = CreateFullscreenPanel(canvasGo, "Victory Panel", new Color(0, 0.5f, 0.1f, 0.7f), um, "NextLevel", "RestartLevel", "QuitGame");

            // Wire UIManager
            SerializedObject umObj = new SerializedObject(um);
            umObj.FindProperty("hudPanel").objectReferenceValue = hud;
            umObj.FindProperty("pausePanel").objectReferenceValue = pause;
            umObj.FindProperty("gameOverPanel").objectReferenceValue = lose;
            umObj.FindProperty("victoryPanel").objectReferenceValue = victory;
            umObj.FindProperty("goldText").objectReferenceValue = goldTxt;
            umObj.FindProperty("minerLevelText").objectReferenceValue = minerTxt; // We use the same text container
            umObj.FindProperty("levelNameText").objectReferenceValue = lvlTxt;
            umObj.FindProperty("minerUpgradeButton").objectReferenceValue = minerBtn;
            umObj.ApplyModifiedProperties();
        }

        private static GameObject CreateFullscreenPanel(
            GameObject canvas, string name, Color color, UIManager um, string action1, string action2, string action3)
        {
            GameObject panel = new GameObject(name);
            panel.transform.SetParent(canvas.transform, false);
            RectTransform pRect = panel.AddComponent<RectTransform>();
            pRect.anchorMin = Vector2.zero;
            pRect.anchorMax = Vector2.one;
            pRect.sizeDelta = Vector2.zero;

            Image img = panel.AddComponent<Image>();
            img.color = color;

            // Panel Title Text
            GameObject titleGo = new GameObject("Title");
            titleGo.transform.SetParent(panel.transform, false);
            Text titleTxt = titleGo.AddComponent<Text>();
            titleTxt.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            titleTxt.fontSize = 40;
            titleTxt.color = Color.white;
            titleTxt.alignment = TextAnchor.MiddleCenter;
            titleTxt.text = name.Replace(" Panel", "");
            RectTransform tRect = titleGo.GetComponent<RectTransform>();
            tRect.anchorMin = new Vector2(0.5f, 0.7f);
            tRect.anchorMax = new Vector2(0.5f, 0.7f);
            tRect.pivot = new Vector2(0.5f, 0.5f);
            tRect.sizeDelta = new Vector2(400f, 80f);

            // Container for buttons
            GameObject btnsGo = new GameObject("ButtonsContainer");
            btnsGo.transform.SetParent(panel.transform, false);
            RectTransform bcRect = btnsGo.AddComponent<RectTransform>();
            bcRect.anchorMin = new Vector2(0.5f, 0.4f);
            bcRect.anchorMax = new Vector2(0.5f, 0.4f);
            bcRect.pivot = new Vector2(0.5f, 0.5f);
            bcRect.sizeDelta = new Vector2(500f, 100f);

            HorizontalLayoutGroup hlg = btnsGo.AddComponent<HorizontalLayoutGroup>();
            hlg.spacing = 20f;
            hlg.childAlignment = TextAnchor.MiddleCenter;
            hlg.childControlWidth = true;
            hlg.childControlHeight = true;

            // Setup buttons
            if (!string.IsNullOrEmpty(action1))
            {
                CreateMenuButton(btnsGo, action1 == "NextLevel" ? "Next Level" : "Resume", um, action1);
            }
            if (!string.IsNullOrEmpty(action2))
            {
                CreateMenuButton(btnsGo, "Restart", um, action2);
            }
            if (!string.IsNullOrEmpty(action3))
            {
                CreateMenuButton(btnsGo, "Quit", um, action3);
            }

            panel.SetActive(false); // Start deactivated
            return panel;
        }

        private static void CreateMenuButton(GameObject container, string text, UIManager um, string methodName)
        {
            GameObject btnGo = new GameObject($"{text} Button");
            btnGo.transform.SetParent(container.transform, false);
            
            Button btn = btnGo.AddComponent<Button>();
            Image img = btnGo.AddComponent<Image>();
            img.color = Color.white;

            GameObject textGo = new GameObject("Text");
            textGo.transform.SetParent(btnGo.transform, false);
            Text txt = textGo.AddComponent<Text>();
            txt.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            txt.fontSize = 18;
            txt.color = Color.black;
            txt.alignment = TextAnchor.MiddleCenter;
            txt.text = text;

            RectTransform tRect = textGo.GetComponent<RectTransform>();
            tRect.anchorMin = Vector2.zero;
            tRect.anchorMax = Vector2.one;
            tRect.sizeDelta = Vector2.zero;

            // Map button action
            btn.onClick.AddListener(() =>
            {
                if (methodName == "ResumeGame") um.OnResumeButtonPressed();
                else if (methodName == "RestartLevel") um.OnRestartButtonPressed();
                else if (methodName == "NextLevel") um.OnNextLevelButtonPressed();
                else if (methodName == "QuitGame") um.OnQuitButtonPressed();
            });
        }
    }
}
#endif
