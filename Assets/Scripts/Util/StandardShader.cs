using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StandardShader : MonoBehaviour
{
    public Material StandardMaterialOBJImporter;
    public static Material st_StandardMaterialOBJImporter;

    private void Awake()
    {
        st_StandardMaterialOBJImporter = StandardMaterialOBJImporter;
    }
}
