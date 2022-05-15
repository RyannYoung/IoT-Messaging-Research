using System.Reflection;
using Spectre.Console;
using TestManager;


/*
 * This is broken and I never got it working
 * Would have been nice to build the system from the ground up utilising this,
 * instead of attempting to add it in last minute :(
 */
var testManager = new TestManager.TestManager();

testManager.RunApp();
