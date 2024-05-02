﻿using System.Drawing;
using FlaUI.Core.Input;
using FlaUI.Core.WindowsAPI;

namespace FlaUI.WebDriver.Services
{
    public class ActionsDispatcher
    {
        public static async Task DispatchAction(Session session, Action action)
        {
            switch (action.Type)
            {
                case "pointer":
                    await DispatchPointerAction(session, action);
                    return;
                case "key":
                    await DispatchKeyAction(session, action);
                    return;
                case "wheel":
                    await DispatchWheelAction(session, action);
                    return;
                case "none":
                    await DispatchNullAction(session, action);
                    return;
                default:
                    throw WebDriverResponseException.UnsupportedOperation($"Action type {action.Type} not supported");
            }
        }

        private static async Task DispatchNullAction(Session session, Action action)
        {
            switch (action.SubType)
            {
                case "pause":
                    await Task.Yield();
                    return;
                default:
                    throw WebDriverResponseException.InvalidArgument($"Null action subtype {action.SubType} unknown");
            }
        }

        private static async Task DispatchKeyAction(Session session, Action action)
        {
            switch (action.SubType)
            {
                case "keyDown":
                    var keyToPress = GetKey(action.Value);
                    Keyboard.Press(keyToPress);
                    var cancelAction = action.Clone();
                    cancelAction.SubType = "keyUp";
                    session.InputState.InputCancelList.Add(cancelAction);
                    await Task.Yield();
                    return;
                case "keyUp":
                    var keyToRelease = GetKey(action.Value);
                    Keyboard.Release(keyToRelease);
                    await Task.Yield();
                    return;
                case "pause":
                    await Task.Yield();
                    return;
                default:
                    throw WebDriverResponseException.InvalidArgument($"Pointer action subtype {action.SubType} unknown");
            }
        }

        private static async Task DispatchWheelAction(Session session, Action action)
        {
            switch (action.SubType)
            {
                case "scroll":
                    if (action.X == null || action.Y == null)
                    {
                        throw WebDriverResponseException.InvalidArgument("For wheel scroll, X and Y are required");
                    }
                    Mouse.MoveTo(action.X.Value, action.Y.Value);
                    if (action.DeltaX == null || action.DeltaY == null)
                    {
                        throw WebDriverResponseException.InvalidArgument("For wheel scroll, delta X and delta Y are required");
                    }
                    if (action.DeltaY != 0)
                    {
                        Mouse.Scroll(action.DeltaY.Value);
                    }
                    if (action.DeltaX != 0)
                    {
                        Mouse.HorizontalScroll(action.DeltaX.Value);
                    }
                    return;
                case "pause":
                    await Task.Yield();
                    return;
                default:
                    throw WebDriverResponseException.InvalidArgument($"Wheel action subtype {action.SubType} unknown");
            }
        }

        private static VirtualKeyShort GetKey(string? value)
        {
            if (value == null || value.Length != 1)
            {
                throw WebDriverResponseException.InvalidArgument($"Key action value argument should be exactly one character");
            }
            switch (value[0])
            {
                case '\uE001': return VirtualKeyShort.CANCEL;
                case '\uE002': return VirtualKeyShort.HELP;
                case '\uE003': return VirtualKeyShort.BACK;
                case '\uE004': return VirtualKeyShort.TAB;
                case '\uE005': return VirtualKeyShort.CLEAR;
                case '\uE006': return VirtualKeyShort.RETURN;
                case '\uE007': return VirtualKeyShort.ENTER;
                case '\uE008': return VirtualKeyShort.LSHIFT;
                case '\uE009': return VirtualKeyShort.LCONTROL;
                case '\uE00A': return VirtualKeyShort.ALT;
                case '\uE00B': return VirtualKeyShort.PAUSE;
                case '\uE00C': return VirtualKeyShort.ESCAPE;
                case '\uE00D': return VirtualKeyShort.SPACE;
                case '\uE00E': return VirtualKeyShort.PRIOR;
                case '\uE00F': return VirtualKeyShort.NEXT;
                case '\uE010': return VirtualKeyShort.END;
                case '\uE011': return VirtualKeyShort.HOME;
                case '\uE012': return VirtualKeyShort.LEFT;
                case '\uE013': return VirtualKeyShort.UP;
                case '\uE014': return VirtualKeyShort.RIGHT;
                case '\uE015': return VirtualKeyShort.DOWN;
                case '\uE016': return VirtualKeyShort.INSERT;
                case '\uE017': return VirtualKeyShort.DELETE;
                // case '\uE018': ";"
                // case '\uE019': "="
                case '\uE01A': return VirtualKeyShort.NUMPAD0;
                case '\uE01B': return VirtualKeyShort.NUMPAD1;
                case '\uE01C': return VirtualKeyShort.NUMPAD2;
                case '\uE01D': return VirtualKeyShort.NUMPAD3;
                case '\uE01E': return VirtualKeyShort.NUMPAD4;
                case '\uE01F': return VirtualKeyShort.NUMPAD5;
                case '\uE020': return VirtualKeyShort.NUMPAD6;
                case '\uE021': return VirtualKeyShort.NUMPAD7;
                case '\uE022': return VirtualKeyShort.NUMPAD8;
                case '\uE023': return VirtualKeyShort.NUMPAD9;
                case '\uE024': return VirtualKeyShort.ADD;
                case '\uE025': return VirtualKeyShort.MULTIPLY;
                case '\uE026': return VirtualKeyShort.SEPARATOR;
                case '\uE027': return VirtualKeyShort.SUBTRACT;
                case '\uE028': return VirtualKeyShort.DECIMAL;
                case '\uE029': return VirtualKeyShort.DIVIDE;
                case '\uE031': return VirtualKeyShort.F1;
                case '\uE032': return VirtualKeyShort.F2;
                case '\uE033': return VirtualKeyShort.F3;
                case '\uE034': return VirtualKeyShort.F4;
                case '\uE035': return VirtualKeyShort.F5;
                case '\uE036': return VirtualKeyShort.F6;
                case '\uE037': return VirtualKeyShort.F7;
                case '\uE038': return VirtualKeyShort.F8;
                case '\uE039': return VirtualKeyShort.F9;
                case '\uE03A': return VirtualKeyShort.F10;
                case '\uE03B': return VirtualKeyShort.F11;
                case '\uE03C': return VirtualKeyShort.F12;
                // case '\uE03D': "Meta"
                // case '\uE040': "ZenkakuHankaku"
                case '\uE050': return VirtualKeyShort.RSHIFT;
                case '\uE051': return VirtualKeyShort.RCONTROL;
                case '\uE052': return VirtualKeyShort.ALT;
                // case '\uE053': "Meta"
                case '\uE054': return VirtualKeyShort.PRIOR;
                case '\uE055': return VirtualKeyShort.NEXT;
                case '\uE056': return VirtualKeyShort.END;
                case '\uE057': return VirtualKeyShort.HOME;
                case '\uE058': return VirtualKeyShort.LEFT;
                case '\uE059': return VirtualKeyShort.UP;
                case '\uE05A': return VirtualKeyShort.RIGHT;
                case '\uE05B': return VirtualKeyShort.DOWN;
                case '\uE05C': return VirtualKeyShort.INSERT;
                case '\uE05D': return VirtualKeyShort.DELETE;
                case 'a': return VirtualKeyShort.KEY_A;
                case 'b': return VirtualKeyShort.KEY_B;
                case 'c': return VirtualKeyShort.KEY_C;
                case 'd': return VirtualKeyShort.KEY_D;
                case 'e': return VirtualKeyShort.KEY_E;
                case 'f': return VirtualKeyShort.KEY_F;
                case 'g': return VirtualKeyShort.KEY_G;
                case 'h': return VirtualKeyShort.KEY_H;
                case 'i': return VirtualKeyShort.KEY_I;
                case 'j': return VirtualKeyShort.KEY_J;
                case 'k': return VirtualKeyShort.KEY_K;
                case 'l': return VirtualKeyShort.KEY_L;
                case 'm': return VirtualKeyShort.KEY_M;
                case 'n': return VirtualKeyShort.KEY_N;
                case 'o': return VirtualKeyShort.KEY_O;
                case 'p': return VirtualKeyShort.KEY_P;
                case 'q': return VirtualKeyShort.KEY_Q;
                case 'r': return VirtualKeyShort.KEY_R;
                case 's': return VirtualKeyShort.KEY_S;
                case 't': return VirtualKeyShort.KEY_T;
                case 'u': return VirtualKeyShort.KEY_U;
                case 'v': return VirtualKeyShort.KEY_V;
                case 'w': return VirtualKeyShort.KEY_W;
                case 'x': return VirtualKeyShort.KEY_X;
                case 'y': return VirtualKeyShort.KEY_Y;
                case 'z': return VirtualKeyShort.KEY_Z;
                default: throw WebDriverResponseException.UnsupportedOperation($"Key {value} is not supported");
            }
        }

        private static async Task DispatchPointerAction(Session session, Action action)
        {
            switch (action.SubType)
            {
                case "pointerMove":
                    var point = GetCoordinates(session, action);
                    Mouse.MoveTo(point);
                    await Task.Yield();
                    return;
                case "pointerDown":
                    Mouse.Down(GetMouseButton(action.Button));
                    var cancelAction = action.Clone();
                    cancelAction.SubType = "pointerUp";
                    session.InputState.InputCancelList.Add(cancelAction);
                    await Task.Yield();
                    return;
                case "pointerUp":
                    Mouse.Up(GetMouseButton(action.Button));
                    await Task.Yield();
                    return;
                case "pause":
                    await Task.Yield();
                    return;
                default:
                    throw WebDriverResponseException.UnsupportedOperation($"Pointer action subtype {action.Type} not supported");
            }
        }

        private static Point GetCoordinates(Session session, Action action)
        {
            var origin = action.Origin ?? "viewport";

            switch (origin)
            {
                case "viewport":
                    if (action.X == null || action.Y == null)
                    {
                        throw WebDriverResponseException.InvalidArgument("For pointer move, X and Y are required");
                    }

                    return new Point(action.X.Value, action.Y.Value);
                case "pointer":
                    if (action.X == null || action.Y == null)
                    {
                        throw WebDriverResponseException.InvalidArgument("For pointer move, X and Y are required");
                    }

                    var current = Mouse.Position;
                    return new Point(current.X + action.X.Value, current.Y + action.Y.Value);
                case Dictionary<string, string> originMap:
                    if (originMap.TryGetValue("element-6066-11e4-a52e-4f735466cecf", out var elementId))
                    {
                        if (session.FindKnownElementById(elementId) is { } element)
                        {
                            var bounds = element.BoundingRectangle;
                            var x = bounds.Left + (bounds.Width / 2) + (action.X ?? 0);
                            var y = bounds.Top + (bounds.Height / 2) + (action.Y ?? 0);
                            return new(x, y);
                        }

                        throw WebDriverResponseException.InvalidArgument(
                            $"An unknown element ID '{elementId}' provided for action item '{action.Type}'.");
                    }

                    throw WebDriverResponseException.InvalidArgument(
                        $"An unknown element '{origin}' provided for action item '{action.Type}'.");
                default:
                    throw WebDriverResponseException.InvalidArgument(
                        $"Unknown origin type '{origin}' provided for action item '{action.Type}'.");
            }
        }

        private static MouseButton GetMouseButton(int? button)
        {
            if (button == null)
            {
                throw WebDriverResponseException.InvalidArgument($"Pointer action button argument missing");
            }
            switch (button)
            {
                case 0: return MouseButton.Left;
                case 1: return MouseButton.Middle;
                case 2: return MouseButton.Right;
                case 3: return MouseButton.XButton1;
                case 4: return MouseButton.XButton2;
                default:
                    throw WebDriverResponseException.UnsupportedOperation($"Pointer button {button} not supported");
            }
        }
    }
}
