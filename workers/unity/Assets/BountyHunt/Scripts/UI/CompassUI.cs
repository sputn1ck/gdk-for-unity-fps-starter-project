using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Fps.Movement;
using System.Linq;
using Improbable.Gdk.Core;

public class CompassUI : MonoBehaviour
{
    public Transform MarkerContainer;
    public Image MarkerPrefab;
    public TextMeshProUGUI CardinalPointPrefab;
    public int ViewAngle;
    int halfViewAngle;
    public float minScaleDist;
    public float maxScaleDist;
    public int playersToTrack;
    public BountyPickUpMaterialSettings materialSettings;

    float delataScaleDist;

    float minScale = 0.3f;
    float maxScale = 1f;


    Camera cam;
    Vector2 camDirection;

    Dictionary<EntityId,TrackObject> _objectsToTrack = new Dictionary<EntityId, TrackObject>();
    List<TrackObject> _trackObjectPool = new List<TrackObject>();
    List<CardinalPoint> _cardinalPoints = new List<CardinalPoint>();

    private void Awake()
    {
        ClientEvents.instance.onPlayerSpawn.AddListener((GameObject o) => cam = FpsDriver.instance.camera) ;
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

        ClientEvents.instance.onScoreboardUpdate.AddListener(OnScoreBoardUpdate);
    }

    private void Update()
    {
        //cam = FpsDriver.instance.camera;
        if (cam == null) return;

        camDirection = new Vector2(cam.transform.forward.x, cam.transform.forward.z);

        foreach(CardinalPoint cp in _cardinalPoints)
        {
            UpdateCardinalPoint(cp);
        }

        
        foreach(var trob in _objectsToTrack)
        {
            UpdateTrackObject(trob.Value);
        }
    }

    void OnScoreBoardUpdate(List<ScoreboardUIItem> items,EntityId playerId)
    {
        Dictionary<EntityId, TrackObject> tempDict = new Dictionary<EntityId, TrackObject>(_objectsToTrack);
        
        items = items.OrderByDescending(i => i.item.Bounty).ToList();
        for(int i = 0; i<items.Count&& i < playersToTrack;i++)
        {
            EntityId id = new EntityId(items[i].item.Entity.Id);

            if (!ClientGameObjectManager.Instance.BountyTracers.ContainsKey(id)||id==playerId||items[i].item.Bounty<1) continue;

            GameObject tracer = ClientGameObjectManager.Instance.BountyTracers[id];

            if (tracer == null)
            {
                continue;
            }

            else if (tempDict.ContainsKey(id))
            {
                tempDict[id].marker.GetComponent<Image>().color = materialSettings.getColorByValue(items[i].item.Bounty);
                UpdateTrackObject(tempDict[id]);
                tempDict.Remove(id);
            }
            else
            {

                var tO = AddNewTrackObject(id);
                tO.marker.GetComponent<Image>().color = materialSettings.getColorByValue(items[i].item.Bounty);
                UpdateTrackObject(tO);
            }
        }
        foreach (var trob in tempDict)
        {
            RemoveTrackObject(trob);
        }
    }

    void RemoveTrackObject(KeyValuePair<EntityId,TrackObject> trob)
    {
        trob.Value.marker.gameObject.SetActive(false);
        _trackObjectPool.Add(trob.Value);
        _objectsToTrack.Remove(trob.Key);
    }

    TrackObject AddNewTrackObject(EntityId id)
    {
        TrackObject trackObj;
        if (_trackObjectPool.Count > 0)
        {
            trackObj = _trackObjectPool[0];
            _trackObjectPool.RemoveAt(0);
        }
        else
        {
            var marker = Instantiate(MarkerPrefab, MarkerContainer);
            trackObj = new TrackObject { marker = marker.GetComponent<RectTransform>()};
        }
        trackObj.marker.gameObject.SetActive(true);
        trackObj.gameObject = ClientGameObjectManager.Instance.GetBountyTracer(id);
        _objectsToTrack[id] = trackObj;
        return trackObj;
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

        pos = Mathf.SmoothStep(0, 1, pos);


        float size = Mathf.Clamp01((distance - minScaleDist) / delataScaleDist);
        size = Mathf.Lerp(minScale, maxScale, 1-size);

        t.anchorMin = new Vector2(pos, 0.5f);
        t.anchorMax = t.anchorMin;
        t.localScale = Vector3.one * size;
    }


    class TrackObject
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

