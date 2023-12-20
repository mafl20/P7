using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering;

public class CutOutMask : Image
{
    //public override Material materialForRendering => base.materialForRendering;
    
    public override Material materialForRendering{

    get{
            Material mat = new Material(base.materialForRendering);
            mat.SetInt("_StencilComp", (int)UnityEngine.Rendering.CompareFunction.NotEqual);
            return mat;
        }
    }
}
