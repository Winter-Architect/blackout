public interface ITransition
{ 
    IState To { get; }
    IPredicate Predicate { get; }
    
}

public class humain : ITransition, IState
{
    public IState To { get; }
    public IPredicate Predicate { get; }
    public void OnEnter()
    {
        throw new System.NotImplementedException();
    }

    public void Update()
    {
        throw new System.NotImplementedException();
    }

    public void FixedUpdate()
    {
        throw new System.NotImplementedException();
    }

    public void OnExit()
    {
        throw new System.NotImplementedException();
    }
}