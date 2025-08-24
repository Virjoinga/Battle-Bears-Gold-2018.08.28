using ExitGames.Client.Photon;
using LitJson;
using UnityEngine;

public class LoadoutManager : MonoBehaviour
{
    private class LoadoutJSON
    {
        public int loadoutNumber;

        public string skin;

        public string taunt;

        public string primary;

        public string secondary;

        public string special;

        public string melee;

        public string equipment1;

        public string equipment2;
    }

    private const string HASH_PREF_NAME = "HASH";

    private static string _EMPTY_SLOT_NAME = "NONE";

    private static LoadoutManager instance;

    private PlayerLoadout _currentLoadout;

    private string _lastLoadoutPrefix = "LastLoadoutNumber";

    private string _lastCharacterPrefix = "LastCharacter";

    private string _defaultModelName = "Oliver";

    private string _lastCharacterUsed = "Oliver";

    private string _loadoutPrefix = "Loadout";

    public static string EMPTY_SLOT_NAME
    {
        get
        {
            return _EMPTY_SLOT_NAME;
        }
    }

    public static LoadoutManager Instance
    {
        get
        {
            return instance;
        }
    }

    public string LoadoutPrefix
    {
        get
        {
            return _loadoutPrefix;
        }
    }

    public PlayerLoadout CurrentLoadout
    {
        get
        {
            return _currentLoadout;
        }
        set
        {
            _currentLoadout = value;
            SaveLoadout(ServiceManager.Instance.GetStats().pid);
        }
    }

    private void Awake()
    {
        instance = this;
        UpdateLocalizedText();
    }

    private void UpdateLocalizedText()
    {
        _EMPTY_SLOT_NAME = Language.Get("GEARUP_NONE");
    }

    public string GetLastCharacterForLoadout(int loadoutNumber)
    {
        return PlayerPrefs.GetString(_lastCharacterPrefix + ServiceManager.Instance.GetStats().pid + " " + loadoutNumber, _defaultModelName);
    }

    public Item GetLoadoutItem(int loadoutNumber)
    {
        return ServiceManager.Instance.GetItemByName(_loadoutPrefix + loadoutNumber);
    }

    public PlayerLoadout GetLoadoutByNumber(int loadoutNumber, string model = "")
    {
        // Always use default, ignore saved data
        return CreateDefaultLoadout(loadoutNumber, string.Empty);
    }


    public PlayerLoadout LoadLastLoadout(int playerID)
    {
        // Always force default loadout #1
        _currentLoadout = CreateDefaultLoadout(1, string.Empty);
        return _currentLoadout;
    }

    public PlayerLoadout LoadLoadout(int pid, string model, int loadoutNumber)
    {
        bool replaceItemOccurred;
        return LoadLoadout(pid, model, loadoutNumber, out replaceItemOccurred);
    }

    public PlayerLoadout LoadLoadout(int pid, string model, int loadoutNumber, out bool replaceItemOccurred)
    {
        replaceItemOccurred = false;

        // Always use default loadout regardless of prefs/saves
        return CreateDefaultLoadout(loadoutNumber, model);
    }

    public PlayerLoadout CreateLoadout(int pid, string model, string loadoutString, int loadoutNumber, bool remotePlayer = false)
    {
        bool replaceItemOccurred = false;
        return CreateLoadout(pid, model, loadoutString, loadoutNumber, out replaceItemOccurred, remotePlayer);
    }

    public PlayerLoadout CreateLoadout(int pid, string model, string loadoutString, int loadoutNumber, out bool replaceItemOccurred, bool remotePlayer = false)
    {
        // Always default
        replaceItemOccurred = false;
        return CreateDefaultLoadout(loadoutNumber, model);
    }
    public PlayerLoadout CreateDefaultLoadout(int loadoutNumber, string model = "")
    {
        PlayerLoadout playerLoadout = new PlayerLoadout();
        if (model == null || model.Equals(string.Empty))
        {
            model = _defaultModelName;
        }
        playerLoadout.model = Store.Instance.characters[model].characterData;
        playerLoadout.model = ServiceManager.Instance.GetItemByName(model);
        playerLoadout.pid = ServiceManager.Instance.GetStats().pid;
        playerLoadout.loadoutNumber = loadoutNumber;
        playerLoadout.skin = Store.Instance.characters[model].skins[0];
        for (int i = 0; i < Store.Instance.characters[model].skins.Count; i++)
        {
            if (Store.Instance.characters[model].skins[i].is_default)
            {
                playerLoadout.skin = Store.Instance.characters[model].skins[i];
            }
        }
        playerLoadout.primary = Store.Instance.characters[model].primaryWeapons[0];
        for (int j = 0; j < Store.Instance.characters[model].primaryWeapons.Count; j++)
        {
            if (Store.Instance.characters[model].primaryWeapons[j].is_default)
            {
                playerLoadout.primary = Store.Instance.characters[model].primaryWeapons[j];
            }
        }
        playerLoadout.secondary = Store.Instance.characters[model].secondaryWeapons[0];
        for (int k = 0; k < Store.Instance.characters[model].secondaryWeapons.Count; k++)
        {
            if (Store.Instance.characters[model].secondaryWeapons[k].is_default)
            {
                playerLoadout.secondary = Store.Instance.characters[model].secondaryWeapons[k];
            }
        }
        playerLoadout.melee = Store.Instance.characters[model].meleeWeapons[0];
        for (int l = 0; l < Store.Instance.characters[model].meleeWeapons.Count; l++)
        {
            if (Store.Instance.characters[model].meleeWeapons[l].is_default)
            {
                playerLoadout.melee = Store.Instance.characters[model].meleeWeapons[l];
            }
        }
        return playerLoadout;
    }

    public void SaveLoadout(int pid)
    {
        string text = CreateJSONForLoadout(_currentLoadout);
        string value = HashUtils.GenerateHash(text);
        PlayerPrefs.SetString(pid + _currentLoadout.model.name + _currentLoadout.loadoutNumber + "HASH", value);
        PlayerPrefs.SetString(pid + _currentLoadout.model.name + _currentLoadout.loadoutNumber, text);
        PlayerPrefs.SetString(_lastCharacterPrefix + pid + " " + _currentLoadout.loadoutNumber, _currentLoadout.model.name);
        PlayerPrefs.SetInt(_lastLoadoutPrefix + ServiceManager.Instance.GetStats().pid, _currentLoadout.loadoutNumber);
        PlayerPrefs.SetString(_loadoutPrefix + _currentLoadout.loadoutNumber + " " + ServiceManager.Instance.GetStats().pid, text);
    }

    public string CreateJSONForLoadout(PlayerLoadout givenLoadout)
    {
        LoadoutJSON loadoutJSON = new LoadoutJSON();
        loadoutJSON.loadoutNumber = givenLoadout.loadoutNumber;
        loadoutJSON.skin = givenLoadout.skin.name;
        if (givenLoadout.taunt != null)
        {
            loadoutJSON.taunt = givenLoadout.taunt.name;
        }
        loadoutJSON.primary = givenLoadout.primary.name;
        loadoutJSON.secondary = givenLoadout.secondary.name;
        loadoutJSON.melee = givenLoadout.melee.name;
        if (_currentLoadout.special != null)
        {
            loadoutJSON.special = givenLoadout.special.name;
        }
        else
        {
            loadoutJSON.special = string.Empty;
        }
        if (_currentLoadout.equipment1 != null)
        {
            loadoutJSON.equipment1 = givenLoadout.equipment1.name;
        }
        else
        {
            loadoutJSON.equipment1 = string.Empty;
        }
        if (_currentLoadout.equipment2 != null)
        {
            loadoutJSON.equipment2 = givenLoadout.equipment2.name;
        }
        else
        {
            loadoutJSON.equipment2 = string.Empty;
        }
        return JsonMapper.ToJson(loadoutJSON);
    }

    public Hashtable GetPhotonUserParametersForCurrentLoadout()
    {
        string text = ((!string.IsNullOrEmpty(Bootloader.Instance.socialName)) ? Bootloader.Instance.socialName : ("Player " + ServiceManager.Instance.GetStats().pid));
        text = text.Replace(' ', '_');
        PlayerLoadout currentLoadout = Instance.CurrentLoadout;
        HandleEnteringRoyaleMode(currentLoadout);
        Stats stats = ServiceManager.Instance.GetStats();
        Hashtable hashtable = new Hashtable();
        //  hashtable.Add((byte)87, currentLoadout.model.name);
        //  hashtable.Add((byte)88, currentLoadout.skin.name);
        //  hashtable.Add((byte)89, currentLoadout.primary.name);
        //  hashtable.Add((byte)90, currentLoadout.secondary.name);
        // hashtable.Add((byte)91, currentLoadout.melee.name);
        //  hashtable.Add((byte)92, (currentLoadout.special != null && !string.IsNullOrEmpty(currentLoadout.special.name)) ? currentLoadout.special.name : EMPTY_SLOT_NAME);
        //  hashtable.Add((byte)93, (currentLoadout.equipment1 != null && !string.IsNullOrEmpty(currentLoadout.equipment1.name)) ? currentLoadout.equipment1.name : EMPTY_SLOT_NAME);
        // hashtable.Add((byte)104, (currentLoadout.equipment2 != null && !string.IsNullOrEmpty(currentLoadout.equipment2.name)) ? currentLoadout.equipment2.name : EMPTY_SLOT_NAME);
        // hashtable.Add((byte)84, text);
        // hashtable.Add((byte)106, stats.pid);
        //hashtable.Add((byte)107, (int)stats.level);
        //hashtable.Add((byte)108, (int)stats.skill);
        //hashtable.Add((byte)109, (currentLoadout.taunt != null && !string.IsNullOrEmpty(currentLoadout.taunt.name)) ? currentLoadout.taunt.name : EMPTY_SLOT_NAME);
        // hashtable.Add((byte)110, ServiceManager.Instance.NameColor);
        return hashtable;
    }

    private void HandleEnteringRoyaleMode(PlayerLoadout loadout)
    {
        GameMode currentGameMode = Preferences.Instance.CurrentGameMode;
        if (currentGameMode == GameMode.ROYL)
        {
            SaveLoadout(ServiceManager.Instance.GetStats().pid);
            loadout.primary = loadout.melee;
            loadout.secondary = loadout.melee;
            loadout.special = null;
            loadout.equipment1 = null;
            loadout.equipment2 = null;
            _currentLoadout = loadout;
        }
    }
}
