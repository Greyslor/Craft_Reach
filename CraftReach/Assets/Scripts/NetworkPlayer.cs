using System.Collections;
using System.Collections.Generic;
using GameInput;
using Mirror;
using UnityEngine;
using UnityEngine.InputSystem;

public class NetworkPlayer : NetworkBehaviour
{
    private InputReader _inputReader;

    [Header("Referencias")]
    public GameObject floating;
    public TextMesh txtName;
    public TextMesh txtHealth;
    public GameObject[] weapons;

    [Header("Estadisticas")]
    [SyncVar(hook = nameof(OnHealthChanged))] public int health = 100;
    [SyncVar] public int maxHealth = 100;
    [SyncVar] public int shield = 100;
    public int maxShield = 100;

    [Header("Respawn")]
    public Transform[] respawnPoints;
    private bool isDead = false;

    [Header("Datos jugador")]
    [SyncVar(hook = nameof(OnNameChanged))] public string playerName;
    [SyncVar(hook = nameof(OnColorChanged))] public Color playerColor;
    [SyncVar(hook = nameof(OnWeaponChanged))] public int activeWeaponIndex;

    private Weapon activeWeapon;
    private float weaponCooldown;
    private Material myMaterial;

    private sceneScript scScript;
    private PlayerController controller;

    public override void OnStartLocalPlayer()
    {
        Camera.main.transform.SetParent(transform);
        Camera.main.transform.localPosition = Vector3.zero;

      

        string _name = "Player_" + Random.Range(100, 999);
        Color _color = new Color(Random.value, Random.value, Random.value);

        CmdSetupPlayer(_name, _color);
    }

    public override void OnStartClient()
    {
        scScript = FindAnyObjectByType<sceneScript>();
        controller = GetComponent<PlayerController>();

        foreach (GameObject w in weapons)
            w.SetActive(false);

        if (isLocalPlayer)
        {
            scScript.plMove = this;
        }

        SelectWeapon(activeWeaponIndex);
    }

    void Update()
    {
        if (!isLocalPlayer || isDead) return;

        //if (Input.GetButtonDown("Fire1"))
            //TryFire();

        if (Input.GetButtonDown("Fire2"))
            Melee();

        //if (Input.GetKeyDown(KeyCode.E))
          //  TryPickupHealth();

        if (Input.GetKeyDown(KeyCode.LeftControl))
            ShowScoreboard();
    }

    void TryFire()
    {
        if (!activeWeapon) return;
        if (Time.time < weaponCooldown) return;
        if (activeWeapon.ammo <= 0) return;

        weaponCooldown = Time.time + activeWeapon.cooldown;
        activeWeapon.ammo--;

        scScript.ammoUI(activeWeapon.ammo);

        CmdShoot();
    }

    void Melee()
    {

    }

    void TryPickupHealth()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, 2f);
        foreach (var hit in hits)
        {
            if (hit.CompareTag("Medkit"))
            {
                CmdHeal(50);
                Destroy(hit.gameObject);
                break;
            }
        }
    }

    void ShowScoreboard()
    {
        Debug.Log("Mostrar Scoreboard");

    }

    void SelectWeapon(int index)
    {
        for (int i = 0; i < weapons.Length; i++)
            weapons[i].SetActive(i == index);

        activeWeapon = weapons[index].GetComponent<Weapon>();
        if (isLocalPlayer)
            scScript.ammoUI(activeWeapon.ammo);
    }

    [Command]
    void CmdShoot()
    {
        GameObject bullet = Instantiate(activeWeapon.bullet, activeWeapon.firePos.position, activeWeapon.firePos.rotation);
        Rigidbody rb = bullet.GetComponent<Rigidbody>();

        if (rb != null)
            rb.velocity = bullet.transform.forward * activeWeapon.speed;

        NetworkServer.Spawn(bullet, connectionToClient); // el cliente que disparo tiene autoridad
    }


    /*[ClientRpc]
    void RpcShoot()
    {
        GameObject bullet = Instantiate(activeWeapon.bullet, activeWeapon.firePos.position, activeWeapon.firePos.rotation);
        NetworkServer.Spawn(bullet, connectionToClient);
        bullet.GetComponent<Rigidbody>().velocity = bullet.transform.forward * activeWeapon.speed;
    }*/


    [Command]
    void CmdHeal(int amount)
    {
        if (isDead) return;

        health = Mathf.Min(health + amount, maxHealth);
    }

    [Command]
    public void CmdSetupPlayer(string _name, Color _color)
    {
        playerName = _name;
        playerColor = _color;
        health = maxHealth;
    }

    [Command]
    public void CmdTakeDamage(int damage)
    {
        if (shield > 0)
        {
            shield -= damage;
            if (shield < 0)
            {
                health += shield; // shield es negativo
                shield = 0;
            }
        }
        else
        {
            health -= damage;
        }

        if (health <= 0)
            Die();
    }

    void Die()
    {
        isDead = true;
        gameObject.SetActive(false);
        StartCoroutine(RespawnCoroutine());
    }

    IEnumerator RespawnCoroutine()
    {
        yield return new WaitForSeconds(5f);
        Transform spawn = respawnPoints[Random.Range(0, respawnPoints.Length)];
        transform.position = spawn.position;
        health = maxHealth;
        shield = maxShield;
        gameObject.SetActive(true);
        isDead = false;
    }

    void OnHealthChanged(int oldVal, int newVal)
    {
        txtHealth.text = newVal.ToString();
        txtHealth.color = newVal >= 70 ? Color.green :
                          newVal >= 30 ? Color.yellow :
                                         Color.red;
    }

    void OnNameChanged(string _, string newName)
    {
        txtName.text = newName;
    }

    void OnColorChanged(Color _, Color newColor)
    {
        myMaterial = new Material(GetComponent<Renderer>().material);
        myMaterial.color = newColor;
        GetComponent<Renderer>().material = myMaterial;
        txtName.color = newColor;
    }

    void OnWeaponChanged(int _, int newIndex)
    {
        SelectWeapon(newIndex);
    }


    public override void OnStartAuthority()
    {
        _inputReader = new InputReader();
        _inputReader.FireEvent += TryFire;
        _inputReader.PickupEvent += TryPickupHealth;
    }

    void OnDisable()
    {
        if (isOwned && _inputReader != null)
        {
            _inputReader.FireEvent -= TryFire;
            _inputReader.PickupEvent -= TryPickupHealth;
        }
    }


}

