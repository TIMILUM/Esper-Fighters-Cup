namespace EsperFightersCup.Object
{
    public class AudioDemoCube : AStaticObject
    {
        protected override void Start()
        {
            base.Start();
            AudioEmitter.Event = "event:/Jukebox";
            AudioEmitter.Play();
        }

        protected override void OnHit(ObjectBase from, ObjectBase to, BuffObject.BuffStruct[] appendBuff)
        {
            base.OnHit(from, to, appendBuff);
            AudioEmitter.SetParameter("Punched", 1f);
        }
    }
}
