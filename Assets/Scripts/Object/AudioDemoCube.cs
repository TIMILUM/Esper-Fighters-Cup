namespace EsperFightersCup.Object
{
    public class AudioDemoCube : AStaticObject
    {
        protected override void Start()
        {
            base.Start();

            if (AudioEmitter is null)
            {
                return;
            }
            AudioEmitter.Event = "event:/Jukebox";
            AudioEmitter.Play();
        }

        protected override void OnHit(ObjectBase from, ObjectBase to, BuffObject.BuffStruct[] appendBuff)
        {
            base.OnHit(from, to, appendBuff);
            AudioEmitter?.SetParameter("Punched", 1f);
        }
    }
}
