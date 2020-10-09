using SFML.Graphics;
using SFML.System;
using SFML.Window;
using System;
using static Util;

class Engine {

    public static Engine GetInstance()
    {
        if (engine == null) {
            engine = new Engine();
        }
        return engine;
    }
    private Engine()
    {
        window = new RenderWindow(
            new VideoMode(WIN_WIDTH, WIN_HEIGHT),
            "Holo",
            Styles.Close
            );

        window.SetVerticalSyncEnabled(true);
        
        SetEvents();

        font = new Font("font.ttf");
        texture = new Texture("img.png");

        InitArrays();
        Reset();
        RandomizePoints();
    }

    public void Run()
    {
        while (window.IsOpen) {
            window.DispatchEvents();
            Update();
            Render();
        }
    }

    private void SetEvents()
    {   
        //window closed event
        window.Closed += (sender, args) => window.Close();

        window.KeyPressed += (sender, args) =>
        {
            switch (args.Code) {
            case Keyboard.Key.Space:
                if (!started) {
                    RandomizePoints();
                    done = false;
                }
                break;

            case Keyboard.Key.Enter:
                if (!started) {
                    Reset();
                    started = true;
                }
                break;

            case Keyboard.Key.X:
                if (started) {
                    Reset();
                }
                break;

            case Keyboard.Key.Up:
                if (!started) {
                    ++pointCount;
                    if (pointCount > 12) {
                        pointCount = 12;
                    }
                    else {
                        InitArrays();
                        Reset();
                        RandomizePoints();
                    }
                }
                break;

            case Keyboard.Key.Down:
                if (!started) {
                    --pointCount;
                    if (pointCount < 4) {
                        pointCount = 4;
                    }
                    else {
                        InitArrays();
                        Reset();
                        RandomizePoints();
                    }
                }
                break;

            default:
                break;
            }
        };
    }

    private void Update()
    {
        //if new best is found, we can break out of update loop
        //so that new best is visible
        bool newBestFound = false;

        //a formula I came up with to scale iterations per frame
        //with the no. of points
        // basically I increase iterations per frame proportional to O(n!)
       
        int itersPerFrame = 1;
        if (pointCount > 5) itersPerFrame = ((Factorial(pointCount) / Factorial(5)) / 4);

        for (int i = 1; i <= itersPerFrame && !newBestFound; ++i) { //repeat perms 1000 times / frame
            if (!done && started) {
                float distance = TotalDistance(points, currentOrder); //compute distance of curr point order
                ++checkedCount;

                if (distance < leastDistance) { //if distance of curr Order is less than least dist so far
                    leastDistance = distance;

                    bestOrder = currentOrder.Clone() as int[];  //current Order is now best order
                    newBestFound = true;
                }
                if (!NextPerm(currentOrder)) {
                    done = true;
                    started = false;
                }
            }
        }
    }

    private void Render()
    {
        window.Clear(new Color(150, 75, 0));

        //Draw Line halfway between window
        Vertex[] partition = {
            new Vertex(new Vector2f(WIN_WIDTH / 2, 0.0f), Color.Red),
            new Vertex(new Vector2f(WIN_WIDTH / 2, WIN_HEIGHT), Color.Red)
        };

        window.Draw(partition, PrimitiveType.LineStrip);

        //draw lines joining points
        if (started || done) {
            DrawLines();
        }

        DrawPoints();

        double percentage = ((double)checkedCount / totalPerms) * 100.0;

        Text percentageText = new Text("Progress: " + percentage.ToString("0.###"), font) {
            CharacterSize = 25,
            FillColor = Color.Cyan,
            Position = new Vector2f(50.0f, 0.0f)
        };

        Text distanceText = new Text("Distance: " + leastDistance.ToString("0.###") + " pixels^2", font) {
            CharacterSize = 25,
            FillColor = Color.Cyan,
            Position = new Vector2f(WIN_WIDTH / 2 + 50, 0.0f)
        };


        Text countText = new Text("Point Count: " + pointCount, font) {
            CharacterSize = 25,
            FillColor = Color.Cyan,
            Position = new Vector2f(400.0f, 0.0f)
        };

        window.Draw(percentageText);
        window.Draw(distanceText);
        window.Draw(countText);

        if (done) {
            Sprite sprite = new Sprite(texture) {
                Scale = new Vector2f(0.7f, 0.7f),
                Position = new Vector2f(100.0f, 100.0f)
            };

            window.Draw(sprite);

            var bounds = sprite.GetGlobalBounds();

            Text doneText = new Text("Done!", font) {
                CharacterSize = 50,
                FillColor = Color.Cyan,
                Style = Text.Styles.Bold,
                Position = sprite.Position + new Vector2f(bounds.Width / 2 - 50.0f, bounds.Height)
            };

            window.Draw(doneText);

            
        }

        window.Display();
    }

    //function to call if point size is changed
    private void InitArrays()
    {
        points = new Vector2f[pointCount];
        currentOrder = new int[pointCount];

        totalPerms = Factorial(points.Length);
    }

    private void Reset()
    {
        //resets order to {0, 1, 2, 3, ...}
        for (int i = 0; i < currentOrder.Length; ++i) {
            currentOrder[i] = i;
        }

        checkedCount = 0;
        bestOrder = currentOrder.Clone() as int[];
        done = false;
        started = false;
        leastDistance = float.PositiveInfinity;
    }

    private void RandomizePoints()
    {
        Random ranGen = new Random();
        for (int i = 0; i < points.Length; ++i) {
            points[i].X = ranGen.Next(PADDING_X, WIN_WIDTH / 2 - PADDING_X);
            points[i].Y = ranGen.Next(PADDING_Y, WIN_HEIGHT - 1 - PADDING_Y);
        }
    }

    private void DrawPoints()
    {
        CircleShape circleShape = new CircleShape();

        for (int i = 0; i < points.Length; ++i) {
            if (i == 0) { //for starting point, color and size different
                circleShape.FillColor = Color.Green;
                circleShape.Radius = POINT_RAD * 1.5f;
            }
            else { //for all others
                circleShape.FillColor = Color.Yellow;
                circleShape.Radius = POINT_RAD;
            }

            circleShape.Origin = new Vector2f(circleShape.Radius, circleShape.Radius);

            //for left half
            if (!done) {
                circleShape.Position = points[currentOrder[i]];
                window.Draw(circleShape);
            }

            circleShape.Position = points[bestOrder[i]];
            circleShape.Position += new Vector2f(WIN_WIDTH / 2, 0); //translate points to right half
            window.Draw(circleShape);
        }
    }

    private void DrawLines()
    {
        if (!done) {
            Vertex[] leftVerices = new Vertex[points.Length + 1];


            for (int i = 0; i < points.Length; ++i) {
                leftVerices[i] = new Vertex(points[currentOrder[i]], Color.White);
            }
            leftVerices[points.Length] = leftVerices[0];

            window.Draw(leftVerices, PrimitiveType.LineStrip);
        }
     
        Vertex[] rightVertices = new Vertex[points.Length + 1];

        for (int i = 0; i < points.Length; ++i) {
            Vector2f position = points[bestOrder[i]];
            position += new Vector2f(WIN_WIDTH / 2, 0);
            rightVertices[i] = new Vertex(position, Color.Green);
        }

        rightVertices[points.Length] = rightVertices[0];
        window.Draw(rightVertices, PrimitiveType.LineStrip);
    }

    static Engine engine = null;

    RenderWindow window;
    Texture texture;
    Font font;

    Vector2f[] points;
    int[] currentOrder;
    int[] bestOrder;
    float leastDistance;
   
    bool done;
    bool started;

    int checkedCount;
    int totalPerms;
    int pointCount = 5;
    
}