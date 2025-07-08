#if UNITY_ANDROID
namespace Mycom.Target.Unity.Editor
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using UnityEditor.Android;

    public sealed class MyTargetAndroidPostProcessor : IPostGenerateGradleAndroidProject
    {
        public Int32 callbackOrder => Int32.MaxValue;

        public void OnPostGenerateGradleAndroidProject(string path)
        {
            var gradlePropertiesPath = Path.Combine(path, "../gradle.properties");
            var newProperties = new List<string>();

            if (File.Exists(gradlePropertiesPath))
            {
                newProperties.AddRange(File.ReadAllLines(gradlePropertiesPath).Where(line => !line.Contains("android.useFullClasspathForDexingTransform")));
            }

            newProperties.Add("android.useFullClasspathForDexingTransform=true");

            try
            {
                File.WriteAllText(gradlePropertiesPath, string.Join("\n", newProperties.ToArray()));
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }
    }
}

#endif