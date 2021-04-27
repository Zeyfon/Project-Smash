using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using PSmash.SceneManagement;

public class DeleteSave
{

    [MenuItem("MyTools/Delete Save File")]
    public static void DeleteSaveFile()
    {
        //GetWindow<DeleteSave>("Example");
        SavingWrapper[] savingWrappers = Resources.FindObjectsOfTypeAll<SavingWrapper>();
        savingWrappers[0].Delete();
    }

}
