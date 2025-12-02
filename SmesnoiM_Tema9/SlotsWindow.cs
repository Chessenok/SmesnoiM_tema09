using System;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;
using System.IO;
using StbImageSharp;

public class SlotsWindow : GameWindow
{

    private const float FRAME_WIDTH = 400f;
    private const float FRAME_HEIGHT = 250f;
    private const float SLOT_SIZE = 80f;
    private const float SLOT_SPACING = 100f;
    private const float BUTTON_WIDTH = 150f;
    private const float BUTTON_HEIGHT = 40f;

    private Texture _texFrame;
    private Texture _texButton;

    private Texture[] _symbolTextures;

    private int[] _currentSlotIndex = new int[] { 0, 0, 0 };
    private Random _random = new Random();

    private bool _isSpinning = false;
    private int _cyclesToSpin = 5;
    private int _remainingCycles;
    private const float CYCLE_TIME = 0.5f;
    private float _timeUntilNextCycle = 0f;

    public SlotsWindow() : base(800, 600, GraphicsMode.Default)
    {
        VSync = VSyncMode.On;
    }
    

    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);
        GL.ClearColor(0.2f, 0.3f, 0.3f, 1.0f);

        GL.Enable(EnableCap.Blend);
        GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);
        GL.Enable(EnableCap.Texture2D);

        // Încărcare Texturi
        try
        {
            _texFrame = new Texture("frame.png");
            _texButton = new Texture("button.png");

            _symbolTextures = new Texture[] {
                new Texture("seven.png"),
                new Texture("cherry.png"),
                new Texture("bar.png"),
                new Texture("limon.png")
            };
        }
        catch (Exception ex)
        {
            Console.WriteLine("Eroare la încărcarea texturilor: " + ex.Message);
        }
    }

    protected override void OnResize(EventArgs e)
    {
        base.OnResize(e);
        GL.Viewport(0, 0, Width, Height);
        GL.MatrixMode(MatrixMode.Projection);
        GL.LoadIdentity();
        GL.Ortho(0, Width, 0, Height, -1.0, 1.0);
        GL.MatrixMode(MatrixMode.Modelview);
    }


    protected override void OnUpdateFrame(FrameEventArgs e)
    {
        base.OnUpdateFrame(e);

        if (_isSpinning)
        {
            _timeUntilNextCycle -= (float)e.Time;

            if (_timeUntilNextCycle <= 0)
            {
                _remainingCycles--;

                // 1. Schimbarea Simbolurilor (Random)
                int numSymbols = _symbolTextures.Length;
                _currentSlotIndex[0] = _random.Next(numSymbols);
                _currentSlotIndex[1] = _random.Next(numSymbols);
                _currentSlotIndex[2] = _random.Next(numSymbols);

                if (_remainingCycles <= 0)
                {
                    _isSpinning = false;

                    _currentSlotIndex[0] = _random.Next(numSymbols);
                    _currentSlotIndex[1] = _random.Next(numSymbols);
                    _currentSlotIndex[2] = _random.Next(numSymbols);

                    if (CheckWin())
                    {
                        Console.WriteLine(">>>> FELICITARI! AI CÂȘTIGAT! <<<<");
                    }
                    else
                    {
                        Console.WriteLine(">> AI PIERDUT. Mai încearcă. <<");
                    }
                }

                _timeUntilNextCycle += CYCLE_TIME;
            }
        }
    }

    protected override void OnMouseDown(MouseButtonEventArgs e)
    {
        base.OnMouseDown(e);

        if (e.Button == MouseButton.Left && !_isSpinning)
        {
            float centerX = Width / 2.0f;
            float machineCenterY = Height / 2.0f + 50f;
            float buttonY = machineCenterY - FRAME_HEIGHT / 2 - 50;

            float xMin = centerX - BUTTON_WIDTH / 2.0f;
            float xMax = centerX + BUTTON_WIDTH / 2.0f;
            float yMin = buttonY - BUTTON_HEIGHT / 2.0f;
            float yMax = buttonY + BUTTON_HEIGHT / 2.0f;
            float mouseY = Height - e.Y;

            if (e.X >= xMin && e.X <= xMax && mouseY >= yMin && mouseY <= yMax)
            {
                Console.WriteLine("--- TRAGERE INCEPUTA ---");
                _isSpinning = true;
                _remainingCycles = _cyclesToSpin;
                _timeUntilNextCycle = 0f;
            }
        }
    }

    private bool CheckWin()
    {
        return _currentSlotIndex[0] == _currentSlotIndex[1] && _currentSlotIndex[1] == _currentSlotIndex[2];
    }

    protected override void OnRenderFrame(FrameEventArgs e)
    {
        base.OnRenderFrame(e);
        GL.Clear(ClearBufferMask.ColorBufferBit);

        GL.MatrixMode(MatrixMode.Modelview);
        GL.LoadIdentity();

        float centerX = Width / 2.0f;
        float machineCenterY = Height / 2.0f + 50f;

        GL.Color3(1.0f, 1.0f, 1.0f);
        _texFrame.Use();
        DrawQuadImmediate(centerX, machineCenterY, FRAME_WIDTH, FRAME_HEIGHT);
        //slots
        _symbolTextures[_currentSlotIndex[0]].Use();
        DrawQuadImmediate(centerX - SLOT_SPACING, machineCenterY, SLOT_SIZE, SLOT_SIZE);

        _symbolTextures[_currentSlotIndex[1]].Use();
        DrawQuadImmediate(centerX, machineCenterY, SLOT_SIZE, SLOT_SIZE);

        _symbolTextures[_currentSlotIndex[2]].Use();
        DrawQuadImmediate(centerX + SLOT_SPACING, machineCenterY, SLOT_SIZE, SLOT_SIZE);

        GL.BindTexture(TextureTarget.Texture2D, 0);

        //draw button
        float buttonY = machineCenterY - FRAME_HEIGHT / 2 - 50;

        GL.Color3(1.0f, 1.0f, 1.0f);
        _texButton.Use();
        DrawQuadImmediate(centerX, buttonY, BUTTON_WIDTH, BUTTON_HEIGHT);

        GL.BindTexture(TextureTarget.Texture2D, 0);

        SwapBuffers();
    }

    private void DrawQuadImmediate(float x, float y, float w, float h)
    {
        GL.Enable(EnableCap.Texture2D);

        GL.PushMatrix();
        GL.Translate(x, y, 0.0f);
        GL.Scale(w, h, 1.0f);

        float halfW = 0.5f;
        float halfH = 0.5f;

        GL.Begin(PrimitiveType.Quads);
        GL.TexCoord2(1.0f, 1.0f); GL.Vertex2(halfW, halfH);
        GL.TexCoord2(1.0f, 0.0f); GL.Vertex2(halfW, -halfH);
        GL.TexCoord2(0.0f, 0.0f); GL.Vertex2(-halfW, -halfH);
        GL.TexCoord2(0.0f, 1.0f); GL.Vertex2(-halfW, halfH);

        GL.End();

        GL.PopMatrix();
    }

}