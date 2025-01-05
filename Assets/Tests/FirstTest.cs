// using NUnit.Framework;
// using UnityEngine;
// using NSubstitute;
//
// namespace Tests
// {
//     public class MenuControllerTests {
//         private MenuController _menuController;
//         private GameObject _soundMenu;
//         private GameObject _generalSettingsCanvas;
//
//         [SetUp]
//         public void Setup()
//         {
//             // Create a GameObject and add MenuController component
//             var menuGameObject = new GameObject();
//             _menuController = menuGameObject.AddComponent<MenuController>();
//
//             // Create mock objects for menus
//             _soundMenu = new GameObject();
//             _generalSettingsCanvas = new GameObject();
//
//             // Assign them to the MenuController
//             _menuController.soundMenu = _soundMenu;
//             _menuController.generalSettingsCanvas = _generalSettingsCanvas;
//
//             // Deactivate menus by default
//             _soundMenu.SetActive(false);
//             _generalSettingsCanvas.SetActive(false);
//         }
//
//         [Test]
//         public void OnMouseClick_SoundButton_OpensSoundMenu()
//         {
//             // Act
//             _menuController.OnMouseClick("Sound");
//
//             // Assert
//             Assert.IsTrue(_soundMenu.activeSelf);
//             Assert.IsFalse(_generalSettingsCanvas.activeSelf);
//         }
//
//         [Test]
//         public void OnMouseClick_UnknownButton_ShowsWarning()
//         {
//             // Use NSubstitute to mock the Warning class
//             var warning = Substitute.For<IWarning>();
//
//             // Replace the Warning instance in your class with the mock
//             _menuController.Warning = warning;
//
//             // Act
//             _menuController.OnMouseClick("UnknownButton");
//
//             // Assert
//             warning.Received().ShowWarning("Unknown button code");
//         }
//     }
// }