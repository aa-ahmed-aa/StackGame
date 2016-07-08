using UnityEngine;
using System.Collections;

public class RemoveRubbles : MonoBehaviour {

	private void OnCollisionEnter(Collision col)
    {
        Destroy(col.gameObject);
    }
}
