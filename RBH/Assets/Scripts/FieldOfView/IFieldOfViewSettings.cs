public interface IFieldOfViewSettings
{
    public float FrontViewAngle { get;}

    public float FrontViewDistance { get;}

    public int AdditionalFrontRayCount { get;}

    public float AroundViewDistance { get;}

    public int AdditionalAroundRayCount { get;}

    public void Update()
    {
        // NO OP
    }
}
