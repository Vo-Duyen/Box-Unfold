using System.Globalization;
using DG.Tweening;
using UnityEngine;

namespace LongNC.Cube
{
    public enum Direction
    {
        Up,
        Down,
        Left,
        Right,
    }
    
    public interface IMovement
    {
        Direction GetDirection(Vector3 posClickDown, Vector3 posClickUp);
        void Move(Transform target, ref Vector3 rotate, Direction direction, float distance, float timeMove);
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
        
        public void Move(Transform target, ref Vector3 rotate, Direction direction, float distance, float timeMove)
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

            var newRotate = Vector3.zero;
            newRotate.x = Mathf.RoundToInt(rotateAxis.x);
            newRotate.y = Mathf.RoundToInt(rotateAxis.y);
            newRotate.z = Mathf.RoundToInt(rotateAxis.z);
            rotate = newRotate;
            DOVirtual.Float(0, 1, timeMove, param =>
            {
                target.transform.RotateAround(rotateCenter, rotateAxis, 180 * Time.deltaTime);
            }).OnComplete(() =>
            {
                target.rotation = Quaternion.Euler(newRotate);
            });
            target.transform.DOLocalMove(curPosition, timeMove);
        }
    }
}