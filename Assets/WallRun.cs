using UnityEngine;
using UnityStandardAssets.Characters.FirstPerson;
using System.Collections;

public class WallRun : MonoBehaviour {

    private bool _wallLeft;
    private bool _wallRight;

    private bool isWallRunning;

    private RaycastHit _hitRight;
    private RaycastHit _hitLeft;

    private int _jumpCount = 0;

    private Rigidbody _rb;
    private RigidbodyFirstPersonController _firstPersonRigidbody;

    public float wallRunTime = 0.5f;


	void Start () {
        _rb = GetComponent<Rigidbody>();
        _firstPersonRigidbody = GetComponent<RigidbodyFirstPersonController>();

	}

    void Update()
    {
      
        if(_firstPersonRigidbody.isGrounded)
        {
            _jumpCount = 0;
        }

        if(Input.GetKeyDown(KeyCode.E) && !_firstPersonRigidbody.isGrounded && _jumpCount <= 1)
        {
            if(Physics.Raycast(transform.position,transform.right,out _hitRight,1))
            {
                if (_hitRight.transform.tag == "wall")
                {
                    isWallRunning = true;
                    _wallRight = true;
                    _wallLeft = false;
                    _jumpCount += 1;
                    _rb.useGravity = false;
                   
                    StartCoroutine(endRun());
                }
                
            }

            if (Physics.Raycast(transform.position, -transform.right, out _hitLeft, 1))
            {
                if (_hitLeft.transform.tag == "wall")
                {
                    isWallRunning = true;
                    _wallRight = false;
                    _wallLeft = true;
                    _jumpCount += 1;
                    _rb.useGravity = false;
                    StartCoroutine(endRun());
                }
            }

            if(isWallRunning)
            {
                if(Input.GetKey(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
                {
                    _rb.AddForce(transform.forward * 6);
                }
            }
        }

        if(!_firstPersonRigidbody.isGrounded && Input.GetKeyDown(KeyCode.Space))
        {
            if(_wallLeft == true)
            {
                _rb.AddForce(_hitLeft.normal * 100,ForceMode.Impulse);
                _jumpCount = 0;
            }
            if (_wallRight == true)
            {
                _rb.AddForce(_hitRight.normal * 100, ForceMode.Impulse);
                _jumpCount = 0;
            }
        }
    }

    IEnumerator endRun()
    {
        yield return new WaitForSeconds(wallRunTime);
        isWallRunning = false;
        _wallRight = false;
        _wallLeft = false;
        _rb.useGravity = true;
    }
}
