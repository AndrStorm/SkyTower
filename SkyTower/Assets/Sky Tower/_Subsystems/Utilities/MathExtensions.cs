using UnityEngine;
using Quaternion = UnityEngine.Quaternion;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;


public static class MathExtensions
{
    
    public static float RemapValue(this float x, float min, float max, float newMin, float newMax)
    {
        return newMin + (newMax - newMin) * (x - min) / (max - min);
    }

    private static float RemapUnityAngleTo360(this float angle)
    {
        return angle < 0 ? angle + 360 : angle;
    }

    
    
    public static float CalculateUnityAngle(this Vector2 vector)
    {
        float angle = Mathf.Atan2(vector.x, vector.y);
        angle *= Mathf.Rad2Deg;
        angle = -angle;
        return angle;
    }

    public static float CalculateMathAngle(this Vector2 vector)
    {
        float angleInRad = Mathf.Atan2(vector.x, vector.y);
        return angleInRad;
    }
    
    
    
    public static Quaternion CalculateWorldRotation
        (this Vector2 weaponDirection, Transform shipTransform)
    {
        Vector2 worldWeaponDirection = shipTransform.TransformDirection
            (weaponDirection.x, weaponDirection.y, 0);
        float angle = worldWeaponDirection.CalculateUnityAngle();
        Quaternion weaponRotation = Quaternion.Euler(0, 0, angle);
        return weaponRotation;
    }
    
    public static Vector2 RotateVectorDeg(this Vector2 vector, float angle)
    {
        angle *= Mathf.Deg2Rad;
        Vector2 rotatedVector = vector;
        rotatedVector.x = vector.x * Mathf.Cos(angle) - vector.y * Mathf.Sin(angle);
        rotatedVector.y = vector.x * Mathf.Sin(angle) + vector.y * Mathf.Cos(angle);
        return rotatedVector;
    }

    public static Vector2 NormolizeDirection(this Vector2 vector, float distance)
    {
        if (distance < 0.01f)
        {
            return new Vector2(0f, 0f);
        }
        else
        {
            return vector / distance;
        }
    }
    
    public static Vector2 CalculateOrbitTangent(this Vector2 targetDirection,
        float distanceToTarget, float OrbitDistance)
    {
        float minOrbitTangentAngle = Mathf.Asin(OrbitDistance / distanceToTarget);
        minOrbitTangentAngle *= Mathf.Rad2Deg;
        return targetDirection.RotateVectorDeg(minOrbitTangentAngle);
    }
    
    public static float CalculateScalarProjection(this Vector2 dirVector, Vector2 vector)
    {
        dirVector = dirVector.normalized;
        return CalculateScalarProjectionToDir(dirVector, vector);
    }
    
    public static float CalculateScalarProjectionToDir(this Vector2 direction, Vector2 vector)
    {
        return Vector2.Dot(direction, vector);
    }

    public static float CalculateDecelerationDistance(this float speed, float fixedDeceleration)
    {
        fixedDeceleration /= Time.fixedDeltaTime;
        float decelerationTime = speed / fixedDeceleration;
        return speed / 2f * decelerationTime;
    }

    public static float CalculateCircleLength(this float rad)
    {
        return 2f * Mathf.PI * rad;
    }
    
    
    public static Vector3 CalculateShipPointWorldPostion(this Vector2 weaponPosition,
        Transform shipTransform)
    {
        Vector3 shipPosition = shipTransform.position;
        Vector2 worldPos = shipTransform.TransformDirection(weaponPosition);
        worldPos.x += shipPosition.x;
        worldPos.y += shipPosition.y;
        return worldPos;
    }
    
    public static Vector3 CalculateBulletWorldPosition(this Vector2 weaponPosition,
        Vector2 weaponDirection, float barrelOffset, Transform shipTransform)
    {
        Vector3 worldPos = weaponPosition.CalculateShipPointWorldPostion(shipTransform);
        
        weaponDirection = shipTransform.TransformDirection(weaponDirection);
        float barrelAngle = weaponDirection.CalculateMathAngle();

        worldPos.x += Mathf.Sin(barrelAngle) * barrelOffset;
        worldPos.y += Mathf.Cos(barrelAngle) * barrelOffset; 
        return worldPos;
    }
    
    public static Vector2 CalculateWorldTargetShipOrigin(this Vector2 localOrigin,
        Vector2 worldShipDirection, Vector3 shipPosition)
    {
        float targetAngle = worldShipDirection.CalculateUnityAngle();
        
        Vector2 worldOrigin = localOrigin.RotateVectorDeg(targetAngle);
        worldOrigin.x += shipPosition.x;
        worldOrigin.y += shipPosition.y;

        return worldOrigin;
    }

    public static Vector3 CalculateBulletVelocityFromLocal(this Vector2 weaponLocalDirection,
        Transform shipTransform, Rigidbody2D shipRB, float bulletVelocity, bool isKinematic = false)
    {
        Vector3 worldWeaponDirection = shipTransform.TransformDirection(weaponLocalDirection);
        //worldWeaponDirection /= worldWeaponDirection.magnitude;

        return ((Vector2) worldWeaponDirection).CalculateBulletVelocityFromWorld
            (shipRB, bulletVelocity, isKinematic); //*added recently
        
        /*Vector3 bulletVelocityVector = worldWeaponDirection * bulletVelocity;

        if (isKinematic) return bulletVelocityVector;
        
        var shipVelocity = shipRB.velocity;
        bulletVelocityVector.x += shipVelocity.x;
        bulletVelocityVector.y += shipVelocity.y;
        return bulletVelocityVector;*/
    }
    
    public static Vector3 CalculateBulletVelocityFromWorld(this Vector2 weaponWorldDirection,
        Rigidbody2D shipRB, float bulletVelocity, bool isKinematic = false)
    {
        Vector3 worldWeaponDirection = weaponWorldDirection;
        //worldWeaponDirection /= worldWeaponDirection.magnitude;

        Vector3 bulletVelocityVector = worldWeaponDirection * bulletVelocity;
        
        if (isKinematic) return bulletVelocityVector; //*added recently
        
        var shipVelocity = shipRB.velocity;
        bulletVelocityVector.x += shipVelocity.x;
        bulletVelocityVector.y += shipVelocity.y;
        return bulletVelocityVector;
    }
    
    /*public static float CalculateReactiveBulletAverageVelocity
    (this Vector2 weaponDirection, Rigidbody2D shipRB, WeaponStats weaponStats, float distance)
    {
        Vector3 bulletVelocity = weaponDirection.
            CalculateBulletVelocityFromWorld(shipRB, weaponStats.velocity, weaponStats.isKinematic); //*isKin
        float startVelocity = bulletVelocity.magnitude;
                
        float averageVelocity = weaponStats.
            CalculateReactiveAverageVelocity(startVelocity, distance);
                
        return averageVelocity;
    }
    
    public static float CalculateReactiveAverageVelocity
        (this WeaponStats weaponStats, float startVelocity, float distance)
    {
        float maxVelocity = weaponStats.MaxVelocity;
                
        float deltaVelocity = maxVelocity - startVelocity;
        float accelerationTime = deltaVelocity / weaponStats.acceleration;
                
        float maxAccelerationDistance = (startVelocity + maxVelocity) * accelerationTime / 2f;
        if (maxAccelerationDistance < 0.0001f)
        {
            maxAccelerationDistance = 0.0001f;
        }

        float accelerationDistance = 
            maxAccelerationDistance < distance ? maxAccelerationDistance : distance;
        float maxVelocityDistance = distance - maxAccelerationDistance;
        if (maxVelocityDistance < 0.0001f)
        {
            maxVelocityDistance = 0.0001f;
        }
                
        float maxVelocityToTarget = 
            deltaVelocity * accelerationDistance / maxAccelerationDistance + startVelocity;

        float averageAccelerationVelocity =
            (startVelocity + maxVelocityToTarget) / 2f * accelerationDistance / distance;
        float averageVelocity =
            averageAccelerationVelocity + maxVelocityToTarget * maxVelocityDistance / distance;
                
        /*Debug.LogWarning($"d {distance}" +
                         $"Calculation !!! Vs {startVelocity} " +
                         $"maxVelocity {maxVelocity}" +
                         $"Vdif {deltaVelocity} ta {accelerationTime} " +
                         $"dma {maxAccelerationDistance} " +
                         $" d1 {accelerationDistance} " +
                         $" d2 {maxVelocityDistance} " +
                         $"Vend {maxVelocityToTarget} ~Va {averageAccelerationVelocity} " +
                         $" ~V {averageVelocity} ");#1#
                
        return averageVelocity;
    }*/
    
    /*
    public static float CalculateReactiveMaxVelocityToTarget
        (this WeaponStats weaponStats, float startVelocity, float distance)
    {
        float maxVelocity = weaponStats.MaxVelocity;
                
        float deltaVelocity = maxVelocity - startVelocity;
        float accelerationTime = deltaVelocity / weaponStats.acceleration;
                
        float maxAccelerationDistance = (startVelocity + maxVelocity) * accelerationTime / 2f;
        if (maxAccelerationDistance < 0.0001f)
        {
            maxAccelerationDistance = 0.0001f;
        }

        float accelerationDistance = 
            maxAccelerationDistance < distance ? maxAccelerationDistance : distance;
        
        float maxVelocityToTarget = 
            deltaVelocity * accelerationDistance / maxAccelerationDistance + startVelocity;

        
        /*Debug.LogWarning($"d {distance}" +
                         $"Calculation !!! Vs {startVelocity} " +
                         $"maxVelocity {maxVelocity}" +
                         $"Vdif {deltaVelocity} ta {accelerationTime} " +
                         $"dma {maxAccelerationDistance} " +
                         $" d1 {accelerationDistance} " +
                         $" d2 {maxVelocityDistance} " +
                         $"Vend {maxVelocityToTarget} ~Va {averageAccelerationVelocity} " +
                         $" ~V {averageVelocity} ");#1#
                
        return maxVelocityToTarget;
    }
    */
    
    
    
    public static bool IsAnglesEqual(this float angle1, float angle2, float accuaracy = 0.001f)
    {
        return Mathf.Abs(Mathf.DeltaAngle(angle1,angle2)) < accuaracy;
    }
    
    public static bool IsFloatsEqual(this float val1, float val2, float accuaracy = 0.001f)
    {
        return Mathf.Abs(val1 - val2) < accuaracy;
    }
    
    public static bool IsVectorsEqual(this Vector2 vec1, Vector2 vec2, float accuaracy = 0.001f)
    {
        return vec1.x.IsFloatsEqual(vec2.x, accuaracy) && vec1.y.IsFloatsEqual(vec2.y, accuaracy);
    }

    public static bool IsVectorMoreThan(this Vector2 vector, float length)
    {
        return vector.x * vector.x + vector.y * vector.y > length * length;
    }
    
    public static bool IsVectorLengthtZero(this Vector2 vector, float accuaracy = 0.001f)
    {
        return vector.IsVectorsEqual(new Vector2(0f, 0f), accuaracy);
        //return vector.x * vector.x + vector.y * vector.y < accuaracy;
    }
    
    public static bool IsBigger(this Vector2 vec1, Vector2 vec2)
    {
        float vec1Length = vec1.x * vec1.x + vec1.y * vec1.y;
        float vec2Length = vec2.x * vec2.x + vec2.y * vec2.y;

        return vec1Length > vec2Length;
    }
    
    public static bool IsInFront(this Vector2 frontVector, Vector2 vector)
    {
        float frontDot = Vector2.Dot(frontVector, vector);
        return frontDot > 0;
    }

    
    
    public static bool IsShipTurned(this Vector2 turnDirection, Transform ship, float turnDot)
    {
        float dotTurn = turnDirection.DotShipFront(ship);
        return dotTurn > turnDot;
    }
    
    public static float DotShipFront(this Vector2 turnDirection, Transform ship)
    {
        //turnDirection = turnDirection.normalized;
        Vector2 shipDirection = ship.TransformDirection(0, 1, 0);
        float dotTurn = Vector2.Dot(shipDirection, turnDirection);
        return dotTurn;
    }
}
