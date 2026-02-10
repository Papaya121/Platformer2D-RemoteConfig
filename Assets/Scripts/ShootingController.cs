using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class ShootingController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform gunPivot;
    [SerializeField] private Transform firePoint;
    [SerializeField] private Bullet bulletPrefab;

    [Header("Shooting Settings")]
    [SerializeField] private float bulletSpeed = 12f;
    [SerializeField] private int bulletDamage = 1;
    [SerializeField] private float fireCooldown = 0.1f;
    [SerializeField] private bool mirrorOnFlip = true;

    [Header("Touch Settings")]
    [SerializeField] private bool allowTouchShoot = true;
    [SerializeField] private bool allowTouchAim = true;

    private Camera _camera;
    private float _nextFireTime;
    private Vector3 _initialPivotScale = Vector3.one;
    private Weapon _weapon;

    private void Awake()
    {
        _camera = Camera.main;
        if (gunPivot != null)
        {
            _initialPivotScale = gunPivot.localScale;
        }

        EnsureWeaponInitialized();
    }

    private void Update()
    {
        AimAtPointer();

        if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
        {
            if (IsPointerOverUI())
                return;

            TryShoot();
        }

        HandleTouchInput();
    }

    private void AimAtPointer()
    {
        if (_camera == null || gunPivot == null || Time.timeScale == 0)
            return;

        Vector2? pointerScreenPosition = GetPointerScreenPositionForAim();
        if (!pointerScreenPosition.HasValue)
            return;

        AimAtScreenPosition(pointerScreenPosition.Value);
    }

    private void AimAtScreenPosition(Vector2 screenPosition)
    {
        if (_camera == null || gunPivot == null)
            return;

        Vector3 pointerWorldPosition = _camera.ScreenToWorldPoint(screenPosition);
        Vector2 direction = pointerWorldPosition - gunPivot.position;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        gunPivot.rotation = Quaternion.Euler(0f, 0f, angle);

        if (mirrorOnFlip && Mathf.Abs(direction.x) > 0.001f)
        {
            var scale = _initialPivotScale;
            scale.y = direction.x < 0 ? -Mathf.Abs(scale.y) : Mathf.Abs(scale.y);
            gunPivot.localScale = scale;
        }
    }

    private void TryShoot()
    {
        if (firePoint == null || bulletPrefab == null || Time.timeScale == 0)
            return;

        if (Time.time < _nextFireTime)
            return;

        int damage = GetDamage();
        float cooldown = GetCooldown();

        Bullet bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
        bullet.Initialize(firePoint.right, bulletSpeed, damage);

        AudioManager.PlaySound("Shoot");

        _nextFireTime = Time.time + cooldown;
    }

    public void SetDamage(int damage)
    {
        EnsureWeaponInitialized();
        _weapon.Apply(damage, _weapon.cooldown);
    }

    public void SetCooldown(float cooldown)
    {
        EnsureWeaponInitialized();
        _weapon.Apply(_weapon.damage, cooldown);
    }

    public void ApplyWeaponConfig(float damage, float cooldown)
    {
        EnsureWeaponInitialized();
        _weapon.Apply(damage, cooldown);
    }

    public void SetWeapon(Weapon weapon)
    {
        if (weapon == null)
        {
            Debug.LogWarning("[ShootingController] SetWeapon called with null. Using defaults.");
            EnsureWeaponInitialized();
            return;
        }

        _weapon = weapon;
    }

    private static bool IsPointerOverUI()
    {
        if (EventSystem.current == null)
            return false;

        return EventSystem.current.IsPointerOverGameObject();
    }

    private void HandleTouchInput()
    {
        if (!allowTouchShoot || Touchscreen.current == null || Time.timeScale == 0)
            return;

        var touches = Touchscreen.current.touches;
        int touchCount = touches.Count;
        for (int i = 0; i < touchCount; i++)
        {
            var touch = touches[i];
            if (!touch.press.wasPressedThisFrame)
                continue;

            if (IsPointerOverUI(touch.touchId.ReadValue()))
                continue;

            if (allowTouchAim)
            {
                AimAtScreenPosition(touch.position.ReadValue());
            }

            TryShoot();
            break;
        }
    }

    private Vector2? GetPointerScreenPositionForAim()
    {
        if (allowTouchAim && Touchscreen.current != null)
        {
            var touches = Touchscreen.current.touches;
            int touchCount = touches.Count;
            for (int i = 0; i < touchCount; i++)
            {
                var touch = touches[i];
                if (!touch.press.isPressed)
                    continue;

                if (IsPointerOverUI(touch.touchId.ReadValue()))
                    continue;

                return touch.position.ReadValue();
            }
        }

        if (Mouse.current != null)
        {
            return Mouse.current.position.ReadValue();
        }

        return null;
    }

    private static bool IsPointerOverUI(int pointerId)
    {
        if (EventSystem.current == null)
            return false;

        return EventSystem.current.IsPointerOverGameObject(pointerId);
    }

    private void EnsureWeaponInitialized()
    {
        if (_weapon != null)
            return;

        _weapon = new Weapon();
        _weapon.Apply(bulletDamage, fireCooldown);
    }

    private int GetDamage()
    {
        float damage = _weapon != null ? _weapon.damage : bulletDamage;
        return Mathf.Max(0, Mathf.RoundToInt(damage));
    }

    private float GetCooldown()
    {
        float cooldown = _weapon != null ? _weapon.cooldown : fireCooldown;
        return Mathf.Max(0.01f, cooldown);
    }
}
