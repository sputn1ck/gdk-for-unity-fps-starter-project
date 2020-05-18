using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class CompassUI : MonoBehaviour
{
    public Transform MarkerContainer;
    public Image MarkerPrefab;
    public TextMeshProUGUI CardinalPointPrefab;
    public Sprite HunterMarkerSprite;
    public Sprite LootMarkerSprite;
    public int ViewAngle;
    int halfViewAngle;
    public float minScaleDist;
    public float maxScaleDist;
    float delataScaleDist;

    float minScale = 0.3f;


    Camera cam;
    Vector2 camDirection;

    List<TrackObject> _objectsToTrack = new List<TrackObject>();
    List<TrackObject> _trackObjectPool = new List<TrackObject>();
    List<CardinalPoint> _cardinalPoints = new List<CardinalPoint>();

    private void Awake()
    {
        cam = Camera.main;
        delataScaleDist = maxScaleDist - minScaleDist;
        halfViewAngle = ViewAngle / 2;
    }

    private void Start()
    {
        var n = Instantiate(CardinalPointPrefab, MarkerContainer); ;
        var s = Instantiate(CardinalPointPrefab, MarkerContainer); ;
        var e = Instantiate(CardinalPointPrefab, MarkerContainer); ;
        var w = Instantiate(CardinalPointPrefab, MarkerContainer); ;

        n.text = "N";
        s.text = "S";
        e.text = "E";
        w.text = "W";

        _cardinalPoints.Add(new CardinalPoint {marker = n.transform as RectTransform, direction = Vector2.up });
        _cardinalPoints.Add(new CardinalPoint {marker = s.transform as RectTransform, direction = Vector2.down });
        _cardinalPoints.Add(new CardinalPoint {marker = e.transform as RectTransform, direction = Vector2.right });
        _cardinalPoints.Add(new CardinalPoint {marker = w.transform as RectTransform, direction = Vector2.left });
    }

    private void Update()
    {
        camDirection = new Vector2(cam.transform.forward.x, cam.transform.forward.z);

        foreach(CardinalPoint cp in _cardinalPoints)
        {
            UpdateCardinalPoint(cp);
        }

    }

    void UpdateTrackObject(TrackObject trackObject)
    {
        Vector3 objDir3 = trackObject.gameObject.transform.position - cam.transform.position;
        Vector2 objDir2 = new Vector2(objDir3.x, objDir3.z);
        float angle = Vector2.SignedAngle(objDir2, camDirection);
        UpdateMarker(trackObject.marker,objDir2.magnitude,angle);
    }

    void UpdateCardinalPoint(CardinalPoint point)
    {
        float angle = Vector2.SignedAngle(point.direction, camDirection);
        UpdateMarker(point.marker, 0, angle);
    }

    void UpdateMarker(RectTransform t, float distance, float angle)
    {
        float pos = (angle + halfViewAngle)/ViewAngle;
        pos = Mathf.Clamp01(pos);
        pos = Mathf.SmoothStep(pos, 0, 1);
        float size = Mathf.Clamp01((distance - minScaleDist) / delataScaleDist);
        size = Mathf.Lerp(minScale, 1, size);

        t.anchorMin = new Vector2(pos, 0.5f);
        t.anchorMax = t.anchorMin;
        t.localScale = Vector3.one * size;

    }


    struct TrackObject
    {
        public GameObject gameObject;
        public RectTransform marker;
    }
    struct CardinalPoint
    {
        public Vector2 direction;
        public RectTransform marker;
    }

}

