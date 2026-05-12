using UnityEngine;
using TMPro;

public class PendulumSwing : MonoBehaviour
{
    public float maxAngle = 20f;
    public float gravity = 9.8f;

    public TMP_InputField oscillationInput;
    public TMP_InputField lengthInput;
    public TMP_Text resultText;

    public LineRenderer line;
    public Transform bar;
    public Transform bob;

    private float omega;
    private float timeElapsed;
    private Quaternion initialRotation;

    private bool isSwinging = false;
    private bool isManualMode = false;

    private int targetOscillations = 20;
    private int oscillationCount = 0;

    private float totalTime = 0f;
    private float previousAngle = 0f;

    private float length = 2f;

    private Camera cam;

    void Start()
    {
        initialRotation = transform.localRotation;

        line.positionCount = 2;

        cam = Camera.main;

        if (cam == null)
            Debug.LogError("MainCamera not found. Tag your camera as MainCamera.");

        omega = Mathf.Sqrt(gravity / length);
    }

    void Update()
    {
        HandleInput();

        if (isSwinging)
        {
            timeElapsed += Time.deltaTime;
            totalTime += Time.deltaTime;

            float angle = maxAngle * Mathf.Cos(omega * timeElapsed);

            transform.localRotation = initialRotation * Quaternion.Euler(0, 0, angle);

            // Count oscillations
            if (previousAngle < 0 && angle >= 0)
            {
                oscillationCount++;

                if (oscillationCount >= targetOscillations)
                {
                    StopExperiment();
                }
            }

            previousAngle = angle;
        }
    }

    void LateUpdate()
    {
        line.SetPosition(0, bar.position);
        line.SetPosition(1, bob.position);
    }

    void HandleInput()
    {
        // START DRAG ONLY IF BOB IS CLICKED
        if (IsPointerDown())
        {
            Ray ray = cam.ScreenPointToRay(GetPointerPosition());
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                if (hit.transform == bob)
                {
                    isManualMode = true;
                    isSwinging = false;
                }
            }
        }

        // DRAGGING
        if (isManualMode && IsPointerHeld())
        {
            Vector3 p = GetPointerPosition();
            p.z = Vector3.Distance(cam.transform.position, bar.position);

            Vector3 world = cam.ScreenToWorldPoint(p);
            Vector3 dir = world - bar.position;

            float angle = Mathf.Atan2(dir.x, -dir.y) * Mathf.Rad2Deg;

            transform.localRotation = initialRotation * Quaternion.Euler(0, 0, angle);

            return;
        }

        // RELEASE → START SWING AUTOMATICALLY
        if (isManualMode && IsPointerUp())
        {
            isManualMode = false;

            float currentAngle = transform.localEulerAngles.z;
            if (currentAngle > 180) currentAngle -= 360;

            maxAngle = currentAngle;

            // 🔥 READ USER INPUTS HERE
            float.TryParse(lengthInput.text, out length);
            int.TryParse(oscillationInput.text, out targetOscillations);
            // Convert User input from cm to m
            length = length / 100f;
            //Set Default values
            if (length <= 0) length = 0.5f;
            if (targetOscillations <= 0) targetOscillations = 20;

            // 🔥 UPDATE PHYSICS
            omega = Mathf.Sqrt(gravity / length);

            // RESET EXPERIMENT
            timeElapsed = 0f;
            totalTime = 0f;
            oscillationCount = 0;
            previousAngle = maxAngle;

            isSwinging = true;

            resultText.text = "Running..";
        }
    }

    // ---------------- INPUT SYSTEM ----------------

    bool IsPointerDown()
    {
        return Input.GetMouseButtonDown(0) ||
               (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began);
    }

    bool IsPointerHeld()
    {
        return Input.GetMouseButton(0) ||
               (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Moved);
    }

    bool IsPointerUp()
    {
        return Input.GetMouseButtonUp(0) ||
               (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Ended);
    }

    Vector3 GetPointerPosition()
    {
        if (Input.touchCount > 0)
            return Input.GetTouch(0).position;

        return Input.mousePosition;
    }
    // =====================================================
    // LENGTH BUTTONS
    // =====================================================

    public void IncreaseLength()
    {
        float currentLength = 0f;

        float.TryParse(lengthInput.text, out currentLength);

        currentLength += 1f;

        // Prevent excessively large values
        if (currentLength > 300f)
            currentLength = 300f;

        lengthInput.text = currentLength.ToString("F0");
    }

    public void DecreaseLength()
    {
        float currentLength = 0f;

        float.TryParse(lengthInput.text, out currentLength);

        currentLength -= 1f;

        // Prevent zero or negative length
        if (currentLength < 1f)
            currentLength = 1f;

        lengthInput.text = currentLength.ToString("F0");
    }

    // =====================================================
    // OSCILLATION BUTTONS
    // =====================================================

    public void IncreaseOscillations()
    {
        int currentOscillations = 0;

        int.TryParse(oscillationInput.text, out currentOscillations);

        currentOscillations += 1;

        // Optional maximum
        if (currentOscillations > 100)
            currentOscillations = 100;

        oscillationInput.text = currentOscillations.ToString();
    }

    public void DecreaseOscillations()
    {
        int currentOscillations = 0;

        int.TryParse(oscillationInput.text, out currentOscillations);

        currentOscillations -= 1;

        // Prevent invalid oscillation count
        if (currentOscillations < 1)
            currentOscillations = 1;

        oscillationInput.text = currentOscillations.ToString();
    }
    void StopExperiment()
    {
        isSwinging = false;

        resultText.text =
            "Time = " + totalTime.ToString("F2") + " s";
        ExperimentData.Add(length, totalTime, targetOscillations);
    }
}