using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunBase : MonoBehaviour
{
    public GunBulletBase defaultBullet;
    public GunBulletBase enhanceBullet;
    public HandPosition handPosition;
    public LaserPoint point;
    public GunStatus status;

    private AudioSource gunAudioSource;
    private ParticleSystem gunParticle;
    private ARAVRInput.Controller controller;
    private bool isEnhance = false;
    private bool canShot = true;



    private const float VIBRATION_TIME = 0.1f;
    private const float VIBRATION_FREQUENCY = 5F;
    private const float VIBRATION_AMPLITUDE = 5F;

    private void Awake()
    {
        Init();
    }

    private void Update()
    {

        //플레이 모드가 아니면 비활성화
        if (!GameManager.Instance.CheckPlayingGame())
        {
            return;
        }


        if ((ARAVRInput.GetDown(ARAVRInput.Button.IndexTrigger, controller) || ARAVRInput.Get(ARAVRInput.Button.IndexTrigger, controller)) && canShot)
        {
            Shot();
        }
    }

    private void Init()
    {
        gunAudioSource = GetComponent<AudioSource>();
        gunParticle = GetComponent<ParticleSystem>();

        if (handPosition == HandPosition.RIGHT)
        {
            controller = ARAVRInput.Controller.RTouch;
        }
        else
        {
            controller = ARAVRInput.Controller.LTouch;
        }
    }

    private void Shot()
    {
        GunBulletBase currBullet;
        Vector3 direction;
        if (handPosition == HandPosition.RIGHT)
        {
            direction = ARAVRInput.RHandDirection;
        }
        else
        {
            direction = ARAVRInput.LHandDirection;
        }

        if (!isEnhance)
        {
            currBullet = Instantiate(defaultBullet, point.startPos.position, Quaternion.identity);
        }
        else
        {
            currBullet = Instantiate(enhanceBullet, point.startPos.position, Quaternion.identity);
        }

        currBullet.transform.up = direction;
        currBullet.Move(direction);
        AttackReaction();

        StartCoroutine(GunDelayRoutine(currBullet.GetRate()));
    }

    private void AttackReaction()
    {
        ARAVRInput.PlayVibration(VIBRATION_TIME, VIBRATION_FREQUENCY, VIBRATION_AMPLITUDE, controller);
        gunAudioSource.Play();
        gunParticle.Play();
    }

    private IEnumerator GunDelayRoutine(float time)
    {
        canShot = false;
        yield return new WaitForSeconds(time);
        canShot = true;
    }

    public void ChangeWeaponMode(bool enable)
    {
        isEnhance = enable;
    }

}
