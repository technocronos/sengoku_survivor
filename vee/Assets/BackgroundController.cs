using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class BackgroundController : MonoBehaviour
{
    [SerializeField]
    private Transform[] backgrounds;
    [SerializeField]
    private Transform playerContainer;

    private Transform currentToMove;
    private Queue<Transform> backgroundQueue = new Queue<Transform>();
    private float space = 20f;

    private void Awake()
    {
        for (int i = 0; i < backgrounds.Length; i++)
        {
            var pos = backgrounds[i].position;
            pos.y = playerContainer.position.y + space * (i - 1);
            backgrounds[i].position = pos;
            backgroundQueue.Enqueue(backgrounds[i]);
        }
        currentToMove = backgroundQueue.Dequeue();
    }

    private void Update()
    {
        var pos = currentToMove.position;
        var distance = playerContainer.transform.position.y - pos.y;
        if (distance > space * 1.5f)
        {
            var nextToMove = backgroundQueue.Dequeue();
            pos.y = nextToMove.position.y + 2 * space;
            currentToMove.position = pos;
            backgroundQueue.Enqueue(currentToMove);
            currentToMove = nextToMove;
        }
    }
}
