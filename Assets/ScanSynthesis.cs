using UnityEngine;

public class ScanSynthesis : MonoBehaviour
{
    public ChuckSubInstance scanSynthChuck;
    public AudioSource chuckAudioSource;

    [Header("Scan Sound Parameters")]
    public float baseFreq = 220f;
    public float baseFreqJewels = 330f;
    public float baseFreqSpikes = 440f;

    public float modDepth = 20f;
    public float modDepthJewels = 10f;
    public float modDepthSpikes = 20f;

    public float scanWaveMagnitude = 0.05f;
    public float scanWaveMagnitudeJewels = 0.03f;
    public float scanWaveMagnitudeSpikes = 0.05f;

    public float k = 0.005f;
    public float kJewels = 0.005f;
    public float kSpikes = 0.005f;

    public float d = 0.01f;
    public float dJewels = 0.01f;
    public float dSpikes = 0.01f;

    public float modCurveMultiplier = 10f;
    public float modRangeMultiplier = 20f;


    
    public void SetStereoPan(float progress)
    {
        if (chuckAudioSource != null)
        {
            float panValue = Mathf.Clamp((progress - 0.5f) * 2f, -1f, 1f);
            chuckAudioSource.panStereo = panValue;
        }
    }

    void Start()
    {
        scanSynthChuck.RunCode(@"
            global float scanWave[64];
            global float scanWaveJewels[64];
            global float scanWaveSpikes[64];

            global float pos[64];
            global float posJewels[64];
            global float posSpikes[64];

            global float vel[64];
            global float velJewels[64];
            global float velSpikes[64];

            global float baseFreq;
            global float modulatedFreq;
            global float baseFreqJewels;
            global float baseFreqSpikes;

            global float modDepth;
            global float modDepthJewels;
            global float modDepthSpikes;

            global float scanWaveMagnitude;
            global float scanWaveMagnitudeJewels;
            global float scanWaveMagnitudeSpikes;
            global float scanWaveAverage;

            global float modCurveMultiplier; 
            global float modRangeMultiplier;

            global float k;
            global float kJewels;
            global float kSpikes;

            global float d;
            global float dJewels;
            global float dSpikes;

            global float scanXProgress;

            global int scanIsActive;
            global Event scanTrigger;
            global Event scanTriggerJewels;
            global Event scanTriggerSpikes;
 
            global int jewelHitActive;
            global int spikeHitActive;

            spork ~ scan();

            // Function to zero pos/vel on event
            fun void resetScan(Event e, float positions[], float velocities[]) {
                while (true) {
                    e => now;
                    for (0 => int i; i < 64; i++) {
                        0.0 => positions[i];
                        0.0 => velocities[i];
                    }
                }
            }

            // Launch reset handlers
            spork ~ resetScan(scanTrigger, pos, vel);
            spork ~ resetScan(scanTriggerJewels, posJewels, velJewels);
            spork ~ resetScan(scanTriggerSpikes, posSpikes, velSpikes);

            fun float computeAverage(float arr[]) {
                0 => float sum;
                for (30 => int i; i < 36; i++) {
                    (arr[i] * 3.0) +=> sum;
                }
                return sum / arr.cap();
            }


            fun void scan() {
                while (true) {
                    for (0 => int i; i < 64; i++) {
                        int prev;
                        int next;
                        (i + 63) % 64 => prev;
                        (i + 1) % 64 => next;

                        float force;
                        float forceJewels;
                        float forceSpikes;

                        -k * (2 * pos[i] - pos[prev] - pos[next]) - d * vel[i] + (scanWave[i] * scanWaveMagnitude) => force;
                        -kJewels * (2 * posJewels[i] - posJewels[prev] - posJewels[next]) - dJewels * velJewels[i] + (scanWaveJewels[i] * scanWaveMagnitudeJewels) => forceJewels;
                        -kSpikes * (2 * posSpikes[i] - posSpikes[prev] - posSpikes[next]) - dSpikes * velSpikes[i] + (scanWaveSpikes[i] * scanWaveMagnitudeSpikes) => forceSpikes;
                        

                        vel[i] + force => vel[i];
                        pos[i] + vel[i] => pos[i];

                        velJewels[i] + forceJewels => velJewels[i];
                        posJewels[i] + velJewels[i] => posJewels[i];

                        velSpikes[i] + forceSpikes => velSpikes[i];
                        posSpikes[i] + velSpikes[i] => posSpikes[i];
                    }

                    computeAverage(scanWave) => scanWaveAverage;
                    (baseFreq + (scanWaveAverage * modCurveMultiplier) * modRangeMultiplier) => modulatedFreq; // these two values here are the ones to mod with

                    1.0::ms => now; 
                }
            }


            // Audio-rate scanning
            SinOsc s => dac;
            0 => int idx;

            TriOsc jewels => dac;
            0 => int jewels_idx;

            SawOsc spikes => dac;
            0 => int spikes_idx;

            SinOsc mazeTone => dac;
            0.0 => mazeTone.gain;


            while (true) {
                if (scanIsActive == 1) {
                    modulatedFreq + (pos[idx] * modDepth) => s.freq;
                    0.2 => s.gain;

                    modulatedFreq + (scanWaveAverage * 300) => mazeTone.freq;
                    0.2 => mazeTone.gain;
                    /// <<< mazeTone.freq() >>>;

                    baseFreqJewels + (posJewels[jewels_idx] * modDepthJewels) => jewels.freq;
                    if (jewelHitActive == 1) {
                        0.3 => jewels.gain;
                    } else {
                        0.0 => jewels.gain;
                    }

                    baseFreqSpikes + (posSpikes[spikes_idx] * modDepthSpikes) => spikes.freq;
                    if (spikeHitActive == 1) {
                        0.3 => spikes.gain;
                    } else {
                        0.0 => spikes.gain;
                    }

                } else {
                    0.0 => s.gain;
                    0.0 => mazeTone.gain;
                    0.0 => jewels.gain;
                    0.0 => spikes.gain;
                }
            
                idx++;
                if (idx >= 64) { 0 => idx; }

                jewels_idx++;
                if (jewels_idx >= 64) { 0 => jewels_idx; }

                spikes_idx++;
                if (spikes_idx >= 64) { 0 => spikes_idx; }

                1::samp => now;
            }

            
        ");

        scanSynthChuck.SetFloat("baseFreq", baseFreq);
        scanSynthChuck.SetFloat("baseFreqJewels", baseFreqJewels);
        scanSynthChuck.SetFloat("baseFreqSpikes", baseFreqSpikes);

        scanSynthChuck.SetFloat("modDepth", modDepth);
        scanSynthChuck.SetFloat("modDepthJewels", modDepthJewels);
        scanSynthChuck.SetFloat("modDepthSpikes", modDepthSpikes);

        scanSynthChuck.SetFloat("scanWaveMagnitude", scanWaveMagnitude);
        scanSynthChuck.SetFloat("scanWaveMagnitudeJewels", scanWaveMagnitudeJewels);
        scanSynthChuck.SetFloat("scanWaveMagnitudeSpikes", scanWaveMagnitudeSpikes);

        scanSynthChuck.SetFloat("k", k);
        scanSynthChuck.SetFloat("kJewels", kJewels);
        scanSynthChuck.SetFloat("kSpikes", kSpikes);

        scanSynthChuck.SetFloat("d", d);
        scanSynthChuck.SetFloat("dJewels", dJewels);
        scanSynthChuck.SetFloat("dSpikes", dSpikes);

        scanSynthChuck.SetFloat("modCurveMultiplier", modCurveMultiplier);
        scanSynthChuck.SetFloat("modRangeMultiplier", modRangeMultiplier);
    
    }
}