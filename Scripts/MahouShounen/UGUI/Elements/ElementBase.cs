using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElementBase : MSUIComponentBase
{
    public MSUIComponentBase parentComponent;

    public virtual void SetParentComponent(MSUIComponentBase component)
    {
        parentComponent = component;
        base.SetManager(component.GetManager());
    }
}
