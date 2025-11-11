using System.Collections.Generic;
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
            // Sound
            SoundManager.Instance.PlayFX(SoundId.Move);
            
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

            var previousAngle = 0f;
            
            DOVirtual.Float(0, 90f, timeMove, currentAngle =>
            {
                var deltaAngle = currentAngle - previousAngle;
                
                target.RotateAround(rotateCenter, rotateAxis, deltaAngle);
                
                previousAngle = currentAngle;
            })
            .SetEase(Ease.Linear)
            .OnComplete(() =>
            {
                var curRotate = target.transform.localRotation.eulerAngles;
                curRotate.x = RoundToNearest10(curRotate.x);
                curRotate.y = RoundToNearest10(curRotate.y);
                curRotate.z = RoundToNearest10(curRotate.z);
                target.transform.localRotation = Quaternion.Euler(curRotate);
                
                target.transform.localPosition = curPosition;
            });

            target.transform.DOLocalMove(curPosition, timeMove).SetEase(Ease.Linear);
        }

        private float RoundToNearest10(float angle)
        {
            angle = angle % 360f;
            if (angle < 0) angle += 360f;
            
            return Mathf.Round(angle / 10f) * 10f;
        }
    }
}