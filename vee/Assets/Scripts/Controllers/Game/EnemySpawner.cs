using MyGame;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using static Cinemachine.DocumentationSortingAttribute;

namespace Vs.Controllers.Game
{
    public sealed class EnemySpawner : SingletonMonoBehaviour<EnemySpawner>
    {
        [SerializeField]
        private Transform world;

        public bool IsCompleted { get; private set; }

        private readonly Dictionary<int, Enemy> enemyPrefabsCache = new Dictionary<int, Enemy>();
        private readonly Dictionary<int, ItemGate> itemGatePrefabsCache = new Dictionary<int, ItemGate>();

        public const int MAX_THIRD_ENEMY_COUNT = 10;
        public int onScreenEnemyCount = 0;
        public readonly Queue<Enemy> ThirdEmenyCache = new Queue<Enemy>();

        private float Timer = 0f; // タイマー
        private const float Interval = 1f; // 間隔（10秒）


        private void Start()
        {
            onScreenEnemyCount = 0;
            StartCoroutine(MainRoutine());
        }

        private IEnumerator MainRoutine()
        {
            yield return null;
            var enemyMst = Backend.MstDatas.Instance.Get("enemy_mst");
            var growthMst = Backend.MstDatas.Instance.Get("growth_mst");

            var waveMst = Backend.MstDatas.Instance.Get("wave_mst");
            var waves = waveMst;

            for (var i = 0; i < waves.Count;)
            {
                var raw = waves[i];
                var waveId = (int)raw["wave_id"];
                var count = 0;
                var weight = 0;
                for (var j = i; j < waves.Count; j++)
                {
                    var next = waves[j];
                    var nextId = (int)next["wave_id"];
                    if (nextId != waveId)
                    {
                        break;
                    }
                    count++;
                    weight += raw["weight"];
                }
                var rand = Random.Range(0, weight);
                var index = Mathf.FloorToInt((float)rand / weight * count);
                raw = waves[i + index];

                var enemyId = (int)raw["enemy_id"];
                var y = (int)raw["y"];
                var x = (int)(raw["x"] - 1) * 2;

                //プレイヤーが近くに来るまで待つ
                while (y > Camera.main.ViewportToWorldPoint(new Vector3(1, 1, -2*Camera.main.transform.position.z)).y)
                {
                    yield return null;
                }

                if (enemyId == 90000001)
                {
                    this.SpawnGate(enemyId, y, x);
                }
                else
                {
                    var sum = raw["number"];
                    for (var k = 0; k < sum; k++)
                    {
                        this.Spawn(enemyId, y, x, raw["level"]);
                    }
                }
                i += count;
            }
            GameManager.Instance.GameClear();
        }

        private void ClearEnemies()
        {
            GameManager.Instance.Clear();
        }

        private void SpawnGate(int enemyId, int y, int x)
        {
            var enemyMst = Backend.MstDatas.Instance.Get("enemy_mst");
            var raw = enemyMst.Find(i => i["enemy_id"] == enemyId);
            var modelId = raw["model_id"];
            if (!itemGatePrefabsCache.ContainsKey(modelId))
            { itemGatePrefabsCache.Add(modelId, Resources.Load<ItemGate>($"Enemies/{modelId}")); }
            var prefeb = itemGatePrefabsCache[modelId];
            var gate = Instantiate(prefeb, world);//(itemGateCache.Count > 0) ? itemGateCache.Dequeue() : Instantiate(prefeb, world);
            gate.transform.SetPositionAndRotation(new Vector3(x, y, 0), Quaternion.identity);
            var skill = GameManager.Instance.EquipmentManager.GetSelectableSkills()[0];
            gate.Initialize(skill);
            gate.SetDropId(raw["drop_id"]);
        }

        public void LimitSpawn(int enemyId, int y, int x, string name)
        {
            onScreenEnemyCount++;
            Spawn(enemyId, y, x, 1, true, name);
        }

        private void Spawn(int enemyId, int y, int x, int level, bool isLimit = false, string name = "")
        {
            var enemyMst = Backend.MstDatas.Instance.Get("enemy_mst");
            var growthMst = Backend.MstDatas.Instance.Get("growth_mst");

            var raw = enemyMst.Find(i => i["enemy_id"] == enemyId);
            if (raw == null)
            {
                Debug.Log(enemyId);
            }

            var growth = growthMst.Find(i => i["level"] == level);
            if (growth == null)
            {
                Debug.Log(enemyId);
            }

            var hp = Mathf.FloorToInt(raw["hp"] * growth["hp_rate"] / 1000.0f);
            var atk = Mathf.FloorToInt(raw["atk"] * growth["atk_rate"] / 1000.0f);
            var enemyType = (SengokuSurvivors.EnemyType)(int)raw["enemy_type"];
            var expAmount = (int)raw["exp_amount"];
            var irritate = (int)raw["irritate"];
            var score = (int)raw["score"];

            var modelId = raw["model_id"];
            if (!enemyPrefabsCache.ContainsKey(modelId))
            { enemyPrefabsCache.Add(modelId, Resources.Load<Enemy>($"Enemies/{modelId}")); }
            var prefeb = enemyPrefabsCache[modelId];

            Enemy enemy;

            enemy = Instantiate(prefeb, world);
            enemy.Initialize(this, name);
            enemy.isLimit = isLimit;
            enemy.SetEnemyId(enemyId);

            if (isLimit && onScreenEnemyCount > MAX_THIRD_ENEMY_COUNT)
            {
                enemy.gameObject.SetActive(false);
                ThirdEmenyCache.Enqueue(enemy);
                return;
            }

            enemy.transform.SetPositionAndRotation(new Vector3(x, y, 0), Quaternion.identity);
            enemy.SetHp(hp);
            enemy.SetAtk(atk);
            enemy.SetDropId(raw["drop_id"]);
            enemy.SetEnemyType(enemyType);
            enemy.SetExpAmount(expAmount);
            enemy.SetIrritate(irritate);
            enemy.SetScore(score);

            GameManager.Instance.RegisterEnemy(enemy);

            if (GameManager.Instance.onScreenEnemy.ContainsKey(enemyId))
            {
                GameManager.Instance.onScreenEnemy[enemyId]++;
            }
            else
            {
                GameManager.Instance.onScreenEnemy.Add(enemyId, 1);
            }
        }

        private void OnDestroy()
        {
            itemGatePrefabsCache.Clear();
            enemyPrefabsCache.Clear();
            Resources.UnloadUnusedAssets();
        }

        public Vector3 getRandumPosition()
        {
            // カメラのビューポート上端（ステージの奥）のY座標を取得（スクロールに対応）
            float stageTopY = Camera.main.ViewportToWorldPoint(new Vector3(1, 1, -2 * Camera.main.transform.position.z)).y;
            var enemy_pos_x = Random.Range(0, 3);
            var enemy_pos_y = stageTopY - 5;

            var position = new Vector3(enemy_pos_x, enemy_pos_y, 0);

            return position;
        }

        private void Update()
        {
            // 1秒ごとに実行
            Timer += Time.deltaTime;
            if (Timer >= Interval)
            {
                Timer = 0f;

                onScreenEnemyCount = 0;

                foreach (var enemy in GameManager.Instance.Enemies)
                {
                    if (enemy.isLimit)
                    {
                        onScreenEnemyCount++;
                    }
                }

                //敵がisLimitならキューにある敵を出現させる
                if (onScreenEnemyCount < MAX_THIRD_ENEMY_COUNT && ThirdEmenyCache.Count > 0)
                {
                    var _enemy = ThirdEmenyCache.Dequeue();
                    Vector3 enemy_position = EnemySpawner.Instance.getRandumPosition();
                    Spawn(_enemy.enemyId, (int)enemy_position.y, (int)enemy_position.x, 1, true, _enemy.name);

                    Despawn(_enemy);
                }
            }
        }

        public void Despawn(Enemy enemy)
        {
            //enemyCache.Enqueue(enemy);
            GameManager.Instance.DeregisterEnemy(enemy);
            Destroy(enemy.gameObject);

        }

        public void Despawn(ItemGate itemGate)
        {
            //itemGateCache.Enqueue(itemGate);
            Destroy(itemGate.gameObject);
        }
    }
}
