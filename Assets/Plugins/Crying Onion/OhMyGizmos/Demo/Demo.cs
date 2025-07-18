using CryingOnion.Tools.Runtime;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace CryingOnion.DemoTool
{
    public class Demo : MonoBehaviour
    {
        [SerializeField, GradientUsage(true)] Gradient cubeGradient;
        [SerializeField] Transform cube;

        [Space(5)]
        [SerializeField, GradientUsage(true)] Gradient sphereGradient;
        [SerializeField] Transform sphere;

        [Space(5)]
        [SerializeField, GradientUsage(true)] Gradient circleGradient;
        [SerializeField] Transform circle;
        [SerializeField] Transform circle2;

        [Space(5)]
        [SerializeField, GradientUsage(true)] Gradient lineGradient;
        [SerializeField] List<Transform> points;

        [Space(5)]
        [SerializeField] Sprite icon;
        [SerializeField, GradientUsage(true)] Gradient iconGradient;
        [SerializeField] List<Transform> iconsTrans;

        [Space(5)]
        [SerializeField, GradientUsage(true)] Gradient arrowGradient;
        [SerializeField] Transform arrowTrans;
        [SerializeField, Min(0)] float arrowSize = 1;
        [SerializeField, Min(0)] float arrowLenght = 1;

        [Space(5)]
        [SerializeField, GradientUsage(true)] Gradient capsuleGradient;
        [SerializeField] Transform capsuleTrans;
        [SerializeField, Min(0)] float capHeight = 1.0f;
        [SerializeField, Min(0)] float capRadius = 0.5f;

        [Space(5)]
        [SerializeField, GradientUsage(true)] Gradient cylinderGradient;
        [SerializeField] Transform cylinderTrans;
        [SerializeField, Min(0)] float cyHeight = 2.0f;
        [SerializeField, Min(0)] float cyRadius = 0.5f;

        [Space(5)]
        [Header("Custom drawer parameters")]
        [SerializeField] private Mesh[] meshes;
        [SerializeField] private Transform[] transforms;
        [SerializeField] private Gradient customGradient;

        List<Guid> guids;

        float pingPongLerp = 0;

        // All shapes (with the exception of spheres and cubes) are procedural,
        // so they are modifiable mesh within a dictionary for each type of request (lines, capsules, etc... have their own request dictionaries),
        // in order to identify them and Modify the mesh correctly using GUID as identification key.
        Guid circle1Id = Guid.NewGuid();
        Guid circle2Id = Guid.NewGuid();
        Guid lineId = Guid.NewGuid();
        Guid arrowId = Guid.NewGuid();
        Guid capsuleId = Guid.NewGuid();
        Guid cylinderId = Guid.NewGuid();

        private void OnGUI()
        {
            string label = OhMyGizmos.Enabled ? "ON" : "OFF";
            GUILayout.FlexibleSpace();
            if (GUILayout.Button($"OhMyGizmos: {label}"))
                OhMyGizmos.Enabled = !OhMyGizmos.Enabled;
        }

        private void LateUpdate()
        {
            Camera.main.transform.RotateAround(Vector3.zero, Vector3.up, 30 * Time.deltaTime);

            pingPongLerp = Mathf.PingPong(Time.time, 1);

            cube.RotateAround(cube.position, Vector3.up, Time.deltaTime * 20);
            arrowTrans.RotateAround(cube.position, Vector3.down, Time.deltaTime * 20);
            arrowTrans.LookAt(cube.position);

            capsuleTrans.RotateAround(capsuleTrans.position, Vector3.up, Time.deltaTime * 20);
            cylinderTrans.RotateAround(cylinderTrans.position, Vector3.down, Time.deltaTime * 20);

            DrawUpdate();
        }

        private void DrawUpdate()
        {
            /*
                When draw requests are made from the OhMyGizmos class,
                it makes requests to drawers that it created internally,
                these only draw when OhMyGizmos.Enable is true,
                otherwise those requests are cleaned up to free up resources.
            */

            if (!OhMyGizmos.Enabled) return;

            OhMyGizmos.Cube(Matrix4x4.TRS(cube.position, cube.rotation, Vector3.one * 1.2f * pingPongLerp), cubeGradient.Evaluate(pingPongLerp));
            OhMyGizmos.Sphere(sphere.position, 1.2f * (1 - pingPongLerp), sphereGradient.Evaluate(pingPongLerp));

            float angleCircle = Vector3.SignedAngle(circle.forward, Vector3.forward, Vector3.up);
            OhMyGizmos.Circle(circle1Id, circle.position, circle.rotation, circleGradient.Evaluate(Mathf.Abs(angleCircle / 180)), 1.5f, angleCircle);
            OhMyGizmos.Circle(circle2Id, circle2.position, Quaternion.identity, circleGradient.Evaluate(1 - pingPongLerp), 2, pingPongLerp * 360);

            var pointsv3 = points.ConvertAll(point => point.position);

            for (int i = 0; i < pointsv3.Count; i++)
            {
                pointsv3[i] = pointsv3[i] + Vector3.forward * Mathf.Sin(Mathf.PI * (pointsv3[i].x + Time.time));
                OhMyGizmos.Cube(Matrix4x4.TRS(pointsv3[i], Quaternion.identity, Vector3.one * .25f), lineGradient.Evaluate((float)i / (pointsv3.Count - 1)));
            }

            OhMyGizmos.Lines(lineId, pointsv3, lineGradient);

            for (int j = 0; j < iconsTrans.Count; j++)
            {
                Matrix4x4 matrix = Matrix4x4.TRS(iconsTrans[j].position + Vector3.up * Mathf.Sin(Mathf.PI * (iconsTrans[j].position.x + Time.time)), iconsTrans[j].rotation, iconsTrans[j].localScale);
                OhMyGizmos.Icon(matrix, iconGradient.Evaluate((float)j / (iconsTrans.Count - 1)), icon.texture);
                OhMyGizmos.Quad(matrix, iconGradient.Evaluate((float)j / (iconsTrans.Count - 1)));
            }

            OhMyGizmos.Capsule(capsuleId, capsuleTrans.localToWorldMatrix, capsuleGradient.Evaluate(0), capHeight, capRadius);
            float capsuleAngle = Vector3.SignedAngle(Vector3.forward, capsuleTrans.forward, capsuleTrans.up);
            OhMyGizmos.Circle(capsuleId, capsuleTrans.position, Quaternion.identity, capsuleGradient.Evaluate(Mathf.Abs(capsuleAngle / 180)), arrowLenght * 2, capsuleAngle);
            OhMyGizmos.Arrow(capsuleId, capsuleTrans.position, capsuleTrans.forward, arrowSize, arrowLenght, capsuleGradient);

            OhMyGizmos.Cylinder(cylinderId, cylinderTrans.localToWorldMatrix, cylinderGradient.Evaluate(0), cyHeight, cyRadius);
            OhMyGizmos.Arrow(cylinderId, cylinderTrans.position, cylinderTrans.forward, arrowSize, arrowLenght, cylinderGradient);

            OhMyGizmos.Cube(Matrix4x4.TRS(arrowTrans.position, arrowTrans.rotation, Vector3.one * arrowSize * 0.25f), arrowGradient.Evaluate(0));
            OhMyGizmos.Arrow(arrowId, arrowTrans.position, arrowTrans.forward, arrowSize, arrowLenght, arrowGradient);

            for (int i = 0; i < transforms.Length; i++)
            {
                transforms[i].RotateAround(transforms[i].position, Vector3.down, Time.deltaTime * 90);

                if (guids == null)
                    guids = new List<Guid>();

                if (guids.Count < transforms.Length)
                    guids.Add(Guid.NewGuid());

                OhMyGizmos.Mesh(guids[i],
                    transforms[i].localToWorldMatrix,
                    customGradient.Evaluate((float)i / (transforms.Length - 1)),
                    meshes[(i + 1) % meshes.Length]);
            }
        }
    }
}