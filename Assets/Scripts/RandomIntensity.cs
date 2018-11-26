using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomIntensity : MonoBehaviour {

    public float maximumIntensity;
    public float minimumIntensity;
    public float fadeTime;
    public float maxIntensityChangePerSecond;
    public float intensityChangeIntervalSeconds;

    private new Light light;
    private float timeSinceIntensityChange = 0.0f;
    private float currentIntensity;
    private float previousIntensity;
    private float secondsSinceFadeStart;

	// Use this for initialization
	void Start () {
        light = GetComponent<Light>();
        currentIntensity = light.intensity;
	}
	
	// Update is called once per frame
	void Update () {
        timeSinceIntensityChange += Time.deltaTime;

        if (timeSinceIntensityChange > intensityChangeIntervalSeconds)
        {
            currentIntensity = Random.Range(minimumIntensity, maximumIntensity);
            previousIntensity = light.intensity;
            timeSinceIntensityChange = 0.0f;
            secondsSinceFadeStart = 0.0f;
        }

        if (light.intensity != currentIntensity)
        {
        secondsSinceFadeStart += Time.deltaTime;
            float t = secondsSinceFadeStart / fadeTime;
            if (t > 1.0f) { t = 1.0f; }
            light.intensity = Mathf.Lerp(previousIntensity, currentIntensity, t);
        }

        light.intensity = Mathf.Clamp(light.intensity, minimumIntensity, maximumIntensity);
    }
}
