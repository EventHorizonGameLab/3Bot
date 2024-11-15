using UnityEngine;

public class ProceduralWalk : MonoBehaviour
{
    [SerializeField] private Transform _leftTarget;
    [SerializeField] private Transform _rightTarget;
    [SerializeField] private Transform _leftIkTarget;
    [SerializeField] private Transform _rightIkTarget;
    [SerializeField] private float stepThreshold = 0.5f;   // Soglia del passo
    [SerializeField] private float stepSpeed = 2f;         // Velocità del passo
    [SerializeField] private float stepHeight = 0.2f;      // Altezza del passo
    [SerializeField] private float strideLength = 1.0f;    // Lunghezza della falcata (maggiore influsso)
    [SerializeField] private LayerMask groundLayer;        // Layer del terreno

    private bool _leftIsStepping = false;     // Stato del passo sinistro
    private bool _rightIsStepping = false;    // Stato del passo destro
    private Vector3 _leftStepStartPos;        // Posizione iniziale del passo sinistro
    private Vector3 _rightStepStartPos;       // Posizione iniziale del passo destro
    private Vector3 _leftStepTargetPos;       // Posizione di destinazione del passo sinistro
    private Vector3 _rightStepTargetPos;      // Posizione di destinazione del passo destro
    private float _leftStepProgress;          // Progresso del passo sinistro
    private float _rightStepProgress;         // Progresso del passo destro

    private void Update()
    {
        AdjustToGround(_leftTarget);  // Aggiorna la posizione del target sinistro al terreno
        AdjustToGround(_rightTarget); // Aggiorna la posizione del target destro al terreno
        AdjustToGround(_leftIkTarget); // Aggiorna la posizione del piede sinistro al terreno
        AdjustToGround(_rightIkTarget); // Aggiorna la posizione del piede destro al terreno
        CheckPosition();
    }

    private void AdjustToGround(Transform point)
    {
        // Effettua un raycast verso il basso per aggiornare la posizione y
        if (Physics.Raycast(point.position + Vector3.up, Vector3.down, out RaycastHit hit, 2f, groundLayer))
        {
            point.position = new Vector3(point.position.x, hit.point.y, point.position.z);
        }
    }

    private void CheckPosition()
    {
        // Controlla la distanza tra il _leftTarget e il centro della sfera di controllo
        float leftDistance = Vector3.Distance(_leftTarget.position, transform.position);
        float rightDistance = Vector3.Distance(_rightTarget.position, transform.position);

        if (!_leftIsStepping && leftDistance > stepThreshold)
        {
            StartLeftStep();
        }
        else if (!_rightIsStepping && rightDistance > stepThreshold)
        {
            StartRightStep();
        }

        // Continua il passo del piede sinistro se è in corso
        if (_leftIsStepping)
        {
            ContinueLeftStep();
        }

        // Continua il passo del piede destro se è in corso
        if (_rightIsStepping)
        {
            ContinueRightStep();
        }
    }

    private void StartLeftStep()
    {
        _leftIsStepping = true;
        _leftStepProgress = 0f;
        _leftStepStartPos = _leftIkTarget.position;

        // Calcola la direzione opposta al movimento del target sinistro
        Vector3 directionToCenter = (transform.position - _leftIkTarget.position).normalized;
        _leftStepTargetPos = _leftIkTarget.position + directionToCenter * strideLength;

        // Verifica che la destinazione sia sul terreno
        if (Physics.Raycast(_leftStepTargetPos + Vector3.up, Vector3.down, out RaycastHit hit, 2f, groundLayer))
        {
            _leftStepTargetPos.y = hit.point.y;
        }
    }

    private void StartRightStep()
    {
        _rightIsStepping = true;
        _rightStepProgress = 0f;
        _rightStepStartPos = _rightIkTarget.position;

        // Calcola la direzione opposta al movimento del target destro
        Vector3 directionToCenter = (transform.position - _rightIkTarget.position).normalized;
        _rightStepTargetPos = _rightIkTarget.position + directionToCenter * strideLength;

        // Verifica che la destinazione sia sul terreno
        if (Physics.Raycast(_rightStepTargetPos + Vector3.up, Vector3.down, out RaycastHit hit, 2f, groundLayer))
        {
            _rightStepTargetPos.y = hit.point.y;
        }
    }

    private void ContinueLeftStep()
    {
        _leftStepProgress += Time.deltaTime * stepSpeed;

        // Interpolazione tra posizione iniziale e destinazione del passo
        Vector3 newPosition = Vector3.Lerp(_leftStepStartPos, _leftStepTargetPos, _leftStepProgress);

        // Aggiunge l’altezza del passo per il sollevamento
        newPosition.y += Mathf.Sin(_leftStepProgress * Mathf.PI) * stepHeight;

        _leftIkTarget.position = newPosition;

        // Se il passo è completato, ferma il movimento e porta il target alla nuova posizione dell'ikTarget
        if (_leftStepProgress >= 1f)
        {
            _leftIsStepping = false;
            _leftTarget.position = _leftIkTarget.position; // Aggiorna la posizione del target sinistro
        }
    }

    private void ContinueRightStep()
    {
        _rightStepProgress += Time.deltaTime * stepSpeed;

        // Interpolazione tra posizione iniziale e destinazione del passo
        Vector3 newPosition = Vector3.Lerp(_rightStepStartPos, _rightStepTargetPos, _rightStepProgress);

        // Aggiunge l’altezza del passo per il sollevamento
        newPosition.y += Mathf.Sin(_rightStepProgress * Mathf.PI) * stepHeight;

        _rightIkTarget.position = newPosition;

        // Se il passo è completato, ferma il movimento e porta il target alla nuova posizione dell'ikTarget
        if (_rightStepProgress >= 1f)
        {
            _rightIsStepping = false;
            _rightTarget.position = _rightIkTarget.position; // Aggiorna la posizione del target destro
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(_leftTarget.position, 0.1f);
        Gizmos.color = Color.green;
        Gizmos.DrawSphere(_leftIkTarget.position, 0.1f);
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, stepThreshold);

        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(_rightTarget.position, 0.1f);
        Gizmos.color = Color.magenta;
        Gizmos.DrawSphere(_rightIkTarget.position, 0.1f);
    }
}
