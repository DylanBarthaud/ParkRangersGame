using BehaviourTrees;
using UnityEngine;

public class Badger_Brain_Default : Ai_Brain
{
    private void Awake()
    {
        base.BaseAwake();

        #region Neutral Tree
        neutral_Root = new Root("Neutral_Root");

        #endregion

        #region Searching Tree
        searching_Root = new Root("Searching_Root"); 

        #endregion

        #region Hunt Tree
        hunt_Root = new Root("Hunt_Root");

        #endregion

        #region ChasePlayer Tree
        chasePlayer_Root = new Root("ChasePlayer_Root");

        #endregion
    }

    protected override void ChasePlayer()
    {
        chasePlayer_Root.Process(); 
    }

    protected override void Hunt()
    {
        hunt_Root.Process();
    }
    protected override void Neutral()
    {
        neutral_Root.Process();
    }

    protected override void Searching()
    {
        searching_Root.Process();
    }
}
