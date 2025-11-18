using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Vs.Controllers.Game
{
    public sealed class EnemySpawner : MonoBehaviour
    {
        [SerializeField]
        private Transform world;

        public bool IsCompleted { get; private set; }

        private readonly Dictionary<int, Enemy> enemyPrefabsCache = new Dictionary<int, Enemy>();
        private readonly Dictionary<int, ItemGate> itemGatePrefabsCache = new Dictionary<int, ItemGate>();

        private void Start()
        {
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
            var skill = GameManager.Instance.SkillManager.GetSelectableSkills()[0];
            gate.Initialize(skill);
            gate.SetDropId(raw["drop_id"]);
        }

        private void Spawn(int enemyId, int y, int x, int level)
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

            var modelId = raw["model_id"];
            if (!enemyPrefabsCache.ContainsKey(modelId))
            { enemyPrefabsCache.Add(modelId, Resources.Load<Enemy>($"Enemies/{modelId}")); }
            var prefeb = enemyPrefabsCache[modelId];
            var enemy = Instantiate(prefeb, world);
            GameManager.Instance.RegisterEnemy(enemy);
            enemy.Initialize(this);
            enemy.transform.SetPositionAndRotation(new Vector3(x, y, 0), Quaternion.identity);
            enemy.SetHp(hp);
            enemy.SetAtk(atk);
            enemy.SetDropId(raw["drop_id"]);
            enemy.SetEnemyType(enemyType);
            enemy.SetExpAmount(expAmount);
        }

        private void OnDestroy()
        {
            itemGatePrefabsCache.Clear();
            enemyPrefabsCache.Clear();
            Resources.UnloadUnusedAssets();
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
