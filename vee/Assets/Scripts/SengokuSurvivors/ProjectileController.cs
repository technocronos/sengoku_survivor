using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class ProjectileController : MonoBehaviour
{
    [SerializeField]
    private Transform world;
    [SerializeField]
    private ArrowProjectile arrowPref;
    [SerializeField]
    private ShurikenProjectile shurikenPref;

    [System.NonSerialized]
    public int ShurikenDamage = 0;
    [System.NonSerialized]
    public int ArrowDamage = 0;

    public float CooldownShuriken { get { return cooldownShuriken; } }
    public float CooldownArrow { get { return cooldownArrow; } }
    public int ArrowCount { get {
            if (ArrowDamage == 0) return 0;
            if (ArrowDamage < 22) return 1;
            if (ArrowDamage < 24) return 2;
            return 3;
        } }
    public int ShurikenCount { get {
            if (ShurikenDamage == 0) return 0;
            if (ShurikenDamage < 22) return 1;
            if (ShurikenDamage < 24) return 2;
            return 3;
        } }
    private float cooldownShuriken = 10f;
    private float cooldownArrow = 10f;
    private int arrowId = 903;
    private int shurikenId = 902;

    private readonly Stack<ArrowProjectile> projectilesCacheArrow = new Stack<ArrowProjectile>();
    private readonly Stack<ShurikenProjectile> projectilesCacheShuriken = new Stack<ShurikenProjectile>();

    private void Start()
    {
        StartCoroutine(ArrowRoutine());
        StartCoroutine(ShurikenRoutine());
    }

    public void RemoveProjectile(ArrowProjectile arrow)
    {
        projectilesCacheArrow.Push(arrow);
        arrow.gameObject.SetActive(false);
    }

    public ArrowProjectile PlaceArrow(Vector2 start, Vector2 dir)
    {
        ArrowProjectile arrow;
        if (projectilesCacheArrow.Count < 1)
        {
            arrow = Instantiate(arrowPref, world);
        }
        else
        {
            arrow = projectilesCacheArrow.Pop();
        }

        arrow.gameObject.SetActive(true);
        arrow.StartMoving(start, dir);
        return arrow;
    }

    public void RemoveProjectile(ShurikenProjectile shuriken)
    {
        projectilesCacheShuriken.Push(shuriken);
        shuriken.gameObject.SetActive(false);
    }

    public ShurikenProjectile PlaceShuriken(Vector2 start, Vs.Controllers.Game.Enemy target)
    {
        ShurikenProjectile shuriken;
        if (projectilesCacheShuriken.Count < 1)
        {
            shuriken = Instantiate(shurikenPref, world);
        }
        else
        {
            shuriken = projectilesCacheShuriken.Pop();
        }

        shuriken.gameObject.SetActive(true);
        shuriken.StartMoving(start, target);
        return shuriken;
    }

    private IEnumerator ShurikenRoutine()
    {
        while (true)
        {
            yield return null;
            UpdateWeaponParameters();
            if (ShurikenDamage == 0) continue;

            FindEnemyAndPlaceShuriken();
            //Vs.SoundService.Instance.PlaySe(soundIdShuriken);
            yield return new WaitForSeconds(cooldownShuriken);
        }
    }

    private IEnumerator ArrowRoutine()
    {
        while (true)
        {
            yield return null;
            UpdateWeaponParameters();
            if (ArrowDamage == 0) continue;

            PlaceArrow(ArrowCount);
            //Vs.SoundService.Instance.PlaySe(soundIdArrow);
            yield return new WaitForSeconds(cooldownArrow);
        }
    }

    private void UpdateWeaponParameters()
    {
        var weaponData = Vs.Controllers.Game.GameManager.Instance.SkillManager
                .GetCurrentSkills().Find(i => i.SkillId == shurikenId);
        if (weaponData != null)
        {
            ShurikenDamage = weaponData.Atk;
            cooldownShuriken = weaponData.CoolTime / 1000f * weaponData.CoolTimeMulti;
        }

        weaponData = Vs.Controllers.Game.GameManager.Instance.SkillManager
                .GetCurrentSkills().Find(i => i.SkillId == arrowId);
        if (weaponData != null)
        {
            ArrowDamage = weaponData.Atk;
            cooldownArrow = weaponData.CoolTime / 1000f * weaponData.CoolTimeMulti;
        }
    }

    private void PlaceArrow(int count = 1)
    {
        if (count == 1)
        { PlaceArrow(transform.position, Vector3.up); }
        else if (count == 2)
        {
            PlaceArrow(transform.position + 0.15f * Vector3.right, Vector3.up);
            PlaceArrow(transform.position + 0.15f * Vector3.left, Vector3.up);
        }
        else
        {
            PlaceArrow(transform.position, Vector3.up);
            PlaceArrow(transform.position + 0.3f * Vector3.right, Vector3.up);
            PlaceArrow(transform.position + 0.3f * Vector3.left, Vector3.up);
        }
    }

    private void FindEnemyAndPlaceShuriken()
    {
        var enemies = FindObjectsByType<Vs.Controllers.Game.Enemy>(FindObjectsSortMode.None);
        var c = enemies.Length;
        int indx = 0;
        int indx2 = 1;
        int indx3 = 2;
        if (c < 1) return;
        var distanceMin = (enemies[0].transform.position - transform.position).magnitude;
        var min2 = (enemies.Length > 1) ? (enemies[1].transform.position - transform.position).magnitude : distanceMin;
        var min3 = (enemies.Length > 2) ? (enemies[2].transform.position - transform.position).magnitude : distanceMin;
        for (int i = 1; i < c; i++)
        {
            var distance = (enemies[i].transform.position - transform.position).magnitude;
            if (distanceMin > distance)
            {
                min3 = min2;
                indx3 = indx2;
                min2 = distanceMin;
                indx2 = indx;
                distanceMin = distance;
                indx = i;
            }
        }
        PlaceShuriken(transform.position, enemies[indx]);
        if (ShurikenCount >= 2 && enemies.Length > 1) PlaceShuriken(transform.position, enemies[indx2]);
        if (ShurikenCount >= 3 && enemies.Length > 2) PlaceShuriken(transform.position, enemies[indx3]);
    }
}
