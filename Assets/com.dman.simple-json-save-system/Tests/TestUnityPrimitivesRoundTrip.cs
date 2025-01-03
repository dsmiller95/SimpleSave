using System;
using NUnit.Framework;
using UnityEngine;
using static Dman.SimpleJson.Tests.SaveDataTestUtils;

namespace Dman.SimpleJson.Tests
{
    [Serializable]
    public struct AllUnityPrimitives
    {
        public Vector2 testVector2;
        public Vector3 testVector3;
        public Vector4 testVector4;
        public Vector2Int testVector2Int;
        public Vector3Int testVector3Int;
        public Quaternion testQuaternion;
        public Matrix4x4 testMatrix4x4;
        public Color testColor;
        public Color32 testColor32;
        public LayerMask testLayerMask;
        
        // TODO: handle these types, currently not serialized
        public Rect testRect;
        public AnimationCurve testAnimationCurve;
        public Gradient testGradient;
    }
    
    public class TestUnityPrimitivesRoundTrip
    {
        [Test]
        public void WhenSavedPrimitiveTypes_SavesJson()
        {
            // arrange
            var savedData = new AllUnityPrimitives
            {
                testVector2 = new Vector2(1.1f, 1.2f),
                testVector3 = new Vector3(2.1f, 2.2f, 2.3f),
                testVector4 = new Vector4(3.1f, 3.2f, 3.3f, 3.4f),
                testVector2Int = new Vector2Int(8, 10),
                testVector3Int = new Vector3Int(19, 77, 7),
                testQuaternion = new Quaternion(4.1f, 4.2f, 4.3f, 4.4f),
                testMatrix4x4 = new Matrix4x4(Vector4.one, Vector4.zero, new Vector4(1,1,0,0), new Vector4(1,0,1,0)),
                testColor = new Color(5.1f, 5.2f, 5.3f, 5.4f),
                testColor32 = new Color32(100, 120, 130, 150),
                testRect = new Rect(6.6f, 6.7f, 6.8f, 6.9f),
                testLayerMask = (LayerMask)0b00100110,
                testAnimationCurve = AnimationCurve.EaseInOut(0, 3, 2, 9),
                testGradient = new Gradient(),
            };
            var expectedSavedString = @"
{
  ""unityPrimitives"": {
    ""testVector2"": {
      ""x"": 1.1000000238418579,
      ""y"": 1.2000000476837158
    },
    ""testVector3"": {
      ""x"": 2.0999999046325684,
      ""y"": 2.2000000476837158,
      ""z"": 2.2999999523162842
    },
    ""testVector4"": {
      ""x"": 3.0999999046325684,
      ""y"": 3.2000000476837158,
      ""z"": 3.2999999523162842,
      ""w"": 3.4000000953674316
    },
    ""testVector2Int"": {
      ""x"": 8,
      ""y"": 10
    },
    ""testVector3Int"": {
      ""x"": 19,
      ""y"": 77,
      ""z"": 7
    },
    ""testQuaternion"": {
      ""x"": 4.0999999046325684,
      ""y"": 4.1999998092651367,
      ""z"": 4.3000001907348633,
      ""w"": 4.4000000953674316
    },
    ""testMatrix4x4"": {
      ""e00"": 1.0,
      ""e01"": 0.0,
      ""e02"": 1.0,
      ""e03"": 1.0,
      ""e10"": 1.0,
      ""e11"": 0.0,
      ""e12"": 1.0,
      ""e13"": 0.0,
      ""e20"": 1.0,
      ""e21"": 0.0,
      ""e22"": 0.0,
      ""e23"": 1.0,
      ""e30"": 1.0,
      ""e31"": 0.0,
      ""e32"": 0.0,
      ""e33"": 0.0
    },
    ""testColor"": {
      ""r"": 5.0999999046325684,
      ""g"": 5.1999998092651367,
      ""b"": 5.3000001907348633,
      ""a"": 5.4000000953674316
    },
    ""testColor32"": {
      ""r"": 100,
      ""g"": 120,
      ""b"": 130,
      ""a"": 150
    },
    ""testLayerMask"": {
      ""serializedVersion"": ""2"",
      ""m_Bits"": 38
    },
    ""testRect"": {
      ""serializedVersion"": ""2"",
      ""x"": 6.5999999046325684,
      ""y"": 6.6999998092651367,
      ""width"": 6.8000001907348633,
      ""height"": 6.9000000953674316
    },
    ""testAnimationCurve"": {
      ""serializedVersion"": ""2"",
      ""m_Curve"": [
        {
          ""serializedVersion"": ""3"",
          ""time"": 0.0,
          ""value"": 3.0,
          ""inSlope"": 0.0,
          ""outSlope"": 0.0,
          ""tangentMode"": 0,
          ""weightedMode"": 0,
          ""inWeight"": 0.0,
          ""outWeight"": 0.0
        },
        {
          ""serializedVersion"": ""3"",
          ""time"": 2.0,
          ""value"": 9.0,
          ""inSlope"": 0.0,
          ""outSlope"": 0.0,
          ""tangentMode"": 0,
          ""weightedMode"": 0,
          ""inWeight"": 0.0,
          ""outWeight"": 0.0
        }
      ],
      ""m_PreInfinity"": 2,
      ""m_PostInfinity"": 2,
      ""m_RotationOrder"": 4
    },
    ""testGradient"": {
      ""serializedVersion"": ""2"",
      ""key0"": {
        ""r"": 1.0,
        ""g"": 1.0,
        ""b"": 1.0,
        ""a"": 1.0
      },
      ""key1"": {
        ""r"": 1.0,
        ""g"": 1.0,
        ""b"": 1.0,
        ""a"": 1.0
      },
      ""key2"": {
        ""r"": 0.0,
        ""g"": 0.0,
        ""b"": 0.0,
        ""a"": 0.0
      },
      ""key3"": {
        ""r"": 0.0,
        ""g"": 0.0,
        ""b"": 0.0,
        ""a"": 0.0
      },
      ""key4"": {
        ""r"": 0.0,
        ""g"": 0.0,
        ""b"": 0.0,
        ""a"": 0.0
      },
      ""key5"": {
        ""r"": 0.0,
        ""g"": 0.0,
        ""b"": 0.0,
        ""a"": 0.0
      },
      ""key6"": {
        ""r"": 0.0,
        ""g"": 0.0,
        ""b"": 0.0,
        ""a"": 0.0
      },
      ""key7"": {
        ""r"": 0.0,
        ""g"": 0.0,
        ""b"": 0.0,
        ""a"": 0.0
      },
      ""ctime0"": 0,
      ""ctime1"": 65535,
      ""ctime2"": 0,
      ""ctime3"": 0,
      ""ctime4"": 0,
      ""ctime5"": 0,
      ""ctime6"": 0,
      ""ctime7"": 0,
      ""atime0"": 0,
      ""atime1"": 65535,
      ""atime2"": 0,
      ""atime3"": 0,
      ""atime4"": 0,
      ""atime5"": 0,
      ""atime6"": 0,
      ""atime7"": 0,
      ""m_Mode"": 0,
      ""m_ColorSpace"": -1,
      ""m_NumColorKeys"": 2,
      ""m_NumAlphaKeys"": 2
    }
  }
}
".Trim();
            // act
            string serializedString = SerializeToString(TokenMode.SerializableObject, 
                assertInternalRoundTrip: false,
                ("unityPrimitives", savedData));
            
            // assert
            AssertDeserializeWithoutError( 
                serializedString,
                TokenMode.SerializableObject,
                ("unityPrimitives", savedData));
            AssertMultilineStringEqual(expectedSavedString, serializedString);
        }
    }
}
