using System;

class MainClass {
    static void Main(string[] args)
    {
        Engine engine = Engine.GetInstance();
        engine.Run();
    }
}