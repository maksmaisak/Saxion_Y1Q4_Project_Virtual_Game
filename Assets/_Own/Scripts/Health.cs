using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Health : MonoBehaviour {

    public delegate void OnHealthChangedHandler(Health sender, int oldValue, int newValue);
    public event OnHealthChangedHandler OnHealthChanged;

    public delegate void OnDeathHandler(Health sender);
    public event OnDeathHandler OnDeath;

    [SerializeField] [Range(0, 100)] int _health = 1;
    [SerializeField] [Range(0, 100)] int _maxHealth = 100;
    [SerializeField] bool destroyOnDeath = true;
    [SerializeField] bool _canBeReduced  = true;

    [SerializeField] UnityEvent OnDeathUnityEvent = new UnityEvent();

    [System.Serializable]
    public class OnHealthChangedEvent : UnityEvent<int, int> {}
    [SerializeField] OnHealthChangedEvent OnHealthChangedUnityEvent = new OnHealthChangedEvent();

    public int health {
        get {return _health;}
        set {SetHealth(value);}
    }

    public int maxHealth {
        get { return _maxHealth;}
        set {
            _maxHealth = value;
            ValidateMaxHealth();
        }
    }

    public bool canBeReduced { get { return _canBeReduced; }}

    public bool isAlive { get { return health > 0; }}
    public bool isDead  { get { return health <= 0;}}

    void OnValidate() {

        ValidateMaxHealth();
    }

    public Health SetHealth(int newHealth) {

        if (!_canBeReduced && newHealth < _health) return this;
        newHealth = Mathf.Clamp(newHealth, 0, _maxHealth);
        if (newHealth == _health) return this;

        int oldValue = _health;
        _health = newHealth;

        if (OnHealthChanged != null) {
            OnHealthChanged.Invoke(this, oldValue, newHealth);
        }
        OnHealthChangedUnityEvent.Invoke(oldValue, newHealth);

        if (_health <= 0) {

            if (OnDeath != null) {
                OnDeath.Invoke(this);
            }
            OnDeathUnityEvent.Invoke();

            if (destroyOnDeath) Destroy(gameObject);
        }

        return this;
    }

    public Health DealDamage(int damage) {

        SetHealth(health - damage);
        return this;
    }

    public Health Increase(int increment) {

        SetHealth(health + increment);
        return this;
    }

    public Health SetDestroyOnDeath(bool newDestroyOnDeath) {

        destroyOnDeath = newDestroyOnDeath;
        return this;
    }

    public Health SetCanBeReduced(bool newCanBeReduced) {

        _canBeReduced = newCanBeReduced;
        return this;
    }

    private void ValidateMaxHealth() {

        if (_health > _maxHealth) {

            SetHealth(_maxHealth);
        }
    }
}
