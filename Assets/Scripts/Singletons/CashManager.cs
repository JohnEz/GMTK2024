using UnityEngine;

public class CashManager : MonoBehaviour
{
    public FloatingTextManager floatingTextManager;

    private float _Cash = 0;

    public float Cash => _Cash;

    void Start()
    {
        GameObject target = GameObject.FindWithTag("Player");
        OnJourneyComplete(target, target.GetComponent<RouteManager>().Routes[0]);
    }

    public void OnJourneyComplete(GameObject finalStation, Route route) {
        TrackPiece start = route.TrackPieces[0].Piece;
        TrackPiece end = route.TrackPieces[route.TrackPieces.Count-1].Piece;

        float routeCash = Mathf.Abs(start.X - end.X) + Mathf.Abs(start.Y - end.Y);

        _Cash += routeCash;

        floatingTextManager.Show($"+₮{routeCash}", finalStation);
    }
}
