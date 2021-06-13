using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;

public class Pole : MonoBehaviour
{
    public static Pole current;
    public PositionConstraint constraint;
    private void Awake() 
    {
        current = this;
        constraint = GetComponent<PositionConstraint>();
    }

    public void AddSource(Transform source)
    {
        ConstraintSource con = new ConstraintSource();
        con.sourceTransform = source;
        con.weight = 1;
        constraint.AddSource(con);
    }
}
