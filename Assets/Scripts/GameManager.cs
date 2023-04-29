using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager GAME;

    public readonly static float STARTING_CPS = 0;
    public readonly static float STARTING_CPC = 1;
    public readonly static float STARTING_SPM = 0;

    public readonly static float STARTING_CASH = 0;
    public readonly static float STARTING_SUSTAIN = 0;

    public float Cash { get; set; }
    public float Sustian { get; set; }

    private List<Func<float, float>> _clickModifiers;
    private List<Func<float, float>> _idleModifiers;
    private List<Func<float, float>> _sustainModifiers;


    public float CPS { get; private set; }
    public float CPC { get; private set; }
    public float SPM { get; private set; }



    private void Awake()
    {
        CPS = STARTING_CPS;
        CPC = STARTING_CPC;
        SPM = STARTING_SPM;
        Cash = STARTING_CASH;
        Sustian = STARTING_SUSTAIN;
        _clickModifiers = new();
        _idleModifiers = new();
    }
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    void Click()
    {
        Cash += CPC;
    }

    public void AddClickMod(Func<float, float> mod, bool front = false)
    {
        if (front) _clickModifiers.Insert(0, mod);
        else _clickModifiers.Add(mod);
        float s = STARTING_CPC;
        foreach (var func in _clickModifiers)
        {
            s = func(s);
        }
        CPC = s;
    }
    public void AddIdleMod(Func<float, float> mod, bool front = false)
    {
        if (front) _idleModifiers.Insert(0, mod);
        else _idleModifiers.Add(mod);
        float s = STARTING_CPS;
        foreach (var func in _idleModifiers)
        {
            s = func(s);
        }
        CPS = s;
    }
}
