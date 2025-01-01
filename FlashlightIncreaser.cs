using UnityEngine;
#if ML
using Il2Cpp;
#elif BIE
using BepInEx.IL2CPP;
#endif

namespace Mod;

public static class FlashlightIncreaser
{
    private const float NOT_INITIALIZED = -1.0f;
    private const float FLASHLIGHT_RANGE = 1000.0f;
    private const float FLASHLIGHT_SPOT_ANGLE = 70.0f;

    private static bool _enabled = true;
    public static bool Enabled
    {
        get => _enabled;
        set
        {
            _enabled = value;

            if (value)
            {
                ModCore.Loader.Update += OnUpdate;
            }
            else
            {
                ModCore.Loader.Update -= OnUpdate;
                if (_isFlashlightEnabled)
                {
                    Toggle();
                }
            }

            ModCore.Log(value ? "Enabled" : "Disabled");
        }
    }

    private static bool _isFlashlightEnabled = false;
    private static WorldPlayer? _worldPlayer;
    private static float _savedFlashlightRange = NOT_INITIALIZED;
    private static float _savedFlashlightSpotAngle = NOT_INITIALIZED;

    public static void Init()
    {
        if (Enabled)
        {
            ModCore.Loader.Update += OnUpdate;
        }

        ModCore.Log("Initialized");
    }

    public static bool Toggle()
    {
        _isFlashlightEnabled = !_isFlashlightEnabled;
        if (_isFlashlightEnabled)
        {
            ActivateFlashlightFeatures();
        }
        else
        {
            RevertFlashlightState();
        }

        ModCore.Log($"Flashlight {(_isFlashlightEnabled ? "increased" : "restored")}");

        return _isFlashlightEnabled;
    }

    private static void OnUpdate()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            Toggle();
        }
    }

    private static void ActivateFlashlightFeatures()
    {
        try
        {
            WorldPlayer? worldPlayer = GetWorldPlayer();
            if (worldPlayer == null)
            {
                ModCore.LogError($"Object {nameof(WorldPlayer)} not found!");

                ResetState();
                return;
            }

            _savedFlashlightRange = worldPlayer.flashLightRange;
            _savedFlashlightSpotAngle = worldPlayer.flashLightSpotAngle;

            worldPlayer.flashLightRange = FLASHLIGHT_RANGE;
            worldPlayer.flashLightSpotAngle = FLASHLIGHT_SPOT_ANGLE;
        }
        catch (Exception e)
        {
            ModCore.LogError(e.Message);

            ResetState();
        }
    }

    private static void RevertFlashlightState()
    {
        try
        {
            if (
                _worldPlayer == null
                || _savedFlashlightRange <= NOT_INITIALIZED
                || _savedFlashlightSpotAngle <= NOT_INITIALIZED
            )
            {
                ResetState();
                return;
            }

            _worldPlayer.flashLightRange = _savedFlashlightRange;
            _worldPlayer.flashLightSpotAngle = _savedFlashlightSpotAngle;

            ResetState();
        }
        catch (Exception e)
        {
            ModCore.LogError(e.Message);

            ResetState();
        }
    }

    private static WorldPlayer? GetWorldPlayer()
    {
        if (!IsWoldPlayerValid())
        {
            _worldPlayer = GameObject.Find("World")?.GetComponent<WorldPlayer>();
        }
        return _worldPlayer;
    }

    private static bool IsWoldPlayerValid()
    {
        return _worldPlayer != null && _worldPlayer.gameObject != null;
    }

    private static void ResetState()
    {
        _worldPlayer = null;
        _isFlashlightEnabled = false;
        _savedFlashlightRange = NOT_INITIALIZED;
        _savedFlashlightSpotAngle = NOT_INITIALIZED;
    }
}
