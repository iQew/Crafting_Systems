using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.Controls;

public sealed class PlayerStats {

    private static PlayerStats instance = null;
    private static readonly object padlock = new object();

    public static PlayerStats Instance { 
        get { 
            lock(padlock) {
                if (instance == null) {
                    instance = new PlayerStats();
                }
                return instance;
            }
        }
    }

    public enum ExperienceType { GENERAL, GATHERING_FORAGING }

    private const int MIN_LEVEL = 1;
    private const int MAX_LEVEL = 100;
    private Dictionary<int, int> _requiredXPPerLevelTable;
    private int _experienceGeneral = 100;
    private int _totalExperienceGatheringForaging = 100;

    public PlayerStats() {
        InitializeXPTable();
    }

    private void InitializeXPTable() {
        _requiredXPPerLevelTable = new Dictionary<int, int>();
        int requiredXP = 0;
        for (int i = 0; i <= MAX_LEVEL; i++) {
            requiredXP += (i + 1) * 100;
            _requiredXPPerLevelTable.Add(i, requiredXP);
        }
    }
    
    public int GetRequiredXPForLevelUp(int level) {
        if(level >= 0 && level <= MAX_LEVEL) {
            int requiredXP;
            if(_requiredXPPerLevelTable.TryGetValue(level, out requiredXP)) {
                return requiredXP;
            } 
        }        
        return int.MinValue;
    }

    public double GetLevelByXPAtCurrentLevel(int experience) {
        // sum function: n(n+1) / 2
        // we are using 100 * sum function with i running from 1 to x
        // therefor we have 100 * (x(x+1)) / 2 = x(x+1) * 50
        // which can be written as (x^2 + x) * 50
        // so if we are trying to solve by n for 1000 experience
        // we have the formula: (x^2 + x) * 50 = 1000
        // which can be written as: (x^2 + x) = 1000 / 50
        // which can be written as: x^2 + x - 1000 / 50 = 0
        // therefor our formula can be written as: x^2 + x - experience/50 = 0

        // a quadratic equation can be written as: a*x^2 + b*x + c = 0
        // so in our case we have: a = 1, b = 1, c = experience/50
        // -> x = (-1 +- sqrt(1^2 - 4 * (-experience/50))) / 2        
        // we are never interested in the negative solution even when two solutions exist
        // therefor we finally arrive at: x = (-1 + sqrt(1 + experience * 0.08)) / 2
         
        // example: experience: 1000 for level 4
        // x = (-1 + sqrt(1  + 1000 * 0.08)) / 2
        // x = (-1 + sqrt(1 + 80)) / 2
        // x = (-1 + sqrt(81)) / 2
        // x = (-1 + 9)) / 2
        // x = 8 / 2
        // x = 4

        // example: experience: 1100 for level 4
        // x = (-1 + sqrt(1 + 1100 * 0.08)) / 2
        // x = (-1 + sqrt(1 + 88)) / 2
        // x = (-1 + sqrt(89)) / 2
        // x = (-1 + 9.43398113206)) / 2
        // x = 8.43398113206 / 2
        // x = 4.21699056603

        return ((Math.Sqrt(1 + experience * 0.08d) - 1) / 2d);
    }   
    
    /**
     * Adding all the XP required per level based on the sum function for required XP per level.
     * **/
    public int GetLevelByTotalXP(int totalXP) {
        int level = 0;
        int accumulatedXP = 0;
        while(accumulatedXP <= totalXP) {
            level++;
            accumulatedXP = (int)((50 * level * (level + 1) * (level + 2)) / 3f);
            if(accumulatedXP == totalXP) {
                return level;
            }
        }
        level--;
        return level;
    }
    
    public int GetTotalXPAtLevel(int level) {
        return (int)((50 * level * (level + 1) * (level + 2)) / 3f);
    }

    public Vector2 AddExperienceGatheringForaging(ItemDataSO itemData) {
        int previousXP = _totalExperienceGatheringForaging;
        int amount = 0;
        switch (itemData.Quality) {
            case ItemDataSO.QualityType.COMMON:
                amount = 99;
                break;
            case ItemDataSO.QualityType.UNCOMMON:
                amount = 200;
                break;
            case ItemDataSO.QualityType.RARE:
                amount = 300;
                break;
            case ItemDataSO.QualityType.EPIC:
                amount = 400;
                break;
            case ItemDataSO.QualityType.LEGENDARY:
                amount = 500;
                break;
            default:
                break;
        }
        
        _totalExperienceGatheringForaging += amount;
        _experienceGeneral += 100;
        return new Vector2(previousXP, amount);
    }
}
