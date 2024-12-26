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
    private static WorldPlayer? _player;
    private static float _savedFlashlightRange = NOT_INITIALIZED;
    private static float _savedFlashlightSpotAngle = NOT_INITIALIZED;

    public static void Init()
    {
        if (Enabled)
        {
            ModCore.Loader.Update += OnUpdate;
        }

        ModCore.Log("Mod Initialized");
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

        ModCore.Log("Flashlight " + (_isFlashlightEnabled ? "increased" : "restored"));
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
            _player = UnityEngine.Object.FindObjectOfType<WorldPlayer>();
            if (_player is null)
            {
                ModCore.LogError($"Object {nameof(WorldPlayer)} not found!");

                _isFlashlightEnabled = false;
                ResetSavedFlashlightState();
                return;
            }

            _savedFlashlightRange = _player.flashLightRange;
            _savedFlashlightSpotAngle = _player.flashLightSpotAngle;

            _player.flashLightRange = FLASHLIGHT_RANGE;
            _player.flashLightSpotAngle = FLASHLIGHT_SPOT_ANGLE;
        }
        catch (Exception e)
        {
            ModCore.LogError(e.Message);

            _isFlashlightEnabled = false;
            _player = null;
            ResetSavedFlashlightState();
        }
    }

    private static void RevertFlashlightState()
    {
        try
        {
            if (
                _player is null
                || _savedFlashlightRange <= NOT_INITIALIZED
                || _savedFlashlightSpotAngle <= NOT_INITIALIZED
            )
            {
                return;
            }

            _player.flashLightRange = _savedFlashlightRange;
            _player.flashLightSpotAngle = _savedFlashlightSpotAngle;
            _player = null;

            ResetSavedFlashlightState();
        }
        catch (Exception e)
        {
            ModCore.LogError(e.Message);

            _isFlashlightEnabled = false;
            _player = null;
            ResetSavedFlashlightState();
        }
    }

    private static void ResetSavedFlashlightState()
    {
        _savedFlashlightRange = NOT_INITIALIZED;
        _savedFlashlightSpotAngle = NOT_INITIALIZED;
    }
}
