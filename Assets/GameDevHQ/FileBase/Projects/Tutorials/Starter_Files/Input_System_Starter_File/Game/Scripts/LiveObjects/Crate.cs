using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking.Types;
using UnityEngine.UIElements;

namespace Game.Scripts.LiveObjects
{
    public class Crate : MonoBehaviour
    {
        [SerializeField] private float _punchDelay;
        [SerializeField] private GameObject _wholeCrate, _brokenCrate;
        [SerializeField] private Rigidbody[] _pieces;
        [SerializeField] private BoxCollider _crateCollider;
        [SerializeField] private InteractableZone _interactableZone;
        private bool _isReadyToBreak = false;
        private PlayerInputActions _input;
        private float _attackTime;
        private bool _powerAttack;
        private bool _powerCharging;
        private int _zone;

        private List<Rigidbody> _brakeOff = new List<Rigidbody>();

        private void OnEnable()
        {
            InteractableZone.onZoneInteractionComplete += InteractableZone_onZoneInteractionComplete;

            _input = new PlayerInputActions();
            _input.Crate.Enable();
            _input.Crate.WholeDestroy.started += WholeDestroy_started;
            _input.Crate.WholeDestroy.canceled += WholeDestroy_canceled;
            _attackTime = 0;
        }

        private void WholeDestroy_started(UnityEngine.InputSystem.InputAction.CallbackContext obj)
        {
            if (_zone == 6)
            {
                Debug.Log("Called2");
                _powerCharging = true;
                StartCoroutine(DamageRoutine());
            }
        }

        private void WholeDestroy_canceled(UnityEngine.InputSystem.InputAction.CallbackContext obj)
        {
            _powerCharging = false;
            _attackTime = 0;
            if (_isReadyToBreak == false && _brakeOff.Count > 0 && _powerAttack == false)
            {
                _wholeCrate.SetActive(false);
                _brokenCrate.SetActive(true);
                _isReadyToBreak = true;
            }
            if (_isReadyToBreak && _powerAttack == true) //Crate zone            
            {
                _powerAttack = false;
                Debug.Log("Called");
                if (_brakeOff.Count > 0)
                {
                    Debug.Log("called Whole Hit");
                    //StopCoroutine(DamageRoutine());
                    BreakWhole();
                    StartCoroutine(PunchDelay());

                }
                else if (_brakeOff.Count == 0)
                {
                    _isReadyToBreak = false;
                    _crateCollider.enabled = false;
                    _interactableZone.CompleteTask(6);
                    Debug.Log("Completely Busted");
                }
            }
        }
        IEnumerator DamageRoutine()
        {
            while (_powerCharging == true)
            {
                _attackTime += (1f * Time.deltaTime);
                Debug.Log(_attackTime);

                yield return null;
                if (_attackTime > 3f)
                {
                    _powerAttack = true;
                }
            }
        }

        private void InteractableZone_onZoneInteractionComplete(InteractableZone zone)
        {
            _zone = zone.GetZoneID();
            if (_isReadyToBreak == false && _brakeOff.Count >0 && _powerAttack == false)
            {
                _wholeCrate.SetActive(false);
                _brokenCrate.SetActive(true);
                _isReadyToBreak = true;
            }

            if (_isReadyToBreak && zone.GetZoneID() == 6) //Crate zone            
            {
                
                if (_brakeOff.Count > 0)
                {
                    Debug.Log("Called Single Hit");
                    BreakPart();
                    StartCoroutine(PunchDelay());
                }
                else if(_brakeOff.Count == 0)
                {
                    _isReadyToBreak = false;
                    _crateCollider.enabled = false;
                    _interactableZone.CompleteTask(6);
                    Debug.Log("Completely Busted");
                }
            }
        }

        private void Start()
        {
            _brakeOff.AddRange(_pieces);
        }
        public void BreakWhole()
        {
            int rng = 1;
            Debug.Log("Called Break Whole");
            if (_brakeOff.Count > 0)
            {
                for (int i = _brakeOff.Count; i > -1; i--)
                {
                    _brakeOff[rng].constraints = RigidbodyConstraints.None;
                    _brakeOff[rng].AddForce(new Vector3(1f, 1f, 1f), ForceMode.Force);
                    _brakeOff.Remove(_brakeOff[rng]);
                    //Debug.Log(rng);  
                }
            }
        }
        public void BreakPart()
        {
            Debug.Log("Called BreakPart");
            int rng = Random.Range(0, _brakeOff.Count);
            _brakeOff[rng].constraints = RigidbodyConstraints.None;
            _brakeOff[rng].AddForce(new Vector3(1f, 1f, 1f), ForceMode.Force);
            _brakeOff.Remove(_brakeOff[rng]);            
        }

        IEnumerator PunchDelay()
        {
            float delayTimer = 0;
            while (delayTimer < _punchDelay)
            {
                yield return new WaitForEndOfFrame();
                delayTimer += Time.deltaTime;
            }

            _interactableZone.ResetAction(6);
        }

        private void OnDisable()
        {
            InteractableZone.onZoneInteractionComplete -= InteractableZone_onZoneInteractionComplete;
        }
    }
}
