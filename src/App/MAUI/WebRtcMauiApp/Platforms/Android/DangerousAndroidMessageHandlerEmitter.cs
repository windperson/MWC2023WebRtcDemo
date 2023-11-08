#if DEBUG // Ensure this never leaves debug stages.
using System.Reflection.Emit;
using System.Reflection;

using Javax.Net.Ssl;

using Xamarin.Android.Net;

namespace WebRtcMauiApp.Platforms.Android
{
    static class DangerousAndroidMessageHandlerEmitter
    {
        private static Assembly _emittedAssembly = null;

        public static void Register(string handlerTypeName = "DangerousAndroidMessageHandler", string assemblyName = "DangerousAndroidMessageHandler")
        {
            AppDomain.CurrentDomain.AssemblyResolve += (s, e) =>
            {
                if (e.Name == assemblyName)
                {
                    if (_emittedAssembly == null)
                    {
                        _emittedAssembly = Emit(handlerTypeName, assemblyName);
                    }

                    return _emittedAssembly;
                }
                return null;
            };
        }

        private static AssemblyBuilder Emit(string handlerTypeName, string assemblyName)
        {
            var assembly = AssemblyBuilder.DefineDynamicAssembly(new AssemblyName(assemblyName), AssemblyBuilderAccess.Run);
            var module = assembly.DefineDynamicModule(assemblyName);

            DefineDangerousAndroidMessageHandler(module, handlerTypeName);

            return assembly;
        }

        private static void DefineDangerousAndroidMessageHandler(ModuleBuilder module, string handlerTypeName)
        {
            var typeBuilder = module.DefineType(handlerTypeName, TypeAttributes.Public);
            typeBuilder.SetParent(typeof(AndroidMessageHandler));
            typeBuilder.DefineDefaultConstructor(MethodAttributes.Public);

            var methodBuilder = typeBuilder.DefineMethod(
                "GetSSLHostnameVerifier",
                MethodAttributes.Public | MethodAttributes.Virtual,
                typeof(IHostnameVerifier),
                new[] { typeof(HttpsURLConnection) }
            );

            var generator = methodBuilder.GetILGenerator();
            generator.Emit(OpCodes.Call, typeof(DangerousHostNameVerifier).GetMethod("Create"));
            generator.Emit(OpCodes.Ret);

            typeBuilder.CreateType();
        }
    }

    public class DangerousHostNameVerifier : Java.Lang.Object, IHostnameVerifier
    {
        public bool Verify(string hostname, ISSLSession session)
        {
            return true;
        }

        public static IHostnameVerifier Create() => new DangerousHostNameVerifier();
    }
}
#endif
