using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// Little more than a singleton marker class.
[RequireComponent(typeof(Health))]
public class Player : Singleton<Player> {}
