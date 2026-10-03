#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

namespace MiladCharacters.EditorTools
{
    /// <summary>
    /// Generates a simple, smooth, visual-only procedural suit character prefab.
    /// No gameplay systems are created.
    /// </summary>
    public static class MiladSuitGenerator
    {
        private const string RootFolder = "Assets/MiladCharacters";
        private const string MaterialFolder = RootFolder + "/Materials";
        private const string MeshFolder = RootFolder + "/Meshes";
        private const string PrefabFolder = RootFolder + "/Prefabs";
        private const string PrefabPath = PrefabFolder + "/Milad_Suit.prefab";
        private const float TargetHeight = 1.80f;

        private sealed class MaterialSet
        {
            public Material Skin;
            public Material SkinShadow;
            public Material Hair;
            public Material Suit;
            public Material SuitDetail;
            public Material Shirt;
            public Material Tie;
            public Material Shoes;
            public Material ShoeSole;
            public Material GlassFrame;
            public Material GlassLens;
            public Material Metal;
            public Material Bracelet;
            public Material Mouth;
        }

        private sealed class MeshSet
        {
            public Mesh Head;
            public Mesh SoftRounded;
            public Mesh FirmRounded;
            public Mesh Capsule;
            public Mesh Cylinder;
            public Mesh Torus;
        }

        [MenuItem("Tools/Milad Characters/Create Suit")]
        public static void CreateSuit()
        {
            EnsureFolders();

            MaterialSet materials = CreateMaterials();
            MeshSet meshes = CreateMeshes();
            GameObject character = BuildCharacter(materials, meshes);

            PrefabUtility.SaveAsPrefabAsset(character, PrefabPath);
            UnityEngine.Object.DestroyImmediate(character);

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(PrefabPath);
            Selection.activeObject = prefab;
            EditorGUIUtility.PingObject(prefab);
            Debug.Log("Milad_Suit visual prefab created at: " + PrefabPath);
        }

        private static GameObject BuildCharacter(MaterialSet m, MeshSet mesh)
        {
            GameObject root = new GameObject("Milad_Suit");
            root.transform.position = Vector3.zero;
            root.transform.rotation = Quaternion.identity;
            root.transform.localScale = Vector3.one;

            Transform visualRoot = Bone("VisualRoot", root.transform, Vector3.zero);
            Transform hips = Bone("Hips", visualRoot, new Vector3(0f, 0.92f, 0f));
            Transform spine = Bone("Spine", hips, new Vector3(0f, 0.17f, 0f));
            Transform chest = Bone("Chest", spine, new Vector3(0f, 0.21f, 0f));
            Transform neck = Bone("Neck", chest, new Vector3(0f, 0.20f, 0f));
            Transform head = Bone("Head", neck, new Vector3(0f, 0.115f, 0f));

            BuildTorso(hips, spine, chest, mesh, m);
            BuildHead(head, mesh, m);
            BuildArms(chest, mesh, m);
            BuildLegs(hips, mesh, m);

            NormalizeVisualHeight(visualRoot, TargetHeight);
            return root;
        }

        private static void BuildTorso(Transform hips, Transform spine, Transform chest, MeshSet mesh, MaterialSet m)
        {
            // Trousers
            Part("Suit_Waist", hips, mesh.FirmRounded, m.Suit,
                new Vector3(0f, -0.035f, 0f), new Vector3(0.45f, 0.24f, 0.28f));
            Part("Belt", hips, mesh.Torus, m.SuitDetail,
                new Vector3(0f, 0.05f, 0f), new Vector3(0.41f, 0.022f, 0.25f));
            Part("Buckle", hips, mesh.FirmRounded, m.Metal,
                new Vector3(0f, 0.042f, 0.145f), new Vector3(0.045f, 0.030f, 0.016f));

            // Shirt and jacket
            Part("Shirt_Core", chest, mesh.SoftRounded, m.Shirt,
                new Vector3(0f, -0.01f, 0.02f), new Vector3(0.42f, 0.40f, 0.23f));
            Part("Jacket_Lower", spine, mesh.SoftRounded, m.Suit,
                new Vector3(0f, -0.02f, 0f), new Vector3(0.50f, 0.32f, 0.29f));
            Part("Jacket_Upper", chest, mesh.SoftRounded, m.Suit,
                new Vector3(0f, -0.01f, 0f), new Vector3(0.57f, 0.44f, 0.31f));
            Part("Jacket_Hem", spine, mesh.Torus, m.SuitDetail,
                new Vector3(0f, -0.165f, 0f), new Vector3(0.46f, 0.018f, 0.27f));

            // Jacket opening
            Part("Shirt_V_Upper", chest, mesh.FirmRounded, m.Shirt,
                new Vector3(0f, 0.10f, 0.14f), new Vector3(0.18f, 0.20f, 0.02f));
            Part("Shirt_V_Lower", chest, mesh.FirmRounded, m.Shirt,
                new Vector3(0f, -0.02f, 0.15f), new Vector3(0.14f, 0.16f, 0.02f));

            // Lapels
            Part("Lapel_Left", chest, mesh.FirmRounded, m.SuitDetail,
                new Vector3(-0.075f, 0.11f, 0.155f), new Vector3(0.12f, 0.18f, 0.024f), new Vector3(0f, 0f, 22f));
            Part("Lapel_Right", chest, mesh.FirmRounded, m.SuitDetail,
                new Vector3(0.075f, 0.11f, 0.155f), new Vector3(0.12f, 0.18f, 0.024f), new Vector3(0f, 0f, -22f));
            Part("Collar_Back", chest, mesh.SoftRounded, m.SuitDetail,
                new Vector3(0f, 0.19f, 0.01f), new Vector3(0.20f, 0.05f, 0.18f));

            // Buttons
            Part("Button_1", chest, mesh.Cylinder, m.Metal,
                new Vector3(0f, 0.015f, 0.165f), new Vector3(0.020f, 0.008f, 0.020f), new Vector3(90f, 0f, 0f));
            Part("Button_2", chest, mesh.Cylinder, m.Metal,
                new Vector3(0f, -0.055f, 0.165f), new Vector3(0.020f, 0.008f, 0.020f), new Vector3(90f, 0f, 0f));

            // Plain tie
            Part("Tie_Knot", chest, mesh.FirmRounded, m.Tie,
                new Vector3(0f, 0.115f, 0.168f), new Vector3(0.045f, 0.050f, 0.020f));
            Part("Tie_Blade", chest, mesh.FirmRounded, m.Tie,
                new Vector3(0f, -0.005f, 0.167f), new Vector3(0.055f, 0.22f, 0.018f));
            Part("Tie_Tip", chest, mesh.FirmRounded, m.Tie,
                new Vector3(0f, -0.150f, 0.167f), new Vector3(0.045f, 0.060f, 0.018f), new Vector3(0f, 0f, 45f));
        }

        private static void BuildHead(Transform head, MeshSet mesh, MaterialSet m)
        {
            Part("Face", head, mesh.Head, m.Skin,
                new Vector3(0f, 0.02f, 0f), new Vector3(0.30f, 0.34f, 0.28f));
            Part("Jaw_Shadow", head, mesh.SoftRounded, m.SkinShadow,
                new Vector3(0f, -0.09f, 0.01f), new Vector3(0.24f, 0.16f, 0.23f));
            Part("Ear_Left", head, mesh.SoftRounded, m.Skin,
                new Vector3(-0.155f, 0.00f, 0.0f), new Vector3(0.05f, 0.095f, 0.045f));
            Part("Ear_Right", head, mesh.SoftRounded, m.Skin,
                new Vector3(0.155f, 0.00f, 0.0f), new Vector3(0.05f, 0.095f, 0.045f));
            Part("Nose", head, mesh.SoftRounded, m.Skin,
                new Vector3(0f, -0.005f, 0.15f), new Vector3(0.05f, 0.085f, 0.065f), new Vector3(10f, 0f, 0f));
            Part("Mouth", head, mesh.Capsule, m.Mouth,
                new Vector3(0f, -0.108f, 0.145f), new Vector3(0.010f, 0.065f, 0.010f), new Vector3(0f, 0f, 90f));

            BuildHair(head, mesh, m);
            BuildGlasses(head, mesh, m);
            BuildMoustache(head, mesh, m);
        }

        private static void BuildHair(Transform head, MeshSet mesh, MaterialSet m)
        {
            Part("Hair_Back", head, mesh.SoftRounded, m.Hair,
                new Vector3(0f, 0.11f, -0.08f), new Vector3(0.28f, 0.23f, 0.17f));
            Part("Hair_Side_Left", head, mesh.SoftRounded, m.Hair,
                new Vector3(-0.125f, 0.09f, -0.005f), new Vector3(0.09f, 0.21f, 0.16f));
            Part("Hair_Side_Right", head, mesh.SoftRounded, m.Hair,
                new Vector3(0.125f, 0.09f, -0.005f), new Vector3(0.09f, 0.21f, 0.16f));
            Part("Hair_Top", head, mesh.SoftRounded, m.Hair,
                new Vector3(0f, 0.22f, 0.03f), new Vector3(0.31f, 0.15f, 0.20f), new Vector3(-12f, 0f, 0f));
            Part("Hair_Front", head, mesh.SoftRounded, m.Hair,
                new Vector3(0.03f, 0.205f, 0.10f), new Vector3(0.22f, 0.08f, 0.11f), new Vector3(-18f, 0f, 0f));
        }

        private static void BuildGlasses(Transform head, MeshSet mesh, MaterialSet m)
        {
            Part("Lens_Left", head, mesh.FirmRounded, m.GlassLens,
                new Vector3(-0.085f, 0.043f, 0.154f), new Vector3(0.14f, 0.09f, 0.020f));
            Part("Lens_Right", head, mesh.FirmRounded, m.GlassLens,
                new Vector3(0.085f, 0.043f, 0.154f), new Vector3(0.14f, 0.09f, 0.020f));
            Part("Frame_Left", head, mesh.FirmRounded, m.GlassFrame,
                new Vector3(-0.085f, 0.043f, 0.165f), new Vector3(0.152f, 0.102f, 0.010f));
            Part("Frame_Right", head, mesh.FirmRounded, m.GlassFrame,
                new Vector3(0.085f, 0.043f, 0.165f), new Vector3(0.152f, 0.102f, 0.010f));
            Part("Bridge", head, mesh.Capsule, m.GlassFrame,
                new Vector3(0f, 0.048f, 0.165f), new Vector3(0.012f, 0.045f, 0.012f), new Vector3(0f, 0f, 90f));
            Part("Arm_Left", head, mesh.Capsule, m.GlassFrame,
                new Vector3(-0.165f, 0.045f, 0.055f), new Vector3(0.016f, 0.17f, 0.016f), new Vector3(90f, 0f, 0f));
            Part("Arm_Right", head, mesh.Capsule, m.GlassFrame,
                new Vector3(0.165f, 0.045f, 0.055f), new Vector3(0.016f, 0.17f, 0.016f), new Vector3(90f, 0f, 0f));
        }

        private static void BuildMoustache(Transform head, MeshSet mesh, MaterialSet m)
        {
            Part("Moustache_Left", head, mesh.SoftRounded, m.Hair,
                new Vector3(-0.043f, -0.07f, 0.152f), new Vector3(0.11f, 0.036f, 0.025f), new Vector3(2f, 0f, 10f));
            Part("Moustache_Right", head, mesh.SoftRounded, m.Hair,
                new Vector3(0.043f, -0.07f, 0.152f), new Vector3(0.11f, 0.036f, 0.025f), new Vector3(2f, 0f, -10f));
            Part("Moustache_Center", head, mesh.SoftRounded, m.Hair,
                new Vector3(0f, -0.066f, 0.153f), new Vector3(0.034f, 0.030f, 0.024f));
        }

        private static void BuildArms(Transform chest, MeshSet mesh, MaterialSet m)
        {
            Transform leftShoulder = Bone("LeftShoulder", chest, new Vector3(-0.305f, 0.095f, 0f));
            Transform leftUpperArm = Bone("LeftUpperArm", leftShoulder, Vector3.zero);
            leftUpperArm.localRotation = Quaternion.Euler(0f, 0f, 4f);
            Transform leftForearm = Bone("LeftForearm", leftUpperArm, new Vector3(0f, -0.300f, 0f));
            Transform leftHand = Bone("LeftHand", leftForearm, new Vector3(0f, -0.270f, 0f));

            Transform rightShoulder = Bone("RightShoulder", chest, new Vector3(0.305f, 0.095f, 0f));
            Transform rightUpperArm = Bone("RightUpperArm", rightShoulder, Vector3.zero);
            rightUpperArm.localRotation = Quaternion.Euler(0f, 0f, -4f);
            Transform rightForearm = Bone("RightForearm", rightUpperArm, new Vector3(0f, -0.300f, 0f));
            Transform rightHand = Bone("RightHand", rightForearm, new Vector3(0f, -0.270f, 0f));

            BuildArm("Left", leftUpperArm, leftForearm, leftHand, true, mesh, m);
            BuildArm("Right", rightUpperArm, rightForearm, rightHand, false, mesh, m);
        }

        private static void BuildArm(string side, Transform upperArm, Transform forearm, Transform hand, bool hasWatch, MeshSet mesh, MaterialSet m)
        {
            Part(side + "_UpperSleeve", upperArm, mesh.Capsule, m.Suit,
                new Vector3(0f, -0.145f, 0f), new Vector3(0.165f, 0.305f, 0.165f));
            Part(side + "_ForearmSleeve", forearm, mesh.Capsule, m.Suit,
                new Vector3(0f, -0.135f, 0f), new Vector3(0.145f, 0.275f, 0.145f));
            Part(side + "_ShirtCuff", forearm, mesh.Torus, m.Shirt,
                new Vector3(0f, -0.248f, 0f), new Vector3(0.145f, 0.015f, 0.145f));
            Part(side + "_Palm", hand, mesh.SoftRounded, m.Skin,
                new Vector3(0f, -0.020f, 0.008f), new Vector3(0.115f, 0.155f, 0.100f));
            Part(side + "_Thumb", hand, mesh.Capsule, m.Skin,
                new Vector3(side == "Left" ? 0.055f : -0.055f, -0.020f, 0.020f),
                new Vector3(0.036f, 0.085f, 0.036f), new Vector3(0f, 0f, side == "Left" ? -25f : 25f));

            if (hasWatch)
            {
                Part("Watch_Band", forearm, mesh.Torus, m.GlassFrame,
                    new Vector3(0f, -0.242f, 0f), new Vector3(0.140f, 0.012f, 0.140f));
                Part("Watch_Face", forearm, mesh.FirmRounded, m.Metal,
                    new Vector3(0f, -0.243f, 0.076f), new Vector3(0.068f, 0.040f, 0.020f));
            }
            else
            {
                Part("Yellow_Bracelet", forearm, mesh.Torus, m.Bracelet,
                    new Vector3(0f, -0.242f, 0f), new Vector3(0.140f, 0.012f, 0.140f));
            }
        }

        private static void BuildLegs(Transform hips, MeshSet mesh, MaterialSet m)
        {
            Transform leftThigh = Bone("LeftThigh", hips, new Vector3(-0.140f, -0.020f, 0f));
            Transform leftLowerLeg = Bone("LeftLowerLeg", leftThigh, new Vector3(0f, -0.390f, 0f));
            Transform leftFoot = Bone("LeftFoot", leftLowerLeg, new Vector3(0f, -0.355f, 0.045f));

            Transform rightThigh = Bone("RightThigh", hips, new Vector3(0.140f, -0.020f, 0f));
            Transform rightLowerLeg = Bone("RightLowerLeg", rightThigh, new Vector3(0f, -0.390f, 0f));
            Transform rightFoot = Bone("RightFoot", rightLowerLeg, new Vector3(0f, -0.355f, 0.045f));

            BuildLeg("Left", leftThigh, leftLowerLeg, leftFoot, mesh, m);
            BuildLeg("Right", rightThigh, rightLowerLeg, rightFoot, mesh, m);
        }

        private static void BuildLeg(string side, Transform thigh, Transform lowerLeg, Transform foot, MeshSet mesh, MaterialSet m)
        {
            Part(side + "_Thigh", thigh, mesh.Capsule, m.Suit,
                new Vector3(0f, -0.195f, 0f), new Vector3(0.205f, 0.395f, 0.205f));
            Part(side + "_LowerLeg", lowerLeg, mesh.Capsule, m.Suit,
                new Vector3(0f, -0.180f, 0f), new Vector3(0.180f, 0.360f, 0.180f));
            BuildShoe(side, foot, mesh, m);
        }

        private static void BuildShoe(string side, Transform foot, MeshSet mesh, MaterialSet m)
        {
            Part(side + "_ShoeUpper", foot, mesh.SoftRounded, m.Shoes,
                new Vector3(0f, -0.040f, 0.10f), new Vector3(0.22f, 0.11f, 0.35f), new Vector3(-4f, 0f, 0f));
            Part(side + "_ShoeToe", foot, mesh.SoftRounded, m.Shoes,
                new Vector3(0f, -0.038f, 0.21f), new Vector3(0.21f, 0.09f, 0.17f));
            Part(side + "_Sole", foot, mesh.FirmRounded, m.ShoeSole,
                new Vector3(0f, -0.095f, 0.10f), new Vector3(0.24f, 0.030f, 0.37f));
        }

        private static Transform Bone(string name, Transform parent, Vector3 localPosition)
        {
            GameObject bone = new GameObject(name);
            bone.transform.SetParent(parent, false);
            bone.transform.localPosition = localPosition;
            bone.transform.localRotation = Quaternion.identity;
            bone.transform.localScale = Vector3.one;
            return bone.transform;
        }

        private static GameObject Part(string name, Transform parent, Mesh mesh, Material material,
            Vector3 localPosition, Vector3 localScale, Vector3? localEuler = null)
        {
            GameObject part = new GameObject(name);
            part.transform.SetParent(parent, false);
            part.transform.localPosition = localPosition;
            part.transform.localRotation = Quaternion.Euler(localEuler ?? Vector3.zero);
            part.transform.localScale = localScale;

            MeshFilter filter = part.AddComponent<MeshFilter>();
            filter.sharedMesh = mesh;
            MeshRenderer renderer = part.AddComponent<MeshRenderer>();
            renderer.sharedMaterial = material;
            renderer.shadowCastingMode = ShadowCastingMode.On;
            renderer.receiveShadows = true;
            return part;
        }

        private static void NormalizeVisualHeight(Transform visualRoot, float targetHeight)
        {
            Renderer[] renderers = visualRoot.GetComponentsInChildren<Renderer>();
            if (renderers.Length == 0) return;
            Bounds bounds = renderers[0].bounds;
            for (int i = 1; i < renderers.Length; i++) bounds.Encapsulate(renderers[i].bounds);
            if (bounds.size.y <= 0.0001f) return;
            float scale = targetHeight / bounds.size.y;
            visualRoot.localScale = Vector3.one * scale;
            visualRoot.localPosition = new Vector3(0f, -bounds.min.y * scale, 0f);
        }

        private static MaterialSet CreateMaterials()
        {
            return new MaterialSet
            {
                Skin = GetOrCreateMaterial("Skin_Warm_Suit", new Color(0.73f, 0.47f, 0.41f), 0f, 0.40f),
                SkinShadow = GetOrCreateMaterial("Skin_Shadow_Suit", new Color(0.62f, 0.39f, 0.35f), 0f, 0.36f),
                Hair = GetOrCreateMaterial("Hair_DarkBrown_Suit", new Color(0.055f, 0.025f, 0.040f), 0f, 0.30f),
                Suit = GetOrCreateMaterial("Suit_Dark", new Color(0.12f, 0.09f, 0.16f), 0f, 0.30f),
                SuitDetail = GetOrCreateMaterial("Suit_Detail", new Color(0.16f, 0.12f, 0.20f), 0f, 0.26f),
                Shirt = GetOrCreateMaterial("Shirt_White", new Color(0.93f, 0.93f, 0.95f), 0f, 0.34f),
                Tie = GetOrCreateMaterial("Tie_Plain", new Color(0.16f, 0.16f, 0.18f), 0f, 0.32f),
                Shoes = GetOrCreateMaterial("Shoes_Black", new Color(0.08f, 0.08f, 0.10f), 0f, 0.32f),
                ShoeSole = GetOrCreateMaterial("Shoes_Sole_Black", new Color(0.20f, 0.20f, 0.22f), 0f, 0.24f),
                GlassFrame = GetOrCreateMaterial("Glasses_Frame_Suit", new Color(0.015f, 0.015f, 0.020f), 0.04f, 0.60f),
                GlassLens = GetOrCreateMaterial("Glasses_Lens_Suit", new Color(0.030f, 0.030f, 0.038f), 0.02f, 0.72f),
                Metal = GetOrCreateMaterial("Metal_Suit", new Color(0.17f, 0.18f, 0.20f), 0.65f, 0.55f),
                Bracelet = GetOrCreateMaterial("Bracelet_Yellow_Suit", new Color(0.96f, 0.72f, 0.10f), 0f, 0.36f),
                Mouth = GetOrCreateMaterial("Mouth_Suit", new Color(0.19f, 0.06f, 0.06f), 0f, 0.34f)
            };
        }

        private static Material GetOrCreateMaterial(string assetName, Color color, float metallic, float smoothness)
        {
            string path = MaterialFolder + "/" + assetName + ".mat";
            Material existing = AssetDatabase.LoadAssetAtPath<Material>(path);
            if (existing != null) return existing;

            Shader shader = Shader.Find("Universal Render Pipeline/Lit");
            if (shader == null) shader = Shader.Find("Standard");
            if (shader == null) throw new InvalidOperationException("No compatible Lit shader was found.");

            Material material = new Material(shader) { name = assetName };
            if (material.HasProperty("_BaseColor")) material.SetColor("_BaseColor", color);
            if (material.HasProperty("_Color")) material.SetColor("_Color", color);
            if (material.HasProperty("_Metallic")) material.SetFloat("_Metallic", metallic);
            if (material.HasProperty("_Smoothness")) material.SetFloat("_Smoothness", smoothness);
            if (material.HasProperty("_Glossiness")) material.SetFloat("_Glossiness", smoothness);
            AssetDatabase.CreateAsset(material, path);
            return material;
        }

        private static MeshSet CreateMeshes()
        {
            return new MeshSet
            {
                Head = GetOrCreateMesh("Suit_Head", () => CreateSuperellipsoidMesh(30, 20, 0.92f)),
                SoftRounded = GetOrCreateMesh("Suit_SoftRounded", () => CreateSuperellipsoidMesh(28, 18, 0.74f)),
                FirmRounded = GetOrCreateMesh("Suit_FirmRounded", () => CreateSuperellipsoidMesh(28, 18, 0.52f)),
                Capsule = GetOrCreateMesh("Suit_Capsule18", () => CreateCapsuleMesh(18, 6)),
                Cylinder = GetOrCreateMesh("Suit_Cylinder18", () => CreateCylinderMesh(18)),
                Torus = GetOrCreateMesh("Suit_Torus24x10", () => CreateTorusMesh(24, 10, 0.38f, 0.11f))
            };
        }

        private static Mesh GetOrCreateMesh(string assetName, Func<Mesh> factory)
        {
            string path = MeshFolder + "/" + assetName + ".asset";
            Mesh existing = AssetDatabase.LoadAssetAtPath<Mesh>(path);
            if (existing != null) return existing;
            Mesh mesh = factory();
            mesh.name = assetName;
            mesh.RecalculateBounds();
            AssetDatabase.CreateAsset(mesh, path);
            return mesh;
        }

        private static Mesh CreateSuperellipsoidMesh(int longitudeSegments, int latitudeSegments, float exponent)
        {
            List<Vector3> vertices = new List<Vector3>();
            List<Vector2> uv = new List<Vector2>();
            List<int> triangles = new List<int>();

            for (int lat = 0; lat <= latitudeSegments; lat++)
            {
                float v = lat / (float)latitudeSegments;
                float latitude = Mathf.Lerp(-Mathf.PI * 0.5f, Mathf.PI * 0.5f, v);
                float cv = Mathf.Cos(latitude);
                float sv = Mathf.Sin(latitude);
                for (int lon = 0; lon <= longitudeSegments; lon++)
                {
                    float u = lon / (float)longitudeSegments;
                    float longitude = u * Mathf.PI * 2f;
                    float cu = Mathf.Cos(longitude);
                    float su = Mathf.Sin(longitude);
                    float x = SignedPow(cv, exponent) * SignedPow(cu, exponent);
                    float y = SignedPow(sv, exponent);
                    float z = SignedPow(cv, exponent) * SignedPow(su, exponent);
                    vertices.Add(new Vector3(x, y, z) * 0.5f);
                    uv.Add(new Vector2(u, v));
                }
            }

            int stride = longitudeSegments + 1;
            for (int lat = 0; lat < latitudeSegments; lat++)
            {
                for (int lon = 0; lon < longitudeSegments; lon++)
                {
                    int i0 = lat * stride + lon;
                    int i1 = i0 + 1;
                    int i2 = i0 + stride;
                    int i3 = i2 + 1;
                    triangles.Add(i0); triangles.Add(i2); triangles.Add(i1);
                    triangles.Add(i1); triangles.Add(i2); triangles.Add(i3);
                }
            }

            Mesh mesh = new Mesh { indexFormat = IndexFormat.UInt32 };
            mesh.SetVertices(vertices);
            mesh.SetUVs(0, uv);
            mesh.SetTriangles(triangles, 0);
            mesh.RecalculateNormals();
            return mesh;
        }

        private static float SignedPow(float value, float power)
        {
            if (Mathf.Abs(value) < 0.000001f) return 0f;
            return Mathf.Sign(value) * Mathf.Pow(Mathf.Abs(value), power);
        }

        private static Mesh CreateCapsuleMesh(int radialSegments, int hemisphereRings)
        {
            List<Vector3> vertices = new List<Vector3>();
            List<Vector2> uv = new List<Vector2>();
            List<int> triangles = new List<int>();
            float radius = 0.34f;
            float cylinderHalf = 0.16f;
            int totalRings = hemisphereRings * 2 + 1;

            for (int ring = 0; ring <= totalRings; ring++)
            {
                float y;
                float ringRadius;
                if (ring <= hemisphereRings)
                {
                    float t = ring / (float)hemisphereRings;
                    float angle = Mathf.Lerp(-Mathf.PI * 0.5f, 0f, t);
                    y = -cylinderHalf + Mathf.Sin(angle) * radius;
                    ringRadius = Mathf.Cos(angle) * radius;
                }
                else if (ring == hemisphereRings + 1)
                {
                    y = cylinderHalf;
                    ringRadius = radius;
                }
                else
                {
                    float t = (ring - hemisphereRings - 1) / (float)hemisphereRings;
                    float angle = Mathf.Lerp(0f, Mathf.PI * 0.5f, t);
                    y = cylinderHalf + Mathf.Sin(angle) * radius;
                    ringRadius = Mathf.Cos(angle) * radius;
                }
                float normalizedY = y / (cylinderHalf + radius) * 0.5f;
                for (int side = 0; side <= radialSegments; side++)
                {
                    float u = side / (float)radialSegments;
                    float angle = u * Mathf.PI * 2f;
                    vertices.Add(new Vector3(Mathf.Cos(angle) * ringRadius, normalizedY, Mathf.Sin(angle) * ringRadius));
                    uv.Add(new Vector2(u, ring / (float)totalRings));
                }
            }

            int stride = radialSegments + 1;
            for (int ring = 0; ring < totalRings; ring++)
            {
                for (int side = 0; side < radialSegments; side++)
                {
                    int i0 = ring * stride + side;
                    int i1 = i0 + 1;
                    int i2 = i0 + stride;
                    int i3 = i2 + 1;
                    triangles.Add(i0); triangles.Add(i2); triangles.Add(i1);
                    triangles.Add(i1); triangles.Add(i2); triangles.Add(i3);
                }
            }

            Mesh mesh = new Mesh();
            mesh.SetVertices(vertices);
            mesh.SetUVs(0, uv);
            mesh.SetTriangles(triangles, 0);
            mesh.RecalculateNormals();
            return mesh;
        }

        private static Mesh CreateCylinderMesh(int sides)
        {
            List<Vector3> vertices = new List<Vector3>();
            List<Vector3> normals = new List<Vector3>();
            List<Vector2> uv = new List<Vector2>();
            List<int> triangles = new List<int>();
            for (int side = 0; side <= sides; side++)
            {
                float u = side / (float)sides;
                float angle = u * Mathf.PI * 2f;
                float x = Mathf.Cos(angle) * 0.5f;
                float z = Mathf.Sin(angle) * 0.5f;
                Vector3 normal = new Vector3(x, 0f, z).normalized;
                vertices.Add(new Vector3(x, -0.5f, z));
                vertices.Add(new Vector3(x, 0.5f, z));
                normals.Add(normal); normals.Add(normal);
                uv.Add(new Vector2(u, 0f)); uv.Add(new Vector2(u, 1f));
            }
            for (int side = 0; side < sides; side++)
            {
                int i0 = side * 2;
                int i1 = i0 + 1;
                int i2 = i0 + 2;
                int i3 = i2 + 1;
                triangles.Add(i0); triangles.Add(i1); triangles.Add(i2);
                triangles.Add(i2); triangles.Add(i1); triangles.Add(i3);
            }
            AddCylinderCap(vertices, normals, uv, triangles, sides, 0.5f, true);
            AddCylinderCap(vertices, normals, uv, triangles, sides, -0.5f, false);
            Mesh mesh = new Mesh();
            mesh.SetVertices(vertices);
            mesh.SetNormals(normals);
            mesh.SetUVs(0, uv);
            mesh.SetTriangles(triangles, 0);
            return mesh;
        }

        private static void AddCylinderCap(List<Vector3> vertices, List<Vector3> normals, List<Vector2> uv, List<int> triangles, int sides, float y, bool top)
        {
            int center = vertices.Count;
            vertices.Add(new Vector3(0f, y, 0f));
            normals.Add(top ? Vector3.up : Vector3.down);
            uv.Add(new Vector2(0.5f, 0.5f));
            int ringStart = vertices.Count;
            for (int side = 0; side < sides; side++)
            {
                float angle = side / (float)sides * Mathf.PI * 2f;
                float x = Mathf.Cos(angle) * 0.5f;
                float z = Mathf.Sin(angle) * 0.5f;
                vertices.Add(new Vector3(x, y, z));
                normals.Add(top ? Vector3.up : Vector3.down);
                uv.Add(new Vector2(x + 0.5f, z + 0.5f));
            }
            for (int side = 0; side < sides; side++)
            {
                int current = ringStart + side;
                int next = ringStart + (side + 1) % sides;
                if (top)
                {
                    triangles.Add(center); triangles.Add(next); triangles.Add(current);
                }
                else
                {
                    triangles.Add(center); triangles.Add(current); triangles.Add(next);
                }
            }
        }

        private static Mesh CreateTorusMesh(int majorSegments, int minorSegments, float majorRadius, float minorRadius)
        {
            List<Vector3> vertices = new List<Vector3>();
            List<Vector2> uv = new List<Vector2>();
            List<int> triangles = new List<int>();
            for (int major = 0; major <= majorSegments; major++)
            {
                float u = major / (float)majorSegments;
                float majorAngle = u * Mathf.PI * 2f;
                Vector3 radial = new Vector3(Mathf.Cos(majorAngle), 0f, Mathf.Sin(majorAngle));
                for (int minor = 0; minor <= minorSegments; minor++)
                {
                    float v = minor / (float)minorSegments;
                    float minorAngle = v * Mathf.PI * 2f;
                    float ringDistance = majorRadius + Mathf.Cos(minorAngle) * minorRadius;
                    Vector3 position = radial * ringDistance;
                    position.y = Mathf.Sin(minorAngle) * minorRadius;
                    vertices.Add(position);
                    uv.Add(new Vector2(u, v));
                }
            }
            int stride = minorSegments + 1;
            for (int major = 0; major < majorSegments; major++)
            {
                for (int minor = 0; minor < minorSegments; minor++)
                {
                    int i0 = major * stride + minor;
                    int i1 = i0 + 1;
                    int i2 = i0 + stride;
                    int i3 = i2 + 1;
                    triangles.Add(i0); triangles.Add(i2); triangles.Add(i1);
                    triangles.Add(i1); triangles.Add(i2); triangles.Add(i3);
                }
            }
            Mesh mesh = new Mesh();
            mesh.SetVertices(vertices);
            mesh.SetUVs(0, uv);
            mesh.SetTriangles(triangles, 0);
            mesh.RecalculateNormals();
            return mesh;
        }

        private static void EnsureFolders()
        {
            EnsureFolder("Assets", "MiladCharacters");
            EnsureFolder(RootFolder, "Editor");
            EnsureFolder(RootFolder, "Materials");
            EnsureFolder(RootFolder, "Meshes");
            EnsureFolder(RootFolder, "Prefabs");
        }

        private static void EnsureFolder(string parent, string child)
        {
            string fullPath = parent + "/" + child;
            if (!AssetDatabase.IsValidFolder(fullPath))
                AssetDatabase.CreateFolder(parent, child);
        }
    }
}
#endif
