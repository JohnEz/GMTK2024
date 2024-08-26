using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using DG.Tweening;

public class RouteBuilderManager : Singleton<RouteBuilderManager> {
    private const int VERY_COOL_SCORE = 15;
    private const int EXTREMELY_COOL_SCORE = 50;

    [SerializeField]
    private BuilderControlsController _builderControlsController;

    [SerializeField]
    private TrackPieceController _trackPreviewPrefab;

    private TrackPiece OriginStation;

    private TrackPiece TerminatingStation;

    private TrackPiece EditFromTrackPiece;

    private readonly List<(TrackPieceController controller, Compass direction)> PreviewTrackPieces = new();

    [SerializeField]
    private GhostTrackPiece GhostTrackPiece;

    [SerializeField]
    private AudioClip _routeCompleteSFX;

    [SerializeField]
    private AudioClip _kerchingSFX;

    [SerializeField]
    private AudioClip _coolSFX;

    [SerializeField]
    private AudioClip _veryCoolSFX;

    [SerializeField]
    private AudioClip _extremelyCoolSFX;

    [SerializeField]
    private AudioClip _removeSFX;

    [SerializeField]
    private AudioClip _cancelSFX;

    [SerializeField]
    private List<AudioClip> _placementSfx;

    private void Awake() {
        _builderControlsController.OnHoverPiece += (template) => {
            GhostTrackPiece.TrackPieceType = template.TrackPieceType;
            CheckNextPieceValidity();
        };
        _builderControlsController.OnConfirmPiece += (template) => {
            PlacePiece();
            CheckNextPieceValidity();
        };
        _builderControlsController.OnUndoPiece += () => RemovePiece();
        _builderControlsController.OnClearRoute += () => StopEditing(true);
    }

    private void CheckNextPieceValidity() {
        TrackPiece piece = GhostTrackPiece.Position;
        var terminatingStation = StationManager.Instance.GetConnectingStation(piece);
        GhostTrackPiece.NextStepIsValidPosition = terminatingStation != OriginStation;
    }

    private void Update() {
        if (Input.GetKeyDown(KeyCode.Escape)) {
            StopEditing(true);
        }
    }

    public void StartEditing(Compass direction, TrackPiece fromTrackPiece) {
        if (!GameStateManager.Instance.TrySetState(GameState.KidEditing)) {
            return;
        }

        OriginStation = fromTrackPiece;
        EditFromTrackPiece = OriginStation;
        GhostTrackPiece.gameObject.SetActive(true);
        GhostTrackPiece.SetPosition(direction, EditFromTrackPiece);
    }

    private void StopEditing(bool isCancelRoute) {
        if (!GameStateManager.Instance.TrySetState(GameState.Kid)) {
            return;
        }

        float reclaimAmount = 0;
        PreviewTrackPieces.ForEach(preview => {
            reclaimAmount += preview.controller.TrackPiece.Template.Price;
            Destroy(preview.controller.gameObject);
        });
        PreviewTrackPieces.Clear();
        EditFromTrackPiece = null;
        OriginStation = null;
        TerminatingStation = null;
        GhostTrackPiece.gameObject.SetActive(false);

        if (isCancelRoute) {
            AudioClipOptions cancelSFXOptions = new AudioClipOptions();
            cancelSFXOptions.Volume = .3f;
            AudioManager.Instance.PlaySound(_cancelSFX, cancelSFXOptions);
            BankManager.Instance.Reclaim(reclaimAmount);
        }
    }

    private void PlacePiece() {
        if (!GhostTrackPiece.IsValidPosition) {
            return;
        }

        TrackPiece piece = GhostTrackPiece.Position;
        Compass direction = GhostTrackPiece.Direction;

        TrackPieceController newTrack = Instantiate(_trackPreviewPrefab, transform);

        TerminatingStation = StationManager.Instance.GetConnectingStation(piece);

        if (TerminatingStation == OriginStation) {
            WorldFloatingTextManager.Instance.Show($"It's not home-time yet", newTrack.gameObject);
            Destroy(newTrack);
            return;
        }

        newTrack.TrackPiece = piece;
        newTrack.GetComponentInChildren<SpriteRenderer>().sprite = ToyMapManager.Instance.TrackPieceConfig[piece.Template.TrackPieceType].sprite;
        PreviewTrackPieces.Add((newTrack, direction));

        if (TerminatingStation != null) {
            CommitRoute();
            StopEditing(false);
            return;
        }

        // Big assumption that there's only ever two connections, and the second one is the "exit" or "forward" connector
        Compass nextDirection = piece.Template.ConnectionPoints[1];
        EditFromTrackPiece = piece;
        GhostTrackPiece.SetPosition(nextDirection, piece);

        AudioClipOptions placementSFXOptions = new AudioClipOptions();
        placementSFXOptions.Volume = .3f;
        AudioManager.Instance.PlaySound(_placementSfx, placementSFXOptions);
    }

    private void CommitRoute() {
        Route route = new() {
            StartStation = OriginStation,
            EndStation = TerminatingStation,
        };

        PreviewTrackPieces.ForEach(preview => {
            route.AddConnection(preview.controller.TrackPiece, preview.direction);
        });

        RouteManager.Instance.AddRoute(route);

        AudioClipOptions _routeCompletedSFXOptions = new AudioClipOptions();
        _routeCompletedSFXOptions.Volume = .5f;
        AudioManager.Instance.PlaySound(_routeCompleteSFX, _routeCompletedSFXOptions);

        ShowRouteScores();
    }

    private void ShowRouteScores() {
        List<TrackPiece> trackPieces = PreviewTrackPieces.Select(preview => preview.controller.TrackPiece).ToList();
        var toys = ToyMapManager.Instance.Toys;

        (
            int totalCool,
            List<(TrackPiece piece, int value, int toyIndex)> coolnesses
        ) = CoolManager.Instance.ScoreRoute(
            trackPieces,
            toys.Select(toy => ((Vector2)toy.transform.position, toy.GetComponent<Toy>().Type)).ToList()
        );

        // Max time to show score should be about 2 seconds,
        // ideally about 0.3 seconds between each
        float delayPerScore = Mathf.Min(2.0f / coolnesses.Count, 0.3f);

        List<GameObject> adjacentToys = new List<GameObject>();

        for (int i = 0; i < coolnesses.Count; i++) {
            (TrackPiece piece, int value, int toyIndex) = coolnesses[i];

            TrackPieceController scoringTrackPiece = ToyMapManager.Instance.FindTrackPiece(piece);
            if (!scoringTrackPiece)
                continue;

            bool isBonus = toyIndex >= 0;
            var coolMessage = isBonus ? $"+{value}!!" : $"+{value}";

            float delay = delayPerScore * i;

            if (isBonus) {
                AudioClipOptions kerchingSFXOptions = new AudioClipOptions();
                kerchingSFXOptions.Delay = delay;
                kerchingSFXOptions.Volume = .3f;
                AudioManager.Instance.PlaySound(_kerchingSFX, kerchingSFXOptions);
            }

            WorldFloatingTextManager.Instance.Show(coolMessage, scoringTrackPiece.gameObject, delay);

            if (isBonus) {
                var toy = toys[toyIndex];

                if (!adjacentToys.Contains(toy)) {
                    adjacentToys.Add(toy);
                }
            }
        }

        adjacentToys.ForEach(toy => {
            var originalPosition = toy.transform.position;

            toy
                .transform
                .DOShakePosition(
                    duration: 1f,
                    strength: new Vector3(0.5f, 0.5f, 0),
                    vibrato: 10,
                    randomness: 90
                ).OnComplete(() => {
                    toy.transform.position = originalPosition;
                });
        });

        TrackPieceController endStation = ToyMapManager.Instance.FindTrackPiece(TerminatingStation);
        float finalDelay = delayPerScore * coolnesses.Count;
        PlayCoolSound(totalCool, finalDelay);
    }

    private void PlayCoolSound(int totalCool, float delay) {
        AudioClip clip;

        if (totalCool >= EXTREMELY_COOL_SCORE) {
            clip = _extremelyCoolSFX;
        } else if (totalCool >= VERY_COOL_SCORE) {
            clip = _veryCoolSFX;
        } else {
            clip = _coolSFX;
        }

        AudioClipOptions coolSFXOptions = new AudioClipOptions();
        coolSFXOptions.Delay = delay;
        coolSFXOptions.Volume = .6f;
        AudioManager.Instance.PlaySound(clip, coolSFXOptions);
    }

    private void RemovePiece() {
        AudioClipOptions removeSFXOptions = new AudioClipOptions();
        removeSFXOptions.Volume = .3f;
        AudioManager.Instance.PlaySound(_removeSFX, removeSFXOptions);

        if (PreviewTrackPieces.Count == 0) {
            StopEditing(false);
            Debug.Log("Route cancelled!");
            return;
        }

        (TrackPieceController controller, Compass direction) = PreviewTrackPieces[^1];

        BankManager.Instance.Reclaim(controller.TrackPiece.Template.Price);

        Destroy(controller.gameObject);
        PreviewTrackPieces.RemoveAt(PreviewTrackPieces.Count - 1);

        EditFromTrackPiece = PreviewTrackPieces.Count == 0 ? OriginStation : PreviewTrackPieces[^1].controller.TrackPiece;

        GhostTrackPiece.SetPosition(direction, EditFromTrackPiece);
    }
}