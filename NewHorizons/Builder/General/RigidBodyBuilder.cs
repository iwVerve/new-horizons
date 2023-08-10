using NewHorizons.External;
using NewHorizons.External.Configs;
using NewHorizons.Utility;
using NewHorizons.Utility.OWML;
using UnityEngine;
namespace NewHorizons.Builder.General
{
    public static class RigidBodyBuilder
    {
        public static OWRigidbody Make(GameObject body, float sphereOfInfluence, PlanetConfig config)
        {
            body.AddComponent<ProxyShadowCasterSuperGroup>()._bounds.radius = sphereOfInfluence;

            Rigidbody rigidBody = body.AddComponent<Rigidbody>();
            rigidBody.drag = 0f;
            rigidBody.angularDrag = 0f;
            rigidBody.useGravity = false;
            rigidBody.isKinematic = true;
            rigidBody.interpolation = RigidbodyInterpolation.None;
            rigidBody.collisionDetectionMode = CollisionDetectionMode.Discrete;

            OWRigidbody owRigidBody = body.AddComponent<OWRigidbody>();
            owRigidBody._autoGenerateCenterOfMass = true;
            owRigidBody.SetIsTargetable(true);
            owRigidBody._maintainOriginalCenterOfMass = true;
            owRigidBody._rigidbody = rigidBody;
            owRigidBody._origParent = SearchUtilities.Find("SolarSystemRoot")?.transform;

            KinematicRigidbody kinematicRigidBody = body.AddComponent<KinematicRigidbody>();
            owRigidBody._kinematicRigidbody = kinematicRigidBody;
            owRigidBody._kinematicSimulation = true;
            owRigidBody.MakeKinematic();
            owRigidBody.EnableKinematicSimulation();
            rigidBody.mass = 10000;

            if (config.Base.addPhysics)
            {
                // hack: make all mesh colliders convex
                // triggers are already convex
                // prints errors for non readable meshes but whatever
                foreach (var meshCollider in body.GetComponentsInChildren<MeshCollider>(true))
                    meshCollider.convex = true;

                var shape = body.AddComponent<SphereShape>();
                shape._collisionMode = Shape.CollisionMode.Detector;
                shape._layerMask = (int)(Shape.Layer.Default | Shape.Layer.Gravity);
                shape._radius = config.Base.surfaceSize;

                var impactSensor = body.AddComponent<ImpactSensor>();
                var audioSource = body.AddComponent<AudioSource>();
                audioSource.maxDistance = 30;
                audioSource.dopplerLevel = 0;
                audioSource.rolloffMode = AudioRolloffMode.Custom;
                audioSource.playOnAwake = false;
                audioSource.spatialBlend = 1;

                var owAudioSource = body.AddComponent<OWAudioSource>();
                owAudioSource._audioSource = audioSource;
                owAudioSource._track = OWAudioMixer.TrackName.Environment;

                var objectImpactAudio = body.AddComponent<ObjectImpactAudio>();
                objectImpactAudio._minPitch = 0.4f;
                objectImpactAudio._maxPitch = 0.6f;
                objectImpactAudio._impactSensor = impactSensor;
                
                owRigidBody.MakeNonKinematic();
                owRigidBody.DisableKinematicSimulation();

                // Should make this number changeable
                Delay.FireOnNextUpdate(() => owRigidBody.SetMass(0.001f));
            }

            return owRigidBody;
        }
    }
}
