using UnityEngine;
using System.Collections;

public class EveAnimation : MonoBehaviour
{
    public bool Enabled = true;

    private Transform head;
    private Material mat;

    public void Initialize()
    {
        head = transform.Find("Head").Find("Animated");
        mat = head.Find("model_face").GetComponent<Renderer>().material;
    }

    public void ReSet()
    {
        SetEyesOffset(3);
    }

    public void LookAt(Vector3 pos)
    {
        float t = Time.deltaTime * 5f;
        head.rotation = Quaternion.Slerp(head.rotation, 
            Quaternion.LookRotation((pos - head.position), Vector3.up), t);
            
        Vector3 e = head.localEulerAngles;
        e.x = Mathf.Clamp(Util.ClampAngle(e.x), -30f, 30f);
        e.y = Mathf.Clamp(Util.ClampAngle(e.y), -60f, 60f);
        e.z = Mathf.Clamp(Util.ClampAngle(e.z), -15f, 15f);
        head.localRotation = Quaternion.Euler(e);

        t = e.x / 30f;
        Vector3 p = head.localPosition;
        p.y = -0.1f + Mathf.Abs(t) * 0.3f;
        p.z = t * 0.2f;
        head.localPosition = p;
    }

    public void Blink()
    {
        StartCoroutine(BlinkCR());
    }

    public void Wink()
    {
        StartCoroutine(WinkCR());
    }

    private IEnumerator BlinkCR()
    {
        SetEyesOffset(2);
        yield return new WaitForSeconds(Random.value * 0.25f);
        SetEyesOffset(3);
    }

    private IEnumerator WinkCR()
    {
        SetEyesOffset(0);
        yield return new WaitForSeconds(0.5f);
        SetEyesOffset(3);
    }

    private void SetEyesOffset(int i)
    {
        mat.SetTextureOffset("_MainTex", new Vector2(0, i * 0.2f));
    }
}