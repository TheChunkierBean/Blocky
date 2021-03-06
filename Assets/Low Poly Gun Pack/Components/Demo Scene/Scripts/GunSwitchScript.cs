﻿using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using BeardedManStudios.Network;

public class GunSwitchScript : NetworkedMonoBehavior {

	[Header("Guns")]
	//Current gun number
	public int currentGun = 0;
	//Get the current gun object
	public Transform currentGunObject;

	//Array of guns
	public Transform[] guns;

	[Header("Gun Text")]
	//Gun text
	public string gun1Text;
	public string gun2Text;
	public string gun3Text;
	public string gun4Text;
	public string gun5Text;

	[Header("UI Components")]
	//UI Text components
	public Text totalAmmoText;
	public Text ammoLeftText;
	public Text tutorialText;
	public Text currentGunText;

	[Header("Customizable Options")]
	//How long the tutorial text will be visible
	public float tutorialTextTimer = 10.0f;
	//How slow the tutorial text will fade out
	public float tutorialTextFadeOutTime = 4.0f;

	void Start () {
        //Start with the first gun selected

		//Set the current gun text
		//currentGunText.text = gun1Text;

		//Get the ammo values from the first guns script and show as text

		//Start the tutorial text timer
		StartCoroutine (TutorialTextTimer ());
	}
    private void Awake()
    {
        //AddNetworkVariable(() => move, x => move = (Vector3)x);
    }
    protected override void NetworkStart()
    {
        base.NetworkStart();
        if (IsOwner)
        {
            totalAmmoText = GameObject.FindObjectsOfType<Text>()[0];
            ammoLeftText = GameObject.FindObjectsOfType<Text>()[1];
            currentGunObject = guns[0];
            currentGunObject.GetComponent<GunScript>().PickUp();
            changeGun(0);
            totalAmmoText.text = guns[0].GetComponent
    <GunScript>().magazineSize.ToString();
            ammoLeftText.text = guns[0].GetComponent
                <GunScript>().bulletsLeft.ToString();
        }
    }
    void Update () {
        //Get the ammo left from the current gun
        //and show it as a text
        if (IsOwner)
        {
            if (GetComponentInChildren<GunScript>() != null)
            {
                ammoLeftText.text = GetComponentInChildren<GunScript>().bulletsLeft.ToString();
                totalAmmoText.text = GetComponentInChildren<GunScript>().totalAmmo.ToString();
            }
            else
            {
                ammoLeftText.text = "0";
                totalAmmoText.text = "0";
            }
        }
		//If key 1 is pressed, and noSwitch is false in GunScript.cs
		if(Input.GetKeyDown(KeyCode.Alpha1) && 
		   currentGunObject.GetComponent<GunScript>().noSwitch == false) {

			changeGun(0);
			totalAmmoText.text = guns[0].GetComponent
				<GunScript>().magazineSize.ToString();
			//Set the currentGunObject to the current gun
			currentGunObject = guns[0];
			//Set the current gun text
			currentGunText.text = gun1Text;
		}
        if (Input.GetKeyDown(KeyCode.F))
        {

            DropGun();
        }
		//If key 2 is pressed, and noSwitch is false in GunScript.cs
		if(Input.GetKeyDown(KeyCode.Alpha2) && 
		   currentGunObject.GetComponent<GunScript>().noSwitch == false) {

			changeGun(1);
			totalAmmoText.text = guns[1].GetComponent
				<GunScript>().magazineSize.ToString();
			//Set the currentGunObject to the current gun
			currentGunObject = guns[1];
			//Set the current gun text
			currentGunText.text = gun2Text;
		}	

		//If key 3 is pressed, and noSwitch is false in GunScript.cs
		if(Input.GetKeyDown(KeyCode.Alpha3) && 
		   currentGunObject.GetComponent<GunScript>().noSwitch == false) {

			changeGun(2);
			totalAmmoText.text = guns[2].GetComponent
				<GunScript>().magazineSize.ToString();
			//Set the currentGunObject to the current gun
			currentGunObject = guns[2];
			//Set the current gun text
			currentGunText.text = gun3Text;
		}	

		//If key 4 is pressed, and noSwitch is false in GunScript.cs
		if(Input.GetKeyDown(KeyCode.Alpha4) && 
		   currentGunObject.GetComponent<GunScript>().noSwitch == false) {

			changeGun(3);
			totalAmmoText.text = guns[3].GetComponent
				<GunScript>().magazineSize.ToString();
			//Set the currentGunObject to the current gun
			currentGunObject = guns[3];
			//Set the current gun text
			currentGunText.text = gun4Text;
		}

		//If key 5 is pressed, and noSwitch is false in GunScript.cs
		if(Input.GetKeyDown(KeyCode.Alpha5) && 
		   currentGunObject.GetComponent<GunScript>().noSwitch == false) {
			
			changeGun(4);
			totalAmmoText.text = guns[4].GetComponent
				<GunScript>().magazineSize.ToString();
			//Set the currentGunObject to the current gun
			currentGunObject = guns[4];
			//Set the current gun text
			currentGunText.text = gun5Text;
		}
	}

	//Activates the current gun from the array
	void changeGun(int num) {
		currentGun = num;
		for(int i = 0; i < guns.Length; i++) {
			if(i == num)
				guns[i].gameObject.SetActive(true);
			else
				guns[i].gameObject.SetActive(false);
		}
	}
    public void DropGun()
    {
        if (guns[0] != null)
            guns[0].gameObject.GetComponent<GunScript>().DropGun(transform.parent.position);
        guns[0] = null;
    }
    public void PickUpGun(Transform gun, Transform mainCamera)
    {
        DropGun();
        if (gun.name == "Holder")
        {
            gun.localPosition = new Vector3(0f, 0f, 0f);
            gun.localRotation = new Quaternion(0f, 0f, 0f, 0f);
            gun.parent.parent = mainCamera.transform;
            guns[0] = gun.parent;
        }
        else
        {
            guns[0] = gun;
            gun.parent = mainCamera.transform;
        }
        guns[0].localScale = new Vector3(0.05f, 0.05f, 0.05f);
        guns[0].localRotation = new Quaternion(0f, 0f, 0f, 0f);
        guns[0].localPosition = new Vector3(0.36f, -0.19f, 0.48f);
        guns[0].gameObject.GetComponent<GunScript>().PickUp();
    }
	//Timer for the tutorial text fade 
	IEnumerator TutorialTextTimer () {
		//Wait the set amount of time
		yield return new WaitForSeconds(tutorialTextTimer);
		//Start fading out the tutorial text
		//tutorialText.CrossFadeAlpha (0.0f, tutorialTextFadeOutTime, false);
	}
}