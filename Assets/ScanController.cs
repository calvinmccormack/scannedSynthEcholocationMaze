using UnityEngine;
using UnityEngine.Audio;

public class ScanController : MonoBehaviour
{

    [Header("Scan Settings")]
    public float scanDuration = 2.0f;
    public float scanWidth = 5f;

    public AudioSource chuckAudioSource;

    private bool isScanning = false;
    private float scanProgress = 0f;

    public ScanSynthesis scanSynthesis;

    public Material verticalScanMaterial;


    void Update()
    {
        
        // Start scan on key press (can change this later)
        if (Input.GetKeyDown(KeyCode.R) && !isScanning)
        {
            StartScan();
        }

        // Update scan progress if active
        if (isScanning)
        {
            scanProgress += Time.deltaTime / scanDuration;

            if (scanSynthesis != null)
            {
                scanSynthesis.SetStereoPan(scanProgress);
            }

            // Send to shader
            if (verticalScanMaterial != null)
            {
                Vector3 scanOrigin = mainCam.transform.position;
                Vector3 forward = mainCam.transform.forward;
                float baseAngle = Mathf.Atan2(mainCam.transform.forward.x, mainCam.transform.forward.z);
                float sweepAngle = baseAngle + Mathf.Lerp(-Mathf.PI / 2f, Mathf.PI / 2f, scanProgress);

                verticalScanMaterial.SetVector("_ScanOrigin", scanOrigin);
                verticalScanMaterial.SetFloat("_ScanAngle", sweepAngle);
                verticalScanMaterial.SetFloat("_RingWidth", scanWidth); // in radians (e.g., 0.1)
            }

            if (scanProgress >= 1.0f)
            {
                isScanning = false;
                scanProgress = 1.0f;
                verticalScanMaterial.SetFloat("_ScanVisible", 0);

            }
            else if (isScanning && scanSynthChuck != null)
            {
                scanSynthChuck.SetInt("scanIsActive", 1);

                scanSynthChuck.SetFloat("scanXProgress", scanProgress);
            }

            SampleAtScanX(scanProgress);
            
            if (isScanning && chuckAudioSource != null)
            {
                chuckAudioSource.panStereo = (scanProgress - 0.5f) * 2f;
            }

        }

        else if (!isScanning && scanSynthChuck != null)
        {
            scanSynthChuck.SetInt("scanIsActive", 0);
        }
    }

    public void StartScan()
    {
        isScanning = true;
        scanProgress = 0f;
        verticalScanMaterial.SetFloat("_ScanVisible", 1);

        if (scanSynthChuck != null)
        {
            scanSynthChuck.BroadcastEvent("scanTrigger");
            scanSynthChuck.BroadcastEvent("scanTriggerJewels");
            scanSynthChuck.BroadcastEvent("scanTriggerSpikes");
        }

        // Debug.Log("Scan started.");
    }

    void OnScanComplete()
    {
        // You could reset or trigger next phase here if needed
    }

    [SerializeField] private Camera mainCam;
    public ChuckSubInstance scanSynthChuck;
    public LayerMask scanLayer;
    public LayerMask scanLayerJewels;
    public LayerMask scanLayerSpikes;
    [SerializeField] private int rayCount = 32;
    [SerializeField] private int rayCountJewels = 32;
    [SerializeField] private int rayCountSpikes = 32;
    [SerializeField] private float scanDepth = 20f; // how far forward to scan

    void SampleAtScanX(float progress)
    {
        if (!mainCam || scanSynthChuck == null) return;

        float[] heightSamples = new float[rayCount];
        float[] heightSamplesJewels = new float[rayCountJewels];
        float[] heightSamplesSpikes = new float[rayCountSpikes];

        for (int i = 0; i < rayCount; i++)
        {
            float y = 1f - (i / (float)(rayCount - 1));
            Vector3 screenPoint = new Vector3(progress * Screen.width, y * Screen.height, 0);
            Ray ray = mainCam.ScreenPointToRay(screenPoint);

            if (Physics.Raycast(ray, out RaycastHit hit, scanDepth, scanLayer))
            {
                heightSamples[i] = hit.point.y;
            }
            else
            {
                heightSamples[i] = 0f;
            }

        }


        for (int i = 0; i < rayCount; i++)
        {
            float y = 1f - (i / (float)(rayCount - 1));
            Vector3 screenPoint = new Vector3(progress * Screen.width, y * Screen.height, 0);
            Ray ray = mainCam.ScreenPointToRay(screenPoint);

            // Check for maze wall hit
            if (Physics.Raycast(ray, out RaycastHit wallHit, scanDepth, scanLayer))
            {
                float wallDist = wallHit.distance;

                // Only use jewel hit if it's before the wall
                if (Physics.Raycast(ray, out RaycastHit jewelHit, wallDist, scanLayerJewels))
                {
                    heightSamplesJewels[i] = jewelHit.point.y;
                }
                else
                {
                    heightSamplesJewels[i] = 0f;
                }
            }
            else
            {
                // No wall; proceed with normal jewel hit check
                if (Physics.Raycast(ray, out RaycastHit jewelHit, scanDepth, scanLayerJewels))
                {
                    heightSamplesJewels[i] = jewelHit.point.y;
                }
                else
                {
                    heightSamplesJewels[i] = 0f;
                }
            }
        }
        for (int i = 0; i < rayCount; i++)
        {
            float y = 1f - (i / (float)(rayCount - 1));
            Vector3 screenPoint = new Vector3(progress * Screen.width, y * Screen.height, 0);
            Ray ray = mainCam.ScreenPointToRay(screenPoint);

            // Check for maze wall hit
            if (Physics.Raycast(ray, out RaycastHit wallHit, scanDepth, scanLayer))
            {
                float wallDist = wallHit.distance;

                // Only use spike hit if it's before the wall
                if (Physics.Raycast(ray, out RaycastHit spikeHit, wallDist, scanLayerSpikes))
                {
                    heightSamplesSpikes[i] = spikeHit.point.y;
                }
                else
                {
                    heightSamplesSpikes[i] = 0f;
                }
            }
            else
            {
                // No wall; proceed with normal spike hit check
                if (Physics.Raycast(ray, out RaycastHit spikeHit, scanDepth, scanLayerSpikes))
                {
                    heightSamplesSpikes[i] = spikeHit.point.y;
                }
                else
                {
                    heightSamplesSpikes[i] = 0f;
                }
            }
        }

        double[] heightSamplesDoubles = new double[rayCount];
        for (int i = 0; i < rayCount; i++)
        {
            heightSamplesDoubles[i] = (double)heightSamples[i];
        }

        double[] heightSamplesJewelsDoubles = new double[rayCountJewels];
        for (int i = 0; i < rayCountJewels; i++)
        {
            heightSamplesJewelsDoubles[i] = (double)heightSamplesJewels[i];
        }

        double[] heightSamplesSpikesDoubles = new double[rayCountSpikes];
        for (int i = 0; i < rayCountSpikes; i++)
        {
            heightSamplesSpikesDoubles[i] = (double)heightSamplesSpikes[i];
        }

        scanSynthChuck.SetFloatArray("scanWave", heightSamplesDoubles);

        // Debug.Log($"[Scan] Sending scanWave to ChucK. Center sample: {heightSamples[rayCount / 2]}");

        scanSynthChuck.SetFloatArray("scanWaveJewels", heightSamplesJewelsDoubles);

        // Debug.Log($"[Scan] Sending scanWaveJewels to ChucK. Center sample: {heightSamplesJewels[rayCount / 2]}");

        scanSynthChuck.SetFloatArray("scanWaveSpikes", heightSamplesSpikesDoubles);

        // Debug.Log($"[Scan] Sending scanWaveSpikes to ChucK. Center sample: {heightSamplesSpikes[rayCount / 2]}"); 

        bool jewelHitThisScan = false;
        bool spikeHitThisScan = false;

        for (int i = 0; i < rayCountJewels; i++)
        {
            if (heightSamplesJewels[i] != 0f)
            {
                jewelHitThisScan = true;
                break;
            }
        }

        for (int i = 0; i < rayCountSpikes; i++)
        {
            if (heightSamplesSpikes[i] != 0f)
            {
                spikeHitThisScan = true;
                break;
            }
        }

        scanSynthChuck.SetInt("jewelHitActive", jewelHitThisScan ? 1 : 0);
        scanSynthChuck.SetInt("spikeHitActive", spikeHitThisScan ? 1 : 0);       
    }
}