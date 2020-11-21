using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class ShockWaveAnim : MonoBehaviour
{

    ///// <summary>
    ///// this is the material that will store the ShockWave Shader
    ///// </summary>
    public Material material;
    public Shader shader;
    private Material mat;

    /// <summary>
    /// The speed. if speed is zero animation will not run, and can be animated with animator
    /// </summary>
    //[Range(0.01f, 100f)]
    public float speed = 1f;

    /// <summary>
    /// The t, the time for this object
    /// </summary>
    public float t = 0f;

    /// <summary>
    /// The radius over time.
    /// </summary>
    public AnimationCurve radiusAnim;

    /// <summary>
    /// The wave size over time.
    /// </summary>
    public AnimationCurve wavesizeAnim;

    /// <summary>
    /// The amplitude over time.
    /// </summary>
    public AnimationCurve amplitudeAnim;

    /// <summary>
    /// color over time
    /// </summary>
    public Gradient colorAnim;

    /// <summary>
    /// Saturation over time
    /// </summary>
    public AnimationCurve SatAnim;

    /// <summary>
    /// destory object when done animating
    /// </summary>
    public bool destoryWhenDone = false;

    // Start is called before the first frame update
    void Start()
    {

        mat = new Material(shader);

        GetComponent<Renderer>().material = mat;
        GetComponent<Renderer>().material.CopyPropertiesFromMaterial(material);
        mat = GetComponent<Renderer>().material;


        //set the radius amplitude and wavesize to zero
        mat.SetFloat("_radius", radiusAnim.Evaluate(0.0f));
        mat.SetFloat("_amplitude", amplitudeAnim.Evaluate(0.0f));
        mat.SetFloat("_wavesize", wavesizeAnim.Evaluate(0.0f));
        mat.SetColor("_color", colorAnim.Evaluate(0.0f));
        mat.SetFloat("_saturation", SatAnim.Evaluate(0.0f));
        mat.SetInt("_active", 1);

        t = 0;
    }

    void OnEnable()
    {
        t = 0;
    }

    // Update is called once per frame
    //void Update()
    void FixedUpdate()
    {

        //update the radius amplitude and waveSize
        mat.SetFloat("_radius", radiusAnim.Evaluate(t));
        mat.SetFloat("_amplitude", amplitudeAnim.Evaluate(t));
        mat.SetFloat("_wavesize", wavesizeAnim.Evaluate(t));
        mat.SetColor("_color", colorAnim.Evaluate(t));
        mat.SetFloat("_saturation", SatAnim.Evaluate(t));

        gameObject.transform.position -= gameObject.transform.forward * 0.001f;

        if (speed == 0f)
        {
            return;
        }

        //increment t
        t += (speed * Time.deltaTime);

        if (t > 1f)
        {
            //mat.SetInt("_active", 0);

            if (destoryWhenDone)
            {
                Destroy(gameObject);
            }
        }

    }

    //this part is used in the editor only...allows a preview of the animation by using the slider

#if UNITY_EDITOR || UNITY_EDITOR_OSX || UNITY_EDITOR_64
    [Range(0.0f, 1.0f)]
    public float timePreview_InEditModeOnly = 0f;
    void Update()
    {
        //not while the editor is in play mode
        if (!Application.isPlaying)
        {
            //GetComponent<MeshRenderer>().material = mat;
            //mat = GetComponent<MeshRenderer>().sharedMaterial;

            mat.SetFloat("_radius", radiusAnim.Evaluate(timePreview_InEditModeOnly));
            mat.SetFloat("_amplitude", amplitudeAnim.Evaluate(timePreview_InEditModeOnly));
            mat.SetFloat("_wavesize", wavesizeAnim.Evaluate(timePreview_InEditModeOnly));
            mat.SetColor("_color", colorAnim.Evaluate(timePreview_InEditModeOnly));
            mat.SetFloat("_saturation", SatAnim.Evaluate(timePreview_InEditModeOnly));
        }
    }
#endif

}
