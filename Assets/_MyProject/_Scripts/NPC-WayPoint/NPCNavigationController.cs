/*using UnityEngine;

// This script assumes you have an Animator component on the same GameObject.
[RequireComponent(typeof(Animator))]
public class NPCNavigationController : MonoBehaviour
{
    // --- Public variables visible in the Inspector ---
    [Tooltip("The speed at which the character moves forward.")]
    public float movementSpeed = 1f;

    [Tooltip("The speed at which the character turns to face its destination.")]
    public float rotationSpeed = 120f;

    [Tooltip("The distance from the destination at which the character will stop moving.")]
    public float stopDistance = 2.5f;

    // --- Public properties for script communication ---
    [Tooltip("The current target destination for the character.")]
    [SerializeField] // Use SerializeField to see private variables in the Inspector
    private Vector3 destination;

    [Tooltip("A flag indicating whether the character has reached its destination.")]
    [SerializeField]
    private bool reachedDestination;

    // --- Private variables for internal logic ---
    private Vector3 lastPosition;
    private Vector3 velocity;
    private Animator animator;

    /// <summary>
    /// Called when the script instance is being loaded.
    /// </summary>
    void Awake()
    {
        // Get the Animator component attached to this GameObject.
        animator = GetComponent<Animator>();

        // Set the initial destination to the character's starting position
        // to ensure it doesn't move until a new destination is set.
        destination = transform.position;
        reachedDestination = true;

        // Initialize lastPosition for velocity calculation.
        lastPosition = transform.position;
    }

    /// <summary>
    /// Update is called once per frame.
    /// </summary>
    void Update()
    {
        // Check if the character is not at its destination.
        if (transform.position != destination)
        {
            // Calculate the direction vector towards the destination, ignoring the y-axis.
            Vector3 destinationDirection = destination - transform.position;
            destinationDirection.y = 0;

            // Calculate the remaining distance to the destination.
            float destinationDistance = destinationDirection.magnitude;

            // If the character is further away than the stopping distance, continue moving.
            if (destinationDistance > stopDistance)
            {
                reachedDestination = false;

                // Determine the target rotation to look at the destination.
                Quaternion targetRotation = Quaternion.LookRotation(destinationDirection);

                // Smoothly rotate towards the target rotation.
                transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

                // Move the character forward.
                transform.Translate(Vector3.forward * movementSpeed * Time.deltaTime);
            }
            // Otherwise, the character has arrived.
            else
            {
                reachedDestination = true;
            }
        }

        // --- Animation Logic ---

        // Calculate the velocity based on the position change since the last frame.
        velocity = (transform.position - lastPosition) / Time.deltaTime;
        lastPosition = transform.position;

        // We only care about movement on the XZ plane, so we zero out the Y velocity.
        velocity.y = 0;

        // Get the speed (magnitude) of the velocity.
        var velocityMagnitude = velocity.magnitude;

        // Normalize the velocity to get a pure direction vector.
        velocity = velocity.normalized;

        // Calculate the dot products to determine the forward and sideways components of the velocity
        // relative to the character's orientation.
        var fwdDotProduct = Vector3.Dot(transform.forward, velocity);
        var rightDotProduct = Vector3.Dot(transform.right, velocity);

        // Pass these values to the Animator to drive the blend tree for animation.
        animator.SetFloat("Horizontal", rightDotProduct);
        animator.SetFloat("Forward", fwdDotProduct);
    }

    /// <summary>
    /// Sets a new destination for the NPC to move towards.
    /// This function can be called from other scripts to control the character.
    /// </summary>
    /// <param name="destination">The new target position in world space.</param>
    public void SetDestination(Vector3 destination)
    {
        this.destination = destination;
        reachedDestination = false;
    }

    /// <summary>
    /// A public property to allow other scripts to check if the destination has been reached.
    /// </summary>
    public bool ReachedDestination
    {
        get { return reachedDestination; }
    }
}*/

using UnityEngine;

[RequireComponent(typeof(Animator))]
public class NPCNavigationController : MonoBehaviour
{
    // --- Public variables ---
    public float movementSpeed = 1f;
    public float rotationSpeed = 120f;
    public float stopDistance = 2.5f;

    // --- Private variables ---
    [SerializeField] private Vector3 destination;
    [SerializeField] private bool reachedDestination;
    private Vector3 lastPosition;
    private Vector3 velocity;
    private Animator animator;
    private bool shouldUpdateYAxis;

    void Awake()
    {
        animator = GetComponent<Animator>();
        destination = transform.position;
        reachedDestination = true;
        lastPosition = transform.position;
    }

    void Update()
    {
        if (!reachedDestination)
        {
            // --- ROTATION LOGIC (Stays the same) ---
            // We calculate a flat direction for rotation so the NPC doesn't tilt up/down.
            Vector3 rotationDirection = destination - transform.position;
            rotationDirection.y = 0;

            if (rotationDirection != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(rotationDirection);
                transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
            }

            // --- MOVEMENT LOGIC (MODIFIED) ---
            // We use MoveTowards to move towards the actual 3D destination point.
            // This will correctly handle changes in the Y-axis for stairs and ramps.
            transform.position = Vector3.MoveTowards(transform.position, destination, movementSpeed * Time.deltaTime);

            // Check if we've reached the destination after moving.
            if (Vector3.Distance(transform.position, destination) < stopDistance)
            {
                reachedDestination = true;
            }
        }

        // --- Animation Logic ---
        velocity = (transform.position - lastPosition) / Time.deltaTime;
        lastPosition = transform.position;
        velocity.y = 0; // Animation is based on horizontal speed
        animator.SetFloat("Forward", velocity.magnitude); // Simplified this for better animation control
    }

    public void SetDestination(Vector3 destination, bool updateY)
    {
        this.destination = destination;
        this.shouldUpdateYAxis = updateY;
        reachedDestination = false;
    }

    public bool ReachedDestination
    {
        get { return reachedDestination; }
    }
}