using System;
using System.Globalization;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

public class RemoteConfigLoader : MonoBehaviour
{
    [Header("Remote config")]
    [SerializeField] private string configUrl;
    [SerializeField] private string targetWeaponId = "pistol";

    [Header("Targets")]
    [SerializeField] private ShootingController targetShootingController;

    [Header("Defaults")]
    [SerializeField] private float defaultDamage = 20f;
    [SerializeField] private float defaultCooldown = 0.1f;

    [Header("Debug")]
    [SerializeField] private bool logRawConfig = false;

    private readonly Weapon weapon = new Weapon();
    public Weapon CurrentWeapon => weapon;

    private string CachePath => Path.Combine(Application.persistentDataPath, "weapon_config_cache.csv");

    private void Awake()
    {
        if (targetShootingController != null)
            targetShootingController.SetWeapon(weapon);
    }

    private void Start()
    {
        ApplyWeaponConfig(defaultDamage, defaultCooldown);

        StartCoroutine(LoadAndApply());
    }

    private System.Collections.IEnumerator LoadAndApply()
    {
        if (!string.IsNullOrWhiteSpace(configUrl))
        {
            using var req = UnityWebRequest.Get(configUrl);
            req.timeout = 10;

            yield return req.SendWebRequest();

            if (req.result == UnityWebRequest.Result.Success)
            {
                var text = req.downloadHandler.text;
                if (logRawConfig) Debug.Log($"Downloaded:\n{text}");

                if (TryParseAndValidateCsv(text, targetWeaponId, out var dmg, out var cd, out var err))
                {
                    SaveCache(text);
                    ApplyWeaponConfig(dmg, cd);
                    yield break;
                }

                Debug.LogError($"Remote config invalid: {err}. Will try cache.");
            }
            else
            {
                Debug.LogError($"Download failed: {req.error}. Will try cache.");
            }
        }
        else
        {
            Debug.LogWarning("configUrl is empty. Will try cache.");
        }

        if (TryLoadCache(out var cachedText))
        {
            if (logRawConfig) Debug.Log($"Cache loaded:\n{cachedText}");

            if (TryParseAndValidateCsv(cachedText, targetWeaponId, out var dmg, out var cd, out var err))
            {
                ApplyWeaponConfig(dmg, cd);
                yield break;
            }

            Debug.LogError($"Cache invalid: {err}. Using defaults.");
        }
        else
        {
            Debug.LogError("No cache found. Using defaults.");
        }
    }

    private void ApplyWeaponConfig(float damage, float cooldown)
    {
        weapon.Apply(damage, cooldown);
    }

    private void SaveCache(string text)
    {
        try
        {
            File.WriteAllText(CachePath, text, Encoding.UTF8);
            Debug.Log($"Cache saved: {CachePath}");
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to save cache: {e.Message}");
        }
    }

    private bool TryLoadCache(out string text)
    {
        text = null;
        try
        {
            if (!File.Exists(CachePath)) return false;
            text = File.ReadAllText(CachePath, Encoding.UTF8);
            return !string.IsNullOrWhiteSpace(text);
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to read cache: {e.Message}");
            return false;
        }
    }

    private bool TryParseAndValidateCsv(string csv, string weaponId, out float damage, out float cooldown, out string error)
    {
        damage = 0;
        cooldown = 0;
        error = null;

        if (string.IsNullOrWhiteSpace(csv))
        {
            error = "CSV is empty";
            return false;
        }

        var lines = csv.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
        if (lines.Length < 2)
        {
            error = "CSV has no data rows";
            return false;
        }

        for (int i = 1; i < lines.Length; i++)
        {
            var line = lines[i].Trim();
            if (string.IsNullOrWhiteSpace(line)) continue;

            var parts = line.Split(',');
            if (parts.Length < 3) continue;

            var id = parts[0].Trim();
            if (!string.Equals(id, weaponId, StringComparison.OrdinalIgnoreCase))
                continue;

            if (!TryParseFloat(parts[1], out var dmg))
            {
                error = $"Invalid damage for id={id}";
                return false;
            }

            if (!TryParseFloat(parts[2], out var cd))
            {
                error = $"Invalid cooldown for id={id}";
                return false;
            }

            if (dmg < 0)
            {
                error = $"Validation failed: damage < 0 for id={id}";
                return false;
            }

            if (cd <= 0)
            {
                error = $"Validation failed: cooldown <= 0 for id={id}";
                return false;
            }

            damage = dmg;
            cooldown = cd;
            return true;
        }

        error = $"Weapon id='{weaponId}' not found in CSV";
        return false;
    }

    private bool TryParseFloat(string s, out float value)
    {
        s = s.Trim();
        if (float.TryParse(s, NumberStyles.Float, CultureInfo.InvariantCulture, out value))
            return true;

        s = s.Replace(',', '.');
        return float.TryParse(s, NumberStyles.Float, CultureInfo.InvariantCulture, out value);
    }
}
