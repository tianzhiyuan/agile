using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security;
using System.Security.Permissions;
using System.Security.Policy;
using System.Web.Razor;
using Agile.Common.Exceptions;
using Agile.Common.Security;
using RazorEngine.Templating;

namespace Agile.Framework.Email
{
	public class RazorRenderer : ITemplateRenderer
	{
		public string Parse<T>(string template, T model, bool isHtml = true)
		{
            try
            {
                return Razor.RunCompile(template, template.Hash(), model?.GetType(), model);
            }
            catch (TemplateParsingException error)
            {
                throw new BusinessException("模版配置错误，template:" + template, error);
            }
        }

	    public string Parse(string template, object model)
	    {
            try
            {
                return Razor.RunCompile(template, template.Hash(), model?.GetType(), model);
            }
            catch (TemplateParsingException error)
            {
                throw new BusinessException("模版配置错误，template:" + template, error);
            }
        }

        private static IRazorEngineService Razor
        {
            get
            {
                if (_razorEngine == null)
                {
                    lock (_lock)
                    {
                        if (_razorEngine == null)
                        {
                            _razorDomain = SandboxCreator();
                            _razorEngine = IsolatedRazorEngineService.Create(SandboxCreator);
                        }
                    }
                }
                return _razorEngine;
            }
        }

        private static readonly object _lock = new object();
        private static AppDomain _razorDomain;
        private static IRazorEngineService _razorEngine;
        private static AppDomain SandboxCreator()
        {
            Evidence ev = new Evidence();
            ev.AddHostEvidence(new Zone(SecurityZone.Internet));
            PermissionSet permSet = new PermissionSet(PermissionState.None);
            permSet.AddPermission(new SecurityPermission(SecurityPermissionFlag.Execution));
            permSet.AddPermission(new SecurityPermission(SecurityPermissionFlag.RemotingConfiguration));
            permSet.AddPermission(new ReflectionPermission(ReflectionPermissionFlag.RestrictedMemberAccess));	// support dynamic
            // We have to load ourself with full trust
            StrongName razorEngineAssembly = typeof(RazorEngineService).Assembly.Evidence.GetHostEvidence<StrongName>();
            // We have to load Razor with full trust (so all methods are SecurityCritical)
            // This is because we apply AllowPartiallyTrustedCallers to RazorEngine, because
            // We need the untrusted (transparent) code to be able to inherit TemplateBase.
            // Because in the normal environment/appdomain we run as full trust and the Razor assembly has no security attributes
            // it will be completely SecurityCritical. 
            // This means we have to mark a lot of our members SecurityCritical (which is fine).
            // However in the sandbox domain we have partial trust and because razor has no Security attributes that means the
            // code will be transparent (this is where we get a lot of exceptions, because we now have different security attributes)
            // To work around this we give Razor full trust in the sandbox as well.
            StrongName razorAssembly = typeof(RazorTemplateEngine).Assembly.Evidence.GetHostEvidence<StrongName>();
            AppDomainSetup adSetup = new AppDomainSetup();
            adSetup.ApplicationBase = AppDomain.CurrentDomain.SetupInformation.ApplicationBase + "bin\\";
            if (AppDomain.CurrentDomain.SetupInformation.ShadowCopyFiles == "true")
            {
                var shadowCopyDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                if (shadowCopyDir.Contains("assembly"))
                    shadowCopyDir = shadowCopyDir.Substring(0, shadowCopyDir.LastIndexOf("assembly"));

                var privatePaths = new List<string>();
                foreach (var assemblyLocation in AppDomain.CurrentDomain.GetAssemblies().Where(a => !a.IsDynamic && a.Location.StartsWith(shadowCopyDir)).Select(a => a.Location))
                    privatePaths.Add(Path.GetDirectoryName(assemblyLocation));

                adSetup.ApplicationBase = shadowCopyDir;
                adSetup.PrivateBinPath = String.Join(";", privatePaths);
            }
            else
            {
                adSetup.ApplicationBase = AppDomain.CurrentDomain.SetupInformation.ApplicationBase;
                adSetup.PrivateBinPath = AppDomain.CurrentDomain.SetupInformation.PrivateBinPath;

            }
            AppDomain newDomain = AppDomain.CreateDomain("Sandbox", null, adSetup, permSet, razorEngineAssembly, razorAssembly);
            return newDomain;
        }
    }
}
