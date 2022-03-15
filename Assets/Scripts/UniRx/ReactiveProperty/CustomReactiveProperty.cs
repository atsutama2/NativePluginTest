using System;
using UnityEngine;

namespace UniRx.ReactiveProperty
{
    [UnityEditor.CustomPropertyDrawer(typeof(CustomReactiveProperty))]
    public class ExtendInspectorDisplayDrawer : InspectorDisplayDrawer
    {
        
    }
    
    public enum Fruit
    {
        Apple,
        Banana,
        Peach,
        Melon,
        Orange
    }
    
    [Serializable]
    public class CustomReactiveProperty : ReactiveProperty<Fruit>
    {
        public CustomReactiveProperty()
        {
            
        }

        public CustomReactiveProperty(Fruit init) : base(init)
        {
            
        }
    }
}
