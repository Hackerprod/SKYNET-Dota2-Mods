using System.Collections;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Resources;
using System.Runtime.CompilerServices;
using System.Runtime.ConstrainedExecution;
using System.Security;
using System.Threading;

internal sealed class ResourceHelper
{
    internal class GetResourceStringUserData
    {
        public ResourceHelper m_resourceHelper;

        public string m_key;

        public CultureInfo m_culture;

        public string m_retVal;

        public bool m_lockWasTaken;

        public GetResourceStringUserData(ResourceHelper resourceHelper, string key, CultureInfo culture)
        {
            m_resourceHelper = resourceHelper;
            m_key = key;
            m_culture = culture;
        }
    }

    private string m_name;

    private ResourceManager SystemResMgr;

    private Stack currentlyLoading;

    internal bool resourceManagerInited;

    private int infinitelyRecursingCount;

    internal ResourceHelper(string name)
    {
        m_name = name;
    }

    [ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail)]
    internal string GetResourceString(string key)
    {
        if (key == null || key.Length == 0)
        {
            return "[Resource lookup failed - null or empty resource name]";
        }
        return GetResourceString(key, null);
    }

    [SecuritySafeCritical]
    [ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail)]
    internal string GetResourceString(string key, CultureInfo culture)
    {
        if (key == null || key.Length == 0)
        {
            return "[Resource lookup failed - null or empty resource name]";
        }
        GetResourceStringUserData getResourceStringUserData = new GetResourceStringUserData(this, key, culture);
        RuntimeHelpers.TryCode code = GetResourceStringCode;
        RuntimeHelpers.CleanupCode backoutCode = GetResourceStringBackoutCode;
        RuntimeHelpers.ExecuteCodeWithGuaranteedCleanup(code, backoutCode, getResourceStringUserData);
        return getResourceStringUserData.m_retVal;
    }

    [SecuritySafeCritical]
    private void GetResourceStringCode(object userDataIn)
    {
        GetResourceStringUserData getResourceStringUserData = (GetResourceStringUserData)userDataIn;
        ResourceHelper resourceHelper = getResourceStringUserData.m_resourceHelper;
        string key = getResourceStringUserData.m_key;
        CultureInfo culture = getResourceStringUserData.m_culture;
        Monitor.Enter(resourceHelper, ref getResourceStringUserData.m_lockWasTaken);
        if (resourceHelper.currentlyLoading != null && resourceHelper.currentlyLoading.Count > 0 && resourceHelper.currentlyLoading.Contains(key))
        {
            if (resourceHelper.infinitelyRecursingCount > 0)
            {
                getResourceStringUserData.m_retVal = "[Resource lookup failed - infinite recursion or critical failure detected.]";
                return;
            }
            resourceHelper.infinitelyRecursingCount++;
            string message = "Infinite recursion during resource lookup within mscorlib.  This may be a bug in mscorlib, or potentially in certain extensibility points such as assembly resolve events or CultureInfo names.  Resource name: " + key;
            modCommon.WriteLine("[mscorlib recursive resource lookup bug] " + message);
            //FailFast(message);
        }
        if (resourceHelper.currentlyLoading == null)
        {
            resourceHelper.currentlyLoading = new Stack(4);
        }
        if (!resourceHelper.resourceManagerInited)
        {
            RuntimeHelpers.PrepareConstrainedRegions();
            try
            {
            }
            finally
            {
                RuntimeHelpers.RunClassConstructor(typeof(ResourceManager).TypeHandle);
                RuntimeHelpers.RunClassConstructor(typeof(ResourceReader).TypeHandle);
    //*            RuntimeHelpers.RunClassConstructor(typeof(RuntimeResourceSet).TypeHandle);
                RuntimeHelpers.RunClassConstructor(typeof(BinaryReader).TypeHandle);
                resourceHelper.resourceManagerInited = true;
            }
        }
        resourceHelper.currentlyLoading.Push(key);
        if (resourceHelper.SystemResMgr == null)
        {
            resourceHelper.SystemResMgr = new ResourceManager(m_name, typeof(object).Assembly);
        }
        string @string = resourceHelper.SystemResMgr.GetString(key, null);
        resourceHelper.currentlyLoading.Pop();
        getResourceStringUserData.m_retVal = @string;
    }

    [PrePrepareMethod]
    private void GetResourceStringBackoutCode(object userDataIn, bool exceptionThrown)
    {
        GetResourceStringUserData getResourceStringUserData = (GetResourceStringUserData)userDataIn;
        ResourceHelper resourceHelper = getResourceStringUserData.m_resourceHelper;
        if (exceptionThrown && getResourceStringUserData.m_lockWasTaken)
        {
            resourceHelper.SystemResMgr = null;
            resourceHelper.currentlyLoading = null;
        }
        if (getResourceStringUserData.m_lockWasTaken)
        {
            Monitor.Exit(resourceHelper);
        }
    }
}

