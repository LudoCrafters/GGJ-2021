using UnityEngine;

public class Actor : MonoBehaviour
{
    [Tooltip("Represents the affiliation (or team) of the actor. Actors of the same affiliation are friendly to eachother")]
    public int affiliation;
    [Tooltip("Represents point where other actors will aim when they attack this actor")]
    public Transform aimPoint;

    ActorsManager m_ActorsManager;

    private void Start()
    {
        m_ActorsManager = GameObject.FindObjectOfType<ActorsManager>();

        // Register as an actor
        if (!m_ActorsManager.actors.Contains(this))
        {
            m_ActorsManager.actors.Add(this);
        }
    }

    private void OnDestroy()
    {
        // Unregister as an actor
        if (m_ActorsManager)
        {
            m_ActorsManager.actors.Remove(this);
        }
    }
}
