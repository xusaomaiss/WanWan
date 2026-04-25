import unittest
from pathlib import Path


ROOT = Path(__file__).resolve().parents[1]


class AutomationFilesTests(unittest.TestCase):
    def test_editor_build_automation_exists(self):
        path = ROOT / "Assets/Editor/BuildAutomation.cs"
        self.assertTrue(path.exists(), f"missing {path}")

        content = path.read_text(encoding="utf-8")
        self.assertIn("BuildAndroidDebug", content)
        self.assertIn("BuildAndroidRelease", content)
        self.assertIn("RunEditModeTests", content)

    def test_shell_automation_scripts_exist(self):
        build_script = ROOT / "scripts/build_android.sh"
        release_script = ROOT / "scripts/build_android_release.sh"
        smoke_script = ROOT / "scripts/android_smoke_test.sh"
        validation_script = ROOT / "scripts/validate_unity_project.py"
        install_script = ROOT / "scripts/install_unity_editor.sh"

        self.assertTrue(build_script.exists(), f"missing {build_script}")
        self.assertTrue(release_script.exists(), f"missing {release_script}")
        self.assertTrue(smoke_script.exists(), f"missing {smoke_script}")
        self.assertTrue(validation_script.exists(), f"missing {validation_script}")
        self.assertTrue(install_script.exists(), f"missing {install_script}")

    def test_readme_mentions_automation_flow(self):
        readme = (ROOT / "README.md").read_text(encoding="utf-8")
        self.assertIn("build_android.sh", readme)
        self.assertIn("build_android_release.sh", readme)
        self.assertIn("android_smoke_test.sh", readme)
        self.assertIn("validate_unity_project.py", readme)

    def test_editmode_runner_uses_project_test_entrypoint(self):
        script = (ROOT / "scripts/run_editmode_tests.sh").read_text(encoding="utf-8")
        self.assertIn("-executeMethod Wanwan.Editor.BuildAutomation.RunEditModeTests", script)
        self.assertIn("Builds/editmode-results.xml", script)

    def test_build_script_repairs_android_cmdline_tools_layout(self):
        script = (ROOT / "scripts/build_android.sh").read_text(encoding="utf-8")
        self.assertIn("cmdline-tools/latest", script)
        self.assertIn("ln -sfn", script)
        self.assertIn("PlaybackEngines/AndroidPlayer/NDK", script)
        self.assertIn("platform-tools-2", script)
        self.assertIn("build-tools/*-2", script)

    def test_editor_install_script_uses_hub_headless_cli(self):
        script = (ROOT / "scripts/install_unity_editor.sh").read_text(encoding="utf-8")
        self.assertIn("Unity Hub.app/Contents/MacOS/Unity Hub", script)
        self.assertIn("-- --headless install", script)
        self.assertIn("--module android", script)
        self.assertIn("mkdir -p \"$INSTALL_PATH\"", script)
        self.assertIn("android-open-jdk-17.0.9+9", script)

    def test_build_automation_uses_current_unity_api_shapes(self):
        script = (ROOT / "Assets/Editor/BuildAutomation.cs").read_text(encoding="utf-8")
        self.assertIn("using UnityEditor.Build;", script)
        self.assertIn("FailCount == 0", script)
        self.assertIn("NamedBuildTarget.Android", script)

    def test_release_build_automation_sets_version_signing_and_artwork(self):
        script = (ROOT / "Assets/Editor/BuildAutomation.cs").read_text(encoding="utf-8")
        self.assertIn('BundleVersion = "1.0.1"', script)
        self.assertIn("AndroidBundleVersionCode = 2", script)
        self.assertIn("ConfigureAndroidSigning", script)
        self.assertIn("WANWAN_ANDROID_KEYSTORE", script)
        self.assertIn("SetIconsForTargetGroup", script)
        self.assertIn("ConfigureLaunchPresentation", script)

    def test_release_script_builds_signed_apk_with_local_keystore(self):
        script = (ROOT / "scripts/build_android_release.sh").read_text(encoding="utf-8")
        self.assertIn("BuildAndroidRelease", script)
        self.assertIn("wanwan-release.keystore", script)
        self.assertIn("WANWAN_ANDROID_KEYSTORE", script)
        self.assertIn("keytool", script)
        self.assertIn("clean_corrupt_gradle_artifacts", script)


if __name__ == "__main__":
    unittest.main()
