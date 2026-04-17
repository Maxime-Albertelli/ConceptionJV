using UnityEngine;

public class IndicatorArrow : MonoBehaviour
{
    [Header("Paramètres d'orbite")]
    [Tooltip("La distance de la flèche par rapport au joueur")]
    [SerializeField] private float orbitRadius = 1.5f;

    [Tooltip("La hauteur de la flèche par rapport au centre du joueur")]
    [SerializeField] private float heightOffset = 0.5f;

    private Transform playerTransform;
    private Transform targetTeleporter;
    private Transform planetTransform;

    private void Awake()
    {
        // On récupère le transform du parent (le joueur)
        playerTransform = transform.parent;
        gameObject.SetActive(false);
    }

    public void Show(Transform teleporter, Transform currentPlanet)
    {
        targetTeleporter = teleporter;
        planetTransform = currentPlanet;
        gameObject.SetActive(true);
    }

    public void Hide()
    {
        targetTeleporter = null;
        planetTransform = null;
        gameObject.SetActive(false);
    }

    private void Update()
    {
        if (targetTeleporter == null || planetTransform == null) return;

        // --- 1. Calcul des vecteurs de base ---

        // Le "haut" local du joueur (vecteur allant du centre de la planète vers le joueur)
        Vector3 playerUp = (playerTransform.position - planetTransform.position).normalized;

        // La direction brute du joueur vers le téléporteur
        Vector3 directionToTP = targetTeleporter.position - playerTransform.position;

        // --- 2. Projection sur le sol (Plan Tangent) ---

        // ProjectOnPlane "écrase" notre direction sur le plan plat sous les pieds du joueur.
        // Cela nous donne la direction exacte à suivre à la surface de la planète.
        Vector3 projectedDirection = Vector3.ProjectOnPlane(directionToTP, playerUp).normalized;

        // Petite sécurité : si on est pile sur le téléporteur, on arrête les calculs pour éviter les bugs de rotation
        if (projectedDirection.sqrMagnitude < 0.001f) return;

        // --- 3. Positionnement en Orbite ---

        // On part de la position du joueur
        // On monte un petit peu (heightOffset) pour que la flèche soit au niveau du torse/tête
        // On avance dans la direction du téléporteur selon le rayon voulu (orbitRadius)
        transform.position = playerTransform.position
                             + (playerUp * heightOffset)
                             + (projectedDirection * orbitRadius);

        // --- 4. Rotation ---

        // On oriente la flèche pour qu'elle regarde dans la direction calculée, 
        // en gardant le vecteur playerUp comme "plafond" pour qu'elle reste bien droite par rapport à la planète.
        transform.rotation = Quaternion.LookRotation(projectedDirection, playerUp);
    }
}