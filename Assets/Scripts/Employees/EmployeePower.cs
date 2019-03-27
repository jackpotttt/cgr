using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.UI;

public class EmployeePower
{
    internal int usesThisRound;
    internal EmployeePowerDefinition type;
    internal Text buttonText;
    internal Action OnCancel;

    internal int range1;
    internal int range2;

    public EmployeePower(EmployeePowerDefinition def)
    {
        type = def;
        usesThisRound = 0;
        range1 = def.range1;
        range2 = def.range2;
    }

    public bool CanUse()
    {
        return usesThisRound < type.usesPerRound;
    }

    public void ResetRound()
    {
        usesThisRound = 0;        
    }

    public void SetButtonText()
    {
        buttonText.text = $"{type.summary}\n({usesThisRound}/{type.usesPerRound})";
    }

    internal void Use(Employee employee)
    {
        EmployeePowerDefinition.powerLookup[type.callbackIndex](employee, this);
    }

    internal void Cancel()
    {
        OnCancel?.Invoke();
    }
}

