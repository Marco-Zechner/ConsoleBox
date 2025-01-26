# ConsoleBox (Or maybe Termux.Net)
This library provides the framework so you can show different output seperated in individual panels.
It works somewhat similar to a Terminal multiplexer, but I am not sure if it is the correct term to use.

## Freaturs

You can create multiple panels and then arange them vertically or horizontally.
And then inside of those split panels you can do the same again, or show content (text).
Currently it has a own reader thread that starts in the static constructor of the ConsoleManager class.

If you want to read input you need to do so in the MAIN thread, other threads will not be able to read input.

If you use the provided ConsoleBuffer to write to the console instead of Console.Write then it will be more efficient,
because it will first get all that needs to be written, compare it to the current buffer and then only write the difference.

This library is very much a work in progress and I started it only for personal use, but feel free to use it if you want.

## Example

look in "test" folder. there is a console project that I use for testing the different features.