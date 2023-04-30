using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

public delegate (float mod, bool front) Modifier(float preVal);
public delegate T Construction<T>();
public class GameManager : MonoBehaviour
{
    [SerializeField]
    private Sprite[] _sprites;
    public event Action<MilestoneInfo> MilestoneConditionMet;
    public event Action<AutoBuff> AutoBuffConditionMet;
    public event Action<BuffEffect> BuffAdded;

    public static GameManager GAME;

    public readonly static float STARTING_CPS = 0; //cash per second?
    public readonly static float STARTING_CPC = 1; //cash per click 
    public readonly static float STARTING_SPM = 0; // sustain per mintue

    public readonly static float STARTING_CASH = 0;
    public readonly static float STARTING_SUSTAIN = 100;
    public float Cash { get; set; }
    public float Sustain { get; set; }

    public float CPS { get; private set; }
    public float CPC { get; private set; }
    public float SPM { get; private set; }

    private List<Modifier> _clickModifiers;
    private List<Modifier> _idleModifiers;
    private List<Modifier> _sustainModifiers;

    private List<ItemInfo> _availableItems;
    private List<BuffEffect> _activeBuffs;
    private List<MilestoneInfo> _availableMilestones;
    private List<AutoBuff> _autoBuffs;

    private void NewGame()
    {
        CPS = STARTING_CPS;
        CPC = STARTING_CPC;
        SPM = STARTING_SPM;
        Cash = STARTING_CASH;
        Sustain = STARTING_SUSTAIN;
        _clickModifiers = new();
        _idleModifiers = new();
        _sustainModifiers = new();
        Setup();
    }
    private void Awake()
    {
        GAME = this;
        NewGame();
        OneSecUpdate();
    }

    async Task OneSecUpdate()
    {
        foreach (var milestone in _availableMilestones)
        {
            if (milestone.Condition())
            {
                MilestoneConditionMet(milestone);
                _availableMilestones.Remove(milestone);
            }
        }
        foreach (var auto in _autoBuffs)
        {
            if (auto.Condition())
            {
                AutoBuffConditionMet(auto);
                _autoBuffs.Remove(auto);
            }
        }
        await Task.Delay(1000);
        OneSecUpdate();
    }

    public void OnClick()
    {
        Cash += CPC;
    }

    public void Refresh()
    {
        float s = STARTING_CPC;
        foreach (var func in _clickModifiers)
        {
            (s, _) = func(s);
        }
        CPC = s;

        s = STARTING_CPS;
        foreach (var func in _idleModifiers)
        {
            (s, _) = func(s);
        }
        CPS = s;

        s = STARTING_SPM;
        foreach (var func in _sustainModifiers)
        {
            (s, _) = func(s);
        }
        SPM = s;
    }
    public void AddBuffEffect(BuffEffect buff)
    {
        BuffAdded(buff);
        buff.OnGetAction();
        _activeBuffs.Add(buff);
        Refresh();
    }
    public void AddClickMod(Modifier mod)
    {
        (_, bool front) = mod(CPC);
        if (front) _clickModifiers.Insert(0, mod);
        else _clickModifiers.Add(mod);
        float s = STARTING_CPC;
        foreach (var func in _clickModifiers)
        {
            (s, _) = func(s);
        }
        CPC = s;
    }
    public void AddIdleMod(Modifier mod)
    {
        (_, bool front) = mod(CPS);
        if (front) _idleModifiers.Insert(0, mod);
        else _idleModifiers.Add(mod);
        float s = STARTING_CPS;
        foreach (var func in _idleModifiers)
        {
            (s, _) = func(s);
        }
        CPS = s;
    }
    public void AddSustainMod(Modifier mod)
    {
        (_, bool front) = mod(SPM);
        if (front) _sustainModifiers.Insert(0, mod);
        else _sustainModifiers.Add(mod);
        float s = STARTING_SPM;
        foreach (var func in _sustainModifiers)
        {
            (s, _) = func(s);
        }
        SPM = s;
    }

    public void ShopTryBuy(int index)
    {
        if (index >= _availableItems.Count)
        {
            Debug.Log("This item is not unlocked yet!");
            return;
        }
        ItemInfo item = _availableItems[index];
        if (item.Price > Cash)
        {
            Debug.Log(item.DisplayName + " is too expensive: $" + item.Price);
            return;
        }
        if (!item.AdditionalBuyConditions(item))
        {
            Debug.Log("You do not meet the conditions to buy a " + item.DisplayName);
            return;
        }
        Debug.Log($"{item.DisplayName} bought for {item.Price}. ({item.AmountOwned} owned)");
        item.Buy();
        

    }
    private void Setup()
    {
        //ITEMS
        _availableItems = new()
        {
            new ItemInfo()
            {//bioderadiable producs, solar panels, domestic goods, wind turbine, plastic packaging, Petrolium barrels, imported goods, oil rig
              //
                ID = "biodegradeable",
                DisplayName = "Biodegradeable Products",
                Desc = "B",
                Categories = ItemInfo.ECategory.Sustainable,
                PriceFunction = i => 10 + 1*i.AmountOwned,
                IdleMod = v => (v + 1, true)
                
            }
           
        };

        //MILESTONES
        _availableMilestones = new()
        {
            new MilestoneInfo()
            {
                ID = "test_milestone",
                DisplayName = "Test Milestone",
                Desc = "D",
                Condition = () => Cash > 100,
                Choices =
                {
                    () => new BuffEffect()
                    {
                        ID = "test_buff",
                        DisplayName = "Test Milestone",
                        Desc = "D",
                        OnGetAction = () =>
                        {
                            foreach (var item in _availableItems.Where(i => i.ID == "test_item"))
                            {
                                item.IdleMod = v => (v + 2, true);
                            }
                        }
                    },
                    () => new BuffEffect()
                    {
                        ID = "test_buff2",
                        DisplayName = "Test Buff 2",
                        Desc = "D",
                        OnGetAction = () =>
                        {
                            ItemInfo item = new()
                            {
                                ID = "upgrade_item",
                                DisplayName = "Upgrade Item",
                                Desc = "D",
                                Categories = ItemInfo.ECategory.Nonsustainable,
                                PriceFunction = i => 15 + 2*i.AmountOwned,
                                ClickMod = v => (v + 1, true)
                            };
                            _availableItems.Add(item);
                        }
                    }
                }

            },
            new MilestoneInfo()
            {
                ID = "eoe",
                DisplayName = "Testoeune",
                Desc = "D",
                Condition = () => Cash > 1000,
                Choices =
                {
                    () => new BuffEffect()
                    {
                        ID = "test_buff",
                        DisplayName = "Test Milestone",
                        Desc = "D",
                        OnGetAction = () =>
                        {
                            ItemInfo item = new()
                            {
                                ID = "upgrade_item",
                                DisplayName = "Upgrade Item",
                                Desc = "D",
                                Categories = ItemInfo.ECategory.Nonsustainable,
                                PriceFunction = i => 15 + 2*i.AmountOwned,
                                ClickMod = v => (v + 1, true)
                            };
                            _availableItems.Add(item);
                        }
                    },
                    () => new BuffEffect()
                    {
                        ID = "test_buff2",
                        DisplayName = "Test Buff 2",
                        Desc = "D",
                        OnGetAction = () =>
                        {
                            ItemInfo item = new()
                            {
                                ID = "upgrade_item",
                                DisplayName = "Upgrade Item",
                                Desc = "D",
                                Categories = ItemInfo.ECategory.Nonsustainable,
                                PriceFunction = i => 15 + 2*i.AmountOwned,
                                ClickMod = v => (v + 1, true)
                            };
                            _availableItems.Add(item);
                        }
                    }
                }

            }
        };

        //AUTOBUFFS
        _autoBuffs = new()
        {
            new AutoBuff()
            {
                Condition = () => Sustain < 50,
                Effect = () => new BuffEffect()
                {
                    ID = "test_autobuff",
                    DisplayName = "Test Autobuff",
                    Desc = "D",
                    OnGetAction = () =>
                    {
                        AddClickMod(v => (v / 2, false));
                    }
                }
                
            },
            new AutoBuff()
            {
                Condition = () => Cash > 1000,
                Effect = () => new BuffEffect()
                {
                    ID = "1k",
                    DisplayName = "1k",
                    Desc = "Desc",
                    OnGetAction = () =>
                    {
                        ItemInfo item = new()
                            {
                                ID = "solar",
                                DisplayName = "Solar Panels",
                                Desc = "Desc",
                                Categories = ItemInfo.ECategory.Sustainable,
                                PriceFunction = i => 500 + 50*i.AmountOwned,
                                IdleMod = v => (v + 10, true)
                            };
                        _availableItems.Add(item);

                        item = new()
                            {
                                ID = "petro",
                                DisplayName = "Petroleum",
                                Desc = "Desc",
                                Categories = ItemInfo.ECategory.Nonsustainable,
                                PriceFunction = i => 300 + 70*i.AmountOwned,
                                ClickMod = v => (v + 5, true),
                                SustianMod = v => (v + .01f, true)
                            };
                        _availableItems.Add(item);
                    }
                }
            },
            new AutoBuff()
            {
                Condition = () => Cash > 1000000,
                Effect = () => new BuffEffect()
                {
                    ID = "1m",
                    DisplayName = "1m",
                    Desc = "Desc",
                    OnGetAction = () =>
                    {
                        ItemInfo item = new()
                            {
                                ID = "import",
                                DisplayName = "Import Goods",
                                Desc = "Desc",
                                Categories = ItemInfo.ECategory.Sustainable,
                                PriceFunction = i => 100000 + 9000*i.AmountOwned,
                                IdleMod = v => (v + 1000, true)
                            };
                        _availableItems.Add(item);

                        item = new()
                            {
                                ID = "rig",
                                DisplayName = "Oil Rig",
                                Desc = "Desc",
                                Categories = ItemInfo.ECategory.Nonsustainable,
                                PriceFunction = i => 60000 + 1000*i.AmountOwned,
                                ClickMod = v => (v + 5000, true),
                                SustianMod = v => (v + .2f, true)
                            };
                        _availableItems.Add(item);
                    }
                }
            }
        };
    }

}
