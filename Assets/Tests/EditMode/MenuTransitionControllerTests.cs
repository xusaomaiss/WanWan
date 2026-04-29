using NUnit.Framework;
using System.Collections;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.UI;
using Wanwan.Runtime;

namespace Wanwan.Tests.EditMode
{
    [TestFixture]
    public class MenuTransitionControllerTests
    {
        [Test]
        public void TransitionController_CanBeReferenced()
        {
            var type = typeof(MenuTransitionController);
            Assert.IsNotNull(type);
        }

        [Test]
        public void TransitionController_HasFadeGroupMethod()
        {
            var type = typeof(MenuTransitionController);
            var method = type.GetMethod("FadeGroup");
            Assert.IsNotNull(method);
        }

        [UnityTest]
        public IEnumerator FadeGroup_PreservesTransparentContainerAlpha()
        {
            GameObject parent = new GameObject("FadeParent", typeof(RectTransform), typeof(Image));
            GameObject child = new GameObject("FadeChild", typeof(RectTransform), typeof(Image));
            GameObject controllerObject = new GameObject("TransitionController");
            child.transform.SetParent(parent.transform, false);
            Image parentImage = parent.GetComponent<Image>();
            Image childImage = child.GetComponent<Image>();
            parentImage.color = Color.clear;
            childImage.color = Color.white;
            MenuTransitionController controller = controllerObject.AddComponent<MenuTransitionController>();

            try
            {
                yield return controller.FadeGroup(parent.transform, 0.01f);

                Assert.That(parentImage.color.a, Is.EqualTo(0f).Within(0.001f));
                Assert.That(childImage.color.a, Is.EqualTo(1f).Within(0.001f));
            }
            finally
            {
                Object.DestroyImmediate(controllerObject);
                Object.DestroyImmediate(parent);
            }
        }
    }
}
