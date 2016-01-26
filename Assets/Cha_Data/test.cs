using UnityEngine;
using System.Collections;

public class test : MonoBehaviour {

    Animation animation;
	// Use this for initialization
	void Start () {
        animation = gameObject.GetComponent<Animation>();
        animation.Play("Dead");
        animation.Play("Damage");
        animation.Play("Walk");
        animation.Play("Wait");
        animation.Play("Attack");
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
