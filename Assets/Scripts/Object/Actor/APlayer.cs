using UnityEngine;

public class APlayer : ACharacter
{
    [SerializeField, Tooltip("직접 컴포넌트를 넣어주세요.")]
    private Rigidbody _rigidbody = null;

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        _rigidbody.isKinematic = !photonView.IsMine;
    }

    // Update is called once per frame
    private void Update()
    {
    }
}
