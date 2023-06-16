using UnityEngine;
namespace UnityAdvancedPlayerPrefs{
    public static class Test{
        static int total = 0, success = 0;
        static void Assert<T>(string testName, T actual, T expected, bool expectedEqual = true){
            ++total;
            if (actual.Equals(expected) == expectedEqual){
                Debug.Log($"Test \"{testName}\" yields good result. Expected " + (expectedEqual ? "" : "not ") + $"{expected}. Got {actual}");
                ++success;
            } else {
                Debug.LogWarning($"Test \"{testName}\" yields bad result. Expected " + (expectedEqual ? "" : "not ") + $"{expected}. Got {actual}");
                throw new System.Exception("Test failed.");
            }
        }
        [RuntimeInitializeOnLoadMethod]
        static void TestAll(){
            string prefix = TestUtils.RandomString(), password = TestUtils.RandomString();
            //Debug.Log(prefix);
            GameSettingManager gsm1 = new GameSettingManager(prefix, password);
            GameSettingManager gsm2 = new GameSettingManager(prefix, password);

            // TestPrefix(gsm1.SetBool, prefix, TestUtils.GenerateBoolTestValues());
            // TestPrefix(gsm1.SetColor32, prefix, TestUtils.GenerateColor32TestValues());
            // TestPrefix(gsm1.SetDouble, prefix, TestUtils.GenerateDoubleTestValues());
            // TestPrefix(gsm1.SetFloat, prefix, TestUtils.GenerateFloatTestValues());
            // TestPrefix(gsm1.SetInt, prefix, TestUtils.GenerateIntTestValues());
            // TestPrefix(gsm1.SetLong, prefix, TestUtils.GenerateLongTestValues());
            // TestPrefix(gsm1.SetString, prefix, TestUtils.GenerateStringTestValues());
            // TestPrefix(gsm1.SetTexture2D, prefix, TestUtils.GenerateTexture2DTestValues());

            TestConsistency(gsm1.SetBool, gsm2.GetBool, TestUtils.GenerateBoolTestValues());
            TestConsistency(gsm1.SetColor32, gsm2.GetColor32, TestUtils.GenerateColor32TestValues());
            TestConsistency(gsm1.SetDouble, gsm2.GetDouble, TestUtils.GenerateDoubleTestValues());
            TestConsistency(gsm1.SetFloat, gsm2.GetFloat, TestUtils.GenerateFloatTestValues());
            TestConsistency(gsm1.SetInt, gsm2.GetInt, TestUtils.GenerateIntTestValues());
            TestConsistency(gsm1.SetLong, gsm2.GetLong, TestUtils.GenerateLongTestValues());
            TestConsistency(gsm1.SetString, gsm2.GetString, TestUtils.GenerateStringTestValues());

            Debug.Log($"Test completed: {success} / {total} tests succeeded");
        }
        
        delegate void SetFunction<T> (string key, T value);
        delegate T GetFunction<T> (string key, T defaultValue);
        static void TestConsistency<T>(SetFunction<T> sf, GetFunction<T> gf, T[] testValues){
            string[] testKeys = TestUtils.GenerateStringTestValues();
            foreach (string key in testKeys)
            foreach (T value in testValues){
                sf.Invoke(key, value);
                Assert($"Test consistency {key} - {value}", gf.Invoke(key, default(T)), value);
            }
        }
        static void TestPrefix<T>(SetFunction<T> sf, string prefix, T[] testValues){
            string[] testKeys = TestUtils.GenerateStringTestValues();
            foreach (string key in testKeys)
            foreach (T value in testValues){
                sf.Invoke(key, value);
                Assert($"Test prefix {key} - {value}", PlayerPrefs.HasKey($"{prefix}_{key}_value") && PlayerPrefs.HasKey($"{prefix}_{key}_salt") , true);
            }
        }
    }
}