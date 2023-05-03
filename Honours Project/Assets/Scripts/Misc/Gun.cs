using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Gun : MonoBehaviour
{
    [Header("Bullet Info")]
    [SerializeField] private Transform bulletSpawnPoint;
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private float bulletSpeed;

    [Header("Ammo Info")]
    [SerializeField] private int maxAmmo = 5;

    private int ammo;

    public List<string> hitList;
    public List<int> abc;
    private void Awake()
    {
        hitList = new List<string>();
        abc = new List<int>();
        ammo = maxAmmo;

    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && ammo > 0)
        {
            Shoot();
        }
        if (Input.GetKeyDown(KeyCode.R) && ammo < maxAmmo)
        {
            Reload();
        }
    }

    public void Shoot()
    {
        if (ammo > 0)
        {
            ammo--;
            var bullet = Instantiate(bulletPrefab, bulletSpawnPoint.position, bulletSpawnPoint.rotation);
            bullet.GetComponent<Rigidbody>().velocity = bulletSpawnPoint.forward * bulletSpeed;
        }
    }

    public void Reload()
    {
        if (ammo < maxAmmo)
            ammo = maxAmmo;
    }

    public float GetAmmo()
    {
        return ammo;
    }

    public void AddToList(string item)
    {
        hitList.Add(item);
    }

    public void RemoveFromList(string item)
    {
        hitList.Remove(item);
    }

    public List<string> GetHitList()
    {
        return hitList;
    }

}
