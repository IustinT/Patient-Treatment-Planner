using System;
using Android.App;
using Android.Runtime;

using Shiny;

namespace ICU.Planner.Droid
{ 
#if DEBUG
    [Application(Debuggable = true,
        Theme = "@style/MainTheme")]
#else
        [Application(Debuggable = false,
        Theme = "@style/MainTheme")]
#endif 
    public class MainApplication : Application
    {
        public MainApplication(IntPtr javaReference, JniHandleOwnership transfer)
            : base(javaReference, transfer)
        {
        }

        public override void OnCreate()
        {
            base.OnCreate();
            this.ShinyOnCreate(new Startup());
        }
    }
}
