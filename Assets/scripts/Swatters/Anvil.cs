using UnityEngine;
using System.Collections;

public class Anvil : Hand {
	public float chargeRate = 1;
	public float charge = 0;
	
	protected override void Update(){
		base.Update();
		if(charge < 100 && Input.GetButton("Fire1")){
			charge += chargeRate;
		}
		GetComponent<SpriteRenderer>().color = Color.Lerp(GetComponent<SpriteRenderer>().color, new Color(255, 255, 255, 1), Time.deltaTime * charge / 100);
		if(Input.GetButtonUp("Fire1")){
			Swat ();
		}
		if(anim.GetCurrentAnimatorStateInfo(0).IsName("swat") && anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 2){
			GetComponent<SpriteRenderer>().color = new Color(255, 255, 255, 0.558f);
		}
	}
	protected override void Swat(){
		if(charge >= 100){
			charge = 0;
			anim.SetTrigger("swat");
		}
	}
}
