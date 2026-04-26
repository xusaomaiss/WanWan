#if UNITY_EDITOR
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.Android;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEditor.TestTools.TestRunner.Api;
using UnityEngine;

namespace Wanwan.Editor
{
    public static class BuildAutomation
    {
        private const string ProductName = "Wanwan Drop Blaster";
        private const string PackageName = "com.mark.wanwan.dropblaster";
        private const string BundleVersion = "1.0.1";
        private const int AndroidBundleVersionCode = 2;
        private const string OutputDirectory = "Builds/Android";
        private const string ApkPath = OutputDirectory + "/WanwanDropBlaster.apk";
        private const string ReleaseApkPath = OutputDirectory + "/WanwanDropBlaster-release.apk";
        private const string EditModeResultsPath = "Builds/editmode-results.xml";
        private const string AppIconPath = "Assets/Art/AppIcon.png";
        private const string LaunchSplashPath = "Assets/Art/AndroidLaunchSplash.png";

        public static void BuildAndroidDebug()
        {
            ConfigureAndroidPlayerSettings();
            ConfigureAndroidDebugSigning();
            Directory.CreateDirectory(OutputDirectory);

            BuildPlayerOptions buildPlayerOptions = new BuildPlayerOptions
            {
                scenes = EditorBuildSettings.scenes.Where(scene => scene.enabled).Select(scene => scene.path).ToArray(),
                locationPathName = ApkPath,
                target = BuildTarget.Android,
                options = BuildOptions.Development
            };

            BuildReport report = BuildPipeline.BuildPlayer(buildPlayerOptions);
            if (report.summary.result != BuildResult.Succeeded)
            {
                throw new BuildFailedException("Android build failed: " + report.summary.result);
            }

            Debug.Log("Android debug build created at " + ApkPath);
        }

        public static void BuildAndroidRelease()
        {
            ConfigureAndroidPlayerSettings();
            ConfigureAndroidSigning();
            Directory.CreateDirectory(OutputDirectory);

            BuildPlayerOptions buildPlayerOptions = new BuildPlayerOptions
            {
                scenes = EditorBuildSettings.scenes.Where(scene => scene.enabled).Select(scene => scene.path).ToArray(),
                locationPathName = ReleaseApkPath,
                target = BuildTarget.Android,
                options = BuildOptions.None
            };

            BuildReport report = BuildPipeline.BuildPlayer(buildPlayerOptions);
            if (report.summary.result != BuildResult.Succeeded)
            {
                throw new BuildFailedException("Android release build failed: " + report.summary.result);
            }

            Debug.Log("Android signed release build created at " + ReleaseApkPath);
        }

        public static void RunEditModeTests()
        {
            Directory.CreateDirectory(Path.GetDirectoryName(EditModeResultsPath));
            TestRunnerApi api = ScriptableObject.CreateInstance<TestRunnerApi>();
            ExecutionSettings executionSettings = new ExecutionSettings(new Filter
            {
                testMode = TestMode.EditMode
            });

            api.RegisterCallbacks(new TestCallbacks(
                runFinished =>
                {
                    WriteEditModeResultFile(runFinished);
                    bool passed = runFinished.FailCount == 0;
                    int total = runFinished.PassCount + runFinished.FailCount + runFinished.SkipCount + runFinished.InconclusiveCount;
                    Debug.Log($"Finished EditMode tests: total={total}, failed={runFinished.FailCount}");
                    EditorApplication.Exit(passed ? 0 : 1);
                },
                runStarted => Debug.Log("Started EditMode tests"),
                testStarted => { },
                testFinished => { }
            ));

            api.Execute(executionSettings);
        }

        private static void ConfigureAndroidPlayerSettings()
        {
            PlayerSettings.productName = ProductName;
            PlayerSettings.companyName = "Mark";
            PlayerSettings.bundleVersion = BundleVersion;
            PlayerSettings.SetApplicationIdentifier(NamedBuildTarget.Android, PackageName);
            PlayerSettings.defaultInterfaceOrientation = UIOrientation.Portrait;
            PlayerSettings.Android.targetArchitectures = AndroidArchitecture.ARM64;
            PlayerSettings.Android.bundleVersionCode = AndroidBundleVersionCode;
            PlayerSettings.Android.minSdkVersion = AndroidSdkVersions.AndroidApiLevel25;
            PlayerSettings.Android.targetSdkVersion = AndroidSdkVersions.AndroidApiLevelAuto;
            PlayerSettings.stripEngineCode = true;
            EditorUserBuildSettings.buildAppBundle = false;

            ConfigureLaunchPresentation();
            ConfigureAndroidIcons();
        }

        private static void ConfigureAndroidSigning()
        {
            string keystorePath = System.Environment.GetEnvironmentVariable("WANWAN_ANDROID_KEYSTORE");
            string keystorePass = System.Environment.GetEnvironmentVariable("WANWAN_ANDROID_KEYSTORE_PASS");
            string keyAlias = System.Environment.GetEnvironmentVariable("WANWAN_ANDROID_KEYALIAS");
            string keyAliasPass = System.Environment.GetEnvironmentVariable("WANWAN_ANDROID_KEYALIAS_PASS");

            if (string.IsNullOrEmpty(keystorePath) || !File.Exists(keystorePath))
            {
                throw new BuildFailedException("WANWAN_ANDROID_KEYSTORE must point to an existing keystore for release builds.");
            }

            if (string.IsNullOrEmpty(keystorePass) || string.IsNullOrEmpty(keyAlias) || string.IsNullOrEmpty(keyAliasPass))
            {
                throw new BuildFailedException("WANWAN_ANDROID_KEYSTORE_PASS, WANWAN_ANDROID_KEYALIAS, and WANWAN_ANDROID_KEYALIAS_PASS are required.");
            }

            PlayerSettings.Android.useCustomKeystore = true;
            PlayerSettings.Android.keystoreName = keystorePath;
            PlayerSettings.Android.keystorePass = keystorePass;
            PlayerSettings.Android.keyaliasName = keyAlias;
            PlayerSettings.Android.keyaliasPass = keyAliasPass;
        }

        private static void ConfigureAndroidDebugSigning()
        {
            PlayerSettings.Android.useCustomKeystore = false;
            PlayerSettings.Android.keystoreName = string.Empty;
            PlayerSettings.Android.keystorePass = string.Empty;
            PlayerSettings.Android.keyaliasName = string.Empty;
            PlayerSettings.Android.keyaliasPass = string.Empty;
        }

        private static void ConfigureLaunchPresentation()
        {
            PlayerSettings.SplashScreen.show = true;
            PlayerSettings.SplashScreen.backgroundColor = new Color(0.031f, 0.031f, 0.063f);
        }

        private static void ConfigureAndroidIcons()
        {
            EnsureReleaseArtwork();
            Texture2D icon = AssetDatabase.LoadAssetAtPath<Texture2D>(AppIconPath);
            if (icon != null)
            {
                PlayerSettings.SetIconsForTargetGroup(BuildTargetGroup.Android, new[] { icon });
            }
        }

        private static void EnsureReleaseArtwork()
        {
            Directory.CreateDirectory(Path.GetDirectoryName(AppIconPath));
            if (!File.Exists(AppIconPath))
            {
                WritePixelArtwork(AppIconPath, 432, true);
            }

            if (!File.Exists(LaunchSplashPath))
            {
                WritePixelArtwork(LaunchSplashPath, 1080, false);
            }

            ConfigureTextureImport(AppIconPath);
            ConfigureTextureImport(LaunchSplashPath);
        }

        private static void WritePixelArtwork(string path, int size, bool icon)
        {
            Texture2D texture = new Texture2D(size, size, TextureFormat.RGBA32, false);
            Color background = new Color(0.031f, 0.031f, 0.063f, 1f);
            Color panel = new Color(0.102f, 0.102f, 0.18f, 1f);
            Color red = new Color(0.93f, 0.09f, 0.16f, 1f);
            Color cyan = new Color(0.12f, 0.92f, 1f, 1f);
            Color white = new Color(0.86f, 0.95f, 1f, 1f);

            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    Color color = background;
                    bool scanline = y % 12 < 2;
                    if (scanline)
                    {
                        color = Color.Lerp(color, Color.black, 0.22f);
                    }

                    bool inFrame = x > size / 10 && x < size - (size / 10) && y > size / 10 && y < size - (size / 10);
                    if (inFrame && (x < size / 10 + 10 || x > size - (size / 10) - 10 || y < size / 10 + 10 || y > size - (size / 10) - 10))
                    {
                        color = panel;
                    }

                    texture.SetPixel(x, y, color);
                }
            }

            int centerX = size / 2;
            int centerY = icon ? size / 2 : (size * 58) / 100;
            int shipHeight = size / 3;
            int shipWidth = size / 4;
            DrawDiamond(texture, centerX, centerY, shipWidth, shipHeight, red);
            DrawDiamond(texture, centerX, centerY + shipHeight / 6, shipWidth / 2, shipHeight / 2, white);
            DrawRect(texture, centerX - shipWidth / 8, centerY - shipHeight / 2, shipWidth / 4, shipHeight / 2, cyan);
            DrawRect(texture, centerX - shipWidth, centerY - shipHeight / 8, shipWidth / 3, shipHeight / 5, cyan);
            DrawRect(texture, centerX + (shipWidth * 2) / 3, centerY - shipHeight / 8, shipWidth / 3, shipHeight / 5, cyan);

            File.WriteAllBytes(path, texture.EncodeToPNG());
            Object.DestroyImmediate(texture);
            AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);
        }

        private static void DrawDiamond(Texture2D texture, int centerX, int centerY, int halfWidth, int halfHeight, Color color)
        {
            for (int y = -halfHeight; y <= halfHeight; y++)
            {
                float widthAtY = halfWidth * (1f - (Mathf.Abs(y) / (float)halfHeight));
                DrawRect(texture, Mathf.RoundToInt(centerX - widthAtY), centerY + y, Mathf.RoundToInt(widthAtY * 2f), 1, color);
            }
        }

        private static void DrawRect(Texture2D texture, int x, int y, int width, int height, Color color)
        {
            for (int py = y; py < y + height; py++)
            {
                for (int px = x; px < x + width; px++)
                {
                    if (px >= 0 && px < texture.width && py >= 0 && py < texture.height)
                    {
                        texture.SetPixel(px, py, color);
                    }
                }
            }
        }

        private static void ConfigureTextureImport(string path)
        {
            TextureImporter importer = AssetImporter.GetAtPath(path) as TextureImporter;
            if (importer == null)
            {
                AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);
                importer = AssetImporter.GetAtPath(path) as TextureImporter;
            }

            if (importer == null)
            {
                return;
            }

            importer.textureType = TextureImporterType.Default;
            importer.mipmapEnabled = false;
            importer.filterMode = FilterMode.Point;
            importer.textureCompression = TextureImporterCompression.Uncompressed;
            importer.SaveAndReimport();
        }

        private static void WriteEditModeResultFile(ITestResultAdaptor result)
        {
            using StreamWriter writer = new StreamWriter(EditModeResultsPath, false);
            writer.WriteLine("<?xml version=\"1.0\" encoding=\"utf-8\"?>");
            writer.WriteLine(
                $"<test-run total=\"{result.PassCount + result.FailCount + result.SkipCount + result.InconclusiveCount}\" passed=\"{result.PassCount}\" failed=\"{result.FailCount}\" skipped=\"{result.SkipCount}\" result=\"{(result.FailCount == 0 ? "Passed" : "Failed")}\">");
            writer.WriteLine($"  <test-suite name=\"EditMode\" result=\"{(result.FailCount == 0 ? "Passed" : "Failed")}\" />");
            writer.WriteLine("</test-run>");
        }

        private sealed class TestCallbacks : ICallbacks
        {
            private readonly System.Action<ITestResultAdaptor> runFinished;
            private readonly System.Action<ITestAdaptor> runStarted;
            private readonly System.Action<ITestAdaptor> testStarted;
            private readonly System.Action<ITestResultAdaptor> testFinished;

            public TestCallbacks(
                System.Action<ITestResultAdaptor> runFinished,
                System.Action<ITestAdaptor> runStarted,
                System.Action<ITestAdaptor> testStarted,
                System.Action<ITestResultAdaptor> testFinished)
            {
                this.runFinished = runFinished;
                this.runStarted = runStarted;
                this.testStarted = testStarted;
                this.testFinished = testFinished;
            }

            public void RunStarted(ITestAdaptor testsToRun)
            {
                runStarted?.Invoke(testsToRun);
            }

            public void RunFinished(ITestResultAdaptor result)
            {
                runFinished?.Invoke(result);
            }

            public void TestStarted(ITestAdaptor test)
            {
                testStarted?.Invoke(test);
            }

            public void TestFinished(ITestResultAdaptor result)
            {
                testFinished?.Invoke(result);
            }
        }
    }

    public sealed class AndroidGradleReleasePostprocessor : IPostGenerateGradleAndroidProject
    {
        private const string DisableAnnotationExtractionMarker = "// WANWAN_DISABLE_RELEASE_ANNOTATION_EXTRACTION";

        public int callbackOrder => 100;

        public void OnPostGenerateGradleAndroidProject(string path)
        {
            PatchBuildGradle(Path.Combine(path, "build.gradle"));
            PatchBuildGradle(Path.Combine(Directory.GetParent(path).FullName, "launcher", "build.gradle"));
        }

        private static void PatchBuildGradle(string buildGradlePath)
        {
            if (!File.Exists(buildGradlePath))
            {
                return;
            }

            string content = File.ReadAllText(buildGradlePath);
            if (content.Contains(DisableAnnotationExtractionMarker))
            {
                return;
            }

            File.AppendAllText(
                buildGradlePath,
                "\n" + DisableAnnotationExtractionMarker + "\n" +
                "afterEvaluate {\n" +
                "    tasks.matching { it.name == 'extractReleaseAnnotations' || it.name == 'extractDebugAnnotations' || it.name.startsWith('lintVital') }.configureEach {\n" +
                "        enabled = false\n" +
                "    }\n" +
                "    def typedefFile = file('build/intermediates/annotations_typedef_file/release/extractReleaseAnnotations/typedefs.txt')\n" +
                "    typedefFile.parentFile.mkdirs()\n" +
                "    if (!typedefFile.exists()) { typedefFile.text = '' }\n" +
                "}\n");
        }
    }
}
#endif
