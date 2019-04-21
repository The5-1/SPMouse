# Goal
Make the mouse cursor move and click in a different location.  
E.g. a smoothed curve or a dragging it along a "rope".

# Approach
Intercept low level input events and re-position cursor before any input registers in any application.

## Get the event

For this use PInvoke to call the native Win32 API, e.g:  
http://www.pinvoke.net/default.aspx/user32/SetWindowsHookEx.html

```CSharp
return SetWindowsHookEx(
    WH_MOUSE_LL, //Hook Location ID
    proc, //a callback to register
    GetModuleHandle(Process.GetCurrentProcess().MainModule.ModuleName), //in what dll the callback can be found, so the current one
    0 //the thread where the procedure shall be called, 0 for all threads
    );
```

```CSharp
private IntPtr interceptInput(int nCode, IntPtr wParam, IntPtr lParam)
{
    if (nCode == Win32Util.HC_ACTION)
    {
        Win32Util.MSLLHOOKSTRUCT hookStruct = (Win32Util.MSLLHOOKSTRUCT)Marshal.PtrToStructure(lParam, typeof(Win32Util.MSLLHOOKSTRUCT));
    
        //...
    }

    //Recommended by the documentation, pass the event along for other hooks to process it.
    return Win32Util.CallNextHookEx(m_nativeHookPtr, nCode, wParam, lParam); 
}
```
## Modify it

But modifying the `MSLLHOOKSTRUCT` did not work.  
Neither with `Marshal` back to the pointer nor within `unsafe` directly modifying it.

```CSharp
hookStruct.pt.x = 960;
Marshal.StructureToPtr(hookStruct, lParam, true);
```

```CSharp
unsafe
{
    Win32Util.MSLLHOOKSTRUCT* data = (Win32Util.MSLLHOOKSTRUCT*)lParam.ToPointer();
    data->pt.x = 960;
}
```

All results Google brought up "confirmed" this. As in, nobody who asked found an answer how to  modify the `MSLLHOOKSTRUCT` before passing it along to `CallNextHookEx`.

## Drop it

The frequently suggested alternate approach is:  
* receive the input event
* apply your logic based on it
* **drop** the actual event
* emit a new one

When simply setting a new cursor position this works quite well!

```CSharp
if (/*condition*/)
{
    //set the cursor position, this works quite well!
    Win32Util.SetCursorPos(ropeLogic.cursorPoint.X, ropeLogic.cursorPoint.Y);
    return (IntPtr)1; //drop the event
}
```
But this did not allow to "draw" elsewhere since the `mouse_down` part of the event is lost.

## Re-emit a new one

Emitting an actual event via `mouse_event()`or `SendInput()` and thus potentially reacting to your own events smells like potential infinite loops.  
(If it happens, **restart** the PC. `taskkill /F` and even shutdown didn't help.)  

```CSharp
if (/*your logics conditions*/)
{
    Win32Util.mouse_event((uint)Win32Util.MouseEventFlags.LEFTDOWN, ropeLogic.cursorPoint.X, ropeLogic.cursorPoint.Y, 0, 0);
    return (IntPtr)1; //drop the event
}
```
## Check if the event is your own and do not process it

You can check a flag inside `MSLLHOOKSTRUCT` if its a "injected" event.  
https://stackoverflow.com/questions/21928956/how-do-i-modify-keys-in-a-keyboardproc-hooking-procedure  


```CSharp
if(hookStruct.flags == (uint)Win32Util.LLHookFLags.LLMHF_INJECTED || hookStruct.flags == (uint)Win32Util.LLHookFLags.LLMHF_LOWER_IL_INJECTED)
{
    return Win32Util.CallNextHookEx(m_nativeHookPtr, nCode, wParam, lParam);
}
```

This did not help in my case, though it should in theory...

