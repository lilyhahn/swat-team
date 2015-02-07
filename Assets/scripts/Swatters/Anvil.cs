using UnityEngine;
using System.Collections;

public class Anvil : Hand {
	public float chargeRate;
	public float charge = 0;
	Animator charge_circle;
	
	protected override void Start(){
		base.Start();
		charge_circle = transform.Find("fire_circle").GetComponent<Animator>();
		chargeRate = charge_circle.speed;
		charge_circle.speed = 0;
	}
	
	protected override void Update(){
		base.Update();
		if(Input.GetButton("Fire1") && !anim.GetCurrentAnimatorStateInfo(0).IsName("swat")){
			charge_circle.speed = chargeRate;
		}
		else{
			charge_circle.speed = 0;
		}
		if(charge_circle.GetCurrentAnimatorStateInfo(0).normalizedTime > 0 && !anim.GetCurrentAnimatorStateInfo(0).IsName("swat")){
			charge_circle.GetComponent<SpriteRenderer>().enabled = true;
		}
		else{
			charge_circle.GetComponent<SpriteRenderer>().enabled = false;
		}
		//GetComponent<SpriteRenderer>().color = Color.Lerp(GetComponent<SpriteRenderer>().color, new Color(255, 255, 255, 1), Time.deltaTime * charge / 100);
		if(Input.GetButton("Fire1")){
			Swat ();
		}
		if(anim.GetCurrentAnimatorStateInfo(0).IsName("swat") && anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 2){
			GetComponent<SpriteRenderer>().color = new Color(255, 255, 255, 0.558f);
		}
	}
	protected override void Swat(){
		if(charge_circle.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1){
			GetComponent<SpriteRenderer>().color = Color.white;
			charge_circle.Play("fire_circle", 0, 0);
			anim.SetTrigger("swat");
		}
	}
}
