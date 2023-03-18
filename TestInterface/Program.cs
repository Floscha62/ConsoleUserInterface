using ConsoleUserInterface.Core;
using ConsoleUserInterface.Core.Components;

var application = Components.Container(Layout.VERTICAL, 0, 0, Console.WindowWidth, Console.WindowHeight, 
    Components.Label(ITransform.Create(1), "Text Area Input", underlined: true),
    Components.TextArea(ITransform.Create(10), "", _ => { }, 100)
);
var renderer = new Renderer(application, logger: ILogger.File("TestProgram.log"));
renderer.Start();