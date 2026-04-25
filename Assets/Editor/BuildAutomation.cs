#if UNITY_EDITOR
using System.IO;
using System.Linq;
using UnityEditor;
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
        private const string OutputDirectory = "Builds/Android";
        private const string ApkPath = OutputDirectory + "/WanwanDropBlaster.apk";

        public static void BuildAndroidDebug()
        {
            ConfigureAndroidPlayerSettings();
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

        public static void RunEditModeTests()
        {
            TestRunnerApi api = ScriptableObject.CreateInstance<TestRunnerApi>();
            ExecutionSettings executionSettings = new ExecutionSettings(new Filter
            {
                testMode = TestMode.EditMode
            });

            bool finished = false;
            bool passed = false;

            api.RegisterCallbacks(new TestCallbacks(
                runFinished => { finished = true; passed = runFinished.FailCount == 0; },
                runStarted => Debug.Log("Started EditMode tests"),
                testStarted => { },
                testFinished => { }
            ));

            api.Execute(executionSettings);

            while (!finished)
            {
            }

            if (!passed)
            {
                throw new BuildFailedException("EditMode tests failed.");
            }
        }

        private static void ConfigureAndroidPlayerSettings()
        {
            PlayerSettings.productName = ProductName;
            PlayerSettings.companyName = "Mark";
            PlayerSettings.SetApplicationIdentifier(NamedBuildTarget.Android, PackageName);
            PlayerSettings.defaultInterfaceOrientation = UIOrientation.Portrait;
            PlayerSettings.Android.targetArchitectures = AndroidArchitecture.ARM64;
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
}
#endif
