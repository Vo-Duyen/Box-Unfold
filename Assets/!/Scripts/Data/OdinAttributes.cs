using System;
using Sirenix.Serialization;
using UnityEngine;

namespace LongNC.Data
{
#if UNTY_EDITOR
    public class LongSerializeAttribute : OdinSerializeAttribute
#else
    public class LongSerializeAttribute : Attribute
#endif
    {
        
    }
}