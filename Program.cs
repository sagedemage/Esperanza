using SDL2;
using System;
using System.Runtime.InteropServices;

class Program
{
    const int WINDOW_WIDTH = 640;
    const int WINDOW_HEIGHT = 480;
    const float PLAYER_SPEED = 1.5f;
    public static void Main(String[] args)
    {
        // Initialize SDL
        if (SDL.SDL_Init(SDL.SDL_INIT_VIDEO) < 0)
        {
            Console.WriteLine($"Unable to initialize SDL: {SDL.SDL_GetError()}");
        }

        // Create a new window
        nint win = SDL.SDL_CreateWindow("SDL C# Window", SDL.SDL_WINDOWPOS_UNDEFINED, SDL.SDL_WINDOWPOS_UNDEFINED, WINDOW_WIDTH, WINDOW_HEIGHT, SDL.SDL_WindowFlags.SDL_WINDOW_VULKAN);

        if (win == IntPtr.Zero)
        {
            Console.WriteLine($"Unable to create a window: {SDL.SDL_GetError()}");
        }

        // Create SDL hardware renderer using the default graphics device and enable VSync.
        nint rend = SDL.SDL_CreateRenderer(win, -1, SDL.SDL_RendererFlags.SDL_RENDERER_ACCELERATED | SDL.SDL_RendererFlags.SDL_RENDERER_PRESENTVSYNC);

        if (rend == IntPtr.Zero)
        {
            Console.WriteLine($"Unable to create the renderer: {SDL.SDL_GetError()}");
        }

        // Initialize SDL_image
        if (SDL_image.IMG_Init(SDL_image.IMG_InitFlags.IMG_INIT_PNG) == 0)
        {
            Console.WriteLine($"Unable to initialize SDL_image {SDL_image.IMG_GetError()}");
        }

        bool quit = false;

        SDL.SDL_FRect rect = new SDL.SDL_FRect
        {
            x = 300,
            y = 100,
            w = 25,
            h = 25
        };

        // Game loop
        while (!quit)
        {
            while (SDL.SDL_PollEvent(out SDL.SDL_Event e) == 1)
            {
                switch (e.type)
                {
                    case SDL.SDL_EventType.SDL_QUIT:
                        quit = true;
                        break;
                    case SDL.SDL_EventType.SDL_KEYDOWN:
                        switch (e.key.keysym.scancode)
                        {
                            case SDL.SDL_Scancode.SDL_SCANCODE_ESCAPE:
                                quit = true;
                                break;
                        }
                        break;
                }
            }

            // Hold keybindings
            if (getKey(SDL.SDL_Scancode.SDL_SCANCODE_A) == true)
            {
                rect.x -= PLAYER_SPEED;
            }
            else if (getKey(SDL.SDL_Scancode.SDL_SCANCODE_D) == true)
            {
                rect.x += PLAYER_SPEED;
            }
            else if (getKey(SDL.SDL_Scancode.SDL_SCANCODE_W) == true)
            {
                rect.y -= PLAYER_SPEED;
            }
            else if (getKey(SDL.SDL_Scancode.SDL_SCANCODE_S) == true)
            {
                rect.y += PLAYER_SPEED;
            }

            playerBoundary(ref rect);

            // Set the color of the screen it will be cleared with
            SDL.SDL_SetRenderDrawColor(rend, 135, 206, 235, 255);

            // Clears the current render surface
            SDL.SDL_RenderClear(rend);

            // Set the color of the shape before drawing it
            SDL.SDL_SetRenderDrawColor(rend, 150, 0, 255, 255);

            // Draw a filled in rectangle
            SDL.SDL_RenderFillRectF(rend, ref rect);

            // Switches out the currently presented render surface with he one we just did work on
            SDL.SDL_RenderPresent(rend);
        }

        // Clean up the resources we created
        SDL.SDL_DestroyRenderer(rend);
        SDL.SDL_DestroyWindow(win);
        SDL.SDL_Quit();
    }

    static void playerBoundary(ref SDL.SDL_FRect rect)
    {
        // Player Boundaries
        if (rect.x < 0)
        {
            // left boundary
            rect.x = 0;
        }
        if (rect.x + rect.w > WINDOW_WIDTH)
        {
            // right boundary
            rect.x = WINDOW_WIDTH - rect.w;
        }
        if (rect.y + rect.h > WINDOW_HEIGHT)
        {
            // bottom boundary
            rect.y = WINDOW_HEIGHT - rect.h;
        }
        if (rect.y < 0)
        {
            // top boundary
            rect.y = 0;
        }
    }

    static bool getKey(SDL.SDL_Scancode scancode)
    {
        /*
        Copy the integer pointer of the array of key states from SDL.SDL_GetKeyboardState
        to the keys byte array called keys.
        */
        int array_size;

        // Return value of the SDL_GetKeyboardState() function:
        // (const Uint8 *) Returns a pointer to an array of key states.
        nint orig_array = SDL.SDL_GetKeyboardState(out array_size);
        byte[] keys = new byte[array_size];
        Marshal.Copy(orig_array, keys, 0, array_size);

        // Check if the key is pressed
        byte scancode_byte = (byte)scancode;
        bool is_key_pressed = keys[scancode_byte] == 1;

        return is_key_pressed;
    }
}