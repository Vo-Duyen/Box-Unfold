using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using LongNC.Manager;
using UnityEngine;

namespace LongNC.Cube
{
    public enum Direction
    {
        Up,
        Down,
        Left,
        Right,
        None,
    }

    public interface IMovement
    {
        Direction GetDirection(Vector3 posClickDown, Vector3 posClickUp);
        Direction[] GetDirections(Transform target);
        void MoveChild(Transform cube, Transform childCube, Vector3 rotationAxis, float distance, float timeMove);

        void MoveParent(Transform transform, Direction direction, float distance, float timeMove);
        void MoveAll(Transform target, float distance, float timeMove);
        void MoveReverseParent(Transform trans, Transform transReverseCore, Collider collider);
    }

    public class Movement : IMovement
    {
        private class DataReverse
        {
            public Vector3 directionValueReverse;
            public float distance;
            public float timeMove;
            public List<(Transform trans, Vector3 rotationAxis)> cubeChilds = new List<(Transform, Vector3)>();
        }
        private Stack<DataReverse> _historyMove = new Stack<DataReverse>();

        public Direction GetDirection(Vector3 posClickDown, Vector3 posClickUp)
        {
            var distance = posClickUp - posClickDown;
            if (Mathf.Abs(distance.x) > Mathf.Abs(distance.y))
            {
                return distance.x > 0 ? Direction.Right : Direction.Left;
            }

            return distance.y > 0 ? Direction.Up : Direction.Down;
        }

        public Direction[] GetDirections(Transform target)
        {
            var res = new List<Direction>();
            for (var i = 0; i < target.childCount; ++i)
            {
                var direct = target.GetChild(i).position - target.position;
                direct = direct.normalized;
                if (Mathf.Abs(direct.x) > 0.7f) direct.x = direct.x > 0 ? 1 : -1;
                else direct.x = 0;
                if (Mathf.Abs(direct.y) > 0.7f) direct.y = direct.y > 0 ? 1 : -1;
                else direct.y = 0;
                if (Mathf.Abs(direct.z) > 0.7f) direct.z = direct.z > 0 ? 1 : -1;
                else direct.z = 0;

                if (direct == Vector3.back)
                {
                    return null;
                }

                var childRes = direct switch
                {
                    _ when direct == Vector3.left => Direction.Left,
                    _ when direct == Vector3.right => Direction.Right,
                    _ when direct == Vector3.up => Direction.Up,
                    _ when direct == Vector3.down => Direction.Down,
                    _ => Direction.None
                };

                if (childRes != Direction.None)
                {
                    res.Add(childRes);
                }
            }

            return res.ToArray();
        }

        public void MoveChild(Transform cube, Transform childCube, Vector3 rotationAxis, float distance, float timeMove)
        {
            var previousAngle = 0f;
            DOVirtual.Float(0, 90f, timeMove, currentAngle =>
                {
                    var deltaAngle = currentAngle - previousAngle;
                    childCube.RotateAround(cube.position, rotationAxis, deltaAngle);

                    previousAngle = currentAngle;
                })
                .SetEase(Ease.Linear)
                .OnComplete(() =>
                {
                    var eulerAngles = childCube.localRotation.eulerAngles;
                    eulerAngles.x = RoundToNearest10(eulerAngles.x);
                    eulerAngles.y = RoundToNearest10(eulerAngles.y);
                    eulerAngles.z = RoundToNearest10(eulerAngles.z);
                    childCube.localRotation = Quaternion.Euler(eulerAngles);

                    var pos = childCube.localPosition;
                    if (Mathf.Approximately(Mathf.Abs(pos.x), 0.0048f))
                    {
                        pos.x = 0.0048f * (pos.x > 0 ? 1 : -1);
                    }
                    else
                    {
                        pos.x = 0;
                    }
                    if (Mathf.Approximately(Mathf.Abs(pos.y), 0.0048f))
                    {
                        pos.y = 0.0048f * (pos.y > 0 ? 1 : -1);
                    }
                    else
                    {
                        pos.y = 0;
                    }
                    if (Mathf.Approximately(Mathf.Abs(pos.z), 0.0048f))
                    {
                        pos.z = 0.0048f * (pos.z > 0 ? 1 : -1);
                    }
                    else
                    {
                        pos.z = 0;
                    }

                    childCube.localPosition = pos;
                });
        }

        public void MoveParent(Transform target, Direction direction, float distance, float timeMove)
        {
            SoundManager.Instance.PlayFX(SoundId.Move);
            var directionValue = direction switch
            {
                Direction.Left => Vector3.left,
                Direction.Right => Vector3.right,
                Direction.Up => Vector3.up,
                Direction.Down => Vector3.down,
                _ => Vector3.zero
            };
            target.DOMove(target.position + directionValue, timeMove).SetEase(Ease.Linear);
            
            var direct = Vector3.zero;
            var rotateAxis = Vector3.zero;

            switch (direction)
            {
                case Direction.Up:
                    direct = Vector3.forward;
                    rotateAxis = Vector3.up;
                    break;
                case Direction.Down:
                    direct = Vector3.back;
                    rotateAxis = Vector3.up;
                    break;
                case Direction.Left:
                    direct = Vector3.right;
                    rotateAxis = Vector3.forward;
                    break;
                case Direction.Right:
                    direct = Vector3.left;
                    rotateAxis = Vector3.forward;
                    break;
            }
            
            var rotationAxis = Vector3.Cross(rotateAxis, direct);
            var rotationAxisReverse = Vector3.Cross(rotateAxis, direct * -1);

            var arrReversed = new DataReverse()
            {
                directionValueReverse = directionValue * -1,
                timeMove = timeMove,
                distance = distance,
            };
            for (var i = 0; i < target.transform.childCount; ++i)
            {
                var childTrans = target.transform.GetChild(i);
                MoveChild(target, childTrans, rotationAxis, distance, timeMove);
                arrReversed.cubeChilds.Add((childTrans, rotationAxisReverse));
            }
            _historyMove.Push(arrReversed);
        }

        public void MoveAll(Transform target, float distance, float timeMove)
        {
            var arr = new List<Transform>();
            foreach (Transform trans in target.transform)
            {
                arr.Add(trans);
            }
            var arrReversed = new DataReverse()
            {
                directionValueReverse = Vector3.zero,
                timeMove = timeMove,
                distance = distance,
            };
            foreach (var child in arr)
            {
                var direct = child.position - target.transform.position;
                direct = direct.normalized;
                if (Mathf.Abs(direct.x) > 0.7f) direct.x = direct.x > 0 ? 1 : -1;
                else direct.x = 0;
                if (Mathf.Abs(direct.y) > 0.7f) direct.y = direct.y > 0 ? 1 : -1;
                else direct.y = 0;
                if (Mathf.Abs(direct.z) > 0.7f) direct.z = direct.z > 0 ? 1 : -1;
                else direct.z = 0;
                if (direct == Vector3.back)
                {
                    continue;
                }
                var childRes = direct switch
                {
                    _ when direct == Vector3.left => Direction.Left,
                    _ when direct == Vector3.right => Direction.Right,
                    _ when direct == Vector3.up => Direction.Up,
                    _ when direct == Vector3.down => Direction.Down,
                    _ => Direction.None
                };
                var nDirect = Vector3.zero;
                var rotateAxis = Vector3.zero;

                switch (childRes)
                {
                    case Direction.Up:
                        nDirect = Vector3.forward;
                        rotateAxis = Vector3.up;
                        break;
                    case Direction.Down:
                        nDirect = Vector3.back;
                        rotateAxis = Vector3.up;
                        break;
                    case Direction.Left:
                        nDirect = Vector3.right;
                        rotateAxis = Vector3.forward;
                        break;
                    case Direction.Right:
                        nDirect = Vector3.left;
                        rotateAxis = Vector3.forward;
                        break;
                }
            
                var rotationAxis = Vector3.Cross(rotateAxis, nDirect);

                var newTarget = new GameObject()
                {
                    transform =
                    {
                        position = target.position,
                        parent = target.parent,
                        localScale = Vector3.one * 100f,
                    }
                };
                child.transform.SetParent(newTarget.transform);
                newTarget.transform.DOMove(newTarget.transform.position + direct, timeMove).SetEase(Ease.Linear);
                MoveChild(newTarget.transform, child, rotationAxis, distance, timeMove);
                arrReversed.cubeChilds.Add((child, Vector3.Cross(rotateAxis, nDirect * -1)));
            }
            _historyMove.Push(arrReversed);
        }

        public void MoveReverseParent(Transform trans, Transform transReverseCore, Collider collider)
        {
            if (_historyMove.Count == 0) return;
            collider.enabled = false;
            var sequence = DOTween.Sequence();
            var totalTime = 0f;
            var pos = trans.position;
            var timeDelay = _historyMove.Count * 0.7f;
            while (_historyMove.Count > 0)
            {
                var dataReverse = _historyMove.Pop();
                var timeMove = dataReverse.timeMove;
                pos += dataReverse.directionValueReverse;

                sequence.AppendInterval(totalTime);
                sequence.Append(trans.DOMove(pos, timeMove).SetEase(Ease.Linear));

                foreach (var data in dataReverse.cubeChilds)
                {
                    sequence.JoinCallback(() =>
                    {
                        data.trans.SetParent(trans);
                        MoveChild(trans, data.trans, data.rotationAxis, dataReverse.distance,
                            dataReverse.timeMove);
                    });
                }

                totalTime += timeMove;
            }

            sequence?.Play();
            DOVirtual.DelayedCall(timeDelay, () =>
            {
                transReverseCore.SetParent(trans);
                collider.enabled = true;
            });
        }

        private float RoundToNearest10(float angle)
        {
            angle = angle % 360f;
            if (angle < 0) angle += 360f;

            return Mathf.Round(angle / 10f) * 10f;
        }
    }
}