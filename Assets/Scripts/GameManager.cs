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

    public event Action<MilestoneInfo> MilestoneConditionMet;

    public static GameManager GAME;

    public readonly static float STARTING_CPS = 0; //cash per second?
    public readonly static float STARTING_CPC = 1; //cash per click 
    public readonly static float STARTING_SPM = 0; // sustain per mintue

    public readonly static float STARTING_CASH = 0;
    public readonly static float STARTING_SUSTAIN = 0;

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
    private List<BuffEffect> _autoBuffs;

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
        await Task.Delay(1000);
        OneSecUpdate();
    }

    void OnClick()
    {
        Cash += CPC;
    }

    public void AddBuffEffect(BuffEffect buff)
    {
        buff.OnGetAction();
        _activeBuffs.Add(buff);
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

    private void Setup()
    {
        //ITEMS
        _availableItems = new()
        {
            new ItemInfo()
            {
                ID = "test_item",
                DisplayName = "Test Item",
                Desc = "D",
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

            }
        };
    }

}
