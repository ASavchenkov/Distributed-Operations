/*

If a bullet hits a collider without one of these, it is destroyed.
That is to say: anything that will definitely just eat a bullet
doesn't need ballistic properties. Eg:

the ground
huge rocks.
Really thick walls. (These are more complicated when it comes to tank shells.)

also emits a signal for when the impact changes game state, eg:
player hp
spray location.
*/
public interface IBallisticTarget
{
    void OnContact(ProjectileFPV projectile);
}
