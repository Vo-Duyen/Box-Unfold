using System.Collections.Generic;
using System.Globalization;
using DG.Tweening;
using Unity.VisualScripting;
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
        void Move(Transform target, Direction direction, float distance, float timeMove);
    }

    public class Movement : IMovement
    {
        public Direction GetDirection(Vector3 posClickDown, Vector3 posClickUp)
        {
            var distance = posClickUp - posClickDown;
            if (Mathf.Abs(distance.x) > Mathf.Abs(distance.y))
            {
                return distance.x > 0 ? Direction.Right : Direction.Left;
            }
            else
            {
                return distance.y > 0 ? Direction.Up : Direction.Down;
            }
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
                    var d when d == Vector3.left => Direction.Left,
                    var d when d == Vector3.right => Direction.Right,
                    var d when d == Vector3.up => Direction.Up,
                    var d when d == Vector3.down => Direction.Down,
                    _ => Direction.None
                };

                if (childRes != Direction.None)
                {
                    res.Add(childRes);
                }
            }

            return res.ToArray();
        }

        public void Move(Transform target, Direction direction, float distance, float timeMove)
        {
            var curPosition = target.transform.localPosition;
            var direct = Vector3.zero;
            var rotateAxis = Vector3.zero;
            switch (direction)
            {
                case Direction.Up:
                    curPosition.y += distance;
                    direct = Vector3.forward;
                    rotateAxis = Vector3.up;
                    break;
                case Direction.Down:
                    curPosition.y -= distance;
                    direct = Vector3.back;
                    rotateAxis = Vector3.up;
                    break;
                case Direction.Left:
                    curPosition.x -= distance;
                    direct = Vector3.right;
                    rotateAxis = Vector3.forward;
                    break;
                case Direction.Right:
                    curPosition.x += distance;
                    direct = Vector3.left;
                    rotateAxis = Vector3.forward;
                    break;
            }

            var rotateCenter = target.position + direct / 2 + Vector3.down / 2;
            rotateAxis = Vector3.Cross(rotateAxis, direct);

            DOVirtual.Float(0, 1, timeMove,
                    param =>
                    {
                        target.transform.RotateAround(rotateCenter, rotateAxis, 90 / timeMove * Time.deltaTime);
                    })
                .OnComplete(() =>
                {
                    var curRotate = target.transform.localRotation.eulerAngles;
                    curRotate.x = (curRotate.x > 0 ? 1 : -1) * (Mathf.Abs(curRotate.x) > 5
                        ? ((int)(Mathf.Abs(curRotate.x) + 5) / 10) * 10
                        : ((int)Mathf.Abs(curRotate.x) / 10) * 10);
                    curRotate.y = (curRotate.y > 0 ? 1 : -1) * (Mathf.Abs(curRotate.y) > 5
                        ? ((int)(Mathf.Abs(curRotate.y) + 5) / 10) * 10
                        : ((int)Mathf.Abs(curRotate.y) / 10) * 10);
                    curRotate.z = (curRotate.z > 0 ? 1 : -1) * (Mathf.Abs(curRotate.z) > 5
                        ? ((int)(Mathf.Abs(curRotate.z) + 5) / 10) * 10
                        : ((int)Mathf.Abs(curRotate.z) / 10) * 10);
                    target.transform.localRotation = Quaternion.Euler(curRotate);
                });
            target.transform.DOLocalMove(curPosition, timeMove).SetEase(Ease.Linear);
        }
    }
}