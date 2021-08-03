// GENERATED AUTOMATICALLY FROM 'Assets/_Input/_Controller.inputactions'

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

public class @_Controller : IInputActionCollection, IDisposable
{
    public InputActionAsset asset { get; }
    public @_Controller()
    {
        asset = InputActionAsset.FromJson(@"{
    ""name"": ""_Controller"",
    ""maps"": [
        {
            ""name"": ""Player"",
            ""id"": ""2cede31d-3b07-411b-8ee4-3f5a69e52eca"",
            ""actions"": [
                {
                    ""name"": ""Move"",
                    ""type"": ""PassThrough"",
                    ""id"": ""88453c9e-ebd0-4271-a669-6cd852506519"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Action1"",
                    ""type"": ""Button"",
                    ""id"": ""8d36a7b8-9cf4-4980-8f02-2ffee49f0207"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Action2"",
                    ""type"": ""Button"",
                    ""id"": ""904d9a70-256f-4b65-819d-6b8068d9833c"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Action3"",
                    ""type"": ""Button"",
                    ""id"": ""94ba8e0b-b6ef-4c2e-91dd-d81234129706"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Action4"",
                    ""type"": ""Button"",
                    ""id"": ""0a2d9c38-81df-4703-89b1-f7bd9384a188"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Action5"",
                    ""type"": ""PassThrough"",
                    ""id"": ""c03656bb-6268-4b05-899b-39efdec0f9dd"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Action6"",
                    ""type"": ""Button"",
                    ""id"": ""86aa0802-bd60-4d0b-9066-bdc6a0d8b463"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""ItemChangeRight"",
                    ""type"": ""Button"",
                    ""id"": ""d6a55b6c-7563-43bb-bd39-f24127da8c57"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""ItemChangeLeft"",
                    ""type"": ""Button"",
                    ""id"": ""e9d609dd-e035-491c-93f7-ca4dd4ef0b7b"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Quit"",
                    ""type"": ""Button"",
                    ""id"": ""7425de88-f98d-41e7-8ac1-ba7e47e403e0"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""ButtonStart"",
                    ""type"": ""Button"",
                    ""id"": ""027dbb4e-086a-4d74-a769-74bc77ffe60a"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Action7"",
                    ""type"": ""PassThrough"",
                    ""id"": ""e375c2fd-20a8-4be9-9371-284419750b33"",
                    ""expectedControlType"": ""Axis"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Action8"",
                    ""type"": ""Button"",
                    ""id"": ""330ef5a3-f7a2-40a5-906a-289e1136d786"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Subweapon Switch"",
                    ""type"": ""Button"",
                    ""id"": ""54cf426d-7f67-42ea-9943-d619012e5b4e"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Utility"",
                    ""type"": ""Button"",
                    ""id"": ""14c763c4-0f0e-40f0-b17b-2876f7d41109"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""978bfe49-cc26-4a3d-ab7b-7d7a29327403"",
                    ""path"": ""<Gamepad>/leftStick"",
                    ""interactions"": """",
                    ""processors"": ""StickDeadzone"",
                    ""groups"": "";Gamepad"",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""WASD"",
                    ""id"": ""00ca640b-d935-4593-8157-c05846ea39b3"",
                    ""path"": ""Dpad"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Move"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""up"",
                    ""id"": ""8180e8bd-4097-4f4e-ab88-4523101a6ce9"",
                    ""path"": ""<Keyboard>/upArrow"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": "";Keyboard&Mouse"",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""down"",
                    ""id"": ""1c5327b5-f71c-4f60-99c7-4e737386f1d1"",
                    ""path"": ""<Keyboard>/downArrow"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": "";Keyboard&Mouse"",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""left"",
                    ""id"": ""2e46982e-44cc-431b-9f0b-c11910bf467a"",
                    ""path"": ""<Keyboard>/leftArrow"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": "";Keyboard&Mouse"",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""right"",
                    ""id"": ""77bff152-3580-4b21-b6de-dcd0c7e41164"",
                    ""path"": ""<Keyboard>/rightArrow"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": "";Keyboard&Mouse"",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": """",
                    ""id"": ""0a3aac74-3050-4c67-9876-75dc69d9bb64"",
                    ""path"": ""<Gamepad>/buttonEast"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Gamepad"",
                    ""action"": ""Action3"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""7ba4e213-464a-4982-a76a-916eba9d6dc4"",
                    ""path"": ""<Keyboard>/leftShift"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard&Mouse"",
                    ""action"": ""Action3"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""4e6e2bba-9679-4449-8745-6d3f402aeeea"",
                    ""path"": ""<Gamepad>/buttonSouth"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Gamepad"",
                    ""action"": ""Action1"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""d5f46185-42ed-40b7-9dee-eadd5a901763"",
                    ""path"": ""<Keyboard>/space"",
                    ""interactions"": ""Hold(duration=0.15)"",
                    ""processors"": """",
                    ""groups"": ""Keyboard&Mouse"",
                    ""action"": ""Action1"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""1f3bacbf-f109-4248-b3e3-70cfb30d0cbd"",
                    ""path"": ""<Gamepad>/buttonNorth"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Gamepad"",
                    ""action"": ""Action4"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""ec7473ca-a596-41e2-9ee6-a4381d19bdb2"",
                    ""path"": ""<Keyboard>/a"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard&Mouse"",
                    ""action"": ""Action4"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""56091307-0007-4725-921a-a3f2fe3aa924"",
                    ""path"": ""<Gamepad>/rightShoulder"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Gamepad"",
                    ""action"": ""Action5"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""b559b90d-67a6-4467-ab3f-5a2db661f489"",
                    ""path"": ""<Keyboard>/s"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard&Mouse"",
                    ""action"": ""Action5"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""f1050d73-bb35-4033-bf95-cf9479a2c9ac"",
                    ""path"": ""<Gamepad>/dpad/up"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Gamepad"",
                    ""action"": ""Action6"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""a09d65b6-ed27-4290-b0d1-7d3ae22c8af7"",
                    ""path"": ""<Keyboard>/1"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard&Mouse"",
                    ""action"": ""Action6"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""67c3e0b6-41ac-4ad1-89e5-7d78539d525b"",
                    ""path"": ""<Keyboard>/e"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard&Mouse"",
                    ""action"": ""ItemChangeRight"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""f7e365a4-d751-458d-8f78-614a1444f6b3"",
                    ""path"": ""<Gamepad>/dpad/right"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Gamepad"",
                    ""action"": ""ItemChangeRight"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""a06140be-077a-45d4-a073-8888f9fff660"",
                    ""path"": ""<Keyboard>/q"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard&Mouse"",
                    ""action"": ""ItemChangeLeft"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""934ae8fe-e6f0-433d-8bef-0bfe968d740f"",
                    ""path"": ""<Gamepad>/dpad/left"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Gamepad"",
                    ""action"": ""ItemChangeLeft"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""43e3f5d3-92ab-4057-953f-b9806fb54e85"",
                    ""path"": ""<Gamepad>/buttonWest"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Gamepad"",
                    ""action"": ""Action2"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""8eb0103b-bc7c-4946-9ad5-60b657573b36"",
                    ""path"": ""<Keyboard>/d"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard&Mouse"",
                    ""action"": ""Action2"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""43ab7040-63e7-4cce-b720-053bc3cc333c"",
                    ""path"": ""<Gamepad>/start"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Gamepad"",
                    ""action"": ""ButtonStart"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""d90d6b46-96a5-49dc-824c-d87d3974bafa"",
                    ""path"": ""<Keyboard>/escape"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard&Mouse"",
                    ""action"": ""ButtonStart"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""c6eb8bc0-a890-401a-9ff3-664d254294e0"",
                    ""path"": ""<Keyboard>/p"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard&Mouse"",
                    ""action"": ""Quit"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""Keyboard"",
                    ""id"": ""f7ac1f51-086e-4d76-a081-057ebad9eafe"",
                    ""path"": ""1DAxis"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Action7"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""negative"",
                    ""id"": ""d883ceed-f9e5-46f4-91d2-9cb7f0878f7c"",
                    ""path"": ""<Keyboard>/q"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Action7"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""positive"",
                    ""id"": ""6b89483b-3c30-4f26-aa64-44c62808ea3f"",
                    ""path"": ""<Keyboard>/e"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard&Mouse"",
                    ""action"": ""Action7"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""DPad"",
                    ""id"": ""0eec683b-fffe-42f1-9f0d-c4c6cab602ce"",
                    ""path"": ""1DAxis"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Action7"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""negative"",
                    ""id"": ""da7642e3-8bc0-467b-931f-1b6063c65c8a"",
                    ""path"": ""<Gamepad>/dpad/left"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Gamepad"",
                    ""action"": ""Action7"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""positive"",
                    ""id"": ""fb9a7095-65ff-4a87-b7a2-9bfa4252eaab"",
                    ""path"": ""<Gamepad>/dpad/right"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Gamepad"",
                    ""action"": ""Action7"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": """",
                    ""id"": ""46e9751f-a0ba-4ec2-8d96-ee3515763c9d"",
                    ""path"": ""<Gamepad>/leftShoulder"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Gamepad"",
                    ""action"": ""Subweapon Switch"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""b6c8deeb-cd57-4595-9f76-01be607c3f7c"",
                    ""path"": ""<Keyboard>/tab"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard&Mouse"",
                    ""action"": ""Subweapon Switch"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""c4e597d9-f902-4707-9fb4-a6b06ebc973e"",
                    ""path"": ""<Keyboard>/r"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard&Mouse"",
                    ""action"": ""Utility"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""a7dac4fb-44be-4a23-8ae4-8d67adba3bb3"",
                    ""path"": ""<Gamepad>/rightTrigger"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Gamepad"",
                    ""action"": ""Utility"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""d6550cdb-52bf-45a3-bdca-58386ca8fd6b"",
                    ""path"": ""<Keyboard>/g"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard&Mouse"",
                    ""action"": ""Action8"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""e81b02fc-c22b-4d57-bc29-92b221693421"",
                    ""path"": ""<Gamepad>/leftTrigger"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Gamepad"",
                    ""action"": ""Action8"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        },
        {
            ""name"": ""UI"",
            ""id"": ""54c7f7d0-7755-41ae-84dd-16fe9e88de68"",
            ""actions"": [
                {
                    ""name"": ""Navigate"",
                    ""type"": ""Value"",
                    ""id"": ""5c46724c-57cd-4a58-a435-448cc4fc4b45"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Submit"",
                    ""type"": ""Button"",
                    ""id"": ""9ba9aab1-df87-485e-a0d2-34b03c21f4a1"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Cancel"",
                    ""type"": ""Button"",
                    ""id"": ""85e95929-fea8-4961-befc-9ec7c4d4bee5"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Point"",
                    ""type"": ""PassThrough"",
                    ""id"": ""f08e3d9d-e7d2-470b-bfd8-f89165f1f200"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Click"",
                    ""type"": ""PassThrough"",
                    ""id"": ""b3f83f66-9c8d-4ca9-b1f7-cc0836761f91"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""ScrollWheel"",
                    ""type"": ""PassThrough"",
                    ""id"": ""a9a209b7-d77e-407b-aefc-8ac0dd91301b"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""MiddleClick"",
                    ""type"": ""PassThrough"",
                    ""id"": ""d623adf0-638f-4ec1-8632-f68ca9c78ddc"",
                    ""expectedControlType"": """",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""RightClick"",
                    ""type"": ""PassThrough"",
                    ""id"": ""2cc535d0-9e4d-4a83-a9ae-f9df82ec3826"",
                    ""expectedControlType"": """",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""TrackedDevicePosition"",
                    ""type"": ""PassThrough"",
                    ""id"": ""0330664e-9f0b-41af-9f2d-f0ac90259566"",
                    ""expectedControlType"": ""Vector3"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""TrackedDeviceOrientation"",
                    ""type"": ""PassThrough"",
                    ""id"": ""aa7e2e07-6bd4-4813-9144-83cab8cf327d"",
                    ""expectedControlType"": ""Quaternion"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""ButtonStart"",
                    ""type"": ""Button"",
                    ""id"": ""da533f0e-10c4-4074-a8b4-d791d835974a"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                }
            ],
            ""bindings"": [
                {
                    ""name"": ""2D Vector"",
                    ""id"": ""93b69c03-f6a2-4b9d-a7c3-aa700a887453"",
                    ""path"": ""2DVector"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Navigate"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""up"",
                    ""id"": ""0b0bd4b4-6609-4b31-b19b-60033ef5e35a"",
                    ""path"": ""<Gamepad>/leftStick/up"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Gamepad"",
                    ""action"": ""Navigate"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""down"",
                    ""id"": ""7086d95c-cd6f-4965-a6cd-906df7d0c8e5"",
                    ""path"": ""<Gamepad>/leftStick/down"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Gamepad"",
                    ""action"": ""Navigate"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""left"",
                    ""id"": ""f0c0a72e-7c2c-41ec-abe3-40739653518e"",
                    ""path"": ""<Gamepad>/leftStick/left"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Gamepad"",
                    ""action"": ""Navigate"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""right"",
                    ""id"": ""a255f81a-deb6-4c67-9cce-2ff8d13df9c6"",
                    ""path"": ""<Gamepad>/leftStick/right"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Gamepad"",
                    ""action"": ""Navigate"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""2D Vector"",
                    ""id"": ""62a627cd-be17-4034-8f41-8c03dd54a948"",
                    ""path"": ""2DVector"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Navigate"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""up"",
                    ""id"": ""3dda3d57-92ff-423f-b255-787c9164ef81"",
                    ""path"": ""<Keyboard>/upArrow"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard&Mouse"",
                    ""action"": ""Navigate"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""down"",
                    ""id"": ""f0003081-5f95-4bef-a468-22a205df23a3"",
                    ""path"": ""<Keyboard>/downArrow"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard&Mouse"",
                    ""action"": ""Navigate"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""left"",
                    ""id"": ""df4734e6-40da-4e0c-a401-2824b2d6adc2"",
                    ""path"": ""<Keyboard>/leftArrow"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard&Mouse"",
                    ""action"": ""Navigate"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""right"",
                    ""id"": ""b94689b3-fc80-40c6-98b3-5bd55e49e57e"",
                    ""path"": ""<Keyboard>/rightArrow"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard&Mouse"",
                    ""action"": ""Navigate"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": """",
                    ""id"": ""9e92bb26-7e3b-4ec4-b06b-3c8f8e498ddc"",
                    ""path"": ""<Keyboard>/space"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard&Mouse"",
                    ""action"": ""Submit"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""033adbbd-4960-405a-9814-c54eb298f4ae"",
                    ""path"": ""<Gamepad>/buttonSouth"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Gamepad"",
                    ""action"": ""Submit"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""82627dcc-3b13-4ba9-841d-e4b746d6553e"",
                    ""path"": ""<Gamepad>/buttonEast"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Gamepad"",
                    ""action"": ""Cancel"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""469b58df-7bcc-4138-a51f-9fcf865cfc76"",
                    ""path"": ""<Keyboard>/backspace"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard&Mouse"",
                    ""action"": ""Cancel"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""c52c8e0b-8179-41d3-b8a1-d149033bbe86"",
                    ""path"": ""<Mouse>/position"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard&Mouse"",
                    ""action"": ""Point"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""e1394cbc-336e-44ce-9ea8-6007ed6193f7"",
                    ""path"": ""<Pen>/position"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard&Mouse"",
                    ""action"": ""Point"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""5693e57a-238a-46ed-b5ae-e64e6e574302"",
                    ""path"": ""<Touchscreen>/touch*/position"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Touch"",
                    ""action"": ""Point"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""4faf7dc9-b979-4210-aa8c-e808e1ef89f5"",
                    ""path"": ""<Mouse>/leftButton"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": "";Keyboard&Mouse"",
                    ""action"": ""Click"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""8d66d5ba-88d7-48e6-b1cd-198bbfef7ace"",
                    ""path"": ""<Pen>/tip"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": "";Keyboard&Mouse"",
                    ""action"": ""Click"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""47c2a644-3ebc-4dae-a106-589b7ca75b59"",
                    ""path"": ""<Touchscreen>/touch*/press"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Touch"",
                    ""action"": ""Click"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""bb9e6b34-44bf-4381-ac63-5aa15d19f677"",
                    ""path"": ""<XRController>/trigger"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""XR"",
                    ""action"": ""Click"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""38c99815-14ea-4617-8627-164d27641299"",
                    ""path"": ""<Mouse>/scroll"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": "";Keyboard&Mouse"",
                    ""action"": ""ScrollWheel"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""24066f69-da47-44f3-a07e-0015fb02eb2e"",
                    ""path"": ""<Mouse>/middleButton"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": "";Keyboard&Mouse"",
                    ""action"": ""MiddleClick"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""4c191405-5738-4d4b-a523-c6a301dbf754"",
                    ""path"": ""<Mouse>/rightButton"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": "";Keyboard&Mouse"",
                    ""action"": ""RightClick"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""7236c0d9-6ca3-47cf-a6ee-a97f5b59ea77"",
                    ""path"": ""<XRController>/devicePosition"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""XR"",
                    ""action"": ""TrackedDevicePosition"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""23e01e3a-f935-4948-8d8b-9bcac77714fb"",
                    ""path"": ""<XRController>/deviceRotation"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""XR"",
                    ""action"": ""TrackedDeviceOrientation"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""548c9917-0887-4117-948c-185c4302a4e6"",
                    ""path"": ""<Gamepad>/start"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Gamepad"",
                    ""action"": ""ButtonStart"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""30673067-11df-44cc-9edf-be209312f67e"",
                    ""path"": ""<Keyboard>/escape"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard&Mouse"",
                    ""action"": ""ButtonStart"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        },
        {
            ""name"": ""WeaponSelection"",
            ""id"": ""e5d7206c-f393-42ac-9205-fd3d5bb0dcde"",
            ""actions"": [
                {
                    ""name"": ""DPadUp"",
                    ""type"": ""Button"",
                    ""id"": ""7daf7340-99d9-4021-802f-8f1652e79e0a"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""DPadRight"",
                    ""type"": ""Button"",
                    ""id"": ""31bbe730-46b1-47aa-a5ca-07f722017ff5"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""DPadDown"",
                    ""type"": ""Button"",
                    ""id"": ""c04eedda-ca76-459c-a101-7cd84f19a79a"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""DPadLeft"",
                    ""type"": ""Button"",
                    ""id"": ""a0bb0e9f-92a4-4982-a764-baca7d7aaa1c"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""b5d71c31-76b8-4f3b-8c0b-180ca8e67392"",
                    ""path"": ""<Gamepad>/dpad/up"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""DPadUp"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""49fb1c5a-01a4-4da5-bf1a-8986ff04fdbb"",
                    ""path"": ""<Gamepad>/dpad/right"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""DPadRight"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""9837534f-149c-4adc-9c74-5adf5c588b06"",
                    ""path"": ""<Gamepad>/dpad/down"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""DPadDown"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""9347f95c-eaaa-458e-ba09-1681d4807b97"",
                    ""path"": ""<Gamepad>/dpad/left"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""DPadLeft"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        },
        {
            ""name"": ""GameManagement"",
            ""id"": ""49ec662b-2cc2-4633-bfb5-cbce482367fc"",
            ""actions"": [
                {
                    ""name"": ""Save"",
                    ""type"": ""Button"",
                    ""id"": ""d28b7b44-9255-4f60-87f7-52fc7d0bdac4"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Load"",
                    ""type"": ""Button"",
                    ""id"": ""06fe26ce-ce28-4830-b24c-c4772b2e22a9"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Delete"",
                    ""type"": ""Button"",
                    ""id"": ""21087661-25b9-4e0b-8a6e-bdc3a73cbf14"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""debd36a1-93a2-4b95-89ea-ed9dacf497ba"",
                    ""path"": ""<Keyboard>/j"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard&Mouse"",
                    ""action"": ""Save"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""b52367cd-d73d-4a92-a635-cb7d235dac6c"",
                    ""path"": ""<Keyboard>/k"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Load"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""fc27302d-cb73-47f3-9d8e-a9f1b7eaff05"",
                    ""path"": ""<Keyboard>/delete"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard&Mouse"",
                    ""action"": ""Delete"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        }
    ],
    ""controlSchemes"": [
        {
            ""name"": ""Keyboard&Mouse"",
            ""bindingGroup"": ""Keyboard&Mouse"",
            ""devices"": [
                {
                    ""devicePath"": ""<Keyboard>"",
                    ""isOptional"": false,
                    ""isOR"": false
                },
                {
                    ""devicePath"": ""<Mouse>"",
                    ""isOptional"": false,
                    ""isOR"": false
                }
            ]
        },
        {
            ""name"": ""Gamepad"",
            ""bindingGroup"": ""Gamepad"",
            ""devices"": [
                {
                    ""devicePath"": ""<Gamepad>"",
                    ""isOptional"": false,
                    ""isOR"": false
                }
            ]
        },
        {
            ""name"": ""Touch"",
            ""bindingGroup"": ""Touch"",
            ""devices"": [
                {
                    ""devicePath"": ""<Touchscreen>"",
                    ""isOptional"": false,
                    ""isOR"": false
                }
            ]
        },
        {
            ""name"": ""Joystick"",
            ""bindingGroup"": ""Joystick"",
            ""devices"": [
                {
                    ""devicePath"": ""<Joystick>"",
                    ""isOptional"": false,
                    ""isOR"": false
                }
            ]
        },
        {
            ""name"": ""XR"",
            ""bindingGroup"": ""XR"",
            ""devices"": [
                {
                    ""devicePath"": ""<XRController>"",
                    ""isOptional"": false,
                    ""isOR"": false
                }
            ]
        }
    ]
}");
        // Player
        m_Player = asset.FindActionMap("Player", throwIfNotFound: true);
        m_Player_Move = m_Player.FindAction("Move", throwIfNotFound: true);
        m_Player_Action1 = m_Player.FindAction("Action1", throwIfNotFound: true);
        m_Player_Action2 = m_Player.FindAction("Action2", throwIfNotFound: true);
        m_Player_Action3 = m_Player.FindAction("Action3", throwIfNotFound: true);
        m_Player_Action4 = m_Player.FindAction("Action4", throwIfNotFound: true);
        m_Player_Action5 = m_Player.FindAction("Action5", throwIfNotFound: true);
        m_Player_Action6 = m_Player.FindAction("Action6", throwIfNotFound: true);
        m_Player_ItemChangeRight = m_Player.FindAction("ItemChangeRight", throwIfNotFound: true);
        m_Player_ItemChangeLeft = m_Player.FindAction("ItemChangeLeft", throwIfNotFound: true);
        m_Player_Quit = m_Player.FindAction("Quit", throwIfNotFound: true);
        m_Player_ButtonStart = m_Player.FindAction("ButtonStart", throwIfNotFound: true);
        m_Player_Action7 = m_Player.FindAction("Action7", throwIfNotFound: true);
        m_Player_Action8 = m_Player.FindAction("Action8", throwIfNotFound: true);
        m_Player_SubweaponSwitch = m_Player.FindAction("Subweapon Switch", throwIfNotFound: true);
        m_Player_Utility = m_Player.FindAction("Utility", throwIfNotFound: true);
        // UI
        m_UI = asset.FindActionMap("UI", throwIfNotFound: true);
        m_UI_Navigate = m_UI.FindAction("Navigate", throwIfNotFound: true);
        m_UI_Submit = m_UI.FindAction("Submit", throwIfNotFound: true);
        m_UI_Cancel = m_UI.FindAction("Cancel", throwIfNotFound: true);
        m_UI_Point = m_UI.FindAction("Point", throwIfNotFound: true);
        m_UI_Click = m_UI.FindAction("Click", throwIfNotFound: true);
        m_UI_ScrollWheel = m_UI.FindAction("ScrollWheel", throwIfNotFound: true);
        m_UI_MiddleClick = m_UI.FindAction("MiddleClick", throwIfNotFound: true);
        m_UI_RightClick = m_UI.FindAction("RightClick", throwIfNotFound: true);
        m_UI_TrackedDevicePosition = m_UI.FindAction("TrackedDevicePosition", throwIfNotFound: true);
        m_UI_TrackedDeviceOrientation = m_UI.FindAction("TrackedDeviceOrientation", throwIfNotFound: true);
        m_UI_ButtonStart = m_UI.FindAction("ButtonStart", throwIfNotFound: true);
        // WeaponSelection
        m_WeaponSelection = asset.FindActionMap("WeaponSelection", throwIfNotFound: true);
        m_WeaponSelection_DPadUp = m_WeaponSelection.FindAction("DPadUp", throwIfNotFound: true);
        m_WeaponSelection_DPadRight = m_WeaponSelection.FindAction("DPadRight", throwIfNotFound: true);
        m_WeaponSelection_DPadDown = m_WeaponSelection.FindAction("DPadDown", throwIfNotFound: true);
        m_WeaponSelection_DPadLeft = m_WeaponSelection.FindAction("DPadLeft", throwIfNotFound: true);
        // GameManagement
        m_GameManagement = asset.FindActionMap("GameManagement", throwIfNotFound: true);
        m_GameManagement_Save = m_GameManagement.FindAction("Save", throwIfNotFound: true);
        m_GameManagement_Load = m_GameManagement.FindAction("Load", throwIfNotFound: true);
        m_GameManagement_Delete = m_GameManagement.FindAction("Delete", throwIfNotFound: true);
    }

    public void Dispose()
    {
        UnityEngine.Object.Destroy(asset);
    }

    public InputBinding? bindingMask
    {
        get => asset.bindingMask;
        set => asset.bindingMask = value;
    }

    public ReadOnlyArray<InputDevice>? devices
    {
        get => asset.devices;
        set => asset.devices = value;
    }

    public ReadOnlyArray<InputControlScheme> controlSchemes => asset.controlSchemes;

    public bool Contains(InputAction action)
    {
        return asset.Contains(action);
    }

    public IEnumerator<InputAction> GetEnumerator()
    {
        return asset.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public void Enable()
    {
        asset.Enable();
    }

    public void Disable()
    {
        asset.Disable();
    }

    // Player
    private readonly InputActionMap m_Player;
    private IPlayerActions m_PlayerActionsCallbackInterface;
    private readonly InputAction m_Player_Move;
    private readonly InputAction m_Player_Action1;
    private readonly InputAction m_Player_Action2;
    private readonly InputAction m_Player_Action3;
    private readonly InputAction m_Player_Action4;
    private readonly InputAction m_Player_Action5;
    private readonly InputAction m_Player_Action6;
    private readonly InputAction m_Player_ItemChangeRight;
    private readonly InputAction m_Player_ItemChangeLeft;
    private readonly InputAction m_Player_Quit;
    private readonly InputAction m_Player_ButtonStart;
    private readonly InputAction m_Player_Action7;
    private readonly InputAction m_Player_Action8;
    private readonly InputAction m_Player_SubweaponSwitch;
    private readonly InputAction m_Player_Utility;
    public struct PlayerActions
    {
        private @_Controller m_Wrapper;
        public PlayerActions(@_Controller wrapper) { m_Wrapper = wrapper; }
        public InputAction @Move => m_Wrapper.m_Player_Move;
        public InputAction @Action1 => m_Wrapper.m_Player_Action1;
        public InputAction @Action2 => m_Wrapper.m_Player_Action2;
        public InputAction @Action3 => m_Wrapper.m_Player_Action3;
        public InputAction @Action4 => m_Wrapper.m_Player_Action4;
        public InputAction @Action5 => m_Wrapper.m_Player_Action5;
        public InputAction @Action6 => m_Wrapper.m_Player_Action6;
        public InputAction @ItemChangeRight => m_Wrapper.m_Player_ItemChangeRight;
        public InputAction @ItemChangeLeft => m_Wrapper.m_Player_ItemChangeLeft;
        public InputAction @Quit => m_Wrapper.m_Player_Quit;
        public InputAction @ButtonStart => m_Wrapper.m_Player_ButtonStart;
        public InputAction @Action7 => m_Wrapper.m_Player_Action7;
        public InputAction @Action8 => m_Wrapper.m_Player_Action8;
        public InputAction @SubweaponSwitch => m_Wrapper.m_Player_SubweaponSwitch;
        public InputAction @Utility => m_Wrapper.m_Player_Utility;
        public InputActionMap Get() { return m_Wrapper.m_Player; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(PlayerActions set) { return set.Get(); }
        public void SetCallbacks(IPlayerActions instance)
        {
            if (m_Wrapper.m_PlayerActionsCallbackInterface != null)
            {
                @Move.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnMove;
                @Move.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnMove;
                @Move.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnMove;
                @Action1.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnAction1;
                @Action1.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnAction1;
                @Action1.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnAction1;
                @Action2.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnAction2;
                @Action2.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnAction2;
                @Action2.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnAction2;
                @Action3.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnAction3;
                @Action3.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnAction3;
                @Action3.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnAction3;
                @Action4.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnAction4;
                @Action4.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnAction4;
                @Action4.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnAction4;
                @Action5.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnAction5;
                @Action5.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnAction5;
                @Action5.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnAction5;
                @Action6.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnAction6;
                @Action6.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnAction6;
                @Action6.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnAction6;
                @ItemChangeRight.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnItemChangeRight;
                @ItemChangeRight.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnItemChangeRight;
                @ItemChangeRight.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnItemChangeRight;
                @ItemChangeLeft.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnItemChangeLeft;
                @ItemChangeLeft.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnItemChangeLeft;
                @ItemChangeLeft.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnItemChangeLeft;
                @Quit.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnQuit;
                @Quit.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnQuit;
                @Quit.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnQuit;
                @ButtonStart.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnButtonStart;
                @ButtonStart.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnButtonStart;
                @ButtonStart.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnButtonStart;
                @Action7.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnAction7;
                @Action7.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnAction7;
                @Action7.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnAction7;
                @Action8.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnAction8;
                @Action8.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnAction8;
                @Action8.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnAction8;
                @SubweaponSwitch.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnSubweaponSwitch;
                @SubweaponSwitch.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnSubweaponSwitch;
                @SubweaponSwitch.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnSubweaponSwitch;
                @Utility.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnUtility;
                @Utility.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnUtility;
                @Utility.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnUtility;
            }
            m_Wrapper.m_PlayerActionsCallbackInterface = instance;
            if (instance != null)
            {
                @Move.started += instance.OnMove;
                @Move.performed += instance.OnMove;
                @Move.canceled += instance.OnMove;
                @Action1.started += instance.OnAction1;
                @Action1.performed += instance.OnAction1;
                @Action1.canceled += instance.OnAction1;
                @Action2.started += instance.OnAction2;
                @Action2.performed += instance.OnAction2;
                @Action2.canceled += instance.OnAction2;
                @Action3.started += instance.OnAction3;
                @Action3.performed += instance.OnAction3;
                @Action3.canceled += instance.OnAction3;
                @Action4.started += instance.OnAction4;
                @Action4.performed += instance.OnAction4;
                @Action4.canceled += instance.OnAction4;
                @Action5.started += instance.OnAction5;
                @Action5.performed += instance.OnAction5;
                @Action5.canceled += instance.OnAction5;
                @Action6.started += instance.OnAction6;
                @Action6.performed += instance.OnAction6;
                @Action6.canceled += instance.OnAction6;
                @ItemChangeRight.started += instance.OnItemChangeRight;
                @ItemChangeRight.performed += instance.OnItemChangeRight;
                @ItemChangeRight.canceled += instance.OnItemChangeRight;
                @ItemChangeLeft.started += instance.OnItemChangeLeft;
                @ItemChangeLeft.performed += instance.OnItemChangeLeft;
                @ItemChangeLeft.canceled += instance.OnItemChangeLeft;
                @Quit.started += instance.OnQuit;
                @Quit.performed += instance.OnQuit;
                @Quit.canceled += instance.OnQuit;
                @ButtonStart.started += instance.OnButtonStart;
                @ButtonStart.performed += instance.OnButtonStart;
                @ButtonStart.canceled += instance.OnButtonStart;
                @Action7.started += instance.OnAction7;
                @Action7.performed += instance.OnAction7;
                @Action7.canceled += instance.OnAction7;
                @Action8.started += instance.OnAction8;
                @Action8.performed += instance.OnAction8;
                @Action8.canceled += instance.OnAction8;
                @SubweaponSwitch.started += instance.OnSubweaponSwitch;
                @SubweaponSwitch.performed += instance.OnSubweaponSwitch;
                @SubweaponSwitch.canceled += instance.OnSubweaponSwitch;
                @Utility.started += instance.OnUtility;
                @Utility.performed += instance.OnUtility;
                @Utility.canceled += instance.OnUtility;
            }
        }
    }
    public PlayerActions @Player => new PlayerActions(this);

    // UI
    private readonly InputActionMap m_UI;
    private IUIActions m_UIActionsCallbackInterface;
    private readonly InputAction m_UI_Navigate;
    private readonly InputAction m_UI_Submit;
    private readonly InputAction m_UI_Cancel;
    private readonly InputAction m_UI_Point;
    private readonly InputAction m_UI_Click;
    private readonly InputAction m_UI_ScrollWheel;
    private readonly InputAction m_UI_MiddleClick;
    private readonly InputAction m_UI_RightClick;
    private readonly InputAction m_UI_TrackedDevicePosition;
    private readonly InputAction m_UI_TrackedDeviceOrientation;
    private readonly InputAction m_UI_ButtonStart;
    public struct UIActions
    {
        private @_Controller m_Wrapper;
        public UIActions(@_Controller wrapper) { m_Wrapper = wrapper; }
        public InputAction @Navigate => m_Wrapper.m_UI_Navigate;
        public InputAction @Submit => m_Wrapper.m_UI_Submit;
        public InputAction @Cancel => m_Wrapper.m_UI_Cancel;
        public InputAction @Point => m_Wrapper.m_UI_Point;
        public InputAction @Click => m_Wrapper.m_UI_Click;
        public InputAction @ScrollWheel => m_Wrapper.m_UI_ScrollWheel;
        public InputAction @MiddleClick => m_Wrapper.m_UI_MiddleClick;
        public InputAction @RightClick => m_Wrapper.m_UI_RightClick;
        public InputAction @TrackedDevicePosition => m_Wrapper.m_UI_TrackedDevicePosition;
        public InputAction @TrackedDeviceOrientation => m_Wrapper.m_UI_TrackedDeviceOrientation;
        public InputAction @ButtonStart => m_Wrapper.m_UI_ButtonStart;
        public InputActionMap Get() { return m_Wrapper.m_UI; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(UIActions set) { return set.Get(); }
        public void SetCallbacks(IUIActions instance)
        {
            if (m_Wrapper.m_UIActionsCallbackInterface != null)
            {
                @Navigate.started -= m_Wrapper.m_UIActionsCallbackInterface.OnNavigate;
                @Navigate.performed -= m_Wrapper.m_UIActionsCallbackInterface.OnNavigate;
                @Navigate.canceled -= m_Wrapper.m_UIActionsCallbackInterface.OnNavigate;
                @Submit.started -= m_Wrapper.m_UIActionsCallbackInterface.OnSubmit;
                @Submit.performed -= m_Wrapper.m_UIActionsCallbackInterface.OnSubmit;
                @Submit.canceled -= m_Wrapper.m_UIActionsCallbackInterface.OnSubmit;
                @Cancel.started -= m_Wrapper.m_UIActionsCallbackInterface.OnCancel;
                @Cancel.performed -= m_Wrapper.m_UIActionsCallbackInterface.OnCancel;
                @Cancel.canceled -= m_Wrapper.m_UIActionsCallbackInterface.OnCancel;
                @Point.started -= m_Wrapper.m_UIActionsCallbackInterface.OnPoint;
                @Point.performed -= m_Wrapper.m_UIActionsCallbackInterface.OnPoint;
                @Point.canceled -= m_Wrapper.m_UIActionsCallbackInterface.OnPoint;
                @Click.started -= m_Wrapper.m_UIActionsCallbackInterface.OnClick;
                @Click.performed -= m_Wrapper.m_UIActionsCallbackInterface.OnClick;
                @Click.canceled -= m_Wrapper.m_UIActionsCallbackInterface.OnClick;
                @ScrollWheel.started -= m_Wrapper.m_UIActionsCallbackInterface.OnScrollWheel;
                @ScrollWheel.performed -= m_Wrapper.m_UIActionsCallbackInterface.OnScrollWheel;
                @ScrollWheel.canceled -= m_Wrapper.m_UIActionsCallbackInterface.OnScrollWheel;
                @MiddleClick.started -= m_Wrapper.m_UIActionsCallbackInterface.OnMiddleClick;
                @MiddleClick.performed -= m_Wrapper.m_UIActionsCallbackInterface.OnMiddleClick;
                @MiddleClick.canceled -= m_Wrapper.m_UIActionsCallbackInterface.OnMiddleClick;
                @RightClick.started -= m_Wrapper.m_UIActionsCallbackInterface.OnRightClick;
                @RightClick.performed -= m_Wrapper.m_UIActionsCallbackInterface.OnRightClick;
                @RightClick.canceled -= m_Wrapper.m_UIActionsCallbackInterface.OnRightClick;
                @TrackedDevicePosition.started -= m_Wrapper.m_UIActionsCallbackInterface.OnTrackedDevicePosition;
                @TrackedDevicePosition.performed -= m_Wrapper.m_UIActionsCallbackInterface.OnTrackedDevicePosition;
                @TrackedDevicePosition.canceled -= m_Wrapper.m_UIActionsCallbackInterface.OnTrackedDevicePosition;
                @TrackedDeviceOrientation.started -= m_Wrapper.m_UIActionsCallbackInterface.OnTrackedDeviceOrientation;
                @TrackedDeviceOrientation.performed -= m_Wrapper.m_UIActionsCallbackInterface.OnTrackedDeviceOrientation;
                @TrackedDeviceOrientation.canceled -= m_Wrapper.m_UIActionsCallbackInterface.OnTrackedDeviceOrientation;
                @ButtonStart.started -= m_Wrapper.m_UIActionsCallbackInterface.OnButtonStart;
                @ButtonStart.performed -= m_Wrapper.m_UIActionsCallbackInterface.OnButtonStart;
                @ButtonStart.canceled -= m_Wrapper.m_UIActionsCallbackInterface.OnButtonStart;
            }
            m_Wrapper.m_UIActionsCallbackInterface = instance;
            if (instance != null)
            {
                @Navigate.started += instance.OnNavigate;
                @Navigate.performed += instance.OnNavigate;
                @Navigate.canceled += instance.OnNavigate;
                @Submit.started += instance.OnSubmit;
                @Submit.performed += instance.OnSubmit;
                @Submit.canceled += instance.OnSubmit;
                @Cancel.started += instance.OnCancel;
                @Cancel.performed += instance.OnCancel;
                @Cancel.canceled += instance.OnCancel;
                @Point.started += instance.OnPoint;
                @Point.performed += instance.OnPoint;
                @Point.canceled += instance.OnPoint;
                @Click.started += instance.OnClick;
                @Click.performed += instance.OnClick;
                @Click.canceled += instance.OnClick;
                @ScrollWheel.started += instance.OnScrollWheel;
                @ScrollWheel.performed += instance.OnScrollWheel;
                @ScrollWheel.canceled += instance.OnScrollWheel;
                @MiddleClick.started += instance.OnMiddleClick;
                @MiddleClick.performed += instance.OnMiddleClick;
                @MiddleClick.canceled += instance.OnMiddleClick;
                @RightClick.started += instance.OnRightClick;
                @RightClick.performed += instance.OnRightClick;
                @RightClick.canceled += instance.OnRightClick;
                @TrackedDevicePosition.started += instance.OnTrackedDevicePosition;
                @TrackedDevicePosition.performed += instance.OnTrackedDevicePosition;
                @TrackedDevicePosition.canceled += instance.OnTrackedDevicePosition;
                @TrackedDeviceOrientation.started += instance.OnTrackedDeviceOrientation;
                @TrackedDeviceOrientation.performed += instance.OnTrackedDeviceOrientation;
                @TrackedDeviceOrientation.canceled += instance.OnTrackedDeviceOrientation;
                @ButtonStart.started += instance.OnButtonStart;
                @ButtonStart.performed += instance.OnButtonStart;
                @ButtonStart.canceled += instance.OnButtonStart;
            }
        }
    }
    public UIActions @UI => new UIActions(this);

    // WeaponSelection
    private readonly InputActionMap m_WeaponSelection;
    private IWeaponSelectionActions m_WeaponSelectionActionsCallbackInterface;
    private readonly InputAction m_WeaponSelection_DPadUp;
    private readonly InputAction m_WeaponSelection_DPadRight;
    private readonly InputAction m_WeaponSelection_DPadDown;
    private readonly InputAction m_WeaponSelection_DPadLeft;
    public struct WeaponSelectionActions
    {
        private @_Controller m_Wrapper;
        public WeaponSelectionActions(@_Controller wrapper) { m_Wrapper = wrapper; }
        public InputAction @DPadUp => m_Wrapper.m_WeaponSelection_DPadUp;
        public InputAction @DPadRight => m_Wrapper.m_WeaponSelection_DPadRight;
        public InputAction @DPadDown => m_Wrapper.m_WeaponSelection_DPadDown;
        public InputAction @DPadLeft => m_Wrapper.m_WeaponSelection_DPadLeft;
        public InputActionMap Get() { return m_Wrapper.m_WeaponSelection; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(WeaponSelectionActions set) { return set.Get(); }
        public void SetCallbacks(IWeaponSelectionActions instance)
        {
            if (m_Wrapper.m_WeaponSelectionActionsCallbackInterface != null)
            {
                @DPadUp.started -= m_Wrapper.m_WeaponSelectionActionsCallbackInterface.OnDPadUp;
                @DPadUp.performed -= m_Wrapper.m_WeaponSelectionActionsCallbackInterface.OnDPadUp;
                @DPadUp.canceled -= m_Wrapper.m_WeaponSelectionActionsCallbackInterface.OnDPadUp;
                @DPadRight.started -= m_Wrapper.m_WeaponSelectionActionsCallbackInterface.OnDPadRight;
                @DPadRight.performed -= m_Wrapper.m_WeaponSelectionActionsCallbackInterface.OnDPadRight;
                @DPadRight.canceled -= m_Wrapper.m_WeaponSelectionActionsCallbackInterface.OnDPadRight;
                @DPadDown.started -= m_Wrapper.m_WeaponSelectionActionsCallbackInterface.OnDPadDown;
                @DPadDown.performed -= m_Wrapper.m_WeaponSelectionActionsCallbackInterface.OnDPadDown;
                @DPadDown.canceled -= m_Wrapper.m_WeaponSelectionActionsCallbackInterface.OnDPadDown;
                @DPadLeft.started -= m_Wrapper.m_WeaponSelectionActionsCallbackInterface.OnDPadLeft;
                @DPadLeft.performed -= m_Wrapper.m_WeaponSelectionActionsCallbackInterface.OnDPadLeft;
                @DPadLeft.canceled -= m_Wrapper.m_WeaponSelectionActionsCallbackInterface.OnDPadLeft;
            }
            m_Wrapper.m_WeaponSelectionActionsCallbackInterface = instance;
            if (instance != null)
            {
                @DPadUp.started += instance.OnDPadUp;
                @DPadUp.performed += instance.OnDPadUp;
                @DPadUp.canceled += instance.OnDPadUp;
                @DPadRight.started += instance.OnDPadRight;
                @DPadRight.performed += instance.OnDPadRight;
                @DPadRight.canceled += instance.OnDPadRight;
                @DPadDown.started += instance.OnDPadDown;
                @DPadDown.performed += instance.OnDPadDown;
                @DPadDown.canceled += instance.OnDPadDown;
                @DPadLeft.started += instance.OnDPadLeft;
                @DPadLeft.performed += instance.OnDPadLeft;
                @DPadLeft.canceled += instance.OnDPadLeft;
            }
        }
    }
    public WeaponSelectionActions @WeaponSelection => new WeaponSelectionActions(this);

    // GameManagement
    private readonly InputActionMap m_GameManagement;
    private IGameManagementActions m_GameManagementActionsCallbackInterface;
    private readonly InputAction m_GameManagement_Save;
    private readonly InputAction m_GameManagement_Load;
    private readonly InputAction m_GameManagement_Delete;
    public struct GameManagementActions
    {
        private @_Controller m_Wrapper;
        public GameManagementActions(@_Controller wrapper) { m_Wrapper = wrapper; }
        public InputAction @Save => m_Wrapper.m_GameManagement_Save;
        public InputAction @Load => m_Wrapper.m_GameManagement_Load;
        public InputAction @Delete => m_Wrapper.m_GameManagement_Delete;
        public InputActionMap Get() { return m_Wrapper.m_GameManagement; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(GameManagementActions set) { return set.Get(); }
        public void SetCallbacks(IGameManagementActions instance)
        {
            if (m_Wrapper.m_GameManagementActionsCallbackInterface != null)
            {
                @Save.started -= m_Wrapper.m_GameManagementActionsCallbackInterface.OnSave;
                @Save.performed -= m_Wrapper.m_GameManagementActionsCallbackInterface.OnSave;
                @Save.canceled -= m_Wrapper.m_GameManagementActionsCallbackInterface.OnSave;
                @Load.started -= m_Wrapper.m_GameManagementActionsCallbackInterface.OnLoad;
                @Load.performed -= m_Wrapper.m_GameManagementActionsCallbackInterface.OnLoad;
                @Load.canceled -= m_Wrapper.m_GameManagementActionsCallbackInterface.OnLoad;
                @Delete.started -= m_Wrapper.m_GameManagementActionsCallbackInterface.OnDelete;
                @Delete.performed -= m_Wrapper.m_GameManagementActionsCallbackInterface.OnDelete;
                @Delete.canceled -= m_Wrapper.m_GameManagementActionsCallbackInterface.OnDelete;
            }
            m_Wrapper.m_GameManagementActionsCallbackInterface = instance;
            if (instance != null)
            {
                @Save.started += instance.OnSave;
                @Save.performed += instance.OnSave;
                @Save.canceled += instance.OnSave;
                @Load.started += instance.OnLoad;
                @Load.performed += instance.OnLoad;
                @Load.canceled += instance.OnLoad;
                @Delete.started += instance.OnDelete;
                @Delete.performed += instance.OnDelete;
                @Delete.canceled += instance.OnDelete;
            }
        }
    }
    public GameManagementActions @GameManagement => new GameManagementActions(this);
    private int m_KeyboardMouseSchemeIndex = -1;
    public InputControlScheme KeyboardMouseScheme
    {
        get
        {
            if (m_KeyboardMouseSchemeIndex == -1) m_KeyboardMouseSchemeIndex = asset.FindControlSchemeIndex("Keyboard&Mouse");
            return asset.controlSchemes[m_KeyboardMouseSchemeIndex];
        }
    }
    private int m_GamepadSchemeIndex = -1;
    public InputControlScheme GamepadScheme
    {
        get
        {
            if (m_GamepadSchemeIndex == -1) m_GamepadSchemeIndex = asset.FindControlSchemeIndex("Gamepad");
            return asset.controlSchemes[m_GamepadSchemeIndex];
        }
    }
    private int m_TouchSchemeIndex = -1;
    public InputControlScheme TouchScheme
    {
        get
        {
            if (m_TouchSchemeIndex == -1) m_TouchSchemeIndex = asset.FindControlSchemeIndex("Touch");
            return asset.controlSchemes[m_TouchSchemeIndex];
        }
    }
    private int m_JoystickSchemeIndex = -1;
    public InputControlScheme JoystickScheme
    {
        get
        {
            if (m_JoystickSchemeIndex == -1) m_JoystickSchemeIndex = asset.FindControlSchemeIndex("Joystick");
            return asset.controlSchemes[m_JoystickSchemeIndex];
        }
    }
    private int m_XRSchemeIndex = -1;
    public InputControlScheme XRScheme
    {
        get
        {
            if (m_XRSchemeIndex == -1) m_XRSchemeIndex = asset.FindControlSchemeIndex("XR");
            return asset.controlSchemes[m_XRSchemeIndex];
        }
    }
    public interface IPlayerActions
    {
        void OnMove(InputAction.CallbackContext context);
        void OnAction1(InputAction.CallbackContext context);
        void OnAction2(InputAction.CallbackContext context);
        void OnAction3(InputAction.CallbackContext context);
        void OnAction4(InputAction.CallbackContext context);
        void OnAction5(InputAction.CallbackContext context);
        void OnAction6(InputAction.CallbackContext context);
        void OnItemChangeRight(InputAction.CallbackContext context);
        void OnItemChangeLeft(InputAction.CallbackContext context);
        void OnQuit(InputAction.CallbackContext context);
        void OnButtonStart(InputAction.CallbackContext context);
        void OnAction7(InputAction.CallbackContext context);
        void OnAction8(InputAction.CallbackContext context);
        void OnSubweaponSwitch(InputAction.CallbackContext context);
        void OnUtility(InputAction.CallbackContext context);
    }
    public interface IUIActions
    {
        void OnNavigate(InputAction.CallbackContext context);
        void OnSubmit(InputAction.CallbackContext context);
        void OnCancel(InputAction.CallbackContext context);
        void OnPoint(InputAction.CallbackContext context);
        void OnClick(InputAction.CallbackContext context);
        void OnScrollWheel(InputAction.CallbackContext context);
        void OnMiddleClick(InputAction.CallbackContext context);
        void OnRightClick(InputAction.CallbackContext context);
        void OnTrackedDevicePosition(InputAction.CallbackContext context);
        void OnTrackedDeviceOrientation(InputAction.CallbackContext context);
        void OnButtonStart(InputAction.CallbackContext context);
    }
    public interface IWeaponSelectionActions
    {
        void OnDPadUp(InputAction.CallbackContext context);
        void OnDPadRight(InputAction.CallbackContext context);
        void OnDPadDown(InputAction.CallbackContext context);
        void OnDPadLeft(InputAction.CallbackContext context);
    }
    public interface IGameManagementActions
    {
        void OnSave(InputAction.CallbackContext context);
        void OnLoad(InputAction.CallbackContext context);
        void OnDelete(InputAction.CallbackContext context);
    }
}
